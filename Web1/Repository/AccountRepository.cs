using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web1.Helps;
using Web1.Models;

namespace Web1.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountRepository(
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                IConfiguration configuration,
                IHttpContextAccessor httpContext,
                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
            _httpContext = httpContext;
            _roleManager = roleManager;
        }

        public async Task<string> LoginJWTAsync(Login login)
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

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return "Error";
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

        public Task<string> PasswordReset(string email)
        {
            throw new NotImplementedException();
        }
    }
}
