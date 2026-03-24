namespace TaskFlow.MVC.Models.Tasks
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DueDate { get; set; }
        public int Status { get; set; }

        // 👤 ADMIN İÇİN: GÖREVİN SAHİBİ
        public string Username { get; set; } = "";

        public string StatusText
        {
            get
            {
                return Status switch
                {
                    0 => "Todo",
                    1 => "In Progress",
                    2 => "Done",
                    _ => "Unknown"
                };
            }
        }

        public string StatusClass => Status switch
        {
            0 => "todo",
            1 => "inprogress",
            2 => "done",
            _ => "todo"
        };

        public string DueStateText
        {
            get
            {
                if (Status == 2)
                    return "Completed";

                var today = DateTime.Today;
                if (DueDate.Date < today)
                    return "Overdue";
                if (DueDate.Date == today)
                    return "Due today";
                if (DueDate.Date <= today.AddDays(3))
                    return "Due soon";

                return "Planned";
            }
        }
    }
}
