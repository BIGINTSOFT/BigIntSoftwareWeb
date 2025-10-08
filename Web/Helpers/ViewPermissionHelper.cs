using Bussiness.Repository.Abstract;
using System.Security.Claims;

namespace Web.Helpers
{
    public static class ViewPermissionHelper
    {
        public static async Task<bool> HasPermissionAsync(this IServiceProvider serviceProvider, 
            ClaimsPrincipal user, string permissionCode)
        {
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var menuRepository = serviceProvider.GetRequiredService<IMenuRepository>();
            
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return false;

            // Controller ve Action'ı permission code'dan çıkar
            var parts = permissionCode.Split('_');
            if (parts.Length < 2) return false;
            
            var controller = parts[0];
            var action = parts[1];
            var permissionLevel = parts.Length > 2 ? parts[2] : "VIEW";

            var menu = await menuRepository.GetByControllerActionAsync(controller, action);
            if (menu == null) return false;

            return await userRepository.HasPermissionAsync(userId, menu.Id, permissionLevel);
        }

        public static async Task<int?> ResolveMenuIdAsync(this IServiceProvider serviceProvider,
            string controller, string action)
        {
            var menuRepository = serviceProvider.GetRequiredService<IMenuRepository>();
            var menu = await menuRepository.GetByControllerActionAsync(controller, action);
            return menu?.Id;
        }

        public static async Task<bool> HasPermissionByRouteAsync(this IServiceProvider serviceProvider,
            ClaimsPrincipal user, string permissionCode, string controller, string action)
        {
            return await serviceProvider.HasPermissionAsync(user, permissionCode);
        }

        public static async Task<IEnumerable<string>> GetUserPermissionsAsync(this IServiceProvider serviceProvider, 
            ClaimsPrincipal user)
        {
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Enumerable.Empty<string>();

            // Kullanıcının tüm menü yetkilerini getir
            var menuPermissions = await userRepository.GetUserPermissionLevelsAsync(userId, 0);
            return menuPermissions;
        }
    }
}
