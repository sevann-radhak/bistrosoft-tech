using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Mappings;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Interfaces;
using MediatR;

namespace Bistrosoft.Application.Handlers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(c => c.ToDto());
    }
}
