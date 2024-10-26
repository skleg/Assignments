using CloudSales.Api.Endpoints;

namespace CloudSales.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapCustomerEndpoints();
    }
}