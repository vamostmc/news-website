using System;
using System.Collections.Generic;

namespace Web1.Data;

public partial class KhachHang
{
    public int KhachhangId { get; set; }

    public string? TenKhachHang { get; set; }

    public string? VaiTro { get; set; }

    public DateOnly? NgaySinh { get; set; }
}
