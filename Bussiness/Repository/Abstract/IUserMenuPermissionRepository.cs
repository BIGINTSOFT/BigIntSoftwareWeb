using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IUserMenuPermissionRepository : IGenericRepository<UserMenuPermission>
    {
        // Kullanıcı-Menü-İzin İlişkileri
        Task<List<UserMenuPermission>> GetUserMenuPermissionsAsync(int userId);
        Task<List<UserMenuPermission>> GetUserMenuPermissionsByMenuAsync(int userId, int menuId);
        Task<List<UserMenuPermission>> GetMenuUserPermissionsAsync(int menuId);
        Task<List<UserMenuPermission>> GetPermissionUserMenusAsync(int permissionId);
        Task<UserMenuPermission?> GetUserMenuPermissionAsync(int userId, int menuId, int permissionId);
        
        // İzin Seviyesi Kontrolü
        Task<List<UserMenuPermission>> GetUserMenuPermissionsByLevelAsync(int userId, string permissionLevel);
        Task<List<UserMenuPermission>> GetMenuUserPermissionsByLevelAsync(int menuId, string permissionLevel);
        
        // Aktif İlişkiler
        Task<List<UserMenuPermission>> GetActiveUserMenuPermissionsAsync(int userId);
        Task<List<UserMenuPermission>> GetActiveMenuUserPermissionsAsync(int menuId);
        
        // Süre Kontrolü
        Task<List<UserMenuPermission>> GetExpiredUserMenuPermissionsAsync();
        Task<List<UserMenuPermission>> GetExpiringUserMenuPermissionsAsync(int daysBeforeExpiry);
        
        // Yetki Kontrolü
        Task<bool> HasUserMenuPermissionAsync(int userId, int menuId, string permissionLevel);
        Task<List<string>> GetUserMenuPermissionLevelsAsync(int userId, int menuId);
        
        // İstatistikler
        Task<int> GetUserMenuPermissionCountAsync(int userId);
        Task<int> GetMenuUserPermissionCountAsync(int menuId);
        Task<int> GetPermissionUserMenuCountAsync(int permissionId);
        Task<int> GetActiveUserMenuPermissionCountAsync(int userId);
        
        // Arama ve Filtreleme
        Task<List<UserMenuPermission>> SearchUserMenuPermissionsAsync(string searchTerm);
        Task<List<UserMenuPermission>> GetUserMenuPermissionsByAssignedByAsync(int assignedBy);
    }
}
