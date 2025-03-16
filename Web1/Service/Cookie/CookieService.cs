
using Azure;

namespace Web1.Service.Cookie
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Task SetCookie(DateTime dateTime, string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = dateTime // Đặt thời gian hết hạn trong quá khứ
            };

            var response = _httpContextAccessor.HttpContext?.Response;

            if (response != null)
            {
                response.Cookies.Append("refreshToken", token, cookieOptions);
            }

            return Task.CompletedTask;
        }
    }
}
