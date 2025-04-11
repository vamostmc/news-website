namespace Web1.Models
{
    public class MessageDto
    {
        public long Id { get; set; }

        public long ConversationId { get; set; }

        public string SenderId { get; set; } = null!;

        public string? ReceiverId { get; set; }

        public string? Text { get; set; }

        public DateTime? SentAt { get; set; }

        public bool? IsRead { get; set; }

        public string? Status { get; set; }
    }
}
