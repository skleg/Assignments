using CloudSales.Api.Contracts;
using CloudSales.Api.Extensions;
using CloudSales.Core.Interfaces;
using ErrorOrAspNetCoreExtensions;

namespace CloudSales.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/accounts")
            .WithTags("Accounts");

        group.MapGet("/", async (int pageNo,
                                 int pageSize,
                                 ISalesService salesService,
                                 CancellationToken ct) =>
        {
            // TODO: Extract customer Id from user claims
            int customerId = 7;
            var accounts = await salesService.GetAccountsAsync(customerId, pageNo, pageSize, ct);
            return accounts.ToOk(page => PageDto<AccountDto>.CreateFrom(page, account => account.ToDto()));
        })
        .Produces<PageDto<AccountDto>>()
        .WithSummary("Get customer accounts")
        .WithName("GetCustomerAccounts");

    }
}