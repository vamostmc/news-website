namespace Web1.Models
{
    public class VerifyCodeModel
    {
        public string EmailUser { get; set; } // Email cần xác nhận
        public string Code { get; set; }  // Mã xác nhận
        public string UserId { get; set; }
    }
}
