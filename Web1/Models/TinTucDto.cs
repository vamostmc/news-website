using Web1.Data;

namespace Web1.Models
{
    public class TinTucDto
    {
        public int TintucId { get; set; }

        public string TieuDe { get; set; } = null!;

        public string HinhAnh { get; set; }

        public string? MoTaNgan { get; set; }

        public DateTime? NgayDang { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public int? LuongKhachTruyCap { get; set; }

        public int? SoLuongComment { get; set; }

        public bool? TrangThai { get; set; }

        public string? NoiDung { get; set; }

        public int? DanhmucId { get; set; }

        public string TenDanhMuc { get; set; } = null!;
    }
}
