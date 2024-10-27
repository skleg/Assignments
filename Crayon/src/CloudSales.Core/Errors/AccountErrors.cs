using ErrorOr;

namespace CloudSales.Core.Errors;

public static class AccountErrors
{
    public static Error AccessDenied => Error.NotFound("Account.AccessDenied", "Account belongs to another customer");
    public static Error NotFound => Error.NotFound("Account.NotFound", "Account not found");

}
