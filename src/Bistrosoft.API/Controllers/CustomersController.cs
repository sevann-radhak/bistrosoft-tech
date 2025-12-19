using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all customers
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all customers</returns>
    /// <response code="200">Customers retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers(
        CancellationToken cancellationToken)
    {
        var query = new GetAllCustomersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    /// <param name="dto">Customer creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created customer with assigned ID</returns>
    /// <response code="201">Customer created successfully</response>
    /// <response code="400">Invalid input data or email already exists</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/customers
    ///     {
    ///         "name": "John Doe",
    ///         "email": "john.doe@example.com",
    ///         "phoneNumber": "+1234567890"
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CreateCustomerDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(
            dto.Name,
            dto.Email,
            dto.PhoneNumber
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a customer by ID including their orders
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer information with orders</returns>
    /// <response code="200">Customer found</response>
    /// <response code="404">Customer not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all orders for a specific customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of customer orders with order items</returns>
    /// <response code="200">Orders retrieved successfully</response>
    [HttpGet("{id}/orders")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetCustomerOrders(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerOrdersQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}



