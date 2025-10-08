using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Kullanıcı-Rol İşlemleri
        Task<bool> AssignRoleToUserAsync(int userId, int roleId, int assignedBy, string? notes = null);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<List<User>> GetUsersByRoleAsync(int roleId);
        
        // Kullanıcı-Menü İşlemleri
        Task<bool> AssignMenuToUserAsync(int userId, int menuId, int assignedBy, DateTime? expiryDate = null, string? notes = null);
        Task<bool> RemoveMenuFromUserAsync(int userId, int menuId);
        Task<List<Menu>> GetUserMenusAsync(int userId);
        
        // Kullanıcı-Menü-İzin İşlemleri
        Task<bool> AssignMenuPermissionToUserAsync(int userId, int menuId, int permissionId, string permissionLevel, int assignedBy, DateTime? expiryDate = null, string? notes = null);
        Task<bool> RemoveMenuPermissionFromUserAsync(int userId, int menuId, int permissionId);
        Task<List<UserMenuPermission>> GetUserMenuPermissionsAsync(int userId);
        Task<List<UserMenuPermission>> GetUserMenuPermissionsByMenuAsync(int userId, int menuId);
        
        // Yetki Kontrolü
        Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel);
        Task<List<string>> GetUserPermissionLevelsAsync(int userId, int menuId);
        Task<bool> CanUserAccessMenuAsync(int userId, int menuId, string permissionLevel);
        
        // Kullanıcı Arama ve Filtreleme
        Task<List<User>> GetAvailableUsersForRoleAsync(int roleId, string? search = null);
        Task<List<User>> GetAvailableUsersForMenuAsync(int menuId, string? search = null);
        Task<List<User>> SearchUsersAsync(string searchTerm);
        Task<List<User>> GetActiveUsersAsync();
        
        // Kullanıcı Bilgileri
        Task<User?> GetUserWithRolesAsync(int userId);
        Task<User?> GetUserWithMenusAsync(int userId);
        Task<User?> GetUserWithPermissionsAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> LoginAsync(string username, string password);
        
        // Kullanıcı İstatistikleri
        Task<int> GetUserCountAsync();
        Task<int> GetActiveUserCountAsync();
        Task<int> GetUserRoleCountAsync(int userId);
        Task<int> GetUserMenuCountAsync(int userId);
    }
}
