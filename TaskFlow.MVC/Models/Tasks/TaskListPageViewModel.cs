namespace TaskFlow.MVC.Models.Tasks
{
    public class TaskListPageViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string EmptyTitle { get; set; } = string.Empty;
        public string EmptyDescription { get; set; } = string.Empty;
        public bool ShowOwnerColumn { get; set; }
        public List<TaskViewModel> Items { get; set; } = [];
        public string Search { get; set; } = string.Empty;
        public int? Status { get; set; }
        public string SortBy { get; set; } = "dueDate";
        public string SortDirection { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalCount { get; set; }
        public int TodoCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
