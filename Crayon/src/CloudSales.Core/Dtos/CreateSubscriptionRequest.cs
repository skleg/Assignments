namespace CloudSales.Core.Dtos;

public record CreateSubscriptionRequest(
    int ServiceId,
    string UserName,
    int NumberOfLicenses,
    int NumberOfMonths);
