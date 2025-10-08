using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Kullanıcı-Rol İlişkileri

        public async Task<List<UserRole>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Include(ur => ur.AssignedByUser)
                .ToListAsync();
        }

        public async Task<List<UserRole>> GetRoleUsersAsync(int roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Include(ur => ur.AssignedByUser)
                .ToListAsync();
        }

        public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Include(ur => ur.AssignedByUser)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        #endregion

        #region Aktif İlişkiler

        public async Task<List<UserRole>> GetActiveUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<UserRole>> GetActiveRoleUsersAsync(int roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .Include(ur => ur.User)
                .ToListAsync();
        }

        #endregion

        #region Süre Kontrolü

        public async Task<List<UserRole>> GetExpiredUserRolesAsync()
        {
            return await _context.UserRoles
                .Where(ur => ur.ExpiryDate.HasValue && ur.ExpiryDate < DateTime.Now && ur.IsActive)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<UserRole>> GetExpiringUserRolesAsync(int daysBeforeExpiry)
        {
            var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);
            return await _context.UserRoles
                .Where(ur => ur.ExpiryDate.HasValue && 
                           ur.ExpiryDate <= expiryDate && 
                           ur.ExpiryDate > DateTime.Now && 
                           ur.IsActive)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        #endregion

        #region İstatistikler

        public async Task<int> GetUserRoleCountAsync(int userId)
        {
            return await _context.UserRoles.CountAsync(ur => ur.UserId == userId && ur.IsActive);
        }

        public async Task<int> GetRoleUserCountAsync(int roleId)
        {
            return await _context.UserRoles.CountAsync(ur => ur.RoleId == roleId && ur.IsActive);
        }

        public async Task<int> GetActiveUserRoleCountAsync(int userId)
        {
            return await _context.UserRoles.CountAsync(ur => ur.UserId == userId && ur.IsActive);
        }

        #endregion

        #region Arama ve Filtreleme

        public async Task<List<UserRole>> SearchUserRolesAsync(string searchTerm)
        {
            return await _context.UserRoles
                .Where(ur => ur.User.Username.Contains(searchTerm) ||
                           ur.User.FirstName.Contains(searchTerm) ||
                           ur.User.LastName.Contains(searchTerm) ||
                           ur.Role.Name.Contains(searchTerm) ||
                           ur.Role.Description.Contains(searchTerm) ||
                           ur.Notes.Contains(searchTerm))
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<UserRole>> GetUserRolesByAssignedByAsync(int assignedBy)
        {
            return await _context.UserRoles
                .Where(ur => ur.AssignedBy == assignedBy)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Include(ur => ur.AssignedByUser)
                .ToListAsync();
        }

        #endregion
    }
}
