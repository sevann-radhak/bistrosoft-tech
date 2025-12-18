---
alwaysApply: true
---

# Design Patterns and Best Practices

## Repository Pattern

### Interface Definition
- Define in Domain or Application layer
- Generic base interface: `IRepository<T> where T : class`
- Specific interfaces extend base or define specific methods
- Methods return `Task<T>` or `Task<IEnumerable<T>>`

### Implementation Rules
- Implement in Infrastructure layer
- Use Entity Framework DbContext
- All methods must be async
- Handle exceptions appropriately
- Use `IQueryable<T>` for complex queries

### Example Structure
```csharp
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    // Additional methods...
}
```

## CQRS Pattern with MediatR

### Command Pattern
- Commands represent actions that change state
- Commands should be immutable (init-only properties)
- Commands return a result (DTO or success indicator)
- Use validation before execution

### Query Pattern
- Queries represent read operations
- Queries should be immutable
- Queries return DTOs, never entities directly
- Queries can include filtering, sorting, paging

### Handler Responsibilities
- Single responsibility: handle one command/query
- Use dependencies via constructor injection
- Validate input (or use pipeline behaviors)
- Execute business logic
- Map to DTOs
- Handle errors appropriately

### Pipeline Behaviors (Optional)
- Use for cross-cutting concerns:
  - Validation
  - Logging
  - Caching
  - Exception handling

## Dependency Injection

### Service Registration
- Register all dependencies in `Program.cs` or extension methods
- Use appropriate lifetimes:
  - **Scoped**: Repositories, DbContext (one per HTTP request)
  - **Transient**: Handlers, Services (new instance each time)
  - **Singleton**: Configuration, Logging (one instance for app lifetime)

### Interface-Based Design
- Depend on interfaces, not concrete types
- Define interfaces in the layer that uses them
- Implement interfaces in outer layers
- Use constructor injection (preferred over property injection)

### Registration Example
```csharp
// Application layer services
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationAssembly).Assembly));
services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);

// Infrastructure layer services
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IProductRepository, ProductRepository>();

// Application services
services.AddScoped<IMapper, Mapper>(); // Or AutoMapper configuration
```

## Mapping Pattern

### Object Mapping
- Use AutoMapper or Mapster
- Define mapping profiles
- Map entities to DTOs in handlers
- Never expose entities directly from API

### Mapping Rules
- Create separate DTOs for requests and responses
- Use projection for queries when possible (better performance)
- Handle null values appropriately
- Map nested objects correctly

## Validation Pattern

### FluentValidation
- Install FluentValidation
- Create validators for commands/queries
- Register validators with MediatR pipeline or DI
- Return validation errors in consistent format

### Validation Rules
- Validate required fields
- Validate data types and formats (email, phone)
- Validate business rules (stock availability, state transitions)
- Return clear error messages

## Error Handling Pattern

### Exception Types
- Create custom exception types:
  - `NotFoundException`
  - `ValidationException`
  - `BusinessRuleException`
- Inherit from `Exception` or create base exception class

### Global Exception Handler
- Implement middleware for exception handling
- Catch all exceptions
- Log exceptions appropriately
- Return consistent error response format
- Use `ProblemDetails` for error responses

### Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred",
  "status": 400,
  "errors": {
    "Email": ["Email is required", "Email format is invalid"]
  }
}
```

## Logging Pattern

### Structured Logging
- Use Serilog or built-in ILogger
- Log at appropriate levels:
  - **Information**: Normal operations
  - **Warning**: Recoverable issues
  - **Error**: Exceptions and errors
  - **Debug**: Detailed debugging information
- Include context in log messages
- Don't log sensitive information

### Logging in Handlers
```csharp
_logger.LogInformation("Creating customer with email {Email}", request.Email);
_logger.LogError(ex, "Error creating customer with email {Email}", request.Email);
```

## Async/Await Pattern

### Rules
- Always use `async/await` for I/O operations
- Use `CancellationToken` in async methods
- Return `Task` or `Task<T>`, never `void`
- Avoid blocking calls (`.Result`, `.Wait()`)
- Use `ConfigureAwait(false)` in library code (optional in ASP.NET Core)

### Example
```csharp
public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
{
    var customer = new Customer
    {
        Name = request.Name,
        Email = request.Email
    };

    await _repository.AddAsync(customer, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

    return _mapper.Map<CustomerDto>(customer);
}
```

## SOLID Principles Application

1. **Single Responsibility**: Each class has one reason to change
   - Handler handles one command/query
   - Repository handles data access for one entity
   - Controller handles HTTP concerns

2. **Open/Closed**: Open for extension, closed for modification
   - Use interfaces for extension points
   - Use strategy pattern for different behaviors

3. **Liskov Substitution**: Derived classes must be substitutable
   - Repository implementations must follow interface contracts
   - Handlers must implement MediatR interfaces correctly

4. **Interface Segregation**: Many specific interfaces over one general
   - `ICustomerRepository` instead of generic `IRepository<Customer>` with all methods
   - Separate read/write interfaces if needed

5. **Dependency Inversion**: Depend on abstractions
   - Controllers depend on MediatR (abstraction)
   - Handlers depend on repository interfaces
   - Infrastructure implements interfaces

