using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Service.Mail;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMailController : ControllerBase
    {
        private readonly SendMailService _sendMailService;

        public SendMailController(SendMailService sendMailService) 
        {
            _sendMailService = sendMailService;

        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(string to, string subject, string body)
        {
            try
            {
                await _sendMailService.SetUpSendMail(to, subject, body);
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending email: {ex.Message}");
            }
        }

        
    }
}
