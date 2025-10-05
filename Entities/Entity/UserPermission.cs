using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public int? MenuId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
        public virtual Menu? Menu { get; set; }
    }
}
