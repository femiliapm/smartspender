using Microsoft.IdentityModel.Tokens;
using SmartSpender.ViewModel;
using System.Text.Json;

namespace SmartSpender.API.Middlewares
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var path = context.Request.Path;

                // Skip authorization for certain paths
                if (path.StartsWithSegments("/api/Auth"))
                {
                    await _next(context);
                    return;
                }
                else
                {
                    // Check if the Authorization header is present
                    if (!context.Request.Headers.ContainsKey("Authorization"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        VMError result = new()
                        {
                            error = "missing_authorization",
                            error_description = "Authorization header is missing."
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                        return;
                    }
                }

                // Process the request if Authorization header exists
                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                VMError result = new()
                {
                    error = "invalid_token",
                    error_description = "The token has expired."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(result));
            }
            catch (SecurityTokenValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                VMError result = new()
                {
                    error = "invalid_token",
                    error_description = "The token is invalid."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(result));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                VMError result = new()
                {
                    error = "server_error",
                    error_description = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(result));
            }
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtValidationMiddleware>();
        }
    }
}
