using Microsoft.AspNetCore.Mvc;
using Bussiness.Repository.Abstract;
using System.Security.Claims;
using Entities.Entity;

namespace Web.ViewComponents
{
    public class DynamicMenuViewComponent : ViewComponent
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserRepository _userRepository;

        public DynamicMenuViewComponent(IMenuRepository menuRepository, IUserRepository userRepository)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userMenus = await GetUserMenusAsync(User as ClaimsPrincipal);
            var menuHierarchy = BuildMenuHierarchy(userMenus);
            return View(menuHierarchy);
        }

        private async Task<IEnumerable<Menu>> GetUserMenusAsync(ClaimsPrincipal? user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return new List<Menu>();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return new List<Menu>();

            return await _menuRepository.GetUserAccessibleMenusAsync(userId);
        }

        private IEnumerable<Menu> BuildMenuHierarchy(IEnumerable<Menu> menus)
        {
            var menuList = menus.ToList();
            var rootMenus = menuList.Where(m => m.ParentId == null).OrderBy(m => m.SortOrder).ToList();
            
            foreach (var rootMenu in rootMenus)
            {
                rootMenu.Children = GetChildMenus(rootMenu.Id, menuList).ToList();
            }

            return rootMenus;
        }

        private IEnumerable<Menu> GetChildMenus(int parentId, List<Menu> allMenus)
        {
            return allMenus
                .Where(m => m.ParentId == parentId && m.IsVisible && m.IsActive)
                .OrderBy(m => m.SortOrder)
                .Select(m =>
                {
                    m.Children = GetChildMenus(m.Id, allMenus).ToList();
                    return m;
                });
        }
    }
}
