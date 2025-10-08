using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class UserMenuDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public UserDto? User { get; set; }
        public MenuDto? Menu { get; set; }
        public UserDto? AssignedByUser { get; set; }
    }

    public class CreateUserMenuDto
    {
        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateUserMenuDto
    {
        [Required(ErrorMessage = "ID gereklidir")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ID gereklidir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Menü ID gereklidir")]
        public int MenuId { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? AssignedBy { get; set; }
        public string? Notes { get; set; }
    }
}
