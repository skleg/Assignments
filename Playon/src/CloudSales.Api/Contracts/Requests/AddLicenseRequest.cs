namespace CloudSales.Api.Contracts.Requests;

public class AddLicenseRequest
{
    public int ServiceId { get; init; }
    public int NumberOfMonths { get; init; }
    public int NumberOfLicenses { get; set; }
}
