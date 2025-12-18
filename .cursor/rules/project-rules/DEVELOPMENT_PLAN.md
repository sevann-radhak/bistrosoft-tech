---
alwaysApply: true
---

# Development Plan - Step by Step Methodology

## Overview
This document defines the step-by-step development plan and methodology for the Bistrosoft Tech assessment project. Each step must be marked as `[PENDING]`, `[IN_PROGRESS]`, or `[COMPLETED]` before moving to the next step.

## Development Phases

### Phase 1: Project Setup and Infrastructure
- [COMPLETED] **Step 1.1**: Create solution structure with Clean Architecture layers
  - Create .NET solution file
  - Create projects: Domain, Application, Infrastructure, API, Tests
  - Configure project references
  - Status: [COMPLETED]

- [COMPLETED] **Step 1.2**: Setup Entity Framework Core
  - Install EF Core packages
  - Configure DbContext
  - Setup In-Memory database provider
  - Status: [COMPLETED]

- [PENDING] **Step 1.3**: Setup MediatR
  - Install MediatR packages
  - Configure MediatR in DI container
  - Status: [PENDING]

- [PENDING] **Step 1.4**: Setup Swagger/OpenAPI
  - Configure Swagger with XML documentation
  - Setup API versioning if needed
  - Status: [PENDING]

- [PENDING] **Step 1.5**: Setup Dependency Injection
  - Configure DI container
  - Register services, repositories, handlers
  - Status: [PENDING]

- [PENDING] **Step 1.6**: Setup Logging
  - Install logging packages (Serilog recommended)
  - Configure logging infrastructure
  - Status: [PENDING]

- [PENDING] **Step 1.7**: Setup Mapping
  - Install AutoMapper or Mapster
  - Configure mapping profiles
  - Status: [PENDING]

- [PENDING] **Step 1.8**: Setup Vue.js 3 Frontend
  - Initialize Vue.js 3 project
  - Configure build tools (Vite recommended)
  - Setup project structure
  - Status: [PENDING]

### Phase 2: Domain Layer
- [PENDING] **Step 2.1**: Create Domain Entities
  - Customer entity with properties
  - Order entity with properties
  - Product entity with properties
  - OrderItem entity with properties
  - Status: [PENDING]

- [PENDING] **Step 2.2**: Create Domain Enums
  - OrderStatus enum (Pending, Paid, Shipped, Delivered, Cancelled)
  - Status: [PENDING]

- [PENDING] **Step 2.3**: Create Domain Value Objects (if applicable)
  - Email value object for validation
  - Status: [PENDING]

- [PENDING] **Step 2.4**: Create Domain Interfaces
  - Repository interfaces (ICustomerRepository, IOrderRepository, IProductRepository)
  - Status: [PENDING]

### Phase 3: Application Layer (CQRS with MediatR)
- [PENDING] **Step 3.1**: Create Commands
  - CreateCustomerCommand
  - CreateOrderCommand
  - UpdateOrderStatusCommand
  - Status: [PENDING]

- [PENDING] **Step 3.2**: Create Queries
  - GetCustomerByIdQuery
  - GetCustomerOrdersQuery
  - Status: [PENDING]

- [PENDING] **Step 3.3**: Create Command Handlers
  - CreateCustomerCommandHandler
  - CreateOrderCommandHandler (with stock validation)
  - UpdateOrderStatusCommandHandler (with state transition validation)
  - Status: [PENDING]

- [PENDING] **Step 3.4**: Create Query Handlers
  - GetCustomerByIdQueryHandler
  - GetCustomerOrdersQueryHandler
  - Status: [PENDING]

- [PENDING] **Step 3.5**: Create DTOs/ViewModels
  - CustomerDto, CreateCustomerDto
  - OrderDto, CreateOrderDto, UpdateOrderStatusDto
  - ProductDto
  - OrderItemDto
  - Status: [PENDING]

- [PENDING] **Step 3.6**: Create Validators (FluentValidation recommended)
  - CreateCustomerCommandValidator
  - CreateOrderCommandValidator
  - UpdateOrderStatusCommandValidator
  - Status: [PENDING]

### Phase 4: Infrastructure Layer
- [PENDING] **Step 4.1**: Implement Repositories
  - CustomerRepository implementation
  - OrderRepository implementation
  - ProductRepository implementation
  - Status: [PENDING]

- [PENDING] **Step 4.2**: Configure Entity Framework
  - Create entity configurations (Fluent API)
  - Configure relationships
  - Configure indexes (e.g., Email unique)
  - Status: [PENDING]

- [PENDING] **Step 4.3**: Create DbContext
  - Implement ApplicationDbContext
  - Configure DbSets
  - Setup database initialization (seed data if needed)
  - Status: [PENDING]

- [PENDING] **Step 4.4**: Implement Exception Handling Middleware
  - Global exception handler
  - Custom exception types
  - Error response formatting
  - Status: [PENDING]

- [PENDING] **Step 4.5**: Implement Caching (optional)
  - Setup caching strategy
  - Implement cache service
  - Status: [PENDING]

### Phase 5: API Layer
- [PENDING] **Step 5.1**: Create Controllers
  - CustomersController (POST /api/customers, GET /api/customers/{id})
  - OrdersController (POST /api/orders, PUT /api/orders/{id}/status)
  - CustomersController extension (GET /api/customers/{id}/orders)
  - Status: [PENDING]

- [PENDING] **Step 5.2**: Configure API Middleware
  - Exception handling middleware
  - CORS configuration
  - Status: [PENDING]

- [PENDING] **Step 5.3**: Add API Documentation
  - XML comments for all endpoints
  - Swagger annotations
  - Status: [PENDING]

- [PENDING] **Step 5.4**: Implement JWT Authentication (optional)
  - Setup JWT configuration
  - Create authentication endpoints
  - Protect endpoints with [Authorize]
  - Status: [PENDING]

### Phase 6: Frontend (Vue.js 3)
- [COMPLETED] **Step 6.1**: Setup API Client
  - Create axios/fetch service
  - Configure base URL
  - Setup interceptors
  - Status: [COMPLETED]

- [PENDING] **Step 6.2**: Create Views/Pages
  - Customer creation form
  - Order creation form
  - Customer orders list
  - Status: [PENDING]

- [PENDING] **Step 6.3**: Create Components
  - Reusable form components
  - Order status display component
  - Product selection component
  - Status: [PENDING]

- [PENDING] **Step 6.4**: Implement State Management (if needed)
  - Pinia store setup
  - State for customers, orders, products
  - Status: [PENDING]

- [PENDING] **Step 6.5**: Add Routing
  - Configure Vue Router
  - Define routes
  - Status: [PENDING]

### Phase 7: Testing
- [PENDING] **Step 7.1**: Setup Test Projects
  - Unit test project structure
  - Configure test dependencies
  - Status: [PENDING]

- [PENDING] **Step 7.2**: Repository Tests
  - CustomerRepository tests
  - OrderRepository tests
  - ProductRepository tests
  - Status: [PENDING]

- [PENDING] **Step 7.3**: Application Layer Tests
  - Command handler tests
  - Query handler tests
  - Validation tests
  - Status: [PENDING]

- [PENDING] **Step 7.4**: Integration Tests (optional)
  - API endpoint tests
  - End-to-end scenarios
  - Status: [PENDING]

### Phase 8: Optional Features
- [PENDING] **Step 8.1**: Docker Configuration
  - Create Dockerfile for API
  - Create Dockerfile for Frontend
  - Create docker-compose.yml
  - Status: [PENDING]

- [PENDING] **Step 8.2**: SQL Server Configuration (if not using In-Memory)
  - Connection string configuration
  - Migration setup
  - Status: [PENDING]

### Phase 9: Documentation and Finalization
- [PENDING] **Step 9.1**: Code Review and Refactoring
  - Review code quality
  - Apply SOLID principles
  - Refactor if needed
  - Status: [PENDING]

- [PENDING] **Step 9.2**: Update Swagger Documentation
  - Ensure all endpoints are documented
  - Add examples
  - Status: [PENDING]

- [PENDING] **Step 9.3**: Create README
  - Project overview
  - Setup instructions
  - API documentation
  - Status: [PENDING]

## Methodology Rules

1. **Status Tracking**: Before starting any step, mark it as `[IN_PROGRESS]`. Upon completion, mark it as `[COMPLETED]`.

2. **Dependencies**: Do not start a step if its dependencies are not completed.

3. **Code Quality**: After each step, ensure:
   - Code compiles without errors
   - No linter warnings
   - Follows established patterns

4. **Testing**: Write tests immediately after implementing features, not at the end.

5. **Commits**: Make small, granular commits after each logical unit of work.

6. **Review**: Before marking a step as completed, verify it meets all requirements from the original specification.

