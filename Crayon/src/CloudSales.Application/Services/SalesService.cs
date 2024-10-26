using CloudSales.Core.Entities;
using CloudSales.Core.Errors;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Shared;
using ErrorOr;

namespace CloudSales.Application.Services;

public class SalesService(ISalesRepository Repository) : ISalesService
{
    public async Task<ErrorOr<EntityPage<Customer>>> GetCustomersAsync(int pageNo, int pageSize, CancellationToken ct = default)
    {
        var pagination = new Pagination(pageNo, pageSize);
        return pagination.IsValid ? await Repository.GetCustomersAsync(pagination, ct) :
            CommonErrors.InvalidPagination;
    }
    
    public async Task<ErrorOr<Customer>> GetCustomerAsync(int customerId, CancellationToken ct = default)
    {
        var customer = await Repository.GetCustomerAsync(customerId, ct);
        return customer is null ? CustomerErrors.NotFound : customer;
    }
}
