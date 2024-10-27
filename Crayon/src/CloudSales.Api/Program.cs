using CloudSales.Api.Endpoints;
using CloudSales.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.AddCloudAuthentication()
    .AddCloudDatabase()
    .AddCloudServices();

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

app.MapAccountEndpoints();

app.Run();
