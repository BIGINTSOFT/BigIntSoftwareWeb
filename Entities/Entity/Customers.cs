using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity
{
    public class Customers
    {
        [Key]
        public int Id { get; set; }

        // Temel Müşteri Bilgileri
        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        // İletişim Bilgileri
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? MobilePhone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        // Adres Bilgileri
        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        // Vergi ve Resmi Bilgiler (Türkiye E-Fatura)
        [StringLength(11)]
        public string? TcNumber { get; set; } // TC Kimlik No

        [StringLength(10)]
        public string? TaxNumber { get; set; } // Vergi Numarası

        [StringLength(50)]
        public string? TaxOffice { get; set; } // Vergi Dairesi

        [StringLength(50)]
        public string? TaxOfficeCode { get; set; } // Vergi Dairesi Kodu

        [StringLength(20)]
        public string? EInvoiceAlias { get; set; } // E-Fatura Alias

        [StringLength(50)]
        public string? EInvoiceTitle { get; set; } // E-Fatura Unvan

        // Avrupa ve Uluslararası Bilgiler
        [StringLength(20)]
        public string? VatNumber { get; set; } // VAT Numarası (Avrupa)

        [StringLength(10)]
        public string? VatCountryCode { get; set; } // VAT Ülke Kodu

        [StringLength(50)]
        public string? LegalEntityType { get; set; } // Hukuki Durum (Şahıs, Şirket, vb.)

        [StringLength(50)]
        public string? CustomerType { get; set; } // Müşteri Tipi (Bireysel, Kurumsal, vb.)

        // Banka Bilgileri
        [StringLength(50)]
        public string? BankName { get; set; }

        [StringLength(50)]
        public string? BankBranch { get; set; }

        [StringLength(30)]
        public string? BankAccountNumber { get; set; }

        [StringLength(20)]
        public string? Iban { get; set; }

        [StringLength(20)]
        public string? SwiftCode { get; set; }

        // Ticari Bilgiler
        [StringLength(50)]
        public string? TradeRegistryNumber { get; set; } // Ticaret Sicil No

        [StringLength(50)]
        public string? ChamberOfCommerce { get; set; } // Ticaret Odası

        [StringLength(50)]
        public string? MersisNumber { get; set; } // MERSİS Numarası

        [StringLength(50)]
        public string? ActivityCode { get; set; } // Faaliyet Kodu

        [StringLength(200)]
        public string? ActivityDescription { get; set; } // Faaliyet Açıklaması

        // Fatura ve Ödeme Bilgileri
        [StringLength(20)]
        public string? PaymentMethod { get; set; } // Ödeme Yöntemi

        public int? PaymentTermDays { get; set; } // Ödeme Vadesi (Gün)

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditLimit { get; set; } // Kredi Limiti

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiscountRate { get; set; } // İndirim Oranı

        [StringLength(20)]
        public string Currency { get; set; } = "TRY"; // Para Birimi

        // E-Fatura ve E-Arşiv Bilgileri
        public bool IsEInvoiceEnabled { get; set; } = false; // E-Fatura Aktif mi?

        public bool IsEArchiveEnabled { get; set; } = false; // E-Arşiv Aktif mi?

        [StringLength(50)]
        public string? EInvoiceProfile { get; set; } // E-Fatura Profili

        [StringLength(50)]
        public string? EArchiveProfile { get; set; } // E-Arşiv Profili

        // Sistem Bilgileri
        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; } // Notlar

        [StringLength(500)]
        public string? InternalNotes { get; set; } // İç Notlar

        // Kategorizasyon
        [StringLength(50)]
        public string? CustomerGroup { get; set; } // Müşteri Grubu

        [StringLength(50)]
        public string? CustomerSegment { get; set; } // Müşteri Segmenti

        [StringLength(50)]
        public string? Source { get; set; } // Müşteri Kaynağı

        // İstatistik Bilgileri
        public int? TotalOrders { get; set; } // Toplam Sipariş Sayısı

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalSales { get; set; } // Toplam Satış Tutarı

        public DateTime? LastOrderDate { get; set; } // Son Sipariş Tarihi

        public DateTime? LastContactDate { get; set; } // Son İletişim Tarihi

        // Sosyal Medya ve İletişim
        [StringLength(100)]
        public string? LinkedIn { get; set; }

        [StringLength(100)]
        public string? Twitter { get; set; }

        [StringLength(100)]
        public string? Facebook { get; set; }

        [StringLength(100)]
        public string? Instagram { get; set; }

        // Ek Adres Bilgileri (Teslimat, Fatura vb.)
        [StringLength(200)]
        public string? DeliveryAddress { get; set; } // Teslimat Adresi

        [StringLength(200)]
        public string? BillingAddress { get; set; } // Fatura Adresi

        [StringLength(50)]
        public string? DeliveryCity { get; set; }

        [StringLength(50)]
        public string? BillingCity { get; set; }

        [StringLength(20)]
        public string? DeliveryPostalCode { get; set; }

        [StringLength(20)]
        public string? BillingPostalCode { get; set; }

        // Risk ve Kredi Bilgileri
        [StringLength(20)]
        public string? RiskLevel { get; set; } // Risk Seviyesi

        [StringLength(20)]
        public string? CreditRating { get; set; } // Kredi Notu

        public bool IsBlacklisted { get; set; } = false; // Kara Liste

        [StringLength(500)]
        public string? BlacklistReason { get; set; } // Kara Liste Sebebi

        // Yasal Uyumluluk
        public bool GdprConsent { get; set; } = false; // GDPR Onayı

        public DateTime? GdprConsentDate { get; set; } // GDPR Onay Tarihi

        public bool MarketingConsent { get; set; } = false; // Pazarlama İletişimi Onayı

        public bool SmsConsent { get; set; } = false; // SMS İletişimi Onayı

        public bool EmailConsent { get; set; } = false; // E-posta İletişimi Onayı

        // Dil ve Bölgesel Ayarlar
        [StringLength(10)]
        public string Language { get; set; } = "tr-TR"; // Dil

        [StringLength(10)]
        public string TimeZone { get; set; } = "dd.MM.yyyy"; // Saat Dilimi

        [StringLength(10)]
        public string DateFormat { get; set; } = "dd.MM.yyyy"; // Tarih Formatı

        [StringLength(10)]
        public string NumberFormat { get; set; } = "tr-TR"; // Sayı Formatı
    }
}