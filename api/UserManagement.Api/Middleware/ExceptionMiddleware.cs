using System.Net;
using System.Text.Json;
using FluentValidation;
using Serilog;
using UserManagement.Application.Models;

namespace UserManagement.Api.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(IHostEnvironment env)
    {
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        // 🔥 Log to Serilog (file + console)
        Log.Error(ex, "Unhandled exception caught by middleware");

        ctx.Response.ContentType = "application/json";

        var response = new ErrorResponse();
        int status = (int)HttpStatusCode.InternalServerError;

        switch (ex)
        {
            case ValidationException fv:
                status = (int)HttpStatusCode.BadRequest;
                response.Message = "Validation failed.";
                response.Errors = fv.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray());
                break;

            case UnauthorizedAccessException:
                status = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized";
                break;

            case KeyNotFoundException:
                status = (int)HttpStatusCode.NotFound;
                response.Message = ex.Message;
                break;

            case InvalidOperationException:
                status = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
                break;

            default:
                status = (int)HttpStatusCode.InternalServerError;
                response.Message = "A server error occurred.";
                if (_env.IsDevelopment())
                    response.Details = ex.Message;
                break;
        }

        ctx.Response.StatusCode = status;

        var opts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await ctx.Response.WriteAsync(JsonSerializer.Serialize(response, opts));
    }
}
