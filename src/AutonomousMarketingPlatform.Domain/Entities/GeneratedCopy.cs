using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa un copy generado por IA (corto, medio o largo).
/// </summary>
public class GeneratedCopy : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del MarketingPack al que pertenece.
    /// </summary>
    public Guid MarketingPackId { get; set; }

    /// <summary>
    /// Tipo de copy (Short, Medium, Long).
    /// </summary>
    public string CopyType { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del copy.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Hashtags sugeridos para este copy.
    /// </summary>
    public string? Hashtags { get; set; }

    /// <summary>
    /// Canal sugerido (Instagram, Facebook, TikTok, etc.).
    /// </summary>
    public string? SuggestedChannel { get; set; }

    /// <summary>
    /// Checklist de publicación para este canal (JSON).
    /// </summary>
    public string? PublicationChecklist { get; set; }

    // Navigation properties
    public virtual MarketingPack MarketingPack { get; set; } = null!;
}

