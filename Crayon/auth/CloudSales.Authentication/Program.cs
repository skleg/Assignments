using CloudSales.Authentication.Endpoints;
using CloudSales.Authentication.Models;
using CloudSales.Authentication.Services;
using CloudSales.Persistence.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddDbContext<AppDbContext>(options => 
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SalesDb") ?? 
            throw new InvalidOperationException("SalesDb connection string is not configured."));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapAuthenticationEndpoints()
    .MapCustomerEndpoints();

app.Run();
