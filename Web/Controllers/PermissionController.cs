using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;

namespace Web.Controllers
{
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserMenuPermissionRepository _userMenuPermissionRepository;
        private readonly IRoleMenuPermissionRepository _roleMenuPermissionRepository;

        public PermissionController(
            IPermissionRepository permissionRepository,
            IUserRepository userRepository,
            IMenuRepository menuRepository,
            IRoleRepository roleRepository,
            IUserMenuPermissionRepository userMenuPermissionRepository,
            IRoleMenuPermissionRepository roleMenuPermissionRepository)
        {
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _menuRepository = menuRepository;
            _roleRepository = roleRepository;
            _userMenuPermissionRepository = userMenuPermissionRepository;
            _roleMenuPermissionRepository = roleMenuPermissionRepository;
        }

        #region Ana Sayfalar

        public async Task<IActionResult> Index()
        {
            // Geçici olarak yetki kontrolünü kaldırdık
            // if (!await HasPermissionAsync("VIEW"))
            //     return Forbid();

            return View();
        }

        #endregion

        #region DevExtreme DataGrid API Methods

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
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermission(int id)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(id);
                if (permission == null)
                    return Json(new { success = false, error = "Yetki bulunamadı" });

                return Json(new { success = true, data = permission });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto permissionDto)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permission = new Permission
                {
                    Name = permissionDto.Name,
                    Description = permissionDto.Description,
                    Code = permissionDto.Code,
                    IsActive = permissionDto.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _permissionRepository.AddAsync(permission);
                return Json(new { success = true, message = "Yetki başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermission([FromBody] UpdatePermissionDto permissionDto)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingPermission = await _permissionRepository.GetByIdAsync(permissionDto.Id);
                if (existingPermission == null)
                    return Json(new { success = false, message = "Yetki bulunamadı." });

                existingPermission.Name = permissionDto.Name;
                existingPermission.Description = permissionDto.Description;
                existingPermission.Code = permissionDto.Code;
                existingPermission.IsActive = permissionDto.IsActive;
                existingPermission.UpdatedDate = DateTime.Now;

                await _permissionRepository.UpdateAsync(existingPermission);
                return Json(new { success = true, message = "Yetki başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePermission(int id)
        {
            if (!await HasPermissionAsync("DELETE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                await _permissionRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Yetki başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yetki Seviyeleri

        [HttpGet]
        public async Task<IActionResult> GetStandardPermissions()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _permissionRepository.GetStandardPermissionsAsync();
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionLevels()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var levels = await _permissionRepository.GetPermissionLevelsAsync();
                return Json(new { success = true, data = levels });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsValidPermissionLevel(string permissionLevel)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var isValid = await _permissionRepository.IsValidPermissionLevelAsync(permissionLevel);
                return Json(new { success = true, data = isValid });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yetki İstatistikleri

        [HttpGet]
        public async Task<IActionResult> GetPermissionUsageStats()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var stats = await _permissionRepository.GetPermissionUsageStatsAsync();
                return Json(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionUsageCount(string permissionLevel)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var count = await _permissionRepository.GetPermissionUsageCountAsync(permissionLevel);
                return Json(new { success = true, data = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yetki Arama

        [HttpGet]
        public async Task<IActionResult> SearchPermissions(string searchTerm)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _permissionRepository.SearchPermissionsAsync(searchTerm);
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetActivePermissions()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _permissionRepository.GetActivePermissionsAsync();
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionByCode(string code)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permission = await _permissionRepository.GetPermissionByCodeAsync(code);
                return Json(new { success = true, data = permission });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yetki Kullanım Analizi

        [HttpGet]
        public async Task<IActionResult> GetPermissionUsageByUser(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var usage = await _permissionRepository.GetPermissionUsageByUserAsync(userId);
                return Json(new { success = true, data = usage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionUsageByRole(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var usage = await _permissionRepository.GetPermissionUsageByRoleAsync(roleId);
                return Json(new { success = true, data = usage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissionUsageByMenu(int menuId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var usage = await _permissionRepository.GetPermissionUsageByMenuAsync(menuId);
                return Json(new { success = true, data = usage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yetki Detayları

        [HttpGet]
        public async Task<IActionResult> GetPermissionDetails(int permissionId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                    return Json(new { success = false, message = "Yetki bulunamadı." });

                var usageStats = await _permissionRepository.GetPermissionUsageStatsAsync();
                var userCount = await _permissionRepository.GetPermissionUserMenuCountAsync(permissionId);
                var roleCount = await _permissionRepository.GetPermissionRoleMenuCountAsync(permissionId);
                var menuCount = await _permissionRepository.GetMenuPermissionCountAsync(permissionId);

                var result = new
                {
                    Permission = permission,
                    UsageStats = usageStats,
                    UserCount = userCount,
                    RoleCount = roleCount,
                    MenuCount = menuCount
                };

                return Json(new { success = true, data = result });
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
            var menuId = await ResolveMenuIdAsync("Permission", "Index");
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
}
