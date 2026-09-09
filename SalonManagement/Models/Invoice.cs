using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalonManagement.Models;

public class Invoice
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Display(Name = "Tổng tiền")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "Phương thức thanh toán")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "Trạng thái thanh toán")]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    [Display(Name = "Thời điểm thanh toán")]
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
