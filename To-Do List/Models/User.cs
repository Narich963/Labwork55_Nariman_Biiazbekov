using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace To_Do_List.Models;

public class User : IdentityUser<int>
{
    [InverseProperty("Creator")]
    public List<MyTask> CreatedTasks { get; set; }
    [InverseProperty("Executor")]
    public List<MyTask> ExecutingTasks { get; set; }
    public User()
    {
        CreatedTasks = new List<MyTask>();
        ExecutingTasks = new List<MyTask>();
    }
}
