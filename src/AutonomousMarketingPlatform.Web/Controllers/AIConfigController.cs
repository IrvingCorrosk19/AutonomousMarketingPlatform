using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.AI;
using AutonomousMarketingPlatform.Web.Attributes;
using AutonomousMarketingPlatform.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para configurar API keys de IA desde el frontend.
/// Requiere rol Owner o Admin.
/// </summary>
[Authorize]
[AuthorizeRole("Owner", "Admin")]
public class AIConfigController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<AIConfigController> _logger;

    public AIConfigController(IMediator mediator, ILogger<AIConfigController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Vista para configurar API key de IA.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = new GetTenantAIConfigQuery
            {
                TenantId = tenantId.Value,
                Provider = "OpenAI"
            };

            var config = await _mediator.Send(query);
            return View(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar configuración de IA");
            return View(new TenantAIConfigDto());
        }
    }

    /// <summary>
    /// Endpoint para guardar/actualizar API key de IA.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([FromForm] CreateTenantAIConfigDto dto)
    {
        try
        {
            var userId = UserHelper.GetUserId(User);
            var tenantId = UserHelper.GetTenantId(User);

            if (!userId.HasValue || !tenantId.HasValue)
            {
                return Unauthorized(new { error = "Usuario no autenticado correctamente" });
            }

            var command = new ConfigureTenantAICommand
            {
                TenantId = tenantId.Value,
                UserId = userId.Value,
                Provider = dto.Provider,
                ApiKey = dto.ApiKey,
                Model = dto.Model,
                IsActive = dto.IsActive
            };

            var result = await _mediator.Send(command);

            TempData["SuccessMessage"] = "API key de IA configurada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de configurar IA");
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar configuración de IA");
            TempData["ErrorMessage"] = "Error al guardar la configuración. Por favor, intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}

