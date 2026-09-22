namespace SalonManagement.Services;

/// <summary>Prompt templates are configured outside application code for review and iteration.</summary>
public class SalonPromptsOptions
{
    public string System { get; set; } = string.Empty;
    public RecommendationPromptTemplate RecommendationV1 { get; set; } = new();
    public RecommendationPromptTemplate RecommendationV2 { get; set; } = new();
    public string Recommendation { get; set; } = string.Empty;
    public string Chat { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class RecommendationPromptTemplate
{
    public string System { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
}
