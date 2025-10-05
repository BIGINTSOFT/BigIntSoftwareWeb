using DataAccess.DbContext;
using Entities.Entity;
using Entities.Dto;
using Bussiness.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Repository.Concrete
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Menu>> GetRootMenusAsync()
        {
            return await GetWhereAsync(m => m.ParentId == null && m.IsActive && m.IsVisible)
                .ContinueWith(t => t.Result.OrderBy(m => m.SortOrder));
        }

        public async Task<IEnumerable<Menu>> GetChildMenusAsync(int parentId)
        {
            return await GetWhereAsync(m => m.ParentId == parentId && m.IsActive && m.IsVisible)
                .ContinueWith(t => t.Result.OrderBy(m => m.SortOrder));
        }

        public async Task<IEnumerable<Menu>> GetUserMenusAsync(int userId)
        {
            // Get menus from user's roles - daha detaylı sorgu
            var roleMenus = await (from ur in _context.UserRoles
                                 where ur.UserId == userId && ur.IsActive
                                 from rp in ur.Role.RolePermissions
                                 where rp.IsActive && rp.MenuId.HasValue
                                 select rp.Menu)
                                 .Where(m => m.IsActive && m.IsVisible)
                                 .Distinct()
                                 .ToListAsync();

            // Get menus from direct user permissions
            var userMenus = await (from up in _context.UserPermissions
                                 where up.UserId == userId && up.IsActive && up.MenuId.HasValue
                                 select up.Menu)
                                 .Where(m => m.IsActive && m.IsVisible)
                                 .Distinct()
                                 .ToListAsync();

            // Combine and return unique menus
            return roleMenus.Union(userMenus).OrderBy(m => m.SortOrder);
        }

        public async Task<IEnumerable<User>> GetUsersByMenuIdAsync(int menuId)
        {
            // Get users from role permissions
            var roleUsers = await (from rp in _context.RolePermissions
                                 where rp.MenuId == menuId && rp.IsActive
                                 from ur in rp.Role.UserRoles
                                 where ur.IsActive
                                 select ur.User)
                                 .Where(u => u.IsActive)
                                 .Distinct()
                                 .ToListAsync();

            // Get users from direct permissions
            var directUsers = await (from up in _context.UserPermissions
                                   where up.MenuId == menuId && up.IsActive
                                   select up.User)
                                   .Where(u => u.IsActive)
                                   .Distinct()
                                   .ToListAsync();

            // Combine and return unique users
            return roleUsers.Union(directUsers);
        }

        public async Task<IEnumerable<UserWithSource>> GetUsersWithSourceByMenuIdAsync(int menuId)
        {
            // Get users from role permissions
            var roleUsers = await (from rp in _context.RolePermissions
                                 where rp.MenuId == menuId && rp.IsActive
                                 from ur in rp.Role.UserRoles
                                 where ur.IsActive && ur.User.IsActive
                                 select new UserWithSource { User = ur.User, Source = "Role" })
                                 .Distinct()
                                 .ToListAsync();

            // Get users from direct permissions
            var directUsers = await (from up in _context.UserPermissions
                                   where up.MenuId == menuId && up.IsActive
                                   where up.User.IsActive
                                   select new UserWithSource { User = up.User, Source = "Direct" })
                                   .Distinct()
                                   .ToListAsync();

            // Combine and return unique users with source
            return roleUsers.Union(directUsers);
        }

        public async Task<IEnumerable<Menu>> GetRoleMenusAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive && rp.MenuId.HasValue)
                .Select(rp => rp.Menu!)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<Menu?> GetByControllerActionAsync(string controller, string action)
        {
            return await GetFirstOrDefaultAsync(m => 
                m.Controller == controller && m.Action == action);
        }

        public async Task<IEnumerable<Menu>> GetVisibleMenusAsync()
        {
            return await GetWhereAsync(m => m.IsActive && m.IsVisible)
                .ContinueWith(t => t.Result.OrderBy(m => m.SortOrder));
        }

        // User Menu Management
        public async Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId)
        {
            return await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive && up.MenuId.HasValue)
                .Select(up => up.Menu!)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetAvailableMenusForUserAsync(int userId, string search = "")
        {
            var query = _context.Menus.Where(m => !m.UserPermissions.Any(up => up.UserId == userId));
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Name.Contains(search) || m.Description.Contains(search));
            }
            
            return await query.Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetUserDirectMenusAsync(int userId)
        {
            return await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsActive && up.MenuId.HasValue)
                .Select(up => up.Menu!)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetUserRoleMenusAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.IsActive && rp.MenuId.HasValue)
                .Select(rp => rp.Menu!)
                .Where(m => m.IsActive && m.IsVisible)
                .Distinct()
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }
    }
}
