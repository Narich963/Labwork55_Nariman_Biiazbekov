using To_Do_List.Models;

namespace To_Do_List.ViewModels;

public class IndexViewModel
{
    public List<MyTask> Tasks { get; set; }
    public PageViewModel PageViewModel { get; set; }
}
