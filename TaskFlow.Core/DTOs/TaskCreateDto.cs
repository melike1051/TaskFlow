using TaskFlow.Core.Enums;
using TaskStatusEnum = TaskFlow.Core.Enums.TaskStatus;


namespace TaskFlow.Core.DTOs
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

         public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
    }
}
