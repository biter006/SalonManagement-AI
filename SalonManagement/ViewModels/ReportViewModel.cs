namespace SalonManagement.ViewModels;

public class ReportViewModel
{
    public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-30);
    public DateTime ToDate { get; set; } = DateTime.Today;
    public decimal TotalRevenue { get; set; }
    public List<ChartPoint> RevenueByDay { get; set; } = new();
    public List<ChartPoint> PopularServices { get; set; } = new();
    public List<ReturningCustomerRow> ReturningCustomers { get; set; } = new();
}

public class ReturningCustomerRow
{
    public string CustomerName { get; set; } = string.Empty;
    public int CompletedVisits { get; set; }
}
