using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class RoleDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rol adı gereklidir")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public List<MenuDto> Menus { get; set; } = new List<MenuDto>();
        public List<RoleMenuPermissionDto> MenuPermissions { get; set; } = new List<RoleMenuPermissionDto>();
    }

    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Rol adı gereklidir")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<int> MenuIds { get; set; } = new List<int>();
    }

    public class UpdateRoleDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Rol adı gereklidir")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<int> MenuIds { get; set; } = new List<int>();
    }
}
