
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using Web1.Data;
using Web1.Exceptions;
using Web1.Helps;
using Web1.Migrations.TinTucDb;
using Web1.Models;

namespace Web1.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TinTucDbContext _tinTuc;

        public CustomerRepository( UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, TinTucDbContext tinTuc) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tinTuc = tinTuc;
        }
        public async Task<int> TotalCustoms()
        {
            // Lấy vai trò Customer
            var role = await _roleManager.FindByNameAsync("Customer");

            if (role == null)
            {
                return 0; // Nếu không có vai trò Customer
            }

            // Lấy danh sách người dùng có vai trò Customer
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            return usersInRole.Count();
        }

        public async Task<List<AppUserDto>> GetListUser()
        {
            var usersInRoleList = new List<AppUserDto>();
            var users = _userManager.Users.ToList(); // Lấy danh sách người dùng

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Lấy danh sách vai trò của người dùng

                var userInRole = new AppUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Address = user.Address,
                    CreationDate = user.CreationDate,
                    DateUser = user.DateUser,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserRoleList = roles.ToList(),
                };

                usersInRoleList.Add(userInRole); 
            }

            return usersInRoleList; 
        }

        public async Task<AppUserDto> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user == null)
                {
                    throw new RepositoryException("User not found.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userInRole = new AppUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Address = user.Address,
                    CreationDate = user.CreationDate,
                    DateUser = user.DateUser,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserRoleList = roles.ToList(),
                };

                return userInRole;

            }
            catch (RepositoryException ex) 
            {
                throw new RepositoryException("not found.");
            }
        }



        public async Task<AppUser> AddUserNew(Register user)
        {
            var data = new AppUser
            {
                Address = user.Address,
                FullName = user.FullName,
                UserName = user.UserName,
                DateUser = user.DateUser,
                IsActive = true,
                CreationDate = LocalTime.GetLocalTime()
            };

            if(user.UserRole == Role.Customer)
            {
                user.UserRole = Role.Customer;
            }

            if (user.UserRole == Role.Manager)
            {
                user.UserRole = Role.Manager;
            }

            var UserNew = await _userManager.CreateAsync(data, user.Password);

            if(UserNew.Succeeded)
            {
                await _userManager.AddToRoleAsync(data, user.UserRole);
            }

            return data;
        }

        public async Task<AppUser> UpdateUserAsync(string id, AppUserDto user)
        {
            try
            {
                var data = await _userManager.FindByIdAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("User not found.");
                }

                //Lấy ra tất cả vai trò của người dùng hiện tại
                var userInRole = await _userManager.GetRolesAsync(data);

                if (userInRole != null && userInRole.Count > 0)
                {
                    // Xóa tất cả các vai trò của người dùng
                    await _userManager.RemoveFromRolesAsync(data, userInRole);
                }

                //Cập nhật lại vai trò mới cho user
                if (user.UserRoleList != null && user.UserRoleList.Count > 0)
                {
                    // Thêm người dùng vào nhiều vai trò
                    var result = await _userManager.AddToRolesAsync(data, user.UserRoleList);
                }

                data.Address = user.Address;
                data.FullName = user.FullName;
                data.UserName = user.UserName;
                data.DateUser = user.DateUser;
                data.IsActive = user.IsActive;

                await _userManager.UpdateAsync(data);
                return data;
                
            }
            catch (RepositoryException ex)
            {
                throw new RepositoryException("Lỗi không thể sửa",ex);
            }
        }

        public async Task<AppUser> DeleteUserAsync(string id)
        {
            try
            {
                //Tìm những bài bình luận người dùng này đã bình luận
                var UserInBinhLuan = _tinTuc.BinhLuans.Where(t => t.UserId == id);
                if(UserInBinhLuan != null)
                {
                    foreach (var item in UserInBinhLuan)
                    {
                        //Set cho user là null để thành tài khoản bị xóa
                        item.UserId = null;
                    }
                }

                await _tinTuc.SaveChangesAsync();

                var data = await _userManager.FindByIdAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("User not found.");
                }

                await _userManager.DeleteAsync(data);
                return data;
            }
            catch (RepositoryException ex)
            {
                throw new RepositoryException("Lỗi không thể xóa User", ex);
            }
        }

        public async Task<List<AppUserDto>> UpdateStatusAsync(string id ,bool status)
        {
            try
            {
                var data = await _userManager.FindByIdAsync(id);
                if (data == null)
                {
                    throw new RepositoryException("User not found.");
                }

                data.IsActive = status;
                await _userManager.UpdateAsync(data);
                return await GetListUser();
            }
            catch (RepositoryException ex)
            {
                throw new Exception("Lỗi không thể xóa User", ex);
            }
        }
    }
}
