using ErrorOr;

namespace CloudSales.Core.Errors;

public static class LicenseErrors
{
    public static Error NotActive => Error.Conflict("License.NotActive", "License is not active");
    public static Error NotFound => Error.NotFound("License.NotFound", "License not found");
}
