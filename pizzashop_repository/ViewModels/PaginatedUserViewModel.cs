using pizzashop_repository.Models;

namespace pizzashop_repository.ViewModels;

public class PaginatedUserViewModel
{
     public List<User> Users { get; set; } = new List<User>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public int StartIndex => ((CurrentPage - 1) * PageSize) + 1;
    public int EndIndex => Math.Min(CurrentPage * PageSize, TotalItems);
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}
