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
        public DbSet<UserMenu> UserMenus { get; set; }
        public DbSet<UserMenuPermission> UserMenuPermissions { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<Customers> CustomerSet { get; set; }

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
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            // UserMenu entity konfigürasyonu
            modelBuilder.Entity<UserMenu>(entity =>
            {
                entity.ToTable("UserMenus", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserMenus)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Menu)
                      .WithMany(e => e.UserMenus)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedUserMenus)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => new { e.UserId, e.MenuId }).IsUnique();
            });

            // UserMenuPermission entity konfigürasyonu
            modelBuilder.Entity<UserMenuPermission>(entity =>
            {
                entity.ToTable("UserMenuPermissions", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PermissionLevel).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserMenuPermissions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Menu)
                      .WithMany(e => e.UserMenuPermissions)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Permission)
                      .WithMany(e => e.UserMenuPermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedUserMenuPermissions)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => new { e.UserId, e.MenuId, e.PermissionId }).IsUnique();
            });

            // RoleMenu entity konfigürasyonu
            modelBuilder.Entity<RoleMenu>(entity =>
            {
                entity.ToTable("RoleMenus", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.Role)
                      .WithMany(e => e.RoleMenus)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Menu)
                      .WithMany(e => e.RoleMenus)
                      .HasForeignKey(e => e.MenuId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedRoleMenus)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();
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
                entity.HasOne(e => e.Permission)
                      .WithMany(e => e.RoleMenuPermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssignedByUser)
                      .WithMany(e => e.AssignedRoleMenuPermissions)
                      .HasForeignKey(e => e.AssignedBy)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasIndex(e => new { e.RoleId, e.MenuId, e.PermissionId }).IsUnique();
            });

            // Customers entity konfigürasyonu
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.ToTable("Customers", "BigIntSoftware");
                entity.HasKey(e => e.Id);
                
                // Temel alanlar
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                
                // İletişim bilgileri
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.MobilePhone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Website).HasMaxLength(100);
                
                // Adres bilgileri
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.State).HasMaxLength(50);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.Country).HasMaxLength(50);
                
                // Vergi bilgileri
                entity.Property(e => e.TcNumber).HasMaxLength(11);
                entity.Property(e => e.TaxNumber).HasMaxLength(10);
                entity.Property(e => e.TaxOffice).HasMaxLength(50);
                entity.Property(e => e.TaxOfficeCode).HasMaxLength(50);
                entity.Property(e => e.EInvoiceAlias).HasMaxLength(20);
                entity.Property(e => e.EInvoiceTitle).HasMaxLength(50);
                
                // Avrupa bilgileri
                entity.Property(e => e.VatNumber).HasMaxLength(20);
                entity.Property(e => e.VatCountryCode).HasMaxLength(10);
                entity.Property(e => e.LegalEntityType).HasMaxLength(50);
                entity.Property(e => e.CustomerType).HasMaxLength(50);
                
                // Banka bilgileri
                entity.Property(e => e.BankName).HasMaxLength(50);
                entity.Property(e => e.BankBranch).HasMaxLength(50);
                entity.Property(e => e.BankAccountNumber).HasMaxLength(30);
                entity.Property(e => e.Iban).HasMaxLength(20);
                entity.Property(e => e.SwiftCode).HasMaxLength(20);
                
                // Ticari bilgiler
                entity.Property(e => e.TradeRegistryNumber).HasMaxLength(50);
                entity.Property(e => e.ChamberOfCommerce).HasMaxLength(50);
                entity.Property(e => e.MersisNumber).HasMaxLength(50);
                entity.Property(e => e.ActivityCode).HasMaxLength(50);
                entity.Property(e => e.ActivityDescription).HasMaxLength(200);
                
                // Fatura bilgileri
                entity.Property(e => e.PaymentMethod).HasMaxLength(20);
                entity.Property(e => e.Currency).HasMaxLength(20).HasDefaultValue("TRY");
                
                // E-Fatura bilgileri
                entity.Property(e => e.EInvoiceProfile).HasMaxLength(50);
                entity.Property(e => e.EArchiveProfile).HasMaxLength(50);
                entity.Property(e => e.IsEInvoiceEnabled).HasDefaultValue(false);
                entity.Property(e => e.IsEArchiveEnabled).HasDefaultValue(false);
                
                // Sistem bilgileri
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.InternalNotes).HasMaxLength(500);
                
                // Kategorizasyon
                entity.Property(e => e.CustomerGroup).HasMaxLength(50);
                entity.Property(e => e.CustomerSegment).HasMaxLength(50);
                entity.Property(e => e.Source).HasMaxLength(50);
                
                // Sosyal medya
                entity.Property(e => e.LinkedIn).HasMaxLength(100);
                entity.Property(e => e.Twitter).HasMaxLength(100);
                entity.Property(e => e.Facebook).HasMaxLength(100);
                entity.Property(e => e.Instagram).HasMaxLength(100);
                
                // Ek adresler
                entity.Property(e => e.DeliveryAddress).HasMaxLength(200);
                entity.Property(e => e.BillingAddress).HasMaxLength(200);
                entity.Property(e => e.DeliveryCity).HasMaxLength(50);
                entity.Property(e => e.BillingCity).HasMaxLength(50);
                entity.Property(e => e.DeliveryPostalCode).HasMaxLength(20);
                entity.Property(e => e.BillingPostalCode).HasMaxLength(20);
                
                // Risk bilgileri
                entity.Property(e => e.RiskLevel).HasMaxLength(20);
                entity.Property(e => e.CreditRating).HasMaxLength(20);
                entity.Property(e => e.IsBlacklisted).HasDefaultValue(false);
                entity.Property(e => e.BlacklistReason).HasMaxLength(500);
                
                // Yasal uyumluluk
                entity.Property(e => e.GdprConsent).HasDefaultValue(false);
                entity.Property(e => e.MarketingConsent).HasDefaultValue(false);
                entity.Property(e => e.SmsConsent).HasDefaultValue(false);
                entity.Property(e => e.EmailConsent).HasDefaultValue(false);
                
                // Dil ayarları
                entity.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("tr-TR");
                entity.Property(e => e.TimeZone).HasMaxLength(10).HasDefaultValue("Turkey Standard Time");
                entity.Property(e => e.DateFormat).HasMaxLength(10).HasDefaultValue("dd.MM.yyyy");
                entity.Property(e => e.NumberFormat).HasMaxLength(10).HasDefaultValue("tr-TR");
                
                // Decimal precision
                entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
                entity.Property(e => e.DiscountRate).HasPrecision(5, 2);
                entity.Property(e => e.TotalSales).HasPrecision(18, 2);
                
                // Index'ler
                entity.HasIndex(e => e.CompanyName);
                entity.HasIndex(e => e.Email).IsUnique().HasFilter("[Email] IS NOT NULL AND [Email] != ''");
                entity.HasIndex(e => e.TaxNumber).IsUnique().HasFilter("[TaxNumber] IS NOT NULL AND [TaxNumber] != ''");
                entity.HasIndex(e => e.TcNumber).IsUnique().HasFilter("[TcNumber] IS NOT NULL AND [TcNumber] != ''");
                entity.HasIndex(e => e.VatNumber);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CustomerType);
                entity.HasIndex(e => e.CreatedDate);
            });
        }
    }
}
