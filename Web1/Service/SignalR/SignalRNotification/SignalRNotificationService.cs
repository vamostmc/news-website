using Microsoft.AspNetCore.SignalR;
using Web1.Helps;
using Web1.Models;
using Web1.Service.Redis;

namespace Web1.Service.SignalR.SignalRNotification
{
    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRedisService _redisService;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext,
                                          IRedisService redisService)
        {
            _hubContext = hubContext;
            _redisService = redisService;
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            try
            {
                // Gửi thông báo tới các ConnectionID mà user đã kết nối
                var connectionIds = await _redisService.GetSetMembersAsync(TypeKeyRedis.CONNECTION_USER_ID_PREFIX, userId);

                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
                }

                Console.WriteLine($"Notification sent to user {userId} successfully.");
            }
            catch (Exception ex)
            {
                
                Console.Error.WriteLine($"Error sending notification to user {userId}: {ex.Message}");
                
            }
        }

        public async Task SendMessageChat(string userId, MessageDto message)
        {
            try
            {
                var connectionIds = await _redisService.GetSetMembersAsync(
                    userId == "Manager" ? TypeKeyRedis.CONNECTION_MANAGER_ID_PREFIX : TypeKeyRedis.CONNECTION_USER_ID_PREFIX,
                    userId) ?? new List<string>();


                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveChatMessage", message);
                }

                Console.WriteLine($"Message sent to user {userId} successfully.");
            }
            catch (Exception ex)
            {

                Console.Error.WriteLine($"Error sending notification to user {userId}: {ex.Message}");

            }
        }

    }
}
