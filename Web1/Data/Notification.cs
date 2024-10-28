using System;
using System.Collections.Generic;
using Web1.DataNew;

namespace Web1.Data;

public partial class Notification
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public bool? IsRead { get; set; }

    public int? TypeId { get; set; }

    public virtual NotifyType? Type { get; set; }
}
