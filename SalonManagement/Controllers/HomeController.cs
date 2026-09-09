using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    public HomeController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var weekAgo = today.AddDays(-6);
        var statusCounts = await _context.Appointments.GroupBy(x => x.Status).Select(x => new { Status = x.Key, Value = x.Count() }).ToListAsync();
        var model = new DashboardViewModel
        {
            CustomerCount = await _context.Customers.CountAsync(),
            AppointmentsToday = await _context.Appointments.CountAsync(x => x.AppointmentDate == today),
            RevenueToday = await _context.Invoices.Where(x => x.PaymentStatus == PaymentStatus.Paid && x.PaidAt!.Value.Date == today).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            CompletedAppointments = await _context.Appointments.CountAsync(x => x.Status == AppointmentStatus.Completed),
            NewCustomersThisMonth = await _context.Customers.CountAsync(x => x.CreatedAt >= new DateTime(today.Year, today.Month, 1)),
            RevenueByDay = await _context.Invoices.Where(x => x.PaymentStatus == PaymentStatus.Paid && x.PaidAt >= weekAgo).GroupBy(x => x.PaidAt!.Value.Date).Select(x => new ChartPoint { Label = x.Key.ToString("dd/MM"), Value = x.Sum(y => y.TotalAmount) }).ToListAsync(),
            AppointmentStatusData = statusCounts.Select(x => new ChartPoint { Label = x.Status.GetDisplayName(), Value = x.Value }).ToList(),
            PopularServices = await _context.Appointments.Where(x => x.Status == AppointmentStatus.Completed).GroupBy(x => x.Service!.Name).OrderByDescending(x => x.Count()).Take(5).Select(x => new ChartPoint { Label = x.Key, Value = x.Count() }).ToListAsync(),
            UpcomingAppointments = await _context.Appointments.Include(x => x.Customer).Include(x => x.Service).Include(x => x.Stylist).Where(x => x.AppointmentDate >= today && (x.Status == AppointmentStatus.Pending || x.Status == AppointmentStatus.Confirmed)).OrderBy(x => x.AppointmentDate).ThenBy(x => x.StartTime).Take(6).ToListAsync()
        };
        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Privacy() => View();
}
