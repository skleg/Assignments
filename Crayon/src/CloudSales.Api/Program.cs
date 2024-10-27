using CloudSales.Api.Endpoints;
using CloudSales.Api.Extensions;
using CloudSales.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.AddCloudAuthentication()
    .AddCloudDatabase()
    .AddCloudServices();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapAccountEndpoints()
    .MapServiceEndpoints();

await app.UseCloudDatabaseAsync();

await app.RunAsync();
