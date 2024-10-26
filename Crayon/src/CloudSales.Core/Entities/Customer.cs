namespace CloudSales.Core.Entities;

public class Customer
{
    public int CustomerId { get; set;}
    public string CustomerName { get; set;} = "";
    public string UserName { get; set;} = "";

    public virtual ICollection<Account> Accounts { get; set;} = default!;
}
