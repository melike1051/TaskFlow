using Microsoft.EntityFrameworkCore;

using TaskFlow.Core.Entities;

namespace TaskFlow.API.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }
    }
}
