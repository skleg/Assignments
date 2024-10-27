using CloudSales.Api.Authentication;
using CloudSales.Api.Extensions;
using CloudSales.Application.Services;
using CloudSales.Core.Interfaces;
using CloudSales.Persistence.Database;
using CloudSales.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddAuthorization();
builder.AddAddJwtBearerAuthentication();

builder.Services.AddDbContext<AppDbContext>(options => 
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SalesDb") ?? 
            throw new InvalidOperationException("SalesDb connection string is not configured."));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<TokenGenerator>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrations();
}

if (app.Environment.IsDevelopment())
{
    await app.Services.SeedInitialData();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapApiEndpoints();

app.Run();
