namespace Web1.Service.Cookie
{
    public interface ICookieService
    {
        public Task SetCookie(DateTime dateTime, string token);
    }
}
