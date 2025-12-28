using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa una campaña de marketing generada por el sistema.
/// </summary>
public class Campaign : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece la campaña.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Nombre de la campaña.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la campaña.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Estado de la campaña (Draft, Active, Paused, Completed, Archived).
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Estrategia de marketing generada por IA.
    /// </summary>
    public string? MarketingStrategy { get; set; }

    /// <summary>
    /// Fecha de inicio programada de la campaña.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Fecha de finalización programada de la campaña.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Presupuesto asignado a la campaña.
    /// </summary>
    public decimal? Budget { get; set; }

    /// <summary>
    /// Presupuesto gastado hasta el momento.
    /// </summary>
    public decimal? SpentAmount { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
    public virtual ICollection<MarketingMemory> MarketingMemories { get; set; } = new List<MarketingMemory>();
}

