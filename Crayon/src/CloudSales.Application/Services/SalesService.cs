using CloudSales.Core.Entities;
using CloudSales.Core.Errors;
using CloudSales.Core.Interfaces;
using ErrorOr;

namespace CloudSales.Application.Services;

public class SalesService(ISalesRepository Repository)
{
    public async Task<ErrorOr<Customer>> GetCustomerAsync(int customerId, CancellationToken ct = default)
    {
        var customer = await Repository.GetCustomerAsync(customerId, ct);
        return customer is null ? CustomerErrors.NotFound : customer;
    }
}
