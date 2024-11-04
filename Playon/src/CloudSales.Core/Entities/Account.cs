namespace CloudSales.Core.Entities;

public class Account
{
    public int AccountId { get; set;}
    public int CustomerId { get; set;}
    public string UserName { get; set;} = "";
    public string FirstName { get; set;} = "";
    public string LastName { get; set;} = "";

    public virtual ICollection<License> Licenses { get; set;} = default!;
    public virtual Customer Customer { get; set; } = null!;

}
