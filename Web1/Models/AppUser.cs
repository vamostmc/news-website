using Microsoft.AspNetCore.Identity;
using Web1.Data;

namespace Web1.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime DateUser { get; set; }

        public string Address { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? FullName { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();
    }
}
