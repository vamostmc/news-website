using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web1.Helps;
using Web1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Web1.Service;
using Azure.Core;

namespace Web1.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountService _accountService;

        public AccountRepository(
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                IConfiguration configuration,
                IHttpContextAccessor httpContext,
                RoleManager<IdentityRole> roleManager,
                
                IAccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
            _httpContext = httpContext;
            _roleManager = roleManager;
            _accountService = accountService;
        }

        public async Task<LoginDto> LoginJWTAsync(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            var result = await _userManager.CheckPasswordAsync(user, login.Password);

            string refreshToken;

            if (result == true)
            {
                var accessToken = await _accountService.CreateAccessToken(user);
                if (login.Remember == true)
                {
                    refreshToken = await _accountService.CreateRefreshToken(user, TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(7)));
                }
                else 
                {
                    refreshToken = await _accountService.CreateRefreshToken(user, TimeZoneInfo.ConvertTimeToUtc(LocalTime.GetLocalTime().AddDays(1)));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var repo = new LoginDto
                {
                    UserId = user.Id,
                    UserName = login.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    ConfirmEmail = user.EmailConfirmed,
                    RoleList = roles.ToList(),
                    Token = new AuthenticateModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                    },
                    Success = true
                };
                return repo;
            }
            return new LoginDto { Success = false};
        }


        public async Task<string> LoginCookeAsync(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            var result = await _userManager.CheckPasswordAsync(user, login.Password);

            if (result == true)
            {
                var Claims = new List<Claim>
                {
                    // Dinh danh nguoi dung user co ten duy nhat 
                    new Claim(ClaimTypes.Name, login.UserName),   
                
                    // Tao ra chuoi token co ngau nhien va duy nhat tu Guid
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
                    expires: DateTime.Now.AddMinutes(20),
                    claims: Claims,
                    signingCredentials: credentials
                );

                // Tạo ClaimsIdentity
                //var claimsIdentity = new ClaimsIdentity(Claims, "CookieAuth");

                //// Tạo AuthenticationProperties cho cookie
                //AuthenticationProperties authProperties = new AuthenticationProperties();
                //if(login.Remember)
                //{
                //    authProperties.IsPersistent = true; // Lưu cookie lâu dài nếu "Remember Me" được chọn
                //    authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(login.Remember ? 14 : 1); // Cookie hết hạn sau 14 ngày nếu "Remember Me"
                //                                                                                        // Tạo cookie cho người dùng
                //    await _httpContext.HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
                //};

                //_httpContext.HttpContext.Response.Cookies.Append("AuthToken", tokenUser, new CookieOptions
                //{
                //    HttpOnly = true,
                //    Secure = true,
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                //});

                var refreshToken = Guid.NewGuid().ToString();

                //Chuoi string cua token tao duoc
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Lưu token vào cookie
                 _httpContext.HttpContext.Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                });

                return tokenString;
            }

            return "Error";
        }

        public async Task<IdentityResult> RegisterAsync(Register register)
        {
            var registerUser = new AppUser
            {
                UserName = register.UserName,
                CreationDate = LocalTime.GetLocalTime(),
                FullName = register.FullName,
                DateUser = register.DateUser,
                Address = register.Address,
                Email = register.Email,
                IsActive = true,
            };

            var result = await _userManager.CreateAsync(registerUser, register.Password);

            if (result.Succeeded)
            {
                //Kiem tra xem vai tro Customer co trong bang Role trong Database chua
                if (!await _roleManager.RoleExistsAsync(Role.Customer))
                {
                    //Neu chua, thi add them vai tro Role moi nay vao bang
                    await _roleManager.CreateAsync(new IdentityRole(Role.Customer));
                }

                //Gan vai tro cho user
                 await _userManager.AddToRoleAsync(registerUser, Role.Customer);
            }

            return result;
        }

        
    }
}
