using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User?> LoginAsync(string usernameOrEmail, string password);
        
        // Role Management
        Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId);
        Task<IEnumerable<User>> GetAvailableUsersForRoleAsync(int roleId, string search = "");
        Task<bool> AssignUserToRoleAsync(int userId, int roleId);
        Task<bool> RemoveUserFromRoleAsync(int userId, int roleId);
        
        // Permission Management
        Task<IEnumerable<User>> GetUsersByMenuIdAsync(int menuId);
        Task<IEnumerable<User>> GetAvailableUsersForMenuPermissionAsync(int menuId, string search = "");
        Task<bool> AssignUserToMenuAsync(int userId, int menuId);
        Task<bool> RemoveUserFromMenuAsync(int userId, int menuId);
    }
}
