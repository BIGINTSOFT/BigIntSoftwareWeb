using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Menü Hiyerarşisi

        public async Task<List<Menu>> GetRootMenusAsync()
        {
            return await _context.Menus
                .Where(m => m.ParentId == null && m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetChildMenusAsync(int parentId)
        {
            return await _context.Menus
                .Where(m => m.ParentId == parentId && m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenuHierarchyAsync()
        {
            var rootMenus = await GetRootMenusAsync();
            var allMenus = new List<Menu>();

            foreach (var rootMenu in rootMenus)
            {
                allMenus.Add(rootMenu);
                await LoadChildMenusRecursive(rootMenu, allMenus);
            }

            return allMenus;
        }

        private async Task LoadChildMenusRecursive(Menu parentMenu, List<Menu> allMenus)
        {
            var childMenus = await GetChildMenusAsync(parentMenu.Id);
            foreach (var childMenu in childMenus)
            {
                allMenus.Add(childMenu);
                await LoadChildMenusRecursive(childMenu, allMenus);
            }
        }

        public async Task<List<Menu>> GetActiveMenusAsync()
        {
            return await _context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetVisibleMenusAsync()
        {
            return await _context.Menus
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        #endregion

        #region Kullanıcı Menü Yetkileri

        public async Task<List<Menu>> GetUserAccessibleMenusAsync(int userId)
        {
            // Kullanıcının rolleri üzerinden erişebileceği menüler
            var roleMenus = await _context.RoleMenus
                .Where(rm => rm.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rm => rm.IsActive)
                .Include(rm => rm.Menu)
                .Select(rm => rm.Menu)
                .ToListAsync();

            // Kullanıcının direkt erişebileceği menüler
            var userMenus = await _context.UserMenus
                .Where(um => um.UserId == userId && um.IsActive)
                .Include(um => um.Menu)
                .Select(um => um.Menu)
                .ToListAsync();

            return roleMenus.Union(userMenus).Distinct().ToList();
        }

        public async Task<List<Menu>> GetUserAccessibleMenusByPermissionAsync(int userId, string permissionLevel)
        {
            // Rol üzerinden izin kontrolü
            var roleMenus = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Select(rmp => rmp.Menu)
                .ToListAsync();

            // Kullanıcı üzerinden izin kontrolü
            var userMenus = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .Include(ump => ump.Menu)
                .Select(ump => ump.Menu)
                .ToListAsync();

            return roleMenus.Union(userMenus).Distinct().ToList();
        }

        public async Task<bool> CanUserAccessMenuAsync(int userId, int menuId, string permissionLevel)
        {
            // Rol üzerinden kontrol
            var roleAccess = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.MenuId == menuId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .AnyAsync();

            if (roleAccess)
                return true;

            // Kullanıcı üzerinden kontrol
            var userAccess = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.MenuId == menuId && ump.PermissionLevel == permissionLevel && ump.IsActive)
                .AnyAsync();

            return userAccess;
        }

        public async Task<List<UserMenu>> GetUserMenusAsync(int userId)
        {
            return await _context.UserMenus
                .Where(um => um.UserId == userId && um.IsActive)
                .Include(um => um.Menu)
                .ToListAsync();
        }

        public async Task<List<UserMenuPermission>> GetUserMenuPermissionsAsync(int userId)
        {
            return await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .Include(ump => ump.Menu)
                .Include(ump => ump.Permission)
                .ToListAsync();
        }

        #endregion

        #region Rol Menü Yetkileri

        public async Task<List<Menu>> GetRoleAccessibleMenusAsync(int roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId && rm.IsActive)
                .Include(rm => rm.Menu)
                .Select(rm => rm.Menu)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetRoleAccessibleMenusByPermissionAsync(int roleId, string permissionLevel)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Select(rmp => rmp.Menu)
                .ToListAsync();
        }

        public async Task<List<RoleMenu>> GetRoleMenusAsync(int roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId && rm.IsActive)
                .Include(rm => rm.Menu)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        #endregion

        #region Menü Yetki Kontrolü

        public async Task<List<string>> GetMenuPermissionLevelsAsync(int menuId, int userId)
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

        public async Task<Dictionary<int, List<string>>> GetUserMenuPermissionLevelsAsync(int userId)
        {
            var rolePermissions = await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive))
                .Where(rmp => rmp.IsActive)
                .GroupBy(rmp => rmp.MenuId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(rmp => rmp.PermissionLevel).ToList());

            var userPermissions = await _context.UserMenuPermissions
                .Where(ump => ump.UserId == userId && ump.IsActive)
                .GroupBy(ump => ump.MenuId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(ump => ump.PermissionLevel).ToList());

            // İki dictionary'yi birleştir
            var result = new Dictionary<int, List<string>>();
            foreach (var kvp in rolePermissions)
            {
                result[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in userPermissions)
            {
                if (result.ContainsKey(kvp.Key))
                {
                    result[kvp.Key] = result[kvp.Key].Union(kvp.Value).Distinct().ToList();
                }
                else
                {
                    result[kvp.Key] = kvp.Value;
                }
            }

            return result;
        }

        public async Task<List<Permission>> GetMenuPermissionsAsync(int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Permission)
                .Select(rmp => rmp.Permission)
                .Union(_context.UserMenuPermissions
                    .Where(ump => ump.MenuId == menuId && ump.IsActive)
                    .Include(ump => ump.Permission)
                    .Select(ump => ump.Permission))
                .Distinct()
                .ToListAsync();
        }

        #endregion

        #region Menü Arama ve Filtreleme

        public async Task<List<Menu>> SearchMenusAsync(string searchTerm)
        {
            return await _context.Menus
                .Where(m => m.Name.Contains(searchTerm) || 
                           m.Description.Contains(searchTerm) ||
                           m.Controller.Contains(searchTerm) ||
                           m.Action.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenusByControllerAsync(string controller)
        {
            return await _context.Menus
                .Where(m => m.Controller == controller)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenusByActionAsync(string controller, string action)
        {
            return await _context.Menus
                .Where(m => m.Controller == controller && m.Action == action)
                .ToListAsync();
        }

        public async Task<Menu?> GetMenuByRouteAsync(string controller, string action)
        {
            return await _context.Menus
                .FirstOrDefaultAsync(m => m.Controller == controller && m.Action == action);
        }

        public async Task<Menu?> GetByControllerActionAsync(string controller, string action)
        {
            return await GetMenuByRouteAsync(controller, action);
        }

        #endregion

        #region Menü İstatistikleri

        public async Task<int> GetMenuUserCountAsync(int menuId)
        {
            var roleUsers = await _context.RoleMenus
                .Where(rm => rm.MenuId == menuId && rm.IsActive)
                .SelectMany(rm => rm.Role.UserRoles.Where(ur => ur.IsActive))
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync();

            var directUsers = await _context.UserMenus
                .Where(um => um.MenuId == menuId && um.IsActive)
                .Select(um => um.UserId)
                .Distinct()
                .CountAsync();

            return roleUsers + directUsers;
        }

        public async Task<int> GetMenuRoleCountAsync(int menuId)
        {
            return await _context.RoleMenus.CountAsync(rm => rm.MenuId == menuId && rm.IsActive);
        }

        public async Task<int> GetMenuPermissionCountAsync(int menuId)
        {
            var rolePermissions = await _context.RoleMenuPermissions
                .CountAsync(rmp => rmp.MenuId == menuId && rmp.IsActive);

            var userPermissions = await _context.UserMenuPermissions
                .CountAsync(ump => ump.MenuId == menuId && ump.IsActive);

            return rolePermissions + userPermissions;
        }

        public async Task<int> GetChildMenuCountAsync(int parentId)
        {
            return await _context.Menus.CountAsync(m => m.ParentId == parentId && m.IsActive);
        }

        #endregion

        #region Menü Bilgileri

        public async Task<Menu?> GetMenuWithPermissionsAsync(int menuId)
        {
            return await _context.Menus
                .Include(m => m.RoleMenuPermissions)
                    .ThenInclude(rmp => rmp.Permission)
                .Include(m => m.UserMenuPermissions)
                    .ThenInclude(ump => ump.Permission)
                .FirstOrDefaultAsync(m => m.Id == menuId);
        }

        public async Task<Menu?> GetMenuWithUsersAsync(int menuId)
        {
            return await _context.Menus
                .Include(m => m.UserMenus)
                    .ThenInclude(um => um.User)
                .Include(m => m.RoleMenus)
                    .ThenInclude(rm => rm.Role)
                    .ThenInclude(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(m => m.Id == menuId);
        }

        public async Task<Menu?> GetMenuWithRolesAsync(int menuId)
        {
            return await _context.Menus
                .Include(m => m.RoleMenus)
                    .ThenInclude(rm => rm.Role)
                .FirstOrDefaultAsync(m => m.Id == menuId);
        }

        public async Task<List<Menu>> GetMenuWithChildrenAsync(int menuId)
        {
            var menu = await _context.Menus
                .Include(m => m.Children)
                .FirstOrDefaultAsync(m => m.Id == menuId);

            return menu?.Children.ToList() ?? new List<Menu>();
        }

        #endregion
    }
}
