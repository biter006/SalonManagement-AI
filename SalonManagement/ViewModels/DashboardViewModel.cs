using SalonManagement.Models;

namespace SalonManagement.ViewModels;

public class DashboardViewModel
{
    public int CustomerCount { get; set; }
    public int AppointmentsToday { get; set; }
    public decimal RevenueToday { get; set; }
    public int CompletedAppointments { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public List<ChartPoint> RevenueByDay { get; set; } = new();
    public List<ChartPoint> AppointmentStatusData { get; set; } = new();
    public List<ChartPoint> PopularServices { get; set; } = new();
    public List<Appointment> UpcomingAppointments { get; set; } = new();
}

public class ChartPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
