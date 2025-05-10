namespace pizzashop_repository.ViewModels;

public class CustomerHistoryViewModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal AverageBill { get; set; }

    public string? ComingSince { get; set; }

    public decimal MaxOrder { get; set; }

    public int Visits { get; set; }

    public List<OrderHistoryViewModel> Orders { get; set; } = new List<OrderHistoryViewModel>();
    public int? ClickedOrderId { get; set; }
}

public class OrderHistoryViewModel
{
    public int OrderId { get; set; }
    public string? Date { get; set; }
    public string? OrderType { get; set; }
    public string? Payment { get; set; }
    public int Items { get; set; }
    public decimal Amount { get; set; }
}