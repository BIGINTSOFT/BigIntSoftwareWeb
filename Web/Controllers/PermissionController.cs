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
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Forbid();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            if (!await HasPermissionAsync("VIEW", 18))
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
            if (!await HasPermissionAsync("CREATE", 18))
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
            if (!await HasPermissionAsync("EDIT", 18))
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
            if (!await HasPermissionAsync("DELETE", 18))
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

        // Permission-Role Management
        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(int roleId, int? menuId = null)
        {
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var permissions = await _permissionRepository.GetRolePermissionsAsync(roleId);
                var permissionDtos = permissions.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    code = p.Code,
                    description = p.Description,
                    isActive = p.IsActive
                }).ToList();

                return Json(new { success = true, data = permissionDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesByPermission(int permissionId)
        {
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var roles = await _permissionRepository.GetRolesByPermissionIdAsync(permissionId);
                var roleDtos = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    description = r.Description,
                    isActive = r.IsActive
                }).ToList();

                return Json(new { success = true, data = roleDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailablePermissionsForRole(int roleId, int? menuId = null, string search = "")
        {
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var permissions = await _permissionRepository.GetAvailablePermissionsForRoleAsync(roleId, menuId, search);
                var permissionDtos = permissions.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    code = p.Code,
                    description = p.Description,
                    isActive = p.IsActive
                }).ToList();

                return Json(new { success = true, data = permissionDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId, int? menuId = null)
        {
            if (!await HasPermissionAsync("EDIT", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var result = await _permissionRepository.AssignPermissionToRoleAsync(roleId, permissionId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Yetki role başarıyla atandı" });
                }
                else
                {
                    return Json(new { error = "Yetki zaten bu role atanmış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId, int? menuId = null)
        {
            if (!await HasPermissionAsync("EDIT", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var result = await _permissionRepository.RemovePermissionFromRoleAsync(roleId, permissionId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Yetki rolden başarıyla çıkarıldı" });
                }
                else
                {
                    return Json(new { error = "Yetki bu role atanmamış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Permission-User Management
        [HttpGet]
        public async Task<IActionResult> GetUserPermissions(int userId, int? menuId = null)
        {
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var permissions = await _permissionRepository.GetUserPermissionsAsync(userId);
                var permissionDtos = permissions.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    code = p.Code,
                    description = p.Description,
                    isActive = p.IsActive
                }).ToList();

                return Json(new { success = true, data = permissionDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByPermission(int permissionId)
        {
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var users = await _permissionRepository.GetUsersByPermissionIdAsync(permissionId);
                var userDtos = users.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    isActive = u.IsActive
                }).ToList();

                return Json(new { success = true, data = userDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailablePermissionsForUser(int userId, int? menuId = null, string search = "")
        {
            if (!await HasPermissionAsync("VIEW", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var permissions = await _permissionRepository.GetAvailablePermissionsForUserAsync(userId, menuId, search);
                var permissionDtos = permissions.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    code = p.Code,
                    description = p.Description,
                    isActive = p.IsActive
                }).ToList();

                return Json(new { success = true, data = permissionDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissionToUser(int userId, int permissionId, int? menuId = null)
        {
            if (!await HasPermissionAsync("EDIT", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var result = await _permissionRepository.AssignPermissionToUserAsync(userId, permissionId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Yetki kullanıcıya başarıyla atandı" });
                }
                else
                {
                    return Json(new { error = "Yetki zaten bu kullanıcıya atanmış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePermissionFromUser(int userId, int permissionId, int? menuId = null)
        {
            if (!await HasPermissionAsync("EDIT", 18))
            {
                return Json(new { error = "Bu işlem için yetkiniz bulunmamaktadır" });
            }

            try
            {
                var result = await _permissionRepository.RemovePermissionFromUserAsync(userId, permissionId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Yetki kullanıcıdan başarıyla çıkarıldı" });
                }
                else
                {
                    return Json(new { error = "Yetki bu kullanıcıya atanmamış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
