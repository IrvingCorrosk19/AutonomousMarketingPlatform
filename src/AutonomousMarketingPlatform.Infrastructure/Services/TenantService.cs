using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de tenant.
/// Obtiene el tenant actual desde el contexto HTTP (headers, subdominio, etc.).
/// </summary>
public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private Guid? _cachedTenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor, IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _contextFactory = contextFactory;
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
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Tenants
            .AnyAsync(t => t.Id == tenantId && t.IsActive, cancellationToken);
    }

    public async Task<Guid?> GetTenantIdBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var tenant = await context.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive, cancellationToken);

        return tenant?.Id;
    }
}

