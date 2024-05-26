using System.ComponentModel.DataAnnotations;

namespace Data.DataViewModel.System
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
