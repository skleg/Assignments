namespace CloudSales.Authentication.Models;

public class AuthSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
