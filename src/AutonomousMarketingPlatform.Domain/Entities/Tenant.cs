using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa una empresa (tenant) en el sistema multi-tenant.
/// Cada tenant tiene su propio espacio aislado de datos.
/// </summary>
public class Tenant : BaseEntity
{
    /// <summary>
    /// Nombre de la empresa.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único del tenant (usado como TenantId en otras entidades).
    /// </summary>
    public Guid TenantId => Id;

    /// <summary>
    /// Subdominio único para acceso al sistema (ej: empresa1.plataforma.com).
    /// </summary>
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Email de contacto principal de la empresa.
    /// </summary>
    public string ContactEmail { get; set; } = string.Empty;

    /// <summary>
    /// Plan de suscripción actual.
    /// </summary>
    public string SubscriptionPlan { get; set; } = "Free";

    /// <summary>
    /// Fecha de inicio de la suscripción.
    /// </summary>
    public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de expiración de la suscripción.
    /// </summary>
    public DateTime? SubscriptionEndDate { get; set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}

