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

        // 👇 SADECE GÖRSEL AMAÇLI
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
    }
}
