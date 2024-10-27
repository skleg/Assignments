using CloudSales.Core.Entities;
using CloudSales.Core.Shared;

namespace CloudSales.Core.Interfaces;

public interface ISalesRepository
{
    public Task<EntityPage<Customer>> GetCustomersAsync(Pagination pagination, CancellationToken ct);
    public Task<Customer?> GetCustomerAsync(int customerId, CancellationToken ct);

    public Task<EntityPage<Account>> GetAccountsAsync(int customerId, Pagination pagination, CancellationToken ct);
    public Task<Account?> GetAccountAsync(int accountId, CancellationToken ct);
    public Task UpdateAccountAsync(Account account, CancellationToken ct);

    public Task<EntityPage<License>> GetAccountLicensesAsync(int accountId, Pagination pagination, CancellationToken ct);
    public Task<License?> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct);
    public Task CreateLicenseAsync(License license, CancellationToken ct);
    public Task UpdateLicenseAsync(License license, CancellationToken ct);
    public Task DeleteLicenseAsync(License license, CancellationToken ct);
}
