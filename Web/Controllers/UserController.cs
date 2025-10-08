using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;

namespace Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUserMenuPermissionRepository _userMenuPermissionRepository;

        public UserController(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMenuRepository menuRepository,
            IPermissionRepository permissionRepository,
            IUserRoleRepository userRoleRepository,
            IUserMenuRepository userMenuRepository,
            IUserMenuPermissionRepository userMenuPermissionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _menuRepository = menuRepository;
            _permissionRepository = permissionRepository;
            _userRoleRepository = userRoleRepository;
            _userMenuRepository = userMenuRepository;
            _userMenuPermissionRepository = userMenuPermissionRepository;
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
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return Json(new { success = false, error = "Kullanıcı bulunamadı" });

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Password = HashPassword(userDto.Password),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    IsActive = userDto.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _userRepository.AddAsync(user);

                // Rolleri ata
                if (userDto.RoleIds.Any())
                {
                    var currentUserId = GetCurrentUserId();
                    foreach (var roleId in userDto.RoleIds)
                    {
                        await _userRepository.AssignRoleToUserAsync(user.Id, roleId, currentUserId);
                    }
                }

                return Json(new { success = true, message = "Kullanıcı başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto userDto)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });

                existingUser.Username = userDto.Username;
                existingUser.Email = userDto.Email;
                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.IsActive = userDto.IsActive;

                await _userRepository.UpdateAsync(existingUser);

                // Rolleri güncelle
                await UpdateUserRoles(userDto.Id, userDto.RoleIds);

                return Json(new { success = true, message = "Kullanıcı başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!await HasPermissionAsync("DELETE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                await _userRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Kullanıcı-Rol İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                return Json(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _userRepository.AssignRoleToUserAsync(request.UserId, request.RoleId, currentUserId, request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Rol başarıyla atandı." });
                else
                    return Json(new { success = false, message = "Rol zaten atanmış veya atanamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _userRepository.RemoveRoleFromUserAsync(request.UserId, request.RoleId);
                
                if (success)
                    return Json(new { success = true, message = "Rol başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Rol kaldırılamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRolesForUser(int userId, string? search = null)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                // Get all roles
                var allRoles = await _roleRepository.GetAllAsync();
                
                // Get user's current roles
                var userRoles = await _userRepository.GetUserRolesAsync(userId);
                var userRoleIds = userRoles.Select(r => r.Id).ToList();
                
                // Filter out assigned roles
                var availableRoles = allRoles.Where(r => !userRoleIds.Contains(r.Id)).ToList();
                
                // Apply search filter if provided
                if (!string.IsNullOrEmpty(search))
                {
                    availableRoles = availableRoles.Where(r => 
                        r.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (r.Description != null && r.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }
                
                return Json(new { success = true, data = availableRoles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Kullanıcı-Menü İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetUserMenus(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _userRepository.GetUserMenusAsync(userId);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignMenuToUser([FromBody] AssignMenuRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _userRepository.AssignMenuToUserAsync(request.UserId, request.MenuId, currentUserId, request.ExpiryDate, request.Notes);
                
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
        public async Task<IActionResult> RemoveMenuFromUser([FromBody] RemoveMenuRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _userRepository.RemoveMenuFromUserAsync(request.UserId, request.MenuId);
                
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

        #region Kullanıcı-Menü-İzin İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetUserMenuPermissions(int userId)
        {
            try
            {
                var permissions = await _userMenuPermissionRepository.GetUserMenuPermissionsAsync(userId);
                
                // DTO'ya dönüştür - Circular reference sorununu önle
                var permissionDtos = permissions.Select(p => new
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    MenuId = p.MenuId,
                    PermissionId = p.PermissionId,
                    PermissionLevel = p.PermissionLevel,
                    Notes = p.Notes,
                    AssignedDate = p.AssignedDate,
                    ExpiryDate = p.ExpiryDate,
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
        public async Task<IActionResult> AssignMenuPermissionToUser([FromBody] AssignMenuPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _userRepository.AssignMenuPermissionToUserAsync(
                    request.UserId, 
                    request.MenuId, 
                    request.PermissionId, 
                    request.PermissionLevel, 
                    currentUserId, 
                    request.ExpiryDate, 
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
        public async Task<IActionResult> RemoveMenuPermissionFromUser([FromBody] RemoveMenuPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _userRepository.RemoveMenuPermissionFromUserAsync(request.UserId, request.MenuId, request.PermissionId);
                
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
        public async Task<IActionResult> RemoveAllMenuPermissions([FromBody] RemoveAllMenuPermissionsRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                // Kullanıcının bu menüye ait tüm izinlerini bul ve kaldır
                var permissions = await _userMenuPermissionRepository.GetUserMenuPermissionsByMenuAsync(request.UserId, request.MenuId);
                
                int removedCount = 0;
                foreach (var permission in permissions)
                {
                    var removed = await _userRepository.RemoveMenuPermissionFromUserAsync(request.UserId, request.MenuId, permission.PermissionId);
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

        #region Kullanıcı Detayları ve İstatistikler

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var user = await _userRepository.GetUserWithRolesAsync(userId);
                if (user == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });

                var userMenus = await _userRepository.GetUserMenusAsync(userId);
                var userPermissions = await _userRepository.GetUserMenuPermissionsAsync(userId);

                var result = new
                {
                    User = user,
                    Menus = userMenus,
                    Permissions = userPermissions,
                    RoleCount = await _userRepository.GetUserRoleCountAsync(userId),
                    MenuCount = await _userRepository.GetUserMenuCountAsync(userId)
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserStatistics()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var totalUsers = await _userRepository.GetUserCountAsync();
                var activeUsers = await _userRepository.GetActiveUserCountAsync();

                var result = new
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    InactiveUsers = totalUsers - activeUsers
                };

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Permission İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var permissions = await _permissionRepository.GetAllAsync();
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableMenusAndPermissionsForUser(int userId)
        {
            try
            {
                // Tüm menüleri al
                var allMenus = await _menuRepository.GetAllAsync();
                
                // Kullanıcının rollerini al
                var userRoles = await _userRepository.GetUserRolesAsync(userId);
                
                // Kullanıcının direkt menü yetkilerini al
                var userMenuPermissions = await _userMenuPermissionRepository.GetUserMenuPermissionsAsync(userId);
                
                // Kullanıcının direkt sahip olduğu menü ID'leri (bunları gösterme!)
                var assignedMenuIds = userMenuPermissions.Select(ump => ump.MenuId).Distinct().ToHashSet();
                
                // Rollerin sahip olduğu menü-yetki kombinasyonları
                var roleMenuPermissions = new Dictionary<int, HashSet<string>>();
                
                foreach (var role in userRoles)
                {
                    var rolePerms = await _roleRepository.GetRoleMenuPermissionsAsync(role.Id);
                    foreach (var rmp in rolePerms)
                    {
                        if (!roleMenuPermissions.ContainsKey(rmp.MenuId))
                        {
                            roleMenuPermissions[rmp.MenuId] = new HashSet<string>();
                        }
                        roleMenuPermissions[rmp.MenuId].Add(rmp.PermissionLevel);
                    }
                }
                
                // Tüm permission'ları al
                var allPermissions = await _permissionRepository.GetAllAsync();
                
                // Her menü için kullanılabilir menü ve permission'ları hesapla
                var availableData = allMenus
                    .Where(menu => !assignedMenuIds.Contains(menu.Id)) // ZATEN ATANMIŞ MENÜLERI GÖSTERME!
                    .Select(menu => 
                    {
                        var hasRoleAccess = roleMenuPermissions.ContainsKey(menu.Id);
                        
                        // Bu menüye rollerden gelen permission'lar
                        var rolePermissionLevels = hasRoleAccess ? roleMenuPermissions[menu.Id] : new HashSet<string>();
                        
                        // Tüm permission'ları kontrol et, sadece eksik olanları göster
                        var availablePermissions = allPermissions.Select(perm => 
                        {
                            var permCode = perm.Code ?? perm.Name;
                            var hasFromRole = rolePermissionLevels.Contains(permCode);
                            
                            return new
                            {
                                Id = perm.Id,
                                Name = perm.Name,
                                Code = permCode,
                                Description = perm.Description,
                                IsAvailable = !hasFromRole,
                                HasFromRole = hasFromRole
                            };
                        }).Where(p => p.IsAvailable).ToList();
                        
                        return new
                        {
                            menuId = menu.Id,
                            menuName = menu.Name,
                            menuIcon = menu.Icon,
                            menuController = menu.Controller,
                            menuAction = menu.Action,
                            hasAnyRolePermission = hasRoleAccess,
                            availablePermissions = availablePermissions,
                            isFullyCoveredByRole = availablePermissions.Count == 0 && hasRoleAccess
                        };
                    })
                    .Where(m => !m.isFullyCoveredByRole) // Tam olarak rolden karşılananları da gösterme
                    .ToList();
                
                return Json(new { success = true, data = availableData });
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
            var menuId = await ResolveMenuIdAsync("User", "Index");
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

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private async Task UpdateUserRoles(int userId, List<int> roleIds)
        {
            var currentUserId = GetCurrentUserId();
            
            // Mevcut rolleri al
            var currentRoles = await _userRepository.GetUserRolesAsync(userId);
            var currentRoleIds = currentRoles.Select(r => r.Id).ToList();

            // Kaldırılacak rolleri bul ve kaldır
            var rolesToRemove = currentRoleIds.Except(roleIds).ToList();
            foreach (var roleId in rolesToRemove)
            {
                await _userRepository.RemoveRoleFromUserAsync(userId, roleId);
            }

            // Eklenmesi gereken rolleri bul ve ekle
            var rolesToAdd = roleIds.Except(currentRoleIds).ToList();
            foreach (var roleId in rolesToAdd)
            {
                await _userRepository.AssignRoleToUserAsync(userId, roleId, currentUserId);
            }
        }

        #endregion
    }

    #region Request Models

    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? Notes { get; set; }
    }

    public class RemoveRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class AssignMenuRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class RemoveMenuRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
    }

    public class AssignMenuPermissionRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
        public string PermissionLevel { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class RemoveMenuPermissionRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
    }

    public class RemoveAllMenuPermissionsRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
    }

    #endregion
}
