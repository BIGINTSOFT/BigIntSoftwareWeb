using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task<Permission?> GetByCodeAsync(string code);
        Task<IEnumerable<Permission>> GetActivePermissionsAsync();
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId);
        Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId);
        Task<bool> HasPermissionAsync(int userId, string permissionCode, int? menuId = null);
        Task<IEnumerable<string>> GetUserPermissionCodesAsync(int userId, int? menuId = null);
        Task<bool> HasAnyPermissionAsync(int userId, IEnumerable<string> permissionCodes, int? menuId = null);
        
        // Permission-Role Management
        Task<IEnumerable<Permission>> GetAvailablePermissionsForRoleAsync(int roleId, int? menuId = null, string search = "");
        Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId, int? menuId = null);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId, int? menuId = null);
        
        // Permission-User Management
        Task<IEnumerable<Permission>> GetAvailablePermissionsForUserAsync(int userId, int? menuId = null, string search = "");
        Task<bool> AssignPermissionToUserAsync(int userId, int permissionId, int? menuId = null);
        Task<bool> RemovePermissionFromUserAsync(int userId, int permissionId, int? menuId = null);
        
        // Get entities that have specific permission
        Task<IEnumerable<Role>> GetRolesByPermissionIdAsync(int permissionId);
        Task<IEnumerable<User>> GetUsersByPermissionIdAsync(int permissionId);
    }
}
