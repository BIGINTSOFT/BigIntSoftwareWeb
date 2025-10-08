using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Kullanıcı-Rol İşlemleri

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, int assignedBy, string? notes = null)
        {
            try
            {
                // Zaten atanmış mı kontrol et
                var existing = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existing != null)
                    return false; // Zaten atanmış

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedBy = assignedBy,
                    AssignedDate = DateTime.Now,
                    IsActive = true,
                    Notes = notes
                };

                _context.UserRoles.Add(userRole);
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

                if (userRole == null)
                    return false;

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Role>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .Include(ur => ur.User)
                .Select(ur => ur.User)
                .ToListAsync();
        }

        #endregion

        #region Kullanıcı-Menü İşlemleri

        public async Task<bool> AssignMenuToUserAsync(int userId, int menuId, int assignedBy, DateTime? expiryDate = null, string? notes = null)
        {
            try
            {
                var existing = await _context.UserMenus
                    .FirstOrDefaultAsync(um => um.UserId == userId && um.MenuId == menuId);

                if (existing != null)
                    return false;

                var userMenu = new UserMenu
                {
                    UserId = userId,
                    MenuId = menuId,
                    AssignedBy = assignedBy,
                    AssignedDate = DateTime.Now,
                    ExpiryDate = expiryDate,
                    IsActive = true,
                    Notes = notes
                };

                _context.UserMenus.Add(userMenu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveMenuFromUserAsync(int userId, int menuId)
        {
            try
            {
                var userMenu = await _context.UserMenus
                    .FirstOrDefaultAsync(um => um.UserId == userId && um.MenuId == menuId);

                if (userMenu == null)
                    return false;

                _context.UserMenus.Remove(userMenu);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Menu>> GetUserMenusAsync(int userId)
        {
            return await _context.UserMenus
                .Where(um => um.UserId == userId && um.IsActive)
                .Include(um => um.Menu)
                .Select(um => um.Menu)
                .ToListAsync();
        }

        #endregion

        #region Kullanıcı-Menü-İzin İşlemleri

        public async Task<bool> AssignMenuPermissionToUserAsync(int userId, int menuId, int permissionId, string permissionLevel, int assignedBy, DateTime? expiryDate = null, string? notes = null)
        {
            try
            {
                var existing = await _context.UserMenuPermissions
                    .FirstOrDefaultAsync(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionId == permissionId);

                if (existing != null)
                    return false;

                var userMenuPermission = new UserMenuPermission
                {
                    UserId = userId,
                    MenuId = menuId,
                    PermissionId = permissionId,
                    PermissionLevel = permissionLevel,
                    AssignedBy = assignedBy,
                    AssignedDate = DateTime.Now,
                    ExpiryDate = expiryDate,
                    IsActive = true,
                    Notes = notes
                };

                _context.UserMenuPermissions.Add(userMenuPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveMenuPermissionFromUserAsync(int userId, int menuId, int permissionId)
        {
            try
            {
                var userMenuPermission = await _context.UserMenuPermissions
                    .FirstOrDefaultAsync(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionId == permissionId);

                if (userMenuPermission == null)
                    return false;

                _context.UserMenuPermissions.Remove(userMenuPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsAsync(int userId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsByMenuAsync(int userId, int menuId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.IsActive)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        #endregion

        #region Yetki Kontrolü

        public async Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel)
        {
            // Önce kullanıcının rolleri üzerinden kontrol et
            var rolePermission = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.MenuId == menuId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .AnyAsync();

            if (rolePermission)
                return true;

            // Sonra kullanıcının direkt izinlerini kontrol et
            var userPermission = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .AnyAsync();

            return userPermission;
        }

        public async Task<List<string>> GetUserPermissionLevelsAsync(int userId, int menuId)
        {
            var rolePermissions = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.MenuId == menuId && rmp.IsActive)
                .Select(rmp => rmp.PermissionLevel)
                .ToListAsync();

            var userPermissions = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.IsActive)
                .Select(ump => ump.PermissionLevel)
                .ToListAsync();

            return rolePermissions.Union(userPermissions).Distinct().ToList();
        }

        public async Task<bool> CanUserAccessMenuAsync(int userId, int menuId, string permissionLevel)
        {
            return await HasPermissionAsync(userId, menuId, permissionLevel);
        }

        #endregion

        #region Kullanıcı Arama ve Filtreleme

        public async Task<List<User>> GetAvailableUsersForRoleAsync(int roleId, string? search = null)
        {
            var query = _context.Users
                .Where(u => u.IsActive && !u.UserRoles.Any(ur => ur.RoleId == roleId && ur.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || u.FirstName.Contains(search) || u.LastName.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<List<User>> GetAvailableUsersForMenuAsync(int menuId, string? search = null)
        {
            var query = _context.Users
                .Where(u => u.IsActive && !u.UserMenus.Any(um => um.MenuId == menuId && um.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || u.FirstName.Contains(search) || u.LastName.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            return await _context.Users
                .Where(u => u.Username.Contains(searchTerm) || 
                           u.FirstName.Contains(searchTerm) || 
                           u.LastName.Contains(searchTerm) ||
                           u.Email.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        #endregion

        #region Kullanıcı Bilgileri

        public async Task<User?> GetUserWithRolesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithMenusAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserMenus)
                    .ThenInclude(um => um.Menu)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithPermissionsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserMenuPermissions)
                    .ThenInclude(ump => ump.Menu)
                .Include(u => u.UserMenuPermissions)
                    .ThenInclude(ump => ump.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);
        }

        #endregion

        #region Kullanıcı İstatistikleri

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            return await _context.Users.CountAsync(u => u.IsActive);
        }

        public async Task<int> GetUserRoleCountAsync(int userId)
        {
            return await _context.UserRoles.CountAsync(ur => ur.UserId == userId && ur.IsActive);
        }

        public async Task<int> GetUserMenuCountAsync(int userId)
        {
            return await _context.UserMenus.CountAsync(um => um.UserId == userId && um.IsActive);
        }

        #endregion
    }
}
