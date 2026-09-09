using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Models;

public class AIRecommendation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    [StringLength(2000)]
    public string InputText { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
