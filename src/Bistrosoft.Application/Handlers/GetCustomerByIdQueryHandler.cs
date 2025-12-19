using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Mappings;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Interfaces;
using MediatR;

namespace Bistrosoft.Application.Handlers;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        return customer?.ToDto();
    }
}



