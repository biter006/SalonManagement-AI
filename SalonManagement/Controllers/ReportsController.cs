using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;
    public ReportsController(ApplicationDbContext context) => _context = context;
    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
    {
        var from = (fromDate ?? DateTime.Today.AddDays(-30)).Date;
        var to = (toDate ?? DateTime.Today).Date;
        if (to < from) (from, to) = (to, from);
        var paid = _context.Invoices.Where(x => x.PaymentStatus == PaymentStatus.Paid && x.PaidAt!.Value.Date >= from && x.PaidAt!.Value.Date <= to);
        var completed = _context.Appointments.Where(x => x.Status == AppointmentStatus.Completed && x.AppointmentDate >= from && x.AppointmentDate <= to);
        var model = new ReportViewModel
        {
            FromDate = from, ToDate = to,
            TotalRevenue = await paid.SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            RevenueByDay = await paid.GroupBy(x => x.PaidAt!.Value.Date).Select(x => new ChartPoint { Label = x.Key.ToString("dd/MM"), Value = x.Sum(y => y.TotalAmount) }).ToListAsync(),
            PopularServices = await completed.GroupBy(x => x.Service!.Name).OrderByDescending(x => x.Count()).Select(x => new ChartPoint { Label = x.Key, Value = x.Count() }).ToListAsync(),
            ReturningCustomers = await completed.GroupBy(x => x.Customer!.FullName).Where(x => x.Count() >= 2).OrderByDescending(x => x.Count()).Select(x => new ReturningCustomerRow { CustomerName = x.Key, CompletedVisits = x.Count() }).ToListAsync()
        };
        return View(model);
    }
}
