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
        if (!pagination.IsValid)
            return CommonErrors.InvalidPagination;

        return await Repository.GetCustomersAsync(pagination, ct);
    }
    
    public async Task<ErrorOr<Customer>> GetCustomerAsync(int customerId, CancellationToken ct = default)
    {
        var customer = await Repository.GetCustomerAsync(customerId, ct);
        return customer is null ? CustomerErrors.NotFound : customer;
    }

    public async Task<ErrorOr<EntityPage<Account>>> GetAccountsAsync(int customerId, int pageNo, int pageSize, CancellationToken ct = default)
    {
        var pagination = new Pagination(pageNo, pageSize);
        if (!pagination.IsValid)
            return CommonErrors.InvalidPagination;

        var customer = await Repository.GetCustomerAsync(customerId, ct);
        if (customer is null)
            return CustomerErrors.NotFound;
        
        return await Repository.GetAccountsAsync(customer.CustomerId, pagination, ct);
    }

    public async Task<ErrorOr<Account>> GetAccountAsync(int accountId, CancellationToken ct = default)
    {
        var account = await Repository.GetAccountAsync(accountId, ct);
        return account is null ? AccountErrors.NotFound : account;
    }
}
