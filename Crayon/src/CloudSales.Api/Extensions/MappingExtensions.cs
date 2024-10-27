using CloudSales.Api.Contracts;
using CloudSales.Core.Entities;

namespace CloudSales.Api.Extensions;

public static class MappingExtensions
{
    public static AccountDto ToDto(this Account account) => new(account.AccountId, account.FirstName, account.LastName);
}