using Web1.Data;

namespace Web1.Models
{
    public class NotificationDto
    {
        public long Id { get; set; }

        public string Message { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public bool? IsRead { get; set; }

        public int TypeId { get; set; }

        public string TargetId { get; set; } = null!;

        public string? SenderId { get; set; }

        public bool? IsSystem { get; set; }


    }
}
