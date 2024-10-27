namespace CloudSales.Core.Dtos;

public record PurchaseRequest(
    int ServiceId,
    string UserName,
    int NumberOfLicenses,
    int NumberOfMonths);
