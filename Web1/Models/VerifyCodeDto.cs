namespace Web1.Models
{
    public class VerifyCodeDto
    {
        public string EmailUser { get; set; } // Email cần xác nhận

        public string Code { get; set; }  // Mã xác nhận
    }
}
