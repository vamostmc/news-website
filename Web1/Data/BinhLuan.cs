﻿using System;
using System.Collections.Generic;
using Web1.DataNew;
using Web1.Models;

namespace Web1.Data;

public partial class BinhLuan
{
    public int BinhluanId { get; set; }

    public int? TintucId { get; set; }

    public string? UserId { get; set; }

    public DateTime? NgayGioBinhLuan { get; set; }

    public string NoiDung { get; set; } = null!;

    public bool? TrangThai { get; set; }

    public virtual TinTuc? Tintuc { get; set; }

    public virtual AppUser? User { get; set; }

}