using System;
using System.Collections.Generic;

namespace Web1.Data;

public partial class Conversation
{
    public long Id { get; set; }

    public string UserId { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
