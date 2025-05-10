namespace pizzashop_repository.ViewModels;

public class OrdersTableViewModel
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } 
    public string CustomerName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    
    public int? Rating { get; set; }
    public decimal TotalAmount { get; set; }
}
