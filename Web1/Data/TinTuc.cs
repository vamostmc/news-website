using System;
using System.Collections.Generic;
using Web1.Data;

namespace Web1.Data;

public partial class TinTuc
{
    public int TintucId { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? HinhAnh { get; set; }

    public string? MoTaNgan { get; set; }

    public DateTime? NgayDang { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public int? LuongKhachTruyCap { get; set; }

    public bool? TrangThai { get; set; }

    public string? NoiDung { get; set; }

    public int? DanhmucId { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual DanhMuc? Danhmuc { get; set; }

}
