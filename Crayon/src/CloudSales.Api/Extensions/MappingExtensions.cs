using System;
using CloudSales.Api.Contracts;
using CloudSales.Core.Entities;

namespace CloudSales.Api.Extensions;

public static class MappingExtensions
{
    public static CustomerDto ToDto(this Customer customer) => new(customer.CustomerId, customer.CustomerName);
}
