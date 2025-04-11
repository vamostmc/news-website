namespace Web1.Models
{
    public class ConversationDto
    {
        public long Id { get; set; }

        public string UserId { get; set; } = null!;

        public bool? IsActive { get; set; }

        public string? LastMessage { get; set; }

        public string? UserName { get; set; }

    }
}
