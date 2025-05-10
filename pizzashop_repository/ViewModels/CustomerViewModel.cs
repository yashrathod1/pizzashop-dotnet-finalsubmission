namespace pizzashop_repository.ViewModels;

public class CustomerViewModel
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string MobileNo {get; set;} = null!;
}
