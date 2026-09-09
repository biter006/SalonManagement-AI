using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.Services;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAppointmentService _appointmentService;
    public AppointmentsController(ApplicationDbContext context, IAppointmentService appointmentService) { _context = context; _appointmentService = appointmentService; }

    public async Task<IActionResult> Index(AppointmentFilterViewModel filter)
    {
        var query = _context.Appointments.Include(x => x.Customer).Include(x => x.Stylist).Include(x => x.Service).AsQueryable();
        if (filter.Date.HasValue) query = query.Where(x => x.AppointmentDate == filter.Date.Value.Date);
        if (filter.StylistId.HasValue) query = query.Where(x => x.StylistId == filter.StylistId);
        if (filter.CustomerId.HasValue) query = query.Where(x => x.CustomerId == filter.CustomerId);
        if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status);
        if (User.IsInRole("Stylist"))
        {
            var stylistId = await _context.Stylists.Where(x => x.Email == User.Identity!.Name).Select(x => (int?)x.Id).FirstOrDefaultAsync();
            query = query.Where(x => x.StylistId == stylistId);
        }
        await PopulateFilterAsync(filter);
        ViewBag.Appointments = await query.OrderByDescending(x => x.AppointmentDate).ThenBy(x => x.StartTime).ToListAsync();
        return View(filter);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return NotFound();
        var appointment = await _context.Appointments.Include(x => x.Customer).Include(x => x.Stylist).Include(x => x.Service).Include(x => x.Invoice).FirstOrDefaultAsync(x => x.Id == id);
        if (appointment is null) return NotFound();
        if (User.IsInRole("Stylist") && !await OwnsAppointmentAsync(appointment)) return Forbid();
        return View(new AppointmentDetailsViewModel { Appointment = appointment });
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Create(int? customerId)
    {
        var model = new AppointmentFormViewModel { CustomerId = customerId ?? 0 };
        await PopulateFormAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Create(AppointmentFormViewModel model)
    {
        if (!ModelState.IsValid) { await PopulateFormAsync(model); return View(model); }
        var result = await _appointmentService.CreateAsync(model);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, result.Error!); await PopulateFormAsync(model); return View(model); }
        TempData["Success"] = "Đã tạo lịch hẹn. Giờ kết thúc được tính theo thời lượng dịch vụ.";
        return RedirectToAction(nameof(Details), new { id = result.EntityId });
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return NotFound();
        var item = await _context.Appointments.FindAsync(id); if (item is null) return NotFound();
        var model = new AppointmentFormViewModel { Id = item.Id, CustomerId = item.CustomerId, StylistId = item.StylistId, ServiceId = item.ServiceId, AppointmentDate = item.AppointmentDate, StartTime = item.StartTime, CustomerNote = item.CustomerNote };
        await PopulateFormAsync(model); return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Edit(int id, AppointmentFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) { await PopulateFormAsync(model); return View(model); }
        var result = await _appointmentService.RescheduleAsync(id, model);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, result.Error!); await PopulateFormAsync(model); return View(model); }
        TempData["Success"] = "Đã cập nhật lịch hẹn.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Cancel(int id, string? cancelReason)
    {
        var result = await _appointmentService.CancelAsync(id, cancelReason);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Đã hủy lịch hẹn." : result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist,Stylist")]
    public async Task<IActionResult> Complete(int id, string? staffNote)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment is null) return NotFound();
        if (User.IsInRole("Stylist") && !await OwnsAppointmentAsync(appointment)) return Forbid();
        var result = await _appointmentService.CompleteAsync(id, staffNote);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Đã hoàn thành lịch hẹn và cập nhật lịch sử khách." : result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> ServiceDuration(int id)
    {
        var service = await _context.SalonServices.FindAsync(id);
        return service is null ? NotFound() : Json(new { durationMinutes = service.DurationMinutes, price = service.Price });
    }

    private async Task PopulateFormAsync(AppointmentFormViewModel model)
    {
        model.Customers = (await _context.Customers.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem($"{x.FullName} - {x.Phone}", x.Id.ToString()));
        model.Stylists = (await _context.Stylists.Where(x => x.Status == StylistStatus.Active).OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
        model.Services = (await _context.SalonServices.Where(x => x.Status).OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem($"{x.Name} ({x.DurationMinutes} phút)", x.Id.ToString()));
    }
    private async Task PopulateFilterAsync(AppointmentFilterViewModel model)
    {
        model.Stylists = (await _context.Stylists.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
        model.Customers = (await _context.Customers.OrderBy(x => x.FullName).ToListAsync()).Select(x => new SelectListItem(x.FullName, x.Id.ToString()));
    }
    private async Task<bool> OwnsAppointmentAsync(Appointment appointment) => await _context.Stylists.AnyAsync(x => x.Id == appointment.StylistId && x.Email == User.Identity!.Name);
}
