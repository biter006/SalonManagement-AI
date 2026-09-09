using SalonManagement.Models;

namespace SalonManagement.ViewModels;

public class CustomerDetailsViewModel
{
    public Customer Customer { get; set; } = null!;
    public List<Appointment> UpcomingAppointments { get; set; } = new();
    public List<ServiceHistory> Histories { get; set; } = new();
    public decimal TotalSpent { get; set; }
    public string? FavoriteService { get; set; }
}
