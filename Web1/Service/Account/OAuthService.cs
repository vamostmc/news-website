using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using Web1.Helps;
using Web1.Models;


namespace Web1.Service.Account
{
    public class OAuthService : IOAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IAccountService _accountService;


        public OAuthService(
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            HttpClient httpClient,
            IAccountService accountService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpClient = httpClient;
            _accountService = accountService;
        }

        public Task AuthenFacebook()
        {
            throw new NotImplementedException();
        }


        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _configuration["OAuth:Google:ClientId"] }  // Lấy từ appsettings.json
            };

            // Log kiểm tra Audience
            if (settings.Audience == null)
            {
                Console.WriteLine("Warning: Audience is null or empty!");
            }
            else
            {
                Console.WriteLine("Audience values:");
                foreach (var aud in settings.Audience)
                {
                    Console.WriteLine($" - {aud}");
                }
            }

            // Log thời gian backend hiện tại
            Console.WriteLine($"Backend time (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

            // Decode token trước để log thông tin dù token expired
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Lấy claim exp
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                DateTime? expTimeUtc = null;
                if (expClaim != null && long.TryParse(expClaim, out var expSeconds))
                {
                    expTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                }

                Console.WriteLine($"Token expires at (UTC): {expTimeUtc:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Payload decoded (pre-verify): Sub={jwtToken.Subject}, Email={jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: Failed to decode token before verify: " + ex.Message);
            }

            // Thực hiện verify chính thức
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                Console.WriteLine($"Token verified successfully: Email={payload.Email}, Sub={payload.Subject}");
                return payload;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error during verify: " + ex.Message);
                return null;
            }
        }

        public async Task<LoginDto> ExternalLoginGoogle(GoogleJsonWebSignature.Payload payload, string token)
        {
            Console.WriteLine($"ExternalLoginGoogle called for Email={payload.Email}, Sub={payload.Subject}");

            var user = await _userManager.FindByEmailAsync(payload.Email);

            Console.WriteLine(user == null ? "User not found, creating new." : $"User exists. UserId={user.Id}");

            var (address, birthday) = await GetUserInfoAsync(token);
            Console.WriteLine($"User info from Google People API: Address={address}, Birthday={birthday}");

            if (user == null)
            {
                var newUser = new AppUser
                {
                    Email = payload.Email,
                    FullName = payload.Name,
                    IsActive = true,
                    EmailConfirmed = true,
                    UserName = payload.Email,
                    Address = address,
                    CreationDate = DateTime.UtcNow,
                    DateUser = DateTime.TryParse(birthday, out var parsedBirthday) ? parsedBirthday : DateTime.MinValue,
                };

                var checkCreate = await _userManager.CreateAsync(newUser);
                if (!checkCreate.Succeeded)
                {
                    Console.WriteLine("Failed to create new user.");
                    return new LoginDto { Success = false };
                }

                await _userManager.AddToRoleAsync(newUser, Role.Customer);
                var userLoginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
                await _userManager.AddLoginAsync(newUser, userLoginInfo);

                Console.WriteLine("New user created successfully.");
                return await CreateExternalToken(newUser);
            }
            else
            {
                var userLogin = await _userManager.FindByLoginAsync(LoginProvider.Google, payload.Subject);
                if (userLogin == null)
                {
                    Console.WriteLine("User exists but no Google login linked.");
                    return new LoginDto { Success = false };
                }

                Console.WriteLine("User exists and Google login linked. Creating token...");
                return await CreateExternalToken(user);
            }
        }


        private async Task<LoginDto> CreateExternalToken(AppUser user)
        {
            var accessToken = await _accountService.CreateAccessToken(user);
            var refreshToken = await _accountService.CreateRefreshToken(user, TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(7)));
            var roles = await _userManager.GetRolesAsync(user);
            var repo = new LoginDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                RoleList = roles.ToList(),
                ConfirmEmail = user.EmailConfirmed,
                Token = new AuthenticateModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                },
                Success = true
            };
            return repo;
        }

        private async Task<(string Address, string Birthday)> GetUserInfoAsync(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Gọi API lấy thông tin địa chỉ và ngày sinh nhật
                var response = await client.GetAsync("https://people.googleapis.com/v1/people/me?personFields=addresses,birthdays");
                if (!response.IsSuccessStatusCode)
                {
                    return ("No Address Found", null);
                }

                var content = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(content);

                // Lấy địa chỉ
                var address = result?.addresses?[0]?.formattedValue ?? "No Address Found";

                // Lấy ngày sinh nhật
                var birthdayData = result?.birthdays?[0]?.date;
                string birthday = birthdayData != null
                    ? $"{birthdayData.month}/{birthdayData.day}/{birthdayData.year}"
                    : "No Birthday Found";

                return (address, birthday);
            }
        }


    }
}
