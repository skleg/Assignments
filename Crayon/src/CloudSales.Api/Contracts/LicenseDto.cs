namespace CloudSales.Api.Contracts;

public record LicenseDto(int ServiceId, string ServiceName, int Quantity, DateTime ExpiresAtUtc, bool IsActive);
