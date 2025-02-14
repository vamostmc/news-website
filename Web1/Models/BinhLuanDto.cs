using Web1.DataNew;

namespace Web1.Models
{
    public class BinhLuanDto
    {
        public int BinhluanId { get; set; }

        public int? TintucId { get; set; }

        public string? UserId { get; set; }

        public DateTime? NgayGioBinhLuan { get; set; }

        public string NoiDung { get; set; } = null!;

        public string? UserName { get; set; }

        public string? TieuDeTinTuc { get; set; }

        public bool? TrangThai {  get; set; }

        public int? ParentId { get; set; }

        public int? Likes { get; set; }

        public List<BinhLuanDto>? Replies { get; set; }
    }
}
