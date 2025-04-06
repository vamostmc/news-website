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
using Web1.Helps;
using Web1.Models;
using Web1.Service.Redis;

namespace Web1.Service.Mail
{
    public class ConfirmMailService : IConfirmMailService
    {
        private readonly MailSetting _mailSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendMailService _sendMailService;
        private readonly IRedisService _redisService;

        public ConfirmMailService(
            IOptions<MailSetting> mailSettings,
            IMemoryCache memoryCache,
            UserManager<AppUser> userManager,
            ISendMailService sendMailService,
            IRedisService redisService)
        {
            _mailSettings = mailSettings.Value;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _sendMailService = sendMailService;
            _redisService = redisService;
        }

        public async Task<AppUser> GetInfoUserMail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task SendVerifyEmailAsync(AppUser user)
        {
            var to = user.Email;
            var newCode = GenerateOtp();
            await _redisService.SetStringRedisAsync(TypeKeyRedis.CONFIRM_EMAIL_PREFIX, to, newCode, TimeSpan.FromMinutes(5));

            string subject = "Mã xác nhận Email";
            string fullBody = $"<br>Mã xác nhận của bạn là: <strong>{newCode}</strong>. Vui lòng nhập mã này trong vòng 3 phút.";

            var result = await _sendMailService.SetUpSendMail(to, subject, fullBody);
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

        public async Task<string> CheckVerifyCode(VerifyCodeDto verify)
        {
            var value = await _redisService.GetStringRedisAsync(TypeKeyRedis.CONFIRM_EMAIL_PREFIX, verify.EmailUser);
            if (value != string.Empty)
            {
                if (value == verify.Code)
                {
                    var user = await _userManager.FindByEmailAsync(verify.EmailUser);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                        await _redisService.RemoveToRedisAsync(TypeKeyRedis.CONFIRM_EMAIL_PREFIX, verify.EmailUser);
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
