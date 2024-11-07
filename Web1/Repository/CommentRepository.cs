using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Exceptions;
using Web1.Models;

namespace Web1.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TinTucDbContext _binhLuan;

        public CommentRepository(TinTucDbContext binhLuan) { _binhLuan = binhLuan; }

        public async Task<BinhLuan> AddComment(BinhLuan comment)
        {
            try
            {
                var checkUser = await _binhLuan.AppUsers.FindAsync(comment.UserId);
                if (checkUser == null)
                {
                    throw new RepositoryException("Id user người dùng tồn tại");
                }

                var checkTinTuc = await _binhLuan.TinTucs.FindAsync(comment.TintucId);
                if (checkTinTuc == null)
                {
                    throw new RepositoryException("Id tin tức tồn tại");
                }

                comment.User = checkUser;
                comment.Tintuc = checkTinTuc;

                await _binhLuan.AddAsync(comment);
                await _binhLuan.SaveChangesAsync();
                return comment;
            }
            catch (RepositoryException ex)
            {
                // Xử lý ngoại lệ
                throw new RepositoryException("Có lỗi xảy ra khi thêm bình luận.", ex);
            }
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

                    else
                    {
                        var checkTinTucId = await _binhLuan.TinTucs.FindAsync(comment.TintucId);
                    }
                }
                    

                var dataNew = new BinhLuan
                {
                    NgayGioBinhLuan = comment.NgayGioBinhLuan,
                    NoiDung = comment.NoiDung,
                    UserId = comment.UserId,
                    TintucId = comment.TintucId,
                    Likes = comment.Likes,
                    ParentId = comment.ParentId,
                };

                await _binhLuan.AddRangeAsync(dataNew);
                await _binhLuan.SaveChangesAsync();
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
