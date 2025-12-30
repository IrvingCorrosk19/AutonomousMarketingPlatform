using System.Linq.Expressions;
using AutonomousMarketingPlatform.Domain.Common;
using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutonomousMarketingPlatform.Infrastructure.Repositories;

/// <summary>
/// Implementación base del repositorio genérico con soporte multi-tenant.
/// Todas las consultas se filtran automáticamente por TenantId.
/// </summary>
public class BaseRepository<T> : IRepository<T> where T : BaseEntity, ITenantEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ITenantService _tenantService;

    public BaseRepository(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _tenantService = tenantService;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.TenantId == tenantId && e.IsActive)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid tenantId, CancellationToken cancellationToken = default)
    {
        // Si tenantId es Guid.Empty, no filtrar por tenant (para SuperAdmins)
        if (tenantId == Guid.Empty)
        {
            // Aplicar solo el predicado y filtro de IsActive
            return await _dbSet
                .Where(e => e.IsActive)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
        
        // Filtrar por tenant y aplicar el predicado adicional
        return await _dbSet
            .Where(e => e.TenantId == tenantId && e.IsActive)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Asegurar que el TenantId esté asignado
        var tenantId = _tenantService.GetCurrentTenantId();
        if (tenantId.HasValue && entity.TenantId == Guid.Empty)
        {
            entity.TenantId = tenantId.Value;
        }

        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Soft delete
        entity.IsActive = false;
        await UpdateAsync(entity, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(e => e.Id == id && e.TenantId == tenantId && e.IsActive, cancellationToken);
    }
}

