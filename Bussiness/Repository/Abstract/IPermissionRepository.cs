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
    }
}
