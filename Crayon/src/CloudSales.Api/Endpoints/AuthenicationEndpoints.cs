using CloudSales.Api.Authentication;
using CloudSales.Api.Contracts;
using ErrorOrAspNetCoreExtensions;

namespace CloudSales.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
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

    }
}