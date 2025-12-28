using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.Consents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para gestión de consentimientos.
/// </summary>
public class ConsentsController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConsentsController> _logger;

    public ConsentsController(IMediator mediator, ILogger<ConsentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los consentimientos del usuario actual.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // TODO: Obtener UserId y TenantId del usuario autenticado
        // Por ahora usamos valores de prueba
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        try
        {
            var query = new GetUserConsentsQuery
            {
                UserId = userId,
                TenantId = tenantId
            };

            var result = await _mediator.Send(query);
            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener consentimientos del usuario {UserId}", userId);
            return View("Error");
        }
    }

    /// <summary>
    /// Otorga un consentimiento.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Grant([FromForm] CreateConsentDto dto)
    {
        // TODO: Obtener UserId y TenantId del usuario autenticado
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        try
        {
            var command = new GrantConsentCommand
            {
                UserId = userId,
                TenantId = tenantId,
                ConsentType = dto.ConsentType,
                ConsentVersion = dto.ConsentVersion ?? "1.0",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            var result = await _mediator.Send(command);
            TempData["SuccessMessage"] = $"Consentimiento '{dto.ConsentType}' otorgado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al otorgar consentimiento {ConsentType} para usuario {UserId}", dto.ConsentType, userId);
            TempData["ErrorMessage"] = "Error al otorgar el consentimiento. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Revoca un consentimiento.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revoke([FromForm] string consentType)
    {
        // TODO: Obtener UserId y TenantId del usuario autenticado
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        try
        {
            var command = new RevokeConsentCommand
            {
                UserId = userId,
                TenantId = tenantId,
                ConsentType = consentType
            };

            var result = await _mediator.Send(command);
            TempData["SuccessMessage"] = $"Consentimiento '{consentType}' revocado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Intento de revocar consentimiento requerido {ConsentType}", consentType);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revocar consentimiento {ConsentType} para usuario {UserId}", consentType, userId);
            TempData["ErrorMessage"] = "Error al revocar el consentimiento. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}

