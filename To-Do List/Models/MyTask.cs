namespace To_Do_List.Models;

public class MyTask
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Executor { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedDate { get; set; }
    public DateTime? OpenedDate { get; set; }
}

