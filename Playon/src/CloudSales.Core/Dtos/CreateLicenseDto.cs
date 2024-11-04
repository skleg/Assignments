namespace CloudSales.Core.Dtos;

public record CreateLicenseDto(int AccountId,
                               int ServiceId,
                               int NumberOfMonths,
                               int NumberOfLicenses);
