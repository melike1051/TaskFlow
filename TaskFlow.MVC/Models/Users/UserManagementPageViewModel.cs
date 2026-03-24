namespace TaskFlow.MVC.Models.Users
{
    public class UserManagementPageViewModel
    {
        public List<UserManagementViewModel> Items { get; set; } = [];
        public string Search { get; set; } = string.Empty;
        public string Role { get; set; } = "All";
        public string SortBy { get; set; } = "username";
        public string SortDirection { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalCount { get; set; }
        public int AdminCount { get; set; }
        public int UserCount { get; set; }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
