using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(255, ErrorMessage = "Şifre en fazla 255 karakter olabilir")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}
