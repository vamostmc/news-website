using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Repository;
using Web1.Models;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _binhLuan;

        public CommentController(ICommentRepository binhLuan ) { _binhLuan = binhLuan; }

        [HttpGet("GetAllComment")]
        public async Task<List<BinhLuanDto>> GetComment()
        {
            return await _binhLuan.GetAllComment();
        }

        [HttpGet("GetCommentById/{id}")]
        public async Task<BinhLuanDto> GetBinhLuanById(int id)
        {
            return await _binhLuan.GetCommentByid(id);
        }

        [HttpDelete("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
           await _binhLuan.DeleteComment(id);
            return Ok(new { message = "Thanh cong" });
        }

        [HttpPost("AddCommentNew")]
        public async Task<IActionResult> AddCommentNew([FromForm] BinhLuanDto NewComment)
        {
            await _binhLuan.AddCommentNew(NewComment);
            return Ok(new { message = "Thanh cong" });
        }


        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromForm] BinhLuan NewComment)
        {
            try
            {
                // Thêm comment mới vào DB
                await _binhLuan.AddComment(NewComment);

                Console.WriteLine("Dữ liệu đã được chèn thành công vào SQL Server.");
                return Ok(NewComment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chèn dữ liệu: {ex.Message}");
                return BadRequest("Lỗi khi thêm bình luận.");
            }
        }


        [HttpPut]
        [Route("EditCommnent/{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromForm] BinhLuanDto binhLuan)
        {
            try
            {
                //Edit Binh luan vao DB
                 await _binhLuan.UpdateCommentRepo(id, binhLuan);
                return Ok(binhLuan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chèn dữ liệu: {ex.Message}");
                return BadRequest("Lỗi khi thêm bình luận.");
            }
        }


        [HttpPut]
        [Route("UpdateStatus/{id}")]
        public async Task<List<BinhLuanDto>> UpdateStatus(int id, [FromBody] bool status)
        {
            try
            {
                //Edit Binh luan vao DB
                return await _binhLuan.UpdateStatusAsync(id, status);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật trạng thái bình luận");
            }
        }


    }
}
