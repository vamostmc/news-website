using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Web1.Helps;
using Web1.Models;


namespace Web1.Service
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

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch
            {
                return null; // Token không hợp lệ
            }
        }

        public async Task<LoginDto> ExternalLoginGoogle(GoogleJsonWebSignature.Payload payload, string token) 
        {
            var user = await _userManager.FindByEmailAsync(payload.Email);
            var (address, birthday) = await GetUserInfoAsync(token);
            if (user == null)
            {
                // Nếu người dùng chưa từng đăng nhập
                var NewUser = new AppUser
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

                var check = await _userManager.CreateAsync(NewUser);
                if (!check.Succeeded) 
                {
                    return new LoginDto { Success = false };
                }
                await _userManager.AddToRoleAsync(NewUser,Role.Customer);
                var userLoginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
                await _userManager.AddLoginAsync(NewUser, userLoginInfo);
                return await CreateExternalToken(NewUser); 
            }
            else    // Nếu người dùng tồn tại
            {
                // Nếu người dùng từng đăng kí qua google trước đó lưu vào bảng UserLogin
                var userLogin = await _userManager.FindByLoginAsync(LoginProvider.Google, payload.Subject);
                if (userLogin == null)
                {
                    return new LoginDto { Success = false };
                }
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
