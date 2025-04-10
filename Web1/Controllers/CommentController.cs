﻿using Microsoft.AspNetCore.Http;
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

        [HttpGet("GetCommentByTinTucId/{id}")]
        public async Task<List<BinhLuanDto>> GetBinhLuanNews(int id)
        {
            return await _binhLuan.GetCommentByTinTucId(id);
        }


        [HttpDelete("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
           await _binhLuan.DeleteComment(id);
            return Ok(new { message = "Thanh cong" });
        }

        [HttpPost("AddCommentNew")]
        public async Task<BinhLuanDto> AddCommentNew([FromForm] BinhLuanDto NewComment)
        {
            var data = await _binhLuan.AddCommentNew(NewComment);
            return data;
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
