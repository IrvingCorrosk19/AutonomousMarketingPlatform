using System.Linq.Expressions;
using AutonomousMarketingPlatform.Domain.Common;

namespace AutonomousMarketingPlatform.Domain.Interfaces;

/// <summary>
/// Interfaz genérica para repositorios.
/// Proporciona operaciones CRUD básicas con soporte multi-tenant.
/// </summary>
/// <typeparam name="T">Tipo de entidad que debe heredar de BaseEntity e implementar ITenantEntity</typeparam>
public interface IRepository<T> where T : BaseEntity, ITenantEntity
{
    /// <summary>
    /// Obtiene una entidad por su ID y TenantId.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las entidades de un tenant.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca entidades que cumplan con un predicado dentro de un tenant.
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva entidad.
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una entidad (soft delete).
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una entidad con el ID y TenantId especificados.
    /// </summary>
    Task<bool> ExistsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
}

