using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.Services;

namespace SalonManagement.Tests;

public class InvoiceServiceTests
{
    [Fact]
    public async Task CreateAsync_CompletedAppointment_UsesServicePrice()
    {
        await using var context = Context();
        var appointment = await AddAppointmentAsync(context, AppointmentStatus.Completed, 345000);
        var result = await new InvoiceService(context).CreateAsync(appointment.Id, PaymentMethod.BankTransfer);
        var invoice = await context.Invoices.SingleAsync();
        Assert.True(result.Succeeded);
        Assert.Equal(345000, invoice.TotalAmount);
        Assert.Equal(PaymentStatus.Unpaid, invoice.PaymentStatus);
    }

    [Fact]
    public async Task CreateAsync_UnfinishedAppointment_IsRejected()
    {
        await using var context = Context();
        var appointment = await AddAppointmentAsync(context, AppointmentStatus.Confirmed, 100000);
        var result = await new InvoiceService(context).CreateAsync(appointment.Id, PaymentMethod.Cash);
        Assert.False(result.Succeeded);
        Assert.Empty(context.Invoices);
    }

    private static ApplicationDbContext Context() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private static async Task<Appointment> AddAppointmentAsync(ApplicationDbContext context, AppointmentStatus status, decimal price)
    {
        var customer = new Customer { FullName = "Test", Phone = "0900111111" };
        var stylist = new Stylist { FullName = "Stylist", Phone = "0900222222", Specialization = "Test" };
        var service = new SalonService { Name = "Service", Price = price, DurationMinutes = 60 };
        context.AddRange(customer, stylist, service); await context.SaveChangesAsync();
        var appointment = new Appointment { CustomerId = customer.Id, StylistId = stylist.Id, ServiceId = service.Id, AppointmentDate = DateTime.Today, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Status = status };
        context.Appointments.Add(appointment); await context.SaveChangesAsync(); return appointment;
    }
}
