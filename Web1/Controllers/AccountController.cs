using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Models;
using Web1.Repository;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _account;

        public AccountController(IAccountRepository account) 
        {
            _account = account;
        }

        [HttpPost("Log-In-JWT")]
        public async Task<IActionResult> LogInJWT(Login login)
        {
            var result = await _account.LoginJWTAsync(login);
            return Ok(result);
        }

        [HttpPost("Log-In-cookie")]
        public async Task<IActionResult> LogInCookie(Login login)
        {
            var result = await _account.LoginCookeAsync(login);
            return Ok(result);
        }

        [HttpGet("Unauthorized")]
        public IActionResult GetUnauthorized()
        { 
            return Unauthorized();
        }

        [HttpPost("Sign-Up")]
        public async Task<IActionResult> LogUp(Register register)
        {
            var result = await _account.RegisterAsync(register);
            return Ok(result);
            
        }
    }
}
