namespace pizzashop_repository.ViewModels;

public class CustomerTableViewModel
{   
    public int Id { get; set; }
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PhoNo { get; set; }

    public DateTime Date { get; set; }

    public int TotalOrder { get; set; }
}
