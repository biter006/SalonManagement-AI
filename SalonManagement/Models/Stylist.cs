using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class Stylist
{
    public int Id { get; set; }

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [Display(Name = "Chuyên môn")]
    [Required(ErrorMessage = "Vui lòng nhập chuyên môn.")]
    [StringLength(200)]
    public string Specialization { get; set; } = string.Empty;

    [Display(Name = "Kinh nghiệm (năm)")]
    [Range(0, 60)]
    public int Experience { get; set; }

    [Display(Name = "Trạng thái")]
    public StylistStatus Status { get; set; } = StylistStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<StylistSchedule> Schedules { get; set; } = new List<StylistSchedule>();
    public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
}
