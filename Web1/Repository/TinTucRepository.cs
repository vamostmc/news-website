using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Models;
using Web1.Service.AWS;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web1.Repository
{
    public class TinTucRepository : ITintucRepository
    {
        private readonly TinTucDbContext _tinTuc;
        private readonly IUploadAWSService _aws;

        public TinTucRepository(
            TinTucDbContext tinTuc, 
            IUploadAWSService aws ) 
        { 
            _tinTuc = tinTuc; 
            _aws = aws;
        }

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
                NewTintuc.HinhAnh = await _aws.UploadFileToAWS(tinTuc.Image);
            }

            else
            {
                NewTintuc.HinhAnh = "/Hinh/NULL.png";
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
                if (data.HinhAnh != "NULL.png")
                {
                    var checkImg = await _aws.DeleteFileAWS(data.HinhAnh);
                    if(checkImg.success == false)
                    {
                        throw new Exception("Lỗi khi xóa ảnh trên AWS: " + checkImg.message);
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
                // Cập nhật đường dẫn hình ảnh trong database và aws
                var checkImg = await _aws.DeleteFileAWS(data.HinhAnh);
                if (checkImg.success == false)
                {
                    throw new Exception("Lỗi khi xóa ảnh trên AWS: " + checkImg.message);
                }

                var UrlImage = await _aws.UploadFileToAWS(tinTuc.Image);
                data.HinhAnh = UrlImage;
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
