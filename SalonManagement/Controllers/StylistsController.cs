using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;

namespace SalonManagement.Controllers;

public class StylistsController : Controller
{
    private readonly ApplicationDbContext _context;
    public StylistsController(ApplicationDbContext context) => _context = context;

    [Authorize(Roles = "Admin,Receptionist")]
    public async Task<IActionResult> Index() => View(await _context.Stylists.OrderBy(x => x.FullName).ToListAsync());

    [Authorize(Roles = "Admin,Receptionist,Stylist")]
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return NotFound();
        var stylist = await _context.Stylists.Include(x => x.Schedules).FirstOrDefaultAsync(x => x.Id == id);
        if (stylist is not null && User.IsInRole("Stylist") && stylist.Email != User.Identity?.Name) return Forbid();
        return stylist is null ? NotFound() : View(stylist);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Stylist());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Stylist stylist)
    {
        if (!ModelState.IsValid) return View(stylist);
        _context.Stylists.Add(stylist);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm thợ.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!id.HasValue) return NotFound();
        var stylist = await _context.Stylists.FindAsync(id);
        return stylist is null ? NotFound() : View(stylist);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Stylist stylist)
    {
        if (id != stylist.Id) return NotFound();
        if (!ModelState.IsValid) return View(stylist);
        var existing = await _context.Stylists.FindAsync(id);
        if (existing is null) return NotFound();
        existing.FullName = stylist.FullName; existing.Phone = stylist.Phone; existing.Email = stylist.Email; existing.Specialization = stylist.Specialization; existing.Experience = stylist.Experience; existing.Status = stylist.Status;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật thợ.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return NotFound();
        var stylist = await _context.Stylists.FindAsync(id);
        return stylist is null ? NotFound() : View(stylist);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var stylist = await _context.Stylists.FindAsync(id);
        if (stylist is null) return NotFound();
        if (await _context.Appointments.AnyAsync(x => x.StylistId == id)) { TempData["Error"] = "Không thể xóa thợ đã có lịch hẹn."; return RedirectToAction(nameof(Details), new { id }); }
        _context.Stylists.Remove(stylist);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
