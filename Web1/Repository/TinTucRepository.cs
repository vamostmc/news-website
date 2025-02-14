using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.DataNew;
using Web1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web1.Repository
{
    public class TinTucRepository : ITintucRepository
    {
        private readonly TinTucDbContext _tinTuc;

        public TinTucRepository(TinTucDbContext tinTuc) { _tinTuc = tinTuc; }

        //Lấy thông tin có chứa thêm tên danh mục
        public async Task<List<TinTucDto>> GetTinTucDto()
        {

            var tinTucs = await _tinTuc.TinTucs
                                .Include(t => t.BinhLuans)
                                .Include(t => t.Danhmuc)                    // Lấy thông tin danh mục liên quan
                                .Select(t => new TinTucDto
                                {
                                    TintucId = t.TintucId,
                                    TieuDe = t.TieuDe,
                                    HinhAnh = t.HinhAnh,
                                    MoTaNgan = t.MoTaNgan,
                                    DanhmucId = t.DanhmucId,
                                    NgayDang = t.NgayDang,
                                    NgayCapNhat = t.NgayCapNhat,
                                    NoiDung = t.NoiDung,
                                    TrangThai = t.TrangThai,
                                    LuongKhachTruyCap = t.LuongKhachTruyCap,
                                    SoLuongComment = t.BinhLuans.Count(),
                                    TenDanhMuc = t.Danhmuc.TenDanhMuc       // Lấy tên danh mục
                                })
                                .ToListAsync();
            return tinTucs;
        }

        //Lấy thông tin gốc của Tin tức
        public async Task<List<TinTuc>> GetALlTinTuc()
        {
            return await _tinTuc.TinTucs.ToListAsync();
        }

        public async Task<TinTucDto> GetTinTuc(int id)
        {
            var data = await _tinTuc.TinTucs
                                    .Include(t => t.Danhmuc)
                                    .Include(t => t.BinhLuans)
                                    .ThenInclude(bl => bl.User)
                                    .Where(t => t.TintucId == id)
                                    .Select(t => new TinTucDto
                                    {
                                        TintucId = t.TintucId,
                                        TieuDe = t.TieuDe,
                                        HinhAnh = t.HinhAnh,
                                        MoTaNgan = t.MoTaNgan,
                                        NgayDang = t.NgayDang,
                                        DanhmucId = t.DanhmucId,
                                        NgayCapNhat = t.NgayCapNhat,
                                        SoLuongComment = t.BinhLuans.Count(),
                                        LuongKhachTruyCap = t.LuongKhachTruyCap,
                                        NoiDung = t.NoiDung,
                                        TrangThai = t.TrangThai,
                                        TenDanhMuc = t.Danhmuc.TenDanhMuc,
                                    })
                                    .FirstOrDefaultAsync();

            return data;
        }



        public async Task<List<DanhMuc>> GetDanhMuc()
        {
            return await _tinTuc.DanhMucs.ToListAsync();
        }

        public async Task AddTinTuc(TinTucImage tinTuc)
        {
            var NewTintuc = new TinTuc
            {
                DanhmucId = tinTuc.DanhmucId,
                MoTaNgan = tinTuc.MoTaNgan,
                NgayDang = tinTuc.NgayDang,
                NgayCapNhat = tinTuc.NgayCapNhat,
                TieuDe = tinTuc.TieuDe,
                BinhLuans = tinTuc.BinhLuans,
                LuongKhachTruyCap = tinTuc.LuongKhachTruyCap,
                TrangThai = tinTuc.TrangThai,
                NoiDung = tinTuc.NoiDung,
            };

            if (tinTuc.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Hinh", tinTuc.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await tinTuc.Image.CopyToAsync(stream);
                }
                NewTintuc.HinhAnh = "/Hinh/" + tinTuc.Image.FileName;
            }

            else
            {
                NewTintuc.HinhAnh = "/Hinh/Null.png";
            }

            _tinTuc.TinTucs.Add(NewTintuc);
            await _tinTuc.SaveChangesAsync();
        }

        public async Task DeleteTinTuc(int id)
        {
            //Xóa tất cả bình luận trong bài viết này 
            var BinhLuanInTinTuc = _tinTuc.BinhLuans.Where(t => t.TintucId == id).ToList();
            if (BinhLuanInTinTuc != null && BinhLuanInTinTuc.Count > 0)
            {
                // Xóa tất cả bình luận
                foreach (var binhLuan in BinhLuanInTinTuc)
                {
                    _tinTuc.BinhLuans.Remove(binhLuan);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _tinTuc.SaveChangesAsync();
            }

            var data = await _tinTuc.TinTucs.FindAsync(id);
            if (data != null)
            {
                if (data.HinhAnh != "/Hinh/Null.png")
                {
                    // Xóa hình ảnh khỏi thư mục nếu có
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), data.HinhAnh.TrimStart('/'));

                    // Xóa file hình ảnh cũ nếu tồn tại
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _tinTuc.TinTucs.Remove(data);
                await _tinTuc.SaveChangesAsync();
            }

            else
            {
                // Xử lý trường hợp không tìm thấy bài viết
                throw new Exception("Bài viết không tồn tại.");
            }
        }

        public async Task UpdateTinTuc(int id, TinTucImage tinTuc)
        {
            var data = await _tinTuc.TinTucs.FindAsync(id);
            if (data == null)
            {
                throw new KeyNotFoundException("TinTuc không tồn tại.");
            }

            data.TieuDe = tinTuc.TieuDe;
            data.MoTaNgan = tinTuc.MoTaNgan;
            data.NgayDang = tinTuc.NgayDang;
            data.NgayCapNhat = tinTuc.NgayCapNhat;
            data.LuongKhachTruyCap = tinTuc.LuongKhachTruyCap;
            data.TrangThai = tinTuc.TrangThai;
            data.DanhmucId = tinTuc.DanhmucId;
            data.BinhLuans = tinTuc.BinhLuans;
            data.NoiDung = tinTuc.NoiDung;
            // Kiểm tra xem có hình ảnh mới không
            if (tinTuc.Image != null && tinTuc.Image.Length > 0)
            {
                // Đường dẫn tới file hình ảnh hiện tại
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), data.HinhAnh.TrimStart('/'));

                // Xóa file hình ảnh cũ nếu tồn tại
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Tạo đường dẫn file mới
                var newImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Hinh", tinTuc.Image.FileName);

                // Tạo file mới với ảnh cập nhật
                using (var stream = System.IO.File.Create(newImagePath))
                {
                    await tinTuc.Image.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn hình ảnh trong database với file mới
                data.HinhAnh = "/Hinh/" + tinTuc.Image.FileName;
            }
            else
            {
                // Nếu không có hình ảnh mới, giữ nguyên ảnh cũ hoặc để xử lý khác nếu muốn
                data.HinhAnh = data.HinhAnh; // Giữ nguyên ảnh cũ
            }

            _tinTuc.TinTucs.Update(data);
            await _tinTuc.SaveChangesAsync();
        }

        public async Task<List<TinTucDto>> UpdateTrangThai(int id, bool trangThai)
        {
            try
            {
                var data = await _tinTuc.TinTucs.FindAsync(id);
                if (data == null)
                {
                    throw new Exception("Không tìm thấy bài viết đó");
                }
                data.TrangThai = trangThai;

                _tinTuc.TinTucs.Update(data);
                await _tinTuc.SaveChangesAsync();
                return await GetTinTucDto();
            }
            catch (Exception ex) 
            {
                throw new Exception("Lỗi không thể cập nhật trạng thái", ex);
            }
        } 
    }
}
