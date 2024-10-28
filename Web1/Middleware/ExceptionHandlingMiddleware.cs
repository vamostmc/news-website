using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web1.Exceptions;

namespace Web1.Middleware
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
            //Bắt lỗi từ Repository
            catch (RepositoryException ex) 
            {
                _logger.LogError(ex, "Lỗi từ Repository: {Message}", ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Lỗi từ Repository",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định: {Message}", ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Lỗi hệ thống",
                    Detail = "Đã xảy ra lỗi không mong muốn",
                    Instance = context.Request.Path
                });
            }
        }
    }
}
