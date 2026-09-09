using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalonManagement.Models;

public class SalonService
{
    public int Id { get; set; }

    [Display(Name = "Tên dịch vụ")]
    [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [Display(Name = "Giá")]
    [Range(1000, 100000000, ErrorMessage = "Giá dịch vụ phải lớn hơn 0.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Display(Name = "Thời lượng (phút)")]
    [Range(1, 1440, ErrorMessage = "Thời lượng phải lớn hơn 0.")]
    public int DurationMinutes { get; set; }

    [Display(Name = "Đang cung cấp")]
    public bool Status { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
}
