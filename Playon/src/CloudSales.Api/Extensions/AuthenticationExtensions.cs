using System.Text;
using CloudSales.Api.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CloudSales.Api.Extensions;

public static class AuthenticationExtensions
{    
    public static WebApplicationBuilder AddCloudAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x => 
            {
                var jwt = builder.Configuration.GetSection("Jwt").Get<AuthSettings>() ??
                    throw new InvalidOperationException("Jwt settings are not configured.");

                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters 
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("customer", policy => policy.RequireClaim("CustomerId"))
            .AddPolicy("account", policy => policy.RequireClaim("AccountId"));

        return builder;
    }
}