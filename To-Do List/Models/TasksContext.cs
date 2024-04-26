using Microsoft.EntityFrameworkCore;

namespace To_Do_List.Models;

public class TasksContext : DbContext
{
    public DbSet<MyTask> MyTasks { get; set; }
    public TasksContext(DbContextOptions<TasksContext> options) : base(options)
    {

    }
}
