using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class SupplierRepository : GenericRepository<Suppliers>, ISupplierRepository
    {
        public SupplierRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        // Temel filtreleme metodları
        public async Task<IEnumerable<Suppliers>> GetActiveSuppliersAsync()
        {
            return await _dbSet.Where(s => s.IsActive == true).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetInactiveSuppliersAsync()
        {
            return await _dbSet.Where(s => s.IsActive != true).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetBlacklistedSuppliersAsync()
        {
            return await _dbSet.Where(s => s.IsBlacklisted == true).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByTypeAsync(string supplierType)
        {
            return await _dbSet.Where(s => s.SupplierType == supplierType).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByCategoryAsync(string supplierCategory)
        {
            return await _dbSet.Where(s => s.SupplierCategory == supplierCategory).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByGroupAsync(string supplierGroup)
        {
            return await _dbSet.Where(s => s.SupplierGroup == supplierGroup).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersBySegmentAsync(string supplierSegment)
        {
            return await _dbSet.Where(s => s.SupplierSegment == supplierSegment).ToListAsync();
        }

        // SAP entegrasyon metodları
        public async Task<Suppliers?> GetSupplierBySapVendorCodeAsync(string sapVendorCode)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.SapVendorCode == sapVendorCode);
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersBySapAccountGroupAsync(string sapAccountGroup)
        {
            return await _dbSet.Where(s => s.SapAccountGroup == sapAccountGroup).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersBySapPurchasingOrgAsync(string sapPurchasingOrg)
        {
            return await _dbSet.Where(s => s.SapPurchasingOrg == sapPurchasingOrg).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersBySapCompanyCodeAsync(string sapCompanyCode)
        {
            return await _dbSet.Where(s => s.SapCompanyCode == sapCompanyCode).ToListAsync();
        }

        // E-Fatura entegrasyon metodları
        public async Task<IEnumerable<Suppliers>> GetEInvoiceEnabledSuppliersAsync()
        {
            return await _dbSet.Where(s => s.IsEInvoiceEnabled == true).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetEArchiveEnabledSuppliersAsync()
        {
            return await _dbSet.Where(s => s.IsEArchiveEnabled == true).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetEDeliveryNoteEnabledSuppliersAsync()
        {
            return await _dbSet.Where(s => s.IsEDeliveryNoteEnabled == true).ToListAsync();
        }

        public async Task<Suppliers?> GetSupplierByEInvoiceAliasAsync(string eInvoiceAlias)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.EInvoiceAlias == eInvoiceAlias);
        }

        public async Task<Suppliers?> GetSupplierByTaxNumberAsync(string taxNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.TaxNumber == taxNumber);
        }

        public async Task<Suppliers?> GetSupplierByTcNumberAsync(string tcNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.TcNumber == tcNumber);
        }

        // Kalite ve performans metodları
        public async Task<IEnumerable<Suppliers>> GetSuppliersByQualityRatingAsync(decimal minRating)
        {
            return await _dbSet.Where(s => s.QualityRating >= minRating).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByDeliveryRatingAsync(decimal minRating)
        {
            return await _dbSet.Where(s => s.DeliveryRating >= minRating).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByOverallRatingAsync(decimal minRating)
        {
            return await _dbSet.Where(s => s.OverallRating >= minRating).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByRiskLevelAsync(string riskLevel)
        {
            return await _dbSet.Where(s => s.RiskLevel == riskLevel).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByCreditRatingAsync(string creditRating)
        {
            return await _dbSet.Where(s => s.CreditRating == creditRating).ToListAsync();
        }

        // Sertifikasyon metodları
        public async Task<IEnumerable<Suppliers>> GetSuppliersByQualityCertificationAsync(string certification)
        {
            return await _dbSet.Where(s => s.QualityCertification == certification).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByIsoCertificationAsync(string isoCertification)
        {
            return await _dbSet.Where(s => s.IsoCertification == isoCertification).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersWithExpiredCertificationsAsync()
        {
            var today = DateTime.Today;
            return await _dbSet.Where(s => s.CertificationExpiryDate.HasValue && s.CertificationExpiryDate.Value < today).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersWithExpiringCertificationsAsync(int daysAhead)
        {
            var futureDate = DateTime.Today.AddDays(daysAhead);
            return await _dbSet.Where(s => s.CertificationExpiryDate.HasValue && 
                                         s.CertificationExpiryDate.Value <= futureDate && 
                                         s.CertificationExpiryDate.Value >= DateTime.Today).ToListAsync();
        }

        // Finansal metodlar
        public async Task<IEnumerable<Suppliers>> GetSuppliersByCreditLimitAsync(decimal minCreditLimit)
        {
            return await _dbSet.Where(s => s.CreditLimit >= minCreditLimit).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByPaymentTermDaysAsync(int maxPaymentTermDays)
        {
            return await _dbSet.Where(s => s.PaymentTermDays <= maxPaymentTermDays).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByCurrencyAsync(string currency)
        {
            return await _dbSet.Where(s => s.Currency == currency).ToListAsync();
        }

        // İstatistikler
        public async Task<int> GetActiveSupplierCountAsync()
        {
            return await _dbSet.CountAsync(s => s.IsActive == true);
        }

        public async Task<int> GetTotalSupplierCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<decimal> GetTotalSupplierPurchasesAsync()
        {
            return await _dbSet.SumAsync(s => s.TotalPurchases ?? 0);
        }

        public async Task<decimal> GetAverageSupplierRatingAsync()
        {
            return await _dbSet.Where(s => s.OverallRating.HasValue).AverageAsync(s => s.OverallRating.Value);
        }

        public async Task<int> GetSuppliersWithEInvoiceEnabledCountAsync()
        {
            return await _dbSet.CountAsync(s => s.IsEInvoiceEnabled == true);
        }

        public async Task<int> GetSuppliersWithEArchiveEnabledCountAsync()
        {
            return await _dbSet.CountAsync(s => s.IsEArchiveEnabled == true);
        }

        public async Task<int> GetSuppliersWithEDeliveryNoteEnabledCountAsync()
        {
            return await _dbSet.CountAsync(s => s.IsEDeliveryNoteEnabled == true);
        }

        // Arama ve filtreleme
        public async Task<IEnumerable<Suppliers>> SearchSuppliersAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _dbSet.Where(s => 
                s.CompanyName.ToLower().Contains(term) ||
                (s.FirstName != null && s.FirstName.ToLower().Contains(term)) ||
                (s.LastName != null && s.LastName.ToLower().Contains(term)) ||
                (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(term)) ||
                (s.Email != null && s.Email.ToLower().Contains(term)) ||
                (s.Phone != null && s.Phone.Contains(term)) ||
                (s.TaxNumber != null && s.TaxNumber.Contains(term)) ||
                (s.SapVendorCode != null && s.SapVendorCode.ToLower().Contains(term))
            ).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByCityAsync(string city)
        {
            return await _dbSet.Where(s => s.City == city).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByCountryAsync(string country)
        {
            return await _dbSet.Where(s => s.Country == country).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByIndustryCodeAsync(string industryCode)
        {
            return await _dbSet.Where(s => s.IndustryCode == industryCode).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersByActivityCodeAsync(string activityCode)
        {
            return await _dbSet.Where(s => s.ActivityCode == activityCode).ToListAsync();
        }

        // Raporlama
        public async Task<IEnumerable<Suppliers>> GetTopRatedSuppliersAsync(int count)
        {
            return await _dbSet.Where(s => s.OverallRating.HasValue)
                              .OrderByDescending(s => s.OverallRating)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetTopPurchasingSuppliersAsync(int count)
        {
            return await _dbSet.Where(s => s.TotalPurchases.HasValue)
                              .OrderByDescending(s => s.TotalPurchases)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetRecentlyAddedSuppliersAsync(int count)
        {
            return await _dbSet.OrderByDescending(s => s.CreatedDate)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersWithRecentOrdersAsync(int days)
        {
            var dateThreshold = DateTime.Today.AddDays(-days);
            return await _dbSet.Where(s => s.LastOrderDate.HasValue && s.LastOrderDate.Value >= dateThreshold).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersWithRecentDeliveriesAsync(int days)
        {
            var dateThreshold = DateTime.Today.AddDays(-days);
            return await _dbSet.Where(s => s.LastDeliveryDate.HasValue && s.LastDeliveryDate.Value >= dateThreshold).ToListAsync();
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliersWithRecentPaymentsAsync(int days)
        {
            var dateThreshold = DateTime.Today.AddDays(-days);
            return await _dbSet.Where(s => s.LastPaymentDate.HasValue && s.LastPaymentDate.Value >= dateThreshold).ToListAsync();
        }

        // Performans metrikleri
        public async Task<decimal> GetAverageOnTimeDeliveryRateAsync()
        {
            return await _dbSet.Where(s => s.OnTimeDeliveryRate.HasValue).AverageAsync(s => s.OnTimeDeliveryRate.Value);
        }

        public async Task<decimal> GetAverageQualityAcceptanceRateAsync()
        {
            return await _dbSet.Where(s => s.QualityAcceptanceRate.HasValue).AverageAsync(s => s.QualityAcceptanceRate.Value);
        }

        public async Task<decimal> GetAveragePriceCompetitivenessAsync()
        {
            return await _dbSet.Where(s => s.PriceCompetitiveness.HasValue).AverageAsync(s => s.PriceCompetitiveness.Value);
        }

        public async Task<decimal> GetAverageServiceLevelAsync()
        {
            return await _dbSet.Where(s => s.ServiceLevel.HasValue).AverageAsync(s => s.ServiceLevel.Value);
        }

        public async Task<int> GetAverageDeliveryDaysAsync()
        {
            return (int)await _dbSet.Where(s => s.AverageDeliveryDays.HasValue).AverageAsync(s => s.AverageDeliveryDays.Value);
        }

        // Tedarikçi yönetimi
        public async Task<bool> IsSupplierBlacklistedAsync(int supplierId)
        {
            var supplier = await _dbSet.FirstOrDefaultAsync(s => s.Id == supplierId);
            return supplier?.IsBlacklisted ?? false;
        }

        public async Task<bool> IsSupplierActiveAsync(int supplierId)
        {
            var supplier = await _dbSet.FirstOrDefaultAsync(s => s.Id == supplierId);
            return supplier?.IsActive ?? false;
        }

        public async Task<bool> HasValidCertificationsAsync(int supplierId)
        {
            var supplier = await _dbSet.FirstOrDefaultAsync(s => s.Id == supplierId);
            if (supplier == null) return false;

            var today = DateTime.Today;
            return supplier.CertificationExpiryDate.HasValue && supplier.CertificationExpiryDate.Value >= today;
        }

        public async Task<bool> IsEInvoiceEnabledAsync(int supplierId)
        {
            var supplier = await _dbSet.FirstOrDefaultAsync(s => s.Id == supplierId);
            return supplier?.IsEInvoiceEnabled ?? false;
        }

        public async Task<bool> IsEArchiveEnabledAsync(int supplierId)
        {
            var supplier = await _dbSet.FirstOrDefaultAsync(s => s.Id == supplierId);
            return supplier?.IsEArchiveEnabled ?? false;
        }

        public async Task<bool> IsEDeliveryNoteEnabledAsync(int supplierId)
        {
            var supplier = await _dbSet.FirstOrDefaultAsync(s => s.Id == supplierId);
            return supplier?.IsEDeliveryNoteEnabled ?? false;
        }

        // Toplu işlemler
        public async Task<int> BulkUpdateSupplierStatusAsync(IEnumerable<int> supplierIds, bool isActive)
        {
            var suppliers = await _dbSet.Where(s => supplierIds.Contains(s.Id)).ToListAsync();
            foreach (var supplier in suppliers)
            {
                supplier.IsActive = isActive;
                supplier.UpdatedDate = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> BulkUpdateSupplierRiskLevelAsync(IEnumerable<int> supplierIds, string riskLevel)
        {
            var suppliers = await _dbSet.Where(s => supplierIds.Contains(s.Id)).ToListAsync();
            foreach (var supplier in suppliers)
            {
                supplier.RiskLevel = riskLevel;
                supplier.UpdatedDate = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> BulkUpdateSupplierCreditRatingAsync(IEnumerable<int> supplierIds, string creditRating)
        {
            var suppliers = await _dbSet.Where(s => supplierIds.Contains(s.Id)).ToListAsync();
            foreach (var supplier in suppliers)
            {
                supplier.CreditRating = creditRating;
                supplier.UpdatedDate = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> BulkBlacklistSuppliersAsync(IEnumerable<int> supplierIds, string reason)
        {
            var suppliers = await _dbSet.Where(s => supplierIds.Contains(s.Id)).ToListAsync();
            foreach (var supplier in suppliers)
            {
                supplier.IsBlacklisted = true;
                supplier.BlacklistReason = reason;
                supplier.BlacklistDate = DateTime.Now;
                supplier.UpdatedDate = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> BulkUnblacklistSuppliersAsync(IEnumerable<int> supplierIds)
        {
            var suppliers = await _dbSet.Where(s => supplierIds.Contains(s.Id)).ToListAsync();
            foreach (var supplier in suppliers)
            {
                supplier.IsBlacklisted = false;
                supplier.BlacklistReason = null;
                supplier.BlacklistDate = null;
                supplier.UpdatedDate = DateTime.Now;
            }
            return await _context.SaveChangesAsync();
        }
    }
}
