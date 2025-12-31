using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Web.Attributes;

/// <summary>
/// Atributo para autorizar por rol específico.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public AuthorizeRoleAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthorizeRoleAttribute>>();

        logger?.LogInformation("=== AuthorizeRoleAttribute.OnAuthorization ===");
        logger?.LogInformation("Path: {Path}", context.HttpContext.Request.Path);
        logger?.LogInformation("User authenticated: {IsAuthenticated}", user?.Identity?.IsAuthenticated ?? false);
        logger?.LogInformation("Allowed roles: {Roles}", string.Join(", ", _allowedRoles));

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            logger?.LogWarning("Usuario no autenticado, redirigiendo a Login");
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
            return;
        }

        // Verificar si el usuario tiene el claim IsSuperAdmin
        var isSuperAdmin = user.HasClaim("IsSuperAdmin", "true");
        
        logger?.LogInformation("IsSuperAdmin: {IsSuperAdmin}", isSuperAdmin);
        var userRoles = user.Claims?.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Select(c => c.Value)?.ToList() ?? new List<string>();
        logger?.LogInformation("User roles: {Roles}", string.Join(", ", userRoles));
        
        // Si es SuperAdmin, permitir acceso (SuperAdmin tiene acceso a todo)
        if (isSuperAdmin)
        {
            logger?.LogInformation("SuperAdmin detectado, permitiendo acceso");
            return;
        }

        // Verificar si el usuario tiene alguno de los roles permitidos
        var hasRole = _allowedRoles.Any(role => user.IsInRole(role));

        logger?.LogInformation("Has allowed role: {HasRole}", hasRole);

        if (!hasRole)
        {
            logger?.LogWarning("Usuario no tiene rol permitido, redirigiendo a AccessDenied");
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}


