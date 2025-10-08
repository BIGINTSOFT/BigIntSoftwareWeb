using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class RoleMenuPermissionDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "İzin seviyesi gereklidir")]
        [StringLength(20, ErrorMessage = "İzin seviyesi en fazla 20 karakter olabilir")]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public RoleDto? Role { get; set; }
        public MenuDto? Menu { get; set; }
        public PermissionDto? Permission { get; set; }
        public UserDto? AssignedByUser { get; set; }
    }

    public class CreateRoleMenuPermissionDto
    {
        [Required(ErrorMessage = "Rol ID gereklidir")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "İzin ID gereklidir")]
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "İzin seviyesi gereklidir")]
        [StringLength(20, ErrorMessage = "İzin seviyesi en fazla 20 karakter olabilir")]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public bool IsActive { get; set; } = true;
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateRoleMenuPermissionDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Rol ID gereklidir")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "İzin ID gereklidir")]
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "İzin seviyesi gereklidir")]
        [StringLength(20, ErrorMessage = "İzin seviyesi en fazla 20 karakter olabilir")]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }
}
