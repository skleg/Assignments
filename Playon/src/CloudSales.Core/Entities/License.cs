using CloudSales.Core.Shared;

namespace CloudSales.Core.Entities;

public class License
{
    public int AccountId { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set;} = "";
    public decimal Price { get; set; }
    public LicenseState State { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }

    public virtual Account Account { get; set; } = null!;
}
