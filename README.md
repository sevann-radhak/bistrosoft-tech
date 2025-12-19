# Bistrosoft Store - Online Order Management System

A full-stack web application for managing online store orders, built with .NET 8 and Vue.js 3, following Clean Architecture principles.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Project Structure](#project-structure)
- [Development Guidelines](#development-guidelines)

## Overview

Bistrosoft Store is a comprehensive order management system that allows you to:
- Manage customers and their information
- Create and track orders
- Manage product inventory
- Update order statuses through a defined workflow

The application follows Clean Architecture principles with clear separation of concerns, making it maintainable, testable, and scalable.

## Features

### Backend (API)
- RESTful API with comprehensive error handling
- CQRS pattern implementation using MediatR
- Repository pattern for data access
- FluentValidation for input validation
- Entity Framework Core with SQL Server or In-Memory database
- Swagger/OpenAPI documentation
- Global exception handling
- Logging with Serilog

### Frontend
- Modern Vue.js 3 application with TypeScript
- Pinia for state management
- Vue Router for navigation
- Responsive and user-friendly UI
- Comprehensive error handling with descriptive messages
- Real-time form validation

### Testing
- Unit tests for repositories
- Unit tests for application handlers
- Unit tests for validators
- Integration tests for API endpoints
- Test coverage using xUnit and Moq

## Architecture

The project follows **Clean Architecture** with the following layers:

```
┌─────────────────────────────────────┐
│         Presentation Layer         │
│         (Bistrosoft.API)            │
│      Controllers, Middleware       │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│       Application Layer             │
│    (Bistrosoft.Application)        │
│  Commands, Queries, Handlers, DTOs  │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│      Infrastructure Layer           │
│   (Bistrosoft.Infrastructure)      │
│  Repositories, EF Core, Services    │
└─────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────┐
│         Domain Layer                │
│      (Bistrosoft.Domain)            │
│   Entities, Interfaces, Exceptions  │
└─────────────────────────────────────┘
```

### Key Patterns
- **CQRS**: Commands and Queries separation using MediatR
- **Repository Pattern**: Abstraction of data access
- **Dependency Injection**: Loose coupling between layers
- **SOLID Principles**: Applied throughout the codebase

## Technology Stack

### Backend
- **.NET 8.0**
- **Entity Framework Core 8.0**
- **MediatR** (CQRS implementation)
- **FluentValidation**
- **Serilog** (Logging)
- **Swagger/OpenAPI**
- **xUnit** & **Moq** (Testing)

### Frontend
- **Vue.js 3** with Composition API
- **TypeScript**
- **Pinia** (State management)
- **Vue Router**
- **Axios** (HTTP client)
- **Vite** (Build tool)

### Database
- **SQL Server** (Production)
- **In-Memory Database** (Development/Testing)

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [npm](https://www.npmjs.com/) or [yarn](https://yarnpkg.com/)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Optional - can use In-Memory database)
- [Git](https://git-scm.com/)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd bistrosoft-tech
```

### 2. Backend Setup

#### Option A: Using In-Memory Database (Recommended for Quick Start)

1. Navigate to the API project:
```bash
cd src/Bistrosoft.API
```

2. Run the application:
```bash
dotnet run
```

The API will start on `http://localhost:5000` (or the port configured in `launchSettings.json`).

#### Option B: Using SQL Server

1. Update the connection string in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Bistrosoft;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

2. Apply database migrations:
```bash
cd src/Bistrosoft.Infrastructure
dotnet ef database update --startup-project ../Bistrosoft.API/Bistrosoft.API.csproj
```

3. Run the application:
```bash
cd ../Bistrosoft.API
dotnet run
```

### 3. Frontend Setup

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`.

### 4. Access the Application

- **Frontend**: http://localhost:5173
- **API Swagger**: http://localhost:5000/swagger
- **API Base URL**: http://localhost:5000/api

## API Documentation

### Base URL
```
http://localhost:5000/api
```

### Endpoints

#### Customers

##### Get All Customers
```http
GET /api/customers
```

**Response:** `200 OK`
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890",
    "orders": []
  }
]
```

##### Get Customer by ID
```http
GET /api/customers/{id}
```

**Response:** `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "orders": []
}
```

##### Create Customer
```http
POST /api/customers
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890"
}
```

**Response:** `201 Created`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid input or email already exists

##### Get Customer Orders
```http
GET /api/customers/{id}/orders
```

**Response:** `200 OK`
```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "totalAmount": 25.98,
    "createdAt": "2024-01-15T10:30:00Z",
    "status": "Pending",
    "orderItems": []
  }
]
```

#### Orders

##### Create Order
```http
POST /api/orders
Content-Type: application/json

{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "items": [
    {
      "productId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "quantity": 2
    }
  ]
}
```

**Response:** `201 Created`
```json
{
  "id": "8d9e6679-7425-40de-944b-e07fc1f90ae8",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalAmount": 25.98,
  "createdAt": "2024-01-15T10:30:00Z",
  "status": "Pending",
  "orderItems": [
    {
      "id": "9e0f7780-8536-51ef-055c-f18gd2g01bf9",
      "orderId": "8d9e6679-7425-40de-944b-e07fc1f90ae8",
      "productId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "quantity": 2,
      "unitPrice": 12.99
    }
  ]
}
```

**Error Responses:**
- `400 Bad Request` - Invalid input, insufficient stock, or empty order
- `404 Not Found` - Customer or product not found

##### Get Order by ID
```http
GET /api/orders/{id}
```

**Response:** `200 OK`
```json
{
  "id": "8d9e6679-7425-40de-944b-e07fc1f90ae8",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalAmount": 25.98,
  "createdAt": "2024-01-15T10:30:00Z",
  "status": "Pending",
  "orderItems": []
}
```

##### Update Order Status
```http
PUT /api/orders/{id}/status
Content-Type: application/json

{
  "orderId": "8d9e6679-7425-40de-944b-e07fc1f90ae8",
  "status": "Paid"
}
```

**Valid Status Transitions:**
- `Pending` → `Paid` or `Cancelled`
- `Paid` → `Shipped` or `Cancelled`
- `Shipped` → `Delivered`
- `Delivered` → (no transitions allowed)
- `Cancelled` → (no transitions allowed)

**Response:** `200 OK`
```json
{
  "id": "8d9e6679-7425-40de-944b-e07fc1f90ae8",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalAmount": 25.98,
  "createdAt": "2024-01-15T10:30:00Z",
  "status": "Paid",
  "orderItems": []
}
```

**Error Responses:**
- `400 Bad Request` - Invalid status transition
- `404 Not Found` - Order not found

#### Products

##### Get All Products
```http
GET /api/products
```

**Response:** `200 OK`
```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "name": "Margherita Pizza",
    "price": 12.99,
    "stockQuantity": 50
  }
]
```

##### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "Margherita Pizza",
  "price": 12.99,
  "stockQuantity": 50
}
```

**Response:** `201 Created`
```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "name": "Margherita Pizza",
  "price": 12.99,
  "stockQuantity": 50
}
```

### Error Responses

All error responses follow the Problem Details format (RFC 7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Business Rule Violation",
  "status": 400,
  "detail": "Customer with email 'john.doe@example.com' already exists.",
  "instance": "/api/customers"
}
```

## Testing

### Running Tests

#### Backend Tests

Run all tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test tests/Bistrosoft.Application.Tests/Bistrosoft.Application.Tests.csproj
dotnet test tests/Bistrosoft.Infrastructure.Tests/Bistrosoft.Infrastructure.Tests.csproj
dotnet test tests/Bistrosoft.API.Tests/Bistrosoft.API.Tests.csproj
```

#### Test Coverage

The test projects include:
- **Repository Tests**: Test data access layer with in-memory database
- **Handler Tests**: Test command and query handlers with mocked dependencies
- **Validator Tests**: Test FluentValidation rules
- **Integration Tests**: Test API endpoints end-to-end

### Test Structure

```
tests/
├── Bistrosoft.Application.Tests/
│   ├── Handlers/
│   └── Validators/
├── Bistrosoft.Infrastructure.Tests/
│   └── Repositories/
└── Bistrosoft.API.Tests/
    └── Controllers/
```

## Project Structure

```
bistrosoft-tech/
├── src/
│   ├── Bistrosoft.Domain/           # Domain entities, interfaces, exceptions
│   ├── Bistrosoft.Application/       # Commands, queries, handlers, DTOs
│   ├── Bistrosoft.Infrastructure/    # Repositories, EF Core, services
│   └── Bistrosoft.API/               # Controllers, middleware, configuration
├── frontend/
│   ├── src/
│   │   ├── components/               # Vue components
│   │   ├── views/                    # Page views
│   │   ├── stores/                  # Pinia stores
│   │   ├── services/                # API services
│   │   └── router/                  # Vue Router configuration
│   └── package.json
├── tests/
│   ├── Bistrosoft.Application.Tests/
│   ├── Bistrosoft.Infrastructure.Tests/
│   └── Bistrosoft.API.Tests/
└── README.md
```

## Development Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful names for variables, classes, and methods
- Keep functions small and focused (Single Responsibility Principle)
- Prefer composition over inheritance

### Commit Guidelines
- Make small, granular commits
- Use descriptive commit messages
- Each commit should represent a single logical change

### Testing
- Write tests for business logic
- Aim for high test coverage
- Use descriptive test names following the pattern: `MethodName_Scenario_ExpectedResult`

### Error Handling
- Use domain exceptions for business rule violations
- Provide clear, user-friendly error messages
- Log errors appropriately

## Configuration

### Backend Configuration

#### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""  // Empty for in-memory database
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

#### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Bistrosoft;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Frontend Configuration

The frontend automatically detects the API URL:
- Development: `http://localhost:5000/api`
- Production: `/api` (relative path)

You can override this by setting the `VITE_API_BASE_URL` environment variable.

## Docker Support

The project includes Docker configuration files:
- `docker-compose.yml` - Orchestrates API and frontend containers
- `Dockerfile` (in frontend/) - Frontend container configuration

To run with Docker:
```bash
docker-compose up
```

## Additional Resources

- [Clean Architecture Documentation](.cursor/rules/project-rules/ARCHITECTURE.md)
- [Development Plan](.cursor/rules/project-rules/DEVELOPMENT_PLAN.md)
- [Testing Guidelines](.cursor/rules/project-rules/TESTING.md)
- [Backend Technical Details](.cursor/rules/project-rules/BACKEND_TECHNICAL.md)
- [Frontend Technical Details](.cursor/rules/project-rules/FRONTEND_TECHNICAL.md)

## Contributing

1. Follow the existing code style and patterns
2. Write tests for new features
3. Update documentation as needed
4. Make small, focused commits

## License

This project is part of a technical assessment.

---

**Built with .NET 8 and Vue.js 3**
