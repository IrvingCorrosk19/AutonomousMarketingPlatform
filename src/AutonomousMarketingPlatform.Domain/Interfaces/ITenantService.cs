namespace AutonomousMarketingPlatform.Domain.Interfaces;

/// <summary>
/// Servicio para obtener y validar información del tenant actual.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Obtiene el TenantId del tenant actual basado en el contexto de la solicitud.
    /// </summary>
    Guid? GetCurrentTenantId();

    /// <summary>
    /// Valida que el tenant existe y está activo.
    /// </summary>
    Task<bool> ValidateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el tenant por subdominio.
    /// </summary>
    Task<Guid?> GetTenantIdBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);
}

