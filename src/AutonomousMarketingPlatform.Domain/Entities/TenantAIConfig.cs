using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Configuración de IA por tenant.
/// Almacena API keys y configuraciones de proveedores de IA de forma encriptada.
/// </summary>
public class TenantAIConfig : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Proveedor de IA (OpenAI, Anthropic, etc.).
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// API Key encriptada.
    /// </summary>
    public string EncryptedApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Modelo a usar (gpt-4, gpt-3.5-turbo, etc.).
    /// </summary>
    public string Model { get; set; } = "gpt-4";

    // Nota: IsActive se hereda de BaseEntity

    /// <summary>
    /// Configuraciones adicionales en JSON.
    /// </summary>
    public string? AdditionalConfig { get; set; }

    /// <summary>
    /// Última vez que se usó esta configuración.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Número de veces que se ha usado.
    /// </summary>
    public int UsageCount { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
}

