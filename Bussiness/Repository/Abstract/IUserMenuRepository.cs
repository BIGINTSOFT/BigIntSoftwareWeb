using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IUserMenuRepository : IGenericRepository<UserMenu>
    {
        // Kullanıcı-Menü İlişkileri
        Task<List<UserMenu>> GetUserMenusAsync(int userId);
        Task<List<UserMenu>> GetMenuUsersAsync(int menuId);
        Task<UserMenu?> GetUserMenuAsync(int userId, int menuId);
        
        // Aktif İlişkiler
        Task<List<UserMenu>> GetActiveUserMenusAsync(int userId);
        Task<List<UserMenu>> GetActiveMenuUsersAsync(int menuId);
        
        // Süre Kontrolü
        Task<List<UserMenu>> GetExpiredUserMenusAsync();
        Task<List<UserMenu>> GetExpiringUserMenusAsync(int daysBeforeExpiry);
        
        // İstatistikler
        Task<int> GetUserMenuCountAsync(int userId);
        Task<int> GetMenuUserCountAsync(int menuId);
        Task<int> GetActiveUserMenuCountAsync(int userId);
        
        // Arama ve Filtreleme
        Task<List<UserMenu>> SearchUserMenusAsync(string searchTerm);
        Task<List<UserMenu>> GetUserMenusByAssignedByAsync(int assignedBy);
    }
}
