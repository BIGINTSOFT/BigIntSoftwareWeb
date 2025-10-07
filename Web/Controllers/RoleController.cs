using Bussiness.Repository.Abstract;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMenuRepository _menuRepository;

        public RoleController(IRoleRepository roleRepository, IUserRepository userRepository, IMenuRepository menuRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _menuRepository = menuRepository;
        }

        #region Ana Sayfalar

        public async Task<IActionResult> Index()
        {
            // Geçici olarak yetki kontrolünü kaldırdık
            // if (!await HasPermissionAsync("VIEW"))
            //     return Forbid();

            var roles = await _roleRepository.GetAllAsync();
            return View(roles);
        }

        #endregion

        #region AJAX API Methods

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

        #endregion


        #region Rol CRUD İşlemleri

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                role.CreatedDate = DateTime.Now;
                role.IsActive = true;

                await _roleRepository.AddAsync(role);
                return Json(new { success = true, message = "Rol başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Role role)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingRole = await _roleRepository.GetByIdAsync(role.Id);
                if (existingRole == null)
                    return Json(new { success = false, message = "Rol bulunamadı." });

                role.UpdatedDate = DateTime.Now;
                await _roleRepository.UpdateAsync(role);
                return Json(new { success = true, message = "Rol başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await HasPermissionAsync("DELETE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                await _roleRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Rol başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Rol-Menü-Yetki İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetRoleMenuPermissions(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _roleRepository.GetRoleMenuPermissionsAsync(roleId);
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignMenuPermissionToRole([FromBody] AssignMenuPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _roleRepository.AssignMenuPermissionToRoleAsync(
                    request.RoleId, 
                    request.MenuId, 
                    request.PermissionLevel, 
                    currentUserId, 
                    request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Menü yetkisi başarıyla atandı." });
                else
                    return Json(new { success = false, message = "Menü yetkisi atanamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMenuPermissionFromRole([FromBody] RemoveMenuPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _roleRepository.RemoveMenuPermissionFromRoleAsync(request.RoleId, request.MenuId);
                
                if (success)
                    return Json(new { success = true, message = "Menü yetkisi başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Menü yetkisi kaldırılamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoleMenuPermission([FromBody] UpdateMenuPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _roleRepository.UpdateRoleMenuPermissionAsync(
                    request.RoleId, 
                    request.MenuId, 
                    request.NewPermissionLevel, 
                    currentUserId, 
                    request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Menü yetkisi başarıyla güncellendi." });
                else
                    return Json(new { success = false, message = "Menü yetkisi güncellenemedi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableMenusForRole(int roleId, string? search = null)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _roleRepository.GetAvailableRolesForMenuAsync(roleId, search);
                return Json(new { success = true, data = menus });
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

        #region Rol Detayları

        [HttpGet]
        public async Task<IActionResult> GetRoleDetails(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var role = await _roleRepository.GetRoleWithMenuPermissionsAsync(roleId);
                if (role == null)
                    return Json(new { success = false, message = "Rol bulunamadı." });

                return Json(new { success = true, data = role });
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

        #endregion
    }

    #region Request Models

    public class AssignMenuPermissionRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public string PermissionLevel { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class RemoveMenuPermissionRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    }

    public class UpdateMenuPermissionRequest
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public string NewPermissionLevel { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    #endregion
}
