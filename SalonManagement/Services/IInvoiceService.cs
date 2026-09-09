using SalonManagement.Models;

namespace SalonManagement.Services;

public interface IInvoiceService
{
    Task<OperationResult> CreateAsync(int appointmentId, PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task<OperationResult> MarkPaidAsync(int invoiceId, PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
}
