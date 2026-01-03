using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Application.UseCases.Metrics;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AutonomousMarketingPlatform.Web.Controllers.Api;

/// <summary>
/// Controlador API para gestión de métricas.
/// Usado por workflows n8n para guardar métricas de campañas y publicaciones.
/// </summary>
/// <remarks>
/// NOTA DE SEGURIDAD: En producción, este endpoint debería tener autenticación por API key
/// o estar protegido por una red privada. Por ahora se permite acceso sin autenticación
/// para facilitar la integración con n8n en desarrollo.
/// </remarks>
[ApiController]
[Route("api/metrics")]
[AllowAnonymous]
public class MetricsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMetricsService _metricsService;
    private readonly IRepository<PublishingJob> _publishingJobRepository;
    private readonly ILogger<MetricsApiController> _logger;

    public MetricsApiController(
        IMediator mediator,
        IMetricsService metricsService,
        IRepository<PublishingJob> publishingJobRepository,
        ILogger<MetricsApiController> logger)
    {
        _mediator = mediator;
        _metricsService = metricsService;
        _publishingJobRepository = publishingJobRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene métricas de publicación desde n8n.
    /// Endpoint usado por workflows n8n para obtener métricas de un publishing job.
    /// </summary>
    /// <param name="publishingJobId">ID del publishing job (requerido)</param>
    /// <param name="fromDate">Fecha desde la cual obtener métricas (opcional)</param>
    /// <param name="toDate">Fecha hasta la cual obtener métricas (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Métricas del publishing job</returns>
    /// <response code="200">Métricas encontradas</response>
    /// <response code="400">Si los parámetros son inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("publishing-job")]
    [ProducesResponseType(typeof(List<PublishingJobMetricsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPublishingJobMetrics(
        [FromQuery] Guid publishingJobId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (publishingJobId == Guid.Empty)
            {
                _logger.LogWarning("GetPublishingJobMetrics llamado con publishingJobId vacío");
                return BadRequest(new { error = "publishingJobId is required and must be a valid GUID" });
            }

            // Obtener tenantId del publishing job
            var jobs = await _publishingJobRepository.FindAsync(
                j => j.Id == publishingJobId,
                Guid.Empty, // Buscar sin filtro de tenant
                cancellationToken);
            
            var job = jobs.FirstOrDefault();
            if (job == null)
            {
                return NotFound(new { error = "PublishingJob not found" });
            }

            var tenantId = job.TenantId;

            // Usar el servicio directamente para obtener métricas
            var metrics = await _metricsService.GetPublishingJobMetricsAsync(
                tenantId,
                publishingJobId,
                fromDate,
                toDate,
                cancellationToken);

            var responses = metrics.Select(m => new PublishingJobMetricsResponse
            {
                Id = m.Id,
                PublishingJobId = m.PublishingJobId,
                MetricDate = m.MetricDate,
                Impressions = (int)m.Impressions,
                Clicks = (int)m.Clicks,
                Likes = (int)m.Likes,
                Comments = (int)m.Comments,
                Shares = (int)m.Shares,
                Engagement = (int)m.Engagement,
                ClickThroughRate = m.ClickThroughRate,
                EngagementRate = m.EngagementRate,
                Source = m.Source
            }).ToList();

            _logger.LogInformation(
                "Publishing job metrics retrieved: PublishingJobId={PublishingJobId}, Count={Count}",
                publishingJobId,
                responses.Count);

            return Ok(responses);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al obtener métricas de publicación para PublishingJobId {PublishingJobId}",
                publishingJobId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to get publishing job metrics"
                });
        }
    }

    /// <summary>
    /// Guarda métricas de campaña desde n8n.
    /// Endpoint usado por workflows n8n para registrar métricas iniciales de campañas.
    /// </summary>
    /// <param name="request">Datos de las métricas a guardar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Métricas guardadas</returns>
    /// <response code="200">Métricas guardadas exitosamente</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("campaign")]
    [ProducesResponseType(typeof(CampaignMetricsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveCampaignMetrics(
        [FromBody] SaveCampaignMetricsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar datos requeridos
            if (request.TenantId == Guid.Empty)
            {
                _logger.LogWarning("SaveCampaignMetrics llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            if (request.CampaignId == Guid.Empty)
            {
                _logger.LogWarning("SaveCampaignMetrics llamado con campaignId vacío");
                return BadRequest(new { error = "campaignId is required and must be a valid GUID" });
            }

            // Parsear fecha
            DateTime metricDate;
            if (string.IsNullOrWhiteSpace(request.MetricDate))
            {
                metricDate = DateTime.UtcNow.Date;
            }
            else
            {
                if (!DateTime.TryParse(request.MetricDate, out metricDate))
                {
                    return BadRequest(new { error = "metricDate must be a valid date (YYYY-MM-DD)" });
                }
                metricDate = metricDate.Date;
            }

            // Crear DTO de métricas
            var metricsDto = new RegisterCampaignMetricsDto
            {
                CampaignId = request.CampaignId,
                MetricDate = metricDate,
                Impressions = request.Impressions,
                Clicks = request.Clicks,
                Likes = request.Likes,
                Comments = request.Comments,
                Shares = request.Shares,
                Source = request.Source ?? "n8n",
                Notes = request.Notes
            };

            // Guardar métricas usando el comando
            var command = new RegisterCampaignMetricsCommand
            {
                TenantId = request.TenantId,
                UserId = request.UserId ?? Guid.Empty,
                Metrics = metricsDto
            };

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Campaign metrics saved: CampaignId={CampaignId}, TenantId={TenantId}, Date={Date}",
                request.CampaignId,
                request.TenantId,
                metricDate);

            var response = new CampaignMetricsResponse
            {
                Id = result.Id,
                CampaignId = result.CampaignId,
                MetricDate = result.MetricDate,
                Impressions = (int)result.Impressions,
                Clicks = (int)result.Clicks,
                Likes = (int)result.Likes,
                Comments = (int)result.Comments,
                Shares = (int)result.Shares,
                Engagement = (int)result.Engagement,
                Source = result.Source
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al guardar métricas de campaña para Tenant {TenantId}",
                request.TenantId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to save campaign metrics"
                });
        }
    }

    /// <summary>
    /// Guarda métricas de publicación desde n8n.
    /// Endpoint usado por workflows n8n para registrar métricas iniciales de publicaciones.
    /// </summary>
    /// <param name="request">Datos de las métricas a guardar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Métricas guardadas</returns>
    /// <response code="200">Métricas guardadas exitosamente</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("publishing-job")]
    [ProducesResponseType(typeof(PublishingJobMetricsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SavePublishingJobMetrics(
        [FromBody] SavePublishingJobMetricsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar datos requeridos
            if (request.TenantId == Guid.Empty)
            {
                _logger.LogWarning("SavePublishingJobMetrics llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            if (request.PublishingJobId == Guid.Empty)
            {
                _logger.LogWarning("SavePublishingJobMetrics llamado con publishingJobId vacío");
                return BadRequest(new { error = "publishingJobId is required and must be a valid GUID" });
            }

            // Parsear fecha
            DateTime metricDate;
            if (string.IsNullOrWhiteSpace(request.MetricDate))
            {
                metricDate = DateTime.UtcNow.Date;
            }
            else
            {
                if (!DateTime.TryParse(request.MetricDate, out metricDate))
                {
                    return BadRequest(new { error = "metricDate must be a valid date (YYYY-MM-DD)" });
                }
                metricDate = metricDate.Date;
            }

            // Crear DTO de métricas
            var metricsDto = new RegisterPublishingJobMetricsDto
            {
                PublishingJobId = request.PublishingJobId,
                MetricDate = metricDate,
                Impressions = request.Impressions,
                Clicks = request.Clicks,
                Likes = request.Likes,
                Comments = request.Comments,
                Shares = request.Shares,
                Source = request.Source ?? "n8n",
                Notes = request.Notes
            };

            // Guardar métricas usando el comando
            var command = new RegisterPublishingJobMetricsCommand
            {
                TenantId = request.TenantId,
                UserId = request.UserId ?? Guid.Empty,
                Metrics = metricsDto
            };

            var result = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Publishing job metrics saved: PublishingJobId={PublishingJobId}, TenantId={TenantId}, Date={Date}",
                request.PublishingJobId,
                request.TenantId,
                metricDate);

            var response = new PublishingJobMetricsResponse
            {
                Id = result.Id,
                PublishingJobId = result.PublishingJobId,
                MetricDate = result.MetricDate,
                Impressions = (int)result.Impressions,
                Clicks = (int)result.Clicks,
                Likes = (int)result.Likes,
                Comments = (int)result.Comments,
                Shares = (int)result.Shares,
                Engagement = (int)result.Engagement,
                ClickThroughRate = result.ClickThroughRate,
                EngagementRate = result.EngagementRate,
                Source = result.Source
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al guardar métricas de publicación para Tenant {TenantId}",
                request.TenantId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to save publishing job metrics"
                });
        }
    }
}

/// <summary>
/// Request para guardar métricas de campaña.
/// </summary>
public class SaveCampaignMetricsRequest
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid CampaignId { get; set; }
    public string? MetricDate { get; set; }
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para guardar métricas de publicación.
/// </summary>
public class SavePublishingJobMetricsRequest
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid PublishingJobId { get; set; }
    public string? MetricDate { get; set; }
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Respuesta del endpoint de métricas de campaña.
/// </summary>
public class CampaignMetricsResponse
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public DateTime MetricDate { get; set; }
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Engagement { get; set; }
    public string? Source { get; set; }
}

/// <summary>
/// Respuesta del endpoint de métricas de publicación.
/// </summary>
public class PublishingJobMetricsResponse
{
    public Guid Id { get; set; }
    public Guid PublishingJobId { get; set; }
    public DateTime MetricDate { get; set; }
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Engagement { get; set; }
    public decimal? ClickThroughRate { get; set; }
    public decimal? EngagementRate { get; set; }
    public string? Source { get; set; }
}

/// <summary>
/// Request para guardar memoria.
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
    public int RelevanceScore { get; set; } = 7;
}

