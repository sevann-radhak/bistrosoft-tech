---
alwaysApply: true
---

# Backend Technical Rules - .NET 7/8/9

## .NET Framework Requirements

- **Target Framework**: .NET 7.0 or higher (.NET 8 recommended)
- **Language Version**: C# 11 or higher
- **Nullable Reference Types**: Enabled globally

## Entity Framework Core

### Configuration
- Use **EF Core 7.0 or 8.0**
- **In-Memory Database** for development/testing
- Optional: SQL Server for production
- Use **Fluent API** for entity configuration (preferred over Data Annotations)
- Configure relationships explicitly
- Use **Value Converters** for enums if needed

### Best Practices
- Use `IQueryable<T>` in repositories for deferred execution
- Always use `async/await` for database operations
- Use `SaveChangesAsync()` with proper error handling
- Configure indexes for frequently queried fields (e.g., Email)
- Use `HasIndex().IsUnique()` for unique constraints

### DbContext Rules
- Inherit from `DbContext`
- Use constructor injection for options
- Override `OnModelCreating` for Fluent API configuration
- Use `DbSet<T>` for entity sets
- Implement `IDbContextFactory<T>` if needed for testing

## MediatR Configuration

### Setup
- Install `MediatR` package
- Install `MediatR.Extensions.Microsoft.DependencyInjection`
- Register MediatR in DI: `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))`

### CQRS Pattern
- **Commands**: Actions that change state (Create, Update, Delete)
  - Implement `IRequest<TResponse>` or `IRequest`
  - Return DTOs or result objects
- **Queries**: Actions that read data
  - Implement `IRequest<TResponse>`
  - Return DTOs or entities (via DTOs)
- **Handlers**: Implement `IRequestHandler<TRequest, TResponse>`
- Use `async/await` in all handlers

### Handler Structure
```csharp
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

## Repository Pattern

### Interface Definition
- Define interfaces in Domain or Application layer
- Generic interface: `IRepository<T> where T : class`
- Specific interfaces: `ICustomerRepository : IRepository<Customer>`
- Include methods: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`
- Add specific methods as needed (e.g., `GetByEmailAsync`)

### Implementation
- Implement in Infrastructure layer
- Use `DbContext` for data access
- Return `Task<T>` or `Task<IEnumerable<T>>`
- Handle exceptions appropriately
- Use `IQueryable<T>` for complex queries

## Dependency Injection

### Service Registration
- Use `Microsoft.Extensions.DependencyInjection`
- Register in `Program.cs` or extension methods
- Use appropriate lifetimes:
  - `Scoped`: Repositories, DbContext
  - `Transient`: Handlers, Services
  - `Singleton`: Configuration, Logging

### Registration Pattern
```csharp
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddScoped<IProductRepository, ProductRepository>();
```

## API Configuration

### Controllers
- Use `[ApiController]` attribute
- Use `[Route("api/[controller]")]` for routing
- Return `ActionResult<T>` or `IActionResult`
- Use HTTP status codes appropriately:
  - `200 OK`: Success
  - `201 Created`: Resource created
  - `400 Bad Request`: Validation errors
  - `404 Not Found`: Resource not found
  - `500 Internal Server Error`: Server errors

### Endpoints
- Use RESTful conventions
- Use appropriate HTTP verbs (GET, POST, PUT, DELETE)
- Use route parameters for IDs: `[HttpGet("{id}")]`
- Use DTOs for request/response, never expose entities directly

## Swagger/OpenAPI

### Configuration
- Install `Swashbuckle.AspNetCore`
- Configure in `Program.cs`:
  ```csharp
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(c =>
  {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bistrosoft API", Version = "v1" });
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      c.IncludeXmlComments(xmlPath);
  });
  ```

### Documentation
- Add XML comments to all public methods and classes
- Use `<summary>`, `<param>`, `<returns>` tags
- Enable XML documentation file generation in `.csproj`

## Async/Await Best Practices

- Always use `async/await` for I/O operations
- Use `CancellationToken` in async methods
- Avoid `Task.Result` or `.Wait()` (use `await` instead)
- Use `ConfigureAwait(false)` in library code (optional in ASP.NET Core)
- Return `Task` or `Task<T>`, not `void`

## Error Handling

- Use custom exception types
- Implement global exception middleware
- Return consistent error response format
- Log exceptions appropriately
- Use `ProblemDetails` for error responses

## Validation

- Use FluentValidation for command/query validation
- Register validators: `services.AddFluentValidation(...)`
- Validate in handlers or use pipeline behaviors
- Return validation errors in consistent format

