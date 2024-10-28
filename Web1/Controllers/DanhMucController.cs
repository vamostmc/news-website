﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Data;
using Web1.Repository;
using Web1.Models;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucController : ControllerBase
    {
        private readonly IDanhMucRepository _danhMuc;

        public DanhMucController( IDanhMucRepository danhMuc) { _danhMuc = danhMuc; }

        [HttpGet("GetDanhmuc")]
        public async Task<List<DanhMucDto>> GetAllDanhMuc()
        {
            return await _danhMuc.GetDanhMuc();
        }

        [HttpGet("GetDanhmuc/{id}")]
        public async Task<DanhMuc> GetDanhMucByID(int id)
        {
            return await _danhMuc.GetDanhMucIDAsync(id);
        }

        [HttpPost("AddDanhMuc")]
        public async Task<DanhMucDto> AddDanhMuc( [FromForm] DanhMucDto newDanhmuc)
        {
            return await _danhMuc.AddDanhMuc(newDanhmuc);
        }

        [HttpDelete("RemoveDanhMuc/{id}")]
        public async Task<IActionResult> DeleteDanhMuc(int id)
        {
            await _danhMuc.DeleteDanhMucAsync(id);
            return Ok(new { message = "Thanh cong" });
        }

        [HttpPut]
        [Route("EditDanhMuc/{id}")]
        public async Task<DanhMucDto> EditDanhMuc(int id, [FromForm] DanhMucDto danhmuc)
        {
            return await _danhMuc.UpdateDanhMuc(id, danhmuc);
        }


        [HttpPut]
        [Route("EditStatusDanhMuc/{id}")]
        public async Task<List<DanhMucDto>> EditStatusDanhMuc(int id, [FromBody] bool status)
        {
            return await _danhMuc.UpdateStatus(id, status);
        }
    }
}