using AutoMapper;
using Web1.Data;
using Web1.Models;

namespace Web1.AutoMap
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            // Danh mục
            CreateMap<DanhMuc, DanhMucDto>()
            .ForMember(dest => dest.SoLuongTinTuc, opt => opt.MapFrom(src => src.TinTucs.Count));

            CreateMap<DanhMucDto, DanhMuc>()
                .ForMember(dest => dest.TinTucs, opt => opt.Ignore());
        }
    }
}
