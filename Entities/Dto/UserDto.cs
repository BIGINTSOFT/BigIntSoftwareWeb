using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        // Navigation Properties
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
        public List<MenuDto> Menus { get; set; } = new List<MenuDto>();
        public List<UserMenuPermissionDto> MenuPermissions { get; set; } = new List<UserMenuPermissionDto>();
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 255 karakter olabilir")]
        public string Password { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<int> RoleIds { get; set; } = new List<int>();
    }

    public class UpdateUserDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
