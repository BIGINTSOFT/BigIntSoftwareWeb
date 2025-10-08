using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class UserMenuPermissionRepository : GenericRepository<UserMenuPermission>, IUserMenuPermissionRepository
    {
        public UserMenuPermissionRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Kullanıcı-Menü-İzin İlişkileri

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsAsync(int userId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .Include(ump => ump.User)
                .Include(ump => ump.AssignedByUser)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsByMenuAsync(int userId, int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.IsActive)
                .Include(ump => ump.Permission)
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetMenuUserPermissionsAsync(int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.MenuId == menuId && ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Permission)
                .Include(ump => ump.Menu)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetPermissionUserMenusAsync(int permissionId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.PermissionId == permissionId && ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<UserMenuPermission?> GetUserMenuPermissionAsync(int userId, int menuId, int permissionId)
        {
            return await _context.UserMenuPermissions
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .Include(ump => ump.AssignedByUser)
                .FirstOrDefaultAsync(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionId == permissionId);
        }

        #endregion

        #region İzin Seviyesi Kontrolü

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsByLevelAsync(int userId, string permissionLevel)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetMenuUserPermissionsByLevelAsync(int menuId, string permissionLevel)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.MenuId == menuId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        #endregion

        #region Aktif İlişkiler

        public async Task<List<UserMenuPermission>> GetActiveUserMenuPermissionsAsync(int userId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetActiveMenuUserPermissionsAsync(int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.MenuId == menuId && ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        #endregion

        #region Süre Kontrolü

        public async Task<List<UserMenuPermission>> GetExpiredUserMenuPermissionsAsync()
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.ExpiryDate.HasValue && ump.ExpiryDate < DateTime.Now && ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetExpiringUserMenuPermissionsAsync(int daysBeforeExpiry)
        {
            var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);
            return await _context.UserMenuPermissions
                .Where(ump => ump.ExpiryDate.HasValue && 
                           ump.ExpiryDate <= expiryDate && 
                           ump.ExpiryDate > DateTime.Now && 
                           ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        #endregion

        #region Yetki Kontrolü

        public async Task<bool> HasUserMenuPermissionAsync(int userId, int menuId, string permissionLevel)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .AnyAsync();
        }

        public async Task<List<string>> GetUserMenuPermissionLevelsAsync(int userId, int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.IsActive)
                .Select(ump => ump.PermissionLevel)
                .Distinct()
                .ToListAsync();
        }

        #endregion

        #region İstatistikler

        public async Task<int> GetUserMenuPermissionCountAsync(int userId)
        {
            return await _context.UserMenuPermissions.CountAsync(ump => ump.UserId == userId && ump.IsActive);
        }

        public async Task<int> GetMenuUserPermissionCountAsync(int menuId)
        {
            return await _context.UserMenuPermissions.CountAsync(ump => ump.MenuId == menuId && ump.IsActive);
        }

        public async Task<int> GetPermissionUserMenuCountAsync(int permissionId)
        {
            return await _context.UserMenuPermissions.CountAsync(ump => ump.PermissionId == permissionId && ump.IsActive);
        }

        public async Task<int> GetActiveUserMenuPermissionCountAsync(int userId)
        {
            return await _context.UserMenuPermissions.CountAsync(ump => ump.UserId == userId && ump.IsActive);
        }

        #endregion

        #region Arama ve Filtreleme

        public async Task<List<UserMenuPermission>> SearchUserMenuPermissionsAsync(string searchTerm)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.User.Username.Contains(searchTerm) ||
                           ump.User.FirstName.Contains(searchTerm) ||
                           ump.User.LastName.Contains(searchTerm) ||
                           ump.Menu.Name.Contains(searchTerm) ||
                           ump.Permission.Name.Contains(searchTerm) ||
                           ump.PermissionLevel.Contains(searchTerm) ||
                           ump.Notes.Contains(searchTerm))
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsByAssignedByAsync(int assignedBy)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.AssignedBy == assignedBy)
                .Include(ump => ump.User)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .Include(ump => ump.AssignedByUser)
                .ToListAsync();
        }

        #endregion
    }
}
