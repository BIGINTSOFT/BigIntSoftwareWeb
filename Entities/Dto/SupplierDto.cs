using System.ComponentModel.DataAnnotations;

namespace Entities.Dto
{
    public class SupplierDto
    {
        public int? Id { get; set; }

        // Temel Tedarikçi Bilgileri
        [Required(ErrorMessage = "Şirket adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir.")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "İletişim kişisi en fazla 100 karakter olabilir.")]
        public string? ContactPerson { get; set; }

        [StringLength(100, ErrorMessage = "İletişim kişisi unvanı en fazla 100 karakter olabilir.")]
        public string? ContactPersonTitle { get; set; }

        [StringLength(100, ErrorMessage = "İletişim kişisi departmanı en fazla 100 karakter olabilir.")]
        public string? ContactPersonDepartment { get; set; }

        // SAP Entegrasyon Alanları
        [StringLength(20, ErrorMessage = "SAP tedarikçi kodu en fazla 20 karakter olabilir.")]
        public string? SapVendorCode { get; set; }

        [StringLength(20, ErrorMessage = "SAP hesap grubu en fazla 20 karakter olabilir.")]
        public string? SapAccountGroup { get; set; }

        [StringLength(20, ErrorMessage = "SAP satın alma organizasyonu en fazla 20 karakter olabilir.")]
        public string? SapPurchasingOrg { get; set; }

        [StringLength(20, ErrorMessage = "SAP şirket kodu en fazla 20 karakter olabilir.")]
        public string? SapCompanyCode { get; set; }

        [StringLength(20, ErrorMessage = "SAP ödeme koşulları en fazla 20 karakter olabilir.")]
        public string? SapPaymentTerms { get; set; }

        [StringLength(20, ErrorMessage = "SAP para birimi en fazla 20 karakter olabilir.")]
        public string? SapCurrency { get; set; }

        [StringLength(20, ErrorMessage = "SAP vergi kodu en fazla 20 karakter olabilir.")]
        public string? SapTaxCode { get; set; }

        [StringLength(20, ErrorMessage = "SAP vergi ülkesi en fazla 20 karakter olabilir.")]
        public string? SapTaxCountry { get; set; }

        // İletişim Bilgileri
        [StringLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        public string? Phone { get; set; }

        [StringLength(20, ErrorMessage = "Cep telefonu en fazla 20 karakter olabilir.")]
        public string? MobilePhone { get; set; }

        [StringLength(20, ErrorMessage = "Faks en fazla 20 karakter olabilir.")]
        public string? Fax { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Geçerli bir web sitesi URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "Web sitesi en fazla 100 karakter olabilir.")]
        public string? Website { get; set; }

        [Url(ErrorMessage = "Geçerli bir LinkedIn URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "LinkedIn en fazla 100 karakter olabilir.")]
        public string? LinkedIn { get; set; }

        [Url(ErrorMessage = "Geçerli bir Twitter URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "Twitter en fazla 100 karakter olabilir.")]
        public string? Twitter { get; set; }

        [Url(ErrorMessage = "Geçerli bir Facebook URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "Facebook en fazla 100 karakter olabilir.")]
        public string? Facebook { get; set; }

        [Url(ErrorMessage = "Geçerli bir Instagram URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "Instagram en fazla 100 karakter olabilir.")]
        public string? Instagram { get; set; }

        // Adres Bilgileri
        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "Şehir en fazla 50 karakter olabilir.")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "İl en fazla 50 karakter olabilir.")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Posta kodu en fazla 20 karakter olabilir.")]
        public string? PostalCode { get; set; }

        [StringLength(50, ErrorMessage = "Ülke en fazla 50 karakter olabilir.")]
        public string? Country { get; set; }

        [StringLength(50, ErrorMessage = "Bölge en fazla 50 karakter olabilir.")]
        public string? Region { get; set; }

        [StringLength(50, ErrorMessage = "İlçe en fazla 50 karakter olabilir.")]
        public string? District { get; set; }

        // Teslimat Adresi
        [StringLength(200, ErrorMessage = "Teslimat adresi en fazla 200 karakter olabilir.")]
        public string? DeliveryAddress { get; set; }

        [StringLength(50, ErrorMessage = "Teslimat şehri en fazla 50 karakter olabilir.")]
        public string? DeliveryCity { get; set; }

        [StringLength(50, ErrorMessage = "Teslimat ili en fazla 50 karakter olabilir.")]
        public string? DeliveryState { get; set; }

        [StringLength(20, ErrorMessage = "Teslimat posta kodu en fazla 20 karakter olabilir.")]
        public string? DeliveryPostalCode { get; set; }

        [StringLength(50, ErrorMessage = "Teslimat ülkesi en fazla 50 karakter olabilir.")]
        public string? DeliveryCountry { get; set; }

        // Fatura Adresi
        [StringLength(200, ErrorMessage = "Fatura adresi en fazla 200 karakter olabilir.")]
        public string? BillingAddress { get; set; }

        [StringLength(50, ErrorMessage = "Fatura şehri en fazla 50 karakter olabilir.")]
        public string? BillingCity { get; set; }

        [StringLength(50, ErrorMessage = "Fatura ili en fazla 50 karakter olabilir.")]
        public string? BillingState { get; set; }

        [StringLength(20, ErrorMessage = "Fatura posta kodu en fazla 20 karakter olabilir.")]
        public string? BillingPostalCode { get; set; }

        [StringLength(50, ErrorMessage = "Fatura ülkesi en fazla 50 karakter olabilir.")]
        public string? BillingCountry { get; set; }

        // Vergi ve Resmi Bilgiler (Türkiye E-Fatura/E-İrsaliye)
        [StringLength(11, ErrorMessage = "TC kimlik numarası 11 karakter olmalıdır.")]
        public string? TcNumber { get; set; }

        [StringLength(10, ErrorMessage = "Vergi numarası 10 karakter olmalıdır.")]
        public string? TaxNumber { get; set; }

        [StringLength(50, ErrorMessage = "Vergi dairesi en fazla 50 karakter olabilir.")]
        public string? TaxOffice { get; set; }

        [StringLength(50, ErrorMessage = "Vergi dairesi kodu en fazla 50 karakter olabilir.")]
        public string? TaxOfficeCode { get; set; }

        [StringLength(20, ErrorMessage = "E-fatura alias en fazla 20 karakter olabilir.")]
        public string? EInvoiceAlias { get; set; }

        [StringLength(50, ErrorMessage = "E-fatura unvan en fazla 50 karakter olabilir.")]
        public string? EInvoiceTitle { get; set; }

        [StringLength(20, ErrorMessage = "E-fatura profili en fazla 20 karakter olabilir.")]
        public string? EInvoiceProfile { get; set; }

        [StringLength(20, ErrorMessage = "E-arşiv profili en fazla 20 karakter olabilir.")]
        public string? EArchiveProfile { get; set; }

        [StringLength(20, ErrorMessage = "E-irsaliye profili en fazla 20 karakter olabilir.")]
        public string? EDeliveryNoteProfile { get; set; }

        [StringLength(20, ErrorMessage = "E-fatura test alias en fazla 20 karakter olabilir.")]
        public string? EInvoiceTestAlias { get; set; }

        [StringLength(20, ErrorMessage = "E-fatura prod alias en fazla 20 karakter olabilir.")]
        public string? EInvoiceProdAlias { get; set; }

        // Avrupa ve Uluslararası Bilgiler
        [StringLength(20, ErrorMessage = "VAT numarası en fazla 20 karakter olabilir.")]
        public string? VatNumber { get; set; }

        [StringLength(10, ErrorMessage = "VAT ülke kodu en fazla 10 karakter olabilir.")]
        public string? VatCountryCode { get; set; }

        [StringLength(50, ErrorMessage = "Hukuki durum en fazla 50 karakter olabilir.")]
        public string? LegalEntityType { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi tipi en fazla 50 karakter olabilir.")]
        public string? SupplierType { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi kategorisi en fazla 50 karakter olabilir.")]
        public string? SupplierCategory { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi grubu en fazla 50 karakter olabilir.")]
        public string? SupplierGroup { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi segmenti en fazla 50 karakter olabilir.")]
        public string? SupplierSegment { get; set; }

        // Banka Bilgileri
        [StringLength(50, ErrorMessage = "Banka adı en fazla 50 karakter olabilir.")]
        public string? BankName { get; set; }

        [StringLength(50, ErrorMessage = "Banka şubesi en fazla 50 karakter olabilir.")]
        public string? BankBranch { get; set; }

        [StringLength(30, ErrorMessage = "Hesap numarası en fazla 30 karakter olabilir.")]
        public string? BankAccountNumber { get; set; }

        [StringLength(20, ErrorMessage = "IBAN en fazla 20 karakter olabilir.")]
        public string? Iban { get; set; }

        [StringLength(20, ErrorMessage = "SWIFT kodu en fazla 20 karakter olabilir.")]
        public string? SwiftCode { get; set; }

        [StringLength(50, ErrorMessage = "Banka adresi en fazla 50 karakter olabilir.")]
        public string? BankAddress { get; set; }

        [StringLength(50, ErrorMessage = "Banka şehri en fazla 50 karakter olabilir.")]
        public string? BankCity { get; set; }

        [StringLength(50, ErrorMessage = "Banka ülkesi en fazla 50 karakter olabilir.")]
        public string? BankCountry { get; set; }

        // Ticari Bilgiler
        [StringLength(50, ErrorMessage = "Ticaret sicil numarası en fazla 50 karakter olabilir.")]
        public string? TradeRegistryNumber { get; set; }

        [StringLength(50, ErrorMessage = "Ticaret odası en fazla 50 karakter olabilir.")]
        public string? ChamberOfCommerce { get; set; }

        [StringLength(50, ErrorMessage = "MERSİS numarası en fazla 50 karakter olabilir.")]
        public string? MersisNumber { get; set; }

        [StringLength(50, ErrorMessage = "Faaliyet kodu en fazla 50 karakter olabilir.")]
        public string? ActivityCode { get; set; }

        [StringLength(200, ErrorMessage = "Faaliyet açıklaması en fazla 200 karakter olabilir.")]
        public string? ActivityDescription { get; set; }

        [StringLength(50, ErrorMessage = "Sektör kodu en fazla 50 karakter olabilir.")]
        public string? IndustryCode { get; set; }

        [StringLength(100, ErrorMessage = "Sektör açıklaması en fazla 100 karakter olabilir.")]
        public string? IndustryDescription { get; set; }

        // Fatura ve Ödeme Bilgileri
        [StringLength(20, ErrorMessage = "Ödeme yöntemi en fazla 20 karakter olabilir.")]
        public string? PaymentMethod { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Ödeme vadesi 0'dan büyük olmalıdır.")]
        public int? PaymentTermDays { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Kredi limiti 0'dan büyük olmalıdır.")]
        public decimal? CreditLimit { get; set; }

        [Range(0, 100, ErrorMessage = "İndirim oranı 0-100 arasında olmalıdır.")]
        public decimal? DiscountRate { get; set; }

        [StringLength(20, ErrorMessage = "Para birimi en fazla 20 karakter olabilir.")]
        public string Currency { get; set; } = "TRY";

        [StringLength(20, ErrorMessage = "Ödeme para birimi en fazla 20 karakter olabilir.")]
        public string? PaymentCurrency { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Döviz kuru 0'dan büyük olmalıdır.")]
        public decimal? ExchangeRate { get; set; }

        // E-Fatura ve E-Arşiv Bilgileri
        public bool IsEInvoiceEnabled { get; set; } = false;

        public bool IsEArchiveEnabled { get; set; } = false;

        public bool IsEDeliveryNoteEnabled { get; set; } = false;

        public bool IsEInvoiceTestMode { get; set; } = true;

        public bool IsEArchiveTestMode { get; set; } = true;

        public bool IsEDeliveryNoteTestMode { get; set; } = true;

        [StringLength(50, ErrorMessage = "E-fatura entegrasyon tipi en fazla 50 karakter olabilir.")]
        public string? EInvoiceIntegrationType { get; set; }

        [StringLength(50, ErrorMessage = "E-fatura servis sağlayıcısı en fazla 50 karakter olabilir.")]
        public string? EInvoiceServiceProvider { get; set; }

        [Url(ErrorMessage = "Geçerli bir API URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "E-fatura API URL en fazla 100 karakter olabilir.")]
        public string? EInvoiceApiUrl { get; set; }

        [StringLength(100, ErrorMessage = "E-fatura kullanıcı adı en fazla 100 karakter olabilir.")]
        public string? EInvoiceUsername { get; set; }

        [StringLength(100, ErrorMessage = "E-fatura şifre en fazla 100 karakter olabilir.")]
        public string? EInvoicePassword { get; set; }

        [StringLength(100, ErrorMessage = "E-fatura sertifika yolu en fazla 100 karakter olabilir.")]
        public string? EInvoiceCertificatePath { get; set; }

        [StringLength(100, ErrorMessage = "E-fatura sertifika şifresi en fazla 100 karakter olabilir.")]
        public string? EInvoiceCertificatePassword { get; set; }

        // Kalite ve Sertifikasyon
        [StringLength(50, ErrorMessage = "Kalite sertifikası en fazla 50 karakter olabilir.")]
        public string? QualityCertification { get; set; }

        [StringLength(50, ErrorMessage = "ISO sertifikası en fazla 50 karakter olabilir.")]
        public string? IsoCertification { get; set; }

        [StringLength(50, ErrorMessage = "Çevre sertifikası en fazla 50 karakter olabilir.")]
        public string? EnvironmentalCertification { get; set; }

        [StringLength(50, ErrorMessage = "Güvenlik sertifikası en fazla 50 karakter olabilir.")]
        public string? SafetyCertification { get; set; }

        public DateTime? CertificationExpiryDate { get; set; }

        // Performans ve Değerlendirme
        [Range(0, 5, ErrorMessage = "Kalite puanı 0-5 arasında olmalıdır.")]
        public decimal? QualityRating { get; set; }

        [Range(0, 5, ErrorMessage = "Teslimat puanı 0-5 arasında olmalıdır.")]
        public decimal? DeliveryRating { get; set; }

        [Range(0, 5, ErrorMessage = "Hizmet puanı 0-5 arasında olmalıdır.")]
        public decimal? ServiceRating { get; set; }

        [Range(0, 5, ErrorMessage = "Fiyat puanı 0-5 arasında olmalıdır.")]
        public decimal? PriceRating { get; set; }

        [Range(0, 5, ErrorMessage = "Genel puan 0-5 arasında olmalıdır.")]
        public decimal? OverallRating { get; set; }

        [StringLength(20, ErrorMessage = "Risk seviyesi en fazla 20 karakter olabilir.")]
        public string? RiskLevel { get; set; }

        [StringLength(20, ErrorMessage = "Kredi notu en fazla 20 karakter olabilir.")]
        public string? CreditRating { get; set; }

        public bool IsBlacklisted { get; set; } = false;

        [StringLength(500, ErrorMessage = "Kara liste sebebi en fazla 500 karakter olabilir.")]
        public string? BlacklistReason { get; set; }

        public DateTime? BlacklistDate { get; set; }

        // Sistem Bilgileri
        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(50, ErrorMessage = "Oluşturan kullanıcı en fazla 50 karakter olabilir.")]
        public string? CreatedBy { get; set; }

        [StringLength(50, ErrorMessage = "Güncelleyen kullanıcı en fazla 50 karakter olabilir.")]
        public string? UpdatedBy { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }

        [StringLength(500, ErrorMessage = "İç notlar en fazla 500 karakter olabilir.")]
        public string? InternalNotes { get; set; }

        [StringLength(500, ErrorMessage = "Kalite notları en fazla 500 karakter olabilir.")]
        public string? QualityNotes { get; set; }

        [StringLength(500, ErrorMessage = "Ödeme notları en fazla 500 karakter olabilir.")]
        public string? PaymentNotes { get; set; }

        // Kategorizasyon
        [StringLength(50, ErrorMessage = "Tedarikçi kaynağı en fazla 50 karakter olabilir.")]
        public string? Source { get; set; }

        [StringLength(50, ErrorMessage = "Öncelik en fazla 50 karakter olabilir.")]
        public string? Priority { get; set; }

        [StringLength(50, ErrorMessage = "Durum en fazla 50 karakter olabilir.")]
        public string? Status { get; set; }

        // İstatistik Bilgileri
        [Range(0, int.MaxValue, ErrorMessage = "Toplam sipariş sayısı 0'dan büyük olmalıdır.")]
        public int? TotalOrders { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam satın alma tutarı 0'dan büyük olmalıdır.")]
        public decimal? TotalPurchases { get; set; }

        public DateTime? LastOrderDate { get; set; }

        public DateTime? LastContactDate { get; set; }

        public DateTime? LastDeliveryDate { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        // Yasal Uyumluluk
        public bool GdprConsent { get; set; } = false;

        public DateTime? GdprConsentDate { get; set; }

        public bool MarketingConsent { get; set; } = false;

        public bool SmsConsent { get; set; } = false;

        public bool EmailConsent { get; set; } = false;

        // Dil ve Bölgesel Ayarlar
        [StringLength(10, ErrorMessage = "Dil en fazla 10 karakter olabilir.")]
        public string Language { get; set; } = "tr-TR";

        [StringLength(10, ErrorMessage = "Saat dilimi en fazla 10 karakter olabilir.")]
        public string TimeZone { get; set; } = "Turkey Standard Time";

        [StringLength(10, ErrorMessage = "Tarih formatı en fazla 10 karakter olabilir.")]
        public string DateFormat { get; set; } = "dd.MM.yyyy";

        [StringLength(10, ErrorMessage = "Sayı formatı en fazla 10 karakter olabilir.")]
        public string NumberFormat { get; set; } = "tr-TR";

        // Ek SAP Alanları
        [StringLength(20, ErrorMessage = "SAP tedarikçi hesabı en fazla 20 karakter olabilir.")]
        public string? SapVendorAccount { get; set; }

        [StringLength(20, ErrorMessage = "SAP mutabakat hesabı en fazla 20 karakter olabilir.")]
        public string? SapReconciliationAccount { get; set; }

        [StringLength(20, ErrorMessage = "SAP sıralama anahtarı en fazla 20 karakter olabilir.")]
        public string? SapSortKey { get; set; }

        [StringLength(20, ErrorMessage = "SAP ödeme engeli en fazla 20 karakter olabilir.")]
        public string? SapPaymentBlock { get; set; }

        [StringLength(20, ErrorMessage = "SAP kayıt engeli en fazla 20 karakter olabilir.")]
        public string? SapPostingBlock { get; set; }

        [StringLength(20, ErrorMessage = "SAP satın alma engeli en fazla 20 karakter olabilir.")]
        public string? SapPurchasingBlock { get; set; }

        [StringLength(20, ErrorMessage = "SAP sipariş para birimi en fazla 20 karakter olabilir.")]
        public string? SapOrderCurrency { get; set; }

        [StringLength(20, ErrorMessage = "SAP teslim şartları en fazla 20 karakter olabilir.")]
        public string? SapIncoterms { get; set; }

        [StringLength(20, ErrorMessage = "SAP taşıma bölgesi en fazla 20 karakter olabilir.")]
        public string? SapTransportationZone { get; set; }

        [StringLength(20, ErrorMessage = "SAP sevkiyat koşulu en fazla 20 karakter olabilir.")]
        public string? SapShippingCondition { get; set; }

        [StringLength(20, ErrorMessage = "SAP teslimat önceliği en fazla 20 karakter olabilir.")]
        public string? SapDeliveryPriority { get; set; }

        [StringLength(20, ErrorMessage = "SAP minimum sipariş değeri en fazla 20 karakter olabilir.")]
        public string? SapMinimumOrderValue { get; set; }

        [StringLength(20, ErrorMessage = "SAP maksimum sipariş değeri en fazla 20 karakter olabilir.")]
        public string? SapMaximumOrderValue { get; set; }

        // E-İrsaliye Özel Alanları
        [StringLength(50, ErrorMessage = "E-irsaliye alias en fazla 50 karakter olabilir.")]
        public string? EDeliveryNoteAlias { get; set; }

        [StringLength(50, ErrorMessage = "E-irsaliye unvan en fazla 50 karakter olabilir.")]
        public string? EDeliveryNoteTitle { get; set; }

        [StringLength(50, ErrorMessage = "E-irsaliye entegrasyon tipi en fazla 50 karakter olabilir.")]
        public string? EDeliveryNoteIntegrationType { get; set; }

        [StringLength(50, ErrorMessage = "E-irsaliye servis sağlayıcısı en fazla 50 karakter olabilir.")]
        public string? EDeliveryNoteServiceProvider { get; set; }

        [Url(ErrorMessage = "Geçerli bir API URL'si giriniz.")]
        [StringLength(100, ErrorMessage = "E-irsaliye API URL en fazla 100 karakter olabilir.")]
        public string? EDeliveryNoteApiUrl { get; set; }

        [StringLength(100, ErrorMessage = "E-irsaliye kullanıcı adı en fazla 100 karakter olabilir.")]
        public string? EDeliveryNoteUsername { get; set; }

        [StringLength(100, ErrorMessage = "E-irsaliye şifre en fazla 100 karakter olabilir.")]
        public string? EDeliveryNotePassword { get; set; }

        // Tedarikçi Özel Alanları
        [StringLength(50, ErrorMessage = "Tedarikçi kodu en fazla 50 karakter olabilir.")]
        public string? SupplierCode { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi adı en fazla 50 karakter olabilir.")]
        public string? SupplierName { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi kısa adı en fazla 50 karakter olabilir.")]
        public string? SupplierShortName { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi görünen adı en fazla 50 karakter olabilir.")]
        public string? SupplierDisplayName { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi yasal adı en fazla 50 karakter olabilir.")]
        public string? SupplierLegalName { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi ticari adı en fazla 50 karakter olabilir.")]
        public string? SupplierCommercialName { get; set; }

        [StringLength(50, ErrorMessage = "Tedarikçi marka adı en fazla 50 karakter olabilir.")]
        public string? SupplierBrandName { get; set; }

        // İletişim Özel Alanları
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "Birincil iletişim e-postası en fazla 100 karakter olabilir.")]
        public string? PrimaryContactEmail { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "İkincil iletişim e-postası en fazla 100 karakter olabilir.")]
        public string? SecondaryContactEmail { get; set; }

        [StringLength(20, ErrorMessage = "Birincil iletişim telefonu en fazla 20 karakter olabilir.")]
        public string? PrimaryContactPhone { get; set; }

        [StringLength(20, ErrorMessage = "İkincil iletişim telefonu en fazla 20 karakter olabilir.")]
        public string? SecondaryContactPhone { get; set; }

        [StringLength(100, ErrorMessage = "Acil durum iletişim kişisi en fazla 100 karakter olabilir.")]
        public string? EmergencyContactName { get; set; }

        [StringLength(20, ErrorMessage = "Acil durum iletişim telefonu en fazla 20 karakter olabilir.")]
        public string? EmergencyContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "Acil durum iletişim e-postası en fazla 100 karakter olabilir.")]
        public string? EmergencyContactEmail { get; set; }

        // Finansal Bilgiler
        [Range(0, double.MaxValue, ErrorMessage = "Yıllık ciro 0'dan büyük olmalıdır.")]
        public decimal? AnnualRevenue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Net değer 0'dan büyük olmalıdır.")]
        public decimal? NetWorth { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam varlık 0'dan büyük olmalıdır.")]
        public decimal? TotalAssets { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam borç 0'dan büyük olmalıdır.")]
        public decimal? TotalLiabilities { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Çalışan sayısı 0'dan büyük olmalıdır.")]
        public int? EmployeeCount { get; set; }

        public DateTime? EstablishmentDate { get; set; }

        [StringLength(50, ErrorMessage = "Mülkiyet tipi en fazla 50 karakter olabilir.")]
        public string? OwnershipType { get; set; }

        [StringLength(50, ErrorMessage = "İşletme büyüklüğü en fazla 50 karakter olabilir.")]
        public string? BusinessSize { get; set; }

        // Tedarikçi Performans Metrikleri
        [Range(0, 100, ErrorMessage = "Zamanında teslimat oranı 0-100 arasında olmalıdır.")]
        public decimal? OnTimeDeliveryRate { get; set; }

        [Range(0, 100, ErrorMessage = "Kalite kabul oranı 0-100 arasında olmalıdır.")]
        public decimal? QualityAcceptanceRate { get; set; }

        [Range(0, 100, ErrorMessage = "Fiyat rekabetçiliği 0-100 arasında olmalıdır.")]
        public decimal? PriceCompetitiveness { get; set; }

        [Range(0, 100, ErrorMessage = "Hizmet seviyesi 0-100 arasında olmalıdır.")]
        public decimal? ServiceLevel { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Ortalama teslimat günü 0'dan büyük olmalıdır.")]
        public int? AverageDeliveryDays { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum sipariş miktarı 0'dan büyük olmalıdır.")]
        public int? MinimumOrderQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Maksimum sipariş miktarı 0'dan büyük olmalıdır.")]
        public int? MaximumOrderQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum sipariş değeri 0'dan büyük olmalıdır.")]
        public decimal? MinimumOrderValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maksimum sipariş değeri 0'dan büyük olmalıdır.")]
        public decimal? MaximumOrderValue { get; set; }

        // Tedarikçi Sertifikaları ve Belgeleri
        [StringLength(100, ErrorMessage = "İşletme ruhsatı en fazla 100 karakter olabilir.")]
        public string? BusinessLicense { get; set; }

        [StringLength(100, ErrorMessage = "Vergi levhası en fazla 100 karakter olabilir.")]
        public string? TaxCertificate { get; set; }

        [StringLength(100, ErrorMessage = "Ticaret sicil belgesi en fazla 100 karakter olabilir.")]
        public string? TradeRegistryCertificate { get; set; }

        [StringLength(100, ErrorMessage = "Ticaret odası belgesi en fazla 100 karakter olabilir.")]
        public string? ChamberOfCommerceCertificate { get; set; }

        [StringLength(100, ErrorMessage = "Sigorta belgesi en fazla 100 karakter olabilir.")]
        public string? InsuranceCertificate { get; set; }

        [StringLength(100, ErrorMessage = "Mali durum belgesi en fazla 100 karakter olabilir.")]
        public string? FinancialStatement { get; set; }

        public DateTime? BusinessLicenseExpiryDate { get; set; }

        public DateTime? TaxCertificateExpiryDate { get; set; }

        public DateTime? TradeRegistryExpiryDate { get; set; }

        public DateTime? ChamberOfCommerceExpiryDate { get; set; }

        public DateTime? InsuranceExpiryDate { get; set; }

        public DateTime? FinancialStatementDate { get; set; }
    }
}
