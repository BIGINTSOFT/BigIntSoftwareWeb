using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; }

        // Temel Müşteri Bilgileri
        [Required(ErrorMessage = "Şirket adı zorunludur")]
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "İletişim kişisi en fazla 100 karakter olabilir")]
        public string ContactPerson { get; set; } = string.Empty;

        // İletişim Bilgileri
        [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Cep telefonu en fazla 20 karakter olabilir")]
        public string MobilePhone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir web sitesi adresi giriniz")]
        [StringLength(100, ErrorMessage = "Web sitesi en fazla 100 karakter olabilir")]
        public string Website { get; set; } = string.Empty;

        // Adres Bilgileri
        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string Address { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Şehir en fazla 50 karakter olabilir")]
        public string City { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "İl en fazla 50 karakter olabilir")]
        public string State { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Posta kodu en fazla 20 karakter olabilir")]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ülke en fazla 50 karakter olabilir")]
        public string Country { get; set; } = string.Empty;

        // Vergi ve Resmi Bilgiler (Türkiye E-Fatura)
        [StringLength(11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcNumber { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Vergi numarası 10 karakter olmalıdır")]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Vergi dairesi en fazla 50 karakter olabilir")]
        public string TaxOffice { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Vergi dairesi kodu en fazla 50 karakter olabilir")]
        public string TaxOfficeCode { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "E-Fatura alias en fazla 20 karakter olabilir")]
        public string EInvoiceAlias { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "E-Fatura unvan en fazla 50 karakter olabilir")]
        public string EInvoiceTitle { get; set; } = string.Empty;

        // Avrupa ve Uluslararası Bilgiler
        [StringLength(20, ErrorMessage = "VAT numarası en fazla 20 karakter olabilir")]
        public string VatNumber { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "VAT ülke kodu en fazla 10 karakter olabilir")]
        public string VatCountryCode { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Hukuki durum en fazla 50 karakter olabilir")]
        public string LegalEntityType { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Müşteri tipi en fazla 50 karakter olabilir")]
        public string CustomerType { get; set; } = string.Empty;

        // Banka Bilgileri
        [StringLength(50, ErrorMessage = "Banka adı en fazla 50 karakter olabilir")]
        public string BankName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Banka şubesi en fazla 50 karakter olabilir")]
        public string BankBranch { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "Hesap numarası en fazla 30 karakter olabilir")]
        public string BankAccountNumber { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "IBAN en fazla 20 karakter olabilir")]
        public string Iban { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "SWIFT kodu en fazla 20 karakter olabilir")]
        public string SwiftCode { get; set; } = string.Empty;

        // Ticari Bilgiler
        [StringLength(50, ErrorMessage = "Ticaret sicil no en fazla 50 karakter olabilir")]
        public string TradeRegistryNumber { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ticaret odası en fazla 50 karakter olabilir")]
        public string ChamberOfCommerce { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "MERSİS numarası en fazla 50 karakter olabilir")]
        public string MersisNumber { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Faaliyet kodu en fazla 50 karakter olabilir")]
        public string ActivityCode { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Faaliyet açıklaması en fazla 200 karakter olabilir")]
        public string ActivityDescription { get; set; } = string.Empty;

        // Fatura ve Ödeme Bilgileri
        [StringLength(20, ErrorMessage = "Ödeme yöntemi en fazla 20 karakter olabilir")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Ödeme vadesi pozitif bir sayı olmalıdır")]
        public int? PaymentTermDays { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Kredi limiti pozitif bir sayı olmalıdır")]
        public decimal? CreditLimit { get; set; }

        [Range(0, 100, ErrorMessage = "İndirim oranı 0-100 arasında olmalıdır")]
        public decimal? DiscountRate { get; set; }

        [StringLength(20, ErrorMessage = "Para birimi en fazla 20 karakter olabilir")]
        public string Currency { get; set; } = "TRY";

        // E-Fatura ve E-Arşiv Bilgileri
        public bool IsEInvoiceEnabled { get; set; } = false;

        public bool IsEArchiveEnabled { get; set; } = false;

        [StringLength(50, ErrorMessage = "E-Fatura profili en fazla 50 karakter olabilir")]
        public string EInvoiceProfile { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "E-Arşiv profili en fazla 50 karakter olabilir")]
        public string EArchiveProfile { get; set; } = string.Empty;

        // Sistem Bilgileri
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50, ErrorMessage = "Oluşturan en fazla 50 karakter olabilir")]
        public string CreatedBy { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Güncelleyen en fazla 50 karakter olabilir")]
        public string UpdatedBy { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string Notes { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "İç notlar en fazla 500 karakter olabilir")]
        public string InternalNotes { get; set; } = string.Empty;

        // Kategorizasyon
        [StringLength(50, ErrorMessage = "Müşteri grubu en fazla 50 karakter olabilir")]
        public string CustomerGroup { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Müşteri segmenti en fazla 50 karakter olabilir")]
        public string CustomerSegment { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Müşteri kaynağı en fazla 50 karakter olabilir")]
        public string Source { get; set; } = string.Empty;

        // İstatistik Bilgileri
        [Range(0, int.MaxValue, ErrorMessage = "Toplam sipariş sayısı pozitif bir sayı olmalıdır")]
        public int? TotalOrders { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Toplam satış tutarı pozitif bir sayı olmalıdır")]
        public decimal? TotalSales { get; set; } = 0;

        public DateTime? LastOrderDate { get; set; }

        public DateTime? LastContactDate { get; set; }

        // Sosyal Medya ve İletişim
        [Url(ErrorMessage = "Geçerli bir LinkedIn adresi giriniz")]
        [StringLength(100, ErrorMessage = "LinkedIn en fazla 100 karakter olabilir")]
        public string LinkedIn { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir Twitter adresi giriniz")]
        [StringLength(100, ErrorMessage = "Twitter en fazla 100 karakter olabilir")]
        public string Twitter { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir Facebook adresi giriniz")]
        [StringLength(100, ErrorMessage = "Facebook en fazla 100 karakter olabilir")]
        public string Facebook { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir Instagram adresi giriniz")]
        [StringLength(100, ErrorMessage = "Instagram en fazla 100 karakter olabilir")]
        public string Instagram { get; set; } = string.Empty;

        // Ek Adres Bilgileri
        [StringLength(200, ErrorMessage = "Teslimat adresi en fazla 200 karakter olabilir")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Fatura adresi en fazla 200 karakter olabilir")]
        public string BillingAddress { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Teslimat şehri en fazla 50 karakter olabilir")]
        public string DeliveryCity { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Fatura şehri en fazla 50 karakter olabilir")]
        public string BillingCity { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Teslimat posta kodu en fazla 20 karakter olabilir")]
        public string DeliveryPostalCode { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Fatura posta kodu en fazla 20 karakter olabilir")]
        public string BillingPostalCode { get; set; } = string.Empty;

        // Risk ve Kredi Bilgileri
        [StringLength(20, ErrorMessage = "Risk seviyesi en fazla 20 karakter olabilir")]
        public string RiskLevel { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Kredi notu en fazla 20 karakter olabilir")]
        public string CreditRating { get; set; } = string.Empty;

        public bool IsBlacklisted { get; set; } = false;

        [StringLength(500, ErrorMessage = "Kara liste sebebi en fazla 500 karakter olabilir")]
        public string BlacklistReason { get; set; } = string.Empty;

        // Yasal Uyumluluk
        public bool GdprConsent { get; set; } = false;

        public DateTime? GdprConsentDate { get; set; }

        public bool MarketingConsent { get; set; } = false;

        public bool SmsConsent { get; set; } = false;

        public bool EmailConsent { get; set; } = false;

        // Dil ve Bölgesel Ayarlar
        [StringLength(10, ErrorMessage = "Dil en fazla 10 karakter olabilir")]
        public string Language { get; set; } = "tr-TR";

        [StringLength(10, ErrorMessage = "Saat dilimi en fazla 10 karakter olabilir")]
        public string TimeZone { get; set; } = "Turkey Standard Time";

        [StringLength(10, ErrorMessage = "Tarih formatı en fazla 10 karakter olabilir")]
        public string DateFormat { get; set; } = "dd.MM.yyyy";

        [StringLength(10, ErrorMessage = "Sayı formatı en fazla 10 karakter olabilir")]
        public string NumberFormat { get; set; } = "tr-TR";
    }
}
