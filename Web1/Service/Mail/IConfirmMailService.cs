using Web1.Models;

namespace Web1.Service.Mail
{
    public interface IConfirmMailService
    {
        public Task<AppUser> GetInfoUserMail(string id);

        public Task SendVerifyEmailAsync(AppUser user);

        public Task<string> CheckVerifyCode(VerifyCodeDto verify);

    }
}
