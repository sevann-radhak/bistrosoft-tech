---
alwaysApply: true
---

# Optional Features - Extras Implementation

## JWT Authentication

### Setup
- Install packages:
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `System.IdentityModel.Tokens.Jwt`

### Configuration
- Add JWT settings to `appsettings.json`
- Configure authentication in `Program.cs`:
  ```csharp
  builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = builder.Configuration["Jwt:Issuer"],
              ValidAudience = builder.Configuration["Jwt:Audience"],
              IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
          };
      });
  ```

### Implementation
- Create login endpoint (POST /api/auth/login)
- Generate JWT token on successful authentication
- Protect endpoints with `[Authorize]` attribute
- Add authorization policies if role-based access is needed

### Frontend Integration
- Store token in localStorage or httpOnly cookie
- Add token to Authorization header: `Bearer {token}`
- Handle token expiration and refresh

## Global Exception Handling

### Middleware Implementation
```csharp
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            BusinessRuleException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception.GetType().Name,
            Detail = exception.Message
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
```

### Registration
```csharp
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
```

## Logging Implementation

### Serilog Configuration
- Install `Serilog.AspNetCore`
- Configure in `Program.cs`:
  ```csharp
  Log.Logger = new LoggerConfiguration()
      .WriteTo.Console()
      .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
      .CreateLogger();

  builder.Host.UseSerilog();
  ```

### Structured Logging
- Use structured logging with parameters
- Log at appropriate levels
- Include correlation IDs for request tracking
- Don't log sensitive information (passwords, tokens)

## Caching Implementation

### In-Memory Caching
- Use `IMemoryCache` from `Microsoft.Extensions.Caching.Memory`
- Register: `services.AddMemoryCache()`
- Cache frequently accessed data (products, customer info)
- Set appropriate expiration times
- Invalidate cache on updates

### Cache Service Pattern
```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiration);
    Task RemoveAsync(string key);
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public async Task<T?> GetAsync<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    // Implementation...
}
```

### Usage in Handlers
- Check cache before database query
- Store results in cache after query
- Invalidate cache on data updates

## Mapping Libraries

### AutoMapper
- Install `AutoMapper` and `AutoMapper.Extensions.Microsoft.DependencyInjection`
- Create mapping profiles:
  ```csharp
  public class CustomerProfile : Profile
  {
      public CustomerProfile()
      {
          CreateMap<Customer, CustomerDto>();
          CreateMap<CreateCustomerCommand, Customer>();
      }
  }
  ```
- Register: `services.AddAutoMapper(typeof(ApplicationAssembly))`

### Mapster (Alternative)
- Install `Mapster` and `Mapster.DependencyInjection`
- Configure mappings or use conventions
- Generally faster than AutoMapper

## Docker Configuration

### API Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Bistrosoft.API/Bistrosoft.API.csproj", "Bistrosoft.API/"]
COPY ["Bistrosoft.Application/Bistrosoft.Application.csproj", "Bistrosoft.Application/"]
COPY ["Bistrosoft.Domain/Bistrosoft.Domain.csproj", "Bistrosoft.Domain/"]
COPY ["Bistrosoft.Infrastructure/Bistrosoft.Infrastructure.csproj", "Bistrosoft.Infrastructure/"]
RUN dotnet restore "Bistrosoft.API/Bistrosoft.API.csproj"
COPY . .
WORKDIR "/src/Bistrosoft.API"
RUN dotnet build "Bistrosoft.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bistrosoft.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bistrosoft.API.dll"]
```

### Frontend Dockerfile
```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Docker Compose
```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Bistrosoft.API/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=Bistrosoft;User Id=sa;Password=YourPassword123;
    depends_on:
      - db

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - api

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

volumes:
  sqlserver_data:
```

## SQL Server Configuration (Alternative to In-Memory)

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Bistrosoft;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### EF Core Configuration
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Bistrosoft.Infrastructure")));
```

### Migrations
- Create migrations: `dotnet ef migrations add InitialCreate`
- Apply migrations: `dotnet ef database update`
- Or use `Database.Migrate()` in `Program.cs` for automatic migration

## Implementation Priority

1. **High Priority** (Core Requirements):
   - Global Exception Handling
   - Logging
   - Mapping

2. **Medium Priority** (Recommended):
   - Caching
   - SQL Server (if not using In-Memory)

3. **Low Priority** (Nice to Have):
   - JWT Authentication
   - Docker Configuration

## Notes

- Implement extras incrementally
- Test each feature after implementation
- Document configuration in README
- Consider performance implications of caching
- Security considerations for JWT (token expiration, refresh tokens)

