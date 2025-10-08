using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class RoleMenuRepository : GenericRepository<RoleMenu>, IRoleMenuRepository
    {
        public RoleMenuRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Rol-Menü İlişkileri

        public async Task<List<RoleMenu>> GetRoleMenusAsync(int roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId && rm.IsActive)
                .Include(rm => rm.Menu)
                .Include(rm => rm.Role)
                .Include(rm => rm.AssignedByUser)
                .ToListAsync();
        }

        public async Task<List<RoleMenu>> GetMenuRolesAsync(int menuId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.MenuId == menuId && rm.IsActive)
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .Include(rm => rm.AssignedByUser)
                .ToListAsync();
        }

        public async Task<RoleMenu?> GetRoleMenuAsync(int roleId, int menuId)
        {
            return await _context.RoleMenus
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .Include(rm => rm.AssignedByUser)
                .FirstOrDefaultAsync(rm => rm.RoleId == roleId && rm.MenuId == menuId);
        }

        #endregion

        #region Aktif İlişkiler

        public async Task<List<RoleMenu>> GetActiveRoleMenusAsync(int roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId && rm.IsActive)
                .Include(rm => rm.Menu)
                .ToListAsync();
        }

        public async Task<List<RoleMenu>> GetActiveMenuRolesAsync(int menuId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.MenuId == menuId && rm.IsActive)
                .Include(rm => rm.Role)
                .ToListAsync();
        }

        #endregion

        #region İstatistikler

        public async Task<int> GetRoleMenuCountAsync(int roleId)
        {
            return await _context.RoleMenus.CountAsync(rm => rm.RoleId == roleId && rm.IsActive);
        }

        public async Task<int> GetMenuRoleCountAsync(int menuId)
        {
            return await _context.RoleMenus.CountAsync(rm => rm.MenuId == menuId && rm.IsActive);
        }

        public async Task<int> GetActiveRoleMenuCountAsync(int roleId)
        {
            return await _context.RoleMenus.CountAsync(rm => rm.RoleId == roleId && rm.IsActive);
        }

        #endregion

        #region Arama ve Filtreleme

        public async Task<List<RoleMenu>> SearchRoleMenusAsync(string searchTerm)
        {
            return await _context.RoleMenus
                .Where(rm => rm.Role.Name.Contains(searchTerm) ||
                           rm.Role.Description.Contains(searchTerm) ||
                           rm.Menu.Name.Contains(searchTerm) ||
                           rm.Menu.Description.Contains(searchTerm) ||
                           rm.Notes.Contains(searchTerm))
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .ToListAsync();
        }

        public async Task<List<RoleMenu>> GetRoleMenusByAssignedByAsync(int assignedBy)
        {
            return await _context.RoleMenus
                .Where(rm => rm.AssignedBy == assignedBy)
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .Include(rm => rm.AssignedByUser)
                .ToListAsync();
        }

        #endregion
    }
}
