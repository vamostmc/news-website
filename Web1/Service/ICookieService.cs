namespace Web1.Service
{
    public interface ICookieService
    {
        public Task SetCookie(DateTime dateTime, string token);
    }
}
