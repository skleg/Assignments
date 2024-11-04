using CloudSales.Api.Authentication;
using CloudSales.Api.Services;
using CloudSales.Application.Services;
using CloudSales.Core.Interfaces;
using CloudSales.Persistence.Repository;

namespace CloudSales.Api.Extensions;

public static class ServicesExtensions
{    
    public static WebApplicationBuilder AddCloudServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddScoped<TenantContext>();
        builder.Services.AddScoped<ISalesRepository, SalesRepository>();
        builder.Services.AddScoped<ISalesService, SalesService>();
        builder.Services.AddScoped<ICloudService, CloudServiceMock>();

        return builder;
    }
}