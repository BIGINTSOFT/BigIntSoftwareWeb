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

        public PermissionController(
            IPermissionRepository permissionRepository,
            IUserRepository userRepository,
            IMenuRepository menuRepository)
        {
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _menuRepository = menuRepository;
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
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var permissions = await _permissionRepository.GetAllAsync();
                
                // DTO'ya dönüştür - Circular reference'ları önle
                var permissionDtos = permissions.Select(permission => new
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Description = permission.Description,
                    IsActive = permission.IsActive,
                    CreatedDate = permission.CreatedDate,
                    UpdatedDate = permission.UpdatedDate
                    // Navigation property'leri dahil etme!
                }).ToList();
                
                return Json(new { success = true, data = permissionDtos });
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

                // DTO'ya dönüştür
                var permissionDto = new
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Description = permission.Description,
                    IsActive = permission.IsActive,
                    CreatedDate = permission.CreatedDate,
                    UpdatedDate = permission.UpdatedDate
                };

                return Json(new { success = true, data = permissionDto });
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
                    Code = permissionDto.Code,
                    Description = permissionDto.Description,
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
                existingPermission.Code = permissionDto.Code;
                existingPermission.Description = permissionDto.Description;
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
                var existingPermission = await _permissionRepository.GetByIdAsync(id);
                if (existingPermission == null)
                    return Json(new { success = false, message = "Yetki bulunamadı." });

                // Soft delete - sadece IsActive'i false yap
                existingPermission.IsActive = false;
                existingPermission.UpdatedDate = DateTime.Now;

                await _permissionRepository.UpdateAsync(existingPermission);
                return Json(new { success = true, message = "Yetki başarıyla pasif hale getirildi." });
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