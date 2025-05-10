namespace pizzashop_repository.ViewModels;

public class AssignTableRequestViewModel
{
    public CustomerViewModel Customer { get; set; } = new();

    public List<int> SelectedTables { get; set; } = null!;

    public int NumberOfPersons { get; set; }
}