namespace TaskFlow.MVC.Models.Tasks
{
    public class AdminTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
        public UserDto User { get; set; } = new();
    }

    public class UserDto
    {
        public string Username { get; set; } = "";
    }
}
