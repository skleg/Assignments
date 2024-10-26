using CloudSales.Core.Entities;
using CloudSales.Core.Interfaces;
using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Persistence.Repository;

public class SalesRepository(AppDbContext dbContext) : ISalesRepository
{
    public Task<Account?> GetAccountAsync(int accountId, CancellationToken ct)
    {
        return dbContext.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId, ct);
    }

    public Task<List<Account>> GetAccountsAsync(int customerId, int pageNo, int pageSize, CancellationToken ct)
    {
        return dbContext.Accounts
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<Customer?> GetCustomerAsync(int customerId, CancellationToken ct)
    {
        return dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct);
    }

    public Task<List<Customer>> GetCustomersAsync(int pageNo, int pageSize, CancellationToken ct)
    {
        return dbContext.Customers
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<License?> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct)
    {
        return dbContext.Licenses.FirstOrDefaultAsync(x => x.AccountId == accountId && x.ServiceId == serviceId, ct);
    }

    public Task<List<License>> GetAccountLicensesAsync(int accountId, int pageNo, int pageSize, CancellationToken ct)
    {
        return dbContext.Licenses
            .Where(x => x.AccountId == accountId)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task UpdateAccountAsync(Account account, CancellationToken ct)
    {
        dbContext.Accounts.Update(account);
        return dbContext.SaveChangesAsync(ct);
    }

    public Task UpdateLicenseAsync(License license, CancellationToken ct)
    {
        dbContext.Licenses.Update(license);
        return dbContext.SaveChangesAsync(ct);
    }
}
