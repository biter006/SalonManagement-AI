using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SalonManagement.Models;
using SalonManagement.Services;

namespace SalonManagement.Tests;

public class AIServiceTests
{
    [Fact]
    public async Task GeminiProvider_UsesKeyHeader_AndReadsGeneratedText()
    {
        var handler = new RecordingHandler("""{"candidates":[{"content":{"parts":[{"text":"Gợi ý Gemini hợp lệ"}]}}]}""");
        var service = new AIService(new TestHttpClientFactory(new HttpClient(handler)), Options.Create(new AIOptions
        {
            Provider = "Gemini",
            ApiKey = "test-gemini-key",
            Model = "gemini-test-model"
        }), Options.Create(Prompts()), NullLogger<AIService>.Instance);

        var result = await service.RecommendAsync(new Customer { FullName = "Khách test", Phone = "0900000000" }, [], [new SalonService { Name = "Cắt tóc", Price = 100000, DurationMinutes = 30, Status = true }], "Muốn tóc gọn");

        Assert.True(result.Succeeded);
        Assert.Equal("Gợi ý Gemini hợp lệ", result.Text);
        Assert.NotNull(handler.Request);
        Assert.Equal("test-gemini-key", handler.Request!.Headers.GetValues("x-goog-api-key").Single());
        Assert.Contains("models/gemini-test-model:generateContent", handler.Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task GeminiProvider_InvalidResponse_UsesLocalFallback()
    {
        var service = new AIService(new TestHttpClientFactory(new HttpClient(new RecordingHandler("{}"))), Options.Create(new AIOptions
        {
            Provider = "Gemini",
            ApiKey = "test-gemini-key",
            Model = "gemini-test-model"
        }), Options.Create(Prompts()), NullLogger<AIService>.Instance);

        var result = await service.RecommendAsync(new Customer { FullName = "Khách test", Phone = "0900000001" }, [], [new SalonService { Name = "Cắt tóc", Price = 100000, DurationMinutes = 30, Status = true }], "Muốn tóc gọn");

        Assert.True(result.Succeeded);
        Assert.Contains("Dịch vụ đề xuất: Cắt tóc", result.Text);
    }

    [Fact]
    public async Task GeminiProvider_Http404_UsesLocalFallback()
    {
        var handler = new RecordingHandler("""{"error":{"message":"Model unavailable"}}""", HttpStatusCode.NotFound);
        var service = new AIService(new TestHttpClientFactory(new HttpClient(handler)), Options.Create(new AIOptions
        {
            Provider = "Gemini", ApiKey = "test-gemini-key", Model = "missing-model"
        }), Options.Create(Prompts()), NullLogger<AIService>.Instance);

        var result = await service.RecommendAsync(new Customer { FullName = "Khách test", Phone = "0900000002" }, [], [new SalonService { Name = "Cắt tóc", Price = 100000, DurationMinutes = 30, Status = true }], "Muốn tóc gọn");

        Assert.True(result.Succeeded);
        Assert.True(result.UsedFallback);
        Assert.Contains("Dịch vụ đề xuất: Cắt tóc", result.Text);
    }

    [Fact]
    public async Task GeminiProvider_Retries503_AndUsesLaterSuccessfulResponse()
    {
        var handler = new SequenceHandler(
            (HttpStatusCode.ServiceUnavailable, """{"error":{"message":"Busy"}}"""),
            (HttpStatusCode.OK, """{"candidates":[{"content":{"parts":[{"text":"Gemini phục hồi sau retry"}]}}]}"""));
        var service = new AIService(new TestHttpClientFactory(new HttpClient(handler)), Options.Create(new AIOptions
        {
            Provider = "Gemini", ApiKey = "test-gemini-key", Model = "gemini-test-model"
        }), Options.Create(Prompts()), NullLogger<AIService>.Instance);

        var result = await service.RecommendAsync(new Customer { FullName = "Khách test", Phone = "0900000003" }, [], [new SalonService { Name = "Cắt tóc", Price = 100000, DurationMinutes = 30, Status = true }], "Muốn tóc gọn");

        Assert.True(result.Succeeded);
        Assert.False(result.UsedFallback);
        Assert.Equal("Gemini phục hồi sau retry", result.Text);
        Assert.Equal(2, handler.CallCount);
    }

    private static SalonPromptsOptions Prompts() => new()
    {
        System = "System prompt",
        Recommendation = "Khách: {{customerName}}; Nhu cầu: {{need}}; Dịch vụ: {{services}}; Lịch sử: {{history}}",
        Message = "Khách: {{customerName}}; Loại: {{messageType}}; Lịch: {{appointmentInfo}}",
        Chat = "Khách: {{customerName}}; Câu hỏi: {{message}}; Lịch sử: {{history}}; Dịch vụ: {{services}}; Hội thoại: {{conversation}}",
        Summary = "Khách: {{customerName}}; Lịch sử: {{history}}"
    };

    private sealed class TestHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class RecordingHandler(string response, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class SequenceHandler(params (HttpStatusCode StatusCode, string Body)[] responses) : HttpMessageHandler
    {
        private readonly Queue<(HttpStatusCode StatusCode, string Body)> _responses = new(responses);
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            var next = _responses.Dequeue();
            return Task.FromResult(new HttpResponseMessage(next.StatusCode)
            {
                Content = new StringContent(next.Body, Encoding.UTF8, "application/json")
            });
        }
    }
}
