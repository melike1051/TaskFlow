using System.ComponentModel.DataAnnotations;

namespace TaskFlow.MVC.Models.Users
{
    public class UpdateUserRoleViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Role { get; set; } = "User";
    }
}
