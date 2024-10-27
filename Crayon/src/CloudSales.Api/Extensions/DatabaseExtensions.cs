using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Api.Extensions;

public static class DatabaseExtensions
{    
    public static WebApplicationBuilder AddCloudDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options => 
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SalesDb") ?? 
                    throw new InvalidOperationException("SalesDb connection string is not configured."));
        });

        return builder;
    }
}