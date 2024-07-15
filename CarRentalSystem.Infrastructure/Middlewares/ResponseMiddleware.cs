using System.Net;
using System.Text.Json;
using CarRentalSystem.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Infrastructure.Middlewares;

public class ResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Catch exceptions and send response with 500 status code
        try
        {
            await _next(context);
        }
        catch (DomainException e)
        {
            await HandleExceptionAsync(context, e, e.StatusCode);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode = 500)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Message = exception.Message,
            Path = context.Request.Path.Value
        };
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}