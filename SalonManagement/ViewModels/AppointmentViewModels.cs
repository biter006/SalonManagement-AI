using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalonManagement.Models;

namespace SalonManagement.ViewModels;

public class AppointmentFormViewModel : IValidatableObject
{
    public int Id { get; set; }
    [Display(Name = "Khách hàng")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khách hàng.")]
    public int CustomerId { get; set; }
    [Display(Name = "Thợ phụ trách")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn thợ.")]
    public int StylistId { get; set; }
    [Display(Name = "Dịch vụ")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn dịch vụ.")]
    public int ServiceId { get; set; }
    [Display(Name = "Ngày hẹn")]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; } = DateTime.Today;
    [Display(Name = "Giờ bắt đầu")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0);
    [Display(Name = "Ghi chú của khách")]
    [StringLength(1000)]
    public string? CustomerNote { get; set; }
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Stylists { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Services { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AppointmentDate.Date < DateTime.Today)
            yield return new ValidationResult("Không thể đặt lịch trong quá khứ.", new[] { nameof(AppointmentDate) });
    }
}

public class AppointmentFilterViewModel
{
    [DataType(DataType.Date)] public DateTime? Date { get; set; }
    public int? StylistId { get; set; }
    public int? CustomerId { get; set; }
    public AppointmentStatus? Status { get; set; }
    public IEnumerable<SelectListItem> Stylists { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class AppointmentDetailsViewModel
{
    public Appointment Appointment { get; set; } = null!;
    public string? CancelReason { get; set; }
}
