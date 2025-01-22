using Web1.Models;

namespace Web1.Service
{
    public interface IForgotPasswordService
    {
        public Task<Success> SendOtpResetPassword(string userNameOrEmail);

        public Task<Success> ResetPasswordAsync(ResetPasswordModel resetPassword);

        public Task<Success> UpdatePasswordOldAsync(ResetPasswordDto resetPasswordDto);

        public Task<Success> CheckOtpCode(VerifyCodeDto verify);
    }
}
