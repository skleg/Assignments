using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CloudSales.Persistence.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        return Create("SalesDb", 
            Directory.GetCurrentDirectory(),
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
    }

    private static AppDbContext Create(string connectionStringName, string basePath, string? environmentName)
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json");

        if (string.IsNullOrEmpty(environmentName))
            builder.AddJsonFile($"appsettings.{environmentName}.json", true);

        builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);

        var config = builder.Build();
        var connectionString = config.GetConnectionString(connectionStringName) ??
            throw new InvalidOperationException($"Could not find a connection string named '{connectionStringName}'.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
