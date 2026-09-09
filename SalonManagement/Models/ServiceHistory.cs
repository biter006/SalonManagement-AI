using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class ServiceHistory
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public int ServiceId { get; set; }
    public SalonService? Service { get; set; }
    public int StylistId { get; set; }
    public Stylist? Stylist { get; set; }

    [DataType(DataType.Date)]
    public DateTime ServiceDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Display(Name = "Kết quả")]
    [StringLength(1000)]
    public string? ResultDescription { get; set; }
}
