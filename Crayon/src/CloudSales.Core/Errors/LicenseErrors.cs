using ErrorOr;

namespace CloudSales.Core.Errors;

public static class LicenseErrors
{
    public static Error AlreadyExists => Error.Validation("License.AlreadyExists", "License already exists");
    public static Error InvalidNumberOfLicenses => Error.Validation("License.InvalidNumberOfLicenses", "Invalid number of licenses");
    public static Error InvalidNumberOfMonths => Error.Validation("License.InvalidNumberOfMonths", "Invalid number of months");
    public static Error FailedToCancelSubscription => Error.Failure("License.FailedToCancelSubscription", "Failed to cancel a subscription");
    public static Error FailedToCreateSubscription => Error.Failure("License.FailedToCreateSubscription", "Failed to purchase a subscription");
    public static Error FailedToUpdateSubscription => Error.Failure("License.FailedToUpdateSubscription", "Failed to update a subscription");
    public static Error NotActive => Error.Conflict("License.NotActive", "License is not active");
    public static Error NotFound => Error.NotFound("License.NotFound", "License not found");
    public static Error ServiceNotFound => Error.Validation("License.ServiceNotFound", "Service not found");
}
