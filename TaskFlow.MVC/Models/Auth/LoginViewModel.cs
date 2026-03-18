using System.ComponentModel.DataAnnotations;

namespace TaskFlow.MVC.Models.Auth
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";
    }
}
