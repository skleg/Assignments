using ErrorOr;

namespace CloudSales.Core.Errors;

public static class CommonErrors
{
    public static Error InvalidPagination => Error.Validation("Common.InvalidPagination", "Invalid pagination");
}