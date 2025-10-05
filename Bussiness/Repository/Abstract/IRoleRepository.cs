using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetActiveRolesAsync();
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        
        // Menu Permission Management
        Task<IEnumerable<Role>> GetRolesByMenuIdAsync(int menuId);
        Task<IEnumerable<Role>> GetAvailableRolesForMenuPermissionAsync(int menuId, string search = "");
        Task<bool> AssignRoleToMenuAsync(int roleId, int menuId);
        Task<bool> RemoveRoleFromMenuAsync(int roleId, int menuId);
        
        // User Role Management
        Task<IEnumerable<Role>> GetAvailableRolesForUserAsync(int userId, string search = "");
        
        // Role Menu Management
        Task<IEnumerable<Menu>> GetRoleMenusAsync(int roleId);
        Task<IEnumerable<Menu>> GetAvailableMenusForRoleAsync(int roleId, string search = "");
        Task<bool> AssignMenuToRoleAsync(int roleId, int menuId);
        Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId);
    }
}
