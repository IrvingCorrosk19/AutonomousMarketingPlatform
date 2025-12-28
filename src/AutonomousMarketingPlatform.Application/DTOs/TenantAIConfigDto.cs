namespace AutonomousMarketingPlatform.Application.DTOs;

/// <summary>
/// DTO para configuración de IA del tenant.
/// </summary>
public class TenantAIConfigDto
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public int UsageCount { get; set; }
    // NO incluir la API key en el DTO por seguridad
}

/// <summary>
/// DTO para crear/actualizar configuración de IA.
/// </summary>
public class CreateTenantAIConfigDto
{
    public string Provider { get; set; } = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4";
    public bool IsActive { get; set; } = true;
}

