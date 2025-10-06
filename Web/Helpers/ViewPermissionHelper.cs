using Bussiness.Repository.Abstract;
using System.Security.Claims;

namespace Web.Helpers
{
    public static class ViewPermissionHelper
    {
        public static async Task<bool> HasPermissionAsync(this IServiceProvider serviceProvider, 
            ClaimsPrincipal user, string permissionCode, int? menuId = null)
        {
            var permissionRepository = serviceProvider.GetRequiredService<IPermissionRepository>();
            
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return false;

            return await permissionRepository.HasPermissionAsync(userId, permissionCode, menuId);
        }

        public static async Task<IEnumerable<string>> GetUserPermissionsAsync(this IServiceProvider serviceProvider, 
            ClaimsPrincipal user, int? menuId = null)
        {
            var permissionRepository = serviceProvider.GetRequiredService<IPermissionRepository>();
            
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Enumerable.Empty<string>();

            return await permissionRepository.GetUserPermissionCodesAsync(userId, menuId);
        }
    }
}
