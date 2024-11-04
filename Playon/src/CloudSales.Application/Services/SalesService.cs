using CloudSales.Core.Dtos;
using CloudSales.Core.Entities;
using CloudSales.Core.Errors;
using CloudSales.Core.Interfaces;
using CloudSales.Core.Shared;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CloudSales.Application.Services;

public class SalesService(ISalesRepository Repository, ICloudService CloudService, ILogger<SalesService> Logger) : ISalesService
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
        if (withMonths <= 0)
            return LicenseErrors.InvalidNumberOfMonths;
        
        var license = await Repository.GetLicenseAsync(accountId, serviceId, ct);
        if (license is null)
            return LicenseErrors.NotFound;

        var account = await Repository.GetAccountAsync(accountId, ct);
        if (account is null)
            return AccountErrors.NotFound;

        var expiryDate = license.ExpiryDate.AddMonths(withMonths);

        try
        {
            await CloudService.UpdateSubscriptionAsync(
                new UpdateSubscriptionRequest(account.UserName, license.ServiceId, license.Quantity, expiryDate), ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to update expiry date on subscription for account {AccountId} and service {ServiceId}", account.AccountId, license.ServiceId);
            return LicenseErrors.FailedToUpdateSubscription;
        }

        license.ExpiryDate = expiryDate;
        if (license.State == LicenseState.Expired && license.ExpiryDate > DateTime.UtcNow)
            license.State = LicenseState.Active;

        await Repository.UpdateLicenseAsync(license, ct);
        
        return license;
    }

    public async Task<ErrorOr<Deleted>> CancelLicenseAsync(int accountId, int serviceId, CancellationToken ct = default)
    {
        var license = await Repository.GetLicenseAsync(accountId, serviceId, ct);
        if (license is null)
            return LicenseErrors.NotFound;

        var account = await Repository.GetAccountAsync(accountId, ct);
        if (account is null)
            return AccountErrors.NotFound;

        try
        {
            await CloudService.CancelSubscriptionAsync(
                new CancelSubscriptionRequest(account.UserName, license.ServiceId), ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to cancel a subscription for account {AccountId} and service {ServiceId}", account.AccountId, license.ServiceId);
            return LicenseErrors.FailedToUpdateSubscription;
        }

        await Repository.DeleteLicenseAsync(license, ct);
        return Result.Deleted;
    }

    public async Task<ErrorOr<License>> UpdateNumberOfLicensesAsync(int accountId, int serviceId, int numberOfLicenses, CancellationToken ct = default)
    {
        if (numberOfLicenses <= 0)
            return LicenseErrors.InvalidNumberOfLicenses;
        
        var license = await Repository.GetLicenseAsync(accountId, serviceId, ct);
        if (license is null)
            return LicenseErrors.NotFound;

        if (license.State != LicenseState.Active)
            return LicenseErrors.NotActive;

        var account = await Repository.GetAccountAsync(accountId, ct);
        if (account is null)
            return AccountErrors.NotFound;

        try
        {
            await CloudService.UpdateSubscriptionAsync(
                new UpdateSubscriptionRequest(account.UserName, license.ServiceId, numberOfLicenses, license.ExpiryDate), ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to change a number of users on subscription for account {AccountId} and service {ServiceId}", account.AccountId, license.ServiceId);
            return LicenseErrors.FailedToUpdateSubscription;
        }

        license.Quantity = numberOfLicenses;
        await Repository.UpdateLicenseAsync(license, ct);
        
        return license;
    }

    public async Task<ErrorOr<License>> CreateLicenseAsync(CreateLicenseDto dto, CancellationToken ct = default)
    {
        if (dto.NumberOfLicenses <= 0)
            return LicenseErrors.InvalidNumberOfLicenses;
        
        if (dto.NumberOfMonths <= 0)
            return LicenseErrors.InvalidNumberOfMonths;

        var account = await Repository.GetAccountAsync(dto.AccountId, ct);
        if (account is null)
            return AccountErrors.NotFound;

        var service = await CloudService.GetServiceAsync(dto.ServiceId, ct);
        if (service is null)
            return LicenseErrors.ServiceNotFound;

        var license = await Repository.GetLicenseAsync(dto.AccountId, dto.ServiceId, ct);
        if (license is not null)
            return LicenseErrors.AlreadyExists;

        PurchaseReceiptDto receipt;

        try
        {
            receipt = await CloudService.CreateSubscriptionAsync(
                new CreateSubscriptionRequest(service.ServiceId, account.UserName, dto.NumberOfLicenses, dto.NumberOfMonths), 
                ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create a subscription for account {AccountId} and service {ServiceId}", account.AccountId, service.ServiceId);
            return LicenseErrors.FailedToCreateSubscription;
        }

        license = new License
        {
            AccountId = account.AccountId,
            State = LicenseState.Active, 

            ServiceId = receipt.ServiceId,
            ServiceName = receipt.ServiceName,
            Price = receipt.Price,
            Quantity = receipt.NumberOfLicenses,
            ExpiryDate = receipt.ValidUntil,
        };            

        await Repository.CreateLicenseAsync(license, ct);

        return license;
    }
}
