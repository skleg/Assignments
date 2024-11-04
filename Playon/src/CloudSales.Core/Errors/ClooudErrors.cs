using ErrorOr;

namespace CloudSales.Core.Errors;

public static class CloudErrors
{
    public static Error ServiceNotFound => Error.NotFound("Account.ServiceNotFound", "Service not found");

}
