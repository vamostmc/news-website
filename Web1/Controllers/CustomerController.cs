using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Models;
using Web1.Repository;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase 
    {
        private readonly ICustomerRepository _customer;

        public CustomerController( ICustomerRepository customer) { _customer = customer; }

        [HttpGet("TongKH")]
        public async Task<int> CountCustomer()
        {
            return await _customer.TotalCustoms();
        }

        [HttpGet("DsachKH")]
        public async Task<List<AppUserDto>> GetListCustomer()
        {
            return await _customer.GetListUser();
        }

        [HttpGet]
        [Route("GetUserId/{id}")]
        public Task<AppUserDto> GetUserId(string id)
        {
            return _customer.GetUserById(id);
        }

        [HttpPost]
        [Route("AddUser")]
        public Task<AppUser> AddUser([FromForm] Register newUser)
        {
            return _customer.AddUserNew(newUser);
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public Task<AppUser> DeleteUser(string id)
        {
            return _customer.DeleteUserAsync(id);
        }

        [HttpPut]
        [Route("EditUser/{id}")]
        public Task<AppUser> UpdateUser(string id, [FromForm] AppUserDto newUser)
        {
            return _customer.UpdateUserAsync(id, newUser);
        }

        [HttpPut]
        [Route("StatusUser/{id}")]
        public Task<List<AppUserDto>> UpdateStatus(string id, [FromBody] bool status)
        {
            return _customer.UpdateStatusAsync(id, status);
        }

    }
}
