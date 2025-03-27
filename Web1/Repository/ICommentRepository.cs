using Web1.Data;
using Web1.Models;

namespace Web1.Repository
{
    public interface ICommentRepository 
    {
        public Task<List<BinhLuanDto>> GetAllComment();

        public Task<BinhLuanDto> GetCommentByid(int id);

        public Task<List<BinhLuanDto>> GetCommentByTinTucId(int id);

        public Task<BinhLuan> AddComment(BinhLuan comment);

        public Task<BinhLuanDto> AddCommentNew(BinhLuanDto comment);

        public Task DeleteComment(int id);

        public Task UpdateCommentRepo(int id, BinhLuanDto comment);

        public Task<List<BinhLuanDto>> UpdateStatusAsync(int id, bool status);
    }
}
