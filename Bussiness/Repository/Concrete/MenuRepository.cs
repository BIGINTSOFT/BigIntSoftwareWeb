using Bussiness.Repository.Abstract;
using DataAccess.DbContext;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

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
                .Where(m => m.ParentId == null && m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetChildMenusAsync(int parentId)
        {
            return await _context.Menus
                .Where(m => m.ParentId == parentId && m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenuHierarchyAsync()
        {
            return await _context.Menus
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetActiveMenusAsync()
        {
            return await _context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        #endregion

        #region Kullanıcı Menü Yetkileri

        public async Task<List<Menu>> GetUserAccessibleMenusAsync(int userId)
        {
            // Kullanıcının ekstra yetkileri
            var extraPermissionMenus = await _context.UserExtraPermissions
                .Where(uep => uep.UserId == userId && 
                             uep.IsActive &&
                             (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now))
                .Include(uep => uep.Menu)
                .Select(uep => uep.Menu)
                .ToListAsync();

            // Kullanıcının rol yetkileri
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var rolePermissionMenus = new List<Menu>();
            if (userRoles.Any())
            {
                rolePermissionMenus = await _context.RoleMenuPermissions
                    .Where(rmp => userRoles.Contains(rmp.RoleId) && rmp.IsActive)
                    .Include(rmp => rmp.Menu)
                    .Select(rmp => rmp.Menu)
                    .ToListAsync();
            }

            // Birleştir ve tekrarları kaldır
            var allMenus = extraPermissionMenus.Union(rolePermissionMenus)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToList();

            return allMenus;
        }

        public async Task<List<Menu>> GetUserAccessibleMenusByPermissionAsync(int userId, string permissionLevel)
        {
            // Kullanıcının ekstra yetkileri
            var extraPermissionMenus = await _context.UserExtraPermissions
                .Where(uep => uep.UserId == userId && 
                             uep.PermissionLevel == permissionLevel &&
                             uep.IsActive &&
                             (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now))
                .Include(uep => uep.Menu)
                .Select(uep => uep.Menu)
                .ToListAsync();

            // Kullanıcının rol yetkileri
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var rolePermissionMenus = new List<Menu>();
            if (userRoles.Any())
            {
                rolePermissionMenus = await _context.RoleMenuPermissions
                    .Where(rmp => userRoles.Contains(rmp.RoleId) && 
                                 rmp.PermissionLevel == permissionLevel && 
                                 rmp.IsActive)
                    .Include(rmp => rmp.Menu)
                    .Select(rmp => rmp.Menu)
                    .ToListAsync();
            }

            // Birleştir ve tekrarları kaldır
            var allMenus = extraPermissionMenus.Union(rolePermissionMenus)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToList();

            return allMenus;
        }

        public async Task<bool> CanUserAccessMenuAsync(int userId, int menuId, string permissionLevel)
        {
            // Ekstra yetki kontrolü
            var hasExtraPermission = await _context.UserExtraPermissions
                .AnyAsync(uep => uep.UserId == userId && 
                                uep.MenuId == menuId && 
                                uep.PermissionLevel == permissionLevel && 
                                uep.IsActive &&
                                (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now));

            if (hasExtraPermission)
                return true;

            // Rol yetki kontrolü
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

        #endregion

        #region Rol Menü Yetkileri

        public async Task<List<Menu>> GetRoleAccessibleMenusAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Select(rmp => rmp.Menu)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetRoleAccessibleMenusByPermissionAsync(int roleId, string permissionLevel)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && 
                             rmp.PermissionLevel == permissionLevel && 
                             rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Select(rmp => rmp.Menu)
                .Where(m => m.IsActive && m.IsVisible)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        #endregion

        #region Menü Yetki Kontrolü

        public async Task<List<string>> GetMenuPermissionLevelsAsync(int menuId, int userId)
        {
            var permissionLevels = new List<string>();

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

            return permissionLevels.Distinct().ToList();
        }

        public async Task<Dictionary<int, List<string>>> GetUserMenuPermissionsAsync(int userId)
        {
            var result = new Dictionary<int, List<string>>();

            // Ekstra yetkiler
            var extraPermissions = await _context.UserExtraPermissions
                .Where(uep => uep.UserId == userId && 
                             uep.IsActive &&
                             (uep.ExpiryDate == null || uep.ExpiryDate > DateTime.Now))
                .ToListAsync();

            foreach (var ep in extraPermissions)
            {
                if (!result.ContainsKey(ep.MenuId))
                    result[ep.MenuId] = new List<string>();
                result[ep.MenuId].Add(ep.PermissionLevel);
            }

            // Rol yetkileri
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (userRoles.Any())
            {
                var rolePermissions = await _context.RoleMenuPermissions
                    .Where(rmp => userRoles.Contains(rmp.RoleId) && rmp.IsActive)
                    .ToListAsync();

                foreach (var rp in rolePermissions)
                {
                    if (!result.ContainsKey(rp.MenuId))
                        result[rp.MenuId] = new List<string>();
                    result[rp.MenuId].Add(rp.PermissionLevel);
                }
            }

            // Tekrarları kaldır
            foreach (var key in result.Keys.ToList())
            {
                result[key] = result[key].Distinct().ToList();
            }

            return result;
        }

        #endregion

        #region Menü Arama ve Filtreleme

        public async Task<List<Menu>> SearchMenusAsync(string searchTerm)
        {
            return await _context.Menus
                .Where(m => m.IsActive && 
                           (m.Name.Contains(searchTerm) || 
                            (m.Description != null && m.Description.Contains(searchTerm)) ||
                            (m.Controller != null && m.Controller.Contains(searchTerm)) ||
                            (m.Action != null && m.Action.Contains(searchTerm))))
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenusByControllerAsync(string controller)
        {
            return await _context.Menus
                .Where(m => m.Controller == controller && m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        public async Task<List<Menu>> GetMenusByActionAsync(string controller, string action)
        {
            return await _context.Menus
                .Where(m => m.Controller == controller && m.Action == action && m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
        }

        #endregion

        #region Menü İstatistikleri

        public async Task<int> GetMenuUserCountAsync(int menuId)
        {
            // Ekstra yetki sahibi kullanıcılar
            var extraUserCount = await _context.UserExtraPermissions
                .CountAsync(uep => uep.MenuId == menuId && uep.IsActive);

            // Rol yetkisi sahibi kullanıcılar
            var roleUserCount = await _context.RoleMenuPermissions
                .Where(rmp => rmp.MenuId == menuId && rmp.IsActive)
                .SelectMany(rmp => _context.UserRoles.Where(ur => ur.RoleId == rmp.RoleId && ur.IsActive))
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync();

            return extraUserCount + roleUserCount;
        }

        public async Task<int> GetMenuRoleCountAsync(int menuId)
        {
            return await _context.RoleMenuPermissions
                .CountAsync(rmp => rmp.MenuId == menuId && rmp.IsActive);
        }

        #endregion

        #region Menü Bilgileri

        public async Task<Menu?> GetMenuWithPermissionsAsync(int menuId)
        {
            return await _context.Menus
                .Include(m => m.RoleMenuPermissions.Where(rmp => rmp.IsActive))
                    .ThenInclude(rmp => rmp.Role)
                .Include(m => m.UserExtraPermissions.Where(uep => uep.IsActive))
                    .ThenInclude(uep => uep.User)
                .FirstOrDefaultAsync(m => m.Id == menuId);
        }

        public async Task<Menu?> GetMenuByRouteAsync(string controller, string action)
        {
            return await _context.Menus
                .FirstOrDefaultAsync(m => m.Controller == controller && m.Action == action && m.IsActive);
        }

        public async Task<List<Menu>> GetUserMenusAsync(int userId)
        {
            return await GetUserAccessibleMenusAsync(userId);
        }

        public async Task<Menu?> GetByControllerActionAsync(string controller, string action)
        {
            return await GetMenuByRouteAsync(controller, action);
        }

        #endregion
    }
}
