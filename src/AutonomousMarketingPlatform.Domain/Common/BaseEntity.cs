namespace AutonomousMarketingPlatform.Domain.Common;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Proporciona propiedades comunes como Id y auditoría.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Fecha y hora de creación del registro.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de la última actualización del registro.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica si el registro está activo (soft delete).
    /// </summary>
    public bool IsActive { get; set; } = true;
}

