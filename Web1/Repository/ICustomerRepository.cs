using Web1.Models;

namespace Web1.Repository
{
    public interface ICustomerRepository
    {
        public Task<int> TotalCustoms();

        public Task<List<AppUserDto>> GetListUser();

        public Task<AppUserDto> GetUserById(string id);

        public Task<AppUser> AddUserNew(Register user);

        public Task<AppUser> UpdateUserAsync(string id, AppUserDto user);

        public Task<AppUser> DeleteUserAsync(string id);

        public Task<List<AppUserDto>> UpdateStatusAsync(string id, bool status);
    }
}
