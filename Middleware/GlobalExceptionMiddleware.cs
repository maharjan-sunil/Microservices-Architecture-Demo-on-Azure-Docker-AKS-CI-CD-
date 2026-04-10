using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // move to next middleware
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = ex switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = isDevelopment ? ex.Message : "An unexpected error occurred.",
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response);
      //  if (!isDevelopment)
            return context.Response.WriteAsync(json);
        //context.Response.Redirect("/Error");
        //return Task.CompletedTask;
    }
}