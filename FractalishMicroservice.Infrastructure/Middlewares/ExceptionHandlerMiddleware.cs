using System.Net;
using FractalishMicroservice.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FractalishMicroservice.Infrastructure.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next,
                                      IHostEnvironment env,
                                      ILogger<ExceptionHandlerMiddleware>? logger = null)
    {
        _next = next;
        _env = env;
        _logger = logger ?? NullLogger<ExceptionHandlerMiddleware>.Instance;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Exception caught while processing request");

        context.Response.StatusCode = ex switch
        {
            ServiceInstanceException serviceInstanceException => (int)serviceInstanceException.StatusCode,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = ex switch
            {
                ServiceInstanceException serviceInstanceException => serviceInstanceException.Message,
                _ => "An unexpected error occurred."
            },
            Detail = GetExceptionDetails(ex)
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private string? GetExceptionDetails(Exception ex)
    {
        if (!_env.IsDevelopment())
        {
            return null;
        }

        return ex switch
        {
            ServiceInstanceException serviceInstanceException => serviceInstanceException.InnerException?.Message,
            _ => ex.Message
        };
    }
}
