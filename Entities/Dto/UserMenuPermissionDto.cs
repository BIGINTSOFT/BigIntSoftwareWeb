using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class UserMenuPermissionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "İzin seviyesi gereklidir")]
        [StringLength(20, ErrorMessage = "İzin seviyesi en fazla 20 karakter olabilir")]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public DateTime AssignedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public UserDto? User { get; set; }
        public MenuDto? Menu { get; set; }
        public PermissionDto? Permission { get; set; }
        public UserDto? AssignedByUser { get; set; }
    }

    public class CreateUserMenuPermissionDto
    {
        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "İzin ID gereklidir")]
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "İzin seviyesi gereklidir")]
        [StringLength(20, ErrorMessage = "İzin seviyesi en fazla 20 karakter olabilir")]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateUserMenuPermissionDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "İzin ID gereklidir")]
        public int PermissionId { get; set; }

        [Required(ErrorMessage = "İzin seviyesi gereklidir")]
        [StringLength(20, ErrorMessage = "İzin seviyesi en fazla 20 karakter olabilir")]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }
}
