using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;

namespace SalonManagement.Controllers;

public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;
    public ServicesController(ApplicationDbContext context) => _context = context;

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Index(string? search, bool? status)
    {
        var query = _context.SalonServices.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        ViewBag.Search = search; ViewBag.Status = status;
        return View(await query.OrderBy(x => x.Name).ToListAsync());
    }

    [Authorize(Roles = "Admin,Receptionist,Stylist")]
    public async Task<IActionResult> Details(int? id) { if (!id.HasValue) return NotFound(); var item = await _context.SalonServices.FindAsync(id); return item is null ? NotFound() : View(item); }
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new SalonService());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(SalonService service)
    {
        if (await _context.SalonServices.AnyAsync(x => x.Name == service.Name)) ModelState.AddModelError(nameof(service.Name), "Tên dịch vụ đã tồn tại.");
        if (!ModelState.IsValid) return View(service);
        _context.SalonServices.Add(service); await _context.SaveChangesAsync(); TempData["Success"] = "Đã thêm dịch vụ."; return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id) { if (!id.HasValue) return NotFound(); var item = await _context.SalonServices.FindAsync(id); return item is null ? NotFound() : View(item); }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, SalonService service)
    {
        if (id != service.Id) return NotFound();
        if (await _context.SalonServices.AnyAsync(x => x.Name == service.Name && x.Id != id)) ModelState.AddModelError(nameof(service.Name), "Tên dịch vụ đã tồn tại.");
        if (!ModelState.IsValid) return View(service);
        var existing = await _context.SalonServices.FindAsync(id); if (existing is null) return NotFound();
        existing.Name = service.Name; existing.Description = service.Description; existing.Price = service.Price; existing.DurationMinutes = service.DurationMinutes; existing.Status = service.Status;
        await _context.SaveChangesAsync(); return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id) { if (!id.HasValue) return NotFound(); var item = await _context.SalonServices.FindAsync(id); return item is null ? NotFound() : View(item); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.SalonServices.FindAsync(id); if (item is null) return NotFound();
        if (await _context.Appointments.AnyAsync(x => x.ServiceId == id)) { TempData["Error"] = "Không thể xóa dịch vụ đã được dùng; hãy chuyển sang ngừng cung cấp."; return RedirectToAction(nameof(Details), new { id }); }
        _context.SalonServices.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
}
