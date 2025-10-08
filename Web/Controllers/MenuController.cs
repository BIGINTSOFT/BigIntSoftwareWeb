using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IRoleMenuRepository _roleMenuRepository;
        private readonly IUserMenuPermissionRepository _userMenuPermissionRepository;
        private readonly IRoleMenuPermissionRepository _roleMenuPermissionRepository;

        public MenuController(
            IMenuRepository menuRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IUserMenuRepository userMenuRepository,
            IRoleMenuRepository roleMenuRepository,
            IUserMenuPermissionRepository userMenuPermissionRepository,
            IRoleMenuPermissionRepository roleMenuPermissionRepository)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _userMenuRepository = userMenuRepository;
            _roleMenuRepository = roleMenuRepository;
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
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                var menus = await _menuRepository.GetMenuHierarchyAsync();
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu(int id)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                    return Json(new { success = false, error = "Menü bulunamadı" });

                return Json(new { success = true, data = menu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto menuDto)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menu = new Menu
                {
                    Name = menuDto.Name,
                    Description = menuDto.Description,
                    Icon = menuDto.Icon,
                    Url = menuDto.Url,
                    Controller = menuDto.Controller,
                    Action = menuDto.Action,
                    ParentId = menuDto.ParentId,
                    SortOrder = menuDto.SortOrder,
                    IsActive = menuDto.IsActive,
                    IsVisible = menuDto.IsVisible,
                    CreatedDate = DateTime.Now
                };

                await _menuRepository.AddAsync(menu);

                return Json(new { success = true, message = "Menü başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMenu([FromBody] UpdateMenuDto menuDto)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingMenu = await _menuRepository.GetByIdAsync(menuDto.Id);
                if (existingMenu == null)
                    return Json(new { success = false, message = "Menü bulunamadı." });

                existingMenu.Name = menuDto.Name;
                existingMenu.Description = menuDto.Description;
                existingMenu.Icon = menuDto.Icon;
                existingMenu.Url = menuDto.Url;
                existingMenu.Controller = menuDto.Controller;
                existingMenu.Action = menuDto.Action;
                existingMenu.ParentId = menuDto.ParentId;
                existingMenu.SortOrder = menuDto.SortOrder;
                existingMenu.IsActive = menuDto.IsActive;
                existingMenu.IsVisible = menuDto.IsVisible;
                existingMenu.UpdatedDate = DateTime.Now;

                await _menuRepository.UpdateAsync(existingMenu);

                return Json(new { success = true, message = "Menü başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            if (!await HasPermissionAsync("DELETE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                await _menuRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Menü başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Menü Hiyerarşisi

        [HttpGet]
        public async Task<IActionResult> GetRootMenus()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetRootMenusAsync();
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChildMenus(int parentId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetChildMenusAsync(parentId);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuHierarchy()
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetMenuHierarchyAsync();
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Kullanıcı Menü Yetkileri

        [HttpGet]
        public async Task<IActionResult> GetUserAccessibleMenus(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetUserAccessibleMenusAsync(userId);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAccessibleMenusByPermission(int userId, string permissionLevel)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetUserAccessibleMenusByPermissionAsync(userId, permissionLevel);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CanUserAccessMenu(int userId, int menuId, string permissionLevel)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var canAccess = await _menuRepository.CanUserAccessMenuAsync(userId, menuId, permissionLevel);
                return Json(new { success = true, data = canAccess });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMenuPermissions(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _menuRepository.GetUserMenuPermissionsAsync(userId);
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Rol Menü Yetkileri

        [HttpGet]
        public async Task<IActionResult> GetRoleAccessibleMenus(int roleId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetRoleAccessibleMenusAsync(roleId);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleAccessibleMenusByPermission(int roleId, string permissionLevel)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetRoleAccessibleMenusByPermissionAsync(roleId, permissionLevel);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Menü İstatistikleri

        [HttpGet]
        public async Task<IActionResult> GetMenuUserCount(int menuId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var count = await _menuRepository.GetMenuUserCountAsync(menuId);
                return Json(new { success = true, data = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuRoleCount(int menuId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var count = await _menuRepository.GetMenuRoleCountAsync(menuId);
                return Json(new { success = true, data = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuPermissionCount(int menuId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var count = await _menuRepository.GetMenuPermissionCountAsync(menuId);
                return Json(new { success = true, data = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Menü Arama

        [HttpGet]
        public async Task<IActionResult> SearchMenus(string searchTerm)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.SearchMenusAsync(searchTerm);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenusByController(string controller)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _menuRepository.GetMenusByControllerAsync(controller);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuByRoute(string controller, string action)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menu = await _menuRepository.GetMenuByRouteAsync(controller, action);
                return Json(new { success = true, data = menu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Menü Detayları

        [HttpGet]
        public async Task<IActionResult> GetMenuDetails(int menuId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menu = await _menuRepository.GetMenuWithPermissionsAsync(menuId);
                if (menu == null)
                    return Json(new { success = false, message = "Menü bulunamadı." });

                var userCount = await _menuRepository.GetMenuUserCountAsync(menuId);
                var roleCount = await _menuRepository.GetMenuRoleCountAsync(menuId);
                var permissionCount = await _menuRepository.GetMenuPermissionCountAsync(menuId);
                var childCount = await _menuRepository.GetChildMenuCountAsync(menuId);

                var result = new
                {
                    Menu = menu,
                    UserCount = userCount,
                    RoleCount = roleCount,
                    PermissionCount = permissionCount,
                    ChildMenuCount = childCount
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
            var menuId = await ResolveMenuIdAsync("Menu", "Index");
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
