using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.AI;
using AutonomousMarketingPlatform.Web.Attributes;
using AutonomousMarketingPlatform.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para operaciones de IA (generación de marketing packs).
/// Requiere rol Marketer, Admin o Owner.
/// </summary>
[Authorize]
[AuthorizeRole("Marketer", "Admin", "Owner")]
public class AIController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<AIController> _logger;

    public AIController(IMediator mediator, ILogger<AIController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Genera un MarketingPack completo a partir de contenido cargado.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateMarketingPack([FromBody] GenerateMarketingPackRequest request)
    {
        try
        {
            var userId = UserHelper.GetUserId(User);
            var tenantId = UserHelper.GetTenantId(User);

            if (!userId.HasValue || !tenantId.HasValue)
            {
                return Unauthorized(new { error = "Usuario no autenticado correctamente" });
            }

            var command = new GenerateMarketingPackFromContentCommand
            {
                TenantId = tenantId.Value,
                UserId = userId.Value,
                ContentId = request.ContentId,
                CampaignId = request.CampaignId
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("MarketingPack generado: {MarketingPackId} para contenido {ContentId}", 
                result.Id, request.ContentId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de generar MarketingPack");
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al generar MarketingPack: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado: {Message}", ex.Message);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar MarketingPack para contenido {ContentId}", request.ContentId);
            return StatusCode(500, new { error = "Error interno al generar el marketing pack. Por favor, intente más tarde." });
        }
    }

    /// <summary>
    /// Vista para ver un MarketingPack generado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ViewPack(Guid id)
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            if (!tenantId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = new GetMarketingPackQuery
            {
                TenantId = tenantId.Value,
                MarketingPackId = id
            };

            var marketingPack = await _mediator.Send(query);

            if (marketingPack == null)
            {
                _logger.LogWarning("MarketingPack {Id} no encontrado para Tenant {TenantId}", id, tenantId.Value);
                TempData["ErrorMessage"] = "MarketingPack no encontrado o no tienes permisos para verlo.";
                return RedirectToAction("Index", "Content");
            }

            ViewBag.MarketingPack = marketingPack;
            return View(marketingPack);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar MarketingPack {Id}", id);
            return View("Error");
        }
    }
}

/// <summary>
/// Request DTO para generar MarketingPack.
/// </summary>
public class GenerateMarketingPackRequest
{
    public Guid ContentId { get; set; }
    public Guid? CampaignId { get; set; }
}

