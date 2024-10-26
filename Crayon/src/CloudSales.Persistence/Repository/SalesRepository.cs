using CloudSales.Core.Entities;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Shared;
using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Persistence.Repository;

public class SalesRepository(AppDbContext dbContext) : ISalesRepository
{
    public Task<Account?> GetAccountAsync(int accountId, CancellationToken ct)
    {
        return dbContext.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId, ct);
    }

    public async Task<EntityPage<Account>> GetAccountsAsync(int customerId, Pagination pagination, CancellationToken ct)
    {
        var query = dbContext.Accounts.Where(x => x.CustomerId == customerId);

        return new EntityPage<Account>(
            await query.Skip(pagination.Offset).Take(pagination.PageSize).ToListAsync(ct),
            await query.CountAsync(ct),
            pagination.PageNo,
            pagination.PageSize);
    }

    public Task<Customer?> GetCustomerAsync(int customerId, CancellationToken ct)
    {
        return dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct);
    }

    public async Task<EntityPage<Customer>> GetCustomersAsync(Pagination pagination, CancellationToken ct)
    {
        return new EntityPage<Customer>(
            await dbContext.Customers.Skip(pagination.Offset).Take(pagination.PageSize).ToListAsync(ct),
            await dbContext.Customers.CountAsync(ct),
            pagination.PageNo,
            pagination.PageSize);
    }

    public Task<License?> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct)
    {
        return dbContext.Licenses.FirstOrDefaultAsync(x => x.AccountId == accountId && x.ServiceId == serviceId, ct);
    }

    public async Task<EntityPage<License>> GetAccountLicensesAsync(int accountId, Pagination pagination, CancellationToken ct)
    {
        var query = dbContext.Licenses.Where(x => x.AccountId == accountId);

        return new EntityPage<License>(
            await query.Skip(pagination.Offset).Take(pagination.PageSize).ToListAsync(ct),
            await query.CountAsync(ct),
            pagination.PageNo,
            pagination.PageSize);
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
