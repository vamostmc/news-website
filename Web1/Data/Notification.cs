using System;
using System.Collections.Generic;
using Web1.DataNew;

namespace Web1.Data;

public partial class Notification
{
    public long Id { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsRead { get; set; }

    public int TypeId { get; set; }

    public string TargetId { get; set; } = null!;

    public string? SenderId { get; set; }

    public bool? IsSystem { get; set; }

    public virtual NotificationType Type { get; set; } = null!;
}
