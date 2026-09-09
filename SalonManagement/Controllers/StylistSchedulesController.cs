using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;

namespace SalonManagement.Controllers;

public class StylistSchedulesController : Controller
{
    private readonly ApplicationDbContext _context;
    public StylistSchedulesController(ApplicationDbContext context) => _context = context;

    [Authorize(Roles = "Admin,Receptionist,Stylist")]
    public async Task<IActionResult> Index(DateTime? date, int? stylistId)
    {
        var query = _context.StylistSchedules.Include(x => x.Stylist).AsQueryable();
        if (date.HasValue) query = query.Where(x => x.WorkDate == date.Value.Date);
        if (stylistId.HasValue) query = query.Where(x => x.StylistId == stylistId);
        if (User.IsInRole("Stylist"))
        {
            var currentStylistId = await _context.Stylists.Where(x => x.Email == User.Identity!.Name).Select(x => (int?)x.Id).FirstOrDefaultAsync();
            query = query.Where(x => x.StylistId == currentStylistId);
        }
        ViewBag.Date = date?.ToString("yyyy-MM-dd"); ViewBag.StylistId = stylistId;
        await SetStylistsAsync();
        return View(await query.OrderBy(x => x.WorkDate).ThenBy(x => x.StartTime).ToListAsync());
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await SetStylistsAsync();
        return View(new StylistSchedule { WorkDate = DateTime.Today, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0) });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(StylistSchedule schedule)
    {
        if (schedule.WorkDate.Date < DateTime.Today) ModelState.AddModelError(nameof(schedule.WorkDate), "Không thể tạo ca trong quá khứ.");
        if (await _context.StylistSchedules.AnyAsync(x => x.StylistId == schedule.StylistId && x.WorkDate == schedule.WorkDate.Date && x.StartTime == schedule.StartTime && x.EndTime == schedule.EndTime)) ModelState.AddModelError(string.Empty, "Ca làm việc này đã tồn tại.");
        if (!ModelState.IsValid) { await SetStylistsAsync(); return View(schedule); }
        schedule.WorkDate = schedule.WorkDate.Date;
        _context.StylistSchedules.Add(schedule); await _context.SaveChangesAsync(); TempData["Success"] = "Đã thêm ca làm việc."; return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id) { if (!id.HasValue) return NotFound(); var item = await _context.StylistSchedules.FindAsync(id); if (item is null) return NotFound(); await SetStylistsAsync(); return View(item); }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, StylistSchedule schedule)
    {
        if (id != schedule.Id) return NotFound(); if (!ModelState.IsValid) { await SetStylistsAsync(); return View(schedule); }
        var existing = await _context.StylistSchedules.FindAsync(id); if (existing is null) return NotFound();
        existing.StylistId = schedule.StylistId; existing.WorkDate = schedule.WorkDate.Date; existing.StartTime = schedule.StartTime; existing.EndTime = schedule.EndTime; existing.Status = schedule.Status;
        await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id) { if (!id.HasValue) return NotFound(); var item = await _context.StylistSchedules.Include(x => x.Stylist).FirstOrDefaultAsync(x => x.Id == id); return item is null ? NotFound() : View(item); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.StylistSchedules.FindAsync(id); if (item is null) return NotFound(); _context.StylistSchedules.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }

    private async Task SetStylistsAsync() => ViewBag.Stylists = new SelectList(await _context.Stylists.OrderBy(x => x.FullName).ToListAsync(), "Id", "FullName");
}
