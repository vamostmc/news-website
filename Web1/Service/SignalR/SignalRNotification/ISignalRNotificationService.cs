namespace Web1.Service.SignalR.SignalRNotification
{
    public interface ISignalRNotificationService
    {
        public Task SendNotificationToUser(string userId, string message);
    }
}
