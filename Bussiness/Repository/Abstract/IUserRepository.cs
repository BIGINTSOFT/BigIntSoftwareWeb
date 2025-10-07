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
        
        // Kullanıcı Ekstra Yetki İşlemleri
        Task<bool> AssignExtraPermissionToUserAsync(int userId, int menuId, string permissionLevel, string reason, int assignedBy, DateTime? expiryDate = null, string? notes = null);
        Task<bool> RemoveExtraPermissionFromUserAsync(int userId, int menuId);
        Task<bool> RemoveExtraPermissionFromUserByIdAsync(int userId, int permissionId);
        Task<List<UserExtraPermission>> GetUserExtraPermissionsAsync(int userId);
        Task<List<Menu>> GetAvailableMenusForExtraPermissionAsync(int userId, string? search = null);
        
        // Yetki Kontrolü
        Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel);
        Task<List<string>> GetUserPermissionLevelsAsync(int userId, int menuId);
        
        // Kullanıcı Arama ve Filtreleme
        Task<List<User>> GetAvailableUsersForRoleAsync(int roleId, string? search = null);
        Task<List<User>> GetAvailableUsersForMenuAsync(int menuId, string? search = null);
        
        // Kullanıcı Bilgileri
        Task<User?> GetUserWithRolesAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> LoginAsync(string username, string password);
    }
}
