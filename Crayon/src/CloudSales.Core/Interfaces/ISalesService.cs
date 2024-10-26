using CloudSales.Core.Entities;
using CloudSales.Core.Shared;
using ErrorOr;

namespace CloudSales.Core.Interfaces;

public interface ISalesService
{
    Task<ErrorOr<EntityPage<Customer>>> GetCustomersAsync(int pageNo, int pageSize, CancellationToken ct = default);
    Task<ErrorOr<Customer>> GetCustomerAsync(int customerId, CancellationToken ct = default);
    Task<ErrorOr<EntityPage<Account>>> GetAccountsAsync(int customerId, int pageNo, int pageSize, CancellationToken ct = default);
    Task<ErrorOr<Account>> GetAccountAsync(int accountId, CancellationToken ct = default);

}