using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Web1.Data;
using Web1.Exceptions;
using Web1.Helps;
using Web1.Models;
using Web1.Service.RabbitMq.Producer;

namespace Web1.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TinTucDbContext _binhLuan;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRabbitMqProducer _rabbitMqProducer;

        public CommentRepository(TinTucDbContext binhLuan,
                                 INotificationRepository notificationRepository,
                                 IRabbitMqProducer rabbitMqProducer) 
        { 
            _binhLuan = binhLuan;
            _notificationRepository = notificationRepository;
            _rabbitMqProducer = rabbitMqProducer;
        }

        public async Task<BinhLuanDto> AddCommentNew(BinhLuanDto comment)
        {
            try
            {
                if(comment.ParentId != null)
                {
                    var checkParentId = await _binhLuan.BinhLuans.FindAsync(comment.ParentId);
                    if(checkParentId == null) 
                    { 
                        throw new RepositoryException("Không tồn tại bình luận cha"); 
                    }
                }

                var tintucs = await _binhLuan.TinTucs.FindAsync(comment.TintucId);
                if(tintucs == null)
                {
                    throw new RepositoryException("Không tồn tại bài viết bình luận");
                }

                var dataNew = new BinhLuan
                {
                    NgayGioBinhLuan = DateTime.Now,
                    NoiDung = comment.NoiDung,
                    UserId = comment.UserId,
                    TintucId = comment.TintucId,
                    Likes = comment.Likes,
                    ParentId = comment.ParentId,
                    ReplyToUserId = comment.ReplyToUserId
                };

                await _binhLuan.AddRangeAsync(dataNew);
                await _binhLuan.SaveChangesAsync();

                if(comment.ReplyToUserId != null)
                {
                    var ReplyUser = await _binhLuan.Users.FirstOrDefaultAsync(c => c.Id == comment.ReplyToUserId);
                    comment.UserReplyName = ReplyUser.UserName;

                    var UserComment = await _binhLuan.Users.FirstOrDefaultAsync(c => c.Id == comment.UserId);

                    var data = await _notificationRepository.CreatCommentNotify(comment.ReplyToUserId, UserComment.UserName, tintucs.TieuDe);
                    await _rabbitMqProducer.PublishEvent(KeyRabbit.USER_NOTIFICATION_ROUTING, data);
                }
                else
                {
                    comment.UserReplyName = null;
                }

                return comment;
                    
            }
            catch (RepositoryException ex)
            {
                throw new RepositoryException("Lỗi không thể thêm bình luận: " + ex.Message, ex);
            }
        }

        public async Task DeleteComment(int id)
        {
            try
            {
                var data = await _binhLuan.BinhLuans.FindAsync(id);
                if (data != null)
                {
                    _binhLuan.BinhLuans.Remove(data);
                    await _binhLuan.SaveChangesAsync(); 
                }
                else
                {
                    throw new RepositoryException($"Bình luận với ID {id} không tồn tại.");
                }
            }
            catch (RepositoryException ex)
            {
                // Xử lý ngoại lệ
                throw new RepositoryException("Có lỗi xảy ra khi xóa bình luận.", ex);
            }
        }


        public async Task<List<BinhLuanDto>> GetAllComment()
        {
            var data = await _binhLuan.BinhLuans.Include(t => t.Tintuc)
                                                .Include(t => t.User)
                                                .Select(t => new BinhLuanDto
                                                {
                                                    TintucId = t.TintucId,
                                                    BinhluanId = t.BinhluanId,
                                                    NgayGioBinhLuan = t.NgayGioBinhLuan,
                                                    NoiDung = t.NoiDung,
                                                    TieuDeTinTuc = t.Tintuc.TieuDe,
                                                    UserName = t.User.UserName,
                                                    UserId = t.UserId,
                                                    ReplyToUserId = t.ReplyToUserId,
                                                    TrangThai = t.TrangThai,
                                                    ParentId = t.ParentId,
                                                    Likes = t.Likes,
                                                }).ToListAsync();

            return data;
        }

        public async Task<BinhLuanDto> GetCommentByid(int id)
        {
            try
            {
                var data = await _binhLuan.BinhLuans.Include(t => t.Tintuc)
                                                    .Include(t => t.User)
                                                    .Where(t => t.BinhluanId == id)
                                                    .Select(t => new BinhLuanDto
                                                    {
                                                        TintucId = t.TintucId,
                                                        BinhluanId = t.BinhluanId,
                                                        NgayGioBinhLuan = t.NgayGioBinhLuan,
                                                        NoiDung = t.NoiDung,
                                                        TieuDeTinTuc = t.Tintuc.TieuDe,
                                                        UserName = t.User.UserName,
                                                        ReplyToUserId = t.ReplyToUserId,
                                                        UserId = t.UserId,
                                                        TrangThai = t.TrangThai,
                                                        ParentId = t.ParentId,
                                                        Likes = t.Likes,
                                                    }).FirstOrDefaultAsync();
                if(data == null)
                {
                    throw new RepositoryException($"Không tìm thấy ID comment {id}.");
                }
                return data; 
            }
            catch (RepositoryException ex)
            {
                // Xử lý ngoại lệ
                throw new RepositoryException($"Có lỗi khi lấy bình luận với ID {id}.", ex);
            }
        }

        public async Task<List<BinhLuanDto>> GetCommentByTinTucId(int id)
        {
            var comments = await _binhLuan.BinhLuans
                .Include(t => t.Tintuc)
                .Include(t => t.User)
                .Where(t => t.TintucId == id)
                .ToListAsync();

            
            var data = comments
                .Where(bl => bl.ParentId == null)
                .Select(blg => new BinhLuanDto
                {
                    BinhluanId = blg.BinhluanId,
                    TintucId = blg.TintucId,
                    UserId = blg.UserId,
                    NgayGioBinhLuan = blg.NgayGioBinhLuan,
                    NoiDung = blg.NoiDung,
                    UserName = blg.User != null ? blg.User.UserName : "Ẩn danh",
                    TieuDeTinTuc = blg.Tintuc.TieuDe,
                    ReplyToUserId = blg.ReplyToUserId,
                    TrangThai = blg.TrangThai,
                    ParentId = blg.ParentId,
                    Likes = blg.Likes ?? 0,
                    Replies = comments
                        .Where(reply => reply.ParentId == blg.BinhluanId)
                        .Select(reply => new BinhLuanDto
                        {
                            BinhluanId = reply.BinhluanId,
                            TintucId = reply.TintucId,
                            UserId = reply.UserId,
                            NgayGioBinhLuan = reply.NgayGioBinhLuan,
                            NoiDung = reply.NoiDung,
                            UserName = reply.User != null ? reply.User.UserName : "Ẩn danh",
                            ReplyToUserId = reply.ReplyToUserId,
                            TieuDeTinTuc = reply.Tintuc.TieuDe,
                            TrangThai = reply.TrangThai,
                            ParentId = reply.ParentId,
                            Likes = reply.Likes ?? 0,
                            UserReplyName = reply.ReplyToUserId != null
                                ? comments.FirstOrDefault(c => c.UserId == reply.ReplyToUserId)?.User.UserName
                                : null
                        })
                        .ToList()
                }).ToList();

            return data;
        }

        public async Task UpdateCommentRepo(int id, BinhLuanDto comment)
        {
            try
            {
                var data = await _binhLuan.BinhLuans.FirstOrDefaultAsync(u => u.BinhluanId == id);
                if (data != null)
                {
                    var checkUser = await _binhLuan.AppUsers.FindAsync(comment.UserId);
                    if (checkUser == null) 
                    {
                        throw new RepositoryException("Người dùng không tồn tại.");
                    }

                    var checkTinTuc = await _binhLuan.TinTucs.FindAsync(comment.TintucId);
                    if (checkTinTuc == null)
                    {
                        throw new RepositoryException("Bài viết không tồn tại.");
                    }

                    // Cập nhật thông tin bình luận
                    data.NoiDung = comment.NoiDung;
                    data.NgayGioBinhLuan = comment.NgayGioBinhLuan;
                    data.User = checkUser;
                    data.TrangThai = comment.TrangThai;
                    data.Tintuc = checkTinTuc;
                    data.Likes = comment.Likes;
                    data.ParentId = comment.ParentId;
                    data.ReplyToUserId = comment.ReplyToUserId;

                    // Lưu thay đổi
                    _binhLuan.BinhLuans.Update(data);
                    await _binhLuan.SaveChangesAsync();
                    
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy bình luận
                    throw new RepositoryException($"Bình luận với ID {id} không tồn tại.");
                }
            }
            catch (RepositoryException ex)
            {
                // Xử lý ngoại lệ
                throw new RepositoryException("Có lỗi xảy ra khi cập nhật bình luận.", ex);
            }
        }

        public async Task<List<BinhLuanDto>> UpdateStatusAsync(int id, bool status)
        {
            try
            {
                var data = await _binhLuan.BinhLuans.FindAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("Lỗi không tìm thấy bình luận");
                }

                data.TrangThai = status;
                _binhLuan.Update(data);
                await _binhLuan.SaveChangesAsync();
                return await GetAllComment();

            }
            catch (RepositoryException ex)
            {
                throw new RepositoryException("Lỗi không cập nhật được trạng thái", ex);
            }
        }
    }

    
}
