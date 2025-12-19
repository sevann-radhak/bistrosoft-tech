using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order for a customer
    /// </summary>
    /// <param name="dto">Order creation data including customer ID and items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order with calculated total amount</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid input data or insufficient stock</response>
    /// <response code="404">Customer or product not found</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/orders
    ///     {
    ///         "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "items": [
    ///             {
    ///                 "productId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    ///                 "quantity": 2
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        var items = dto.Items.Select(item => new OrderItemRequest(
            item.ProductId,
            item.Quantity
        )).ToList();

        var command = new CreateOrderCommand(
            dto.CustomerId,
            items
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order information with order items</returns>
    /// <response code="200">Order found</response>
    /// <response code="404">Order not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Updates the status of an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="dto">Status update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated order</returns>
    /// <response code="200">Order status updated successfully</response>
    /// <response code="400">Invalid status transition or invalid input</response>
    /// <response code="404">Order not found</response>
    /// <remarks>
    /// Valid status transitions:
    /// - Pending → Paid or Cancelled
    /// - Paid → Shipped or Cancelled
    /// - Shipped → Delivered
    /// - Delivered → (no transitions allowed)
    /// - Cancelled → (no transitions allowed)
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/orders/{id}/status
    ///     {
    ///         "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "status": "Paid"
    ///     }
    /// </remarks>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.OrderId)
        {
            return BadRequest("Order ID in URL does not match the ID in the request body");
        }

        var command = new UpdateOrderStatusCommand(
            dto.OrderId,
            dto.Status
        );

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}



