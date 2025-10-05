using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bussiness.Repository.Abstract;
using Entities.Entity;
using Entities.Dto;

namespace BigIntSoftwareWeb.Controllers
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

        // GET: User
        public IActionResult Index()
        {
            return View();
        }

        // GET: User/GetUsers (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userList = users.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    isActive = u.IsActive,
                    createdDate = u.CreatedDate.ToString("dd.MM.yyyy HH:mm"),
                    lastLoginDate = u.LastLoginDate?.ToString("dd.MM.yyyy HH:mm") ?? "Hiç giriş yapmamış"
                }).ToList();

                return Json(new { data = userList });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET: User/GetUser (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return Json(new { error = "Kullanıcı bulunamadı" });
                }

                var userData = new
                {
                    id = user.Id,
                    username = user.Username,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    isActive = user.IsActive,
                    createdDate = user.CreatedDate.ToString("dd.MM.yyyy HH:mm"),
                    lastLoginDate = user.LastLoginDate?.ToString("dd.MM.yyyy HH:mm") ?? "Hiç giriş yapmamış"
                };

                return Json(new { success = true, data = userData });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // POST: User/Create (AJAX)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { error = string.Join(", ", errors) });
                }

                // Kullanıcı adı kontrolü
                var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
                if (existingUser != null)
                {
                    return Json(new { error = "Bu kullanıcı adı zaten kullanılıyor" });
                }

                // Email kontrolü
                var existingEmail = await _userRepository.GetByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    return Json(new { error = "Bu email adresi zaten kullanılıyor" });
                }

                var user = new User
                {
                    Username = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _userRepository.AddAsync(user);

                return Json(new { success = true, message = "Kullanıcı başarıyla oluşturuldu" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // PUT: User/Update/5 (AJAX)
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { error = string.Join(", ", errors) });
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return Json(new { error = "Kullanıcı bulunamadı" });
                }

                // Kullanıcı adı kontrolü (kendisi hariç)
                var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
                if (existingUser != null && existingUser.Id != id)
                {
                    return Json(new { error = "Bu kullanıcı adı zaten kullanılıyor" });
                }

                // Email kontrolü (kendisi hariç)
                var existingEmail = await _userRepository.GetByEmailAsync(model.Email);
                if (existingEmail != null && existingEmail.Id != id)
                {
                    return Json(new { error = "Bu email adresi zaten kullanılıyor" });
                }

                user.Username = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.IsActive = model.IsActive;

                // Şifre değiştirilmişse güncelle
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.Password = HashPassword(model.Password);
                }

                await _userRepository.UpdateAsync(user);

                return Json(new { success = true, message = "Kullanıcı başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // DELETE: User/Delete/5 (AJAX)
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return Json(new { error = "Kullanıcı bulunamadı" });
                }

                await _userRepository.DeleteAsync(user);

                return Json(new { success = true, message = "Kullanıcı başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // User Role Management
        [HttpGet]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { error = "Kullanıcı bulunamadı" });
                }

                var roles = await _roleRepository.GetUserRolesAsync(userId);
                var roleDtos = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    description = r.Description,
                    isActive = r.IsActive
                }).ToList();

                return Json(new { success = true, data = roleDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRolesForUser(int userId, string search = "")
        {
            try
            {
                var roles = await _roleRepository.GetAvailableRolesForUserAsync(userId, search);
                var roleDtos = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    description = r.Description,
                    isActive = r.IsActive
                }).ToList();

                return Json(new { success = true, data = roleDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
        {
            try
            {
                var result = await _roleRepository.AssignRoleToUserAsync(userId, roleId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcıya rol atandı" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı zaten bu role sahip" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                var result = await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcının rolü kaldırıldı" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı bu role sahip değil" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // User Menu Permissions Management
        [HttpGet]
        public async Task<IActionResult> GetUserMenus(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { error = "Kullanıcı bulunamadı" });
                }

                // Kullanıcının direkt menü yetkilerini getir
                var directMenus = await _menuRepository.GetUserDirectMenusAsync(userId);
                
                // Kullanıcının rol bazlı menü yetkilerini getir
                var roleMenus = await _menuRepository.GetUserRoleMenusAsync(userId);
                
                var menuDtos = new List<object>();
                
                // Direkt yetkileri ekle
                foreach (var menu in directMenus)
                {
                    menuDtos.Add(new
                    {
                        id = menu.Id,
                        name = menu.Name,
                        icon = menu.Icon,
                        controller = menu.Controller,
                        action = menu.Action,
                        isActive = menu.IsActive,
                        source = "Direct"
                    });
                }
                
                // Rol bazlı yetkileri ekle (direkt yetkilerle çakışmayanlar)
                var directMenuIds = directMenus.Select(m => m.Id).ToHashSet();
                foreach (var menu in roleMenus.Where(m => !directMenuIds.Contains(m.Id)))
                {
                    menuDtos.Add(new
                    {
                        id = menu.Id,
                        name = menu.Name,
                        icon = menu.Icon,
                        controller = menu.Controller,
                        action = menu.Action,
                        isActive = menu.IsActive,
                        source = "Role"
                    });
                }

                return Json(new { success = true, data = menuDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableMenusForUser(int userId, string search = "")
        {
            try
            {
                // Kullanıcının mevcut menü yetkilerini al (rol + direkt)
                var userMenus = await _menuRepository.GetUserMenusAsync(userId);
                var userMenuIds = userMenus.Select(m => m.Id).ToHashSet();

                // Tüm aktif menüleri al
                var allMenus = await _menuRepository.GetVisibleMenusAsync();
                
                // Kullanıcının yetkisi olmayan menüleri filtrele
                var availableMenus = allMenus.Where(m => !userMenuIds.Contains(m.Id));
                
                // Arama filtresi uygula
                if (!string.IsNullOrEmpty(search))
                {
                    availableMenus = availableMenus.Where(m => 
                        m.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                        (m.Description != null && m.Description.Contains(search, StringComparison.OrdinalIgnoreCase)));
                }

                var menuDtos = availableMenus.Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    icon = m.Icon,
                    controller = m.Controller,
                    action = m.Action,
                    isActive = m.IsActive
                }).ToList();

                return Json(new { success = true, data = menuDtos });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignMenuToUser(int userId, int menuId)
        {
            try
            {
                var result = await _userRepository.AssignUserToMenuAsync(userId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcıya menü yetkisi verildi" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı zaten bu menüye yetkili" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMenuFromUser(int userId, int menuId)
        {
            try
            {
                var result = await _userRepository.RemoveUserFromMenuAsync(userId, menuId);
                if (result)
                {
                    return Json(new { success = true, message = "Kullanıcının menü yetkisi kaldırıldı" });
                }
                else
                {
                    return Json(new { error = "Kullanıcı bu menüye yetkili değil" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
