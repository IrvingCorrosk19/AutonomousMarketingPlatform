namespace AutonomousMarketingPlatform.Domain.Interfaces;

/// <summary>
/// Interfaz para Unit of Work pattern.
/// Permite guardar cambios de múltiples repositorios en una sola transacción.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Guarda todos los cambios pendientes en la base de datos.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

