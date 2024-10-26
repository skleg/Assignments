using CloudSales.Core.Entities;
using CloudSales.Core.Shared;
using ErrorOr;

namespace CloudSales.Core.Interfaces;

public interface ISalesService
{
    Task<ErrorOr<EntityPage<Customer>>> GetCustomersAsync(int pageNo, int pageSize, CancellationToken ct = default);
    Task<ErrorOr<Customer>> GetCustomerAsync(int customerId, CancellationToken ct = default);
}