using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        // Rol-Menü İşlemleri
        Task<bool> AssignMenuToRoleAsync(int roleId, int menuId, int assignedBy, string? notes = null);
        Task<bool> RemoveMenuFromRoleAsync(int roleId, int menuId);
        Task<List<Menu>> GetRoleMenusAsync(int roleId);
        
        // Rol-Menü-İzin İşlemleri
        Task<bool> AssignMenuPermissionToRoleAsync(int roleId, int menuId, int permissionId, string permissionLevel, int assignedBy, string? notes = null);
        Task<bool> RemoveMenuPermissionFromRoleAsync(int roleId, int menuId, int permissionId);
        Task<bool> UpdateRoleMenuPermissionAsync(int roleId, int menuId, int permissionId, string newPermissionLevel, int assignedBy, string? notes = null);
        
        // Rol Yetkilerini Getirme
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId);
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByMenuAsync(int roleId, int menuId);
        Task<RoleMenuPermission?> GetRoleMenuPermissionAsync(int roleId, int menuId, int permissionId);
        
        // Rol Kullanıcıları
        Task<List<User>> GetRoleUsersAsync(int roleId);
        Task<int> GetRoleUserCountAsync(int roleId);
        
        // Rol Arama ve Filtreleme
        Task<List<Role>> GetAvailableRolesForUserAsync(int userId, string? search = null);
        Task<List<Role>> GetAvailableRolesForMenuAsync(int menuId, string? search = null);
        Task<List<Role>> SearchRolesAsync(string searchTerm);
        Task<List<Role>> GetActiveRolesAsync();
        
        // Rol Bilgileri
        Task<Role?> GetRoleWithMenusAsync(int roleId);
        Task<Role?> GetRoleWithPermissionsAsync(int roleId);
        Task<Role?> GetRoleWithUsersAsync(int roleId);
        
        // Rol İstatistikleri
        Task<int> GetRoleCountAsync();
        Task<int> GetActiveRoleCountAsync();
        Task<int> GetRoleMenuCountAsync(int roleId);
        Task<int> GetRolePermissionCountAsync(int roleId);
    }
}
