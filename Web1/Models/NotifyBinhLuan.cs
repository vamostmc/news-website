namespace Web1.Models
{
    //Lấy dữ liệu người dùng và bài viết đã bình luận để trả về thông báo
    public class NotifyBinhLuan
    {
        public int? TinTucId { get; set; }

        public string? UserId { get; set; }

        public string? Action { get; set; }
    }
}
