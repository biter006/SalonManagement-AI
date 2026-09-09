using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;

namespace SalonManagement.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    public InvoiceService(ApplicationDbContext context) => _context = context;

    public async Task<OperationResult> CreateAsync(int appointmentId, PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == appointmentId, cancellationToken);
        if (appointment is null) return OperationResult.Failure("Lịch hẹn không tồn tại.");
        if (appointment.Status != AppointmentStatus.Completed) return OperationResult.Failure("Chỉ có thể lập hóa đơn cho lịch hẹn đã hoàn thành.");
        if (await _context.Invoices.AnyAsync(x => x.AppointmentId == appointmentId, cancellationToken)) return OperationResult.Failure("Lịch hẹn này đã có hóa đơn.");
        var invoice = new Invoice { AppointmentId = appointmentId, CustomerId = appointment.CustomerId, TotalAmount = appointment.Service!.Price, PaymentMethod = paymentMethod };
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success(invoice.Id);
    }

    public async Task<OperationResult> MarkPaidAsync(int invoiceId, PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices.FindAsync(new object[] { invoiceId }, cancellationToken);
        if (invoice is null) return OperationResult.Failure("Không tìm thấy hóa đơn.");
        if (invoice.PaymentStatus == PaymentStatus.Paid) return OperationResult.Failure("Hóa đơn đã được thanh toán.");
        invoice.PaymentMethod = paymentMethod;
        invoice.PaymentStatus = PaymentStatus.Paid;
        invoice.PaidAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success(invoiceId);
    }
}
