using Entities.Entity;
using Entities.Dto;

namespace Bussiness.Repository.Abstract
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task<IEnumerable<Menu>> GetRootMenusAsync();
        Task<IEnumerable<Menu>> GetChildMenusAsync(int parentId);
        Task<IEnumerable<Menu>> GetUserMenusAsync(int userId);
        Task<IEnumerable<Menu>> GetRoleMenusAsync(int roleId);
        Task<Menu?> GetByControllerActionAsync(string controller, string action);
        Task<IEnumerable<Menu>> GetVisibleMenusAsync();
        
        // User Menu Management
        Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId);
        Task<IEnumerable<Menu>> GetAvailableMenusForUserAsync(int userId, string search = "");
        Task<IEnumerable<User>> GetUsersByMenuIdAsync(int menuId);
        Task<IEnumerable<UserWithSource>> GetUsersWithSourceByMenuIdAsync(int menuId);
        Task<IEnumerable<Menu>> GetUserDirectMenusAsync(int userId);
        Task<IEnumerable<Menu>> GetUserRoleMenusAsync(int userId);
    }
}
