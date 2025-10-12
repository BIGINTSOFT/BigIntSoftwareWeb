using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Entities.Entity;

namespace Bussiness.Repository.Concrete
{
    public class CustomerRepository : GenericRepository<Customers>, ICustomerRepository
    {
        public CustomerRepository(BigIntSoftwareDbContext context) : base(context)
        {
        }

        // Sadece temel işlevler - DevExtreme DataGrid kendi filtreleme özelliklerini kullanıyor

        public async Task<IEnumerable<Customers>> GetActiveCustomersAsync()
        {
            return await _dbSet.Where(c => c.IsActive == true).ToListAsync();
        }

        public async Task<IEnumerable<Customers>> GetInactiveCustomersAsync()
        {
            return await _dbSet.Where(c => c.IsActive != true).ToListAsync();
        }

        public async Task<IEnumerable<Customers>> GetBlacklistedCustomersAsync()
        {
            return await _dbSet.Where(c => c.IsBlacklisted == true).ToListAsync();
        }


        // İstatistikler için
        public async Task<int> GetActiveCustomerCountAsync()
        {
            return await _dbSet.CountAsync(c => c.IsActive == true);
        }

        public async Task<decimal> GetTotalCustomerSalesAsync()
        {
            return await _dbSet.SumAsync(c => c.TotalSales ?? 0);
        }
    }
}