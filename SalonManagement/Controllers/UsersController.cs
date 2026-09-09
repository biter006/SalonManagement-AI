using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.ViewModels;

namespace SalonManagement.Controllers;

[Authorize(Roles = DbInitializer.AdminRole)]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) { _userManager = userManager; _roleManager = roleManager; }
    public async Task<IActionResult> Index()
    {
        // Materialize the users query before querying Identity again for each user's roles.
        // SQL Server permits only one active DataReader per connection by default.
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .ToListAsync();
        var rows = new List<UserRowViewModel>();
        foreach (var user in users)
            rows.Add(new UserRowViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? string.Empty,
                Role = string.Join(", ", (await _userManager.GetRolesAsync(user)).Select(DisplayNameExtensions.GetRoleDisplayName))
            });
        return View(rows);
    }
    public async Task<IActionResult> Create() { await SetRolesAsync(); return View(new CreateUserViewModel()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!await _roleManager.RoleExistsAsync(model.Role)) ModelState.AddModelError(nameof(model.Role), "Vai trò không hợp lệ.");
        if (!ModelState.IsValid) { await SetRolesAsync(); return View(model); }
        var result = await _userManager.CreateAsync(new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true }, model.Password);
        if (!result.Succeeded) { foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description); await SetRolesAsync(); return View(model); }
        var user = await _userManager.FindByEmailAsync(model.Email); await _userManager.AddToRoleAsync(user!, model.Role);
        TempData["Success"] = "Đã tạo tài khoản."; return RedirectToAction(nameof(Index));
    }
    private async Task SetRolesAsync() => ViewBag.Roles = await _roleManager.Roles
        .OrderBy(x => x.Name)
        .Select(x => x.Name!)
        .ToListAsync();
}
