using CloudSales.Core.Entities;
using CloudSales.Core.Errors;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Shared;
using ErrorOr;

namespace CloudSales.Application.Services;

public class SalesService(ISalesRepository Repository) : ISalesService
{
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

    public async Task<ErrorOr<EntityPage<License>>> GetLicensesAsync(int accountId, int pageNo, int pageSize, CancellationToken ct = default)
    {
        var pagination = new Pagination(pageNo, pageSize);
        if (!pagination.IsValid)
            return CommonErrors.InvalidPagination;

        var account = await Repository.GetAccountAsync(accountId, ct);
        if (account is null)
            return AccountErrors.NotFound;
        
        return await Repository.GetAccountLicensesAsync(account.AccountId, pagination, ct);
    }

    public async Task<ErrorOr<License>> GetLicenseAsync(int accountId, int serviceId, CancellationToken ct = default)
    {
        var license = await Repository.GetLicenseAsync(accountId, serviceId, ct);
        return license is null ? LicenseErrors.NotFound : license;
    }

    public async Task<ErrorOr<License>> ExtendLicenseAsync(int accountId, int serviceId, int withMonths, CancellationToken ct = default)
    {
        var license = await Repository.GetLicenseAsync(accountId, serviceId, ct);
        if (license is null)
            return LicenseErrors.NotFound;

        if (license.State != LicenseState.Active)
            return LicenseErrors.NotActive;
        
        license.ExpiryDate = license.ExpiryDate.AddMonths(withMonths);
        await Repository.UpdateLicenseAsync(license, ct);
        
        return license;
    }

    public async Task<ErrorOr<Deleted>> CancelLicenseAsync(int accountId, int serviceId, CancellationToken ct = default)
    {
        var license = await Repository.GetLicenseAsync(accountId, serviceId, ct);
        if (license is null)
            return LicenseErrors.NotFound;

        await Repository.DeleteLicenseAsync(license, ct);        
        return Result.Deleted;
    }
}
