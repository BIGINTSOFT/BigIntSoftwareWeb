using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class UserMenuRepository : GenericRepository<UserMenu>, IUserMenuRepository
    {
        public UserMenuRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Kullanıcı-Menü İlişkileri

        public async Task<List<UserMenu>> GetUserMenusAsync(int userId)
        {
            return await _context.UserMenus
                .Where(um => um.UserId == userId && um.IsActive)
                .Include(um => um.Menu)
                .Include(um => um.User)
                .Include(um => um.AssignedByUser)
                .ToListAsync();
        }

        public async Task<List<UserMenu>> GetMenuUsersAsync(int menuId)
        {
            return await _context.UserMenus
                .Where(um => um.MenuId == menuId && um.IsActive)
                .Include(um => um.User)
                .Include(um => um.Menu)
                .Include(um => um.AssignedByUser)
                .ToListAsync();
        }

        public async Task<UserMenu?> GetUserMenuAsync(int userId, int menuId)
        {
            return await _context.UserMenus
                .Include(um => um.User)
                .Include(um => um.Menu)
                .Include(um => um.AssignedByUser)
                .FirstOrDefaultAsync(um => um.UserId == userId && um.MenuId == menuId);
        }

        #endregion

        #region Aktif İlişkiler

        public async Task<List<UserMenu>> GetActiveUserMenusAsync(int userId)
        {
            return await _context.UserMenus
                .Where(um => um.UserId == userId && um.IsActive)
                .Include(um => um.Menu)
                .ToListAsync();
        }

        public async Task<List<UserMenu>> GetActiveMenuUsersAsync(int menuId)
        {
            return await _context.UserMenus
                .Where(um => um.MenuId == menuId && um.IsActive)
                .Include(um => um.User)
                .ToListAsync();
        }

        #endregion

        #region Süre Kontrolü

        public async Task<List<UserMenu>> GetExpiredUserMenusAsync()
        {
            return await _context.UserMenus
                .Where(um => um.ExpiryDate.HasValue && um.ExpiryDate < DateTime.Now && um.IsActive)
                .Include(um => um.User)
                .Include(um => um.Menu)
                .ToListAsync();
        }

        public async Task<List<UserMenu>> GetExpiringUserMenusAsync(int daysBeforeExpiry)
        {
            var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);
            return await _context.UserMenus
                .Where(um => um.ExpiryDate.HasValue && 
                           um.ExpiryDate <= expiryDate && 
                           um.ExpiryDate > DateTime.Now && 
                           um.IsActive)
                .Include(um => um.User)
                .Include(um => um.Menu)
                .ToListAsync();
        }

        #endregion

        #region İstatistikler

        public async Task<int> GetUserMenuCountAsync(int userId)
        {
            return await _context.UserMenus.CountAsync(um => um.UserId == userId && um.IsActive);
        }

        public async Task<int> GetMenuUserCountAsync(int menuId)
        {
            return await _context.UserMenus.CountAsync(um => um.MenuId == menuId && um.IsActive);
        }

        public async Task<int> GetActiveUserMenuCountAsync(int userId)
        {
            return await _context.UserMenus.CountAsync(um => um.UserId == userId && um.IsActive);
        }

        #endregion

        #region Arama ve Filtreleme

        public async Task<List<UserMenu>> SearchUserMenusAsync(string searchTerm)
        {
            return await _context.UserMenus
                .Where(um => um.User.Username.Contains(searchTerm) ||
                           um.User.FirstName.Contains(searchTerm) ||
                           um.User.LastName.Contains(searchTerm) ||
                           um.Menu.Name.Contains(searchTerm) ||
                           um.Notes.Contains(searchTerm))
                .Include(um => um.User)
                .Include(um => um.Menu)
                .ToListAsync();
        }

        public async Task<List<UserMenu>> GetUserMenusByAssignedByAsync(int assignedBy)
        {
            return await _context.UserMenus
                .Where(um => um.AssignedBy == assignedBy)
                .Include(um => um.User)
                .Include(um => um.Menu)
                .Include(um => um.AssignedByUser)
                .ToListAsync();
        }

        #endregion
    }
}
