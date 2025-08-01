using Microsoft.AspNetCore.Diagnostics;
using System.Security.Authentication;

namespace FlowForge.UI.Middleware
{
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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access error occurred.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.Redirect("/Error?code=403");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found error occurred.");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.Redirect("/Error?code=404");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Redirect("/Error?code=500");
            }
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
