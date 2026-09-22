using SalonManagement.Models;

namespace SalonManagement.Services;

/// <summary>
/// Provides an explainable local ranking when an external AI provider is unavailable.
/// It also supplies the service cards shown to staff before they use an AI answer.
/// </summary>
public sealed class SalonConsultationProfile
{
    public string CustomerNeed { get; init; } = string.Empty;
    public string HairCondition { get; init; } = string.Empty;
    public string DesiredStyle { get; init; } = string.Empty;
    public decimal? Budget { get; init; }
    public string MaintenancePreference { get; init; } = string.Empty;

    public string ToPromptContext() => string.Join("\n", new[]
    {
        $"Nhu cầu: {CustomerNeed}",
        $"Tình trạng tóc: {HairCondition}",
        $"Mong muốn: {DesiredStyle}",
        Budget.HasValue ? $"Ngân sách tối đa: {Budget.Value:N0}đ" : "Ngân sách tối đa: chưa cung cấp",
        $"Mức độ chăm sóc tại nhà: {MaintenancePreference}"
    });
}

public sealed record SalonServiceSuggestion(SalonService Service, int Score, string Reason);

public static class SalonConsultationAdvisor
{
    public static IReadOnlyList<SalonServiceSuggestion> Build(
        IEnumerable<SalonService> services,
        IEnumerable<ServiceHistory> history,
        SalonConsultationProfile profile)
    {
        var normalized = profile.ToPromptContext().ToLowerInvariant();
        var usedServiceIds = history.GroupBy(x => x.ServiceId)
            .ToDictionary(x => x.Key, x => x.Count());

        return services.Where(x => x.Status).Select(service =>
        {
            var serviceText = $"{service.Name} {service.Description}".ToLowerInvariant();
            var score = 10;
            var reasons = new List<string>();

            AddScoreIfMatches(new[] { "khô", "xơ", "hư", "phục hồi", "yếu", "tẩy" }, new[] { "phục hồi", "dưỡng", "hấp", "treatment", "keratin" }, "phù hợp nhu cầu phục hồi tóc", normalized, serviceText, ref score, reasons);
            AddScoreIfMatches(new[] { "uốn", "xoăn", "bồng", "sóng" }, new[] { "uốn", "xoăn", "sóng" }, "hỗ trợ tạo độ xoăn/bồng", normalized, serviceText, ref score, reasons);
            AddScoreIfMatches(new[] { "nhuộm", "màu", "sáng", "tone" }, new[] { "nhuộm", "màu", "tẩy" }, "phù hợp mong muốn thay đổi màu tóc", normalized, serviceText, ref score, reasons);
            AddScoreIfMatches(new[] { "gọn", "cắt", "ngắn", "tỉa", "dễ chăm" }, new[] { "cắt", "tỉa", "tạo kiểu" }, "phù hợp nhu cầu tóc gọn, dễ chăm", normalized, serviceText, ref score, reasons);
            AddScoreIfMatches(new[] { "dầu", "gàu", "da đầu" }, new[] { "gội", "da đầu", "chăm sóc" }, "có liên quan đến chăm sóc da đầu", normalized, serviceText, ref score, reasons);

            // When the customer explicitly asks for a colour change, that new request
            // must take precedence over an older recovery-service history.
            var wantsColorChange = ContainsAny(normalized, "nhuộm", "màu", "lên màu", "đổi màu");
            var isColorService = ContainsAny(serviceText, "nhuộm", "màu", "tẩy");
            var isRecoveryService = ContainsAny(serviceText, "phục hồi", "dưỡng", "hấp", "keratin", "treatment");
            if (wantsColorChange && isColorService)
            {
                score += 20;
                reasons.Add("ưu tiên đúng nhu cầu đổi màu hiện tại");
            }
            else if (wantsColorChange && isRecoveryService)
            {
                score -= 12;
            }

            if (profile.Budget is > 0)
            {
                if (service.Price <= profile.Budget)
                {
                    score += 8;
                    reasons.Add("nằm trong ngân sách");
                }
                else
                {
                    score -= 8;
                    reasons.Add("vượt ngân sách, cần xác nhận lại");
                }
            }

            // Lịch sử chỉ là tín hiệu phụ: không được lấn át nhu cầu mới của khách.
            if (usedServiceIds.TryGetValue(service.Id, out var count))
            {
                score += Math.Min(count * 2, 4);
                reasons.Add("khách đã từng sử dụng");
            }

            var reason = reasons.Count == 0
                ? "là dịch vụ đang được salon cung cấp; cần thợ kiểm tra trực tiếp trước khi chốt"
                : string.Join(", ", reasons.Distinct()) + "; cần thợ kiểm tra trực tiếp trước khi chốt";
            return new SalonServiceSuggestion(service, score, reason);
        })
        .OrderByDescending(x => x.Score)
        .ThenBy(x => x.Service.Price)
        .Take(3)
        .ToList();
    }

    private static void AddScoreIfMatches(string[] customerWords, string[] serviceWords, string reason,
        string normalized, string serviceText, ref int score, List<string> reasons)
    {
        if (customerWords.Any(word => normalized.Contains(word, StringComparison.Ordinal)) &&
            serviceWords.Any(word => serviceText.Contains(word, StringComparison.Ordinal)))
        {
            score += 18;
            reasons.Add(reason);
        }
    }

    private static bool ContainsAny(string value, params string[] words) =>
        words.Any(word => value.Contains(word, StringComparison.Ordinal));
}
