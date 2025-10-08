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

        public MenuController(
            IMenuRepository menuRepository,
            IUserRepository userRepository)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
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
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                var menus = await _menuRepository.GetAllAsync();
                
                // DTO'ya dönüştür - Circular reference'ları önle
                var menuDtos = menus.Select(menu => new
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Description = menu.Description,
                    Icon = menu.Icon,
                    Url = menu.Url,
                    Controller = menu.Controller,
                    Action = menu.Action,
                    ParentId = menu.ParentId,
                    SortOrder = menu.SortOrder,
                    IsActive = menu.IsActive,
                    IsVisible = menu.IsVisible,
                    CreatedDate = menu.CreatedDate,
                    UpdatedDate = menu.UpdatedDate
                    // Navigation property'leri dahil etme!
                }).ToList();
                
                return Json(new { success = true, data = menuDtos });
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

                // DTO'ya dönüştür
                var menuDto = new
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Description = menu.Description,
                    Icon = menu.Icon,
                    Url = menu.Url,
                    Controller = menu.Controller,
                    Action = menu.Action,
                    ParentId = menu.ParentId,
                    SortOrder = menu.SortOrder,
                    IsActive = menu.IsActive,
                    IsVisible = menu.IsVisible,
                    CreatedDate = menu.CreatedDate,
                    UpdatedDate = menu.UpdatedDate
                };

                return Json(new { success = true, data = menuDto });
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
                var existingMenu = await _menuRepository.GetByIdAsync(id);
                if (existingMenu == null)
                    return Json(new { success = false, message = "Menü bulunamadı." });

                // Soft delete - sadece IsActive'i false yap
                existingMenu.IsActive = false;
                existingMenu.UpdatedDate = DateTime.Now;

                await _menuRepository.UpdateAsync(existingMenu);
                return Json(new { success = true, message = "Menü başarıyla pasif hale getirildi." });
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