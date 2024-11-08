using CloudSales.Api.Authentication;
using CloudSales.Api.Contracts;
using CloudSales.Api.Contracts.Requests;
using CloudSales.Api.Extensions;
using CloudSales.Core.Dtos;
using CloudSales.Core.Entities;
using CloudSales.Core.Errors;
using CloudSales.Core.Interfaces;
using ErrorOr;
using ErrorOrAspNetCoreExtensions;

namespace CloudSales.Api.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/accounts")
            .WithTags("Accounts")
            .WithOpenApi()
            .RequireAuthorization("customer");

        group.MapGet("/", async (
            int pageNo,
            int pageSize,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var accounts = await salesService.GetAccountsAsync(tenantContext.CustomerId, pageNo, pageSize, ct);
            return accounts.ToOk(page => PageResponse<AccountResponse>.CreateFrom(page, account => account.ToResponse()));
        })
        .Produces<PageResponse<AccountResponse>>()
        .WithSummary("Returns customer accounts")
        .WithName("GetAccounts");

        group.MapGet("/{accountId:int}", async (
            int accountId,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var account = await salesService.GetAccountAsync(accountId, ct)
                .Then(a => ValidateCustomerAccount(a, tenantContext));
            
            return account.ToOk(x => x.ToResponse());
        })
        .Produces<AccountResponse>()
        .WithSummary("Returns a customer account")
        .WithName("GetAccount");

        group.MapGet("/{accountId:int}/licenses", async (
            int accountId,
            int pageNo,
            int pageSize,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var licenses = await salesService.GetAccountAsync(accountId, ct)
                .Then(account => ValidateCustomerAccount(account, tenantContext))
                .ThenAsync(x => salesService.GetLicensesAsync(accountId, pageNo, pageSize, ct));
            
            return licenses.ToOk(page => PageResponse<LicenseResponse>.CreateFrom(page, license => license.ToResponse()));
        })
        .Produces<PageResponse<LicenseResponse>>()
        .WithSummary("Returns licenses for an account")
        .WithName("GetLicenses");

        group.MapPost("/{accountId:int}/licenses", async (
            int accountId,
            AddLicenseRequest request,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var license = await salesService.GetAccountAsync(accountId, ct)
                .Then(account => ValidateCustomerAccount(account, tenantContext))
                .ThenAsync(account => salesService.CreateLicenseAsync(
                    new CreateLicenseDto(account.AccountId, request.ServiceId, request.NumberOfMonths, request.NumberOfLicenses), 
                    ct));

            return license.ToOk(x => x.ToResponse());
        })
        .Produces<LicenseResponse>()
        .WithSummary("Creates a new user license")
        .WithName("AddLicense");

        group.MapGet("/{accountId:int}/licenses/{serviceId:int}", async (
            int accountId,
            int serviceId,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var license = await salesService.GetAccountAsync(accountId, ct)
                .Then(account => ValidateCustomerAccount(account, tenantContext))
                .ThenAsync(x => salesService.GetLicenseAsync(accountId, serviceId, ct));
            
            return license.ToOk(x => x.ToResponse());
        })
        .Produces<LicenseResponse>()
        .WithSummary("Returns user license")
        .WithName("GetLicense");

        group.MapPost("/{accountId:int}/licenses/{serviceId:int}/extend", async (
            int accountId,
            int serviceId,
            ExtendLicenseRequest request,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var license = await salesService.GetAccountAsync(accountId, ct)
                .Then(account => ValidateCustomerAccount(account, tenantContext))
                .ThenAsync(x => salesService.ExtendLicenseAsync(accountId, serviceId, request.NumberOfMonths, ct));
            
            return license.ToOk(x => x.ToResponse());
        })
        .Produces<LicenseResponse>()
        .WithSummary("Extends a license with specified number of months")
        .WithName("ExtendLicense");

        group.MapPost("/{accountId:int}/licenses/{serviceId:int}/cancel", async (
            int accountId,
            int serviceId,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var license = await salesService.GetAccountAsync(accountId, ct)
                .Then(account => ValidateCustomerAccount(account, tenantContext))
                .ThenAsync(x => salesService.CancelLicenseAsync(accountId, serviceId, ct));
            
            return license.ToNoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Cancels a license")
        .WithName("CancelLicense");

        group.MapPut("/{accountId:int}/licenses/{serviceId:int}", async (
            int accountId,
            int serviceId,
            UpdateLicenseRequest request,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var license = await salesService.GetAccountAsync(accountId, ct)
                .Then(account => ValidateCustomerAccount(account, tenantContext))
                .ThenAsync(x => salesService.UpdateNumberOfLicensesAsync(accountId, serviceId, request.NumberOfLicenses, ct));
            
            return license.ToOk(x => x.ToResponse());
        })
        .Produces<LicenseResponse>()
        .WithSummary("Updates number of user licenses")
        .WithName("UpdateLicense");

        return builder;
    }

    private static ErrorOr<Account> ValidateCustomerAccount(Account account, TenantContext tenantContext)
    {
        return account.CustomerId == tenantContext.CustomerId ?
            account : 
            AccountErrors.AccessDenied;
    }
}