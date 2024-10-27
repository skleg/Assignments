using CloudSales.Api.Authentication;
using CloudSales.Api.Contracts;
using CloudSales.Api.Extensions;
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
            .RequireAuthorization("customer");

        group.MapGet("/", async (
            int pageNo,
            int pageSize,
            TenantContext tenantContext,
            ISalesService salesService,
            CancellationToken ct) =>
        {
            var accounts = await salesService.GetAccountsAsync(tenantContext.CustomerId, pageNo, pageSize, ct);
            return accounts.ToOk(page => PageDto<AccountDto>.CreateFrom(page, account => account.ToDto()));
        })
        .Produces<PageDto<AccountDto>>()
        .WithSummary("Get customer accounts")
        .WithName("GetCustomerAccounts");

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
                .ThenAsync(x => salesService.GetAccountLicensesAsync(accountId, pageNo, pageSize, ct));
            
            return licenses.ToOk(page => PageDto<LicenseDto>.CreateFrom(page, license => license.ToDto()));
        })
        .Produces<PageDto<LicenseDto>>()
        .WithSummary("Get account licenses")
        .WithName("GetAccountLicenses");

        return builder;
    }

    private static ErrorOr<Account> ValidateCustomerAccount(Account account, TenantContext tenantContext)
    {
        return account.CustomerId == tenantContext.CustomerId ?
            account : 
            AccountErrors.AccessDenied;
    }
}