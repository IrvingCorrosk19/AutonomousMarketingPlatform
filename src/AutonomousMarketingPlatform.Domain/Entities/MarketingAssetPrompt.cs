using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa un prompt generado para crear assets (imágenes o videos).
/// </summary>
public class MarketingAssetPrompt : BaseEntity, ITenantEntity
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
    /// Tipo de asset (Image, Video, Reel).
    /// </summary>
    public string AssetType { get; set; } = string.Empty;

    /// <summary>
    /// Prompt para el generador de imágenes/videos.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Prompt negativo (lo que NO debe aparecer).
    /// </summary>
    public string? NegativePrompt { get; set; }

    /// <summary>
    /// Parámetros adicionales en JSON (estilo, resolución, etc.).
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// Canal sugerido para este asset.
    /// </summary>
    public string? SuggestedChannel { get; set; }

    // Navigation properties
    public virtual MarketingPack MarketingPack { get; set; } = null!;
}

