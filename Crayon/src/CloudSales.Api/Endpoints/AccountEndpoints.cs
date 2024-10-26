using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            .WithTags("Accounts")
            .RequireAuthorization();

        group.MapGet("/", async (int pageNo,
                                 int pageSize,
                                 ClaimsPrincipal principal,
                                 ISalesService salesService,
                                 CancellationToken ct) =>
        {
            // Extract customer Id from user claims
            var customerClaimValue = principal.FindFirst("CustomerId")?.Value;
            if (string.IsNullOrEmpty(customerClaimValue))
                return Results.Unauthorized();

            int customerId = int.Parse(customerClaimValue);
            var accounts = await salesService.GetAccountsAsync(customerId, pageNo, pageSize, ct);
            return accounts.ToOk(page => PageDto<AccountDto>.CreateFrom(page, account => account.ToDto()));
        })
        .Produces<PageDto<AccountDto>>()
        .WithSummary("Get customer accounts")
        .WithName("GetCustomerAccounts");
    }
}