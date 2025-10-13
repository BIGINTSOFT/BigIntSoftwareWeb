using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entity
{
    public class Suppliers
    {
        [Key]
        public int Id { get; set; }

        // Temel Tedarikçi Bilgileri
        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(100)]
        public string? ContactPersonTitle { get; set; } // İletişim Kişisi Unvanı

        [StringLength(100)]
        public string? ContactPersonDepartment { get; set; } // İletişim Kişisi Departmanı

        // SAP Entegrasyon Alanları
        [StringLength(20)]
        public string? SapVendorCode { get; set; } // SAP Tedarikçi Kodu

        [StringLength(20)]
        public string? SapAccountGroup { get; set; } // SAP Hesap Grubu

        [StringLength(20)]
        public string? SapPurchasingOrg { get; set; } // SAP Satın Alma Organizasyonu

        [StringLength(20)]
        public string? SapCompanyCode { get; set; } // SAP Şirket Kodu

        [StringLength(20)]
        public string? SapPaymentTerms { get; set; } // SAP Ödeme Koşulları

        [StringLength(20)]
        public string? SapCurrency { get; set; } // SAP Para Birimi

        [StringLength(20)]
        public string? SapTaxCode { get; set; } // SAP Vergi Kodu

        [StringLength(20)]
        public string? SapTaxCountry { get; set; } // SAP Vergi Ülkesi

        // İletişim Bilgileri
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? MobilePhone { get; set; }

        [StringLength(20)]
        public string? Fax { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? LinkedIn { get; set; }

        [StringLength(100)]
        public string? Twitter { get; set; }

        [StringLength(100)]
        public string? Facebook { get; set; }

        [StringLength(100)]
        public string? Instagram { get; set; }

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

        [StringLength(50)]
        public string? Region { get; set; } // Bölge

        [StringLength(50)]
        public string? District { get; set; } // İlçe

        // Teslimat Adresi
        [StringLength(200)]
        public string? DeliveryAddress { get; set; }

        [StringLength(50)]
        public string? DeliveryCity { get; set; }

        [StringLength(50)]
        public string? DeliveryState { get; set; }

        [StringLength(20)]
        public string? DeliveryPostalCode { get; set; }

        [StringLength(50)]
        public string? DeliveryCountry { get; set; }

        // Fatura Adresi
        [StringLength(200)]
        public string? BillingAddress { get; set; }

        [StringLength(50)]
        public string? BillingCity { get; set; }

        [StringLength(50)]
        public string? BillingState { get; set; }

        [StringLength(20)]
        public string? BillingPostalCode { get; set; }

        [StringLength(50)]
        public string? BillingCountry { get; set; }

        // Vergi ve Resmi Bilgiler (Türkiye E-Fatura/E-İrsaliye)
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

        [StringLength(20)]
        public string? EInvoiceProfile { get; set; } // E-Fatura Profili

        [StringLength(20)]
        public string? EArchiveProfile { get; set; } // E-Arşiv Profili

        [StringLength(20)]
        public string? EDeliveryNoteProfile { get; set; } // E-İrsaliye Profili

        [StringLength(20)]
        public string? EInvoiceTestAlias { get; set; } // E-Fatura Test Alias

        [StringLength(20)]
        public string? EInvoiceProdAlias { get; set; } // E-Fatura Prod Alias

        // Avrupa ve Uluslararası Bilgiler
        [StringLength(20)]
        public string? VatNumber { get; set; } // VAT Numarası (Avrupa)

        [StringLength(10)]
        public string? VatCountryCode { get; set; } // VAT Ülke Kodu

        [StringLength(50)]
        public string? LegalEntityType { get; set; } // Hukuki Durum

        [StringLength(50)]
        public string? SupplierType { get; set; } // Tedarikçi Tipi

        [StringLength(50)]
        public string? SupplierCategory { get; set; } // Tedarikçi Kategorisi

        [StringLength(50)]
        public string? SupplierGroup { get; set; } // Tedarikçi Grubu

        [StringLength(50)]
        public string? SupplierSegment { get; set; } // Tedarikçi Segmenti

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

        [StringLength(50)]
        public string? BankAddress { get; set; }

        [StringLength(50)]
        public string? BankCity { get; set; }

        [StringLength(50)]
        public string? BankCountry { get; set; }

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

        [StringLength(50)]
        public string? IndustryCode { get; set; } // Sektör Kodu

        [StringLength(100)]
        public string? IndustryDescription { get; set; } // Sektör Açıklaması

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

        [StringLength(20)]
        public string? PaymentCurrency { get; set; } // Ödeme Para Birimi

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ExchangeRate { get; set; } // Döviz Kuru

        // E-Fatura ve E-Arşiv Bilgileri
        public bool IsEInvoiceEnabled { get; set; } = false; // E-Fatura Aktif mi?

        public bool IsEArchiveEnabled { get; set; } = false; // E-Arşiv Aktif mi?

        public bool IsEDeliveryNoteEnabled { get; set; } = false; // E-İrsaliye Aktif mi?

        public bool IsEInvoiceTestMode { get; set; } = true; // E-Fatura Test Modu

        public bool IsEArchiveTestMode { get; set; } = true; // E-Arşiv Test Modu

        public bool IsEDeliveryNoteTestMode { get; set; } = true; // E-İrsaliye Test Modu

        [StringLength(50)]
        public string? EInvoiceIntegrationType { get; set; } // E-Fatura Entegrasyon Tipi

        [StringLength(50)]
        public string? EInvoiceServiceProvider { get; set; } // E-Fatura Servis Sağlayıcısı

        [StringLength(100)]
        public string? EInvoiceApiUrl { get; set; } // E-Fatura API URL

        [StringLength(100)]
        public string? EInvoiceUsername { get; set; } // E-Fatura Kullanıcı Adı

        [StringLength(100)]
        public string? EInvoicePassword { get; set; } // E-Fatura Şifre

        [StringLength(100)]
        public string? EInvoiceCertificatePath { get; set; } // E-Fatura Sertifika Yolu

        [StringLength(100)]
        public string? EInvoiceCertificatePassword { get; set; } // E-Fatura Sertifika Şifresi

        // Kalite ve Sertifikasyon
        [StringLength(50)]
        public string? QualityCertification { get; set; } // Kalite Sertifikası

        [StringLength(50)]
        public string? IsoCertification { get; set; } // ISO Sertifikası

        [StringLength(50)]
        public string? EnvironmentalCertification { get; set; } // Çevre Sertifikası

        [StringLength(50)]
        public string? SafetyCertification { get; set; } // Güvenlik Sertifikası

        public DateTime? CertificationExpiryDate { get; set; } // Sertifika Bitiş Tarihi

        // Performans ve Değerlendirme
        [Column(TypeName = "decimal(3,2)")]
        public decimal? QualityRating { get; set; } // Kalite Puanı (0-5)

        [Column(TypeName = "decimal(3,2)")]
        public decimal? DeliveryRating { get; set; } // Teslimat Puanı (0-5)

        [Column(TypeName = "decimal(3,2)")]
        public decimal? ServiceRating { get; set; } // Hizmet Puanı (0-5)

        [Column(TypeName = "decimal(3,2)")]
        public decimal? PriceRating { get; set; } // Fiyat Puanı (0-5)

        [Column(TypeName = "decimal(3,2)")]
        public decimal? OverallRating { get; set; } // Genel Puan (0-5)

        [StringLength(20)]
        public string? RiskLevel { get; set; } // Risk Seviyesi

        [StringLength(20)]
        public string? CreditRating { get; set; } // Kredi Notu

        public bool IsBlacklisted { get; set; } = false; // Kara Liste

        [StringLength(500)]
        public string? BlacklistReason { get; set; } // Kara Liste Sebebi

        public DateTime? BlacklistDate { get; set; } // Kara Liste Tarihi

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

        [StringLength(500)]
        public string? QualityNotes { get; set; } // Kalite Notları

        [StringLength(500)]
        public string? PaymentNotes { get; set; } // Ödeme Notları

        // Kategorizasyon
        [StringLength(50)]
        public string? Source { get; set; } // Tedarikçi Kaynağı

        [StringLength(50)]
        public string? Priority { get; set; } // Öncelik

        [StringLength(50)]
        public string? Status { get; set; } // Durum

        // İstatistik Bilgileri
        public int? TotalOrders { get; set; } // Toplam Sipariş Sayısı

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPurchases { get; set; } // Toplam Satın Alma Tutarı

        public DateTime? LastOrderDate { get; set; } // Son Sipariş Tarihi

        public DateTime? LastContactDate { get; set; } // Son İletişim Tarihi

        public DateTime? LastDeliveryDate { get; set; } // Son Teslimat Tarihi

        public DateTime? LastPaymentDate { get; set; } // Son Ödeme Tarihi

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
        public string TimeZone { get; set; } = "Turkey Standard Time"; // Saat Dilimi

        [StringLength(10)]
        public string DateFormat { get; set; } = "dd.MM.yyyy"; // Tarih Formatı

        [StringLength(10)]
        public string NumberFormat { get; set; } = "tr-TR"; // Sayı Formatı

        // Ek SAP Alanları
        [StringLength(20)]
        public string? SapVendorAccount { get; set; } // SAP Tedarikçi Hesabı

        [StringLength(20)]
        public string? SapReconciliationAccount { get; set; } // SAP Mutabakat Hesabı

        [StringLength(20)]
        public string? SapSortKey { get; set; } // SAP Sıralama Anahtarı

        [StringLength(20)]
        public string? SapPaymentBlock { get; set; } // SAP Ödeme Engeli

        [StringLength(20)]
        public string? SapPostingBlock { get; set; } // SAP Kayıt Engeli

        [StringLength(20)]
        public string? SapPurchasingBlock { get; set; } // SAP Satın Alma Engeli

        [StringLength(20)]
        public string? SapOrderCurrency { get; set; } // SAP Sipariş Para Birimi

        [StringLength(20)]
        public string? SapIncoterms { get; set; } // SAP Teslim Şartları

        [StringLength(20)]
        public string? SapTransportationZone { get; set; } // SAP Taşıma Bölgesi

        [StringLength(20)]
        public string? SapShippingCondition { get; set; } // SAP Sevkiyat Koşulu

        [StringLength(20)]
        public string? SapDeliveryPriority { get; set; } // SAP Teslimat Önceliği

        [StringLength(20)]
        public string? SapMinimumOrderValue { get; set; } // SAP Minimum Sipariş Değeri

        [StringLength(20)]
        public string? SapMaximumOrderValue { get; set; } // SAP Maksimum Sipariş Değeri

        // E-İrsaliye Özel Alanları
        [StringLength(50)]
        public string? EDeliveryNoteAlias { get; set; } // E-İrsaliye Alias

        [StringLength(50)]
        public string? EDeliveryNoteTitle { get; set; } // E-İrsaliye Unvan

        [StringLength(50)]
        public string? EDeliveryNoteIntegrationType { get; set; } // E-İrsaliye Entegrasyon Tipi

        [StringLength(50)]
        public string? EDeliveryNoteServiceProvider { get; set; } // E-İrsaliye Servis Sağlayıcısı

        [StringLength(100)]
        public string? EDeliveryNoteApiUrl { get; set; } // E-İrsaliye API URL

        [StringLength(100)]
        public string? EDeliveryNoteUsername { get; set; } // E-İrsaliye Kullanıcı Adı

        [StringLength(100)]
        public string? EDeliveryNotePassword { get; set; } // E-İrsaliye Şifre

        // Tedarikçi Özel Alanları
        [StringLength(50)]
        public string? SupplierCode { get; set; } // Tedarikçi Kodu

        [StringLength(50)]
        public string? SupplierName { get; set; } // Tedarikçi Adı

        [StringLength(50)]
        public string? SupplierShortName { get; set; } // Tedarikçi Kısa Adı

        [StringLength(50)]
        public string? SupplierDisplayName { get; set; } // Tedarikçi Görünen Adı

        [StringLength(50)]
        public string? SupplierLegalName { get; set; } // Tedarikçi Yasal Adı

        [StringLength(50)]
        public string? SupplierCommercialName { get; set; } // Tedarikçi Ticari Adı

        [StringLength(50)]
        public string? SupplierBrandName { get; set; } // Tedarikçi Marka Adı

        // İletişim Özel Alanları
        [StringLength(100)]
        public string? PrimaryContactEmail { get; set; } // Birincil İletişim E-postası

        [StringLength(100)]
        public string? SecondaryContactEmail { get; set; } // İkincil İletişim E-postası

        [StringLength(20)]
        public string? PrimaryContactPhone { get; set; } // Birincil İletişim Telefonu

        [StringLength(20)]
        public string? SecondaryContactPhone { get; set; } // İkincil İletişim Telefonu

        [StringLength(100)]
        public string? EmergencyContactName { get; set; } // Acil Durum İletişim Kişisi

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; } // Acil Durum İletişim Telefonu

        [StringLength(100)]
        public string? EmergencyContactEmail { get; set; } // Acil Durum İletişim E-postası

        // Finansal Bilgiler
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AnnualRevenue { get; set; } // Yıllık Ciro

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetWorth { get; set; } // Net Değer

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAssets { get; set; } // Toplam Varlık

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalLiabilities { get; set; } // Toplam Borç

        public int? EmployeeCount { get; set; } // Çalışan Sayısı

        public DateTime? EstablishmentDate { get; set; } // Kuruluş Tarihi

        [StringLength(50)]
        public string? OwnershipType { get; set; } // Mülkiyet Tipi

        [StringLength(50)]
        public string? BusinessSize { get; set; } // İşletme Büyüklüğü

        // Tedarikçi Performans Metrikleri
        [Column(TypeName = "decimal(5,2)")]
        public decimal? OnTimeDeliveryRate { get; set; } // Zamanında Teslimat Oranı

        [Column(TypeName = "decimal(5,2)")]
        public decimal? QualityAcceptanceRate { get; set; } // Kalite Kabul Oranı

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PriceCompetitiveness { get; set; } // Fiyat Rekabetçiliği

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ServiceLevel { get; set; } // Hizmet Seviyesi

        public int? AverageDeliveryDays { get; set; } // Ortalama Teslimat Günü

        public int? MinimumOrderQuantity { get; set; } // Minimum Sipariş Miktarı

        public int? MaximumOrderQuantity { get; set; } // Maksimum Sipariş Miktarı

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumOrderValue { get; set; } // Minimum Sipariş Değeri

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaximumOrderValue { get; set; } // Maksimum Sipariş Değeri

        // Tedarikçi Sertifikaları ve Belgeleri
        [StringLength(100)]
        public string? BusinessLicense { get; set; } // İşletme Ruhsatı

        [StringLength(100)]
        public string? TaxCertificate { get; set; } // Vergi Levhası

        [StringLength(100)]
        public string? TradeRegistryCertificate { get; set; } // Ticaret Sicil Belgesi

        [StringLength(100)]
        public string? ChamberOfCommerceCertificate { get; set; } // Ticaret Odası Belgesi

        [StringLength(100)]
        public string? InsuranceCertificate { get; set; } // Sigorta Belgesi

        [StringLength(100)]
        public string? FinancialStatement { get; set; } // Mali Durum Belgesi

        public DateTime? BusinessLicenseExpiryDate { get; set; } // İşletme Ruhsatı Bitiş Tarihi

        public DateTime? TaxCertificateExpiryDate { get; set; } // Vergi Levhası Bitiş Tarihi

        public DateTime? TradeRegistryExpiryDate { get; set; } // Ticaret Sicil Bitiş Tarihi

        public DateTime? ChamberOfCommerceExpiryDate { get; set; } // Ticaret Odası Bitiş Tarihi

        public DateTime? InsuranceExpiryDate { get; set; } // Sigorta Bitiş Tarihi

        public DateTime? FinancialStatementDate { get; set; } // Mali Durum Belgesi Tarihi
    }
}
