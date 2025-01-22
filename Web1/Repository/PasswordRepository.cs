
using Microsoft.AspNetCore.Identity;
using Web1.Models;

namespace Web1.Repository
{
    public class PasswordRepository: IPasswordRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public PasswordRepository(UserManager<AppUser> userManager) 
        {
            _userManager = userManager;
        }

        public async Task<string> GetPassword(string email, string userName)
        {
            if (userName != null)
            {
                var user = await _userManager.FindByNameAsync(userName);
            }

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                
            }
            return null;

            
        }

        public Task UpdatePassword(string email, string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
