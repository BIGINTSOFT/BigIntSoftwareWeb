using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface IRoleMenuRepository : IGenericRepository<RoleMenu>
    {
        // Rol-Menü İlişkileri
        Task<List<RoleMenu>> GetRoleMenusAsync(int roleId);
        Task<List<RoleMenu>> GetMenuRolesAsync(int menuId);
        Task<RoleMenu?> GetRoleMenuAsync(int roleId, int menuId);
        
        // Aktif İlişkiler
        Task<List<RoleMenu>> GetActiveRoleMenusAsync(int roleId);
        Task<List<RoleMenu>> GetActiveMenuRolesAsync(int menuId);
        
        // İstatistikler
        Task<int> GetRoleMenuCountAsync(int roleId);
        Task<int> GetMenuRoleCountAsync(int menuId);
        Task<int> GetActiveRoleMenuCountAsync(int roleId);
        
        // Arama ve Filtreleme
        Task<List<RoleMenu>> SearchRoleMenusAsync(string searchTerm);
        Task<List<RoleMenu>> GetRoleMenusByAssignedByAsync(int assignedBy);
    }
}
