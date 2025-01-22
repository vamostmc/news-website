using Microsoft.AspNetCore.Identity;
using Web1.Models;

namespace Web1.Repository
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> RegisterAsync(Register register);

        public Task<LoginDto> LoginJWTAsync(Login login);

        public Task<string> LoginCookeAsync(Login login);

    }
}
