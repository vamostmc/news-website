using Web1.Models;

namespace Web1.Service.Account
{
    public interface IForgotPasswordService
    {
        public Task SendOtpResetPassword(AppUser user);

        public Task<Success> ResetPasswordAsync(ResetPasswordModel resetPassword);

        public Task<Success> UpdatePasswordOldAsync(ResetPasswordDto resetPasswordDto);

        public Task<Success> CheckOtpCode(VerifyCodeDto verify);

        public Task<AppUser> GetUserPassword(string userNameorEmail);
    }
}
