using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace To_Do_List.Models;

public class TasksContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<MyTask> MyTasks { get; set; }
    public DbSet<User> Users { get; set; }
    public TasksContext(DbContextOptions<TasksContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
