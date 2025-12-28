namespace AutonomousMarketingPlatform.Application.Services;

/// <summary>
/// Servicio para validar consentimientos antes de ejecutar operaciones que los requieren.
/// </summary>
public interface IConsentValidationService
{
    /// <summary>
    /// Valida que el usuario tenga el consentimiento requerido otorgado.
    /// </summary>
    Task<bool> ValidateConsentAsync(Guid userId, Guid tenantId, string consentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida múltiples consentimientos requeridos.
    /// </summary>
    Task<bool> ValidateConsentsAsync(Guid userId, Guid tenantId, IEnumerable<string> consentTypes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene los consentimientos faltantes para una operación.
    /// </summary>
    Task<List<string>> GetMissingConsentsAsync(Guid userId, Guid tenantId, IEnumerable<string> requiredConsentTypes, CancellationToken cancellationToken = default);
}

