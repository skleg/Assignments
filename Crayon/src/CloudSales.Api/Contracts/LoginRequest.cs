namespace CloudSales.Api.Contracts;

public class LoginRequest
{
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
}