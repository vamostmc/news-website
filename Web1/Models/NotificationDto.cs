using Web1.Data;

namespace Web1.Models
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Message { get; set; } = null!;

        public DateTime? Timestamp { get; set; }

        public bool? IsRead { get; set; }

        public int? TypeId { get; set; }

    }
}
