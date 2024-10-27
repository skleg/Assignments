using CloudSales.Authentication.Contracts;
using CloudSales.Core.Entities;

namespace CloudSales.Authentication.Extensions;

public static class MappingExtensions
{
    public static CustomerDto ToDto(this Customer customer) => new(customer.CustomerId, customer.CustomerName);
}
