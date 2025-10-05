using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;

namespace Web.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public MenuController(IMenuRepository menuRepository, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                var menus = await _menuRepository.GetAllAsync();
                var menuList = menus.Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    description = m.Description,
                    icon = m.Icon,
                    url = m.Url,
                    controller = m.Controller,
                    action = m.Action,
                    parentId = m.ParentId,
                    sortOrder = m.SortOrder,
                    isActive = m.IsActive,
                    isVisible = m.IsVisible,
                    createdDate = m.CreatedDate,
                    parentName = m.Parent?.Name
                }).ToList();
                
                return Json(new { data = menuList });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu(int id)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                {
                    return Json(new { success = false, error = "Menü bulunamadı" });
                }
                
                var menuDto = new
                {
                    id = menu.Id,
                    name = menu.Name,
                    description = menu.Description,
                    icon = menu.Icon,
                    url = menu.Url,
                    controller = menu.Controller,
                    action = menu.Action,
                    parentId = menu.ParentId,
                    sortOrder = menu.SortOrder,
                    isActive = menu.IsActive,
                    isVisible = menu.IsVisible,
                    createdDate = menu.CreatedDate,
                    parentName = menu.Parent?.Name
                };
            
            return Json(new { success = true, data = menuDto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMenuDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz veri", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var menu = new Menu
                {
                    Name = model.Name,
                    Description = model.Description,
                    Icon = model.Icon,
                    Url = model.Url,
                    Controller = model.Controller,
                    Action = model.Action,
                    ParentId = model.ParentId,
                    SortOrder = model.SortOrder,
                    IsActive = model.IsActive,
                    IsVisible = model.IsVisible,
                    CreatedDate = DateTime.Now
                };

                await _menuRepository.AddAsync(menu);

                return Json(new { success = true, message = "Menü başarıyla oluşturuldu" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateMenuDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz veri", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var menu = await _menuRepository.GetByIdAsync(model.Id);
                if (menu == null)
                {
                    return Json(new { error = "Menü bulunamadı" });
                }

                menu.Name = model.Name;
                menu.Description = model.Description;
                menu.Icon = model.Icon;
                menu.Url = model.Url;
                menu.Controller = model.Controller;
                menu.Action = model.Action;
                menu.ParentId = model.ParentId;
                menu.SortOrder = model.SortOrder;
                menu.IsActive = model.IsActive;
                menu.IsVisible = model.IsVisible;
                menu.UpdatedDate = DateTime.Now;

                await _menuRepository.UpdateAsync(menu);

                return Json(new { success = true, message = "Menü başarıyla güncellendi" });
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
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                {
                    return Json(new { error = "Menü bulunamadı" });
                }

                await _menuRepository.DeleteAsync(menu);

                return Json(new { success = true, message = "Menü başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // Menu Permissions Management
        [HttpGet]
        public async Task<IActionResult> GetMenuUserPermissions(int menuId)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(menuId);
                if (menu == null)
                {
                    return Json(new { error = "Menü bulunamadı" });
                }

                // Hem rol bazlı hem direkt kullanıcı yetkilerini getir
                var usersWithSource = await _menuRepository.GetUsersWithSourceByMenuIdAsync(menuId);
                var userDtos = usersWithSource.Select(u => new
                {
                    id = u.User.Id,
                    username = u.User.Username,
                    firstName = u.User.FirstName,
                    lastName = u.User.LastName,
                    email = u.User.Email,
                    isActive = u.User.IsActive,
                    source = u.Source // Rol veya Direct
                }).ToList();

                return Json(new { success = true, data = userDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableUsersForMenuPermission(int menuId, string search = "")
        {
            try
            {
                var users = await _userRepository.GetAvailableUsersForMenuPermissionAsync(menuId, search);
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
        public async Task<IActionResult> GetMenuRolePermissions(int menuId)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(menuId);
                if (menu == null)
                {
                    return Json(new { error = "Menü bulunamadı" });
                }

                var roles = await _roleRepository.GetRolesByMenuIdAsync(menuId);
                var roleDtos = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    description = r.Description,
                    isActive = r.IsActive,
                    source = "Direct" // Direkt rol yetkisi
                }).Distinct().ToList();

                return Json(new { success = true, data = roleDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRolesForMenuPermission(int menuId, string search = "")
        {
            try
            {
                var roles = await _roleRepository.GetAvailableRolesForMenuPermissionAsync(menuId, search);
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

        [HttpPost]
        public async Task<IActionResult> AssignUserToMenu(int menuId, int userId)
        {
            try
            {
                var result = await _userRepository.AssignUserToMenuAsync(userId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcıya menü yetkisi verildi" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı zaten bu menüye yetkili" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUserFromMenu(int menuId, int userId)
        {
            try
            {
                var result = await _userRepository.RemoveUserFromMenuAsync(userId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcının menü yetkisi kaldırıldı" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı bu menüye yetkili değil" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToMenu(int menuId, int roleId)
        {
            try
            {
                var result = await _roleRepository.AssignRoleToMenuAsync(roleId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Role menü yetkisi verildi" });
                }
                else
                {
                    return Json(new { error = "Rol zaten bu menüye yetkili" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveRoleFromMenu(int menuId, int roleId)
        {
            try
            {
                var result = await _roleRepository.RemoveRoleFromMenuAsync(roleId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Rolün menü yetkisi kaldırıldı" });
                }
                else
                {
                    return Json(new { error = "Rol bu menüye yetkili değil" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
