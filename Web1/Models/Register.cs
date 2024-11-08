using System.ComponentModel.DataAnnotations;

namespace Web1.Models
{
    public class Register
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        public DateTime DateUser { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required] 
        public string Email { get; set;}

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        public string? UserRole { get; set; }
    }
}
