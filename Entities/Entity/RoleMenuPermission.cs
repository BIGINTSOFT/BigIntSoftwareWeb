using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class RoleMenuPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int MenuId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        [Required]
        [StringLength(20)]
        public string PermissionLevel { get; set; } = string.Empty; // VIEW, CREATE, EDIT, DELETE

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public int? AssignedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Role Role { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
        public virtual User? AssignedByUser { get; set; }
    }
}
