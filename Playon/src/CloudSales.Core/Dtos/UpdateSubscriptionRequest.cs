namespace CloudSales.Core.Dtos;

public record UpdateSubscriptionRequest(
    string UserName,
    int ServiceId,
    int NumberOfLicenses,
    DateTime ValidUntil);
