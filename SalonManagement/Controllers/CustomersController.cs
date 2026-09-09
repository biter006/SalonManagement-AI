using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;
    public CustomersController(ApplicationDbContext context) => _context = context;

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.FullName.Contains(search) || x.Phone.Contains(search) || (x.Email != null && x.Email.Contains(search)));
        ViewBag.Search = search;
        return View(await query.OrderBy(x => x.FullName).ToListAsync());
    }

    [Authorize(Roles = "Admin,Receptionist,Stylist")]
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return NotFound();
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer is null) return NotFound();
        if (User.IsInRole("Stylist") && !await _context.Appointments.AnyAsync(x => x.CustomerId == id && x.Stylist!.Email == User.Identity!.Name)) return Forbid();
        var histories = await _context.ServiceHistories.Include(x => x.Service).Include(x => x.Stylist).Where(x => x.CustomerId == id).OrderByDescending(x => x.ServiceDate).ToListAsync();
        var model = new CustomerDetailsViewModel
        {
            Customer = customer,
            Histories = histories,
            UpcomingAppointments = await _context.Appointments.Include(x => x.Service).Include(x => x.Stylist).Where(x => x.CustomerId == id && x.AppointmentDate >= DateTime.Today && (x.Status == AppointmentStatus.Pending || x.Status == AppointmentStatus.Confirmed)).OrderBy(x => x.AppointmentDate).ToListAsync(),
            TotalSpent = await _context.Invoices.Where(x => x.CustomerId == id && x.PaymentStatus == PaymentStatus.Paid).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
            FavoriteService = histories.GroupBy(x => x.Service!.Name).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault()
        };
        return View(model);
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public IActionResult Create() => View(new Customer());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (await _context.Customers.AnyAsync(x => x.Phone == customer.Phone)) ModelState.AddModelError(nameof(customer.Phone), "Số điện thoại đã tồn tại.");
        if (!ModelState.IsValid) return View(customer);
        customer.CreatedAt = DateTime.UtcNow;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm khách hàng.";
        return RedirectToAction(nameof(Details), new { id = customer.Id });
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return NotFound();
        var customer = await _context.Customers.FindAsync(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.Id) return NotFound();
        if (await _context.Customers.AnyAsync(x => x.Phone == customer.Phone && x.Id != id)) ModelState.AddModelError(nameof(customer.Phone), "Số điện thoại đã tồn tại.");
        if (!ModelState.IsValid) return View(customer);
        var existing = await _context.Customers.FindAsync(id);
        if (existing is null) return NotFound();
        existing.FullName = customer.FullName; existing.Phone = customer.Phone; existing.Email = customer.Email; existing.DateOfBirth = customer.DateOfBirth; existing.Gender = customer.Gender; existing.Address = customer.Address; existing.Notes = customer.Notes;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật khách hàng.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return NotFound();
        var customer = await _context.Customers.FindAsync(id);
        return customer is null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null) return NotFound();
        if (await _context.Appointments.AnyAsync(x => x.CustomerId == id))
        {
            TempData["Error"] = "Không thể xóa khách hàng đã có lịch hẹn. Hãy giữ lại để bảo toàn lịch sử.";
            return RedirectToAction(nameof(Details), new { id });
        }
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa khách hàng.";
        return RedirectToAction(nameof(Index));
    }
}
