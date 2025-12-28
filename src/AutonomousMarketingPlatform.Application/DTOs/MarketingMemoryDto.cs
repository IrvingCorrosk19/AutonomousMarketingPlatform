namespace AutonomousMarketingPlatform.Application.DTOs;

/// <summary>
/// DTO para memoria de marketing.
/// </summary>
public class MarketingMemoryDto
{
    public Guid Id { get; set; }
    public Guid? CampaignId { get; set; }
    public string? CampaignName { get; set; }
    public string MemoryType { get; set; } = string.Empty;
    public string MemoryTypeDisplayName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object>? Context { get; set; }
    public List<string> Tags { get; set; } = new();
    public int RelevanceScore { get; set; }
    public DateTime MemoryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para crear nueva memoria.
/// </summary>
public class CreateMarketingMemoryDto
{
    public Guid? CampaignId { get; set; }
    public string MemoryType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object>? Context { get; set; }
    public List<string>? Tags { get; set; }
    public int RelevanceScore { get; set; } = 5;
}

/// <summary>
/// DTO para consultar memoria relevante.
/// </summary>
public class MemoryQueryDto
{
    public Guid? UserId { get; set; }
    public Guid? CampaignId { get; set; }
    public List<string>? MemoryTypes { get; set; }
    public List<string>? Tags { get; set; }
    public int? MinRelevanceScore { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Limit { get; set; } = 10;
}

/// <summary>
/// DTO para contexto de memoria para IA.
/// </summary>
public class MemoryContextForAI
{
    public List<MarketingMemoryDto> UserPreferences { get; set; } = new();
    public List<MarketingMemoryDto> RecentConversations { get; set; } = new();
    public List<MarketingMemoryDto> CampaignMemories { get; set; } = new();
    public List<MarketingMemoryDto> Learnings { get; set; } = new();
    public string SummarizedContext { get; set; } = string.Empty;
}

