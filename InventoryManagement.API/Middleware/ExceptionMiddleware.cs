using System.Text.Json;
using Azure.Core;
using InventoryManagement.API.Common;

namespace InventoryManagement.API.Middleware;

public class ExceptionMiddleware 
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next=next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            context.Response.ContentType="application/json";

            context.Response.StatusCode = ex switch
            {
                UnauthorizedAccessException=>StatusCodes.Status401Unauthorized,
                KeyNotFoundException=>StatusCodes.Status404NotFound,
                ArgumentException=>StatusCodes.Status400BadRequest,
                _=>StatusCodes.Status500InternalServerError
            };

            var response=new ApiResponse<object>
            {
                Success=false,
                Message=ex.Message,
                Data=null
            };
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}