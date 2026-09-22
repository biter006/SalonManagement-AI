using SalonManagement.Models;

namespace SalonManagement.Services;

public interface IAIService
{
    Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default);
    Task<AITextResult> ChatAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, IReadOnlyCollection<AIConversationTurn> conversation, string message, CancellationToken cancellationToken = default);
    Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default);
    Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default);
}

public record AITextResult(bool Succeeded, string Text, string? Error = null, bool UsedFallback = false);
