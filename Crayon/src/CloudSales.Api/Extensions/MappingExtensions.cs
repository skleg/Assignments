using CloudSales.Api.Contracts;
using CloudSales.Core.Entities;

namespace CloudSales.Api.Extensions;

public static class MappingExtensions
{
    public static CustomerDto ToDto(this Customer customer) => new(customer.CustomerId, customer.CustomerName);
    public static AccountDto ToDto(this Account account) => new(account.AccountId, account.FirstName, account.LastName);
}
