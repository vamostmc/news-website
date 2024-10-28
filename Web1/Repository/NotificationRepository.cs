using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web1.Data;
using Web1.DataNew;
using Web1.Helps;
using Web1.Models;

namespace Web1.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TinTucDbContext _noti;

        public NotificationRepository(TinTucDbContext noti) { _noti = noti; }

        public async Task<NotificationDto> AddNotifyTinTucRepo(NotificationDto notification)
        {
            try
            {
                notification.Timestamp = LocalTime.GetLocalTime();
                var data = new Notification
                {
                    Message = notification.Message,
                    Timestamp = notification.Timestamp,
                    TypeId = notification.TypeId,
                };
                await _noti.Notifications.AddAsync(data);
                await _noti.SaveChangesAsync();
                return notification;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không thể thêm thông báo");
            }
        }

        public async Task<List<NotificationDto>> GetNotify(int IdNotify)
        {
            //Với TypeId = 1 là thông báo phần tin tức
            //Với TypeId = 2 là thông báo phần bình luận
            var data = await _noti.Notifications.Where(t => t.TypeId == IdNotify)
                                                .Select(t => new NotificationDto
                                                {
                                                    Id = t.Id,
                                                    Timestamp = t.Timestamp,
                                                    TypeId = t.TypeId,
                                                    IsRead = t.IsRead,
                                                    Message = t.Message,
                                                }).ToListAsync();
            return data;
        }

        


        public async Task<Notification> GetNotificationRepo(int id)
        {
            try
            {
                var data = await _noti.Notifications.FindAsync(id);
                if (data == null)
                {
                    throw new Exception("Lỗi không tìm thấy");
                }

                return data;
            }
            catch (Exception ex) 
            {
                throw new Exception("Lỗi không tìm thấy");
            }
        }

        public async Task RemoveNotificationRepo(int id)
        {
            try
            {
                var data = await _noti.Notifications.FindAsync(id);
                if (data == null)
                {
                    throw new Exception("Lỗi không tìm thấy");
                }

                _noti.Remove(data);
                await _noti.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception("Lỗi không thể xóa");
            }
        }

        public async Task UpdateNotificationRepo(int id)
        {
            try
            {
                TimeZoneInfo hanoiBangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime utcNow = DateTime.UtcNow; // Thời gian UTC hiện tại
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, hanoiBangkokTimeZone); // Chuyển đổi sang giờ địa phương

                // Tìm Notification dựa trên id
                var notifications = await _noti.Notifications.ToListAsync();

                foreach (var data in notifications)
                {
                    if (data == null)
                    {
                        throw new Exception("Lỗi không tìm thấy");
                    }

                    // Cập nhật các trường của Notification
                    data.IsRead = false; // Hoặc giá trị nào đó mà bạn muốn gán
                    data.Timestamp = localTime;

                    // Nếu có bài viết liên quan, cập nhật Message
                    

                    // Đặt TintucId và Tintuc thành null
                    
                }

                _noti.Notifications.UpdateRange(notifications);
                await _noti.SaveChangesAsync();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không thể xóa", ex);
            }
        }

        public async Task<NotifyBinhLuan> AddNotifyBinhLuanRepo(NotifyBinhLuan notifyBinhLuan)
        {
            try
            {
                var user = await _noti.Users.FindAsync(notifyBinhLuan.UserId);
                var news = await _noti.TinTucs.FindAsync(notifyBinhLuan.TinTucId);

                if (user == null || news == null)
                {
                    throw new Exception("Lỗi người dùng hoặc bài viết không tồn tại");
                }

                var DataNew = new Notification
                {
                    IsRead = false,
                    Timestamp = LocalTime.GetLocalTime(),
                    TypeId = 2,
                };

                if(notifyBinhLuan.Action == "Add")
                {
                   DataNew.Message = $"Người dùng: {user.UserName} thêm mới bình luận ở bài viết {news.TieuDe}";
                }    
                
                if(notifyBinhLuan.Action == "Edit")
                {
                    DataNew.Message = $"Người dùng: {user.UserName} sửa bình luận ở bài viết {news.TieuDe}";
                }

                if (notifyBinhLuan.Action == "Delete")
                {
                    DataNew.Message = $"Người dùng: {user.UserName} xóa bình luận ở bài viết {news.TieuDe}";
                }

                await _noti.AddAsync(DataNew);
                await _noti.SaveChangesAsync();
                return notifyBinhLuan;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không thể thêm thông báo bình luận", ex);
            }
        }
    }
}
