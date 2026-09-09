using SalonManagement.Models;
using SalonManagement.ViewModels;

namespace SalonManagement.Services;

public interface IAppointmentService
{
    Task<OperationResult> CreateAsync(AppointmentFormViewModel input, CancellationToken cancellationToken = default);
    Task<OperationResult> RescheduleAsync(int id, AppointmentFormViewModel input, CancellationToken cancellationToken = default);
    Task<OperationResult> CancelAsync(int id, string? reason, CancellationToken cancellationToken = default);
    Task<OperationResult> CompleteAsync(int id, string? staffNote, CancellationToken cancellationToken = default);
}
