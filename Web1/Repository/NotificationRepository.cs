using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web1.Data;
using Web1.DataNew;
using Web1.Helps;
using Web1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web1.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TinTucDbContext _noti;
        private readonly UserManager<AppUser> _userManager;

        public NotificationRepository(TinTucDbContext noti, 
                                      UserManager<AppUser> userManager) 
        { 
            _noti = noti;
            _userManager = userManager;
        }

        public async Task<int> TotalNotify(string userId)
        {
            var count = await _noti.Notifications.CountAsync(n => n.TargetId == userId && n.IsRead == false);
            return count;
        }

        public async Task<Notification> CreatForgotPasswordNotify(string UserId)
        {
            var data = new Notification
            {
                IsRead = false,
                CreatedAt = DateTime.Now,
                TypeId = TypeNotification.TYPE_NOTIFICATION_FORGOTPASSWORD,
                IsSystem = false,
                Message = "Cảnh báo: Bạn vừa yêu cầu thầy đổi mật khẩu",
                SenderId = null,
                TargetId = UserId,
            };

            await _noti.AddAsync(data);
            await _noti.SaveChangesAsync();
            return data;
        }

        public async Task<Notification> CreatCommentNotify(string UserId, string UserNameReply, string NameTinTuc)
        {
            var data = new Notification
            {
                IsRead = false,
                CreatedAt = DateTime.Now,
                TypeId = TypeNotification.TYPE_NOTIFICATION_COMMENT,
                IsSystem = false,
                Message = "Người dùng "+ UserNameReply + " vừa trả lời bình luận của bạn ở bài viết: " + NameTinTuc,
                SenderId = null,
                TargetId = UserId,
            };

            await _noti.AddAsync(data);
            await _noti.SaveChangesAsync();
            return data;
        }

        public async Task<Success> DeleteNotify(long id)
        {
            var data = await _noti.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) 
            {
                return new Success { success = false, message = "Lỗi không tìm thấy id thông báo" };
            }
            _noti.Notifications.Remove(data);
            await _noti.SaveChangesAsync();
            return new Success { success = true };
        }

        public async Task<List<NotificationDto>> GetNotifyUser(string userId)
        {
            var data = await _noti.Notifications.Where(t => t.TargetId == userId)
                                                 .Select(t => new NotificationDto {
                                                     Id = t.Id,
                                                     TargetId= t.TargetId,
                                                     CreatedAt= t.CreatedAt,
                                                     IsRead = t.IsRead,
                                                     IsSystem = t.IsSystem,
                                                     Message = t.Message,
                                                     TypeId = t.TypeId,
                                                     SenderId = t.SenderId,
                                                 })
                                                 .ToListAsync();
            return data;
        }

        public async Task<Success> UpdateReadStatusId(long id, bool statusRead)
        {
            var dataNotify = await _noti.Notifications.FindAsync(id);
            if (dataNotify == null)
            {
                return new Success { success = false, message = "Không tìm thấy id thông báo" };
            }
            dataNotify.IsRead = true;
            await _noti.SaveChangesAsync();
            return new Success { success = true };
        }

        public async Task<Success> UpdateAllRead(string userId,bool statusRead)
        {
            var notifications = await _noti.Notifications
                                .Where(n => n.TargetId == userId)
                                .ToListAsync();
            if (notifications == null)
            {
                return new Success { success = false, message = "Không tìm thấy id người dùng" };
            }
            foreach (var notification in notifications)
            {
                notification.IsRead = true;    
            }
            await _noti.SaveChangesAsync();
            return new Success { success = true };
        }
    }
}
