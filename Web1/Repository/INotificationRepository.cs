using Web1.Data;
using Web1.Models;

namespace Web1.Repository
{
    public interface INotificationRepository
    {
        public Task<Notification> CreatForgotPasswordNotify(string UserId);

        public Task<Notification> CreatCommentNotify(string UserId, string UserNameReply, string NameTinTuc);

        public Task<Success> DeleteNotify(long id);

        public Task<List<NotificationDto>> GetNotifyUser (string userId);

        public Task<int> TotalNotify(string userId); 

        public Task<Success> UpdateReadStatusId(long id, bool statusRead);

        public Task<Success> UpdateAllRead(string userId, bool statusRead);

    }
}
