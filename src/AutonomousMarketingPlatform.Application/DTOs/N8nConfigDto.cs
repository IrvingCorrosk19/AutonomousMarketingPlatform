namespace AutonomousMarketingPlatform.Application.DTOs;

/// <summary>
/// DTO para la configuración de n8n.
/// </summary>
public class N8nConfigDto
{
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
    /// API Key de n8n (opcional, para autenticación).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// URL por defecto para webhooks.
    /// </summary>
    public string DefaultWebhookUrl { get; set; } = "http://localhost:5678/webhook";

    /// <summary>
    /// URLs de webhooks para cada tipo de workflow.
    /// </summary>
    public Dictionary<string, string> WebhookUrls { get; set; } = new();
}

/// <summary>
/// DTO para actualizar la configuración de n8n.
/// </summary>
public class UpdateN8nConfigDto
{
    public bool UseMock { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string DefaultWebhookUrl { get; set; } = string.Empty;
    public Dictionary<string, string> WebhookUrls { get; set; } = new();
}

/// <summary>
/// DTO para probar la conexión con n8n.
/// </summary>
public class TestN8nConnectionDto
{
    public string BaseUrl { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
}

/// <summary>
/// Respuesta de prueba de conexión con n8n.
/// </summary>
public class TestN8nConnectionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public int? StatusCode { get; set; }
}

/// <summary>
/// Información de un workflow de n8n.
/// </summary>
public class N8nWorkflowInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? WebhookUrl { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = "Unknown"; // Active, Inactive, Error
}

