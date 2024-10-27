using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudSales.Authentication.Models;
using CloudSales.Core.Errors;
using CloudSales.Persistence.Database;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CloudSales.Authentication.Services;

public class TokenGenerator(IOptions<AuthSettings> configurationOptions, AppDbContext dbContext)
{
    private readonly AuthSettings _configuration = configurationOptions.Value;
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<ErrorOr<string>> GenerateTokenAsync(string userName, string password, CancellationToken ct = default)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.UserName == userName, ct);
        if (customer is null)
            return CommonErrors.InvalidCredentials;

        if (!string.Equals(password, _configuration.Password))
            return CommonErrors.InvalidCredentials;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, customer.UserName),
            new("CustomerId", customer.CustomerId.ToString()),
        };

        var key = Encoding.UTF8.GetBytes(_configuration.Key);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _configuration.Issuer,
            Audience = _configuration.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
