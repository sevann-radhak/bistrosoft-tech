using Bistrosoft.Application.DTOs;
using Bistrosoft.Domain.Entities;

namespace Bistrosoft.Application.Mappings;

public static class CustomerMappingExtensions
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email.Value,
            customer.PhoneNumber,
            customer.Orders.Select(o => o.ToDto()).ToList()
        );
    }
}



