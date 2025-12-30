using AutonomousMarketingPlatform.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa las preferencias del usuario para personalizar el comportamiento del sistema.
/// El sistema aprende y recuerda estas preferencias.
/// </summary>
public class UserPreference : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece la preferencia.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador del usuario propietario de la preferencia.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Clave de la preferencia (ej: "Tone", "Style", "TargetAudience", "BrandColors").
    /// </summary>
    public string PreferenceKey { get; set; } = string.Empty;

    /// <summary>
    /// Valor de la preferencia (puede ser JSON para valores complejos).
    /// </summary>
    public string PreferenceValue { get; set; } = string.Empty;

    /// <summary>
    /// Categoría de la preferencia (Marketing, Design, Publishing, etc.).
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Fecha de la última actualización de esta preferencia.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}

