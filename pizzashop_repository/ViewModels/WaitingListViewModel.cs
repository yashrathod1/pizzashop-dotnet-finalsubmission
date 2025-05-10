namespace pizzashop_repository.ViewModels;

public class WaitingListViewModel
{
    public List<WaitingListSectionViewModel> Sections { get; set; } = new();

    public List<WaitingListItemViewModel> WaitingList { get; set; } = new();

    public WaitingTokenViewModel WaitingTokens { get; set; } = new WaitingTokenViewModel();

    public int? SectionId { get; set; }
}


public class WaitingListSectionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public int WaitingListCount { get; set; }
}

public class WaitingListItemViewModel
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime WaitingTime { get; set; }

    public string Name { get; set; } = null!;

    public int NoOfPerson { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int SectionId { get; set; }

    public int CustomerId { get; set;}

}