using InventoryManagement.API.Common;

namespace InventoryManagement.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(
               ex,
               "Unhandled exception while processing {Method} {Path}",
               context.Request.Method,
               context.Request.Path
           );
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = ex switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var message = context.Response.StatusCode ==
                         StatusCodes.Status500InternalServerError
               ? "An unexpected error occurred."
               : ex.Message;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}