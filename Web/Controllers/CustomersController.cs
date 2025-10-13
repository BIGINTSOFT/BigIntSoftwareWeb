using Microsoft.AspNetCore.Mvc;
using Bussiness.Repository.Abstract;
using Entities.Dto;
using Entities.Entity;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // Ana sayfa
        public IActionResult Index()
        {
            return View();
        }

        // Müşteri listesi - DevExtreme DataGrid için
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                // Tüm müşteri verilerini getir
                var customers = await _customerRepository.GetAllAsync();
                
                // Debug bilgisi
                Console.WriteLine($"Toplam müşteri sayısı: {customers?.Count() ?? 0}");
                
                return Json(new { 
                    success = true, 
                    data = customers,
                    count = customers?.Count() ?? 0
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


        // Tek müşteri getir - Edit için
        [HttpGet]
        public async Task<IActionResult> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return Json(new { 
                        success = false, 
                        error = "Müşteri bulunamadı." 
                    });
                }

                return Json(new { 
                    success = true, 
                    data = customer 
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

        // Yeni müşteri oluştur - Raw Body ile
        [HttpPost]
        [Route("Customers/CreateCustomerJson")]
        public async Task<IActionResult> CreateCustomerJson()
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
                Console.WriteLine("=== CreateCustomerJson Debug ===");
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
                        message = "Müşteri bilgileri alınamadı." 
                    });
                }

                // JSON'u CustomerDto'ya dönüştür
                CustomerDto? customerDto;
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
                        
                        customerDto = System.Text.Json.JsonSerializer.Deserialize<CustomerDto>(cleanJson, options);
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
                        
                        customerDto = System.Text.Json.JsonSerializer.Deserialize<CustomerDto>(jsonData, options);
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
                if (customerDto == null)
                {
                    Console.WriteLine("customerDto null!");
                    return Json(new { 
                        success = false, 
                        message = "Müşteri bilgileri alınamadı." 
                    });
                }

                Console.WriteLine($"Deserialize edilen - CompanyName: {customerDto.CompanyName}");

                var customer = new Customers
                {
                    CompanyName = customerDto.CompanyName,
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    ContactPerson = customerDto.ContactPerson,
                    CustomerType = customerDto.CustomerType,
                    CustomerGroup = customerDto.CustomerGroup,
                    CustomerSegment = customerDto.CustomerSegment,
                    IsActive = customerDto.IsActive,
                    Phone = customerDto.Phone,
                    MobilePhone = customerDto.MobilePhone,
                    Email = customerDto.Email,
                    Website = customerDto.Website,
                    LinkedIn = customerDto.LinkedIn,
                    Twitter = customerDto.Twitter,
                    Address = customerDto.Address,
                    City = customerDto.City,
                    State = customerDto.State,
                    PostalCode = customerDto.PostalCode,
                    Country = customerDto.Country,
                    DeliveryAddress = customerDto.DeliveryAddress,
                    DeliveryCity = customerDto.DeliveryCity,
                    DeliveryPostalCode = customerDto.DeliveryPostalCode,
                    TcNumber = customerDto.TcNumber,
                    TaxNumber = customerDto.TaxNumber,
                    TaxOffice = customerDto.TaxOffice,
                    TaxOfficeCode = customerDto.TaxOfficeCode,
                    EInvoiceAlias = customerDto.EInvoiceAlias,
                    EInvoiceTitle = customerDto.EInvoiceTitle,
                    VatNumber = customerDto.VatNumber,
                    VatCountryCode = customerDto.VatCountryCode,
                    LegalEntityType = customerDto.LegalEntityType,
                    BankName = customerDto.BankName,
                    BankBranch = customerDto.BankBranch,
                    BankAccountNumber = customerDto.BankAccountNumber,
                    Iban = customerDto.Iban,
                    SwiftCode = customerDto.SwiftCode,
                    TradeRegistryNumber = customerDto.TradeRegistryNumber,
                    ChamberOfCommerce = customerDto.ChamberOfCommerce,
                    MersisNumber = customerDto.MersisNumber,
                    ActivityCode = customerDto.ActivityCode,
                    ActivityDescription = customerDto.ActivityDescription,
                    PaymentMethod = customerDto.PaymentMethod,
                    PaymentTermDays = customerDto.PaymentTermDays,
                    CreditLimit = customerDto.CreditLimit,
                    DiscountRate = customerDto.DiscountRate,
                    Currency = customerDto.Currency,
                    IsEInvoiceEnabled = customerDto.IsEInvoiceEnabled,
                    IsEArchiveEnabled = customerDto.IsEArchiveEnabled,
                    EInvoiceProfile = customerDto.EInvoiceProfile,
                    EArchiveProfile = customerDto.EArchiveProfile,
                    Notes = customerDto.Notes,
                    InternalNotes = customerDto.InternalNotes,
                    Source = customerDto.Source,
                    Language = customerDto.Language,
                    CreatedDate = DateTime.Now,
                    CreatedBy = GetCurrentUserId()
                };

                await _customerRepository.AddAsync(customer);

                return Json(new { 
                    success = true, 
                    message = "Müşteri başarıyla oluşturuldu.",
                    data = customer 
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

        // Müşteri güncelle - Raw Body ile
        [HttpPost]
        [Route("Customers/UpdateCustomerJson")]
        public async Task<IActionResult> UpdateCustomerJson()
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
                Console.WriteLine("=== UpdateCustomerJson Debug ===");
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
                        message = "Müşteri bilgileri alınamadı." 
                    });
                }

                // JSON'u CustomerDto'ya dönüştür
                CustomerDto? customerDto;
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };
                    
                    customerDto = System.Text.Json.JsonSerializer.Deserialize<CustomerDto>(jsonData, options);
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
                if (customerDto == null)
                {
                    Console.WriteLine("customerDto null!");
                    return Json(new { 
                        success = false, 
                        message = "Müşteri bilgileri alınamadı." 
                    });
                }

                Console.WriteLine($"Deserialize edilen - ID: {customerDto.Id}, CompanyName: {customerDto.CompanyName}");

                if (customerDto.Id <= 0)
                {
                    Console.WriteLine($"Geçersiz ID: {customerDto.Id}");
                    return Json(new { 
                        success = false, 
                        message = "Geçersiz müşteri ID'si." 
                    });
                }

                var existingCustomer = await _customerRepository.GetByIdAsync(customerDto.Id.Value);
                if (existingCustomer == null)
                {
                    return Json(new { 
                        success = false, 
                        message = "Müşteri bulunamadı." 
                    });
                }

                // Güncelleme
                existingCustomer.CompanyName = customerDto.CompanyName;
                existingCustomer.FirstName = customerDto.FirstName;
                existingCustomer.LastName = customerDto.LastName;
                existingCustomer.ContactPerson = customerDto.ContactPerson;
                existingCustomer.CustomerType = customerDto.CustomerType;
                existingCustomer.CustomerGroup = customerDto.CustomerGroup;
                existingCustomer.CustomerSegment = customerDto.CustomerSegment;
                existingCustomer.IsActive = customerDto.IsActive;
                existingCustomer.Phone = customerDto.Phone;
                existingCustomer.MobilePhone = customerDto.MobilePhone;
                existingCustomer.Email = customerDto.Email;
                existingCustomer.Website = customerDto.Website;
                existingCustomer.LinkedIn = customerDto.LinkedIn;
                existingCustomer.Twitter = customerDto.Twitter;
                existingCustomer.Address = customerDto.Address;
                existingCustomer.City = customerDto.City;
                existingCustomer.State = customerDto.State;
                existingCustomer.PostalCode = customerDto.PostalCode;
                existingCustomer.Country = customerDto.Country;
                existingCustomer.DeliveryAddress = customerDto.DeliveryAddress;
                existingCustomer.DeliveryCity = customerDto.DeliveryCity;
                existingCustomer.DeliveryPostalCode = customerDto.DeliveryPostalCode;
                existingCustomer.TcNumber = customerDto.TcNumber;
                existingCustomer.TaxNumber = customerDto.TaxNumber;
                existingCustomer.TaxOffice = customerDto.TaxOffice;
                existingCustomer.TaxOfficeCode = customerDto.TaxOfficeCode;
                existingCustomer.EInvoiceAlias = customerDto.EInvoiceAlias;
                existingCustomer.EInvoiceTitle = customerDto.EInvoiceTitle;
                existingCustomer.VatNumber = customerDto.VatNumber;
                existingCustomer.VatCountryCode = customerDto.VatCountryCode;
                existingCustomer.LegalEntityType = customerDto.LegalEntityType;
                existingCustomer.BankName = customerDto.BankName;
                existingCustomer.BankBranch = customerDto.BankBranch;
                existingCustomer.BankAccountNumber = customerDto.BankAccountNumber;
                existingCustomer.Iban = customerDto.Iban;
                existingCustomer.SwiftCode = customerDto.SwiftCode;
                existingCustomer.TradeRegistryNumber = customerDto.TradeRegistryNumber;
                existingCustomer.ChamberOfCommerce = customerDto.ChamberOfCommerce;
                existingCustomer.MersisNumber = customerDto.MersisNumber;
                existingCustomer.ActivityCode = customerDto.ActivityCode;
                existingCustomer.ActivityDescription = customerDto.ActivityDescription;
                existingCustomer.PaymentMethod = customerDto.PaymentMethod;
                existingCustomer.PaymentTermDays = customerDto.PaymentTermDays;
                existingCustomer.CreditLimit = customerDto.CreditLimit;
                existingCustomer.DiscountRate = customerDto.DiscountRate;
                existingCustomer.Currency = customerDto.Currency;
                existingCustomer.IsEInvoiceEnabled = customerDto.IsEInvoiceEnabled;
                existingCustomer.IsEArchiveEnabled = customerDto.IsEArchiveEnabled;
                existingCustomer.EInvoiceProfile = customerDto.EInvoiceProfile;
                existingCustomer.EArchiveProfile = customerDto.EArchiveProfile;
                existingCustomer.Notes = customerDto.Notes;
                existingCustomer.InternalNotes = customerDto.InternalNotes;
                existingCustomer.Source = customerDto.Source;
                existingCustomer.Language = customerDto.Language;
                existingCustomer.UpdatedDate = DateTime.Now;
                existingCustomer.UpdatedBy = GetCurrentUserId();

                await _customerRepository.UpdateAsync(existingCustomer);

                return Json(new { 
                    success = true, 
                    message = "Müşteri başarıyla güncellendi.",
                    data = existingCustomer 
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

        // Müşteri sil
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return Json(new { 
                        success = false, 
                        message = "Müşteri bulunamadı." 
                    });
                }

                await _customerRepository.DeleteAsync(customer);

                return Json(new { 
                    success = true, 
                    message = "Müşteri başarıyla silindi." 
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
        public async Task<IActionResult> GetUpdateStatistics()
        {
            try
            {
                var totalCustomers = await _customerRepository.CountAsync();
                var activeCustomers = await _customerRepository.GetActiveCustomerCountAsync();
                var totalSales = await _customerRepository.GetTotalCustomerSalesAsync();


                return Json(new { 
                    success = true, 
                    data = new {
                        totalCustomers,
                        activeCustomers,
                        totalSales
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