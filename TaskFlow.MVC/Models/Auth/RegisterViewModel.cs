using System.ComponentModel.DataAnnotations;

namespace TaskFlow.MVC.Models.Auth
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public string Role { get; set; } = "User";
    }
}
