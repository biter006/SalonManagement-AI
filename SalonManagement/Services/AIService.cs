using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SalonManagement.Models;

namespace SalonManagement.Services;

public class AIService : IAIService
{
    private const string SystemPrompt = "You are an assistant for a hair salon. Only recommend services that exist in the provided service list. Do not invent services, prices, duration, or salon policies. Use the customer's history and current needs to make recommendations.";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AIOptions _options;
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory httpClientFactory, IOptions<AIOptions> options, ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default)
    {
        var active = services.Where(x => x.Status).ToList();
        if (active.Count == 0) return new AITextResult(false, string.Empty, "Salon hiện chưa có dịch vụ đang cung cấp để gợi ý.");
        var prompt = $"Khách: {customer.FullName}\nNhu cầu: {need}\nDịch vụ hiện có: {string.Join("; ", active.Select(x => $"{x.Name} ({x.Price:N0}đ, {x.DurationMinutes} phút)"))}\nLịch sử: {FormatHistory(history)}\nTrả lời bằng tiếng Việt với: dịch vụ đề xuất, lý do, mức độ phù hợp, gợi ý tiếp theo.";
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);
        var mostUsedId = history.GroupBy(x => x.ServiceId).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault();
        var chosen = active.FirstOrDefault(x => x.Id == mostUsedId) ?? active.First();
        return new AITextResult(true, $"Dịch vụ đề xuất: {chosen.Name}.\nLý do: {BuildReason(customer, history, need)}\nMức độ phù hợp: Phù hợp để trao đổi thêm với khách.\nGợi ý tiếp theo: Xác nhận tình trạng tóc hiện tại và đặt lịch tư vấn với thợ trước khi thực hiện.");
    }

    public async Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default)
    {
        if (type == MessageType.Reminder && appointment is null) return new AITextResult(false, string.Empty, "Tin nhắn nhắc lịch cần có lịch hẹn.");
        var appointmentInfo = appointment is null ? "Không có lịch hẹn cụ thể." : $"Lịch hẹn: {appointment.AppointmentDate:dd/MM/yyyy} lúc {appointment.StartTime:hh\\:mm}; dịch vụ: {appointment.Service?.Name ?? "chưa xác định"}; thợ: {appointment.Stylist?.FullName ?? "chưa xác định"}.";
        var prompt = $"Viết một tin nhắn tiếng Việt lịch sự cho khách {customer.FullName}. Loại: {type}. {appointmentInfo} Không tự bịa thông tin.";
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);
        var message = type switch
        {
            MessageType.Reminder => $"Salon xin nhắc {customer.FullName} có lịch {appointment!.Service?.Name} vào {appointment.AppointmentDate:dd/MM/yyyy} lúc {appointment.StartTime:hh\\:mm} cùng {appointment.Stylist?.FullName}. Hẹn gặp bạn tại salon!",
            MessageType.ThankYou => $"Salon cảm ơn {customer.FullName} đã sử dụng dịch vụ. Chúc bạn luôn hài lòng với mái tóc mới!",
            MessageType.FollowUp => $"Salon xin hỏi thăm {customer.FullName}: mái tóc của bạn sau dịch vụ hiện thế nào ạ? Chúng tôi luôn sẵn sàng hỗ trợ.",
            _ => $"Salon rất mong được đón {customer.FullName} trở lại để chăm sóc mái tóc của bạn. Liên hệ chúng tôi để được tư vấn lịch phù hợp nhé!"
        };
        return new AITextResult(true, message);
    }

    public async Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default)
    {
        if (history.Count == 0) return new AITextResult(true, $"Khách {customer.FullName} chưa có lịch sử dịch vụ để tóm tắt.");
        var prompt = $"Tóm tắt ngắn lịch sử tóc của khách {customer.FullName}. Chỉ dùng dữ liệu sau, không suy diễn: {FormatHistory(history)}";
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);
        var latest = history.OrderByDescending(x => x.ServiceDate).Take(3).ToList();
        var serviceNames = string.Join(", ", latest.Select(x => x.Service?.Name ?? "dịch vụ chưa xác định"));
        var notes = latest.Where(x => !string.IsNullOrWhiteSpace(x.Notes)).Select(x => x.Notes!.Trim()).ToList();
        var noteText = notes.Count == 0 ? "Chưa có ghi chú cụ thể trong các lần gần đây." : $"Ghi chú gần đây: {string.Join("; ", notes)}.";
        return new AITextResult(true, $"Khách đã sử dụng gần đây: {serviceNames}. {noteText}");
    }

    private async Task<string?> TryGenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        if (!string.Equals(_options.Provider, "OpenAI", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.Model)) return null;
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            var payload = new { model = _options.Model, input = $"{SystemPrompt}\n\n{prompt}" };
            using var response = await client.PostAsync(_options.Endpoint, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            if (document.RootElement.TryGetProperty("output_text", out var outputText) && !string.IsNullOrWhiteSpace(outputText.GetString())) return outputText.GetString();
            _logger.LogWarning("AI provider returned an empty response.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(ex, "AI provider call failed; using local fallback.");
        }
        return null;
    }

    private static string FormatHistory(IEnumerable<ServiceHistory> history) => string.Join("; ", history.OrderByDescending(x => x.ServiceDate).Take(5).Select(x => $"{x.ServiceDate:dd/MM/yyyy}: {x.Service?.Name ?? "không rõ"}; ghi chú: {x.Notes ?? "không có"}"));
    private static string BuildReason(Customer customer, IReadOnlyCollection<ServiceHistory> history, string need) => history.Count > 0 ? $"Khách có lịch sử dịch vụ liên quan và đang nêu nhu cầu: {need}." : $"Dựa trên nhu cầu khách cung cấp: {need}. Khách chưa có đủ lịch sử dịch vụ.";
}
