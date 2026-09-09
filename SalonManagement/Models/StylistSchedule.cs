using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class StylistSchedule : IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Thợ")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thợ.")]
    public int StylistId { get; set; }
    public Stylist? Stylist { get; set; }

    [Display(Name = "Ngày làm việc")]
    [DataType(DataType.Date)]
    public DateTime WorkDate { get; set; }

    [Display(Name = "Bắt đầu")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Display(Name = "Kết thúc")]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    [Display(Name = "Trạng thái")]
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Working;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == ScheduleStatus.Working && EndTime <= StartTime)
            yield return new ValidationResult("Giờ kết thúc phải sau giờ bắt đầu.", new[] { nameof(EndTime) });
    }
}
