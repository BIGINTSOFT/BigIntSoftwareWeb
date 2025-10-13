using Microsoft.AspNetCore.Mvc;
using Bussiness.Repository.Abstract;
using Entities.Dto;
using Entities.Entity;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;

        public SuppliersController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        // Ana sayfa
        public IActionResult Index()
        {
            return View();
        }

        // Tedarikçi listesi - DevExtreme DataGrid için
        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                // Tüm tedarikçi verilerini getir
                var suppliers = await _supplierRepository.GetAllAsync();
                
                // Debug bilgisi
                Console.WriteLine($"Toplam tedarikçi sayısı: {suppliers?.Count() ?? 0}");
                
                return Json(new { 
                    success = true, 
                    data = suppliers,
                    count = suppliers?.Count() ?? 0
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return Json(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // Tek tedarikçi getir - Edit için
        [HttpGet]
        public async Task<IActionResult> GetSupplier(int id)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier == null)
                {
                    return Json(new { 
                        success = false, 
                        error = "Tedarikçi bulunamadı." 
                    });
                }

                return Json(new { 
                    success = true, 
                    data = supplier 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    error = ex.Message 
                });
            }
        }

        // Yeni tedarikçi oluştur - Raw Body ile
        [HttpPost]
        [Route("Suppliers/CreateSupplierJson")]
        public async Task<IActionResult> CreateSupplierJson()
        {
            try
            {
                // Raw body'yi oku
                string jsonData;
                using (var reader = new StreamReader(Request.Body))
                {
                    jsonData = await reader.ReadToEndAsync();
                }

                // Detaylı debug log
                Console.WriteLine("=== CreateSupplierJson Debug ===");
                Console.WriteLine($"Request Content-Type: {Request.ContentType}");
                Console.WriteLine($"Request Content-Length: {Request.ContentLength}");
                Console.WriteLine($"jsonData null mu: {jsonData == null}");
                Console.WriteLine($"jsonData empty mu: {string.IsNullOrEmpty(jsonData)}");
                Console.WriteLine($"jsonData length: {jsonData?.Length ?? 0}");
                Console.WriteLine($"Gelen JSON: '{jsonData}'");
                Console.WriteLine("================================");

                // JSON string kontrolü
                if (string.IsNullOrEmpty(jsonData))
                {
                    Console.WriteLine("JSON data null veya boş!");
                    return Json(new { 
                        success = false, 
                        message = "Tedarikçi bilgileri alınamadı." 
                    });
                }

                // JSON'u SupplierDto'ya dönüştür
                SupplierDto? supplierDto;
                try
                {
                    // Önce Id alanını temizle (create işlemi için)
                    var jsonObject = System.Text.Json.JsonDocument.Parse(jsonData);
                    var root = jsonObject.RootElement;
                    
                    // Id alanını kaldır veya null yap
                    if (root.TryGetProperty("Id", out var idElement))
                    {
                        // Id alanını kaldırmak için yeni bir JSON oluştur
                        var jsonDict = new Dictionary<string, object>();
                        foreach (var property in root.EnumerateObject())
                        {
                            if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                            {
                                // Id alanını null olarak ayarla
                                jsonDict[property.Name] = null;
                            }
                            else if (property.Name.Equals("CertificationExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("BusinessLicenseExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("TaxCertificateExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("TradeRegistryExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("ChamberOfCommerceExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("InsuranceExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("FinancialStatementDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("EstablishmentDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("LastOrderDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("LastContactDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("LastDeliveryDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("LastPaymentDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("BlacklistDate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("GdprConsentDate", StringComparison.OrdinalIgnoreCase))
                            {
                                // DateTime alanları için boş string kontrolü
                                var stringValue = property.Value.GetString();
                                jsonDict[property.Name] = string.IsNullOrEmpty(stringValue) ? null : stringValue;
                            }
                            else if (property.Name.Equals("QualityRating", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("DeliveryRating", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("ServiceRating", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("PriceRating", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("OverallRating", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("CreditLimit", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("DiscountRate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("ExchangeRate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("TotalPurchases", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("AnnualRevenue", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("NetWorth", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("TotalAssets", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("TotalLiabilities", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("OnTimeDeliveryRate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("QualityAcceptanceRate", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("PriceCompetitiveness", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("ServiceLevel", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("MinimumOrderValue", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("MaximumOrderValue", StringComparison.OrdinalIgnoreCase))
                            {
                                // Decimal alanları için boş string kontrolü
                                var stringValue = property.Value.GetString();
                                if (string.IsNullOrEmpty(stringValue))
                                {
                                    jsonDict[property.Name] = null;
                                }
                                else if (decimal.TryParse(stringValue, out var decimalValue))
                                {
                                    jsonDict[property.Name] = decimalValue;
                                }
                                else
                                {
                                    jsonDict[property.Name] = null;
                                }
                            }
                            else if (property.Name.Equals("PaymentTermDays", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("TotalOrders", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("EmployeeCount", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("AverageDeliveryDays", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("MinimumOrderQuantity", StringComparison.OrdinalIgnoreCase) ||
                                     property.Name.Equals("MaximumOrderQuantity", StringComparison.OrdinalIgnoreCase))
                            {
                                // Integer alanları için boş string kontrolü
                                var stringValue = property.Value.GetString();
                                if (string.IsNullOrEmpty(stringValue))
                                {
                                    jsonDict[property.Name] = null;
                                }
                                else if (int.TryParse(stringValue, out var intValue))
                                {
                                    jsonDict[property.Name] = intValue;
                                }
                                else
                                {
                                    jsonDict[property.Name] = null;
                                }
                            }
                            else
                            {
                                // Diğer alanları olduğu gibi ekle
                                jsonDict[property.Name] = property.Value.ValueKind switch
                                {
                                    JsonValueKind.String => property.Value.GetString(),
                                    JsonValueKind.Number => property.Value.GetDecimal(),
                                    JsonValueKind.True => true,
                                    JsonValueKind.False => false,
                                    JsonValueKind.Null => null,
                                    _ => property.Value.ToString()
                                };
                            }
                        }
                        
                        // Temizlenmiş JSON'u serialize et
                        var cleanJson = System.Text.Json.JsonSerializer.Serialize(jsonDict);
                        Console.WriteLine($"Temizlenmiş JSON: {cleanJson}");
                        
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                        };
                        
                        supplierDto = System.Text.Json.JsonSerializer.Deserialize<SupplierDto>(cleanJson, options);
                    }
                    else
                    {
                        // Id alanı yoksa direkt deserialize et
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                        };
                        
                        supplierDto = System.Text.Json.JsonSerializer.Deserialize<SupplierDto>(jsonData, options);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialize hatası: {ex.Message}");
                    return Json(new { 
                        success = false, 
                        message = "Geçersiz veri formatı." 
                    });
                }

                // Null kontrolü
                if (supplierDto == null)
                {
                    Console.WriteLine("supplierDto null!");
                    return Json(new { 
                        success = false, 
                        message = "Tedarikçi bilgileri alınamadı." 
                    });
                }

                Console.WriteLine($"Deserialize edilen - CompanyName: {supplierDto.CompanyName}");

                var supplier = new Suppliers
                {
                    CompanyName = supplierDto.CompanyName,
                    FirstName = supplierDto.FirstName,
                    LastName = supplierDto.LastName,
                    ContactPerson = supplierDto.ContactPerson,
                    ContactPersonTitle = supplierDto.ContactPersonTitle,
                    ContactPersonDepartment = supplierDto.ContactPersonDepartment,
                    SapVendorCode = supplierDto.SapVendorCode,
                    SapAccountGroup = supplierDto.SapAccountGroup,
                    SapPurchasingOrg = supplierDto.SapPurchasingOrg,
                    SapCompanyCode = supplierDto.SapCompanyCode,
                    SapPaymentTerms = supplierDto.SapPaymentTerms,
                    SapCurrency = supplierDto.SapCurrency,
                    SapTaxCode = supplierDto.SapTaxCode,
                    SapTaxCountry = supplierDto.SapTaxCountry,
                    Phone = supplierDto.Phone,
                    MobilePhone = supplierDto.MobilePhone,
                    Fax = supplierDto.Fax,
                    Email = supplierDto.Email,
                    Website = supplierDto.Website,
                    LinkedIn = supplierDto.LinkedIn,
                    Twitter = supplierDto.Twitter,
                    Facebook = supplierDto.Facebook,
                    Instagram = supplierDto.Instagram,
                    Address = supplierDto.Address,
                    City = supplierDto.City,
                    State = supplierDto.State,
                    PostalCode = supplierDto.PostalCode,
                    Country = supplierDto.Country,
                    Region = supplierDto.Region,
                    District = supplierDto.District,
                    DeliveryAddress = supplierDto.DeliveryAddress,
                    DeliveryCity = supplierDto.DeliveryCity,
                    DeliveryState = supplierDto.DeliveryState,
                    DeliveryPostalCode = supplierDto.DeliveryPostalCode,
                    DeliveryCountry = supplierDto.DeliveryCountry,
                    BillingAddress = supplierDto.BillingAddress,
                    BillingCity = supplierDto.BillingCity,
                    BillingState = supplierDto.BillingState,
                    BillingPostalCode = supplierDto.BillingPostalCode,
                    BillingCountry = supplierDto.BillingCountry,
                    TcNumber = supplierDto.TcNumber,
                    TaxNumber = supplierDto.TaxNumber,
                    TaxOffice = supplierDto.TaxOffice,
                    TaxOfficeCode = supplierDto.TaxOfficeCode,
                    EInvoiceAlias = supplierDto.EInvoiceAlias,
                    EInvoiceTitle = supplierDto.EInvoiceTitle,
                    EInvoiceProfile = supplierDto.EInvoiceProfile,
                    EArchiveProfile = supplierDto.EArchiveProfile,
                    EDeliveryNoteProfile = supplierDto.EDeliveryNoteProfile,
                    EInvoiceTestAlias = supplierDto.EInvoiceTestAlias,
                    EInvoiceProdAlias = supplierDto.EInvoiceProdAlias,
                    VatNumber = supplierDto.VatNumber,
                    VatCountryCode = supplierDto.VatCountryCode,
                    LegalEntityType = supplierDto.LegalEntityType,
                    SupplierType = supplierDto.SupplierType,
                    SupplierCategory = supplierDto.SupplierCategory,
                    SupplierGroup = supplierDto.SupplierGroup,
                    SupplierSegment = supplierDto.SupplierSegment,
                    BankName = supplierDto.BankName,
                    BankBranch = supplierDto.BankBranch,
                    BankAccountNumber = supplierDto.BankAccountNumber,
                    Iban = supplierDto.Iban,
                    SwiftCode = supplierDto.SwiftCode,
                    BankAddress = supplierDto.BankAddress,
                    BankCity = supplierDto.BankCity,
                    BankCountry = supplierDto.BankCountry,
                    TradeRegistryNumber = supplierDto.TradeRegistryNumber,
                    ChamberOfCommerce = supplierDto.ChamberOfCommerce,
                    MersisNumber = supplierDto.MersisNumber,
                    ActivityCode = supplierDto.ActivityCode,
                    ActivityDescription = supplierDto.ActivityDescription,
                    IndustryCode = supplierDto.IndustryCode,
                    IndustryDescription = supplierDto.IndustryDescription,
                    PaymentMethod = supplierDto.PaymentMethod,
                    PaymentTermDays = supplierDto.PaymentTermDays,
                    CreditLimit = supplierDto.CreditLimit,
                    DiscountRate = supplierDto.DiscountRate,
                    Currency = supplierDto.Currency,
                    PaymentCurrency = supplierDto.PaymentCurrency,
                    ExchangeRate = supplierDto.ExchangeRate,
                    IsEInvoiceEnabled = supplierDto.IsEInvoiceEnabled,
                    IsEArchiveEnabled = supplierDto.IsEArchiveEnabled,
                    IsEDeliveryNoteEnabled = supplierDto.IsEDeliveryNoteEnabled,
                    IsEInvoiceTestMode = supplierDto.IsEInvoiceTestMode,
                    IsEArchiveTestMode = supplierDto.IsEArchiveTestMode,
                    IsEDeliveryNoteTestMode = supplierDto.IsEDeliveryNoteTestMode,
                    EInvoiceIntegrationType = supplierDto.EInvoiceIntegrationType,
                    EInvoiceServiceProvider = supplierDto.EInvoiceServiceProvider,
                    EInvoiceApiUrl = supplierDto.EInvoiceApiUrl,
                    EInvoiceUsername = supplierDto.EInvoiceUsername,
                    EInvoicePassword = supplierDto.EInvoicePassword,
                    EInvoiceCertificatePath = supplierDto.EInvoiceCertificatePath,
                    EInvoiceCertificatePassword = supplierDto.EInvoiceCertificatePassword,
                    QualityCertification = supplierDto.QualityCertification,
                    IsoCertification = supplierDto.IsoCertification,
                    EnvironmentalCertification = supplierDto.EnvironmentalCertification,
                    SafetyCertification = supplierDto.SafetyCertification,
                    CertificationExpiryDate = supplierDto.CertificationExpiryDate,
                    QualityRating = supplierDto.QualityRating,
                    DeliveryRating = supplierDto.DeliveryRating,
                    ServiceRating = supplierDto.ServiceRating,
                    PriceRating = supplierDto.PriceRating,
                    OverallRating = supplierDto.OverallRating,
                    RiskLevel = supplierDto.RiskLevel,
                    CreditRating = supplierDto.CreditRating,
                    IsBlacklisted = supplierDto.IsBlacklisted,
                    BlacklistReason = supplierDto.BlacklistReason,
                    BlacklistDate = supplierDto.BlacklistDate,
                    IsActive = supplierDto.IsActive,
                    Notes = supplierDto.Notes,
                    InternalNotes = supplierDto.InternalNotes,
                    QualityNotes = supplierDto.QualityNotes,
                    PaymentNotes = supplierDto.PaymentNotes,
                    Source = supplierDto.Source,
                    Priority = supplierDto.Priority,
                    Status = supplierDto.Status,
                    TotalOrders = supplierDto.TotalOrders,
                    TotalPurchases = supplierDto.TotalPurchases,
                    LastOrderDate = supplierDto.LastOrderDate,
                    LastContactDate = supplierDto.LastContactDate,
                    LastDeliveryDate = supplierDto.LastDeliveryDate,
                    LastPaymentDate = supplierDto.LastPaymentDate,
                    GdprConsent = supplierDto.GdprConsent,
                    GdprConsentDate = supplierDto.GdprConsentDate,
                    MarketingConsent = supplierDto.MarketingConsent,
                    SmsConsent = supplierDto.SmsConsent,
                    EmailConsent = supplierDto.EmailConsent,
                    Language = supplierDto.Language,
                    TimeZone = supplierDto.TimeZone,
                    DateFormat = supplierDto.DateFormat,
                    NumberFormat = supplierDto.NumberFormat,
                    SapVendorAccount = supplierDto.SapVendorAccount,
                    SapReconciliationAccount = supplierDto.SapReconciliationAccount,
                    SapSortKey = supplierDto.SapSortKey,
                    SapPaymentBlock = supplierDto.SapPaymentBlock,
                    SapPostingBlock = supplierDto.SapPostingBlock,
                    SapPurchasingBlock = supplierDto.SapPurchasingBlock,
                    SapOrderCurrency = supplierDto.SapOrderCurrency,
                    SapIncoterms = supplierDto.SapIncoterms,
                    SapTransportationZone = supplierDto.SapTransportationZone,
                    SapShippingCondition = supplierDto.SapShippingCondition,
                    SapDeliveryPriority = supplierDto.SapDeliveryPriority,
                    SapMinimumOrderValue = supplierDto.SapMinimumOrderValue,
                    SapMaximumOrderValue = supplierDto.SapMaximumOrderValue,
                    EDeliveryNoteAlias = supplierDto.EDeliveryNoteAlias,
                    EDeliveryNoteTitle = supplierDto.EDeliveryNoteTitle,
                    EDeliveryNoteIntegrationType = supplierDto.EDeliveryNoteIntegrationType,
                    EDeliveryNoteServiceProvider = supplierDto.EDeliveryNoteServiceProvider,
                    EDeliveryNoteApiUrl = supplierDto.EDeliveryNoteApiUrl,
                    EDeliveryNoteUsername = supplierDto.EDeliveryNoteUsername,
                    EDeliveryNotePassword = supplierDto.EDeliveryNotePassword,
                    SupplierCode = supplierDto.SupplierCode,
                    SupplierName = supplierDto.SupplierName,
                    SupplierShortName = supplierDto.SupplierShortName,
                    SupplierDisplayName = supplierDto.SupplierDisplayName,
                    SupplierLegalName = supplierDto.SupplierLegalName,
                    SupplierCommercialName = supplierDto.SupplierCommercialName,
                    SupplierBrandName = supplierDto.SupplierBrandName,
                    PrimaryContactEmail = supplierDto.PrimaryContactEmail,
                    SecondaryContactEmail = supplierDto.SecondaryContactEmail,
                    PrimaryContactPhone = supplierDto.PrimaryContactPhone,
                    SecondaryContactPhone = supplierDto.SecondaryContactPhone,
                    EmergencyContactName = supplierDto.EmergencyContactName,
                    EmergencyContactPhone = supplierDto.EmergencyContactPhone,
                    EmergencyContactEmail = supplierDto.EmergencyContactEmail,
                    AnnualRevenue = supplierDto.AnnualRevenue,
                    NetWorth = supplierDto.NetWorth,
                    TotalAssets = supplierDto.TotalAssets,
                    TotalLiabilities = supplierDto.TotalLiabilities,
                    EmployeeCount = supplierDto.EmployeeCount,
                    EstablishmentDate = supplierDto.EstablishmentDate,
                    OwnershipType = supplierDto.OwnershipType,
                    BusinessSize = supplierDto.BusinessSize,
                    OnTimeDeliveryRate = supplierDto.OnTimeDeliveryRate,
                    QualityAcceptanceRate = supplierDto.QualityAcceptanceRate,
                    PriceCompetitiveness = supplierDto.PriceCompetitiveness,
                    ServiceLevel = supplierDto.ServiceLevel,
                    AverageDeliveryDays = supplierDto.AverageDeliveryDays,
                    MinimumOrderQuantity = supplierDto.MinimumOrderQuantity,
                    MaximumOrderQuantity = supplierDto.MaximumOrderQuantity,
                    MinimumOrderValue = supplierDto.MinimumOrderValue,
                    MaximumOrderValue = supplierDto.MaximumOrderValue,
                    BusinessLicense = supplierDto.BusinessLicense,
                    TaxCertificate = supplierDto.TaxCertificate,
                    TradeRegistryCertificate = supplierDto.TradeRegistryCertificate,
                    ChamberOfCommerceCertificate = supplierDto.ChamberOfCommerceCertificate,
                    InsuranceCertificate = supplierDto.InsuranceCertificate,
                    FinancialStatement = supplierDto.FinancialStatement,
                    BusinessLicenseExpiryDate = supplierDto.BusinessLicenseExpiryDate,
                    TaxCertificateExpiryDate = supplierDto.TaxCertificateExpiryDate,
                    TradeRegistryExpiryDate = supplierDto.TradeRegistryExpiryDate,
                    ChamberOfCommerceExpiryDate = supplierDto.ChamberOfCommerceExpiryDate,
                    InsuranceExpiryDate = supplierDto.InsuranceExpiryDate,
                    FinancialStatementDate = supplierDto.FinancialStatementDate,
                    CreatedDate = DateTime.Now,
                    CreatedBy = GetCurrentUserId()
                };

                await _supplierRepository.AddAsync(supplier);

                return Json(new { 
                    success = true, 
                    message = "Tedarikçi başarıyla oluşturuldu.",
                    data = supplier 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Hata: {ex.Message}" 
                });
            }
        }

        // Tedarikçi güncelle - Raw Body ile
        [HttpPost]
        [Route("Suppliers/UpdateSupplierJson")]
        public async Task<IActionResult> UpdateSupplierJson()
        {
            try
            {
                // Raw body'yi oku
                string jsonData;
                using (var reader = new StreamReader(Request.Body))
                {
                    jsonData = await reader.ReadToEndAsync();
                }

                // Detaylı debug log
                Console.WriteLine("=== UpdateSupplierJson Debug ===");
                Console.WriteLine($"Request Content-Type: {Request.ContentType}");
                Console.WriteLine($"Request Content-Length: {Request.ContentLength}");
                Console.WriteLine($"jsonData null mu: {jsonData == null}");
                Console.WriteLine($"jsonData empty mu: {string.IsNullOrEmpty(jsonData)}");
                Console.WriteLine($"jsonData length: {jsonData?.Length ?? 0}");
                Console.WriteLine($"Gelen JSON: '{jsonData}'");
                Console.WriteLine("================================");

                // JSON string kontrolü
                if (string.IsNullOrEmpty(jsonData))
                {
                    Console.WriteLine("JSON data null veya boş!");
                    return Json(new { 
                        success = false, 
                        message = "Tedarikçi bilgileri alınamadı." 
                    });
                }

                // JSON'u SupplierDto'ya dönüştür
                SupplierDto? supplierDto;
                try
                {
                    // DateTime alanları için özel işlem
                    var jsonObject = System.Text.Json.JsonDocument.Parse(jsonData);
                    var root = jsonObject.RootElement;
                    
                    var jsonDict = new Dictionary<string, object>();
                    foreach (var property in root.EnumerateObject())
                    {
                        if (property.Name.Equals("CertificationExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("BusinessLicenseExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("TaxCertificateExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("TradeRegistryExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("ChamberOfCommerceExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("InsuranceExpiryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("FinancialStatementDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("EstablishmentDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("LastOrderDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("LastContactDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("LastDeliveryDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("LastPaymentDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("BlacklistDate", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("GdprConsentDate", StringComparison.OrdinalIgnoreCase))
                        {
                            // DateTime alanları için boş string kontrolü
                            var stringValue = property.Value.GetString();
                            jsonDict[property.Name] = string.IsNullOrEmpty(stringValue) ? null : stringValue;
                        }
                        else if (property.Name.Equals("QualityRating", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("DeliveryRating", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("ServiceRating", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("PriceRating", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("OverallRating", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("CreditLimit", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("DiscountRate", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("ExchangeRate", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("TotalPurchases", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("AnnualRevenue", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("NetWorth", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("TotalAssets", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("TotalLiabilities", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("OnTimeDeliveryRate", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("QualityAcceptanceRate", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("PriceCompetitiveness", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("ServiceLevel", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("MinimumOrderValue", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("MaximumOrderValue", StringComparison.OrdinalIgnoreCase))
                        {
                            // Decimal alanları için boş string kontrolü
                            var stringValue = property.Value.GetString();
                            if (string.IsNullOrEmpty(stringValue))
                            {
                                jsonDict[property.Name] = null;
                            }
                            else if (decimal.TryParse(stringValue, out var decimalValue))
                            {
                                jsonDict[property.Name] = decimalValue;
                            }
                            else
                            {
                                jsonDict[property.Name] = null;
                            }
                        }
                        else if (property.Name.Equals("PaymentTermDays", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("TotalOrders", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("EmployeeCount", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("AverageDeliveryDays", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("MinimumOrderQuantity", StringComparison.OrdinalIgnoreCase) ||
                                 property.Name.Equals("MaximumOrderQuantity", StringComparison.OrdinalIgnoreCase))
                        {
                            // Integer alanları için boş string kontrolü
                            var stringValue = property.Value.GetString();
                            if (string.IsNullOrEmpty(stringValue))
                            {
                                jsonDict[property.Name] = null;
                            }
                            else if (int.TryParse(stringValue, out var intValue))
                            {
                                jsonDict[property.Name] = intValue;
                            }
                            else
                            {
                                jsonDict[property.Name] = null;
                            }
                        }
                        else
                        {
                            // Diğer alanları olduğu gibi ekle
                            jsonDict[property.Name] = property.Value.ValueKind switch
                            {
                                JsonValueKind.String => property.Value.GetString(),
                                JsonValueKind.Number => property.Value.GetDecimal(),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                JsonValueKind.Null => null,
                                _ => property.Value.ToString()
                            };
                        }
                    }
                    
                    var cleanJson = System.Text.Json.JsonSerializer.Serialize(jsonDict);
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };
                    
                    supplierDto = System.Text.Json.JsonSerializer.Deserialize<SupplierDto>(cleanJson, options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialize hatası: {ex.Message}");
                    return Json(new { 
                        success = false, 
                        message = "Geçersiz veri formatı." 
                    });
                }

                // Null kontrolü
                if (supplierDto == null)
                {
                    Console.WriteLine("supplierDto null!");
                    return Json(new { 
                        success = false, 
                        message = "Tedarikçi bilgileri alınamadı." 
                    });
                }

                Console.WriteLine($"Deserialize edilen - ID: {supplierDto.Id}, CompanyName: {supplierDto.CompanyName}");

                if (supplierDto.Id <= 0)
                {
                    Console.WriteLine($"Geçersiz ID: {supplierDto.Id}");
                    return Json(new { 
                        success = false, 
                        message = "Geçersiz tedarikçi ID'si." 
                    });
                }

                var existingSupplier = await _supplierRepository.GetByIdAsync(supplierDto.Id.Value);
                if (existingSupplier == null)
                {
                    return Json(new { 
                        success = false, 
                        message = "Tedarikçi bulunamadı." 
                    });
                }

                // Güncelleme - Tüm alanları güncelle
                existingSupplier.CompanyName = supplierDto.CompanyName;
                existingSupplier.FirstName = supplierDto.FirstName;
                existingSupplier.LastName = supplierDto.LastName;
                existingSupplier.ContactPerson = supplierDto.ContactPerson;
                existingSupplier.ContactPersonTitle = supplierDto.ContactPersonTitle;
                existingSupplier.ContactPersonDepartment = supplierDto.ContactPersonDepartment;
                existingSupplier.SapVendorCode = supplierDto.SapVendorCode;
                existingSupplier.SapAccountGroup = supplierDto.SapAccountGroup;
                existingSupplier.SapPurchasingOrg = supplierDto.SapPurchasingOrg;
                existingSupplier.SapCompanyCode = supplierDto.SapCompanyCode;
                existingSupplier.SapPaymentTerms = supplierDto.SapPaymentTerms;
                existingSupplier.SapCurrency = supplierDto.SapCurrency;
                existingSupplier.SapTaxCode = supplierDto.SapTaxCode;
                existingSupplier.SapTaxCountry = supplierDto.SapTaxCountry;
                existingSupplier.Phone = supplierDto.Phone;
                existingSupplier.MobilePhone = supplierDto.MobilePhone;
                existingSupplier.Fax = supplierDto.Fax;
                existingSupplier.Email = supplierDto.Email;
                existingSupplier.Website = supplierDto.Website;
                existingSupplier.LinkedIn = supplierDto.LinkedIn;
                existingSupplier.Twitter = supplierDto.Twitter;
                existingSupplier.Facebook = supplierDto.Facebook;
                existingSupplier.Instagram = supplierDto.Instagram;
                existingSupplier.Address = supplierDto.Address;
                existingSupplier.City = supplierDto.City;
                existingSupplier.State = supplierDto.State;
                existingSupplier.PostalCode = supplierDto.PostalCode;
                existingSupplier.Country = supplierDto.Country;
                existingSupplier.Region = supplierDto.Region;
                existingSupplier.District = supplierDto.District;
                existingSupplier.DeliveryAddress = supplierDto.DeliveryAddress;
                existingSupplier.DeliveryCity = supplierDto.DeliveryCity;
                existingSupplier.DeliveryState = supplierDto.DeliveryState;
                existingSupplier.DeliveryPostalCode = supplierDto.DeliveryPostalCode;
                existingSupplier.DeliveryCountry = supplierDto.DeliveryCountry;
                existingSupplier.BillingAddress = supplierDto.BillingAddress;
                existingSupplier.BillingCity = supplierDto.BillingCity;
                existingSupplier.BillingState = supplierDto.BillingState;
                existingSupplier.BillingPostalCode = supplierDto.BillingPostalCode;
                existingSupplier.BillingCountry = supplierDto.BillingCountry;
                existingSupplier.TcNumber = supplierDto.TcNumber;
                existingSupplier.TaxNumber = supplierDto.TaxNumber;
                existingSupplier.TaxOffice = supplierDto.TaxOffice;
                existingSupplier.TaxOfficeCode = supplierDto.TaxOfficeCode;
                existingSupplier.EInvoiceAlias = supplierDto.EInvoiceAlias;
                existingSupplier.EInvoiceTitle = supplierDto.EInvoiceTitle;
                existingSupplier.EInvoiceProfile = supplierDto.EInvoiceProfile;
                existingSupplier.EArchiveProfile = supplierDto.EArchiveProfile;
                existingSupplier.EDeliveryNoteProfile = supplierDto.EDeliveryNoteProfile;
                existingSupplier.EInvoiceTestAlias = supplierDto.EInvoiceTestAlias;
                existingSupplier.EInvoiceProdAlias = supplierDto.EInvoiceProdAlias;
                existingSupplier.VatNumber = supplierDto.VatNumber;
                existingSupplier.VatCountryCode = supplierDto.VatCountryCode;
                existingSupplier.LegalEntityType = supplierDto.LegalEntityType;
                existingSupplier.SupplierType = supplierDto.SupplierType;
                existingSupplier.SupplierCategory = supplierDto.SupplierCategory;
                existingSupplier.SupplierGroup = supplierDto.SupplierGroup;
                existingSupplier.SupplierSegment = supplierDto.SupplierSegment;
                existingSupplier.BankName = supplierDto.BankName;
                existingSupplier.BankBranch = supplierDto.BankBranch;
                existingSupplier.BankAccountNumber = supplierDto.BankAccountNumber;
                existingSupplier.Iban = supplierDto.Iban;
                existingSupplier.SwiftCode = supplierDto.SwiftCode;
                existingSupplier.BankAddress = supplierDto.BankAddress;
                existingSupplier.BankCity = supplierDto.BankCity;
                existingSupplier.BankCountry = supplierDto.BankCountry;
                existingSupplier.TradeRegistryNumber = supplierDto.TradeRegistryNumber;
                existingSupplier.ChamberOfCommerce = supplierDto.ChamberOfCommerce;
                existingSupplier.MersisNumber = supplierDto.MersisNumber;
                existingSupplier.ActivityCode = supplierDto.ActivityCode;
                existingSupplier.ActivityDescription = supplierDto.ActivityDescription;
                existingSupplier.IndustryCode = supplierDto.IndustryCode;
                existingSupplier.IndustryDescription = supplierDto.IndustryDescription;
                existingSupplier.PaymentMethod = supplierDto.PaymentMethod;
                existingSupplier.PaymentTermDays = supplierDto.PaymentTermDays;
                existingSupplier.CreditLimit = supplierDto.CreditLimit;
                existingSupplier.DiscountRate = supplierDto.DiscountRate;
                existingSupplier.Currency = supplierDto.Currency;
                existingSupplier.PaymentCurrency = supplierDto.PaymentCurrency;
                existingSupplier.ExchangeRate = supplierDto.ExchangeRate;
                existingSupplier.IsEInvoiceEnabled = supplierDto.IsEInvoiceEnabled;
                existingSupplier.IsEArchiveEnabled = supplierDto.IsEArchiveEnabled;
                existingSupplier.IsEDeliveryNoteEnabled = supplierDto.IsEDeliveryNoteEnabled;
                existingSupplier.IsEInvoiceTestMode = supplierDto.IsEInvoiceTestMode;
                existingSupplier.IsEArchiveTestMode = supplierDto.IsEArchiveTestMode;
                existingSupplier.IsEDeliveryNoteTestMode = supplierDto.IsEDeliveryNoteTestMode;
                existingSupplier.EInvoiceIntegrationType = supplierDto.EInvoiceIntegrationType;
                existingSupplier.EInvoiceServiceProvider = supplierDto.EInvoiceServiceProvider;
                existingSupplier.EInvoiceApiUrl = supplierDto.EInvoiceApiUrl;
                existingSupplier.EInvoiceUsername = supplierDto.EInvoiceUsername;
                existingSupplier.EInvoicePassword = supplierDto.EInvoicePassword;
                existingSupplier.EInvoiceCertificatePath = supplierDto.EInvoiceCertificatePath;
                existingSupplier.EInvoiceCertificatePassword = supplierDto.EInvoiceCertificatePassword;
                existingSupplier.QualityCertification = supplierDto.QualityCertification;
                existingSupplier.IsoCertification = supplierDto.IsoCertification;
                existingSupplier.EnvironmentalCertification = supplierDto.EnvironmentalCertification;
                existingSupplier.SafetyCertification = supplierDto.SafetyCertification;
                existingSupplier.CertificationExpiryDate = supplierDto.CertificationExpiryDate;
                existingSupplier.QualityRating = supplierDto.QualityRating;
                existingSupplier.DeliveryRating = supplierDto.DeliveryRating;
                existingSupplier.ServiceRating = supplierDto.ServiceRating;
                existingSupplier.PriceRating = supplierDto.PriceRating;
                existingSupplier.OverallRating = supplierDto.OverallRating;
                existingSupplier.RiskLevel = supplierDto.RiskLevel;
                existingSupplier.CreditRating = supplierDto.CreditRating;
                existingSupplier.IsBlacklisted = supplierDto.IsBlacklisted;
                existingSupplier.BlacklistReason = supplierDto.BlacklistReason;
                existingSupplier.BlacklistDate = supplierDto.BlacklistDate;
                existingSupplier.IsActive = supplierDto.IsActive;
                existingSupplier.Notes = supplierDto.Notes;
                existingSupplier.InternalNotes = supplierDto.InternalNotes;
                existingSupplier.QualityNotes = supplierDto.QualityNotes;
                existingSupplier.PaymentNotes = supplierDto.PaymentNotes;
                existingSupplier.Source = supplierDto.Source;
                existingSupplier.Priority = supplierDto.Priority;
                existingSupplier.Status = supplierDto.Status;
                existingSupplier.TotalOrders = supplierDto.TotalOrders;
                existingSupplier.TotalPurchases = supplierDto.TotalPurchases;
                existingSupplier.LastOrderDate = supplierDto.LastOrderDate;
                existingSupplier.LastContactDate = supplierDto.LastContactDate;
                existingSupplier.LastDeliveryDate = supplierDto.LastDeliveryDate;
                existingSupplier.LastPaymentDate = supplierDto.LastPaymentDate;
                existingSupplier.GdprConsent = supplierDto.GdprConsent;
                existingSupplier.GdprConsentDate = supplierDto.GdprConsentDate;
                existingSupplier.MarketingConsent = supplierDto.MarketingConsent;
                existingSupplier.SmsConsent = supplierDto.SmsConsent;
                existingSupplier.EmailConsent = supplierDto.EmailConsent;
                existingSupplier.Language = supplierDto.Language;
                existingSupplier.TimeZone = supplierDto.TimeZone;
                existingSupplier.DateFormat = supplierDto.DateFormat;
                existingSupplier.NumberFormat = supplierDto.NumberFormat;
                existingSupplier.SapVendorAccount = supplierDto.SapVendorAccount;
                existingSupplier.SapReconciliationAccount = supplierDto.SapReconciliationAccount;
                existingSupplier.SapSortKey = supplierDto.SapSortKey;
                existingSupplier.SapPaymentBlock = supplierDto.SapPaymentBlock;
                existingSupplier.SapPostingBlock = supplierDto.SapPostingBlock;
                existingSupplier.SapPurchasingBlock = supplierDto.SapPurchasingBlock;
                existingSupplier.SapOrderCurrency = supplierDto.SapOrderCurrency;
                existingSupplier.SapIncoterms = supplierDto.SapIncoterms;
                existingSupplier.SapTransportationZone = supplierDto.SapTransportationZone;
                existingSupplier.SapShippingCondition = supplierDto.SapShippingCondition;
                existingSupplier.SapDeliveryPriority = supplierDto.SapDeliveryPriority;
                existingSupplier.SapMinimumOrderValue = supplierDto.SapMinimumOrderValue;
                existingSupplier.SapMaximumOrderValue = supplierDto.SapMaximumOrderValue;
                existingSupplier.EDeliveryNoteAlias = supplierDto.EDeliveryNoteAlias;
                existingSupplier.EDeliveryNoteTitle = supplierDto.EDeliveryNoteTitle;
                existingSupplier.EDeliveryNoteIntegrationType = supplierDto.EDeliveryNoteIntegrationType;
                existingSupplier.EDeliveryNoteServiceProvider = supplierDto.EDeliveryNoteServiceProvider;
                existingSupplier.EDeliveryNoteApiUrl = supplierDto.EDeliveryNoteApiUrl;
                existingSupplier.EDeliveryNoteUsername = supplierDto.EDeliveryNoteUsername;
                existingSupplier.EDeliveryNotePassword = supplierDto.EDeliveryNotePassword;
                existingSupplier.SupplierCode = supplierDto.SupplierCode;
                existingSupplier.SupplierName = supplierDto.SupplierName;
                existingSupplier.SupplierShortName = supplierDto.SupplierShortName;
                existingSupplier.SupplierDisplayName = supplierDto.SupplierDisplayName;
                existingSupplier.SupplierLegalName = supplierDto.SupplierLegalName;
                existingSupplier.SupplierCommercialName = supplierDto.SupplierCommercialName;
                existingSupplier.SupplierBrandName = supplierDto.SupplierBrandName;
                existingSupplier.PrimaryContactEmail = supplierDto.PrimaryContactEmail;
                existingSupplier.SecondaryContactEmail = supplierDto.SecondaryContactEmail;
                existingSupplier.PrimaryContactPhone = supplierDto.PrimaryContactPhone;
                existingSupplier.SecondaryContactPhone = supplierDto.SecondaryContactPhone;
                existingSupplier.EmergencyContactName = supplierDto.EmergencyContactName;
                existingSupplier.EmergencyContactPhone = supplierDto.EmergencyContactPhone;
                existingSupplier.EmergencyContactEmail = supplierDto.EmergencyContactEmail;
                existingSupplier.AnnualRevenue = supplierDto.AnnualRevenue;
                existingSupplier.NetWorth = supplierDto.NetWorth;
                existingSupplier.TotalAssets = supplierDto.TotalAssets;
                existingSupplier.TotalLiabilities = supplierDto.TotalLiabilities;
                existingSupplier.EmployeeCount = supplierDto.EmployeeCount;
                existingSupplier.EstablishmentDate = supplierDto.EstablishmentDate;
                existingSupplier.OwnershipType = supplierDto.OwnershipType;
                existingSupplier.BusinessSize = supplierDto.BusinessSize;
                existingSupplier.OnTimeDeliveryRate = supplierDto.OnTimeDeliveryRate;
                existingSupplier.QualityAcceptanceRate = supplierDto.QualityAcceptanceRate;
                existingSupplier.PriceCompetitiveness = supplierDto.PriceCompetitiveness;
                existingSupplier.ServiceLevel = supplierDto.ServiceLevel;
                existingSupplier.AverageDeliveryDays = supplierDto.AverageDeliveryDays;
                existingSupplier.MinimumOrderQuantity = supplierDto.MinimumOrderQuantity;
                existingSupplier.MaximumOrderQuantity = supplierDto.MaximumOrderQuantity;
                existingSupplier.MinimumOrderValue = supplierDto.MinimumOrderValue;
                existingSupplier.MaximumOrderValue = supplierDto.MaximumOrderValue;
                existingSupplier.BusinessLicense = supplierDto.BusinessLicense;
                existingSupplier.TaxCertificate = supplierDto.TaxCertificate;
                existingSupplier.TradeRegistryCertificate = supplierDto.TradeRegistryCertificate;
                existingSupplier.ChamberOfCommerceCertificate = supplierDto.ChamberOfCommerceCertificate;
                existingSupplier.InsuranceCertificate = supplierDto.InsuranceCertificate;
                existingSupplier.FinancialStatement = supplierDto.FinancialStatement;
                existingSupplier.BusinessLicenseExpiryDate = supplierDto.BusinessLicenseExpiryDate;
                existingSupplier.TaxCertificateExpiryDate = supplierDto.TaxCertificateExpiryDate;
                existingSupplier.TradeRegistryExpiryDate = supplierDto.TradeRegistryExpiryDate;
                existingSupplier.ChamberOfCommerceExpiryDate = supplierDto.ChamberOfCommerceExpiryDate;
                existingSupplier.InsuranceExpiryDate = supplierDto.InsuranceExpiryDate;
                existingSupplier.FinancialStatementDate = supplierDto.FinancialStatementDate;
                existingSupplier.UpdatedDate = DateTime.Now;
                existingSupplier.UpdatedBy = GetCurrentUserId();

                await _supplierRepository.UpdateAsync(existingSupplier);

                return Json(new { 
                    success = true, 
                    message = "Tedarikçi başarıyla güncellendi.",
                    data = existingSupplier 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Hata: {ex.Message}" 
                });
            }
        }

        // Tedarikçi sil
        [HttpPost]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier == null)
                {
                    return Json(new { 
                        success = false, 
                        message = "Tedarikçi bulunamadı." 
                    });
                }

                await _supplierRepository.DeleteAsync(supplier);

                return Json(new { 
                    success = true, 
                    message = "Tedarikçi başarıyla silindi." 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Hata: {ex.Message}" 
                });
            }
        }

        // İstatistikler
        [HttpGet]
        public async Task<IActionResult> GetSupplierStatistics()
        {
            try
            {
                var totalSuppliers = await _supplierRepository.GetTotalSupplierCountAsync();
                var activeSuppliers = await _supplierRepository.GetActiveSupplierCountAsync();
                var totalPurchases = await _supplierRepository.GetTotalSupplierPurchasesAsync();
                var averageRating = await _supplierRepository.GetAverageSupplierRatingAsync();
                var eInvoiceEnabled = await _supplierRepository.GetSuppliersWithEInvoiceEnabledCountAsync();
                var eArchiveEnabled = await _supplierRepository.GetSuppliersWithEArchiveEnabledCountAsync();
                var eDeliveryNoteEnabled = await _supplierRepository.GetSuppliersWithEDeliveryNoteEnabledCountAsync();

                return Json(new { 
                    success = true, 
                    data = new {
                        totalSuppliers,
                        activeSuppliers,
                        totalPurchases,
                        averageRating,
                        eInvoiceEnabled,
                        eArchiveEnabled,
                        eDeliveryNoteEnabled
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"İstatistik hatası: {ex.Message}");
                return Json(new { 
                    success = false, 
                    error = ex.Message
                });
            }
        }

        // Yardımcı metodlar
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        }
    }
}
