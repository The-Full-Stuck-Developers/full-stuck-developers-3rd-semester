namespace api.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int Total { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}