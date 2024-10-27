using CloudSales.Core.Entities;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Shared;
using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Persistence.Repository;

public class SalesRepository(AppDbContext dbContext) : ISalesRepository
{
    public async Task<Account?> GetAccountAsync(int accountId, CancellationToken ct)
    {
        return await dbContext.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId, ct);
    }

    public async Task<EntityPage<Account>> GetAccountsAsync(int customerId, Pagination pagination, CancellationToken ct)
    {
        var query = dbContext.Accounts.Where(x => x.CustomerId == customerId);

        return new EntityPage<Account>(
            await query
                .OrderBy(x => x.AccountId)
                .Skip(pagination.Offset)
                .Take(pagination.PageSize)
                .ToListAsync(ct),
            await query.CountAsync(ct),
            pagination.PageNo,
            pagination.PageSize);
    }

    public async Task<Customer?> GetCustomerAsync(int customerId, CancellationToken ct)
    {
        return await dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, ct);
    }

    public async Task<EntityPage<Customer>> GetCustomersAsync(Pagination pagination, CancellationToken ct)
    {
        return new EntityPage<Customer>(
            await dbContext.Customers
                .OrderBy(x => x.CustomerId)
                .Skip(pagination.Offset)
                .Take(pagination.PageSize)
                .ToListAsync(ct),
            await dbContext.Customers.CountAsync(ct),
            pagination.PageNo,
            pagination.PageSize);
    }

    public async Task<License?> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct)
    {
        return await dbContext.Licenses.FirstOrDefaultAsync(x => x.AccountId == accountId && x.ServiceId == serviceId, ct);
    }

    public async Task<EntityPage<License>> GetAccountLicensesAsync(int accountId, Pagination pagination, CancellationToken ct)
    {
        var query = dbContext.Licenses.Where(x => x.AccountId == accountId);

        return new EntityPage<License>(
            await query
                .OrderBy(x => x.ServiceName)
                .Skip(pagination.Offset)
                .Take(pagination.PageSize)
                .ToListAsync(ct),
            await query.CountAsync(ct),
            pagination.PageNo,
            pagination.PageSize);
    }

    public async Task UpdateAccountAsync(Account account, CancellationToken ct)
    {
        dbContext.Accounts.Update(account);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateLicenseAsync(License license, CancellationToken ct)
    {
        dbContext.Licenses.Update(license);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteLicenseAsync(License license, CancellationToken ct)
    {
        dbContext.Licenses.Remove(license);
        await dbContext.SaveChangesAsync(ct);
    }
}
