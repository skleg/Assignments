namespace CloudSales.Api.Contracts;

public record LicenseResponse(int ServiceId, string ServiceName, int Quantity, DateTime ExpiresAtUtc, bool IsActive);
