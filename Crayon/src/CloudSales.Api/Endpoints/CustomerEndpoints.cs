using CloudSales.Api.Contracts;
using CloudSales.Api.Extensions;
using CloudSales.Core.Interfaces;
using ErrorOrAspNetCoreExtensions;

namespace CloudSales.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/customers")
            .WithTags("Customers");

        group.MapGet("/{id:int}", async (int id, ISalesService salesService, CancellationToken ct) =>
        {
            var customer = await salesService.GetCustomerAsync(id, ct);
            return customer.ToOk(x => x.ToDto());
        })
        .Produces<CustomerDto>()
        .WithSummary("Get customer")
        .WithName("GetCustomer");
    }
}