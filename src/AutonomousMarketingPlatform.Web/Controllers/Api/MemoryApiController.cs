using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers.Api;

/// <summary>
/// Controlador API para gestión de memoria de marketing.
/// Usado por workflows n8n para cargar contexto de memoria.
/// </summary>
/// <remarks>
/// NOTA DE SEGURIDAD: En producción, este endpoint debería tener autenticación por API key
/// o estar protegido por una red privada. Por ahora se permite acceso sin autenticación
/// para facilitar la integración con n8n en desarrollo.
/// </remarks>
[ApiController]
[Route("api/memory")]
[AllowAnonymous]
public class MemoryApiController : ControllerBase
{
    private readonly IMarketingMemoryService _memoryService;
    private readonly IMediator _mediator;
    private readonly ILogger<MemoryApiController> _logger;

    public MemoryApiController(
        IMarketingMemoryService memoryService,
        IMediator mediator,
        ILogger<MemoryApiController> logger)
    {
        _memoryService = memoryService;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene memorias de marketing por tipo.
    /// Endpoint usado por workflows n8n para obtener memorias específicas.
    /// </summary>
    /// <param name="tenantId">ID del tenant (requerido)</param>
    /// <param name="memoryType">Tipo de memoria (opcional, ej: "Pattern", "Learning")</param>
    /// <param name="tags">Tags para filtrar (opcional)</param>
    /// <param name="limit">Límite de resultados (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de memorias</returns>
    /// <response code="200">Lista de memorias</response>
    /// <response code="400">Si los parámetros son inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<MarketingMemoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMemories(
        [FromQuery] Guid tenantId,
        [FromQuery] string? memoryType = null,
        [FromQuery] string? tags = null,
        [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (tenantId == Guid.Empty)
            {
                _logger.LogWarning("GetMemories llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            // Usar el servicio para obtener memorias
            var memoryContext = await _memoryService.GetMemoryContextForAIAsync(
                tenantId,
                null,
                null,
                null,
                cancellationToken);

            var memories = new List<MarketingMemoryDto>();

            // Filtrar por tipo de memoria
            if (!string.IsNullOrWhiteSpace(memoryType))
            {
                if (memoryType.Equals("Pattern", StringComparison.OrdinalIgnoreCase))
                {
                    memories = memoryContext.Learnings
                        .Where(m => m.MemoryType.Equals("Pattern", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else if (memoryType.Equals("Learning", StringComparison.OrdinalIgnoreCase))
                {
                    memories = memoryContext.Learnings
                        .Where(m => m.MemoryType.Equals("Learning", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else
                {
                    memories = memoryContext.Learnings
                        .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            else
            {
                memories = memoryContext.Learnings.ToList();
            }

            // Filtrar por tags si se proporciona
            if (!string.IsNullOrWhiteSpace(tags))
            {
                var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
                memories = memories.Where(m => 
                    m.Tags.Any(t => tagList.Any(tag => t.Contains(tag, StringComparison.OrdinalIgnoreCase))))
                    .ToList();
            }

            // Aplicar límite
            if (limit.HasValue && limit.Value > 0)
            {
                memories = memories.Take(limit.Value).ToList();
            }

            _logger.LogInformation(
                "Memories retrieved for Tenant {TenantId}: Type={MemoryType}, Count={Count}",
                tenantId,
                memoryType ?? "All",
                memories.Count);

            return Ok(memories);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al obtener memorias para Tenant {TenantId}",
                tenantId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to get memories"
                });
        }
    }

    /// <summary>
    /// Obtiene el contexto de memoria de marketing para un tenant.
    /// Endpoint usado por workflows n8n para cargar preferencias, aprendizajes y restricciones.
    /// </summary>
    /// <param name="tenantId">ID del tenant</param>
    /// <param name="userId">ID del usuario (opcional)</param>
    /// <param name="campaignId">ID de la campaña (opcional)</param>
    /// <param name="memoryType">Tipo de memoria a filtrar (opcional: Preference, Learning, Feedback, Pattern)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Contexto de memoria con preferencias, conversaciones, campañas y aprendizajes</returns>
    /// <response code="200">Retorna el contexto de memoria</response>
    /// <response code="400">Si tenantId no es válido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("context")]
    [ProducesResponseType(typeof(MemoryContextResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMemoryContext(
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
                _logger.LogWarning("GetMemoryContext llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            // Obtener contexto de memoria
            var memoryContext = await _memoryService.GetMemoryContextForAIAsync(
                tenantId,
                userId,
                campaignId,
                null,
                cancellationToken);

            // Si se especifica memoryType, filtrar las memorias
            if (!string.IsNullOrWhiteSpace(memoryType))
            {
                // Filtrar UserPreferences, CampaignMemories y Learnings por memoryType
                var filteredUserPreferences = memoryContext.UserPreferences
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var filteredCampaignMemories = memoryContext.CampaignMemories
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var filteredLearnings = memoryContext.Learnings
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Crear contexto filtrado
                var filteredRecentConversations = memoryContext.RecentConversations
                    .Where(m => m.MemoryType.Equals(memoryType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                var filteredContext = new MemoryContextForAI
                {
                    UserPreferences = filteredUserPreferences,
                    RecentConversations = filteredRecentConversations,
                    CampaignMemories = filteredCampaignMemories,
                    Learnings = filteredLearnings,
                    SummarizedContext = memoryContext.SummarizedContext
                };

                // Extraer preferencias, aprendizajes y restricciones del contexto filtrado
                var preferences = ExtractPreferences(filteredContext);
                var learnings = ExtractLearnings(filteredContext);
                var restrictions = ExtractRestrictions(filteredContext);

                var response = new MemoryContextResponse
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

                _logger.LogInformation(
                    "Memory context loaded for Tenant {TenantId} with MemoryType filter {MemoryType}: Preferences={PrefCount}, Learnings={LearnCount}, Restrictions={RestCount}",
                    tenantId,
                    memoryType,
                    preferences.Count,
                    learnings.Count,
                    restrictions.Count);

                return Ok(response);
            }

            // Si no se especifica memoryType, retornar todo el contexto
            var allPreferences = ExtractPreferences(memoryContext);
            var allLearnings = ExtractLearnings(memoryContext);
            var allRestrictions = ExtractRestrictions(memoryContext);

            var fullResponse = new MemoryContextResponse
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

            _logger.LogInformation(
                "Memory context loaded for Tenant {TenantId}: Preferences={PrefCount}, Learnings={LearnCount}, Restrictions={RestCount}",
                tenantId,
                allPreferences.Count,
                allLearnings.Count,
                allRestrictions.Count);

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
    private Dictionary<string, object> ExtractPreferences(MemoryContextForAI memoryContext)
    {
        var preferences = new Dictionary<string, object>();

        // Buscar preferencias en UserPreferences
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

            // Extraer de contenido si contiene información de preferencias
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

        // Valores por defecto si no se encontraron
        if (!preferences.ContainsKey("preferredTone"))
            preferences["preferredTone"] = "profesional";
        
        if (!preferences.ContainsKey("dislikedFormats"))
            preferences["dislikedFormats"] = new List<string>();

        return preferences;
    }

    /// <summary>
    /// Extrae aprendizajes del sistema de la memoria.
    /// </summary>
    private Dictionary<string, object> ExtractLearnings(MemoryContextForAI memoryContext)
    {
        var learnings = new Dictionary<string, object>();
        var bestChannels = new List<string>();

        // Buscar aprendizajes sobre canales
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

        // Buscar en memorias de campañas
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
    private List<string> ExtractRestrictions(MemoryContextForAI memoryContext)
    {
        var restrictions = new List<string>();

        // Buscar restricciones en todas las memorias
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
                // Extraer la restricción del contenido
                var restriction = memory.Content;
                if (restriction.Length > 100)
                    restriction = restriction.Substring(0, 100) + "...";
                
                if (!restrictions.Contains(restriction))
                    restrictions.Add(restriction);
            }

            // Buscar en tags
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
    /// Guarda una memoria de marketing desde n8n.
    /// Endpoint usado por workflows n8n para guardar aprendizajes y decisiones.
    /// </summary>
    /// <param name="request">Datos de la memoria a guardar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Memoria guardada</returns>
    /// <response code="200">Memoria guardada exitosamente</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("save")]
    [ProducesResponseType(typeof(MarketingMemoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveMemory(
        [FromBody] SaveMemoryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar datos requeridos
            if (request.TenantId == Guid.Empty)
            {
                _logger.LogWarning("SaveMemory llamado con tenantId vacío");
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

            // Guardar memoria usando el servicio
            Guid memoryId;
            if (request.MemoryType == "Learning")
            {
                memoryId = await _memoryService.SaveLearningMemoryAsync(
                    request.TenantId,
                    request.Content,
                    request.Tags,
                    request.RelevanceScore,
                    cancellationToken);
            }
            else if (request.MemoryType == "Conversation")
            {
                memoryId = await _memoryService.SaveConversationMemoryAsync(
                    request.TenantId,
                    request.UserId,
                    request.Content,
                    request.Context,
                    cancellationToken);
            }
            else if (request.MemoryType == "Decision")
            {
                memoryId = await _memoryService.SaveDecisionMemoryAsync(
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

                var result = await _mediator.Send(command, cancellationToken);
                memoryId = result.Id;
            }

            _logger.LogInformation(
                "Memory saved: Type={MemoryType}, TenantId={TenantId}, Id={MemoryId}",
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
/// Respuesta del endpoint de contexto de memoria.
/// </summary>
public class MemoryContextResponse
{
    /// <summary>
    /// Preferencias del usuario (tono, formatos, etc.).
    /// </summary>
    public Dictionary<string, object> Preferences { get; set; } = new();

    /// <summary>
    /// Aprendizajes del sistema (canales que mejor funcionan, etc.).
    /// </summary>
    public Dictionary<string, object> Learnings { get; set; } = new();

    /// <summary>
    /// Restricciones y limitaciones.
    /// </summary>
    public List<string> Restrictions { get; set; } = new();

    /// <summary>
    /// Lista completa de preferencias del usuario.
    /// </summary>
    public List<MarketingMemoryDto> UserPreferences { get; set; } = new();

    /// <summary>
    /// Conversaciones recientes.
    /// </summary>
    public List<MarketingMemoryDto> RecentConversations { get; set; } = new();

    /// <summary>
    /// Memorias de campañas.
    /// </summary>
    public List<MarketingMemoryDto> CampaignMemories { get; set; } = new();

    /// <summary>
    /// Lista de aprendizajes.
    /// </summary>
    public List<MarketingMemoryDto> LearningsList { get; set; } = new();

    /// <summary>
    /// Contexto resumido en texto.
    /// </summary>
    public string SummarizedContext { get; set; } = string.Empty;
}

