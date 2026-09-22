using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalonManagement.Models;
using SalonManagement.Services;

namespace SalonManagement.ViewModels;

public class AIRecommendationViewModel
{
    [Display(Name = "Phiên bản prompt KT3")]
    public RecommendationPromptVersion PromptVersion { get; set; } = RecommendationPromptVersion.V3;
    [Display(Name = "Khách hàng")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khách hàng.")]
    public int CustomerId { get; set; }
    [Display(Name = "Nhu cầu hiện tại")]
    [Required(ErrorMessage = "Vui lòng nhập nhu cầu của khách.")]
    [StringLength(1000, ErrorMessage = "Nhu cầu của khách không được vượt quá 1.000 ký tự.")]
    public string InputText { get; set; } = string.Empty;
    [Display(Name = "Tình trạng tóc")]
    [Required(ErrorMessage = "Vui lòng chọn tình trạng tóc.")]
    public string HairCondition { get; set; } = string.Empty;
    [Display(Name = "Mong muốn của khách")]
    [Required(ErrorMessage = "Vui lòng chọn mong muốn của khách.")]
    public string DesiredStyle { get; set; } = string.Empty;
    [Display(Name = "Ngân sách tối đa (VNĐ)")]
    [Range(50000, 100000000, ErrorMessage = "Ngân sách phải từ 50.000 VNĐ.")]
    public decimal? Budget { get; set; }
    [Display(Name = "Mức độ chăm sóc tại nhà")]
    [Required(ErrorMessage = "Vui lòng chọn mức độ chăm sóc tại nhà.")]
    public string MaintenancePreference { get; set; } = string.Empty;
    public string? Result { get; set; }
    public bool UsedFallback { get; set; }
    public IReadOnlyList<AIServiceSuggestionViewModel> SuggestedServices { get; set; } = Array.Empty<AIServiceSuggestionViewModel>();
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
}

public sealed class AIServiceSuggestionViewModel
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class AIChatViewModel
{
    [Display(Name = "Khách hàng")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khách hàng.")]
    public int CustomerId { get; set; }
    [Display(Name = "Câu hỏi tư vấn")]
    [Required(ErrorMessage = "Vui lòng nhập câu hỏi.")]
    [StringLength(600, ErrorMessage = "Câu hỏi không được vượt quá 600 ký tự.")]
    public string Message { get; set; } = string.Empty;
    public IReadOnlyList<AIChatTurnViewModel> Turns { get; set; } = Array.Empty<AIChatTurnViewModel>();
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
}

public sealed record AIChatTurnViewModel(string Role, string Text, bool UsedFallback = false);

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
