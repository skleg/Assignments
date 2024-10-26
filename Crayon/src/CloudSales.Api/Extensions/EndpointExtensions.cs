using CloudSales.Api.Endpoints;

namespace CloudSales.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapAuthenticationEndpoints();
            app.MapCustomerEndpoints();
        }

        app.MapAccountEndpoints();
    }
}