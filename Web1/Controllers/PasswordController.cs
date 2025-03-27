using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Helps;
using Web1.Models;
using Web1.Service.Account;
using Web1.Service.RabbitMq.Producer;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPassword;
        private readonly IRabbitMqProducer _producerService;

        public PasswordController(IForgotPasswordService forgotPassword,
                                  IRabbitMqProducer producerService) 
        {
            _forgotPassword = forgotPassword;
            _producerService = producerService;
        }

        [HttpPost("SendOtpPassword/{userNameOrEmail}")]
        public async Task<IActionResult> SendOtpPassword(string userNameOrEmail)
        {
            var user = await _forgotPassword.GetUserPassword(userNameOrEmail);
            await _producerService.PublishEvent(KeyRabbit.FORGOT_PASSWORD_ROUTING, user);
            return Ok(new Success { success = true, message = $"{user.Email}|{user.Id}" });
        }

        [HttpPost("check-verify-code-password")]
        public async Task<IActionResult> VerifyCodeFromUser(VerifyCodeDto verify)
        {
            var result = await _forgotPassword.CheckOtpCode(verify);
            return Ok(result);
        }

        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel resetPassword)
        {
            var result = await _forgotPassword.ResetPasswordAsync(resetPassword);
            return Ok(result);
        }

        [HttpPost("Update-password")]
        public async Task<IActionResult> UpdatePassword(ResetPasswordDto resetPassword)
        {
            var result = await _forgotPassword.UpdatePasswordOldAsync(resetPassword);
            return Ok(result);
        }
    }
}
