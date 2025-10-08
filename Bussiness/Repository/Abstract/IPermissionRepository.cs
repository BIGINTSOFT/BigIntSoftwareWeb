using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        // Yetki Seviyeleri
        Task<List<Permission>> GetStandardPermissionsAsync();
        Task<List<string>> GetPermissionLevelsAsync();
        Task<List<Permission>> GetActivePermissionsAsync();
        
        // Yetki Kontrolü
        Task<bool> IsValidPermissionLevelAsync(string permissionLevel);
        Task<Permission?> GetPermissionByCodeAsync(string code);
        Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel);
        
        // Kullanıcı Yetki İşlemleri
        Task<List<UserMenuPermission>> GetUserPermissionsAsync(int userId);
        Task<List<UserMenuPermission>> GetUserPermissionsByMenuAsync(int userId, int menuId);
        Task<List<string>> GetUserPermissionCodesAsync(int userId);
        
        // Rol Yetki İşlemleri
        Task<List<RoleMenuPermission>> GetRolePermissionsAsync(int roleId);
        Task<List<RoleMenuPermission>> GetRolePermissionsByMenuAsync(int roleId, int menuId);
        
        // Yetki İstatistikleri
        Task<Dictionary<string, int>> GetPermissionUsageStatsAsync();
        Task<int> GetPermissionUsageCountAsync(string permissionLevel);
        Task<int> GetUserPermissionCountAsync(int userId);
        Task<int> GetRolePermissionCountAsync(int roleId);
        Task<int> GetMenuPermissionCountAsync(int menuId);
        Task<int> GetPermissionUserMenuCountAsync(int permissionId);
        Task<int> GetPermissionRoleMenuCountAsync(int permissionId);
        
        // Yetki Arama
        Task<List<Permission>> SearchPermissionsAsync(string searchTerm);
        Task<List<Permission>> GetPermissionsByLevelAsync(string permissionLevel);
        
        // Yetki Kullanım Analizi
        Task<List<UserMenuPermission>> GetPermissionUsageByUserAsync(int userId);
        Task<List<RoleMenuPermission>> GetPermissionUsageByRoleAsync(int roleId);
        Task<List<UserMenuPermission>> GetPermissionUsageByMenuAsync(int menuId);
    }
}
