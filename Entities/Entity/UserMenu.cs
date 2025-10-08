using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class UserMenu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MenuId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

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
