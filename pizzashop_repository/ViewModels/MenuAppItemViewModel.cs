namespace pizzashop_repository.ViewModels;

public class MenuAppItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public string ItemType { get; set; } = null!;

    public bool IsFavourite { get; set; }

    public string? ItemImagePath { get; set;}
}
