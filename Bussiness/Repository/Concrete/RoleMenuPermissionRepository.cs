using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class RoleMenuPermissionRepository : GenericRepository<RoleMenuPermission>, IRoleMenuPermissionRepository
    {
        public RoleMenuPermissionRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        #region Rol-Menü-İzin İlişkileri

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.AssignedByUser)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByMenuAsync(int roleId, int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Permission)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Menu)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetMenuRolePermissionsAsync(int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Permission)
                .Include(rmp => rmp.Menu)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetPermissionRoleMenusAsync(int permissionId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.PermissionId == permissionId && rmp.IsActive)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<RoleMenuPermission?> GetRoleMenuPermissionAsync(int roleId, int menuId, int permissionId)
        {
            return await _context.RoleMenuPermissions
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .Include(rmp => rmp.AssignedByUser)
                .FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.PermissionId == permissionId);
        }

        #endregion

        #region İzin Seviyesi Kontrolü

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByLevelAsync(int roleId, string permissionLevel)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetMenuRolePermissionsByLevelAsync(int menuId, string permissionLevel)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.MenuId == menuId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        #endregion

        #region Aktif İlişkiler

        public async Task<List<RoleMenuPermission>> GetActiveRoleMenuPermissionsAsync(int roleId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.IsActive)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetActiveMenuRolePermissionsAsync(int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.MenuId == menuId && rmp.IsActive)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        #endregion

        #region Yetki Kontrolü

        public async Task<bool> HasRoleMenuPermissionAsync(int roleId, int menuId, string permissionLevel)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.PermissionLevel == permissionLevel && rmp.IsActive)
                .AnyAsync();
        }

        public async Task<List<string>> GetRoleMenuPermissionLevelsAsync(int roleId, int menuId)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId && rmp.IsActive)
                .Select(rmp => rmp.PermissionLevel)
                .Distinct()
                .ToListAsync();
        }

        #endregion

        #region İstatistikler

        public async Task<int> GetRoleMenuPermissionCountAsync(int roleId)
        {
            return await _context.RoleMenuPermissions.CountAsync(rmp => rmp.RoleId == roleId && rmp.IsActive);
        }

        public async Task<int> GetMenuRolePermissionCountAsync(int menuId)
        {
            return await _context.RoleMenuPermissions.CountAsync(rmp => rmp.MenuId == menuId && rmp.IsActive);
        }

        public async Task<int> GetPermissionRoleMenuCountAsync(int permissionId)
        {
            return await _context.RoleMenuPermissions.CountAsync(rmp => rmp.PermissionId == permissionId && rmp.IsActive);
        }

        public async Task<int> GetActiveRoleMenuPermissionCountAsync(int roleId)
        {
            return await _context.RoleMenuPermissions.CountAsync(rmp => rmp.RoleId == roleId && rmp.IsActive);
        }

        #endregion

        #region Arama ve Filtreleme

        public async Task<List<RoleMenuPermission>> SearchRoleMenuPermissionsAsync(string searchTerm)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.Role.Name.Contains(searchTerm) ||
                           rmp.Role.Description.Contains(searchTerm) ||
                           rmp.Menu.Name.Contains(searchTerm) ||
                           rmp.Menu.Description.Contains(searchTerm) ||
                           rmp.Permission.Name.Contains(searchTerm) ||
                           rmp.PermissionLevel.Contains(searchTerm) ||
                           rmp.Notes.Contains(searchTerm))
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .ToListAsync();
        }

        public async Task<List<RoleMenuPermission>> GetRoleMenuPermissionsByAssignedByAsync(int assignedBy)
        {
            return await _context.RoleMenuPermissions
                .Where(rmp => rmp.AssignedBy == assignedBy)
                .Include(rmp => rmp.Role)
                .Include(rmp => rmp.Menu)
                .Include(rmp => rmp.Permission)
                .Include(rmp => rmp.AssignedByUser)
                .ToListAsync();
        }

        #endregion
    }
}
