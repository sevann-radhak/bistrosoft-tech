---
alwaysApply: true
---

# Clean Architecture Rules

## Architecture Principles

### Layer Structure
The solution MUST follow Clean Architecture with the following layers:

1. **Domain Layer** (Core)
   - Entities (Customer, Order, Product, OrderItem)
   - Enums (OrderStatus)
   - Domain interfaces (IRepository<T>)
   - Domain exceptions
   - NO dependencies on other layers

2. **Application Layer**
   - Commands and Queries (CQRS)
   - Command/Query Handlers (MediatR)
   - DTOs/ViewModels
   - Validators (FluentValidation)
   - Application interfaces
   - Dependencies: Domain only

3. **Infrastructure Layer**
   - Repository implementations
   - Entity Framework configurations
   - DbContext
   - External service implementations
   - Dependencies: Domain, Application

4. **API Layer (Presentation)**
   - Controllers
   - Middleware
   - API configuration
   - Dependencies: Application, Infrastructure

5. **Tests Layer**
   - Unit tests
   - Integration tests
   - Test utilities
   - Dependencies: All layers

## Project Structure

```
src/
├── Bistrosoft.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Interfaces/
│   └── Exceptions/
├── Bistrosoft.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   ├── Validators/
│   └── Mappings/
├── Bistrosoft.Infrastructure/
│   ├── Data/
│   │   ├── Repositories/
│   │   ├── Configurations/
│   │   └── ApplicationDbContext.cs
│   ├── Services/
│   └── Middleware/
├── Bistrosoft.API/
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
└── tests/
    ├── Bistrosoft.Application.Tests/
    └── Bistrosoft.Infrastructure.Tests/
```

## Dependency Rules

1. **Dependency Direction**: Dependencies MUST point inward (toward Domain)
   - Domain: No dependencies
   - Application: Depends on Domain
   - Infrastructure: Depends on Domain and Application
   - API: Depends on Application and Infrastructure

2. **Interface Segregation**: Define interfaces in the layer that uses them, implement in outer layers.

3. **Dependency Inversion**: Depend on abstractions (interfaces), not concrete implementations.

## Naming Conventions

- **Entities**: Singular nouns (Customer, Order, Product)
- **Repositories**: I{Entity}Repository (ICustomerRepository)
- **Commands**: {Action}{Entity}Command (CreateCustomerCommand)
- **Queries**: Get{Entity}Query (GetCustomerByIdQuery)
- **Handlers**: {Command/Query}Handler (CreateCustomerCommandHandler)
- **DTOs**: {Entity}Dto, Create{Entity}Dto, Update{Entity}Dto
- **Controllers**: {Entity}Controller (CustomersController)

## SOLID Principles

1. **Single Responsibility**: Each class has one reason to change
2. **Open/Closed**: Open for extension, closed for modification
3. **Liskov Substitution**: Derived classes must be substitutable for base classes
4. **Interface Segregation**: Many specific interfaces over one general interface
5. **Dependency Inversion**: Depend on abstractions, not concretions

## Separation of Concerns

- **Business Logic**: Application layer (handlers)
- **Data Access**: Infrastructure layer (repositories)
- **API Concerns**: API layer (controllers)
- **Domain Rules**: Domain layer (entities, value objects)

