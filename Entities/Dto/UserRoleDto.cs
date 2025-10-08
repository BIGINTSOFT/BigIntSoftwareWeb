using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public UserDto? User { get; set; }
        public RoleDto? Role { get; set; }
        public UserDto? AssignedByUser { get; set; }
    }

    public class CreateUserRoleDto
    {
        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Rol ID gereklidir")]
        public int RoleId { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateUserRoleDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Rol ID gereklidir")]
        public int RoleId { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }
}
