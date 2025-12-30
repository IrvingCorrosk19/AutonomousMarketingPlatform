using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Configuración de n8n por tenant.
/// Almacena URLs de webhooks y configuraciones de n8n.
/// </summary>
public class TenantN8nConfig : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Indica si se debe usar modo mock (simulación) o conexión real con n8n.
    /// </summary>
    public bool UseMock { get; set; } = true;

    /// <summary>
    /// URL base de n8n (ej: http://localhost:5678).
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5678";

    /// <summary>
    /// URL de la API de n8n (ej: http://localhost:5678/api/v1).
    /// </summary>
    public string ApiUrl { get; set; } = "http://localhost:5678/api/v1";

    /// <summary>
    /// API Key de n8n encriptada (opcional, para autenticación).
    /// </summary>
    public string? EncryptedApiKey { get; set; }

    /// <summary>
    /// URL por defecto para webhooks.
    /// </summary>
    public string DefaultWebhookUrl { get; set; } = "http://localhost:5678/webhook";

    /// <summary>
    /// URLs de webhooks en formato JSON.
    /// Estructura: {"MarketingRequest": "url1", "ValidateConsents": "url2", ...}
    /// </summary>
    public string WebhookUrlsJson { get; set; } = "{}";

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

