---
alwaysApply: true
---

# Testing Rules - xUnit and Moq

## Testing Framework

- **Framework**: xUnit
- **Mocking**: Moq
- **Test Projects**: Separate projects for each layer
  - `Bistrosoft.Application.Tests`
  - `Bistrosoft.Infrastructure.Tests`
  - `Bistrosoft.API.Tests` (optional integration tests)

## Test Structure

### Project Organization
```
tests/
├── Bistrosoft.Application.Tests/
│   ├── Commands/
│   ├── Queries/
│   └── Validators/
└── Bistrosoft.Infrastructure.Tests/
    └── Repositories/
```

### Test Class Naming
- Pattern: `{ClassUnderTest}Tests`
- Example: `CreateCustomerCommandHandlerTests`

### Test Method Naming
- Pattern: `{MethodName}_{Scenario}_{ExpectedResult}`
- Example: `Handle_ValidCommand_ReturnsCustomerDto`
- Use descriptive names that explain what is being tested

## Unit Testing Rules

### Application Layer Tests

#### Command Handler Tests
- Test successful execution
- Test validation failures
- Test business rule violations
- Test exception scenarios
- Mock dependencies (repositories, mappers, loggers)

#### Query Handler Tests
- Test successful data retrieval
- Test not found scenarios
- Test filtering and sorting
- Mock repository dependencies

#### Validator Tests
- Test all validation rules
- Test valid inputs
- Test invalid inputs
- Test edge cases

### Infrastructure Layer Tests

#### Repository Tests
- Use In-Memory database for testing
- Test CRUD operations
- Test query methods
- Test edge cases (empty results, null handling)
- Clean up after each test

### Test Setup Pattern
```csharp
public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateCustomerCommandHandler>> _loggerMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();
        _handler = new CreateCustomerCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCustomerDto()
    {
        // Arrange
        var command = new CreateCustomerCommand { /* ... */ };
        var customer = new Customer { /* ... */ };
        var dto = new CustomerDto { /* ... */ };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);
        _mapperMock.Setup(m => m.Map<CustomerDto>(customer))
            .Returns(dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
    }
}
```

## Test Attributes

- `[Fact]`: Parameterless test method
- `[Theory]`: Parameterized test method
- `[InlineData]`: Provide test data for Theory
- `[MemberData]`: Provide test data from property/method

## Assertions

- Use xUnit assertions: `Assert.Equal`, `Assert.NotNull`, `Assert.True`, etc.
- Use `Assert.ThrowsAsync<T>` for exception testing
- Use FluentAssertions (optional) for more readable assertions

## Mocking Best Practices

### Moq Setup
- Setup return values: `mock.Setup(m => m.Method()).ReturnsAsync(value)`
- Setup exceptions: `mock.Setup(m => m.Method()).ThrowsAsync(new Exception())`
- Verify calls: `mock.Verify(m => m.Method(), Times.Once)`
- Use `It.IsAny<T>()` for flexible matching
- Use `It.Is<T>(predicate)` for specific conditions

### Mock Behavior
- Use `MockBehavior.Strict` for strict mocking (fails on unexpected calls)
- Use `MockBehavior.Loose` (default) for loose mocking

## Test Data

- Use builders or factories for test data creation
- Use meaningful test data
- Avoid magic numbers and strings
- Consider using AutoFixture (optional) for generating test data

## Test Coverage Requirements

- **Minimum Coverage**: 70% for application and infrastructure layers
- Focus on:
  - Business logic
  - Error handling
  - Edge cases
  - Validation logic

## Test Execution

- Tests should be fast (unit tests < 100ms each)
- Tests should be independent (no shared state)
- Tests should be repeatable
- Use `[Collection]` attribute if tests need to share fixtures

## Integration Tests (Optional)

- Test API endpoints end-to-end
- Use TestServer or WebApplicationFactory
- Test with real database (In-Memory or TestContainer)
- Test authentication/authorization if implemented

## Test Categories

- Use `[Trait("Category", "Unit")]` for unit tests
- Use `[Trait("Category", "Integration")]` for integration tests
- Filter tests by category when running

## Best Practices

1. **AAA Pattern**: Arrange, Act, Assert
2. **One Assertion Per Test**: Prefer multiple test methods over multiple assertions
3. **Test Isolation**: Each test should be independent
4. **Clear Test Names**: Test names should describe what is being tested
5. **Mock External Dependencies**: Don't test framework code
6. **Test Behavior, Not Implementation**: Focus on what, not how
7. **Keep Tests Simple**: Complex tests are hard to maintain
8. **Refactor Tests**: Keep test code clean and maintainable

