using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de tenant.
/// Obtiene el tenant actual desde el contexto HTTP (headers, subdominio, etc.).
/// </summary>
public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbContextFactory<ApplicationDbContext>? _contextFactory;
    private readonly ApplicationDbContext? _context;
    private Guid? _cachedTenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _contextFactory = null;
    }

    public TenantService(IHttpContextAccessor httpContextAccessor, IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _contextFactory = contextFactory;
        _context = null;
    }

    public Guid? GetCurrentTenantId()
    {
        if (_cachedTenantId.HasValue)
            return _cachedTenantId;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        // Estrategia 1: Obtener desde header personalizado
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (Guid.TryParse(tenantIdHeader.ToString(), out var tenantId))
            {
                _cachedTenantId = tenantId;
                return tenantId;
            }
        }

        // Estrategia 2: Obtener desde claims del usuario autenticado
        var tenantIdClaim = httpContext.User?.FindFirst("TenantId")?.Value;
        if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out var tenantIdFromClaim))
        {
            _cachedTenantId = tenantIdFromClaim;
            return tenantIdFromClaim;
        }

        // Estrategia 3: Obtener desde subdominio
        var host = httpContext.Request.Host.Host;
        if (!string.IsNullOrEmpty(host))
        {
            var subdomain = host.Split('.').FirstOrDefault();
            if (!string.IsNullOrEmpty(subdomain) && subdomain != "www" && subdomain != "localhost")
            {
                // Buscar tenant por subdominio (se hará de forma asíncrona en el middleware)
                // Por ahora retornamos null, el middleware se encargará de resolverlo
            }
        }

        return null;
    }

    public async Task<bool> ValidateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var context = _context ?? (_contextFactory != null ? await _contextFactory.CreateDbContextAsync(cancellationToken) : null);
        if (context == null) return false;

        try
        {
            return await context.Tenants
                .AnyAsync(t => t.Id == tenantId && t.IsActive, cancellationToken);
        }
        finally
        {
            if (_contextFactory != null && context != _context)
            {
                await context.DisposeAsync();
            }
        }
    }

    public async Task<Guid?> GetTenantIdBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        var context = _context ?? (_contextFactory != null ? await _contextFactory.CreateDbContextAsync(cancellationToken) : null);
        if (context == null) return null;

        try
        {
            var tenant = await context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive, cancellationToken);

            return tenant?.Id;
        }
        finally
        {
            if (_contextFactory != null && context != _context)
            {
                await context.DisposeAsync();
            }
        }
    }
}

