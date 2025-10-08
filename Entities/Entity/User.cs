using System.ComponentModel.DataAnnotations;

namespace Entities.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? LastLoginDate { get; set; }

        // Navigation Properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();
        public virtual ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
        
        // Assigned by relationships
        public virtual ICollection<UserRole> AssignedUserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserMenu> AssignedUserMenus { get; set; } = new List<UserMenu>();
        public virtual ICollection<UserMenuPermission> AssignedUserMenuPermissions { get; set; } = new List<UserMenuPermission>();
        public virtual ICollection<RoleMenu> AssignedRoleMenus { get; set; } = new List<RoleMenu>();
        public virtual ICollection<RoleMenuPermission> AssignedRoleMenuPermissions { get; set; } = new List<RoleMenuPermission>();
    }
}
