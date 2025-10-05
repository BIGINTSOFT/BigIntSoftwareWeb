using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;
using System.Security.Claims;

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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                var roleList = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    description = r.Description,
                    isActive = r.IsActive,
                    createdDate = r.CreatedDate
                }).ToList();
                
                return Json(new { data = roleList });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRole(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return Json(new { success = false, error = "Rol bulunamadı" });
                }
                
                var roleData = new
                {
                    id = role.Id,
                    name = role.Name,
                    description = role.Description,
                    isActive = role.IsActive,
                    createdDate = role.CreatedDate
                };
                
                return Json(new { success = true, data = roleData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz veri", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var existingRole = await _roleRepository.GetByNameAsync(model.Name);
                if (existingRole != null)
                {
                    return Json(new { error = "Bu rol adı zaten kullanılıyor" });
                }

                var role = new Role
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _roleRepository.AddAsync(role);

                return Json(new { success = true, message = "Rol başarıyla oluşturuldu" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz veri", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var role = await _roleRepository.GetByIdAsync(model.Id);
                if (role == null)
                {
                    return Json(new { error = "Rol bulunamadı" });
                }

                // Check for duplicate name if changed
                if (role.Name != model.Name)
                {
                    var existingRole = await _roleRepository.GetByNameAsync(model.Name);
                    if (existingRole != null && existingRole.Id != model.Id)
                    {
                        return Json(new { error = "Bu rol adı zaten kullanılıyor" });
                    }
                }

                role.Name = model.Name;
                role.Description = model.Description;
                role.IsActive = model.IsActive;
                role.UpdatedDate = DateTime.Now;

                await _roleRepository.UpdateAsync(role);

                return Json(new { success = true, message = "Rol başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return Json(new { error = "Rol bulunamadı" });
                }

                await _roleRepository.DeleteAsync(role);

                return Json(new { success = true, message = "Rol başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Role Users Management
        [HttpGet]
        public async Task<IActionResult> GetRoleUsers(int roleId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return Json(new { error = "Rol bulunamadı" });
                }

                var users = await _userRepository.GetUsersByRoleIdAsync(roleId);
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
        public async Task<IActionResult> GetAvailableUsers(int roleId, string search = "")
        {
            try
            {
                var users = await _userRepository.GetAvailableUsersForRoleAsync(roleId, search);
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

        [HttpPost]
        public async Task<IActionResult> AssignUserToRole(int roleId, int userId)
        {
            try
            {
                Console.WriteLine($"AssignUserToRole: roleId={roleId}, userId={userId}");
                var result = await _userRepository.AssignUserToRoleAsync(userId, roleId);
                Console.WriteLine($"AssignUserToRole result: {result}");
                
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcı role başarıyla atandı" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı zaten bu role atanmış veya kullanıcı bulunamadı" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AssignUserToRole Exception: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUserFromRole(int roleId, int userId)
        {
            try
            {
                var result = await _userRepository.RemoveUserFromRoleAsync(userId, roleId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcı rolden başarıyla çıkarıldı" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı bu role atanmamış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Role Menu Permissions Management
        [HttpGet]
        public async Task<IActionResult> GetRoleMenus(int roleId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return Json(new { error = "Rol bulunamadı" });
                }

                var menus = await _menuRepository.GetRoleMenusAsync(roleId);
                var menuDtos = menus.Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    icon = m.Icon,
                    controller = m.Controller,
                    action = m.Action,
                    isActive = m.IsActive
                }).ToList();

                return Json(new { success = true, data = menuDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableMenusForRole(int roleId, string search = "")
        {
            try
            {
                var menus = await _roleRepository.GetAvailableMenusForRoleAsync(roleId, search);
                var menuDtos = menus.Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    icon = m.Icon,
                    controller = m.Controller,
                    action = m.Action,
                    isActive = m.IsActive
                }).ToList();

                return Json(new { success = true, data = menuDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignMenuToRole(int roleId, int menuId)
        {
            try
            {
                var result = await _roleRepository.AssignMenuToRoleAsync(roleId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Menü role başarıyla atandı" });
                }
                else
                {
                    return Json(new { error = "Menü zaten bu role atanmış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMenuFromRole(int roleId, int menuId)
        {
            try
            {
                var result = await _roleRepository.RemoveMenuFromRoleAsync(roleId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Menü rolden başarıyla çıkarıldı" });
                }
                else
                {
                    return Json(new { error = "Menü bu role atanmamış" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
