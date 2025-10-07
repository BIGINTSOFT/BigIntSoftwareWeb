using Bussiness.Repository.Abstract;
using DataAccess.DbContext;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

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
                // Kullanıcı ve rol var mı kontrol et
                var user = await _context.Users.FindAsync(userId);
                var role = await _context.Roles.FindAsync(roleId);
                
                if (user == null || role == null)
                    return false;

                // Zaten atanmış mı kontrol et
                var existingAssignment = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingAssignment != null)
                {
                    // Zaten atanmış, aktif hale getir
                    existingAssignment.IsActive = true;
                    existingAssignment.AssignedDate = DateTime.Now;
                    existingAssignment.AssignedBy = assignedBy;
                    existingAssignment.Notes = notes;
                }
                else
                {
                    // Yeni atama
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        AssignedBy = assignedBy,
                        Notes = notes,
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
                    return true;
                }
                return false;
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

        #region Kullanıcı Ekstra Yetki İşlemleri

        public async Task<bool> AssignExtraPermissionToUserAsync(int userId, int menuId, string permissionLevel, string reason, int assignedBy, DateTime? expiryDate = null, string? notes = null)
        {
            try
            {
                // Kullanıcı ve menü var mı kontrol et
                var user = await _context.Users.FindAsync(userId);
                var menu = await _context.Menus.FindAsync(menuId);
                
                if (user == null || menu == null)
                    return false;

                // Zaten atanmış mı kontrol et
                var existingPermission = await _context.UserExtraPermissions
                    .FirstOrDefaultAsync(uep => uep.UserId == userId && uep.MenuId == menuId);

                if (existingPermission != null)
                {
                    // Güncelle
                    existingPermission.PermissionLevel = permissionLevel;
                    existingPermission.Reason = reason;
                    existingPermission.ExpiryDate = expiryDate;
                    existingPermission.AssignedBy = assignedBy;
                    existingPermission.Notes = notes;
                    existingPermission.AssignedDate = DateTime.Now;
                    existingPermission.IsActive = true;
                }
                else
                {
                    // Yeni atama
                    var userExtraPermission = new UserExtraPermission
                    {
                        UserId = userId,
                        MenuId = menuId,
                        PermissionLevel = permissionLevel,
                        Reason = reason,
                        ExpiryDate = expiryDate,
                        AssignedBy = assignedBy,
                        Notes = notes,
                        AssignedDate = DateTime.Now,
                        IsActive = true
                    };
                    _context.UserExtraPermissions.Add(userExtraPermission);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveExtraPermissionFromUserAsync(int userId, int menuId)
        {
            try
            {
                var userExtraPermission = await _context.UserExtraPermissions
                    .FirstOrDefaultAsync(uep => uep.UserId == userId && uep.MenuId == menuId);

                if (userExtraPermission != null)
                {
                    userExtraPermission.IsActive = false;
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

        public async Task<List<UserExtraPermission>> GetUserExtraPermissionsAsync(int userId)
        {
            return await _context.UserExtraPermissions
                .Where(uep => uep.UserId == userId && uep.IsActive)
                .Include(uep => uep.Menu)
                .Include(uep => uep.AssignedByUser)
                .ToListAsync();
        }

        public async Task<bool> RemoveExtraPermissionFromUserByIdAsync(int userId, int permissionId)
        {
            try
            {
                var permission = await _context.UserExtraPermissions
                    .FirstOrDefaultAsync(uep => uep.UserId == userId && uep.Id == permissionId);

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

        public async Task<List<Menu>> GetAvailableMenusForExtraPermissionAsync(int userId, string? search = null)
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

        #region Yetki Kontrolü

        public async Task<bool> HasPermissionAsync(int userId, int menuId, string permissionLevel)
        {
            // Önce ekstra yetkileri kontrol et
            var hasExtraPermission = await _context.UserExtraPermissions
                .AnyAsync(uep => uep.UserId == userId && 
                                uep.MenuId == menuId && 
                                uep.PermissionLevel == permissionLevel && 
                                uep.IsActive &&
                                (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now));

            if (hasExtraPermission)
                return true;

            // Rol yetkilerini kontrol et
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!userRoles.Any())
                return false;

            var hasRolePermission = await _context.RoleMenuPermissions
                .AnyAsync(rmp => userRoles.Contains(rmp.RoleId) && 
                                rmp.MenuId == menuId && 
                                rmp.PermissionLevel == permissionLevel && 
                                rmp.IsActive);

            return hasRolePermission;
        }

        public async Task<List<string>> GetUserPermissionLevelsAsync(int userId, int menuId)
        {
            var permissionLevels = new List<string>();

            if (menuId == 0)
            {
                // Tüm menüler için yetkileri getir
                // Ekstra yetkiler
                var extraPermissions = await _context.UserExtraPermissions
                    .Where(uep => uep.UserId == userId && 
                                 uep.IsActive &&
                                 (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now))
                    .Select(uep => uep.PermissionLevel)
                    .ToListAsync();

                permissionLevels.AddRange(extraPermissions);

                // Rol yetkileri
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.IsActive)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                if (userRoles.Any())
                {
                    var rolePermissions = await _context.RoleMenuPermissions
                        .Where(rmp => userRoles.Contains(rmp.RoleId) && rmp.IsActive)
                        .Select(rmp => rmp.PermissionLevel)
                        .ToListAsync();

                    permissionLevels.AddRange(rolePermissions);
                }
            }
            else
            {
                // Belirli menü için yetkileri getir
                // Ekstra yetkiler
                var extraPermissions = await _context.UserExtraPermissions
                    .Where(uep => uep.UserId == userId && 
                                 uep.MenuId == menuId && 
                                 uep.IsActive &&
                                 (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now))
                    .Select(uep => uep.PermissionLevel)
                    .ToListAsync();

                permissionLevels.AddRange(extraPermissions);

                // Rol yetkileri
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.IsActive)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                if (userRoles.Any())
                {
                    var rolePermissions = await _context.RoleMenuPermissions
                        .Where(rmp => userRoles.Contains(rmp.RoleId) && 
                                     rmp.MenuId == menuId && 
                                     rmp.IsActive)
                        .Select(rmp => rmp.PermissionLevel)
                        .ToListAsync();

                    permissionLevels.AddRange(rolePermissions);
                }
            }

            return permissionLevels.Distinct().ToList();
        }

        #endregion

        #region Kullanıcı Arama ve Filtreleme

        public async Task<List<User>> GetAvailableUsersForRoleAsync(int roleId, string? search = null)
        {
            var query = _context.Users
                .Where(u => u.IsActive && 
                           !_context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId && ur.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || 
                                        u.FirstName.Contains(search) || 
                                        u.LastName.Contains(search) || 
                                        u.Email.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<List<User>> GetAvailableUsersForMenuAsync(int menuId, string? search = null)
        {
            var query = _context.Users
                .Where(u => u.IsActive && 
                           !_context.UserExtraPermissions.Any(uep => uep.UserId == u.Id && uep.MenuId == menuId && uep.IsActive));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || 
                                        u.FirstName.Contains(search) || 
                                        u.LastName.Contains(search) || 
                                        u.Email.Contains(search));
            }

            return await query.ToListAsync();
        }

        #endregion

        #region Kullanıcı Bilgileri

        public async Task<User?> GetUserWithRolesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserExtraPermissions.Where(uep => uep.IsActive))
                    .ThenInclude(uep => uep.Menu)
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
            var hashedPassword = HashPassword(password);
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        #endregion
    }
}
