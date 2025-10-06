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

        // Permission-Role Management
        public async Task<IEnumerable<Permission>> GetAvailablePermissionsForRoleAsync(int roleId, int? menuId = null, string search = "")
        {
            // Get role's current permissions
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            // Get all active permissions
            var query = _context.Permissions.Where(p => p.IsActive);

            // Filter out already assigned permissions
            if (rolePermissions.Any())
            {
                query = query.Where(p => !rolePermissions.Contains(p.Id));
            }

            // Apply menu filter if specified
            if (menuId.HasValue)
            {
                // Only show permissions that are not already assigned to this role for this menu
                var roleMenuPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId && rp.MenuId == menuId && rp.IsActive)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                if (roleMenuPermissions.Any())
                {
                    query = query.Where(p => !roleMenuPermissions.Contains(p.Id));
                }
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || 
                                       (p.Code != null && p.Code.Contains(search)) ||
                                       (p.Description != null && p.Description.Contains(search)));
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId, int? menuId = null)
        {
            // Check if already exists
            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && 
                               (menuId == null || rp.MenuId == menuId));

            if (exists)
                return false;

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                MenuId = menuId,
                AssignedDate = DateTime.Now,
                IsActive = true
            };

            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId, int? menuId = null)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && 
                                          (menuId == null || rp.MenuId == menuId));

            if (rolePermission == null)
                return false;

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }

        // Permission-User Management
        public async Task<IEnumerable<Permission>> GetAvailablePermissionsForUserAsync(int userId, int? menuId = null, string search = "")
        {
            // Get user's current direct permissions
            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive)
                .Select(up => up.PermissionId)
                .ToListAsync();

            // Get all active permissions
            var query = _context.Permissions.Where(p => p.IsActive);

            // Filter out already assigned permissions
            if (userPermissions.Any())
            {
                query = query.Where(p => !userPermissions.Contains(p.Id));
            }

            // Apply menu filter if specified
            if (menuId.HasValue)
            {
                // Only show permissions that are not already assigned to this user for this menu
                var userMenuPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == userId && up.MenuId == menuId && up.IsActive)
                    .Select(up => up.PermissionId)
                    .ToListAsync();

                if (userMenuPermissions.Any())
                {
                    query = query.Where(p => !userMenuPermissions.Contains(p.Id));
                }
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || 
                                       (p.Code != null && p.Code.Contains(search)) ||
                                       (p.Description != null && p.Description.Contains(search)));
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AssignPermissionToUserAsync(int userId, int permissionId, int? menuId = null)
        {
            // Check if already exists
            var exists = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId && 
                               (menuId == null || up.MenuId == menuId));

            if (exists)
                return false;

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                MenuId = menuId,
                AssignedDate = DateTime.Now,
                IsActive = true
            };

            _context.UserPermissions.Add(userPermission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionFromUserAsync(int userId, int permissionId, int? menuId = null)
        {
            var userPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId && 
                                          (menuId == null || up.MenuId == menuId));

            if (userPermission == null)
                return false;

            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get entities that have specific permission
        public async Task<IEnumerable<Role>> GetRolesByPermissionIdAsync(int permissionId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.PermissionId == permissionId && rp.IsActive)
                .Select(rp => rp.Role)
                .Where(r => r.IsActive)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByPermissionIdAsync(int permissionId)
        {
            return await _context.UserPermissions
                .Where(up => up.PermissionId == permissionId && up.IsActive)
                .Select(up => up.User)
                .Where(u => u.IsActive)
                .Distinct()
                .ToListAsync();
        }
    }
}
