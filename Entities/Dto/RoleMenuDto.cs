using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class RoleMenuDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public RoleDto? Role { get; set; }
        public MenuDto? Menu { get; set; }
        public UserDto? AssignedByUser { get; set; }
    }

    public class CreateRoleMenuDto
    {
        [Required(ErrorMessage = "Rol ID gereklidir")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        public bool IsActive { get; set; } = true;
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateRoleMenuDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Rol ID gereklidir")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }
}
