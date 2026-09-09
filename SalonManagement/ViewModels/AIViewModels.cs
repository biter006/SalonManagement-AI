using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalonManagement.Models;

namespace SalonManagement.ViewModels;

public class AIRecommendationViewModel
{
    [Display(Name = "Khách hàng")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khách hàng.")]
    public int CustomerId { get; set; }
    [Display(Name = "Nhu cầu hiện tại")]
    [Required(ErrorMessage = "Vui lòng nhập nhu cầu của khách.")]
    [StringLength(1000)]
    public string InputText { get; set; } = string.Empty;
    public string? Result { get; set; }
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class AIMessageViewModel
{
    [Display(Name = "Khách hàng")]
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    [Display(Name = "Lịch hẹn (nếu có)")] public int? AppointmentId { get; set; }
    [Display(Name = "Loại tin nhắn")] public MessageType MessageType { get; set; } = MessageType.Reminder;
    public string? Result { get; set; }
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Appointments { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class AISummaryViewModel
{
    [Display(Name = "Khách hàng")]
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    public string? Result { get; set; }
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
}
