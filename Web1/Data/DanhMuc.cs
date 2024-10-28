using System;
using System.Collections.Generic;
using Web1.DataNew;

namespace Web1.Data;

public partial class DanhMuc
{
    public int DanhmucId { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public bool TrangThai { get; set; }

    public virtual ICollection<TinTuc> TinTucs { get; set; } = new List<TinTuc>();
}
