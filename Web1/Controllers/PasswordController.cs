using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Models;
using Web1.Service;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPassword;

        public PasswordController(IForgotPasswordService forgotPassword) 
        {
            _forgotPassword = forgotPassword;
        }

        [HttpPost("SendOtpPassword/{userNameOrEmail}")]
        public async Task<IActionResult> SendOtpPassword(string userNameOrEmail)
        {
            var emailUser = await _forgotPassword.SendOtpResetPassword(userNameOrEmail);
            return Ok(emailUser);
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
