using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Mappings;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Interfaces;
using MediatR;

namespace Bistrosoft.Application.Handlers;

public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetCustomerOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
        return orders.Select(o => o.ToDto());
    }
}



