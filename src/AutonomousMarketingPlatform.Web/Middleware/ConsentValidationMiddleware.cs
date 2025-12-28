using AutonomousMarketingPlatform.Application.Services;

namespace AutonomousMarketingPlatform.Web.Middleware;

/// <summary>
/// Middleware para validar consentimientos antes de permitir acceso a funcionalidades que los requieren.
/// </summary>
public class ConsentValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ConsentValidationMiddleware> _logger;

    // Rutas que requieren consentimientos específicos
    private static readonly Dictionary<string, string[]> RequiredConsentsByRoute = new()
    {
        { "/campaigns/create", new[] { "AIGeneration", "DataProcessing" } },
        { "/campaigns/generate", new[] { "AIGeneration" } },
        { "/content/generate", new[] { "AIGeneration" } },
        { "/automations", new[] { "AIGeneration", "DataProcessing", "AutoPublishing" } }
    };

    public ConsentValidationMiddleware(RequestDelegate next, ILogger<ConsentValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IConsentValidationService consentValidationService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Verificar si la ruta requiere consentimientos
        var requiredConsents = RequiredConsentsByRoute
            .FirstOrDefault(kvp => path.Contains(kvp.Key.ToLower()))
            .Value;

        if (requiredConsents != null && requiredConsents.Length > 0)
        {
            // TODO: Obtener UserId y TenantId del usuario autenticado
            // Por ahora, si no hay usuario autenticado, permitir continuar
            // En producción, esto debe validar contra el usuario real

            // Ejemplo de validación (comentado hasta tener autenticación):
            /*
            var userId = GetUserIdFromContext(context);
            var tenantId = GetTenantIdFromContext(context);

            if (userId.HasValue && tenantId.HasValue)
            {
                var missingConsents = await consentValidationService.GetMissingConsentsAsync(
                    userId.Value,
                    tenantId.Value,
                    requiredConsents,
                    context.RequestAborted);

                if (missingConsents.Any())
                {
                    _logger.LogWarning(
                        "Usuario {UserId} intentó acceder a {Path} sin los consentimientos requeridos: {MissingConsents}",
                        userId, path, string.Join(", ", missingConsents));

                    context.Response.Redirect("/Consents?missing=" + string.Join(",", missingConsents));
                    return;
                }
            }
            */
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method para registrar el middleware.
/// </summary>
public static class ConsentValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseConsentValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ConsentValidationMiddleware>();
    }
}

