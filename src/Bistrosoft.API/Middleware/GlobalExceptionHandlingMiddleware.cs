using System.Net;
using System.Text.Json;
using Bistrosoft.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail, errors) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                notFound.Message,
                (Dictionary<string, string[]>?)null
            ),
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "Validation Error",
                validation.Message,
                validation.Errors
            ),
            BusinessRuleException businessRule => (
                HttpStatusCode.BadRequest,
                "Business Rule Violation",
                businessRule.Message,
                (Dictionary<string, string[]>?)null
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "An error occurred while processing your request",
                exception.Message,
                (Dictionary<string, string[]>?)null
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors != null && errors.Any())
        {
            problemDetails.Extensions["errors"] = errors;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        return context.Response.WriteAsync(json);
    }
}



