using System;
using System.Collections.Generic;

namespace Web1.Data;

public partial class NotifyType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
