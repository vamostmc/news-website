using Microsoft.AspNetCore.Mvc;
using Web1.Data;
using Web1.DataNew;
using Web1.Models;

namespace Web1.Repository
{
    public interface ITintucRepository
    {
        public Task<List<TinTuc>> GetALlTinTuc();
        
        public Task<TinTucDto> GetTinTuc(int id);

        public Task AddTinTuc(TinTucImage tinTuc);

        public Task DeleteTinTuc(int id);

        public Task UpdateTinTuc(int id, TinTucImage tinTuc);

        public Task<List<TinTucDto>> UpdateTrangThai(int id, bool trangThai);

        public Task<List<TinTucDto>> GetTinTucDto();

        public Task<List<DanhMuc>> GetDanhMuc();
    }
}
