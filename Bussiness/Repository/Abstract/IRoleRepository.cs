using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        // Rol-Menü-Yetki İşlemleri
        Task<bool> AssignMenuPermissionToRoleAsync(int roleId, int menuId, string permissionLevel, int assignedBy, string? notes = null);
        Task<bool> RemoveMenuPermissionFromRoleAsync(int roleId, int menuId);
        Task<bool> RemoveMenuPermissionFromRoleByIdAsync(int roleId, int permissionId);
        Task<bool> UpdateRoleMenuPermissionAsync(int roleId, int menuId, string newPermissionLevel, int assignedBy, string? notes = null);
        
        // Rol Yetkilerini Getirme
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId);
        Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByMenuAsync(int menuId);
        Task<RoleMenuPermission?> GetRoleMenuPermissionAsync(int roleId, int menuId);
        
        // Rol Kullanıcıları
        Task<List<User>> GetRoleUsersAsync(int roleId);
        Task<int> GetRoleUserCountAsync(int roleId);
        
        // Rol Arama ve Filtreleme
        Task<List<Role>> GetAvailableRolesForUserAsync(int userId, string? search = null);
        Task<List<Role>> GetAvailableRolesForMenuAsync(int menuId, string? search = null);
        Task<List<Menu>> GetAvailableMenusForRolePermissionAsync(int roleId, string? search = null);
        
        // Rol Bilgileri
        Task<Role?> GetRoleWithMenuPermissionsAsync(int roleId);
        Task<List<Role>> GetActiveRolesAsync();
    }
}
