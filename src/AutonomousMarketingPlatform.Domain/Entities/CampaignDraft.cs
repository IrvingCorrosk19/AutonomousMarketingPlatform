using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa un borrador de campaña generado automáticamente.
/// </summary>
public class CampaignDraft : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del usuario que creó el borrador.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Identificador del MarketingPack que generó este borrador.
    /// </summary>
    public Guid MarketingPackId { get; set; }

    /// <summary>
    /// Nombre del borrador de campaña.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del borrador.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Objetivos de la campaña (JSON).
    /// </summary>
    public string? Objectives { get; set; }

    /// <summary>
    /// Audiencia objetivo (JSON).
    /// </summary>
    public string? TargetAudience { get; set; }

    /// <summary>
    /// Canales sugeridos (JSON array).
    /// </summary>
    public string? SuggestedChannels { get; set; }

    /// <summary>
    /// Estado del borrador (Draft, Review, Approved).
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Indica si el borrador fue convertido en campaña real.
    /// </summary>
    public bool IsConverted { get; set; }

    /// <summary>
    /// Identificador de la campaña real si fue convertido.
    /// </summary>
    public Guid? ConvertedCampaignId { get; set; }

    // Navigation properties
    public virtual MarketingPack MarketingPack { get; set; } = null!;
    public virtual Campaign? ConvertedCampaign { get; set; }
}

