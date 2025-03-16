using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Models;
using Web1.Service.Mail;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly IConfirmMailService _confirmMailService;

        public ConfirmEmailController(IConfirmMailService confirmMailService) 
        {
            _confirmMailService = confirmMailService;
        }

        [HttpPost("send-verify-mail/{Id}")]
        public async Task<IActionResult> SendVerifyEmail(string Id)
        {
            var result = await _confirmMailService.SendVerifyEmailAsync(Id);
            return Ok(result);
        }

        [HttpPost("check-verify-code")]
        public async Task<IActionResult> VerifyCodeFromUser(VerifyCodeDto verify)
        {
            var result = await _confirmMailService.CheckVerifyCode(verify);
            return result switch
            {
                "valid" => Ok(new { success = true } ) ,
                "expired" => Ok(new { success = false, message = "Expired" }),
                _ => Ok(new { success = false, message = "Invalid" })
            };
        }
    }
}
