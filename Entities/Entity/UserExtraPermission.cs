using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class UserExtraPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MenuId { get; set; }

        [Required]
        [StringLength(20)]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        [StringLength(200)]
        public string Reason { get; set; } = string.Empty; // Neden verildi

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; } // Ne zaman sona erer

        public bool IsActive { get; set; } = true;

        public int? AssignedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
        public virtual User? AssignedByUser { get; set; }
    }
}
