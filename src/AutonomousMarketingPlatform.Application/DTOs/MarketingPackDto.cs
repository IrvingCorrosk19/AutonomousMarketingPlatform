namespace AutonomousMarketingPlatform.Application.DTOs;

/// <summary>
/// DTO para MarketingPack generado.
/// </summary>
public class MarketingPackDto
{
    public Guid Id { get; set; }
    public Guid ContentId { get; set; }
    public Guid? CampaignId { get; set; }
    public string Strategy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GeneratedCopyDto> Copies { get; set; } = new();
    public List<MarketingAssetPromptDto> AssetPrompts { get; set; } = new();
    public CampaignDraftDto? CampaignDraft { get; set; }
}

/// <summary>
/// DTO para GeneratedCopy.
/// </summary>
public class GeneratedCopyDto
{
    public Guid Id { get; set; }
    public string CopyType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Hashtags { get; set; }
    public string? SuggestedChannel { get; set; }
    public Dictionary<string, object>? PublicationChecklist { get; set; }
}

/// <summary>
/// DTO para MarketingAssetPrompt.
/// </summary>
public class MarketingAssetPromptDto
{
    public Guid Id { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string? NegativePrompt { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
    public string? SuggestedChannel { get; set; }
}

/// <summary>
/// DTO para CampaignDraft.
/// </summary>
public class CampaignDraftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object>? Objectives { get; set; }
    public Dictionary<string, object>? TargetAudience { get; set; }
    public List<string>? SuggestedChannels { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsConverted { get; set; }
    public Guid? ConvertedCampaignId { get; set; }
}

