using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskFlow.Core.Enums;
using TaskStatusEnum = TaskFlow.Core.Enums.TaskStatus;




namespace TaskFlow.Core.DTOs
{
    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }= string.Empty;
        public DateTime DueDate { get; set; }

         //public TaskFlow.Core.Enums.TaskStatus Status { get; set; }

        public TaskStatusEnum Status { get; set; }

        public TaskUpdateDto()
        {
        }
    }
}
