using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using Web1.Helps;
using Web1.Models;
using Web1.Repository;
using Web1.Service.Account;
using Web1.Service.Cookie;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _account;
        private readonly IConfiguration _configuration;
        private readonly IOAuthService _oAuthService;
        private readonly IAccountService _accountService;
        private readonly ICookieService _cookieService;

        public AccountController(
            IAccountRepository account,
            IConfiguration configuration,
            IOAuthService oAuthService,
            IAccountService accountService,
            ICookieService cookieService
            ) 
        {
            _account = account;
            _configuration = configuration;
            _oAuthService = oAuthService;
            _accountService = accountService;
            _cookieService = cookieService;
        }

        [HttpPost("Log-In-JWT")]
        public async Task<IActionResult> LogInJWT(Login login)
        {
            var result = await _account.LoginJWTAsync(login);
            if (result?.Token?.RefreshToken != null)
            {
                await _cookieService.SetCookie(TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(7)),
                result.Token.RefreshToken);
            }
            return Ok(result);
        }

        [HttpPost("Log-In-cookie")]
        public async Task<IActionResult> LogInCookie(Login login)
        {
            var result = await _account.LoginCookeAsync(login);
            return Ok(result);
        }

        [HttpPost("Login-Google")]
        public async Task<LoginDto> LoginGoogle([FromBody] GoogleRequest request)
        {
            Console.WriteLine("Login-Google called with token length: " + (request?.Token?.Length ?? 0));

            var payload = await _oAuthService.VerifyGoogleToken(request.Token);
            if (payload == null)
            {
                Console.WriteLine("VerifyGoogleToken returned null. Token invalid or expired.");
                return new LoginDto { Success = false };
            }

            Console.WriteLine($"VerifyGoogleToken succeeded. Email: {payload.Email}, Name: {payload.Name}, Sub: {payload.Subject}");

            var check = await _oAuthService.ExternalLoginGoogle(payload, request.Token);

            if (check?.Token?.RefreshToken != null)
            {
                Console.WriteLine("Refresh token created, setting cookie...");
                await _cookieService.SetCookie(
                    TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(7)),
                    check.Token.RefreshToken
                );
            }

            Console.WriteLine("ExternalLoginGoogle result Success: " + check?.Success);
            return check;
        }


        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            // Nếu không có refresh token → coi như đã logout
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Ok(new { message = "Already logged out" });
            }

            string? accessToken = null;
            var authorizationHeader = Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader) &&
                authorizationHeader.StartsWith("Bearer "))
            {
                accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                var user = await _accountService.GetInfo(accessToken);
                if (user != null)
                {
                    await _accountService.RemoveRefreshToken(user, refreshToken);
                }
            }

            // Xóa cookie luôn
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logout success" });
        }



        [HttpGet("Unauthorized")]
        public IActionResult GetUnauthorized()
        { 
            return Unauthorized();
        }


        [HttpPost("Sign-Up")]
        public async Task<IActionResult> LogUp([FromForm] Register register)
        {
            var result = await _account.RegisterAsync(register);

            if (result.Succeeded)
            {
                // Trả về kết quả thành công dưới dạng chuỗi "true"
                return Ok(new { succeeded = true, errors = new string[] { } });
            }
            else
            {
                // Trả về các lỗi dưới dạng chuỗi "false"
                return Ok(new { succeeded = false, errors = result.Errors.Select(e => e.Description).ToArray() });
            }
        }


        [HttpGet("send-cookie")]
        public async Task<IActionResult> SendCookie()
        {
            // Tạo các claim cho người dùng
            var claims = new List<Claim>
    {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Email, "testuser@example.com")
    };

            // Tạo ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

            // Cấu hình các thuộc tính của cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,                 // Cookie sẽ tồn tại sau khi đóng trình duyệt
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Cookie có hiệu lực trong 7 ngày
            };

            // Đăng nhập và gửi cookie
                await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return Ok(new { message = "Cookie sent successfully with authentication!" });
        }


        [HttpGet("check-header")]
        public IActionResult CheckHeader()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            return Ok(new { message = "Đã nhận Header", refreshToken, authorizationHeader });
        }

        [HttpGet("RefreshToken")]
        public async Task<string> RefreshToken()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return null;
            }

            var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            var authenToken = new AuthenticateModel { AccessToken = accessToken, RefreshToken = refreshToken };

            var user = await _accountService.GetInfo(accessToken);
            var data = await _accountService.ResetRefreshToken(user, authenToken);
            if (data?.RefreshToken != null)
            {
                await _cookieService.SetCookie(TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(7)),
                data.RefreshToken);
            }

            return data.AccessToken;
        }

        [HttpGet("Check-Admin")]
        public async Task<Success> CheckAdmin()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return new Success { success = false, message = "Không có accessToken" };
            }

            var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            var checkRole = await _accountService.CheckRoleUser(accessToken);
            if(checkRole == true)
            {
                return new Success { success = true };
            }
            return new Success { success = false };
        }

    }
}
