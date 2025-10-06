using DataAccess.DbContext;
using Entities.Entity;
using Bussiness.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Repository.Concrete
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        public async Task<Permission?> GetByCodeAsync(string code)
        {
            return await GetFirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
        {
            return await GetWhereAsync(p => p.IsActive);
        }

        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
        {
            // Get permissions from user's roles
            var rolePermissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .SelectMany(ur => ur.Role.RolePermissions
                    .Where(rp => rp.IsActive)
                    .Select(rp => rp.Permission))
                .Where(p => p.IsActive)
                .Distinct()
                .ToListAsync();

            // Get direct user permissions
            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive)
                .Select(up => up.Permission)
                .Where(p => p.IsActive)
                .Distinct()
                .ToListAsync();

            // Combine and return unique permissions
            return rolePermissions.Union(userPermissions);
        }

        public async Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive)
                .Select(rp => rp.Permission)
                .Where(p => p.IsActive)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionCode, int? menuId = null)
        {
            // Check role permissions
            var hasRolePermission = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .SelectMany(ur => ur.Role.RolePermissions
                    .Where(rp => rp.IsActive && 
                           rp.Permission.Code == permissionCode &&
                           (menuId == null || rp.MenuId == menuId)))
                .AnyAsync();

            if (hasRolePermission)
                return true;

            // Check direct user permissions
            var hasUserPermission = await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive &&
                       up.Permission.Code == permissionCode &&
                       (menuId == null || up.MenuId == menuId))
                .AnyAsync();

            return hasUserPermission;
        }

        public async Task<IEnumerable<string>> GetUserPermissionCodesAsync(int userId, int? menuId = null)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            
            return permissions
                .Where(p => menuId == null || 
                           p.RolePermissions.Any(rp => rp.MenuId == menuId) || 
                           p.UserPermissions.Any(up => up.MenuId == menuId))
                .Select(p => p.Code ?? "")
                .Where(code => !string.IsNullOrEmpty(code));
        }

        public async Task<bool> HasAnyPermissionAsync(int userId, IEnumerable<string> permissionCodes, int? menuId = null)
        {
            foreach (var permissionCode in permissionCodes)
            {
                if (await HasPermissionAsync(userId, permissionCode, menuId))
                    return true;
            }
            return false;
        }
    }
}
