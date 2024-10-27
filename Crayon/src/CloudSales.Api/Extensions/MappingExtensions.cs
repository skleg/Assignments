using CloudSales.Api.Contracts;
using CloudSales.Core.Entities;
using CloudSales.Core.Shared;

namespace CloudSales.Api.Extensions;

public static class MappingExtensions
{
    public static AccountResponse ToResponse(this Account account) => 
        new(account.AccountId, account.FirstName, account.LastName);
    
    public static LicenseResponse ToResponse(this License license) => 
        new(license.ServiceId, 
            license.ServiceName, 
            license.Quantity, 
            license.ExpiryDate, 
            license.State == LicenseState.Active && license.ExpiryDate > DateTime.UtcNow);

    public static ServiceResponse ToResponse(this Service service) => 
        new(service.ServiceId, service.ServiceName, service.Price);
}
