using Entities.Entity;

namespace Bussiness.Repository.Abstract
{
    public interface ICustomerRepository : IGenericRepository<Customers>
    {
        // Temel filtreleme metodları
        Task<IEnumerable<Customers>> GetActiveCustomersAsync();
        Task<IEnumerable<Customers>> GetInactiveCustomersAsync();
        Task<IEnumerable<Customers>> GetBlacklistedCustomersAsync();


        // İstatistikler
        Task<int> GetActiveCustomerCountAsync();
        Task<decimal> GetTotalCustomerSalesAsync();
    }
}