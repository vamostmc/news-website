using Web1.Models;

namespace Web1.Service
{
    public interface IConfirmMailService
    {
        public Task<Success> SendVerifyEmailAsync(string Id);

        public Task<string> CheckVerifyCode(VerifyCodeDto verify);

    }
}
