using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class AIGeneratedMessage
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public MessageType MessageType { get; set; }
    [StringLength(4000)]
    public string GeneratedMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
