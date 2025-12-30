using AutonomousMarketingPlatform.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa un consentimiento explícito del usuario para el uso de IA.
/// Requerido antes de que el sistema pueda generar contenido automáticamente.
/// </summary>
public class Consent : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece el consentimiento.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del usuario que otorga el consentimiento.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Tipo de consentimiento (AIGeneration, DataProcessing, AutoPublishing, etc.).
    /// </summary>
    public string ConsentType { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el consentimiento está otorgado.
    /// </summary>
    public bool IsGranted { get; set; }

    /// <summary>
    /// Fecha y hora en que se otorgó el consentimiento.
    /// </summary>
    public DateTime? GrantedAt { get; set; }

    /// <summary>
    /// Fecha y hora en que se revocó el consentimiento (si aplica).
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Versión del documento de consentimiento aceptado.
    /// </summary>
    public string? ConsentVersion { get; set; }

    /// <summary>
    /// IP desde la cual se otorgó el consentimiento.
    /// </summary>
    public string? IpAddress { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

