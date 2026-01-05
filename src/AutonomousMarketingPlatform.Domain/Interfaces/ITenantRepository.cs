using AutonomousMarketingPlatform.Domain.Entities;

namespace AutonomousMarketingPlatform.Domain.Interfaces;

/// <summary>
/// Interfaz para repositorio de Tenants.
/// Tenant no implementa ITenantEntity porque es la entidad raíz del sistema multi-tenant.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Obtiene un tenant por su ID.
    /// </summary>
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tenants.
    /// </summary>
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca un tenant por subdomain (solo activos, usado para autenticación).
    /// </summary>
    Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe un tenant con el subdomain especificado (activo o inactivo, usado para validación de unicidad).
    /// </summary>
    Task<Tenant?> GetBySubdomainAnyAsync(string subdomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo tenant.
    /// </summary>
    Task<Tenant> AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un tenant existente.
    /// </summary>
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
}


