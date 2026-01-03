using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
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

    /// <summary>
    /// Endpoint API para obtener contexto de memoria (compatibilidad con n8n).
    /// Este endpoint existe para mantener compatibilidad con workflows que usan /api/Memory/context.
    /// </summary>
    [HttpGet("api/Memory/context")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMemoryContextApi(
        [FromQuery] Guid tenantId,
        [FromQuery] Guid? userId = null,
        [FromQuery] Guid? campaignId = null,
        [FromQuery] string? memoryType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar que tenantId sea válido
            if (tenantId == Guid.Empty)
            {
                _logger.LogWarning("GetMemoryContextApi llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            // Usar el mismo servicio que MemoryApiController
            var memoryService = HttpContext.RequestServices.GetRequiredService<Application.Services.IMarketingMemoryService>();

            // Obtener contexto de memoria
            var memoryContext = await memoryService.GetMemoryContextForAIAsync(
                tenantId,
                userId,
                campaignId,
                null,
                cancellationToken);

            // Si se especifica memoryType, filtrar las memorias
            if (!string.IsNullOrWhiteSpace(memoryType))
            {
                var filteredUserPreferences = memoryContext.UserPreferences
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var filteredCampaignMemories = memoryContext.CampaignMemories
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var filteredLearnings = memoryContext.Learnings
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var filteredRecentConversations = memoryContext.RecentConversations
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var filteredContext = new Application.DTOs.MemoryContextForAI
                {
                    UserPreferences = filteredUserPreferences,
                    RecentConversations = filteredRecentConversations,
                    CampaignMemories = filteredCampaignMemories,
                    Learnings = filteredLearnings,
                    SummarizedContext = memoryContext.SummarizedContext
                };

                // Extraer preferencias, aprendizajes y restricciones
                var preferences = ExtractPreferences(filteredContext);
                var learnings = ExtractLearnings(filteredContext);
                var restrictions = ExtractRestrictions(filteredContext);

                var response = new
                {
                    Preferences = preferences,
                    Learnings = learnings,
                    Restrictions = restrictions,
                    UserPreferences = filteredUserPreferences,
                    RecentConversations = filteredContext.RecentConversations,
                    CampaignMemories = filteredCampaignMemories,
                    LearningsList = filteredLearnings,
                    SummarizedContext = filteredContext.SummarizedContext
                };

                return Ok(response);
            }

            // Si no se especifica memoryType, retornar todo el contexto
            var allPreferences = ExtractPreferences(memoryContext);
            var allLearnings = ExtractLearnings(memoryContext);
            var allRestrictions = ExtractRestrictions(memoryContext);

            var fullResponse = new
            {
                Preferences = allPreferences,
                Learnings = allLearnings,
                Restrictions = allRestrictions,
                UserPreferences = memoryContext.UserPreferences,
                RecentConversations = memoryContext.RecentConversations,
                CampaignMemories = memoryContext.CampaignMemories,
                LearningsList = memoryContext.Learnings,
                SummarizedContext = memoryContext.SummarizedContext
            };

            return Ok(fullResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al obtener contexto de memoria para Tenant {TenantId}",
                tenantId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to load marketing memory from backend"
                });
        }
    }

    /// <summary>
    /// Extrae preferencias del usuario de la memoria.
    /// </summary>
    private Dictionary<string, object> ExtractPreferences(Application.DTOs.MemoryContextForAI memoryContext)
    {
        var preferences = new Dictionary<string, object>();

        foreach (var memory in memoryContext.UserPreferences)
        {
            if (memory.Context != null)
            {
                foreach (var kvp in memory.Context)
                {
                    if (kvp.Key.Contains("tone", StringComparison.OrdinalIgnoreCase) ||
                        kvp.Key.Contains("style", StringComparison.OrdinalIgnoreCase) ||
                        kvp.Key.Contains("format", StringComparison.OrdinalIgnoreCase))
                    {
                        preferences[kvp.Key] = kvp.Value;
                    }
                }
            }

            if (memory.Content.Contains("tono", StringComparison.OrdinalIgnoreCase) ||
                memory.Content.Contains("tone", StringComparison.OrdinalIgnoreCase))
            {
                if (memory.Content.Contains("profesional", StringComparison.OrdinalIgnoreCase))
                    preferences["preferredTone"] = "profesional";
                else if (memory.Content.Contains("casual", StringComparison.OrdinalIgnoreCase))
                    preferences["preferredTone"] = "casual";
                else if (memory.Content.Contains("formal", StringComparison.OrdinalIgnoreCase))
                    preferences["preferredTone"] = "formal";
            }

            if (memory.Content.Contains("texto largo", StringComparison.OrdinalIgnoreCase) ||
                memory.Content.Contains("long text", StringComparison.OrdinalIgnoreCase))
            {
                if (!preferences.ContainsKey("dislikedFormats"))
                    preferences["dislikedFormats"] = new List<string>();
                
                var dislikedFormats = (List<string>)preferences["dislikedFormats"];
                if (!dislikedFormats.Contains("texto largo"))
                    dislikedFormats.Add("texto largo");
            }
        }

        if (!preferences.ContainsKey("preferredTone"))
            preferences["preferredTone"] = "profesional";
        
        if (!preferences.ContainsKey("dislikedFormats"))
            preferences["dislikedFormats"] = new List<string>();

        return preferences;
    }

    /// <summary>
    /// Extrae aprendizajes del sistema de la memoria.
    /// </summary>
    private Dictionary<string, object> ExtractLearnings(Application.DTOs.MemoryContextForAI memoryContext)
    {
        var learnings = new Dictionary<string, object>();
        var bestChannels = new List<string>();

        foreach (var memory in memoryContext.Learnings)
        {
            if (memory.Content.Contains("instagram", StringComparison.OrdinalIgnoreCase) ||
                memory.Tags.Contains("instagram", StringComparer.OrdinalIgnoreCase))
            {
                if (!bestChannels.Contains("instagram"))
                    bestChannels.Add("instagram");
            }

            if (memory.Content.Contains("facebook", StringComparison.OrdinalIgnoreCase) ||
                memory.Tags.Contains("facebook", StringComparer.OrdinalIgnoreCase))
            {
                if (!bestChannels.Contains("facebook"))
                    bestChannels.Add("facebook");
            }

            if (memory.Content.Contains("tiktok", StringComparison.OrdinalIgnoreCase) ||
                memory.Tags.Contains("tiktok", StringComparer.OrdinalIgnoreCase))
            {
                if (!bestChannels.Contains("tiktok"))
                    bestChannels.Add("tiktok");
            }
        }

        foreach (var memory in memoryContext.CampaignMemories)
        {
            if (memory.Context != null && memory.Context.ContainsKey("bestChannel"))
            {
                var channel = memory.Context["bestChannel"]?.ToString();
                if (!string.IsNullOrEmpty(channel) && !bestChannels.Contains(channel))
                    bestChannels.Add(channel);
            }
        }

        learnings["bestPerformingChannels"] = bestChannels;

        return learnings;
    }

    /// <summary>
    /// Extrae restricciones de la memoria.
    /// </summary>
    private List<string> ExtractRestrictions(Application.DTOs.MemoryContextForAI memoryContext)
    {
        var restrictions = new List<string>();

        var allMemories = memoryContext.UserPreferences
            .Concat(memoryContext.CampaignMemories)
            .Concat(memoryContext.Learnings)
            .ToList();

        foreach (var memory in allMemories)
        {
            if (memory.Content.Contains("no usar", StringComparison.OrdinalIgnoreCase) ||
                memory.Content.Contains("evitar", StringComparison.OrdinalIgnoreCase) ||
                memory.Content.Contains("prohibido", StringComparison.OrdinalIgnoreCase))
            {
                var restriction = memory.Content;
                if (restriction.Length > 100)
                    restriction = restriction.Substring(0, 100) + "...";
                
                if (!restrictions.Contains(restriction))
                    restrictions.Add(restriction);
            }

            if (memory.Tags.Any(t => t.Contains("restriction", StringComparison.OrdinalIgnoreCase) ||
                                    t.Contains("prohibido", StringComparison.OrdinalIgnoreCase)))
            {
                var restriction = memory.Content;
                if (restriction.Length > 100)
                    restriction = restriction.Substring(0, 100) + "...";
                
                if (!restrictions.Contains(restriction))
                    restrictions.Add(restriction);
            }
        }

        return restrictions;
    }

    /// <summary>
    /// Endpoint API para guardar memoria (compatibilidad con n8n).
    /// Este endpoint existe para mantener compatibilidad con workflows que usan /api/Memory/save.
    /// </summary>
    [HttpPost("api/Memory/save")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveMemoryApi(
        [FromBody] SaveMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar datos requeridos
            if (request.TenantId == Guid.Empty)
            {
                _logger.LogWarning("SaveMemoryApi llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            if (string.IsNullOrWhiteSpace(request.MemoryType))
            {
                return BadRequest(new { error = "memoryType is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { error = "content is required" });
            }

            // Usar el mismo servicio que MemoryApiController
            var memoryService = HttpContext.RequestServices.GetRequiredService<IMarketingMemoryService>();
            var mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();

            // Guardar memoria usando el servicio
            Guid memoryId;
            if (request.MemoryType == "Learning")
            {
                memoryId = await memoryService.SaveLearningMemoryAsync(
                    request.TenantId,
                    request.Content,
                    request.Tags,
                    request.RelevanceScore,
                    cancellationToken);
            }
            else if (request.MemoryType == "Conversation")
            {
                memoryId = await memoryService.SaveConversationMemoryAsync(
                    request.TenantId,
                    request.UserId,
                    request.Content,
                    request.Context,
                    cancellationToken);
            }
            else if (request.MemoryType == "Decision")
            {
                memoryId = await memoryService.SaveDecisionMemoryAsync(
                    request.TenantId,
                    request.UserId,
                    request.CampaignId,
                    request.Content,
                    request.Context,
                    cancellationToken);
            }
            else
            {
                // Usar el comando directamente para otros tipos
                var command = new Application.UseCases.Memory.SaveMemoryCommand
                {
                    TenantId = request.TenantId,
                    UserId = request.UserId,
                    CampaignId = request.CampaignId,
                    MemoryType = request.MemoryType,
                    Content = request.Content,
                    Context = request.Context,
                    Tags = request.Tags,
                    RelevanceScore = request.RelevanceScore
                };

                var result = await mediator.Send(command, cancellationToken);
                memoryId = result.Id;
            }

            _logger.LogInformation(
                "Memory saved via /api/Memory/save: Type={MemoryType}, TenantId={TenantId}, Id={MemoryId}",
                request.MemoryType,
                request.TenantId,
                memoryId);

            return Ok(new
            {
                id = memoryId,
                memoryType = request.MemoryType,
                tenantId = request.TenantId,
                campaignId = request.CampaignId,
                message = "Memory saved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al guardar memoria para Tenant {TenantId}",
                request.TenantId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to save memory"
                });
        }
    }
}

/// <summary>
/// Request para guardar memoria desde n8n.
/// </summary>
public class SaveMemoryRequest
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CampaignId { get; set; }
    public string MemoryType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object>? Context { get; set; }
    public List<string>? Tags { get; set; }
    public int RelevanceScore { get; set; } = 5;
}

