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
        
        // Kullanıcı Menü Yetkileri
        Task<List<Menu>> GetUserAccessibleMenusAsync(int userId);
        Task<List<Menu>> GetUserAccessibleMenusByPermissionAsync(int userId, string permissionLevel);
        Task<bool> CanUserAccessMenuAsync(int userId, int menuId, string permissionLevel);
        
        // Rol Menü Yetkileri
        Task<List<Menu>> GetRoleAccessibleMenusAsync(int roleId);
        Task<List<Menu>> GetRoleAccessibleMenusByPermissionAsync(int roleId, string permissionLevel);
        
        // Menü Yetki Kontrolü
        Task<List<string>> GetMenuPermissionLevelsAsync(int menuId, int userId);
        Task<Dictionary<int, List<string>>> GetUserMenuPermissionsAsync(int userId);
        
        // Menü Arama ve Filtreleme
        Task<List<Menu>> SearchMenusAsync(string searchTerm);
        Task<List<Menu>> GetMenusByControllerAsync(string controller);
        Task<List<Menu>> GetMenusByActionAsync(string controller, string action);
        
        // Menü İstatistikleri
        Task<int> GetMenuUserCountAsync(int menuId);
        Task<int> GetMenuRoleCountAsync(int menuId);
        
        // Menü Bilgileri
        Task<Menu?> GetMenuWithPermissionsAsync(int menuId);
        Task<Menu?> GetMenuByRouteAsync(string controller, string action);
        Task<List<Menu>> GetUserMenusAsync(int userId);
        Task<Menu?> GetByControllerActionAsync(string controller, string action);
    }
}
