namespace CloudSales.Core.Dtos;

public class PurchaseReceiptDto
{
    public int ServiceId { get; init;}
    public string ServiceName { get; init;} = "";
    public int NumberOfLicenses { get; init;}
    public decimal Price { get; init; }
    public string UserName { get; init;} = "";
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }
}
