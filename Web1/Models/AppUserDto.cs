using Web1.Data;

namespace Web1.Models
{
    public class AppUserDto 
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public DateTime DateUser { get; set; }

        public string Address { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Email { get; set; }

        public string? FullName { get; set; }

        public bool IsActive { get; set; }

        public List<string>? UserRoleList { get; set; }

        public string? UserRole { get; set; }
    }
}
