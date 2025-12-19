using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Mappings;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Interfaces;
using Bistrosoft.Domain.ValueObjects;
using MediatR;

namespace Bistrosoft.Application.Handlers;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingCustomer != null)
        {
            throw new InvalidOperationException($"Customer with email '{request.Email}' already exists.");
        }

        var email = Email.Create(request.Email);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = email,
            PhoneNumber = request.PhoneNumber
        };

        var createdCustomer = await _customerRepository.AddAsync(customer, cancellationToken);
        return createdCustomer.ToDto();
    }
}

