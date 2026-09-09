using System.Data;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Data;
using SalonManagement.Models;
using SalonManagement.ViewModels;

namespace SalonManagement.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AppointmentService> _logger;
    public AppointmentService(ApplicationDbContext context, ILogger<AppointmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OperationResult> CreateAsync(AppointmentFormViewModel input, CancellationToken cancellationToken = default)
    {
        var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
            : null;
        try
        {
        var validation = await ValidateSlotAsync(input, null, cancellationToken);
        if (!validation.Succeeded) return validation;
        var service = await _context.SalonServices.FindAsync(new object[] { input.ServiceId }, cancellationToken);
        var appointment = new Appointment
        {
            CustomerId = input.CustomerId, StylistId = input.StylistId, ServiceId = input.ServiceId,
            AppointmentDate = input.AppointmentDate.Date, StartTime = input.StartTime,
            EndTime = input.StartTime.Add(TimeSpan.FromMinutes(service!.DurationMinutes)), CustomerNote = input.CustomerNote,
            Status = AppointmentStatus.Pending
        };
        _context.Appointments.Add(appointment);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null) await transaction.CommitAsync(cancellationToken);
            return OperationResult.Success(appointment.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating appointment for stylist {StylistId}", input.StylistId);
            return OperationResult.Failure("Không thể lưu lịch hẹn lúc này. Vui lòng thử lại.");
        }
        }
        finally
        {
            if (transaction is not null) await transaction.DisposeAsync();
        }
    }

    public async Task<OperationResult> RescheduleAsync(int id, AppointmentFormViewModel input, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        if (appointment is null) return OperationResult.Failure("Không tìm thấy lịch hẹn.");
        if (appointment.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.NoShow)
            return OperationResult.Failure("Lịch hẹn ở trạng thái hiện tại không thể đổi lịch.");
        var validation = await ValidateSlotAsync(input, id, cancellationToken);
        if (!validation.Succeeded) return validation;
        var service = await _context.SalonServices.FindAsync(new object[] { input.ServiceId }, cancellationToken);
        appointment.CustomerId = input.CustomerId;
        appointment.StylistId = input.StylistId;
        appointment.ServiceId = input.ServiceId;
        appointment.AppointmentDate = input.AppointmentDate.Date;
        appointment.StartTime = input.StartTime;
        appointment.EndTime = input.StartTime.Add(TimeSpan.FromMinutes(service!.DurationMinutes));
        appointment.CustomerNote = input.CustomerNote;
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult.Success(id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while rescheduling appointment {AppointmentId}", id);
            return OperationResult.Failure("Không thể đổi lịch hẹn lúc này. Vui lòng thử lại.");
        }
    }

    public async Task<OperationResult> CancelAsync(int id, string? reason, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        if (appointment is null) return OperationResult.Failure("Không tìm thấy lịch hẹn.");
        if (appointment.Status == AppointmentStatus.Completed) return OperationResult.Failure("Không thể hủy lịch đã hoàn thành.");
        if (appointment.Status == AppointmentStatus.Cancelled) return OperationResult.Failure("Lịch hẹn đã được hủy.");
        appointment.Status = AppointmentStatus.Cancelled;
        appointment.StaffNote = string.IsNullOrWhiteSpace(reason) ? appointment.StaffNote : reason;
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success(id);
    }

    public async Task<OperationResult> CompleteAsync(int id, string? staffNote, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (appointment is null) return OperationResult.Failure("Không tìm thấy lịch hẹn.");
        if (appointment.Status is AppointmentStatus.Cancelled or AppointmentStatus.NoShow) return OperationResult.Failure("Không thể hoàn thành lịch đã hủy hoặc vắng mặt.");
        appointment.Status = AppointmentStatus.Completed;
        appointment.StaffNote = staffNote;
        if (!await _context.ServiceHistories.AnyAsync(x => x.AppointmentId == id, cancellationToken))
        {
            _context.ServiceHistories.Add(new ServiceHistory
            {
                CustomerId = appointment.CustomerId, AppointmentId = appointment.Id, ServiceId = appointment.ServiceId,
                StylistId = appointment.StylistId, ServiceDate = appointment.AppointmentDate, Notes = staffNote,
                ResultDescription = "Đã hoàn thành dịch vụ."
            });
        }
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success(id);
    }

    private async Task<OperationResult> ValidateSlotAsync(AppointmentFormViewModel input, int? excludedAppointmentId, CancellationToken cancellationToken)
    {
        if (input.AppointmentDate.Date < DateTime.Today) return OperationResult.Failure("Không thể đặt lịch trong quá khứ.");
        var customerExists = await _context.Customers.AnyAsync(x => x.Id == input.CustomerId, cancellationToken);
        var stylist = await _context.Stylists.FindAsync(new object[] { input.StylistId }, cancellationToken);
        var service = await _context.SalonServices.FindAsync(new object[] { input.ServiceId }, cancellationToken);
        if (!customerExists) return OperationResult.Failure("Khách hàng không tồn tại.");
        if (stylist is null || stylist.Status != StylistStatus.Active) return OperationResult.Failure("Thợ không tồn tại hoặc hiện không sẵn sàng.");
        if (service is null || !service.Status) return OperationResult.Failure("Dịch vụ không tồn tại hoặc đã ngừng cung cấp.");
        var end = input.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
        var hasShift = await _context.StylistSchedules.AnyAsync(x => x.StylistId == input.StylistId && x.WorkDate.Date == input.AppointmentDate.Date && x.Status == ScheduleStatus.Working && x.StartTime <= input.StartTime && x.EndTime >= end, cancellationToken);
        if (!hasShift) return OperationResult.Failure("Khung giờ đã chọn không nằm trọn trong ca làm việc của thợ.");
        var conflict = await _context.Appointments.AnyAsync(x => x.StylistId == input.StylistId && x.AppointmentDate.Date == input.AppointmentDate.Date && (x.Status == AppointmentStatus.Pending || x.Status == AppointmentStatus.Confirmed) && (!excludedAppointmentId.HasValue || x.Id != excludedAppointmentId.Value) && input.StartTime < x.EndTime && end > x.StartTime, cancellationToken);
        if (conflict)
        {
            _logger.LogWarning("Appointment conflict for stylist {StylistId} at {Date} {Start}", input.StylistId, input.AppointmentDate.Date, input.StartTime);
            return OperationResult.Failure("Thợ đã có lịch trùng với khung giờ này.");
        }
        return OperationResult.Success();
    }
}
