using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IRoleMenuPermissionRepository : IGenericRepository<RoleMenuPermission>
    {
        // Rol-Menü-İzin İlişkileri
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId);
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByMenuAsync(int roleId, int menuId);
        Task<List<RoleMenuPermission>> GetMenuRolePermissionsAsync(int menuId);
        Task<List<RoleMenuPermission>> GetPermissionRoleMenusAsync(int permissionId);
        Task<RoleMenuPermission?> GetRoleMenuPermissionAsync(int roleId, int menuId, int permissionId);
        
        // İzin Seviyesi Kontrolü
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByLevelAsync(int roleId, string permissionLevel);
        Task<List<RoleMenuPermission>> GetMenuRolePermissionsByLevelAsync(int menuId, string permissionLevel);
        
        // Aktif İlişkiler
        Task<List<RoleMenuPermission>> GetActiveRoleMenuPermissionsAsync(int roleId);
        Task<List<RoleMenuPermission>> GetActiveMenuRolePermissionsAsync(int menuId);
        
        // Yetki Kontrolü
        Task<bool> HasRoleMenuPermissionAsync(int roleId, int menuId, string permissionLevel);
        Task<List<string>> GetRoleMenuPermissionLevelsAsync(int roleId, int menuId);
        
        // İstatistikler
        Task<int> GetRoleMenuPermissionCountAsync(int roleId);
        Task<int> GetMenuRolePermissionCountAsync(int menuId);
        Task<int> GetPermissionRoleMenuCountAsync(int permissionId);
        Task<int> GetActiveRoleMenuPermissionCountAsync(int roleId);
        
        // Arama ve Filtreleme
        Task<List<RoleMenuPermission>> SearchRoleMenuPermissionsAsync(string searchTerm);
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByAssignedByAsync(int assignedBy);
    }
}
