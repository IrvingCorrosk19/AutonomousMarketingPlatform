using AutonomousMarketingPlatform.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AutonomousMarketingPlatform.Web.Middleware;

/// <summary>
/// Middleware para validar tenant en cada request.
/// </summary>
public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantValidationMiddleware> _logger;
    private readonly bool _validationEnabled;

    public TenantValidationMiddleware(
        RequestDelegate next,
        ILogger<TenantValidationMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _validationEnabled = configuration.GetValue<bool>("MultiTenant:ValidationEnabled", true);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip validation para health checks, endpoints públicos, login y archivos estáticos
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var originalPath = context.Request.Path.Value ?? "";
        
        // Log para debugging
        _logger.LogDebug("TenantValidationMiddleware: Path={Path}, Method={Method}", originalPath, context.Request.Method);
        
        // Excluir archivos estáticos (CSS, JS, imágenes, fuentes, etc.)
        var isStaticFile = path.StartsWith("/css/") ||
                          path.StartsWith("/js/") ||
                          path.StartsWith("/images/") ||
                          path.StartsWith("/img/") ||
                          path.StartsWith("/fonts/") ||
                          path.StartsWith("/lib/") ||
                          path.StartsWith("/favicon.ico") ||
                          path.EndsWith(".css") ||
                          path.EndsWith(".js") ||
                          path.EndsWith(".png") ||
                          path.EndsWith(".jpg") ||
                          path.EndsWith(".jpeg") ||
                          path.EndsWith(".gif") ||
                          path.EndsWith(".svg") ||
                          path.EndsWith(".ico") ||
                          path.EndsWith(".woff") ||
                          path.EndsWith(".woff2") ||
                          path.EndsWith(".ttf") ||
                          path.EndsWith(".eot");
        
        // Excluir rutas públicas (case-insensitive)
        var isPublicRoute = path == "/" ||
                           path.StartsWith("/health") || 
                           path.StartsWith("/api/public") || 
                           path.StartsWith("/account/login") ||
                           path.StartsWith("/account/accessdenied") ||
                           path.StartsWith("/account/register") ||
                           path.StartsWith("/account/logout");
        
        if (isStaticFile || isPublicRoute)
        {
            _logger.LogDebug("TenantValidationMiddleware: Skipping validation for public route: {Path}", originalPath);
            await _next(context);
            return;
        }

        if (_validationEnabled)
        {
            // Obtener TenantId del request (ya resuelto por el middleware anterior o desde Items)
            var tenantId = context.Items["TenantId"] as Guid?;

            // Si no está en Items, intentar obtenerlo
            if (!tenantId.HasValue)
            {
                tenantId = GetTenantIdFromRequest(context);
            }

            // Si aún no hay tenant y el usuario está autenticado, obtenerlo del claim
            if (!tenantId.HasValue && context.User?.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = context.User.FindFirst("TenantId");
                if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var claimTenantId))
                {
                    tenantId = claimTenantId;
                }
            }

            if (tenantId == null || tenantId == Guid.Empty)
            {
                // Si no está autenticado, redirigir a login (pero solo si no es una ruta pública)
                if (context.User?.Identity?.IsAuthenticated != true)
                {
                    _logger.LogWarning("TenantValidationMiddleware: No tenant y usuario no autenticado, redirigiendo a login. Path={Path}", originalPath);
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                // Si está autenticado, verificar si es super admin
                var isSuperAdmin = context.User.HasClaim("IsSuperAdmin", "true");
                if (isSuperAdmin)
                {
                    // Super admin puede acceder sin tenant
                    _logger.LogInformation("SuperAdmin accediendo sin TenantId: Path={Path}", context.Request.Path);
                    await _next(context);
                    return;
                }

                _logger.LogWarning("Request sin TenantId y no es super admin: Path={Path}, IP={IP}",
                    context.Request.Path, context.Connection.RemoteIpAddress);

                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = new
                    {
                        code = "MISSING_TENANT",
                        message = "TenantId es requerido"
                    }
                });
                return;
            }

            // Resolver ISecurityService desde el scope del request (no desde constructor)
            var securityService = context.RequestServices.GetRequiredService<ISecurityService>();
            
            // Validar que el tenant existe y está activo
            var isValid = await securityService.ValidateTenantAsync(tenantId.Value);
            if (!isValid)
            {
                _logger.LogWarning("Intento de acceso con tenant inválido: TenantId={TenantId}, Path={Path}, IP={IP}",
                    tenantId, context.Request.Path, context.Connection.RemoteIpAddress);

                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = new
                    {
                        code = "INVALID_TENANT",
                        message = "Tenant no válido o inactivo"
                    }
                });
                return;
            }

            // Agregar TenantId al contexto para uso posterior
            context.Items["TenantId"] = tenantId.Value;
        }

        await _next(context);
    }

    private Guid? GetTenantIdFromRequest(HttpContext context)
    {
        // 1. Intentar desde header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue))
        {
            if (Guid.TryParse(headerValue.ToString(), out var tenantId))
                return tenantId;
        }

        // 2. Intentar desde claims (si está autenticado)
        var tenantClaim = context.User?.FindFirst("TenantId");
        if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var claimTenantId))
            return claimTenantId;

        // 3. Intentar desde subdomain
        var host = context.Request.Host.Host;
        // TODO: Implementar resolución por subdomain si es necesario

        return null;
    }
}

