using Bussiness.Repository.Abstract;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMenuRepository _menuRepository;

        public PermissionController(IPermissionRepository permissionRepository, IUserRepository userRepository, IMenuRepository menuRepository)
        {
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _menuRepository = menuRepository;
        }

        #region Ana Sayfalar

        public async Task<IActionResult> Index()
        {
            // Geçici olarak yetki kontrolünü kaldırdık
            // if (!await HasPermissionAsync("VIEW"))
            //     return Forbid();

            var permissions = await _permissionRepository.GetAllAsync();
            return View(permissions);
        }

        #endregion

        #region AJAX API Methods

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

        #endregion

        #region Yetki CRUD İşlemleri

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Permission permission)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                permission.CreatedDate = DateTime.Now;
                permission.IsActive = true;

                await _permissionRepository.AddAsync(permission);
                return Json(new { success = true, message = "Yetki başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Permission permission)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingPermission = await _permissionRepository.GetByIdAsync(permission.Id);
                if (existingPermission == null)
                    return Json(new { success = false, message = "Yetki bulunamadı." });

                permission.UpdatedDate = DateTime.Now;
                await _permissionRepository.UpdateAsync(permission);
                return Json(new { success = true, message = "Yetki başarıyla güncellendi." });
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
