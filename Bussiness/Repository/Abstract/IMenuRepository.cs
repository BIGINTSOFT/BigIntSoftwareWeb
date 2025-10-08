using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        // Menü Hiyerarşisi
        Task<List<Menu>> GetRootMenusAsync();
        Task<List<Menu>> GetChildMenusAsync(int parentId);
        Task<List<Menu>> GetMenuHierarchyAsync();
        Task<List<Menu>> GetActiveMenusAsync();
        Task<List<Menu>> GetVisibleMenusAsync();
        
        // Kullanıcı Menü Yetkileri
        Task<List<Menu>> GetUserAccessibleMenusAsync(int userId);
        Task<List<Menu>> GetUserAccessibleMenusByPermissionAsync(int userId, string permissionLevel);
        Task<bool> CanUserAccessMenuAsync(int userId, int menuId, string permissionLevel);
        Task<List<UserMenu>> GetUserMenusAsync(int userId);
        Task<List<UserMenuPermission>> GetUserMenuPermissionsAsync(int userId);
        
        // Rol Menü Yetkileri
        Task<List<Menu>> GetRoleAccessibleMenusAsync(int roleId);
        Task<List<Menu>> GetRoleAccessibleMenusByPermissionAsync(int roleId, string permissionLevel);
        Task<List<RoleMenu>> GetRoleMenusAsync(int roleId);
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId);
        
        // Menü Yetki Kontrolü
        Task<List<string>> GetMenuPermissionLevelsAsync(int menuId, int userId);
        Task<Dictionary<int, List<string>>> GetUserMenuPermissionLevelsAsync(int userId);
        Task<List<Permission>> GetMenuPermissionsAsync(int menuId);
        
        // Menü Arama ve Filtreleme
        Task<List<Menu>> SearchMenusAsync(string searchTerm);
        Task<List<Menu>> GetMenusByControllerAsync(string controller);
        Task<List<Menu>> GetMenusByActionAsync(string controller, string action);
        Task<Menu?> GetMenuByRouteAsync(string controller, string action);
        Task<Menu?> GetByControllerActionAsync(string controller, string action);
        
        // Menü İstatistikleri
        Task<int> GetMenuUserCountAsync(int menuId);
        Task<int> GetMenuRoleCountAsync(int menuId);
        Task<int> GetMenuPermissionCountAsync(int menuId);
        Task<int> GetChildMenuCountAsync(int parentId);
        
        // Menü Bilgileri
        Task<Menu?> GetMenuWithPermissionsAsync(int menuId);
        Task<Menu?> GetMenuWithUsersAsync(int menuId);
        Task<Menu?> GetMenuWithRolesAsync(int menuId);
        Task<List<Menu>> GetMenuWithChildrenAsync(int menuId);
    }
}
