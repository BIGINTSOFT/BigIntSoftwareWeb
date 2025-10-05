using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public int? MenuId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual Role Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
        public virtual Menu? Menu { get; set; }
    }
}
