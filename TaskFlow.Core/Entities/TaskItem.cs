using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TaskStatusEnum = TaskFlow.Core.Enums.TaskStatus;


namespace TaskFlow.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public TaskStatusEnum Status { get; set; }


        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
