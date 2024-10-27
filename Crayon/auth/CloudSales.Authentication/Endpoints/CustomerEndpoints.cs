using CloudSales.Authentication.Contracts;
using CloudSales.Authentication.Extensions;
using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Authentication.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/customers")
            .WithTags("Customers");

        group.MapGet("/", async (AppDbContext dbContext, CancellationToken ct) =>
        {
            var customers = await dbContext.Customers
                .Take(100)
                .ToListAsync(ct);

            return Results.Ok(customers.Select(x => x.ToDto()));
        })
        .Produces<IEnumerable<CustomerDto>>()
        .WithSummary("Get customers")
        .WithName("GetCustomers");

        group.MapGet("/{id:int}", async (int id, AppDbContext dbContext, CancellationToken ct) =>
        {
            var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == id, ct);
            return customer is null ? Results.NotFound() : Results.Ok(customer.ToDto());
        })
        .Produces<CustomerDto>()
        .WithSummary("Get customer")
        .WithName("GetCustomer");

        return builder;
    }
}