namespace Web1.Models
{
    public class DanhMucDto
    {
        public int DanhmucId { get; set; }

        public string TenDanhMuc { get; set; } = null!;

        public int? SoLuongTinTuc { get; set; }

        public bool TrangThai { get; set; }


    }
}
