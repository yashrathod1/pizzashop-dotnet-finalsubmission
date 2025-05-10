namespace pizzashop_repository.ViewModels;

public class UpdatePreparedItemsViewModel
{
    public int OrderId { get; set; }
    public List<PreparedItemViewModel>? Items { get; set; } = new();

    public string status { get; set;} = null!;
}

public class PreparedItemViewModel
{
    public int ItemId { get; set; }
    public int changeInQuantity { get; set; }
}
