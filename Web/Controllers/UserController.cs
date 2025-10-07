using Bussiness.Repository.Abstract;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMenuRepository _menuRepository;

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository, IMenuRepository menuRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _menuRepository = menuRepository;
        }

        #region Ana Sayfalar

        public async Task<IActionResult> Index()
        {
            // Geçici olarak yetki kontrolünü kaldırdık
            // if (!await HasPermissionAsync("VIEW"))
            //     return Forbid();

            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        #endregion

        #region AJAX API Methods

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return Json(new { success = false, error = "Kullanıcı bulunamadı" });

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion


        #region Kullanıcı CRUD İşlemleri

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!await HasPermissionAsync("CREATE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                // Şifre hash'le
                user.Password = HashPassword(user.Password);
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;

                await _userRepository.AddAsync(user);
                return Json(new { success = true, message = "Kullanıcı başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var existingUser = await _userRepository.GetByIdAsync(user.Id);
                if (existingUser == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });

                // Şifre değişmemişse eski şifreyi koru
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = existingUser.Password;
                }
                else
                {
                    user.Password = HashPassword(user.Password);
                }

                await _userRepository.UpdateAsync(user);
                return Json(new { success = true, message = "Kullanıcı başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await HasPermissionAsync("DELETE"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                await _userRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Kullanıcı-Rol İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var roles = await _userRepository.GetUserRolesAsync(userId);
                return Json(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _userRepository.AssignRoleToUserAsync(request.UserId, request.RoleId, currentUserId, request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Rol başarıyla atandı." });
                else
                    return Json(new { success = false, message = "Rol atanamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _userRepository.RemoveRoleFromUserAsync(request.UserId, request.RoleId);
                
                if (success)
                    return Json(new { success = true, message = "Rol başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Rol kaldırılamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRolesForUser(int userId, string? search = null)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var roles = await _roleRepository.GetAvailableRolesForUserAsync(userId, search);
                return Json(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Kullanıcı Ekstra Yetki İşlemleri

        [HttpGet]
        public async Task<IActionResult> GetUserExtraPermissions(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var permissions = await _userRepository.GetUserExtraPermissionsAsync(userId);
                return Json(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignExtraPermissionToUser([FromBody] AssignExtraPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var currentUserId = GetCurrentUserId();
                var success = await _userRepository.AssignExtraPermissionToUserAsync(
                    request.UserId, 
                    request.MenuId, 
                    request.PermissionLevel, 
                    request.Reason, 
                    currentUserId, 
                    request.ExpiryDate, 
                    request.Notes);
                
                if (success)
                    return Json(new { success = true, message = "Ekstra yetki başarıyla atandı." });
                else
                    return Json(new { success = false, message = "Ekstra yetki atanamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExtraPermissionFromUser([FromBody] RemoveExtraPermissionRequest request)
        {
            if (!await HasPermissionAsync("EDIT"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var success = await _userRepository.RemoveExtraPermissionFromUserAsync(request.UserId, request.MenuId);
                
                if (success)
                    return Json(new { success = true, message = "Ekstra yetki başarıyla kaldırıldı." });
                else
                    return Json(new { success = false, message = "Ekstra yetki kaldırılamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableMenusForUser(int userId, string? search = null)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var menus = await _userRepository.GetAvailableUsersForMenuAsync(userId, search);
                return Json(new { success = true, data = menus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Kullanıcı Detayları

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            if (!await HasPermissionAsync("VIEW"))
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });

            try
            {
                var user = await _userRepository.GetUserWithRolesAsync(userId);
                if (user == null)
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        #endregion

        #region Yardımcı Metodlar

        private async Task<bool> HasPermissionAsync(string permissionLevel)
        {
            var currentUserId = GetCurrentUserId();
            var menuId = await ResolveMenuIdAsync("User", "Index");
            return await _userRepository.HasPermissionAsync(currentUserId, menuId, permissionLevel);
        }

        private async Task<int> ResolveMenuIdAsync(string controller, string action)
        {
            var menu = await _menuRepository.GetMenuByRouteAsync(controller, action);
            return menu?.Id ?? 0;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        #endregion
    }

    #region Request Models

    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? Notes { get; set; }
    }

    public class RemoveRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class AssignExtraPermissionRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public string PermissionLevel { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class RemoveExtraPermissionRequest
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
    }

    #endregion
}
