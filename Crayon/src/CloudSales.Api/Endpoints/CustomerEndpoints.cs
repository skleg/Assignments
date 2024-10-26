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

        group.MapGet("/", async (int pageNo, int pageSize, ISalesService salesService, CancellationToken ct) =>
        {
            var customer = await salesService.GetCustomersAsync(pageNo, pageSize, ct);
            return customer.ToOk(page => PageDto<CustomerDto>.CreateFrom(page, customer => customer.ToDto()));
        })
        .Produces<PageDto<CustomerDto>>()
        .WithSummary("Get customers")
        .WithName("GetCustomers");

        group.MapGet("/{id:int}", async (int id, ISalesService salesService, CancellationToken ct) =>
        {
            var customer = await salesService.GetCustomerAsync(id, ct);
            return customer.ToOk(customer => customer.ToDto());
        })
        .Produces<CustomerDto>()
        .WithSummary("Get customer")
        .WithName("GetCustomer");
    }
}