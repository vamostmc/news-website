namespace Web1.Models
{
    public class LoginDto
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool ConfirmEmail { get; set; }

        public AuthenticateModel Token { get; set; }

        public List<string> RoleList { get; set; }

        public bool Success { get; set; }
    }
}
