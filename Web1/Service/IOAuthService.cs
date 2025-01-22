using Google.Apis.Auth;
using Web1.Models;

namespace Web1.Service
{
    public interface IOAuthService
    {
        public Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string token);

        public Task AuthenFacebook();

        public Task<LoginDto> ExternalLoginGoogle(GoogleJsonWebSignature.Payload payload, string token);
    }
}
