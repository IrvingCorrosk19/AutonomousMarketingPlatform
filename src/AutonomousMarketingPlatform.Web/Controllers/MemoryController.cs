using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.Memory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para visualización de memoria de marketing (solo lectura).
/// </summary>
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
            // TODO: Obtener TenantId del usuario autenticado
            var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var query = new QueryMemoryQuery
            {
                TenantId = tenantId,
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
            // TODO: Obtener TenantId del usuario autenticado
            var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var query = new QueryMemoryQuery
            {
                TenantId = tenantId,
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
            // TODO: Obtener TenantId y UserId del usuario autenticado
            var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var query = new GetMemoryContextForAIQuery
            {
                TenantId = tenantId,
                UserId = userId,
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

