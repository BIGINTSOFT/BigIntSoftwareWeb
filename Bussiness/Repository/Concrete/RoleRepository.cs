using DataAccess.DbContext;
using Entities.Entity;
using Bussiness.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Repository.Concrete
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await GetFirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
        {
            return await GetWhereAsync(r => r.IsActive);
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            try
            {
                var existingUserRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole != null)
                {
                    existingUserRole.IsActive = true;
                    existingUserRole.AssignedDate = DateTime.Now;
                }
                else
                {
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        AssignedDate = DateTime.Now,
                        IsActive = true
                    };
                    _context.UserRoles.Add(userRole);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole != null)
                {
                    userRole.IsActive = false;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Menu Permission Management
        public async Task<IEnumerable<Role>> GetRolesByMenuIdAsync(int menuId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.MenuId == menuId && rp.IsActive)
                .Include(rp => rp.Role)
                .Select(rp => rp.Role)
                .Where(r => r.IsActive)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetAvailableRolesForMenuPermissionAsync(int menuId, string search = "")
        {
            var query = _context.Roles.Where(r => r.IsActive && 
                                                 !r.RolePermissions.Any(rp => rp.MenuId == menuId && rp.IsActive));
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search) || r.Description.Contains(search));
            }
            
            return await query.ToListAsync();
        }

        public async Task<bool> AssignRoleToMenuAsync(int roleId, int menuId)
        {
            try
            {
                var existingRolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.MenuId == menuId);

                if (existingRolePermission != null)
                {
                    return false; // Already assigned
                }

                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    MenuId = menuId,
                    AssignedDate = DateTime.Now
                };

                _context.RolePermissions.Add(rolePermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveRoleFromMenuAsync(int roleId, int menuId)
        {
            try
            {
                var rolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.MenuId == menuId);

                if (rolePermission == null)
                {
                    return false; // Not assigned
                }

                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // User Role Management
        public async Task<IEnumerable<Role>> GetAvailableRolesForUserAsync(int userId, string search = "")
        {
            var query = _context.Roles.Where(r => !r.UserRoles.Any(ur => ur.UserId == userId));
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Name.Contains(search) || r.Description.Contains(search));
            }
            
            return await query.ToListAsync();
        }

        // Role Menu Management
        public async Task<IEnumerable<Menu>> GetRoleMenusAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive && rp.MenuId.HasValue)
                .Select(rp => rp.Menu!)
                .Where(m => m.IsActive && m.IsVisible)
                .Distinct()
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetAvailableMenusForRoleAsync(int roleId, string search = "")
        {
            // Bu role atanmış menüleri al
            var assignedMenuIds = (await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive && rp.MenuId.HasValue)
                .Select(rp => rp.MenuId!.Value)
                .ToListAsync()).ToHashSet();

            // Atanmamış menüleri getir
            var query = _context.Menus.Where(m => !assignedMenuIds.Contains(m.Id));
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Name.Contains(search) || 
                                       (m.Description != null && m.Description.Contains(search)));
            }
            
            return await query.Where(m => m.IsActive && m.IsVisible)
                .Distinct()
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<bool> AssignMenuToRoleAsync(int roleId, int menuId)
        {
            try
            {
                // Önce aktif kayıt var mı kontrol et
                var existingActiveAssignment = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.MenuId == menuId && rp.IsActive);
                
                if (existingActiveAssignment != null) return false; // Zaten aktif

                // Inactive kayıt var mı kontrol et
                var existingInactiveAssignment = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.MenuId == menuId && !rp.IsActive);
                
                if (existingInactiveAssignment != null)
                {
                    // Inactive kaydı aktif yap
                    existingInactiveAssignment.IsActive = true;
                    existingInactiveAssignment.AssignedDate = DateTime.Now;
                }
                else
                {
                    // Yeni kayıt oluştur
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                        AssignedDate = DateTime.Now,
                        IsActive = true
                    };
                    _context.RolePermissions.Add(rolePermission);
                }

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
                // Tüm aktif kayıtları bul ve soft delete yap
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId && rp.MenuId == menuId && rp.IsActive)
                    .ToListAsync();
                
                if (!rolePermissions.Any()) return false;

                // Tüm kayıtları soft delete yap
                foreach (var rolePermission in rolePermissions)
                {
                    rolePermission.IsActive = false;
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
