using Bussiness.Repository.Abstract;
using DataAccess.DbContext;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

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

        public Task<List<string>> GetPermissionLevelsAsync()
        {
            return Task.FromResult(new List<string> { "VIEW", "CREATE", "EDIT", "DELETE" });
        }

        #endregion

        #region Yetki Kontrolü

        public Task<bool> IsValidPermissionLevelAsync(string permissionLevel)
        {
            var validLevels = new[] { "VIEW", "CREATE", "EDIT", "DELETE" };
            return Task.FromResult(validLevels.Contains(permissionLevel.ToUpper()));
        }

        public async Task<Permission?> GetPermissionByCodeAsync(string code)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
        }

        public async Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel)
        {
            // Bu metod UserRepository'de implement edildi, burada sadece interface uyumluluğu için
            return await Task.FromResult(false);
        }

        public async Task<List<string>> GetUserPermissionCodesAsync(int userId)
        {
            // Bu metod UserRepository'de implement edildi, burada sadece interface uyumluluğu için
            return await Task.FromResult(new List<string>());
        }

        #endregion

        #region Yetki İstatistikleri

        public async Task<Dictionary<string, int>> GetPermissionUsageStatsAsync()
        {
            var stats = new Dictionary<string, int>();

            // Rol yetkileri
            var rolePermissionStats = await _context.RoleMenuPermissions
                .Where(rmp => rmp.IsActive)
                .GroupBy(rmp => rmp.PermissionLevel)
                .Select(g => new { PermissionLevel = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var stat in rolePermissionStats)
            {
                if (stat.PermissionLevel != null)
                {
                    if (stats.ContainsKey(stat.PermissionLevel))
                        stats[stat.PermissionLevel] += stat.Count;
                    else
                        stats[stat.PermissionLevel] = stat.Count;
                }
            }

            // Kullanıcı ekstra yetkileri
            var userPermissionStats = await _context.UserExtraPermissions
                .Where(uep => uep.IsActive)
                .GroupBy(uep => uep.PermissionLevel)
                .Select(g => new { PermissionLevel = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var stat in userPermissionStats)
            {
                if (stat.PermissionLevel != null)
                {
                    if (stats.ContainsKey(stat.PermissionLevel))
                        stats[stat.PermissionLevel] += stat.Count;
                    else
                        stats[stat.PermissionLevel] = stat.Count;
                }
            }

            return stats;
        }

        public async Task<int> GetPermissionUsageCountAsync(string permissionLevel)
        {
            var roleCount = await _context.RoleMenuPermissions
                .CountAsync(rmp => rmp.PermissionLevel == permissionLevel && rmp.IsActive);

            var userCount = await _context.UserExtraPermissions
                .CountAsync(uep => uep.PermissionLevel == permissionLevel && uep.IsActive);

            return roleCount + userCount;
        }

        #endregion

        #region Yetki Arama

        public async Task<List<Permission>> SearchPermissionsAsync(string searchTerm)
        {
            return await _context.Permissions
                .Where(p => p.IsActive && 
                           (p.Name.Contains(searchTerm) || 
                            p.Description.Contains(searchTerm) ||
                            p.Code.Contains(searchTerm)))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Permission>> GetActivePermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        #endregion
    }
}
