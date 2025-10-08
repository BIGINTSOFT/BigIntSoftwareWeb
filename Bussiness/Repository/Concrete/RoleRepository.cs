using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Rol-Menü İşlemleri

        public async Task<bool> AssignMenuToRoleAsync(int roleId, int menuId, int assignedBy, string? notes = null)
        {
            try
            {
                var existing = await _context.RoleMenus
                    .FirstOrDefaultAsync(rm => rm.RoleId == roleId && rm.MenuId == menuId);

                if (existing != null)
                    return false;

                var roleMenu = new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId,
                    AssignedBy = assignedBy,
                    AssignedDate = DateTime.Now,
                    IsActive = true,
                    Notes = notes
                };

                _context.RoleMenus.Add(roleMenu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId)
        {
            try
            {
                var roleMenu = await _context.RoleMenus
                    .FirstOrDefaultAsync(rm => rm.RoleId == roleId && rm.MenuId == menuId);

                if (roleMenu == null)
                    return false;

                _context.RoleMenus.Remove(roleMenu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Menu>> GetRoleMenusAsync(int roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId && rm.IsActive)
                .Include(rm => rm.Menu)
                .Select(rm => rm.Menu)
                .ToListAsync();
        }

        #endregion

        #region Rol-Menü-İzin İşlemleri

        public async Task<bool> AssignMenuPermissionToRoleAsync(int roleId, int menuId, int permissionId, string permissionLevel, int assignedBy, string? notes = null)
        {
            try
            {
                var existing = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.PermissionId == permissionId);

                if (existing != null)
                    return false;

                var roleMenuPermission = new RoleMenuPermission
                {
                    RoleId = roleId,
                    MenuId = menuId,
                    PermissionId = permissionId,
                    PermissionLevel = permissionLevel,
                    AssignedBy = assignedBy,
                    AssignedDate = DateTime.Now,
                    IsActive = true,
                    Notes = notes
                };

                _context.RoleMenuPermissions.Add(roleMenuPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveMenuPermissionFromRoleAsync(int roleId, int menuId, int permissionId)
        {
            try
            {
                var roleMenuPermission = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.PermissionId == permissionId);

                if (roleMenuPermission == null)
                    return false;

                _context.RoleMenuPermissions.Remove(roleMenuPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRoleMenuPermissionAsync(int roleId, int menuId, int permissionId, string newPermissionLevel, int assignedBy, string? notes = null)
        {
            try
            {
                var roleMenuPermission = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.PermissionId == permissionId);

                if (roleMenuPermission == null)
                    return false;

                roleMenuPermission.PermissionLevel = newPermissionLevel;
                roleMenuPermission.AssignedBy = assignedBy;
                roleMenuPermission.Notes = notes;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByMenuAsync(int roleId, int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<RoleMenuPermission?> GetRoleMenuPermissionAsync(int roleId, int menuId, int permissionId)
        {
            return await _context.RoleMenuPermissions
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.PermissionId == permissionId);
        }

        #endregion

        #region Rol Kullanıcıları

        public async Task<List<User>> GetRoleUsersAsync(int roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .Include(ur => ur.User)
                .Select(ur => ur.User)
                .ToListAsync();
        }

        public async Task<int> GetRoleUserCountAsync(int roleId)
        {
            return await _context.UserRoles.CountAsync(ur => ur.RoleId == roleId && ur.IsActive);
        }

        #endregion

        #region Rol Arama ve Filtreleme

        public async Task<List<Role>> GetAvailableRolesForUserAsync(int userId, string? search = null)
        {
            var query = _context.Roles
                .Where(r => r.IsActive && !r.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search) || r.Description.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Role>> GetAvailableRolesForMenuAsync(int menuId, string? search = null)
        {
            var query = _context.Roles
                .Where(r => r.IsActive && !r.RoleMenus.Any(rm => rm.MenuId == menuId && rm.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search) || r.Description.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Role>> SearchRolesAsync(string searchTerm)
        {
            return await _context.Roles
                .Where(r => r.Name.Contains(searchTerm) || r.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<List<Role>> GetActiveRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        #endregion

        #region Rol Bilgileri

        public async Task<Role?> GetRoleWithMenusAsync(int roleId)
        {
            return await _context.Roles
                .Include(r => r.RoleMenus)
                    .ThenInclude(rm => rm.Menu)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<Role?> GetRoleWithPermissionsAsync(int roleId)
        {
            return await _context.Roles
                .Include(r => r.RoleMenuPermissions)
                    .ThenInclude(rmp => rmp.Menu)
                .Include(r => r.RoleMenuPermissions)
                    .ThenInclude(rmp => rmp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<Role?> GetRoleWithUsersAsync(int roleId)
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        #endregion

        #region Rol İstatistikleri

        public async Task<int> GetRoleCountAsync()
        {
            return await _context.Roles.CountAsync();
        }

        public async Task<int> GetActiveRoleCountAsync()
        {
            return await _context.Roles.CountAsync(r => r.IsActive);
        }

        public async Task<int> GetRoleMenuCountAsync(int roleId)
        {
            return await _context.RoleMenus.CountAsync(rm => rm.RoleId == roleId && rm.IsActive);
        }

        public async Task<int> GetRolePermissionCountAsync(int roleId)
        {
            return await _context.RoleMenuPermissions.CountAsync(rmp => rmp.RoleId == roleId && rmp.IsActive);
        }

        #endregion
    }
}
