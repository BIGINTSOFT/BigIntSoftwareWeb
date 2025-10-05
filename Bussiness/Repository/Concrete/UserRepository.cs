using DataAccess.DbContext;
using Entities.Entity;
using Bussiness.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Repository.Concrete
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await GetFirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await GetFirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await GetFirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public async Task<User?> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await GetByUsernameOrEmailAsync(usernameOrEmail);
            
            if (user == null || !user.IsActive)
                return null;

            if (ValidatePassword(password, user.Password))
            {
                user.LastLoginDate = DateTime.Now;
                await UpdateAsync(user);
                return user;
            }

            return null;
        }

        private bool ValidatePassword(string inputPassword, string hashedPassword)
        {
            // Hash karşılaştırması
            return HashPassword(inputPassword) == hashedPassword;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Role Management
        public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId)
        {
            return await _context.Users
                .Where(u => u.IsActive && u.UserRoles.Any(ur => ur.RoleId == roleId && ur.IsActive))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAvailableUsersForRoleAsync(int roleId, string search = "")
        {
            // Sadece aktif kullanıcıları al ve bu role atanmamış olanları filtrele
            var query = _context.Users.Where(u => u.IsActive && 
                                                 !u.UserRoles.Any(ur => ur.RoleId == roleId && ur.IsActive));
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || 
                                       u.FirstName.Contains(search) || 
                                       u.LastName.Contains(search) || 
                                       u.Email.Contains(search));
            }
            
            return await query.ToListAsync();
        }

        public async Task<bool> AssignUserToRoleAsync(int userId, int roleId)
        {
            try
            {
                // Check if user exists and is active
                var userExists = await _context.Users
                    .AnyAsync(u => u.Id == userId && u.IsActive);
                if (!userExists) return false;

                // Check if already assigned (active assignment)
                var existingActiveAssignment = await _context.UserRoles
                    .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);
                if (existingActiveAssignment) return false;

                // Check if there's an inactive assignment (soft deleted) - reactivate it
                var existingInactiveAssignment = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsActive);
                
                if (existingInactiveAssignment != null)
                {
                    // Reactivate the existing assignment
                    existingInactiveAssignment.IsActive = true;
                    existingInactiveAssignment.AssignedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // Create new assignment
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        AssignedDate = DateTime.Now,
                        IsActive = true
                    };

                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"AssignUserToRoleAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveUserFromRoleAsync(int userId, int roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);
                if (userRole == null) return false;

                // Soft delete - set IsActive to false
                userRole.IsActive = false;
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Permission Management
        public async Task<IEnumerable<User>> GetUsersByMenuIdAsync(int menuId)
        {
            return await GetWhereAsync(u => u.UserPermissions.Any(up => up.MenuId == menuId));
        }

        public async Task<IEnumerable<User>> GetAvailableUsersForMenuPermissionAsync(int menuId, string search = "")
        {
            // Kullanıcının bu menüye direkt yetkisi var mı kontrol et
            var usersWithDirectPermission = _context.Users
                .Where(u => u.UserPermissions.Any(up => up.MenuId == menuId && up.IsActive))
                .Select(u => u.Id)
                .ToHashSet();

            // Kullanıcının bu menüye rol bazlı yetkisi var mı kontrol et
            var usersWithRolePermission = _context.UserRoles
                .Where(ur => ur.IsActive)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.MenuId == menuId && rp.IsActive)
                .Select(rp => rp.Role.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.UserId))
                .SelectMany(userIds => userIds)
                .ToHashSet();

            // Hem direkt hem rol bazlı yetkisi olmayan kullanıcıları getir
            var query = _context.Users.Where(u => u.IsActive && 
                                                 !usersWithDirectPermission.Contains(u.Id) && 
                                                 !usersWithRolePermission.Contains(u.Id));
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Username.Contains(search) || 
                                       u.FirstName.Contains(search) || 
                                       u.LastName.Contains(search) || 
                                       u.Email.Contains(search));
            }
            
            return await query.ToListAsync();
        }

        public async Task<bool> AssignUserToMenuAsync(int userId, int menuId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                // Check if already assigned
                var existingAssignment = user.UserPermissions.Any(up => up.MenuId == menuId);
                if (existingAssignment) return false;

                var userPermission = new UserPermission
                {
                    UserId = userId,
                    MenuId = menuId,
                    AssignedDate = DateTime.Now
                };

                user.UserPermissions.Add(userPermission);
                await UpdateAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveUserFromMenuAsync(int userId, int menuId)
        {
            try
            {
                var user = await GetByIdAsync(userId);
                if (user == null) return false;

                var userPermission = user.UserPermissions.FirstOrDefault(up => up.MenuId == menuId);
                if (userPermission == null) return false;

                user.UserPermissions.Remove(userPermission);
                await UpdateAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
