using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Web1.Helps;
using Web1.Models;
using Web1.Service.Account;
using Web1.Service.Redis;

namespace Web1.Service.SignalR
{
    public class NotificationHub : Hub
    {
        private readonly IRedisService _redis;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;

        public NotificationHub(IRedisService redis, 
                               IHttpContextAccessor httpContextAccessor,
                               UserManager<AppUser> userManager,
                               IAccountService accountService)
        {
            _redis = redis;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        } 
        
        public async Task SendToUser(string userId, string message)
        {
            Console.WriteLine($"Nhận tin nhắn từ client: {message}");
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var user = await _accountService.GetInfo(accessToken); 
                if (user != null)
                {
                    await _redis.SetAddRedisAsync(TypeKeyRedis.CONNECTION_USER_ID_PREFIX, user.Id, Context.ConnectionId);
                }
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var user = await _accountService.GetInfo(accessToken);
                if (user != null)
                {
                    // Xoá connectionId ra khỏi Set Redis
                    await _redis.RemoveSetRedisAsync(TypeKeyRedis.CONNECTION_USER_ID_PREFIX, user.Id, Context.ConnectionId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
