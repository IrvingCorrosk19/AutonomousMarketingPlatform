using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa un paquete completo de marketing generado por IA a partir de contenido.
/// </summary>
public class MarketingPack : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del usuario que solicitó la generación.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Identificador del contenido que originó este pack.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Identificador de la campaña asociada (opcional).
    /// </summary>
    public Guid? CampaignId { get; set; }

    /// <summary>
    /// Estrategia de campaña generada (texto estructurado).
    /// </summary>
    public string Strategy { get; set; } = string.Empty;

    /// <summary>
    /// Estado del pack (Draft, Generated, Approved, Published).
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Versión del pack (para versionado).
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Metadatos adicionales en JSON.
    /// </summary>
    public string? Metadata { get; set; }

    // Navigation properties
    public virtual Content Content { get; set; } = null!;
    public virtual Campaign? Campaign { get; set; }
    public virtual ICollection<GeneratedCopy> Copies { get; set; } = new List<GeneratedCopy>();
    public virtual ICollection<MarketingAssetPrompt> AssetPrompts { get; set; } = new List<MarketingAssetPrompt>();
}

