using Web1.Data;
using Web1.Models;

namespace Web1.Repository
{
    public interface INotificationRepository
    {
        public Task<List<NotificationDto>> GetNotify(int IdNotify);

        public Task<Notification> GetNotificationRepo(int id);

        public Task<NotifyBinhLuan> AddNotifyBinhLuanRepo(NotifyBinhLuan notifyBinhLuan);

        public Task<NotificationDto> AddNotifyTinTucRepo(NotificationDto notification);

        public Task RemoveNotificationRepo(int id);

        public Task UpdateNotificationRepo(int id);
    }
}
