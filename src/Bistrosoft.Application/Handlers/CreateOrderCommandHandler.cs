using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Mappings;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Enums;
using Bistrosoft.Domain.Interfaces;
using MediatR;

namespace Bistrosoft.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{request.CustomerId}' not found.");
        }

        if (request.Items == null || !request.Items.Any())
        {
            throw new InvalidOperationException("Order must contain at least one item.");
        }

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID '{itemRequest.ProductId}' not found.");
            }

            if (product.StockQuantity < itemRequest.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemRequest.Quantity}");
            }

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = itemRequest.Quantity,
                UnitPrice = product.Price
            };

            orderItems.Add(orderItem);
            totalAmount += product.Price * itemRequest.Quantity;

            product.StockQuantity -= itemRequest.Quantity;
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = orderItems
        };

        foreach (var orderItem in orderItems)
        {
            orderItem.OrderId = order.Id;
        }

        var createdOrder = await _orderRepository.AddAsync(order, cancellationToken);
        return createdOrder.ToDto();
    }
}

