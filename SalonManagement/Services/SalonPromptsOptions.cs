namespace SalonManagement.Services;

/// <summary>Prompt templates are configured outside application code for review and iteration.</summary>
public class SalonPromptsOptions
{
    public string System { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public string Chat { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
