using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;
using System.Security.Claims;

namespace Web.Controllers
{
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : null;
        }

        private async Task<bool> HasPermissionAsync(string permissionCode, int? menuId = null)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return false;
            return await _permissionRepository.HasPermissionAsync(userId.Value, permissionCode, menuId);
        }

        public async Task<IActionResult> Index()
        {
            // Permission Yönetimi menü ID'si: 4 (varsayılan)
            if (!await HasPermissionAsync("VIEW", 4))
            {
                return Forbid();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            if (!await HasPermissionAsync("VIEW", 4))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var permissions = await _permissionRepository.GetAllAsync();
                var permissionList = permissions.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    code = p.Code,
                    isActive = p.IsActive,
                    createdDate = p.CreatedDate
                }).ToList();
                
                return Json(new { data = permissionList });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermission(int id)
        {
            try
            {
                Console.WriteLine($"🔍 GetPermission called with id: {id}");
                var permission = await _permissionRepository.GetByIdAsync(id);
                if (permission == null)
                {
                    Console.WriteLine("❌ Permission not found");
                    return Json(new { success = false, error = "Yetki bulunamadı" });
                }
                
                Console.WriteLine($"✅ Permission found: {permission.Name}");
                var permissionDto = new
                {
                    id = permission.Id,
                    name = permission.Name,
                    code = permission.Code,
                    description = permission.Description,
                    isActive = permission.IsActive,
                    createdDate = permission.CreatedDate
                };
                
                Console.WriteLine($"🔍 PermissionDto created: {permissionDto.name}");
                return Json(new { success = true, data = permissionDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetPermission error: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionDto model)
        {
            if (!await HasPermissionAsync("CREATE", 4))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz veri", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var existingPermission = await _permissionRepository.GetByCodeAsync(model.Code);
                if (existingPermission != null)
                {
                    return Json(new { error = "Bu yetki kodu zaten kullanılıyor" });
                }

                var permission = new Permission
                {
                    Name = model.Name,
                    Description = model.Description,
                    Code = model.Code,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _permissionRepository.AddAsync(permission);

                return Json(new { success = true, message = "Yetki başarıyla oluşturuldu" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdatePermissionDto model)
        {
            if (!await HasPermissionAsync("EDIT", 4))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz veri", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var permission = await _permissionRepository.GetByIdAsync(model.Id);
                if (permission == null)
                {
                    return Json(new { error = "Yetki bulunamadı" });
                }

                // Check for duplicate code if changed
                if (permission.Code != model.Code)
                {
                    var existingPermission = await _permissionRepository.GetByCodeAsync(model.Code);
                    if (existingPermission != null && existingPermission.Id != model.Id)
                    {
                        return Json(new { error = "Bu yetki kodu zaten kullanılıyor" });
                    }
                }

                permission.Name = model.Name;
                permission.Description = model.Description;
                permission.Code = model.Code;
                permission.IsActive = model.IsActive;
                permission.UpdatedDate = DateTime.Now;

                await _permissionRepository.UpdateAsync(permission);

                return Json(new { success = true, message = "Yetki başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await HasPermissionAsync("DELETE", 4))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var permission = await _permissionRepository.GetByIdAsync(id);
                if (permission == null)
                {
                    return Json(new { error = "Yetki bulunamadı" });
                }

                await _permissionRepository.DeleteAsync(permission);

                return Json(new { success = true, message = "Yetki başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
