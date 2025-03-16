
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web1.Helps;
using Web1.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Web1.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AccountService(
            UserManager<AppUser> userManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _config = configuration;
        }

        public async Task<string> CreateAccessToken(AppUser user)
        {
            var Claims = new List<Claim>
                {
                    // Dinh danh nguoi dung user co ten duy nhat 
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            //Lay tat ca ra cac vai tro ma user co
            var roleUser = await _userManager.GetRolesAsync(user);
            foreach (var roles in roleUser)
            {
                Claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }

            // Ma hoa theo Key nay
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            // Thuat toan su dung
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            //Tao token
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires: TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddMinutes(5)),
                claims: Claims,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Tạo mới refreshToken
        public async Task<string> CreateRefreshToken(AppUser user, DateTime dateTime)
        {
            string RefreshToken = Guid.NewGuid().ToString();
            string expirationDateString = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            await _userManager.SetAuthenticationTokenAsync(user, LoginProvider.Local, RefreshToken, expirationDateString);
            return RefreshToken;
        }

        public async Task RemoveRefreshToken(AppUser user, string refreshToken)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, LoginProvider.Local, refreshToken);
        }

        // Trường hợp refreshToken và accessToken được truyền từ Client hết hạn
        public async Task<AuthenticateModel> ResetRefreshToken(AppUser user, AuthenticateModel authenticate)
        {
            var checkUser = await IsUserToken(user, authenticate.AccessToken);
            if (checkUser == false)
            {
                throw new Exception("Xác thực người dùng không dúng");
            }
            var TimeRefreshToken = await IsRefreshTokenExpired(user, authenticate.RefreshToken);
            var TimeAccessToken = await IsAccessTokenExpired(user, authenticate.AccessToken);
            // RefreshToken hoặc AccessToken hết hạn
            if (TimeRefreshToken == true || TimeAccessToken == true)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, LoginProvider.Local, authenticate.RefreshToken);
                var accessToken = await CreateAccessToken(user);
                var refreshToken = await CreateRefreshToken(user, TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(7)));
                return new AuthenticateModel { AccessToken = accessToken, RefreshToken = refreshToken };
            }
            return authenticate;
        }

        public async Task<bool> IsAccessTokenExpired(AppUser user, string accessToken)
        {
            // Giải mã và kiểm tra thời gian hết hạn của Access Token
            if (string.IsNullOrEmpty(accessToken))
            {
                return true;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

            if (jwtToken != null)
            {
                var exp = jwtToken?.Claims?.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (exp != null)
                {
                    var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).DateTime;
                    return expiryDate < DateTime.UtcNow;
                }
            }

            return true; // Nếu không thể giải mã, coi như hết hạn
        }


        public async Task<bool> IsRefreshTokenExpired(AppUser user, string refreshToken)
        {
            try
            {
                var expirationDateString = await _userManager.GetAuthenticationTokenAsync(user, "Local", refreshToken);

                // Nếu không tìm thấy thời gian hết hạn, trả về false (token chưa hết hạn)
                if (string.IsNullOrEmpty(expirationDateString))
                {
                    return true;
                }

                DateTime expirationDate = DateTime.ParseExact(expirationDateString, "yyyy-MM-dd HH:mm:ss", null);

                return DateTime.UtcNow > expirationDate;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi " + ex.Message, ex);
            }
        }

        public async Task<bool> IsUserToken(AppUser userInput, string accessToken)
        {
            var UserToken = await GetInfo(accessToken);
            if (UserToken == null)
            {
                throw new Exception("Lỗi không tìm được người dùng từ accessToken");
            }

            if (userInput.Id == UserToken.Id) { return true; }
            return false;
        }

        public async Task<bool> CheckRoleUser(string accessToken)
        {
            var user = await GetInfo(accessToken);
            if (user == null) { return false; }
            var roleUser = await _userManager.GetRolesAsync(user);
            if (roleUser.Contains(Role.Manager))
            {
                return true;
            }
            return false;
        }

        public async Task<AppUser> GetInfo(string token)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                throw new Exception("Lỗi Claim không xác định");
            }
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception("Lỗi không tìm thấy UserID từ Claim");
            }

            var user = await _userManager.FindByIdAsync(userId);
            return user;

        }

        // Giải mã token
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"])),
                ValidateLifetime = false // Cho phép token đã hết hạn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
