using Microsoft.AspNetCore.Http;
using Web1.Data;

namespace Web1.Models
{
    public class TinTucImage
    {
        public int TintucId { get; set; }

        public string TieuDe { get; set; } = null!;

        public IFormFile? Image { get; set; }

        public string? MoTaNgan { get; set; }

        public DateTime? NgayDang { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public int? LuongKhachTruyCap { get; set; }

        public bool? TrangThai { get; set; }

        public int? DanhmucId { get; set; }

        public string? NoiDung { get; set; }

        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

        public virtual DanhMuc? Danhmuc { get; set; }
    }
}
