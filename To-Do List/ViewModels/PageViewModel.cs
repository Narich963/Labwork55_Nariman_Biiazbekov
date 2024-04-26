namespace To_Do_List.ViewModels;

public class PageViewModel
{
    public int PageNumger { get; private set; }
    public int TotalPages { get; private set; }

    public PageViewModel(int count, int pageNumber, int pageSize)
    {
        PageNumger = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
    public bool HasPreviousPage
    {
        get => PageNumger > 1;
    }
    public bool HasNextPage
    {
        get => PageNumger < TotalPages; 
    }
}
