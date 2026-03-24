namespace TaskFlow.MVC.Models.Users
{
    public class UserManagementViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public bool CanDelete { get; set; }
    }
}
