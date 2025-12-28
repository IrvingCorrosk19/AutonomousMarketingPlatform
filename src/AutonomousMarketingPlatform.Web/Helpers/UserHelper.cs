using System.Security.Claims;

namespace AutonomousMarketingPlatform.Web.Helpers;

/// <summary>
/// Helper para obtener información del usuario autenticado.
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// Obtiene el UserId del usuario autenticado.
    /// </summary>
    public static Guid? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }

    /// <summary>
    /// Obtiene el TenantId del usuario autenticado.
    /// </summary>
    public static Guid? GetTenantId(ClaimsPrincipal user)
    {
        var tenantIdClaim = user.FindFirst("TenantId")?.Value;
        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            return tenantId;
        }
        return null;
    }

    /// <summary>
    /// Obtiene el nombre completo del usuario autenticado.
    /// </summary>
    public static string? GetFullName(ClaimsPrincipal user)
    {
        return user.FindFirst("FullName")?.Value ?? user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Obtiene el email del usuario autenticado.
    /// </summary>
    public static string? GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Verifica si el usuario tiene un rol específico.
    /// </summary>
    public static bool HasRole(ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    /// <summary>
    /// Obtiene todos los roles del usuario.
    /// </summary>
    public static IEnumerable<string> GetRoles(ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
}

