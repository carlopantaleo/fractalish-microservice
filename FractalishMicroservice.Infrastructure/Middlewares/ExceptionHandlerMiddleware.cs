using System.Net;
using FractalishMicroservice.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace FractalishMicroservice.Infrastructure.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public ExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
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
