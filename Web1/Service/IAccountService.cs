using Web1.Models;

namespace Web1.Service
{
    public interface IAccountService
    {
        public Task<string> CreateRefreshToken(AppUser user , DateTime dateTime);

        public Task<string> CreateAccessToken(AppUser user);

        public Task<AuthenticateModel> ResetRefreshToken(AppUser user, AuthenticateModel authenticate);

        public Task RemoveRefreshToken(AppUser user, string refreshToken);

        public Task<bool> IsRefreshTokenExpired(AppUser user, string refreshToken);

        public Task<bool> IsAccessTokenExpired(AppUser user, string accessToken);

        public Task<bool> IsUserToken(AppUser userInput, string accessToken);

        public Task<bool> CheckRoleUser(string accessToken);

        public Task<AppUser> GetInfo(string token);
    }
}
