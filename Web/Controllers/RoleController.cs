using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;

namespace Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleMenuRepository _roleMenuRepository;
        private readonly IRoleMenuPermissionRepository _roleMenuPermissionRepository;

        public RoleController(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IMenuRepository menuRepository,
            IPermissionRepository permissionRepository,
            IUserRoleRepository userRoleRepository,
            IRoleMenuRepository roleMenuRepository,
            IRoleMenuPermissionRepository roleMenuPermissionRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _menuRepository = menuRepository;
            _permissionRepository = permissionRepository;
            _userRoleRepository = userRoleRepository;
            _roleMenuRepository = roleMenuRepository;
            _roleMenuPermissionRepository = roleMenuPermissionRepository;
        }

        #region Ana Sayfalar

        public IActionResult Index()
        {
            // Geçici olarak yetki kontrolünü kaldırdık
            // if (!await HasPermissionAsync("VIEW"))
            //     return Forbid();

            return View();
        }

        #endregion

        #region DevExtreme DataGrid API Methods

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                return Json(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                    return Json(new { success = false, error = "Rol bulunamadı" });

                return Json(new { success = true, data = role });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto roleDto)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var role = new Role
                {
                    Name = roleDto.Name,
                    Description = roleDto.Description,
                    IsActive = roleDto.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _roleRepository.AddAsync(role);

                // Menüleri ata
                if (roleDto.MenuIds.Any())
                {
                    var currentUserId = GetCurrentUserId();
                    foreach (var menuId in roleDto.MenuIds)
                    {
                        await _roleRepository.AssignMenuToRoleAsync(role.Id, menuId, currentUserId);
                    }
                }

                return Json(new { success = true, message = "Rol başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto roleDto)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingRole = await _roleRepository.GetByIdAsync(roleDto.Id);
                if (existingRole == null)
                    return Json(new { success = false, message = "Rol bulunamadı." });

                existingRole.Name = roleDto.Name;
                existingRole.Description = roleDto.Description;
                existingRole.IsActive = roleDto.IsActive;
                existingRole.UpdatedDate = DateTime.Now;

                await _roleRepository.UpdateAsync(existingRole);

                return Json(new { success = true, message = "Rol başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (!await HasPermissionAsync("DELETE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingRole = await _roleRepository.GetByIdAsync(id);
                if (existingRole == null)
                    return Json(new { success = false, message = "Rol bulunamadı." });

                // Soft delete - sadece IsActive'i false yap
                existingRole.IsActive = false;
                existingRole.UpdatedDate = DateTime.Now;

                await _roleRepository.UpdateAsync(existingRole);
                return Json(new { success = true, message = "Rol başarıyla pasif hale getirildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Rol-Menü İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetRoleMenus(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _roleRepository.GetRoleMenusAsync(roleId);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignMenuToRole([FromBody] AssignMenuToRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _roleRepository.AssignMenuToRoleAsync(request.RoleId, request.MenuId, currentUserId, request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Menü başarıyla atandı." });
                else
                    return Json(new { success = false, message = "Menü zaten atanmış veya atanamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMenuFromRole([FromBody] RemoveMenuFromRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _roleRepository.RemoveMenuFromRoleAsync(request.RoleId, request.MenuId);
                
                if (success)
                    return Json(new { success = true, message = "Menü başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Menü kaldırılamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Rol-Menü-İzin İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetRoleMenuPermissions(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _roleRepository.GetRoleMenuPermissionsAsync(roleId);
                
                // DTO'ya dönüştür - Circular reference sorununu önle ve duplicate'ları kaldır
                var permissionDtos = permissions
                    .GroupBy(p => p.Id) // Duplicate'ları kaldır
                    .Select(g => g.First()) // İlk kaydı al
                    .Select(p => new
                    {
                        Id = p.Id,
                        RoleId = p.RoleId,
                        MenuId = p.MenuId,
                        PermissionId = p.PermissionId,
                        PermissionLevel = p.PermissionLevel,
                        Notes = p.Notes,
                        AssignedDate = p.AssignedDate,
                        IsActive = p.IsActive,
                        // Menu bilgileri
                        MenuName = p.Menu?.Name,
                        MenuIcon = p.Menu?.Icon,
                        MenuController = p.Menu?.Controller,
                        MenuAction = p.Menu?.Action,
                        // Permission bilgileri
                        PermissionName = p.Permission?.Name,
                        PermissionCode = p.Permission?.Code,
                        PermissionDescription = p.Permission?.Description
                    }).ToList();
                
                return Json(new { success = true, data = permissionDtos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignMenuPermissionToRole([FromBody] AssignMenuPermissionToRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _roleRepository.AssignMenuPermissionToRoleAsync(
                    request.RoleId, 
                    request.MenuId, 
                    request.PermissionId, 
                    request.PermissionLevel, 
                    currentUserId, 
                    request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Menü izni başarıyla atandı." });
                else
                    return Json(new { success = false, message = "Menü izni zaten atanmış veya atanamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMenuPermissionFromRole([FromBody] RemoveMenuPermissionFromRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _roleRepository.RemoveMenuPermissionFromRoleAsync(request.RoleId, request.MenuId, request.PermissionId);
                
                if (success)
                    return Json(new { success = true, message = "Menü izni başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Menü izni kaldırılamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoleMenuPermission([FromBody] UpdateRoleMenuPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _roleRepository.UpdateRoleMenuPermissionAsync(
                    request.RoleId, 
                    request.MenuId, 
                    request.PermissionId, 
                    request.NewPermissionLevel, 
                    currentUserId, 
                    request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Menü izni başarıyla güncellendi." });
                else
                    return Json(new { success = false, message = "Menü izni güncellenemedi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Rol Kullanıcıları

        [HttpGet]
        public async Task<IActionResult> GetRoleUsers(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var users = await _roleRepository.GetRoleUsersAsync(roleId);
                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleUserCount(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var count = await _roleRepository.GetRoleUserCountAsync(roleId);
                return Json(new { success = true, data = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Rol Detayları ve İstatistikler

        [HttpGet]
        public async Task<IActionResult> GetRoleDetails(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var role = await _roleRepository.GetRoleWithMenusAsync(roleId);
                if (role == null)
                    return Json(new { success = false, message = "Rol bulunamadı." });

                var roleMenus = await _roleRepository.GetRoleMenusAsync(roleId);
                var rolePermissions = await _roleRepository.GetRoleMenuPermissionsAsync(roleId);
                var roleUsers = await _roleRepository.GetRoleUsersAsync(roleId);

                var result = new
                {
                    Role = role,
                    Menus = roleMenus,
                    Permissions = rolePermissions,
                    Users = roleUsers,
                    MenuCount = await _roleRepository.GetRoleMenuCountAsync(roleId),
                    PermissionCount = await _roleRepository.GetRolePermissionCountAsync(roleId),
                    UserCount = await _roleRepository.GetRoleUserCountAsync(roleId)
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleStatistics()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var totalRoles = await _roleRepository.GetRoleCountAsync();
                var activeRoles = await _roleRepository.GetActiveRoleCountAsync();

                var result = new
                {
                    TotalRoles = totalRoles,
                    ActiveRoles = activeRoles,
                    InactiveRoles = totalRoles - activeRoles
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Available Menus and Permissions for Role

        [HttpGet]
        public async Task<IActionResult> GetAvailableMenusForRolePermission(int roleId, string? search = null)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                // Tüm menüleri al
                var allMenus = await _menuRepository.GetAllAsync();
                
                // Rolün mevcut menü yetkilerini al
                var roleMenuPermissions = await _roleRepository.GetRoleMenuPermissionsAsync(roleId);
                
                // Rolün direkt sahip olduğu menü ID'leri (bunları gösterme!)
                var assignedMenuIds = roleMenuPermissions.Select(rmp => rmp.MenuId).Distinct().ToHashSet();
                
                // Kullanılabilir menüleri filtrele ve duplicate'ları kaldır
                var availableMenus = allMenus
                    .Where(menu => !assignedMenuIds.Contains(menu.Id))
                    .GroupBy(menu => menu.Id) // Duplicate'ları kaldır
                    .Select(g => g.First()) // İlk kaydı al
                    .Select(menu => new // DTO'ya dönüştür - Circular reference'ları önle
                    {
                        Id = menu.Id,
                        Name = menu.Name,
                        Icon = menu.Icon,
                        Controller = menu.Controller,
                        Action = menu.Action,
                        IsActive = menu.IsActive,
                        CreatedDate = menu.CreatedDate
                        // Navigation property'leri dahil etme!
                    })
                    .ToList();
                
                // Apply search filter if provided
                if (!string.IsNullOrEmpty(search))
                {
                    availableMenus = availableMenus.Where(menu => 
                        menu.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (menu.Controller != null && menu.Controller.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (menu.Action != null && menu.Action.Contains(search, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }
                
                return Json(new { success = true, data = availableMenus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAllMenuPermissionsFromRole([FromBody] RemoveAllMenuPermissionsFromRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                // Rolün bu menüye ait tüm izinlerini bul ve kaldır
                var allPermissions = await _roleRepository.GetRoleMenuPermissionsAsync(request.RoleId);
                var permissions = allPermissions.Where(p => p.MenuId == request.MenuId).ToList();
                
                int removedCount = 0;
                foreach (var permission in permissions)
                {
                    var removed = await _roleRepository.RemoveMenuPermissionFromRoleAsync(request.RoleId, request.MenuId, permission.PermissionId);
                    if (removed) removedCount++;
                }
                
                if (removedCount > 0)
                    return Json(new { success = true, message = $"{removedCount} yetki başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Kaldırılacak yetki bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yardımcı Metodlar

        private async Task<bool> HasPermissionAsync(string permissionLevel)
        {
            var currentUserId = GetCurrentUserId();
            var menuId = await ResolveMenuIdAsync("Role", "Index");
            return await _userRepository.HasPermissionAsync(currentUserId, menuId, permissionLevel);
        }

        private async Task<int> ResolveMenuIdAsync(string controller, string action)
        {
            var menu = await _menuRepository.GetMenuByRouteAsync(controller, action);
            return menu?.Id ?? 0;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private async Task UpdateRoleMenus(int roleId, List<int> menuIds)
        {
            var currentUserId = GetCurrentUserId();
            
            // Mevcut menüleri al
            var currentMenus = await _roleRepository.GetRoleMenusAsync(roleId);
            var currentMenuIds = currentMenus.Select(m => m.Id).ToList();

            // Kaldırılacak menüleri bul ve kaldır
            var menusToRemove = currentMenuIds.Except(menuIds).ToList();
            foreach (var menuId in menusToRemove)
            {
                await _roleRepository.RemoveMenuFromRoleAsync(roleId, menuId);
            }

            // Eklenmesi gereken menüleri bul ve ekle
            var menusToAdd = menuIds.Except(currentMenuIds).ToList();
            foreach (var menuId in menusToAdd)
            {
                await _roleRepository.AssignMenuToRoleAsync(roleId, menuId, currentUserId);
            }
        }

        #endregion
    }

    #region Request Models

    public class AssignMenuToRoleRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public string? Notes { get; set; }
    }

    public class RemoveMenuFromRoleRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    }

    public class AssignMenuPermissionToRoleRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
        public string PermissionLevel { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class RemoveMenuPermissionFromRoleRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
    }

    public class UpdateRoleMenuPermissionRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
        public string NewPermissionLevel { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class RemoveAllMenuPermissionsFromRoleRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    }

    #endregion
}
