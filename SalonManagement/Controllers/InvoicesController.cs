using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.Services;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

[Authorize(Roles = "Admin,Receptionist")]
public class InvoicesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IInvoiceService _invoiceService;
    public InvoicesController(ApplicationDbContext context, IInvoiceService invoiceService) { _context = context; _invoiceService = invoiceService; }
    public async Task<IActionResult> Index() => View(await _context.Invoices.Include(x => x.Customer).Include(x => x.Appointment).OrderByDescending(x => x.CreatedAt).ToListAsync());
    public async Task<IActionResult> Details(int? id) { if (!id.HasValue) return NotFound(); var invoice = await _context.Invoices.Include(x => x.Customer).Include(x => x.Appointment).ThenInclude(x => x!.Service).FirstOrDefaultAsync(x => x.Id == id); return invoice is null ? NotFound() : View(invoice); }
    public async Task<IActionResult> Create() { var model = new InvoiceCreateViewModel(); await PopulateAppointmentsAsync(model); return View(model); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceCreateViewModel model)
    {
        if (!ModelState.IsValid) { await PopulateAppointmentsAsync(model); return View(model); }
        var result = await _invoiceService.CreateAsync(model.AppointmentId, model.PaymentMethod);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, result.Error!); await PopulateAppointmentsAsync(model); return View(model); }
        TempData["Success"] = "Đã lập hóa đơn. Hãy xác nhận thanh toán khi nhận tiền.";
        return RedirectToAction(nameof(Details), new { id = result.EntityId });
    }
    public async Task<IActionResult> Payment(int? id) { if (!id.HasValue) return NotFound(); var invoice = await _context.Invoices.FindAsync(id); return invoice is null ? NotFound() : View(new PaymentViewModel { InvoiceId = invoice.Id, PaymentMethod = invoice.PaymentMethod }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Payment(PaymentViewModel model)
    {
        var result = await _invoiceService.MarkPaidAsync(model.InvoiceId, model.PaymentMethod);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Đã ghi nhận thanh toán." : result.Error;
        return RedirectToAction(nameof(Details), new { id = model.InvoiceId });
    }
    private async Task PopulateAppointmentsAsync(InvoiceCreateViewModel model)
    {
        model.Appointments = (await _context.Appointments.Include(x => x.Customer).Include(x => x.Service).Where(x => x.Status == AppointmentStatus.Completed && x.Invoice == null).OrderByDescending(x => x.AppointmentDate).ToListAsync()).Select(x => new SelectListItem($"{x.AppointmentDate:dd/MM/yyyy} - {x.Customer!.FullName} - {x.Service!.Name}", x.Id.ToString()));
    }
}
