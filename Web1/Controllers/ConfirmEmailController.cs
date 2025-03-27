using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Web1.Helps;
using Web1.Models;
using Web1.Service.Mail;
using Web1.Service.RabbitMq.Producer;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly IConfirmMailService _confirmMailService;
        private readonly IRabbitMqProducer _producerService;

        public ConfirmEmailController(IConfirmMailService confirmMailService, 
                                      IRabbitMqProducer producerService) 
        {
            _confirmMailService = confirmMailService;
            _producerService = producerService;
        }

        [HttpPost("send-verify-mail/{Id}")]
        public async Task<IActionResult> SendVerifyEmail(string Id)
        {
            var user = await _confirmMailService.GetInfoUserMail(Id);
            await _producerService.PublishEvent(KeyRabbit.CONFIRM_EMAIL_ROUTING, user);
            return Ok(new Success { success = true, message = user.Email});
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
