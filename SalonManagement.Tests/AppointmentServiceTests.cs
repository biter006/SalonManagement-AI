using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.Services;
using SalonManagement.ViewModels;

namespace SalonManagement.Tests;

public class AppointmentServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidSlot_CalculatesEndTime()
    {
        await using var context = CreateContext();
        var (customer, stylist, service) = await AddBookingDataAsync(context);
        var day = DateTime.Today.AddDays(1);
        context.StylistSchedules.Add(new StylistSchedule { StylistId = stylist.Id, WorkDate = day, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0) });
        await context.SaveChangesAsync();
        var result = await Service(context).CreateAsync(Form(customer.Id, stylist.Id, service.Id, day, new TimeSpan(9, 0, 0)));
        var appointment = await context.Appointments.SingleAsync();
        Assert.True(result.Succeeded);
        Assert.Equal(new TimeSpan(10, 0, 0), appointment.EndTime);
    }

    [Fact]
    public async Task CreateAsync_OverlappingAppointment_IsRejected()
    {
        await using var context = CreateContext();
        var (customer, stylist, service) = await AddBookingDataAsync(context);
        var day = DateTime.Today.AddDays(1);
        context.StylistSchedules.Add(new StylistSchedule { StylistId = stylist.Id, WorkDate = day, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0) });
        context.Appointments.Add(new Appointment { CustomerId = customer.Id, StylistId = stylist.Id, ServiceId = service.Id, AppointmentDate = day, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Status = AppointmentStatus.Confirmed });
        await context.SaveChangesAsync();
        var result = await Service(context).CreateAsync(Form(customer.Id, stylist.Id, service.Id, day, new TimeSpan(9, 30, 0)));
        Assert.False(result.Succeeded);
        Assert.Contains("trùng", result.Error!);
        Assert.Single(context.Appointments);
    }

    [Fact]
    public async Task CreateAsync_OutsideWorkShift_IsRejected()
    {
        await using var context = CreateContext();
        var (customer, stylist, service) = await AddBookingDataAsync(context);
        var day = DateTime.Today.AddDays(1);
        context.StylistSchedules.Add(new StylistSchedule { StylistId = stylist.Id, WorkDate = day, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(10, 0, 0) });
        await context.SaveChangesAsync();
        var result = await Service(context).CreateAsync(Form(customer.Id, stylist.Id, service.Id, day, new TimeSpan(9, 30, 0)));
        Assert.False(result.Succeeded);
        Assert.Contains("ca làm", result.Error!);
    }

    [Fact]
    public async Task CreateAsync_PastDate_IsRejected()
    {
        await using var context = CreateContext();
        var (customer, stylist, service) = await AddBookingDataAsync(context);
        var result = await Service(context).CreateAsync(Form(customer.Id, stylist.Id, service.Id, DateTime.Today.AddDays(-1), new TimeSpan(9, 0, 0)));
        Assert.False(result.Succeeded);
        Assert.Contains("quá khứ", result.Error!);
    }

    [Fact]
    public async Task CreateAsync_ActiveStylistWithShift_IsAllowed()
    {
        await using var context = CreateContext();
        var (customer, stylist, service) = await AddBookingDataAsync(context);
        var day = DateTime.Today.AddDays(2);
        context.StylistSchedules.Add(new StylistSchedule { StylistId = stylist.Id, WorkDate = day, StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0) });
        await context.SaveChangesAsync();
        Assert.True((await Service(context).CreateAsync(Form(customer.Id, stylist.Id, service.Id, day, new TimeSpan(16, 30, 0)))).Succeeded);
    }

    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private static AppointmentService Service(ApplicationDbContext context) => new(context, NullLogger<AppointmentService>.Instance);
    private static AppointmentFormViewModel Form(int customerId, int stylistId, int serviceId, DateTime date, TimeSpan start) => new() { CustomerId = customerId, StylistId = stylistId, ServiceId = serviceId, AppointmentDate = date, StartTime = start };
    private static async Task<(Customer customer, Stylist stylist, SalonService service)> AddBookingDataAsync(ApplicationDbContext context)
    {
        var customer = new Customer { FullName = "Test Customer", Phone = "0900123456" };
        var stylist = new Stylist { FullName = "Test Stylist", Phone = "0900123457", Specialization = "Cut", Status = StylistStatus.Active };
        var service = new SalonService { Name = "Test Cut", Price = 100000, DurationMinutes = 60, Status = true };
        context.AddRange(customer, stylist, service); await context.SaveChangesAsync(); return (customer, stylist, service);
    }
}
