using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using StackExchange.Redis;
using Web1.Models;

namespace Web1.Service.Mail
{
    public class ConfirmMailService : IConfirmMailService
    {
        private readonly MailSetting _mailSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendMailService _sendMailService;

        public ConfirmMailService(
            IOptions<MailSetting> mailSettings,
            IMemoryCache memoryCache,
            UserManager<AppUser> userManager,
            ISendMailService sendMailService)
        {
            _mailSettings = mailSettings.Value;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _sendMailService = sendMailService;
        }

        public async Task<Success> SendVerifyEmailAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return new Success { success = false, message = "Lỗi người dùng không tồn tại" };
            }

            var to = user.Email;
            var newCode = GenerateOtp();
            _memoryCache.Set($"verify:{to}", newCode, TimeSpan.FromMinutes(5));

            string subject = "Mã xác nhận Email";
            string fullBody = $"<br>Mã xác nhận của bạn là: <strong>{newCode}</strong>. Vui lòng nhập mã này trong vòng 3 phút.";

            var result = await _sendMailService.SetUpSendMail(to, subject, fullBody);
            if (result.success == true)
            {
                return new Success { success = true, message = $"{to}" };
            }
            return new Success { success = false, message = "Lỗi khi gửi mail" };
        }

        private string GenerateOtp()
        {
            var verificationCode = new Random().Next(100000, 999999).ToString();
            return verificationCode;
        }

        public async Task<string> CheckVerifyCode(VerifyCodeDto verify)
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
                    return "valid";
                }
                else
                {
                    return "invalid";
                }

            }
            else
            {
                // Nếu mã không tồn tại hoặc đã hết hạn
                return "expired";
            }
        }

    }
}
