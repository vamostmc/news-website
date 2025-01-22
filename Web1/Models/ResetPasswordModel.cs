namespace Web1.Models
{
    public class ResetPasswordModel
    {
        public string newPassword { get; set; }

        public string userId { get; set; }

        public string resetToken { get; set; }
    }
}
