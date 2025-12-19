using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Mappings;
using Bistrosoft.Domain.Enums;
using Bistrosoft.Domain.Exceptions;
using Bistrosoft.Domain.Interfaces;
using MediatR;

namespace Bistrosoft.Application.Handlers;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException("Order", request.OrderId);
        }

        if (!IsValidStatusTransition(order.Status, request.Status))
        {
            throw new BusinessRuleException(
                $"Invalid status transition from '{order.Status}' to '{request.Status}'.");
        }

        order.Status = request.Status;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return order.ToDto();
    }

    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => newStatus == OrderStatus.Paid || newStatus == OrderStatus.Cancelled,
            OrderStatus.Paid => newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled,
            OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
            OrderStatus.Delivered => false,
            OrderStatus.Cancelled => false,
            _ => false
        };
    }
}



