using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Yetki Seviyeleri

        public async Task<List<Permission>> GetStandardPermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetPermissionLevelsAsync()
        {
            return new List<string> { "VIEW", "CREATE", "EDIT", "DELETE", "EXPORT", "IMPORT", "PRINT", "APPROVE" };
        }

        public async Task<List<Permission>> GetActivePermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        #endregion

        #region Yetki Kontrolü

        public async Task<bool> IsValidPermissionLevelAsync(string permissionLevel)
        {
            var validLevels = await GetPermissionLevelsAsync();
            return validLevels.Contains(permissionLevel);
        }

        public async Task<Permission?> GetPermissionByCodeAsync(string code)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
        }

        public async Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel)
        {
            // Rol üzerinden kontrol
            var rolePermission = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.MenuId == menuId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .AnyAsync();

            if (rolePermission)
                return true;

            // Kullanıcı üzerinden kontrol
            var userPermission = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .AnyAsync();

            return userPermission;
        }

        #endregion

        #region Kullanıcı Yetki İşlemleri

        public async Task<List<UserMenuPermission>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetUserPermissionsByMenuAsync(int userId, int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.IsActive)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserPermissionCodesAsync(int userId)
        {
            var rolePermissions = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.IsActive)
                .Include(rmp => rmp.Permission)
                .Select(rmp => rmp.Permission.Code)
                .ToListAsync();

            var userPermissions = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Permission)
                .Select(ump => ump.Permission.Code)
                .ToListAsync();

            return rolePermissions.Union(userPermissions).Distinct().ToList();
        }

        #endregion

        #region Rol Yetki İşlemleri

        public async Task<List<RoleMenuPermission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetRolePermissionsByMenuAsync(int roleId, int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        #endregion

        #region Yetki İstatistikleri

        public async Task<Dictionary<string, int>> GetPermissionUsageStatsAsync()
        {
            var roleStats = await _context.RoleMenuPermissions
                .Where(rmp => rmp.IsActive)
                .GroupBy(rmp => rmp.PermissionLevel)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var userStats = await _context.UserMenuPermissions
                .Where(ump => ump.IsActive)
                .GroupBy(ump => ump.PermissionLevel)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var result = new Dictionary<string, int>();
            foreach (var kvp in roleStats)
            {
                result[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in userStats)
            {
                if (result.ContainsKey(kvp.Key))
                {
                    result[kvp.Key] += kvp.Value;
                }
                else
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        public async Task<int> GetPermissionUsageCountAsync(string permissionLevel)
        {
            var roleCount = await _context.RoleMenuPermissions
                .CountAsync(rmp => rmp.PermissionLevel == permissionLevel && rmp.IsActive);

            var userCount = await _context.UserMenuPermissions
                .CountAsync(ump => ump.PermissionLevel == permissionLevel && ump.IsActive);

            return roleCount + userCount;
        }

        public async Task<int> GetUserPermissionCountAsync(int userId)
        {
            var rolePermissions = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.IsActive)
                .CountAsync();

            var userPermissions = await _context.UserMenuPermissions
                .CountAsync(ump => ump.UserId == userId && ump.IsActive);

            return rolePermissions + userPermissions;
        }

        public async Task<int> GetRolePermissionCountAsync(int roleId)
        {
            return await _context.RoleMenuPermissions.CountAsync(rmp => rmp.RoleId == roleId && rmp.IsActive);
        }

        public async Task<int> GetMenuPermissionCountAsync(int menuId)
        {
            var rolePermissions = await _context.RoleMenuPermissions
                .CountAsync(rmp => rmp.MenuId == menuId && rmp.IsActive);

            var userPermissions = await _context.UserMenuPermissions
                .CountAsync(ump => ump.MenuId == menuId && ump.IsActive);

            return rolePermissions + userPermissions;
        }

        public async Task<int> GetPermissionUserMenuCountAsync(int permissionId)
        {
            return await _context.UserMenuPermissions
                .CountAsync(ump => ump.PermissionId == permissionId && ump.IsActive);
        }

        public async Task<int> GetPermissionRoleMenuCountAsync(int permissionId)
        {
            return await _context.RoleMenuPermissions
                .CountAsync(rmp => rmp.PermissionId == permissionId && rmp.IsActive);
        }

        #endregion

        #region Yetki Arama

        public async Task<List<Permission>> SearchPermissionsAsync(string searchTerm)
        {
            return await _context.Permissions
                .Where(p => p.Name.Contains(searchTerm) || 
                           p.Description.Contains(searchTerm) ||
                           p.Code.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<List<Permission>> GetPermissionsByLevelAsync(string permissionLevel)
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        #endregion

        #region Yetki Kullanım Analizi

        public async Task<List<UserMenuPermission>> GetPermissionUsageByUserAsync(int userId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetPermissionUsageByRoleAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetPermissionUsageByMenuAsync(int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.MenuId == menuId && ump.IsActive)
                .Include(ump => ump.User)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        #endregion
    }
}
