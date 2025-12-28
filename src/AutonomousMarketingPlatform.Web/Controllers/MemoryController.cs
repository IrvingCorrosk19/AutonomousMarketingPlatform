using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.Memory;
using AutonomousMarketingPlatform.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para visualización de memoria de marketing (solo lectura).
/// Accesible para todos los usuarios autenticados.
/// </summary>
[Authorize]
public class MemoryController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<MemoryController> _logger;

    public MemoryController(IMediator mediator, ILogger<MemoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Vista principal de memoria de marketing.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? type, [FromQuery] string? tags)
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            if (!tenantId.HasValue)
            {
                _logger.LogWarning("Usuario autenticado sin TenantId");
                return RedirectToAction("Login", "Account");
            }

            var query = new QueryMemoryQuery
            {
                TenantId = tenantId.Value,
                MemoryTypes = !string.IsNullOrEmpty(type) ? new List<string> { type } : null,
                Tags = !string.IsNullOrEmpty(tags) ? tags.Split(',').ToList() : null,
                Limit = 50
            };

            var memories = await _mediator.Send(query);
            
            ViewBag.MemoryTypes = new[] { "Conversation", "Decision", "Learning", "Feedback", "Pattern", "Preference" };
            ViewBag.SelectedType = type;
            ViewBag.SelectedTags = tags;

            return View(memories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar memorias");
            return View(new List<MarketingMemoryDto>());
        }
    }

    /// <summary>
    /// Vista de memoria de una campaña específica.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Campaign(Guid campaignId)
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            if (!tenantId.HasValue)
            {
                _logger.LogWarning("Usuario autenticado sin TenantId");
                return RedirectToAction("Login", "Account");
            }

            var query = new QueryMemoryQuery
            {
                TenantId = tenantId.Value,
                CampaignId = campaignId,
                Limit = 100
            };

            var memories = await _mediator.Send(query);
            return View(memories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar memorias de campaña {CampaignId}", campaignId);
            return View(new List<MarketingMemoryDto>());
        }
    }

    /// <summary>
    /// Vista de contexto de memoria para IA (solo visualización).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> AIContext([FromQuery] Guid? campaignId)
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            var userId = UserHelper.GetUserId(User);

            if (!tenantId.HasValue || !userId.HasValue)
            {
                _logger.LogWarning("Usuario autenticado sin TenantId o UserId");
                return RedirectToAction("Login", "Account");
            }

            var query = new GetMemoryContextForAIQuery
            {
                TenantId = tenantId.Value,
                UserId = userId.Value,
                CampaignId = campaignId
            };

            var context = await _mediator.Send(query);
            return View(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar contexto de memoria para IA");
            return View(new MemoryContextForAI());
        }
    }
}

