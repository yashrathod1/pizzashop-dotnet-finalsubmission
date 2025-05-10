namespace pizzashop_repository.ViewModels;

public class TablesOrderAppViewModel
{
    public List<OrderAppSectionViewModel> Sections { get; set; } = new();

    public List<OrderAppTableViewModel> Tables { get; set; } = new();

    public WaitingTokenViewModel WaitingTokens { get; set; } = new WaitingTokenViewModel();

}
public class OrderAppTableViewModel
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? OrderTableTime { get; set; }

    public decimal TotalAmount { get; set; }

    public int OrderId { get; set; }
}

public class OrderAppSectionViewModel
{
    public int Id {get; set;}

    public string Name {get; set;} = null!;

    public int AvailableCount {get; set;}

    public int AssignedCount { get; set; }

    public int OccupiedCount { get; set; }
}
