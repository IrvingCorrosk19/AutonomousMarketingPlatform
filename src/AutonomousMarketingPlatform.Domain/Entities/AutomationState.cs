using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Entities;

/// <summary>
/// Representa el estado de las automatizaciones del sistema.
/// Permite rastrear y controlar el marketing autónomo 24/7.
/// </summary>
public class AutomationState : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Identificador del tenant al que pertenece el estado de automatización.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador de la campaña asociada (opcional).
    /// </summary>
    public Guid? CampaignId { get; set; }

    /// <summary>
    /// Tipo de automatización (ContentGeneration, Publishing, Analytics, StrategyUpdate).
    /// </summary>
    public string AutomationType { get; set; } = string.Empty;

    /// <summary>
    /// Estado actual de la automatización (Running, Paused, Completed, Error, Scheduled).
    /// </summary>
    public string Status { get; set; } = "Scheduled";

    /// <summary>
    /// Configuración de la automatización en formato JSON.
    /// </summary>
    public string? ConfigurationJson { get; set; }

    /// <summary>
    /// Última ejecución de la automatización.
    /// </summary>
    public DateTime? LastExecutionAt { get; set; }

    /// <summary>
    /// Próxima ejecución programada.
    /// </summary>
    public DateTime? NextExecutionAt { get; set; }

    /// <summary>
    /// Frecuencia de ejecución (Hourly, Daily, Weekly, Custom).
    /// </summary>
    public string? ExecutionFrequency { get; set; }

    /// <summary>
    /// Resultado de la última ejecución (puede ser JSON con detalles).
    /// </summary>
    public string? LastExecutionResult { get; set; }

    /// <summary>
    /// Mensaje de error si la última ejecución falló.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Número de ejecuciones exitosas.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Número de ejecuciones fallidas.
    /// </summary>
    public int FailureCount { get; set; }

    // Navigation properties
    public virtual Campaign? Campaign { get; set; }
}

