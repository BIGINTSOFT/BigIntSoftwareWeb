using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        // Yetki Seviyeleri
        Task<List<Permission>> GetStandardPermissionsAsync();
        Task<List<string>> GetPermissionLevelsAsync();
        
        // Yetki Kontrolü
        Task<bool> IsValidPermissionLevelAsync(string permissionLevel);
        Task<Permission?> GetPermissionByCodeAsync(string code);
        
        // Yetki İstatistikleri
        Task<Dictionary<string, int>> GetPermissionUsageStatsAsync();
        Task<int> GetPermissionUsageCountAsync(string permissionLevel);
        
        // Yetki Arama
        Task<List<Permission>> SearchPermissionsAsync(string searchTerm);
        Task<List<Permission>> GetActivePermissionsAsync();
        Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel);
        Task<List<string>> GetUserPermissionCodesAsync(int userId);
    }
}
