using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface ISupplierRepository : IGenericRepository<Suppliers>
    {
        // Temel filtreleme metodları
        Task<IEnumerable<Suppliers>> GetActiveSuppliersAsync();
        Task<IEnumerable<Suppliers>> GetInactiveSuppliersAsync();
        Task<IEnumerable<Suppliers>> GetBlacklistedSuppliersAsync();
        Task<IEnumerable<Suppliers>> GetSuppliersByTypeAsync(string supplierType);
        Task<IEnumerable<Suppliers>> GetSuppliersByCategoryAsync(string supplierCategory);
        Task<IEnumerable<Suppliers>> GetSuppliersByGroupAsync(string supplierGroup);
        Task<IEnumerable<Suppliers>> GetSuppliersBySegmentAsync(string supplierSegment);

        // SAP entegrasyon metodları
        Task<Suppliers?> GetSupplierBySapVendorCodeAsync(string sapVendorCode);
        Task<IEnumerable<Suppliers>> GetSuppliersBySapAccountGroupAsync(string sapAccountGroup);
        Task<IEnumerable<Suppliers>> GetSuppliersBySapPurchasingOrgAsync(string sapPurchasingOrg);
        Task<IEnumerable<Suppliers>> GetSuppliersBySapCompanyCodeAsync(string sapCompanyCode);

        // E-Fatura entegrasyon metodları
        Task<IEnumerable<Suppliers>> GetEInvoiceEnabledSuppliersAsync();
        Task<IEnumerable<Suppliers>> GetEArchiveEnabledSuppliersAsync();
        Task<IEnumerable<Suppliers>> GetEDeliveryNoteEnabledSuppliersAsync();
        Task<Suppliers?> GetSupplierByEInvoiceAliasAsync(string eInvoiceAlias);
        Task<Suppliers?> GetSupplierByTaxNumberAsync(string taxNumber);
        Task<Suppliers?> GetSupplierByTcNumberAsync(string tcNumber);

        // Kalite ve performans metodları
        Task<IEnumerable<Suppliers>> GetSuppliersByQualityRatingAsync(decimal minRating);
        Task<IEnumerable<Suppliers>> GetSuppliersByDeliveryRatingAsync(decimal minRating);
        Task<IEnumerable<Suppliers>> GetSuppliersByOverallRatingAsync(decimal minRating);
        Task<IEnumerable<Suppliers>> GetSuppliersByRiskLevelAsync(string riskLevel);
        Task<IEnumerable<Suppliers>> GetSuppliersByCreditRatingAsync(string creditRating);

        // Sertifikasyon metodları
        Task<IEnumerable<Suppliers>> GetSuppliersByQualityCertificationAsync(string certification);
        Task<IEnumerable<Suppliers>> GetSuppliersByIsoCertificationAsync(string isoCertification);
        Task<IEnumerable<Suppliers>> GetSuppliersWithExpiredCertificationsAsync();
        Task<IEnumerable<Suppliers>> GetSuppliersWithExpiringCertificationsAsync(int daysAhead);

        // Finansal metodlar
        Task<IEnumerable<Suppliers>> GetSuppliersByCreditLimitAsync(decimal minCreditLimit);
        Task<IEnumerable<Suppliers>> GetSuppliersByPaymentTermDaysAsync(int maxPaymentTermDays);
        Task<IEnumerable<Suppliers>> GetSuppliersByCurrencyAsync(string currency);

        // İstatistikler
        Task<int> GetActiveSupplierCountAsync();
        Task<int> GetTotalSupplierCountAsync();
        Task<decimal> GetTotalSupplierPurchasesAsync();
        Task<decimal> GetAverageSupplierRatingAsync();
        Task<int> GetSuppliersWithEInvoiceEnabledCountAsync();
        Task<int> GetSuppliersWithEArchiveEnabledCountAsync();
        Task<int> GetSuppliersWithEDeliveryNoteEnabledCountAsync();

        // Arama ve filtreleme
        Task<IEnumerable<Suppliers>> SearchSuppliersAsync(string searchTerm);
        Task<IEnumerable<Suppliers>> GetSuppliersByCityAsync(string city);
        Task<IEnumerable<Suppliers>> GetSuppliersByCountryAsync(string country);
        Task<IEnumerable<Suppliers>> GetSuppliersByIndustryCodeAsync(string industryCode);
        Task<IEnumerable<Suppliers>> GetSuppliersByActivityCodeAsync(string activityCode);

        // Raporlama
        Task<IEnumerable<Suppliers>> GetTopRatedSuppliersAsync(int count);
        Task<IEnumerable<Suppliers>> GetTopPurchasingSuppliersAsync(int count);
        Task<IEnumerable<Suppliers>> GetRecentlyAddedSuppliersAsync(int count);
        Task<IEnumerable<Suppliers>> GetSuppliersWithRecentOrdersAsync(int days);
        Task<IEnumerable<Suppliers>> GetSuppliersWithRecentDeliveriesAsync(int days);
        Task<IEnumerable<Suppliers>> GetSuppliersWithRecentPaymentsAsync(int days);

        // Performans metrikleri
        Task<decimal> GetAverageOnTimeDeliveryRateAsync();
        Task<decimal> GetAverageQualityAcceptanceRateAsync();
        Task<decimal> GetAveragePriceCompetitivenessAsync();
        Task<decimal> GetAverageServiceLevelAsync();
        Task<int> GetAverageDeliveryDaysAsync();

        // Tedarikçi yönetimi
        Task<bool> IsSupplierBlacklistedAsync(int supplierId);
        Task<bool> IsSupplierActiveAsync(int supplierId);
        Task<bool> HasValidCertificationsAsync(int supplierId);
        Task<bool> IsEInvoiceEnabledAsync(int supplierId);
        Task<bool> IsEArchiveEnabledAsync(int supplierId);
        Task<bool> IsEDeliveryNoteEnabledAsync(int supplierId);

        // Toplu işlemler
        Task<int> BulkUpdateSupplierStatusAsync(IEnumerable<int> supplierIds, bool isActive);
        Task<int> BulkUpdateSupplierRiskLevelAsync(IEnumerable<int> supplierIds, string riskLevel);
        Task<int> BulkUpdateSupplierCreditRatingAsync(IEnumerable<int> supplierIds, string creditRating);
        Task<int> BulkBlacklistSuppliersAsync(IEnumerable<int> supplierIds, string reason);
        Task<int> BulkUnblacklistSuppliersAsync(IEnumerable<int> supplierIds);
    }
}
