using System.Data.Common;
using CloudSales.Authentication.Models;
using CloudSales.Authentication.Services;
using CloudSales.Core.Entities;
using CloudSales.Persistence.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Respawn;
using Testcontainers.SqlEdge;

namespace CloudSales.Integration.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqlEdgeContainer _container = new SqlEdgeBuilder()
        .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
        .Build();
    
    private readonly AuthSettings _authSettings = new()
    {
        Audience = "audience",
        Issuer = "issuer",
        Key = "TKYGRccRV72aNQMtNkqtur9vOUHocGVH",
        Password = "password",
    };

    private TokenGenerator _tokenGenerator = default!;
    public AppDbContext Db { get; private set; } = default!;
    private DbConnection _connection = default!;
    private Respawner _respawner = default!;
    private Customer _customer = default!;

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        var services = Services.CreateScope().ServiceProvider;
        
        Db = services.GetRequiredService<AppDbContext>();

        _connection = Db.Database.GetDbConnection();
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            TablesToInclude = ["Accounts", "Licenses"],
        });
        
        _tokenGenerator = new (services.GetService<IOptions<AuthSettings>>()!, Db);

        _customer = new Customer { 
            CustomerName = "Test Customer", 
            UserName = "customer@test.com" 
        };
        await Db.Customers.AddAsync(_customer);
        await Db.SaveChangesAsync();
    }

    public new async Task DisposeAsync()
    {
        await _connection.CloseAsync();
        await _container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<AppDbContext>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_container.GetConnectionString());
            });
            services.EnsureDbCreated<AppDbContext>();

            services.Configure<AuthSettings>(x => 
            {
               x.Audience = _authSettings.Audience;
               x.Issuer = _authSettings.Issuer;
               x.Key = _authSettings.Key;
               x.Password = _authSettings.Password;
            });
        });
    }

    public string GetToken()
    {
        return _tokenGenerator.GenerateToken(_customer);
    }

    public Account CreateAccount()
    {
        return new Account { 
            CustomerId = _customer.CustomerId, 
            FirstName = "Bob",
            LastName = "Normal",
            UserName = "bob@example.com",
        };
    }
}
