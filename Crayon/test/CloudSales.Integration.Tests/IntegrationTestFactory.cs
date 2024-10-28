using System.Data.Common;
using System.Text;
using CloudSales.Authentication.Models;
using CloudSales.Authentication.Services;
using CloudSales.Core.Entities;
using CloudSales.Persistence.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    public Customer Customer { get; private set; } = default!;

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

            services.AddScoped<TokenGenerator>();

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {                
                options.TokenValidationParameters = new TokenValidationParameters 
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Key)),
                    ValidIssuer = _authSettings.Issuer,
                    ValidAudience = _authSettings.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
        });
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
        
        _tokenGenerator = services.GetRequiredService<TokenGenerator>();

        var customer = new Customer { 
            CustomerName = "Test Customer", 
            UserName = "customer@test.com"
        };
        await Db.Customers.AddAsync(customer);
        await Db.SaveChangesAsync();

        await Db.Accounts.AddRangeAsync(
        [
            new Account { 
                CustomerId = customer.CustomerId, 
                FirstName = "Bob",
                LastName = "Normal",
                UserName = "bob@example.com" },
            new Account { 
                CustomerId = customer.CustomerId, 
                FirstName = "Sally",
                LastName = "Cornell",
                UserName = "sally@example.com" }
        ]);
        await Db.SaveChangesAsync();

        Customer = await Db.Customers.Include(x => x.Accounts).FirstAsync();
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_connection);
    }

    public new async Task DisposeAsync()
    {
        await _connection.CloseAsync();
        await _container.DisposeAsync();
    }

    public string GetToken()
    {
        return _tokenGenerator.GenerateToken(Customer);
    }

}
