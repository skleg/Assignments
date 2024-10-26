using CloudSales.Core.Shared;

namespace CloudSales.Core.Models;

public class License
{
    public int AccountId { get; set; }
    public int ServiceId { get; set; }
    public int CustomerId { get; set; }
    public LicenseState State { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
}
