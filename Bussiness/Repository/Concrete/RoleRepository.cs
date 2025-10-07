using Bussiness.Repository.Abstract;
using DataAccess.DbContext;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Repository.Concrete
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Rol-Menü-Yetki İşlemleri

        public async Task<bool> AssignMenuPermissionToRoleAsync(int roleId, int menuId, string permissionLevel, int assignedBy, string? notes = null)
        {
            try
            {
                // Rol ve menü var mı kontrol et
                var role = await _context.Roles.FindAsync(roleId);
                var menu = await _context.Menus.FindAsync(menuId);
                
                if (role == null || menu == null)
                    return false;

                // Zaten atanmış mı kontrol et
                var existingPermission = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId);

                if (existingPermission != null)
                {
                    // Güncelle
                    existingPermission.PermissionLevel = permissionLevel;
                    existingPermission.AssignedBy = assignedBy;
                    existingPermission.Notes = notes;
                    existingPermission.AssignedDate = DateTime.Now;
                    existingPermission.IsActive = true;
                }
                else
                {
                    // Yeni atama
                    var roleMenuPermission = new RoleMenuPermission
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                        PermissionLevel = permissionLevel,
                        AssignedBy = assignedBy,
                        Notes = notes,
                        AssignedDate = DateTime.Now,
                        IsActive = true
                    };
                    _context.RoleMenuPermissions.Add(roleMenuPermission);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveMenuPermissionFromRoleAsync(int roleId, int menuId)
        {
            try
            {
                var roleMenuPermission = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId);

                if (roleMenuPermission != null)
                {
                    roleMenuPermission.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveMenuPermissionFromRoleByIdAsync(int roleId, int permissionId)
        {
            try
            {
                var permission = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.Id == permissionId);

                if (permission != null)
                {
                    permission.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRoleMenuPermissionAsync(int roleId, int menuId, string newPermissionLevel, int assignedBy, string? notes = null)
        {
            try
            {
                var roleMenuPermission = await _context.RoleMenuPermissions
                    .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId);

                if (roleMenuPermission != null)
                {
                    roleMenuPermission.PermissionLevel = newPermissionLevel;
                    roleMenuPermission.AssignedBy = assignedBy;
                    roleMenuPermission.Notes = notes;
                    roleMenuPermission.AssignedDate = DateTime.Now;
                    roleMenuPermission.IsActive = true;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Rol Yetkilerini Getirme

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.AssignedByUser)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByMenuAsync(int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.AssignedByUser)
                .ToListAsync();
        }

        public async Task<RoleMenuPermission?> GetRoleMenuPermissionAsync(int roleId, int menuId)
        {
            return await _context.RoleMenuPermissions
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.AssignedByUser)
                .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.IsActive);
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
            return await _context.UserRoles
                .CountAsync(ur => ur.RoleId == roleId && ur.IsActive);
        }

        #endregion

        #region Rol Arama ve Filtreleme

        public async Task<List<Role>> GetAvailableRolesForUserAsync(int userId, string? search = null)
        {
            var query = _context.Roles
                .Where(r => r.IsActive && 
                           !_context.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == r.Id && ur.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search) || 
                                        (r.Description != null && r.Description.Contains(search)));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Role>> GetAvailableRolesForMenuAsync(int menuId, string? search = null)
        {
            var query = _context.Roles
                .Where(r => r.IsActive && 
                           !_context.RoleMenuPermissions.Any(rmp => rmp.RoleId == r.Id && rmp.MenuId == menuId && rmp.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search) || 
                                        (r.Description != null && r.Description.Contains(search)));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Menu>> GetAvailableMenusForRolePermissionAsync(int roleId, string? search = null)
        {
            var query = _context.Menus
                .Where(m => m.IsActive && m.IsVisible);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Name.Contains(search) || 
                                        (m.Controller != null && m.Controller.Contains(search)) ||
                                        (m.Action != null && m.Action.Contains(search)));
            }

            return await query.ToListAsync();
        }

        #endregion

        #region Rol Bilgileri

        public async Task<Role?> GetRoleWithMenuPermissionsAsync(int roleId)
        {
            return await _context.Roles
                .Include(r => r.RoleMenuPermissions.Where(rmp => rmp.IsActive))
                    .ThenInclude(rmp => rmp.Menu)
                .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<List<Role>> GetActiveRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        #endregion
    }
}
