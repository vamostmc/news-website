using System;
using System.Collections.Generic;

namespace Web1.Data;

public partial class Message
{
    public long Id { get; set; }

    public long ConversationId { get; set; }

    public string SenderId { get; set; } = null!;

    public string? ReceiverId { get; set; }

    public string? Text { get; set; }

    public DateTime? SentAt { get; set; }

    public bool? IsRead { get; set; }

    public string? Status { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
}
