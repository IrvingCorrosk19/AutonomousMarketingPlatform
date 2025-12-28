namespace AutonomousMarketingPlatform.Domain.Common;

/// <summary>
/// Interfaz que deben implementar todas las entidades que pertenecen a un tenant.
/// Garantiza el aislamiento de datos por empresa.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// Identificador único del tenant (empresa) al que pertenece la entidad.
    /// Este campo es obligatorio y se usa para filtrar todos los datos.
    /// </summary>
    Guid TenantId { get; set; }
}

