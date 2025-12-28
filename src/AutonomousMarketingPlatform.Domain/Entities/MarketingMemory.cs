using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa la memoria del sistema sobre conversaciones, decisiones y aprendizajes.
/// Permite que el sistema recuerde contexto y preferencias a lo largo del tiempo.
/// </summary>
public class MarketingMemory : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece la memoria.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador de la campaña asociada (opcional).
    /// </summary>
    public Guid? CampaignId { get; set; }

    /// <summary>
    /// Tipo de memoria (Conversation, Decision, Learning, Feedback).
    /// </summary>
    public string MemoryType { get; set; } = string.Empty;

    /// <summary>
    /// Contenido de la memoria (conversación, decisión tomada, aprendizaje, etc.).
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Contexto adicional en formato JSON.
    /// </summary>
    public string? ContextJson { get; set; }

    /// <summary>
    /// Tags para categorizar y buscar la memoria.
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Relevancia o importancia de la memoria (1-10).
    /// </summary>
    public int RelevanceScore { get; set; } = 5;

    /// <summary>
    /// Fecha en que ocurrió el evento que generó esta memoria.
    /// </summary>
    public DateTime MemoryDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Campaign? Campaign { get; set; }
}

