using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Exceptions;
using Web1.Models;

namespace Web1.Repository
{
    public class DanhMucRepository: IDanhMucRepository
    {
        private readonly TinTucDbContext _danhMuc;
        private readonly IMapper _mapper;

        public DanhMucRepository(TinTucDbContext danhMuc, 
                                    IMapper mapper) 
        { 
            _danhMuc = danhMuc; 
            _mapper = mapper; 
        }

        public async Task<DanhMucDto> AddDanhMuc(DanhMucDto danhMuc)
        {
            try
            {
                var dataNew = _mapper.Map<DanhMuc>(danhMuc);
                await _danhMuc.DanhMucs.AddAsync(dataNew);
                await _danhMuc.SaveChangesAsync();
                return danhMuc;
            }
            catch (Exception ex) 
            {
                throw new RepositoryException("Lỗi không thêm được danh mục", ex);
            }
        }

        public async Task DeleteDanhMucAsync(int id)
        {
            try
            {
                
                // Set những tin tức nằm trong danh mục bị xóa thành null
                var TinTucs = _danhMuc.TinTucs.Where(x => x.DanhmucId == id);
                foreach (var item in TinTucs)
                {
                    item.DanhmucId = null;
                }
                await _danhMuc.SaveChangesAsync();

                // Tiến hành xóa danh mục 
                var data = await _danhMuc.DanhMucs.FindAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("Lỗi không tìm thấy danh mục ID cần xóa");
                }
                _danhMuc.Remove(data);
                await _danhMuc.SaveChangesAsync();

            }
            catch (RepositoryException ex)
            {
                throw new RepositoryException("Lỗi không thể xóa được danh mục", ex);
            }
        }

        public async Task<List<DanhMucDto>> GetDanhMuc()
        {
            var danhMucsWithTinTucs = await _danhMuc.DanhMucs
                                                    .AsNoTracking()
                                                    .Include(t => t.TinTucs)
                                                    .ProjectTo<DanhMucDto>(_mapper.ConfigurationProvider)
                                                    .ToListAsync();

            return danhMucsWithTinTucs;
        }

        public async Task<DanhMuc> GetDanhMucIDAsync(int id)
        {
            var data = await _danhMuc.DanhMucs.FindAsync(id);

            if(data == null)
            {
                return null;
            }

            return data;
        }

        public async Task<DanhMucDto> UpdateDanhMuc(int id, DanhMucDto danhMuc)
        {
            try
            {
                var data = await _danhMuc.DanhMucs.FindAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("Lỗi không tìm thấy ID cần sửa");
                }
                data.TrangThai = danhMuc.TrangThai;
                data.TenDanhMuc = danhMuc.TenDanhMuc;
                

                _danhMuc.DanhMucs.Update(data);
                await _danhMuc.SaveChangesAsync();
                return danhMuc;
            }
            catch (RepositoryException ex)
            {
                throw new RepositoryException("Lỗi không thể xóa được danh mục", ex);
            }
        }

        public async Task<List<DanhMucDto>> UpdateStatus(int id, bool status)
        {
            try
            {
                var data = await _danhMuc.DanhMucs.FindAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("Lỗi không tìm thấy Id cần tìm");
                }

                data.TrangThai = status;
                _danhMuc.DanhMucs.Update(data);
                await _danhMuc.SaveChangesAsync();
                return await GetDanhMuc();
            }
            catch (RepositoryException ex) 
            {
                throw new RepositoryException("Lỗi không cập nhật được trạng thái");
            }
        }
    }
}
