using CloudSales.Core.Entities;

namespace CloudSales.Core.Interfaces;

public interface ISalesRepository
{
    public Task<List<Customer>> GetCustomersAsync(int pageNo, int pageSize, CancellationToken ct);
    public Task<Customer?> GetCustomerAsync(int customerId, CancellationToken ct);

    public Task<List<Account>> GetAccountsAsync(int customerId, int pageNo, int pageSize, CancellationToken ct);
    public Task<Account?> GetAccountAsync(int accountId, CancellationToken ct);
    public Task UpdateAccountAsync(Account account, CancellationToken ct);

    public Task<List<License>> GetAccountLicensesAsync(int accountId, int pageNo, int pageSize, CancellationToken ct);
    public Task<License?> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct);
    public Task UpdateLicenseAsync(License license, CancellationToken ct);
}
