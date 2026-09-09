using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class Customer
{
    public int Id { get; set; }

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    [StringLength(20)]
    public string? Gender { get; set; }

    [Display(Name = "Địa chỉ")]
    [StringLength(250)]
    public string? Address { get; set; }

    [Display(Name = "Ghi chú")]
    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
    public ICollection<AIRecommendation> AIRecommendations { get; set; } = new List<AIRecommendation>();
    public ICollection<AIGeneratedMessage> AIGeneratedMessages { get; set; } = new List<AIGeneratedMessage>();
}
