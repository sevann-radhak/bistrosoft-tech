using System.Reflection;
using Bistrosoft.API.Middleware;
using Bistrosoft.Application;
using Bistrosoft.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Bistrosoft API");

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bistrosoft API",
        Version = "v1",
        Description = "API for managing online store orders"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080", "http://localhost:5000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Bistrosoft.Infrastructure.Data.ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogInformation("Applying database migrations...");
            
            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation($"Found {pendingMigrations.Count()} pending migration(s)");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("No pending migrations. Verifying database schema...");
                    try
                    {
                        await context.Database.ExecuteSqlRawAsync("SELECT TOP 1 1 FROM Products");
                        logger.LogInformation("Database schema verified");
                    }
                    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 208)
                    {
                        logger.LogWarning("Database tables do not exist despite migrations being applied. This may indicate a corrupted database state.");
                        logger.LogInformation("Attempting to reapply migrations...");
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Migrations reapplied successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during migration process");
                throw;
            }
        }
        else
        {
            logger.LogInformation("Using in-memory database");
        }
        
        await Bistrosoft.Infrastructure.Data.DbInitializer.SeedProductsAsync(context);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bistrosoft API V1");
    });
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseCors();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
