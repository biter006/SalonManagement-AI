using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Models;

namespace SalonManagement.Data;

public static class DbInitializer
{
    public const string AdminRole = "Admin";
    public const string ReceptionistRole = "Receptionist";
    public const string StylistRole = "Stylist";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();
        foreach (var role in new[] { AdminRole, ReceptionistRole, StylistRole })
            if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

        var password = configuration["DemoAccounts:Password"];
        if (string.IsNullOrWhiteSpace(password))
            logger.LogWarning("Demo accounts were skipped because DemoAccounts:Password has not been configured.");
        else
        {
            await EnsureUserAsync(userManager, "admin@salon.local", AdminRole, password);
            await EnsureUserAsync(userManager, "receptionist@salon.local", ReceptionistRole, password);
            await EnsureUserAsync(userManager, "stylist1@salon.local", StylistRole, password);
            await EnsureUserAsync(userManager, "stylist2@salon.local", StylistRole, password);
        }

        if (await context.Customers.AnyAsync()) return;
        var today = DateTime.Today;
        var customers = Enumerable.Range(1, 10).Select(i => new Customer
        {
            FullName = $"Khách hàng {i:00}", Phone = $"09000000{i:00}", Email = $"customer{i:00}@example.com",
            Gender = i % 2 == 0 ? "Nữ" : "Nam", Notes = i % 3 == 0 ? "Ưu tiên phong cách tự nhiên." : null,
            CreatedAt = today.AddDays(-45 + i)
        }).ToList();
        var stylists = new List<Stylist>
        {
            new() { FullName = "Nguyễn Minh", Phone = "0911000001", Email = "stylist1@salon.local", Specialization = "Cắt, tạo kiểu nam", Experience = 6 },
            new() { FullName = "Trần An", Phone = "0911000002", Email = "stylist2@salon.local", Specialization = "Nhuộm và phục hồi", Experience = 8 },
            new() { FullName = "Lê Hương", Phone = "0911000003", Email = "stylist3@salon.local", Specialization = "Uốn và tạo kiểu nữ", Experience = 5 }
        };
        var salonServices = new List<SalonService>
        {
            new() { Name = "Cắt tóc nam", Description = "Tư vấn và cắt tạo kiểu.", Price = 150000, DurationMinutes = 45 },
            new() { Name = "Cắt tóc nữ", Description = "Cắt và tạo kiểu cơ bản.", Price = 250000, DurationMinutes = 60 },
            new() { Name = "Gội đầu thư giãn", Description = "Gội, massage da đầu.", Price = 80000, DurationMinutes = 30 },
            new() { Name = "Nhuộm tóc", Description = "Nhuộm màu theo tư vấn.", Price = 850000, DurationMinutes = 150 },
            new() { Name = "Uốn tóc", Description = "Uốn tạo kiểu.", Price = 950000, DurationMinutes = 180 },
            new() { Name = "Phục hồi keratin", Description = "Phục hồi tóc hư tổn.", Price = 650000, DurationMinutes = 120 },
            new() { Name = "Duỗi tóc", Description = "Duỗi và chăm sóc tóc.", Price = 800000, DurationMinutes = 150 },
            new() { Name = "Tạo kiểu sự kiện", Description = "Tạo kiểu dự tiệc.", Price = 400000, DurationMinutes = 75 }
        };
        context.AddRange(customers);
        context.AddRange(stylists);
        context.AddRange(salonServices);
        await context.SaveChangesAsync();

        var schedules = new List<StylistSchedule>();
        for (var day = -20; day <= 20; day++)
            foreach (var stylist in stylists)
                schedules.Add(new StylistSchedule { StylistId = stylist.Id, WorkDate = today.AddDays(day), StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0), Status = ScheduleStatus.Working });
        context.StylistSchedules.AddRange(schedules);
        var statuses = new[] { AppointmentStatus.Completed, AppointmentStatus.Completed, AppointmentStatus.Completed, AppointmentStatus.Confirmed, AppointmentStatus.Pending, AppointmentStatus.Cancelled, AppointmentStatus.NoShow };
        var appointments = new List<Appointment>();
        for (var i = 0; i < 15; i++)
        {
            var service = salonServices[i % salonServices.Count];
            var date = today.AddDays(i - 10);
            var start = new TimeSpan(9 + (i % 5), 0, 0);
            appointments.Add(new Appointment { CustomerId = customers[i % customers.Count].Id, StylistId = stylists[i % stylists.Count].Id, ServiceId = service.Id, AppointmentDate = date, StartTime = start, EndTime = start.Add(TimeSpan.FromMinutes(service.DurationMinutes)), Status = statuses[i % statuses.Length], CustomerNote = i % 4 == 0 ? "Muốn tư vấn kiểu hợp khuôn mặt." : null, CreatedAt = date.AddDays(-3) });
        }
        context.Appointments.AddRange(appointments);
        await context.SaveChangesAsync();
        var completed = appointments.Where(x => x.Status == AppointmentStatus.Completed).ToList();
        context.Invoices.AddRange(completed.Select(x => new Invoice { AppointmentId = x.Id, CustomerId = x.CustomerId, TotalAmount = salonServices.Single(s => s.Id == x.ServiceId).Price, PaymentMethod = PaymentMethod.Cash, PaymentStatus = PaymentStatus.Paid, PaidAt = x.AppointmentDate.Add(x.EndTime) }));
        context.ServiceHistories.AddRange(completed.Select(x => new ServiceHistory { CustomerId = x.CustomerId, AppointmentId = x.Id, ServiceId = x.ServiceId, StylistId = x.StylistId, ServiceDate = x.AppointmentDate, Notes = x.CustomerNote, ResultDescription = "Khách hài lòng với kết quả dịch vụ." }));
        await context.SaveChangesAsync();
    }

    private static async Task EnsureUserAsync(UserManager<IdentityUser> userManager, string email, string role, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
        if (!await userManager.IsInRoleAsync(user, role)) await userManager.AddToRoleAsync(user, role);
    }
}
