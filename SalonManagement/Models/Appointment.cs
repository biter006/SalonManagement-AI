using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class Appointment
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int StylistId { get; set; }
    public Stylist? Stylist { get; set; }
    public int ServiceId { get; set; }
    public SalonService? Service { get; set; }

    [Display(Name = "Ngày hẹn")]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; }

    [Display(Name = "Giờ bắt đầu")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Display(Name = "Giờ kết thúc")]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    [Display(Name = "Trạng thái")]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    [Display(Name = "Ghi chú của khách")]
    [StringLength(1000)]
    public string? CustomerNote { get; set; }

    [Display(Name = "Ghi chú của nhân viên")]
    [StringLength(1000)]
    public string? StaffNote { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Invoice? Invoice { get; set; }
    public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
    public ICollection<AIGeneratedMessage> AIGeneratedMessages { get; set; } = new List<AIGeneratedMessage>();
}
