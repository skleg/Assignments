using System.Data;
using CloudSales.Core.Entities;
using CloudSales.Core.Shared;
using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Api.Extensions;

internal static class MigrationExtensions
{
    public static async Task ApplyMigrations(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        var hostingLifeTime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
        var cancellationToken = hostingLifeTime.ApplicationStopping;

        if (await context.Database.CanConnectAsync(cancellationToken))
        {
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pendingMigrations.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
                try
                {
                    logger.LogInformation("Migration Starts");
                    await context.Database.MigrateAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    logger.LogInformation("Migration Ended");
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    logger.LogError(e, "Database Migration failed on the startup initiation");
                }
            }
            else
            {
                logger.LogInformation("No pending changes to the database");
            }
        }
        else
        {
            logger.LogError("The database does not exist or connecting to the database failed.");
        }
    }

    public static async Task SeedInitialData(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hostingLifeTime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
        var cancellationToken = hostingLifeTime.ApplicationStopping;
        await SeedData.InitializeAsync(context, cancellationToken);
    }
}

internal static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        await dbContext.Database.EnsureCreatedAsync(ct);

        if (dbContext.Customers.Any())
            return;

        // Services
        var microsoftOffice = new Service
        {
            ServiceId = 1,
            ServiceName = "Microsoft Office 365",
            Price = 100,
        };
        var visualStudio = new Service
        {
            ServiceId = 2,
            ServiceName = "Visual Studio 2022",
            Price = 120,
        };

        // Customers
        var metrics = new Customer
        {
            CustomerName = "Metrics Corporation",
            UserName = "admin@metrics.com",
        };
        dbContext.Customers.Add(metrics);

        var olson = new Customer
        {
            CustomerName = "Olson Cosmetics AB",
            UserName = "admin@olson.se",
        };
        dbContext.Customers.Add(olson);

        await dbContext.SaveChangesAsync(ct);

        // Accounts
        var alice = new Account
        {
            CustomerId = metrics.CustomerId,
            FirstName = "Alice",
            LastName = "Smith",
        };
        dbContext.Accounts.Add(alice);

        var bob = new Account
        {
            CustomerId = olson.CustomerId,
            FirstName = "Bob",
            LastName = "Olson",
        };
        dbContext.Accounts.Add(bob);

        await dbContext.SaveChangesAsync(ct);

        // Licenses
        dbContext.Licenses.Add(new License
        {
            AccountId = alice.AccountId,
            ServiceId = microsoftOffice.ServiceId,
            ServiceName = microsoftOffice.ServiceName,
            Price = microsoftOffice.Price,
            Quantity = 1,
            State = LicenseState.Active,
            ExpiryDate = new DateTime(2025, 1, 31),
        });
        dbContext.Licenses.Add(new License
        {
            AccountId = bob.AccountId,
            ServiceId = microsoftOffice.ServiceId,
            ServiceName = microsoftOffice.ServiceName,
            Price = microsoftOffice.Price,
            Quantity = 1,
            State = LicenseState.Active,
            ExpiryDate = new DateTime(2025, 12, 31),
        });
        dbContext.Licenses.Add(new License
        {
            AccountId = bob.AccountId,
            ServiceId = visualStudio.ServiceId,
            ServiceName = visualStudio.ServiceName,
            Price = visualStudio.Price,
            Quantity = 1,
            State = LicenseState.Expired,
            ExpiryDate = new DateTime(2024, 7, 31),
        });

        await dbContext.SaveChangesAsync(ct);
    }
}