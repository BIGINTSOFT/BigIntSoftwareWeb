using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Rol adı gereklidir")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
