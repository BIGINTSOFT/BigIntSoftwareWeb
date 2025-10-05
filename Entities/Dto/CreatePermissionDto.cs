using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class CreatePermissionDto
    {
        [Required(ErrorMessage = "Yetki adı gereklidir")]
        [StringLength(100, ErrorMessage = "Yetki adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Yetki kodu gereklidir")]
        [StringLength(100, ErrorMessage = "Yetki kodu en fazla 100 karakter olabilir")]
        public string Code { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
