using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalonManagement.Models;

namespace SalonManagement.ViewModels;

public class InvoiceCreateViewModel
{
    [Display(Name = "Lịch hẹn đã hoàn thành")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn lịch hẹn.")]
    public int AppointmentId { get; set; }
    [Display(Name = "Phương thức thanh toán")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public IEnumerable<SelectListItem> Appointments { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class PaymentViewModel
{
    public int InvoiceId { get; set; }
    [Display(Name = "Phương thức thanh toán")]
    public PaymentMethod PaymentMethod { get; set; }
}
