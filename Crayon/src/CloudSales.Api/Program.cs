using CloudSales.Api.Endpoints;
using CloudSales.Api.Extensions;
using CloudSales.Application.Services;
using CloudSales.Core.Interfaces;
using CloudSales.Persistence.Database;
using CloudSales.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SalesDb") ?? 
            throw new InvalidOperationException("SalesDb connection string is not configured."));
});

builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISalesService, SalesService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrations();
}

if (app.Environment.IsDevelopment())
{
    await app.Services.SeedInitialData();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapCustomerEndpoints();
}

app.UseHttpsRedirection();

app.MapApiEndpoints();

app.Run();
