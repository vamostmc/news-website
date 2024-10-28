using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.DataNew;
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
        public async Task<List<TinTucDto>> GetAllWithDm()
        {
            return await _tinTuc.GetTinTucDto();
        }

        [HttpGet("GetDanhmuc")]
        public async Task<List<DanhMuc>> GetAllDanhMuc()
        {
            return await _tinTuc.GetDanhMuc();
        }

        [HttpGet]
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
    }
}
