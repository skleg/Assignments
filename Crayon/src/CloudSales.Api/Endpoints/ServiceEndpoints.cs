using CloudSales.Api.Contracts;
using CloudSales.Api.Extensions;
using CloudSales.Core.Interfaces;

namespace CloudSales.Api.Endpoints;

public static class ServiceEndpoints
{
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/services")
            .WithTags("Services")
            .RequireAuthorization();

        group.MapGet("/", async (ICloudService cloudService, CancellationToken ct) =>
        {
            var services = await cloudService.GetServicesAsync(ct);
            return Results.Ok(services.Select(x => x.ToResponse()));
        })
        .Produces<IEnumerable<ServiceResponse>>()
        .WithSummary("Returns all services")
        .WithName("GetServices");

        group.MapGet("/{serviceId:int}", async (int serviceId, ICloudService cloudService, CancellationToken ct) =>
        {
            var service = await cloudService.GetServiceAsync(serviceId, ct);
            return service is null ? 
                Results.NotFound() : 
                Results.Ok(service.ToResponse());
        })
        .Produces<ServiceResponse>()
        .WithSummary("Returns a service")
        .WithName("GetService");

        return builder;
    }

}