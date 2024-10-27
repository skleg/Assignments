using CloudSales.Api.Authentication;
using CloudSales.Api.Contracts;
using CloudSales.Api.Extensions;
using CloudSales.Core.Interfaces;
using ErrorOrAspNetCoreExtensions;

namespace CloudSales.Api.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/accounts")
            .WithTags("Accounts")
            .RequireAuthorization();

        group.MapGet("/", async (int pageNo,
                                 int pageSize,
                                 TenantContext tenantContext,
                                 ISalesService salesService,
                                 CancellationToken ct) =>
        {
            if (!tenantContext.IsValidCustomer)
                return Results.Unauthorized();

            var accounts = await salesService.GetAccountsAsync(tenantContext.CustomerId, pageNo, pageSize, ct);
            return accounts.ToOk(page => PageDto<AccountDto>.CreateFrom(page, account => account.ToDto()));
        })
        .Produces<PageDto<AccountDto>>()
        .WithSummary("Get customer accounts")
        .WithName("GetCustomerAccounts");

        return builder;
    }
}