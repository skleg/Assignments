using CloudSales.Core.Entities;

namespace CloudSales.Core.Dtos;

public record CreateLicenseDto(int AccountId,
                               int ServiceId,
                               DateTime StartDate,
                               int NumberOfMonths,
                               int NumberOfLicenses);
