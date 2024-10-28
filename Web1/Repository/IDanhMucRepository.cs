using Web1.Data;
using Web1.Models;
namespace Web1.Repository
{
    public interface IDanhMucRepository
    {
        public Task<List<DanhMucDto>> GetDanhMuc();

        public Task<DanhMuc> GetDanhMucIDAsync(int id);

        public Task<DanhMucDto> UpdateDanhMuc(int id, DanhMucDto danhMuc);

        public Task DeleteDanhMucAsync(int id);

        public Task<DanhMucDto> AddDanhMuc(DanhMucDto danhMuc);

        public Task<List<DanhMucDto>> UpdateStatus(int id, bool status);
    }
}
