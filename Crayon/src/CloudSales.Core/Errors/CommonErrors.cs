using ErrorOr;

namespace CloudSales.Core.Errors;

public static class CommonErrors
{
    public static Error InvalidPagination => Error.Conflict("Common.InvalidPagination", "Invalid pagination");
}