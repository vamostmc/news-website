using Web1.Models;

namespace Web1.Service.SignalR.SignalRNotification
{
    public interface ISignalRNotificationService
    {
        public Task SendNotificationToUser(string userId, string message);

        public Task SendMessageChat(string userId, MessageDto message);
    }
}
