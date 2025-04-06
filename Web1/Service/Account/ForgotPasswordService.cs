using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using Web1.Data;
using Web1.Helps;
using Web1.Models;
using Web1.Repository;
using Web1.Service.Mail;
using Web1.Service.Redis;

namespace Web1.Service.Account
{
    public class ForgotPasswordService : IForgotPasswordService
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ISendMailService _sendMailService;
        private readonly IRedisService _redisService;
        private readonly INotificationRepository _notificationRepo;

        public ForgotPasswordService(
            
            UserManager<AppUser> userManager,
            IMemoryCache memoryCache,
            ISendMailService sendMailService,
            IRedisService redisService,
            INotificationRepository notificationRepo)
        {
           
            _userManager = userManager;
            _memoryCache = memoryCache;
            _sendMailService = sendMailService;
            _redisService = redisService;
            _notificationRepo = notificationRepo;
        }

        public async Task<AppUser> GetUserPassword(string userNameorEmail)
        {
            AppUser user = null;
            if (userNameorEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(userNameorEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(userNameorEmail);
            }

            if (user == null)
            {
                return null;
            }
            return user;
        }


        public async Task SendOtpResetPassword(AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentException("Người dùng không tồn tại.");
            }

            var newCode = GenerateOtp();
            await _redisService.SetStringRedisAsync(TypeKeyRedis.FORGOT_PASSWORD_PREFIX, user.Email, newCode, TimeSpan.FromMinutes(5));

            string subject = "Mã xác nhận để đổi mật khẩu";
            string fullBody = $"<br>Mã xác nhận đổi mật khẩu của bạn là: <strong>{newCode}</strong>. Vui lòng nhập mã này trong vòng 3 phút.";

            var result = await _sendMailService.SetUpSendMail(user.Email, subject, fullBody);


            if (!result.success)
            {
                throw new Exception("Lỗi khi gửi mail.");
            }
        }

        private string GenerateOtp()
        {
            var verificationCode = new Random().Next(100000, 999999).ToString();
            return verificationCode;
        }

        public async Task<Success> ResetPasswordAsync(ResetPasswordModel resetPassword)
        {
            var value = await _redisService.GetStringRedisAsync(TypeKeyRedis.FORGOT_PASSWORD_PREFIX, resetPassword.userId);
            if (value != string.Empty) 
                {
                if (value == resetPassword.resetToken)
                {
                    // Token hợp lệ, tiếp tục thực hiện reset mật khẩu
                    var user = await _userManager.FindByIdAsync(resetPassword.userId);
                    if (user == null)
                    {
                        throw new Exception("Không tìm thấy người dùng");
                    }
                    var result = await ActionChangePassword(user, resetPassword.newPassword);
                    return result;
                }
                else
                {
                    // Token không hợp lệ hoặc đã hết hạn
                    return new Success { success = false, message = "Bạn không có quyền truy cập" };

                }
            }
            return new Success { success = false, message = "Bạn không có quyền truy cập" };

        }

        public async Task<Success> UpdatePasswordOldAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByNameAsync(resetPasswordDto.UserName);
            if (user == null)
            {
                return new Success { success = false, message = "Lỗi không tìm thấy người dùng" };
            }

            var check = await _userManager.CheckPasswordAsync(user, resetPasswordDto.oldPassword);
            if (check == true)
            {
                var result = await ActionChangePassword(user, resetPasswordDto.newPassword);
                return result;
            }
            return new Success { success = false, message = "Password sai" };
        }

        private async Task<Success> ActionChangePassword(AppUser user, string newPassword)
        {
            var resetPasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!resetPasswordResult.Succeeded)
            {
                return new Success { success = false, message = "Lỗi không thể xóa mật khẩu" };
            }

            // Thêm mật khẩu mới
            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addPasswordResult.Succeeded)
            {
                return new Success { success = false, message = "Lỗi không thể thêm mật khẩu" };
            }

            // Lưu thông tin người dùng
            await _userManager.UpdateAsync(user);
            await _redisService.RemoveToRedisAsync(TypeKeyRedis.FORGOT_PASSWORD_PREFIX, user.Id);
            await _redisService.RemoveToRedisAsync(TypeKeyRedis.FORGOT_PASSWORD_PREFIX, user.Email);
            return new Success { success = true };
        }

        public async Task<Success> CheckOtpCode(VerifyCodeDto verify)
        {
            var value = await _redisService.GetStringRedisAsync(TypeKeyRedis.FORGOT_PASSWORD_PREFIX, verify.EmailUser);
            if (value != string.Empty)
            {
                    if (value == verify.Code)
                    {
                        var user = await _userManager.FindByEmailAsync(verify.EmailUser);
                        if (user != null)
                        {
                            user.EmailConfirmed = true;
                            await _userManager.UpdateAsync(user);
                        }

                        var token = Guid.NewGuid().ToString();
                        await _redisService.SetStringRedisAsync(TypeKeyRedis.FORGOT_PASSWORD_PREFIX, user.Id, token, TimeSpan.FromMinutes(2));
                        return new Success { success = true, message = token };
                    }
                    else
                    {
                        return new Success { success = false, message = "invalid" };
                    }
            }
            else
            {
                // Nếu mã không tồn tại hoặc đã hết hạn
                return new Success { success = false, message = "expired" };
            }
        }

    }
}
