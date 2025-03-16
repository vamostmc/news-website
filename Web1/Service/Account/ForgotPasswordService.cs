using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using Web1.Models;
using Web1.Repository;
using Web1.Service.Mail;

namespace Web1.Service.Account
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IPasswordRepository _password;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ISendMailService _sendMailService;

        public ForgotPasswordService(
            IPasswordRepository password,
            UserManager<AppUser> userManager,
            IMemoryCache memoryCache,
            ISendMailService sendMailService)
        {
            _password = password;
            _userManager = userManager;
            _memoryCache = memoryCache;
            _sendMailService = sendMailService;
        }

        public async Task<Success> SendOtpResetPassword(string userNameOrEmail)
        {
            try
            {
                AppUser user = null;
                if (userNameOrEmail.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(userNameOrEmail);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(userNameOrEmail);
                }

                if (user == null)
                {
                    return new Success { success = false, message = "Người dùng không tồn tại" };
                }

                else
                {
                    var to = user.Email;
                    var Id = user.Id;
                    var newCode = GenerateOtp();
                    _memoryCache.Set($"verify:{to}", newCode, TimeSpan.FromMinutes(5));

                    string subject = "Mã xác nhận để đổi mật khẩu";
                    string fullBody = $"<br>Mã xác nhận đổi mật khẩu của bạn là: <strong>{newCode}</strong>. Vui lòng nhập mã này trong vòng 3 phút.";

                    var result = await _sendMailService.SetUpSendMail(to, subject, fullBody);
                    if (result.success == true)
                    {
                        return new Success { success = true, message = $"{to}|{Id}" };
                    }
                    return new Success { success = false, message = "Lỗi khi gửi mail" };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi " + ex.Message, ex);
            }

        }

        private string GenerateOtp()
        {
            var verificationCode = new Random().Next(100000, 999999).ToString();
            return verificationCode;
        }

        public async Task<Success> ResetPasswordAsync(ResetPasswordModel resetPassword)
        {
            if (_memoryCache.TryGetValue($"resetToken:{resetPassword.userId}", out string storedToken))
            {
                if (storedToken == resetPassword.resetToken)
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
            return new Success { success = true };
        }

        public async Task<Success> CheckOtpCode(VerifyCodeDto verify)
        {
            // Kiểm tra mã xác nhận trong MemoryCache
            if (_memoryCache.TryGetValue($"verify:{verify.EmailUser}", out string storedCode))
            {
                if (storedCode == verify.Code)
                {
                    var user = await _userManager.FindByEmailAsync(verify.EmailUser);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }

                    var token = Guid.NewGuid().ToString();
                    _memoryCache.Set($"resetToken:{user.Id}", token, TimeSpan.FromMinutes(5));
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
