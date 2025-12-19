using System.Net;
using System.Text.Json;
using Bistrosoft.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bistrosoft.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            NotFoundException notFound => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFound.Message,
                Details = notFound.EntityName != null && notFound.Key != null
                    ? new Dictionary<string, object>
                    {
                        { "EntityName", notFound.EntityName },
                        { "Key", notFound.Key }
                    }
                    : null
            },
            ValidationException validation => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = validation.Message,
                Errors = validation.Errors
            },
            BusinessRuleException businessRule => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = businessRule.Message
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An error occurred while processing your request."
            }
        };

        response.StatusCode = errorResponse.StatusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
        return response.WriteAsync(json);
    }

    private class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}



