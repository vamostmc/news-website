using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Web1.Exceptions;
using Web1.Helps;
using Web1.Service.Account;
using Web1.Service.Redis;

namespace Web1.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IRedisService _redisService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAccountService _account;

        public ExceptionHandlingMiddleware(
                RequestDelegate next, 
                ILogger<ExceptionHandlingMiddleware> logger,
                IRedisService redisService,
                IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _redisService = redisService;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //Xử lý người dùng bị chặn request trong blacklist
                string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

                        var userInfo = await _accountService.GetInfo(token);
                        if (userInfo != null)
                        {
                            string userId = userInfo.Id;
                            string typeKey = TypeKeyRedis.BLACKLIST_PREFIX;

                            bool isBlacklisted = await _redisService.IsCheckListRedis(typeKey, userId);
                            if (isBlacklisted)
                            {
                                _logger.LogWarning("Tài khoản bị khóa: {UserId}", userId);
                                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                await context.Response.WriteAsJsonAsync(new ProblemDetails
                                {
                                    Status = StatusCodes.Status403Forbidden,
                                    Title = "Tài khoản bị khóa",
                                    Detail = "Tài khoản của bạn đã bị khóa bởi Admin.",
                                    Instance = context.Request.Path
                                });
                                return;
                            }
                        }
                    }
                }

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
                    Detail = ex.Message,
                    Instance = context.Request.Path
                });
            }
        }

        
    }
}
