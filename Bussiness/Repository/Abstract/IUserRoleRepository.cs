using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        // Kullanıcı-Rol İlişkileri
        Task<List<UserRole>> GetUserRolesAsync(int userId);
        Task<List<UserRole>> GetRoleUsersAsync(int roleId);
        Task<UserRole?> GetUserRoleAsync(int userId, int roleId);
        
        // Aktif İlişkiler
        Task<List<UserRole>> GetActiveUserRolesAsync(int userId);
        Task<List<UserRole>> GetActiveRoleUsersAsync(int roleId);
        
        // Süre Kontrolü
        Task<List<UserRole>> GetExpiredUserRolesAsync();
        Task<List<UserRole>> GetExpiringUserRolesAsync(int daysBeforeExpiry);
        
        // İstatistikler
        Task<int> GetUserRoleCountAsync(int userId);
        Task<int> GetRoleUserCountAsync(int roleId);
        Task<int> GetActiveUserRoleCountAsync(int userId);
        
        // Arama ve Filtreleme
        Task<List<UserRole>> SearchUserRolesAsync(string searchTerm);
        Task<List<UserRole>> GetUserRolesByAssignedByAsync(int assignedBy);
    }
}
