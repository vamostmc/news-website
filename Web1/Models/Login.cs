using Microsoft.Build.Framework;

namespace Web1.Models
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        
        public bool Remember {  get; set; }
    }
}
