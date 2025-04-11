using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Web1.Data;
using Web1.Helps;
using Web1.Models;
using Web1.Service.Account;
using Web1.Service.ChatBox;
using Web1.Service.Redis;

namespace Web1.Service.SignalR
{
    public class NotificationHub : Hub
    {
        private readonly IRedisService _redis;
        private readonly IChatService _chatService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;

        public NotificationHub(IRedisService redis, 
                               IHttpContextAccessor httpContextAccessor,
                               UserManager<AppUser> userManager,
                               IAccountService accountService,
                               IChatService chatService)
        {
            _redis = redis;
            _chatService = chatService;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        } 
        
        public async Task SendMessagefromClient(MessageDto message)
        {
            Console.WriteLine($"Nhận tin nhắn từ client: {message}");
            await _chatService.SendMessageAsync(message);
            //await Clients.User(userId).SendAsync("ReceiveChatMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var user = await _accountService.GetInfo(accessToken);
                var checkManager = await _accountService.CheckRoleUser(accessToken);
                if (user != null)
                {
                   if(checkManager == true)
                   {
                        await _redis.SetAddRedisAsync(TypeKeyRedis.CONNECTION_MANAGER_ID_PREFIX, "Manager", Context.ConnectionId);
                   }
                   else
                    {
                        await _redis.SetAddRedisAsync(TypeKeyRedis.CONNECTION_USER_ID_PREFIX, user.Id, Context.ConnectionId);
                    }
                   
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
                var checkManager = await _accountService.CheckRoleUser(accessToken);
                if (user != null)
                {
                    if (checkManager == true)
                    {
                        await _redis.RemoveSetRedisAsync(TypeKeyRedis.CONNECTION_MANAGER_ID_PREFIX, "Manager", Context.ConnectionId);
                    }
                    // Xoá connectionId ra khỏi Set Redis
                    else {
                        await _redis.RemoveSetRedisAsync(TypeKeyRedis.CONNECTION_USER_ID_PREFIX, user.Id, Context.ConnectionId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
