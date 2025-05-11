namespace pizzashop_repository.ViewModels;

public class DashboardViewModel
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public double AverageWaitingTime { get; set; }
    public List<ChartDataPoint>? RevenueChartData { get; set; }
    public List<ChartDataPoint>? CustomerGrowthData { get; set; }
    public List<TopItem>? TopSellingItems { get; set; }
    public List<TopItem>? LeastSellingItems { get; set; }
    public int WaitingListCount { get; set; }
    public int NewCustomer { get; set; }

    public string Filter { get; set; }
}
public class ChartDataPoint
{
    public string? Label { get; set; }
    public decimal Value { get; set; }
}
public class TopItem
{
    public string? Name { get; set; }
    public int OrderCount { get; set; }
    public string? ImageUrl { get; set; }
}