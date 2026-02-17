using System.ComponentModel.DataAnnotations;

namespace TaskFlow.MVC.Models.Tasks
{
    public class TaskCreateViewModel
    {
        [Required]
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int Status { get; set; }
    }
}
