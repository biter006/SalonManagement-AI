namespace SalonManagement.Services;

public class AIOptions
{
    public string Provider { get; set; } = "LocalFallback";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.openai.com/v1/responses";
    public string GeminiEndpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta";
}
