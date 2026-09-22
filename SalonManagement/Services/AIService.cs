using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SalonManagement.Models;

namespace SalonManagement.Services;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AIOptions _options;
    private readonly SalonPromptsOptions _prompts;
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory httpClientFactory, IOptions<AIOptions> options, IOptions<SalonPromptsOptions> prompts, ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _prompts = prompts.Value;
        _logger = logger;
    }

    public async Task<AITextResult> RecommendAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, IReadOnlyCollection<SalonService> services, string need, CancellationToken cancellationToken = default)
    {
        var active = services.Where(x => x.Status).ToList();
        if (active.Count == 0) return new AITextResult(false, string.Empty, "Salon hiện chưa có dịch vụ đang cung cấp để gợi ý.");
        var prompt = ApplyTemplate(_prompts.Recommendation,
            ("customerName", customer.FullName),
            ("need", LimitText(need, 1000)),
            ("services", string.Join("; ", active.Select(x => $"{x.Name} ({x.Price:N0}đ, {x.DurationMinutes} phút)"))),
            ("history", FormatHistory(history)));
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);
        var suggestions = SalonConsultationAdvisor.Build(active, history, new SalonConsultationProfile { CustomerNeed = need });
        var primary = suggestions.First();
        var lines = suggestions.Select((x, index) =>
            $"{index + 1}. {x.Service.Name} — {x.Service.Price:N0}đ / {x.Service.DurationMinutes} phút. Lý do: {x.Reason}.");
        return new AITextResult(true, $"Dịch vụ đề xuất: {primary.Service.Name}.\n{string.Join("\n", lines)}\n\nLưu ý: Xác nhận tình trạng tóc thực tế và mong muốn của khách với thợ trước khi thực hiện.", UsedFallback: true);
    }

    public async Task<AITextResult> ChatAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history,
        IReadOnlyCollection<SalonService> services, IReadOnlyCollection<AIConversationTurn> conversation,
        string message, CancellationToken cancellationToken = default)
    {
        var active = services.Where(x => x.Status).ToList();
        if (active.Count == 0) return new AITextResult(false, string.Empty, "Salon hiện chưa có dịch vụ để tư vấn.");

        var prompt = ApplyTemplate(_prompts.Chat,
            ("customerName", customer.FullName),
            ("message", LimitText(message, 600)),
            ("history", FormatHistory(history)),
            ("services", string.Join("; ", active.Select(x => $"{x.Name} ({x.Price:N0}đ, {x.DurationMinutes} phút)"))),
            ("conversation", FormatConversation(conversation)));
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);

        var suggestions = SalonConsultationAdvisor.Build(active, history, new SalonConsultationProfile { CustomerNeed = message });
        var top = suggestions.FirstOrDefault();
        if (top is null) return new AITextResult(false, string.Empty, "Chưa tìm được dịch vụ phù hợp.");
        var other = suggestions.Skip(1).Select(x => x.Service.Name).ToList();
        var alternative = other.Count == 0 ? string.Empty : $" Bạn cũng có thể cân nhắc: {string.Join(", ", other)}.";
        return new AITextResult(true,
            $"Dựa trên câu hỏi của {customer.FullName}, bạn có thể tham khảo {top.Service.Name} ({top.Service.Price:N0}đ, {top.Service.DurationMinutes} phút) vì {top.Reason}.{alternative} Thợ cần kiểm tra trực tiếp tình trạng tóc trước khi chốt dịch vụ.", UsedFallback: true);
    }

    public async Task<AITextResult> GenerateMessageAsync(Customer customer, Appointment? appointment, MessageType type, CancellationToken cancellationToken = default)
    {
        if (type == MessageType.Reminder && appointment is null) return new AITextResult(false, string.Empty, "Tin nhắn nhắc lịch cần có lịch hẹn.");
        var appointmentInfo = appointment is null ? "Không có lịch hẹn cụ thể." : $"Lịch hẹn: {appointment.AppointmentDate:dd/MM/yyyy} lúc {appointment.StartTime:hh\\:mm}; dịch vụ: {appointment.Service?.Name ?? "chưa xác định"}; thợ: {appointment.Stylist?.FullName ?? "chưa xác định"}.";
        var prompt = ApplyTemplate(_prompts.Message,
            ("customerName", customer.FullName),
            ("messageType", type.ToString()),
            ("appointmentInfo", appointmentInfo));
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);
        var message = type switch
        {
            MessageType.Reminder => $"Salon xin nhắc {customer.FullName} có lịch {appointment!.Service?.Name} vào {appointment.AppointmentDate:dd/MM/yyyy} lúc {appointment.StartTime:hh\\:mm} cùng {appointment.Stylist?.FullName}. Hẹn gặp bạn tại salon!",
            MessageType.ThankYou => $"Salon cảm ơn {customer.FullName} đã sử dụng dịch vụ. Chúc bạn luôn hài lòng với mái tóc mới!",
            MessageType.FollowUp => $"Salon xin hỏi thăm {customer.FullName}: mái tóc của bạn sau dịch vụ hiện thế nào ạ? Chúng tôi luôn sẵn sàng hỗ trợ.",
            _ => $"Salon rất mong được đón {customer.FullName} trở lại để chăm sóc mái tóc của bạn. Liên hệ chúng tôi để được tư vấn lịch phù hợp nhé!"
        };
        return new AITextResult(true, message, UsedFallback: true);
    }

    public async Task<AITextResult> SummarizeHistoryAsync(Customer customer, IReadOnlyCollection<ServiceHistory> history, CancellationToken cancellationToken = default)
    {
        if (history.Count == 0) return new AITextResult(true, $"Khách {customer.FullName} chưa có lịch sử dịch vụ để tóm tắt.", UsedFallback: true);
        var prompt = ApplyTemplate(_prompts.Summary,
            ("customerName", customer.FullName),
            ("history", FormatHistory(history)));
        var remote = await TryGenerateAsync(prompt, cancellationToken);
        if (remote is not null) return new AITextResult(true, remote);
        var latest = history.OrderByDescending(x => x.ServiceDate).Take(3).ToList();
        var serviceNames = string.Join(", ", latest.Select(x => x.Service?.Name ?? "dịch vụ chưa xác định"));
        var notes = latest.Where(x => !string.IsNullOrWhiteSpace(x.Notes)).Select(x => x.Notes!.Trim()).ToList();
        var noteText = notes.Count == 0 ? "Chưa có ghi chú cụ thể trong các lần gần đây." : $"Ghi chú gần đây: {string.Join("; ", notes)}.";
        return new AITextResult(true, $"Khách đã sử dụng gần đây: {serviceNames}. {noteText}", UsedFallback: true);
    }

    private async Task<string?> TryGenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.Model)) return null;
        if (string.Equals(_options.Provider, "Gemini", StringComparison.OrdinalIgnoreCase)) return await TryGenerateWithGeminiAsync(prompt, cancellationToken);
        if (string.Equals(_options.Provider, "OpenAI", StringComparison.OrdinalIgnoreCase)) return await TryGenerateWithOpenAIAsync(prompt, cancellationToken);
        return null;
    }

    private async Task<string?> TryGenerateWithOpenAIAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            var payload = new { model = _options.Model, input = $"{_prompts.System}\n\n{prompt}" };
            using var response = await client.PostAsync(_options.Endpoint, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            if (document.RootElement.TryGetProperty("output_text", out var outputText) && !string.IsNullOrWhiteSpace(outputText.GetString())) return outputText.GetString();
            _logger.LogWarning("AI provider returned an empty response.");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogError(ex, "OpenAI provider call failed; using local fallback.");
        }
        return null;
    }

    private async Task<string?> TryGenerateWithGeminiAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Add("x-goog-api-key", _options.ApiKey);
            var endpoint = $"{_options.GeminiEndpoint.TrimEnd('/')}/models/{Uri.EscapeDataString(_options.Model)}:generateContent";
            var payload = new
            {
                contents = new[] { new { parts = new[] { new { text = $"{_prompts.System}\n\n{prompt}" } } } },
                generationConfig = new { temperature = 0.3, maxOutputTokens = 500 }
            };
            const int maximumAttempts = 3;
            for (var attempt = 1; attempt <= maximumAttempts; attempt++)
            {
                using var response = await client.PostAsync(endpoint, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"), cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var retryable = response.StatusCode is System.Net.HttpStatusCode.TooManyRequests or System.Net.HttpStatusCode.ServiceUnavailable;
                    if (retryable && attempt < maximumAttempts)
                    {
                        var delay = TimeSpan.FromSeconds(attempt);
                        _logger.LogWarning("Gemini returned HTTP {StatusCode} for model {Model}; retry {Attempt}/{MaximumAttempts} after {DelaySeconds}s.",
                            (int)response.StatusCode, _options.Model, attempt, maximumAttempts, delay.TotalSeconds);
                        await Task.Delay(delay, cancellationToken);
                        continue;
                    }
                    _logger.LogWarning("Gemini returned HTTP {StatusCode} for model {Model} at {Endpoint}. Detail: {Detail}",
                        (int)response.StatusCode, _options.Model, endpoint, LimitText(responseBody, 1000));
                    return null;
                }
                using var document = JsonDocument.Parse(responseBody);
                var text = document.RootElement
                    .GetProperty("candidates")
                    .EnumerateArray()
                    .SelectMany(x => x.GetProperty("content").GetProperty("parts").EnumerateArray())
                    .Select(x => x.TryGetProperty("text", out var value) ? value.GetString() : null)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                if (!string.IsNullOrWhiteSpace(text)) return text;
                _logger.LogWarning("Gemini provider returned an empty response.");
                return null;
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or KeyNotFoundException or InvalidOperationException)
        {
            _logger.LogError(ex, "Gemini provider call failed; using local fallback.");
        }
        return null;
    }

    private static string FormatHistory(IEnumerable<ServiceHistory> history) => string.Join("; ", history.OrderByDescending(x => x.ServiceDate).Take(5).Select(x => $"{x.ServiceDate:dd/MM/yyyy}: {x.Service?.Name ?? "không rõ"}; ghi chú: {x.Notes ?? "không có"}"));
    private static string FormatConversation(IEnumerable<AIConversationTurn> conversation) => string.Join(" | ", conversation.TakeLast(6).Select(x => $"{x.Role}: {LimitText(x.Text, 300)}"));
    private static string LimitText(string value, int maximum) => value.Length <= maximum ? value : value[..maximum];
    private static string ApplyTemplate(string template, params (string Name, string Value)[] values)
    {
        foreach (var (name, value) in values) template = template.Replace($"{{{{{name}}}}}", value, StringComparison.Ordinal);
        return template;
    }
}
