using Microsoft.EntityFrameworkCore;
using Entities.Entity;

namespace DataAccess.DbContext
{
    public class BigIntSoftwareDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BigIntSoftwareDbContext(DbContextOptions<BigIntSoftwareDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<UserExtraPermission> UserExtraPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity konfigürasyonu
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Role entity konfigürasyonu
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Menu entity konfigürasyonu
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menus", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.Url).HasMaxLength(200);
                entity.Property(e => e.Controller).HasMaxLength(100);
                entity.Property(e => e.Action).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsVisible).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // Self-referencing relationship
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Permission entity konfigürasyonu
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Code).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // UserRole entity konfigürasyonu
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserRoles)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role)
                      .WithMany(e => e.UserRoles)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedUserRoles)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            // RoleMenuPermission entity konfigürasyonu
            modelBuilder.Entity<RoleMenuPermission>(entity =>
            {
                entity.ToTable("RoleMenuPermissions", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PermissionLevel).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.Role)
                      .WithMany(e => e.RoleMenuPermissions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Menu)
                      .WithMany(e => e.RoleMenuPermissions)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedRoleMenuPermissions)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();
            });

            // UserExtraPermission entity konfigürasyonu
            modelBuilder.Entity<UserExtraPermission>(entity =>
            {
                entity.ToTable("UserExtraPermissions", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PermissionLevel).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserExtraPermissions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Menu)
                      .WithMany(e => e.UserExtraPermissions)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedUserExtraPermissions)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.UserId, e.MenuId }).IsUnique();
            });
        }
    }
}
