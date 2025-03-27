using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Web1.Data;
using Web1.DataNew;
using Web1.Helps;
using Web1.Models;
using Web1.Repository;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinTucController : ControllerBase
    {
        private readonly ITintucRepository _tinTuc;

        public TinTucController(ITintucRepository tinTuc)
        {
            _tinTuc = tinTuc;
        }

        [HttpGet("TintucWithDanhmuc")]
        [Authorize(Roles = Role.Customer)]
        public async Task<List<TinTucDto>> GetAllWithDm()
        {
            return await _tinTuc.GetTinTucDto();
        }

        [HttpGet("GetDanhmuc")]
        public async Task<List<DanhMuc>> GetAllDanhMuc()
        {
            return await _tinTuc.GetDanhMuc();
        }

        [HttpGet("AllTinTuc")]
        public async Task<List<TinTuc>> GetAll()
        {
            return await _tinTuc.GetALlTinTuc();
        }

        [HttpGet("{id}")]
        public async Task<TinTucDto> GetTinTucByID(int id)
        {
            var data = await _tinTuc.GetTinTuc(id);

            if (data == null)
            {
                return null;
            }

            else { return data; }
        }


        [HttpPost("ThemTinTuc")]
        public async Task<IActionResult> AddTinTuc([FromForm] TinTucImage tintuc)
        {
            await _tinTuc.AddTinTuc(tintuc);
            return Ok(tintuc);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteTinTuc(int id)
        {
            await _tinTuc.DeleteTinTuc(id);
            return Ok(new { message = "Thanh cong" });
        }

        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<IActionResult> EditTinTuc(int id, [FromForm] TinTucImage tinTuc)
        {
            try
            {
                await _tinTuc.UpdateTinTuc(id, tinTuc);
                return Ok(new { Message = "Cập nhật thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdateStatus/{id}")]
        public async Task<List<TinTucDto>> UpdateStatus(int id, [FromBody] bool trangThai)
        {
            try
            {
                return await _tinTuc.UpdateTrangThai(id, trangThai);
                
            }
            
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost("Import-Excel")]
        public async Task<Success> ImportExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new Success { success = false, message = "Không có file Excel truyền vào" };
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;
                        if (rowCount < 2) 
                        {
                            return new Success { success = false, message = "Điền đầy đủ các trường trong file Excel" };
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var tieuDe = worksheet.Cells[row, 1].Text;
                            var moTaNgan = worksheet.Cells[row, 2].Text;
                            var ngayDangText = worksheet.Cells[row, 3].Text;
                            var ngayCapNhatText = worksheet.Cells[row, 4].Text;
                            var danhmucIdText = worksheet.Cells[row, 5].Text;
                            var luongKhachTruyCapText = worksheet.Cells[row, 6].Text;
                            var trangThaiText = worksheet.Cells[row, 7].Text;
                            var noiDung = worksheet.Cells[row, 8].Text;

                            // Kiểm tra các trường rỗng hoặc không hợp lệ
                            if (string.IsNullOrWhiteSpace(tieuDe) ||
                                string.IsNullOrWhiteSpace(moTaNgan) ||
                                string.IsNullOrWhiteSpace(ngayDangText) ||
                                string.IsNullOrWhiteSpace(ngayCapNhatText) ||
                                string.IsNullOrWhiteSpace(danhmucIdText) ||
                                string.IsNullOrWhiteSpace(luongKhachTruyCapText) ||
                                string.IsNullOrWhiteSpace(trangThaiText))
                            {
                                return new Success { success = false, message = $"Dữ liệu hàng {row} bị thiếu hoặc không hợp lệ." };
                            }

                            // Chuyển đổi giá trị
                            var ngayDang = DateTime.Parse(ngayDangText);
                            var ngayCapNhat = DateTime.Parse(ngayCapNhatText);
                            var danhmucId = int.Parse(danhmucIdText);
                            var luongKhachTruyCap = int.Parse(luongKhachTruyCapText);
                            var trangThai = bool.Parse(trangThaiText);

                            // Tạo đối tượng TinTucImage
                            var tinTucImage = new TinTucImage
                            {
                                TieuDe = tieuDe,
                                MoTaNgan = moTaNgan,
                                NgayDang = ngayDang,
                                NgayCapNhat = ngayCapNhat,
                                DanhmucId = danhmucId,
                                LuongKhachTruyCap = luongKhachTruyCap,
                                TrangThai = trangThai,
                                NoiDung = noiDung,
                                Image = null
                            };

                            // Thêm tin tức
                            await AddTinTuc(tinTucImage);
                        }
                    }
                }
                return new Success { success = true };
            }
            catch (Exception ex)
            {
                return new Success { success = false, message = ex.ToString() };
            }
        }

        [HttpGet("Template-Excel")]
        public IActionResult GetTemplateExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Tạo một worksheet
                var worksheet = package.Workbook.Worksheets.Add("Template");

                // Thêm tiêu đề cột
                worksheet.Cells[1, 1].Value = "Tiêu đề";
                worksheet.Cells[1, 2].Value = "Mô tả ngắn";
                worksheet.Cells[1, 3].Value = "Ngày đăng";
                worksheet.Cells[1, 4].Value = "Ngày cập nhật";
                worksheet.Cells[1, 5].Value = "Danh mục ID";
                worksheet.Cells[1, 6].Value = "Lượt khách truy cập";
                worksheet.Cells[1, 7].Value = "Bình luận";
                worksheet.Cells[1, 8].Value = "Trạng thái (true/false)";
                worksheet.Cells[1, 9].Value = "Nội dung";

                // Định dạng tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Set kích thước cột tự động
                worksheet.Cells.AutoFitColumns();

                // Trả về file Excel dưới dạng byte[]
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template.xlsx");
            }
        }
    }


}
