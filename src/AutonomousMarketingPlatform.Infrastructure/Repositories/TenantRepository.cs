using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de Tenants.
/// Tenant no implementa ITenantEntity porque es la entidad raíz del sistema multi-tenant.
/// </summary>
public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TenantRepository> _logger;

    public TenantRepository(ApplicationDbContext context, ILogger<TenantRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // No filtrar por IsActive para permitir editar tenants inactivos (solo para administradores)
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Mostrar todos los tenants (activos e inactivos) para administradores
        // El controlador está protegido con [AuthorizeRole("Owner", "SuperAdmin")]
        return await _context.Tenants
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        // Solo tenants activos (usado para autenticación)
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive, cancellationToken);
    }

    public async Task<Tenant?> GetBySubdomainAnyAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[TenantRepository.GetBySubdomainAnyAsync] Buscando tenant con subdomain: {Subdomain}", subdomain);
        // Cualquier tenant (activo o inactivo) - usado para validación de unicidad
        var result = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain, cancellationToken);
        _logger.LogInformation("[TenantRepository.GetBySubdomainAnyAsync] Resultado: {Found} (Id: {Id})", 
            result != null ? "ENCONTRADO" : "NO ENCONTRADO", result?.Id);
        return result;
    }

    public async Task<Tenant> AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("=== [TenantRepository.AddAsync] INICIADO ===");
        _logger.LogInformation("[TenantRepository.AddAsync] Tenant - Id: {Id}, Name: {Name}, Subdomain: {Subdomain}, ContactEmail: {ContactEmail}",
            tenant.Id, tenant.Name, tenant.Subdomain, tenant.ContactEmail);
        
        _logger.LogInformation("[TenantRepository.AddAsync] Agregando tenant al contexto...");
        _context.Tenants.Add(tenant);
        
        _logger.LogInformation("[TenantRepository.AddAsync] Guardando cambios en la base de datos...");
        try
        {
            var rowsAffected = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[TenantRepository.AddAsync] Cambios guardados - RowsAffected: {RowsAffected}", rowsAffected);
            _logger.LogInformation("[TenantRepository.AddAsync] Tenant guardado exitosamente - Id: {Id}", tenant.Id);
            _logger.LogInformation("=== [TenantRepository.AddAsync] COMPLETADO ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TenantRepository.AddAsync] ERROR al guardar tenant");
            _logger.LogError("[TenantRepository.AddAsync] Exception Type: {Type}", ex.GetType().Name);
            _logger.LogError("[TenantRepository.AddAsync] Exception Message: {Message}", ex.Message);
            _logger.LogError("[TenantRepository.AddAsync] Exception StackTrace: {StackTrace}", ex.StackTrace);
            throw;
        }
        
        return tenant;
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }
}


