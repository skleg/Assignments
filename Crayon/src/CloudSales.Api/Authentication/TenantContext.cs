namespace CloudSales.Api.Authentication;

public class TenantContext
{
    public TenantContext(IHttpContextAccessor accessor)
    {
        var claim = accessor.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == "CustomerId");
        if (claim is null) return;
        if (!int.TryParse(claim.Value, out int customerId)) return;
        CustomerId = customerId;
    }

    public int CustomerId { get; }
    public bool IsValidCustomer => CustomerId > 0;
}
