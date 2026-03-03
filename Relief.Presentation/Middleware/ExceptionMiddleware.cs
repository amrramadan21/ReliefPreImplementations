using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Relief.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Relief.Presentation.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger)
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
            catch (AppException ex)
            {
                _logger.LogWarning(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;

                var response = new
                {
                    success = false,
                    message = ex.Message,
                    statusCode = ex.StatusCode,
                    errors = ex is ValidationException v ? v.Errors : null
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // ⬇️ TEMPORARY: Show the real error so you can debug on AWS
                var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

                var response = new
                {
                    success = false,
                    message = isDev ? ex.Message : "Internal Server Error",
                    statusCode = 500,
                    // ADD THESE TEMPORARILY — REMOVE BEFORE PRODUCTION
                    debugMessage = ex.Message,
                    debugStackTrace = ex.StackTrace,
                    debugInner = ex.InnerException?.Message
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
