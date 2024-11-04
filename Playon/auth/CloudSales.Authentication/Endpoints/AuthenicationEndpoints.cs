using CloudSales.Api.Contracts;
using CloudSales.Authentication.Contracts;
using CloudSales.Authentication.Services;
using ErrorOrAspNetCoreExtensions;

namespace CloudSales.Authentication.Endpoints;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/login", async (LoginRequest request, TokenGenerator tokenGenerator) =>
        {
            var result = await tokenGenerator.GenerateTokenAsync(request.UserName, request.Password);
            return result.ToOk(token => new TokenDto(token));
        })
        .Produces<TokenDto>()
        .WithName("Login")
        .WithTags("Authentication")
        .WithSummary("User login");

        return builder;
    }
}