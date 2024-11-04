using ErrorOr;

namespace CloudSales.Core.Errors;

public static class CustomerErrors
{
    public static Error NotFound => Error.NotFound("Customer.NotFound", "Customer not found");

}
