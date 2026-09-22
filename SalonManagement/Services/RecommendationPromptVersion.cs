using System.ComponentModel.DataAnnotations;

namespace SalonManagement.Services;

/// <summary>
/// Prompt versions used only to run a controlled KT3 comparison. V3 is the
/// production default and should be selected for normal salon consultation.
/// </summary>
public enum RecommendationPromptVersion
{
    [Display(Name = "V1 - Nhu cầu cơ bản")]
    V1 = 1,

    [Display(Name = "V2 - Bổ sung dữ liệu salon")]
    V2 = 2,

    [Display(Name = "V3 - Prompt chính thức")]
    V3 = 3
}
