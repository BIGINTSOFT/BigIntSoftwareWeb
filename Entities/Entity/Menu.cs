using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [StringLength(200)]
        public string? Url { get; set; }

        [StringLength(100)]
        public string? Controller { get; set; }

        [StringLength(100)]
        public string? Action { get; set; }

        public int? ParentId { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual Menu? Parent { get; set; }
        public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
        public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();
        public virtual ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
        public virtual ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; } = new List<RoleMenuPermission>();
    }
}
