using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Web.Controllers.Api;

/// <summary>
/// Controlador API para gestión de PublishingJobs.
/// Usado por workflows n8n para guardar resultados de publicaciones.
/// </summary>
/// <remarks>
/// NOTA DE SEGURIDAD: En producción, este endpoint debería tener autenticación por API key
/// o estar protegido por una red privada. Por ahora se permite acceso sin autenticación
/// para facilitar la integración con n8n en desarrollo.
/// </remarks>
[ApiController]
[Route("api/publishing-jobs")]
[AllowAnonymous]
public class PublishingJobsApiController : ControllerBase
{
    private readonly IRepository<PublishingJob> _publishingJobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishingJobsApiController> _logger;

    public PublishingJobsApiController(
        IRepository<PublishingJob> publishingJobRepository,
        IUnitOfWork unitOfWork,
        ILogger<PublishingJobsApiController> logger)
    {
        _publishingJobRepository = publishingJobRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Crea un PublishingJob desde n8n después de publicar contenido.
    /// Endpoint usado por workflows n8n para guardar resultados de publicaciones.
    /// </summary>
    /// <param name="request">Datos del PublishingJob a guardar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>PublishingJob guardado</returns>
    /// <response code="200">PublishingJob guardado exitosamente</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(PublishingJobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePublishingJob(
        [FromBody] CreatePublishingJobRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar datos requeridos
            if (request.TenantId == Guid.Empty)
            {
                _logger.LogWarning("CreatePublishingJob llamado con tenantId vacío");
                return BadRequest(new { error = "tenantId is required and must be a valid GUID" });
            }

            if (request.CampaignId == Guid.Empty)
            {
                _logger.LogWarning("CreatePublishingJob llamado con campaignId vacío");
                return BadRequest(new { error = "campaignId is required and must be a valid GUID" });
            }

            if (string.IsNullOrWhiteSpace(request.Channel))
            {
                return BadRequest(new { error = "channel is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { error = "content is required" });
            }

            // Construir payload si no se proporciona
            var payload = request.Payload;
            if (string.IsNullOrWhiteSpace(payload))
            {
                var payloadObj = new
                {
                    Copy = request.Content,
                    Hashtags = request.Hashtags,
                    MediaUrls = !string.IsNullOrWhiteSpace(request.MediaUrl) 
                        ? new List<string> { request.MediaUrl } 
                        : new List<string>(),
                    CampaignId = request.CampaignId.ToString(),
                    MarketingPackId = request.MarketingPackId?.ToString(),
                    GeneratedCopyId = request.GeneratedCopyId?.ToString(),
                    Metadata = new Dictionary<string, object>()
                };
                payload = JsonSerializer.Serialize(payloadObj);
            }

            // Crear PublishingJob
            var publishingJob = new PublishingJob
            {
                Id = request.Id ?? Guid.NewGuid(),
                TenantId = request.TenantId,
                CampaignId = request.CampaignId,
                MarketingPackId = request.MarketingPackId,
                GeneratedCopyId = request.GeneratedCopyId,
                Channel = request.Channel,
                Status = request.Status ?? "Success",
                ScheduledDate = request.ScheduledDate.HasValue 
                    ? DateTime.SpecifyKind(request.ScheduledDate.Value, DateTimeKind.Utc) 
                    : null,
                PublishedDate = request.PublishedDate.HasValue 
                    ? DateTime.SpecifyKind(request.PublishedDate.Value, DateTimeKind.Utc) 
                    : DateTime.UtcNow,
                PublishedUrl = request.PublishedUrl,
                ExternalPostId = request.ExternalPostId,
                Content = request.Content,
                Hashtags = request.Hashtags,
                MediaUrl = request.MediaUrl,
                ErrorMessage = request.ErrorMessage,
                Payload = payload,
                Metadata = request.Metadata,
                RequiresApproval = false, // Ya fue publicado, no requiere aprobación
                RetryCount = 0,
                MaxRetries = 3
            };

            await _publishingJobRepository.AddAsync(publishingJob, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "PublishingJob {JobId} saved for Tenant {TenantId} on Channel {Channel} with status {Status}",
                publishingJob.Id,
                request.TenantId,
                request.Channel,
                publishingJob.Status);

            var response = new PublishingJobResponse
            {
                Id = publishingJob.Id,
                TenantId = publishingJob.TenantId,
                CampaignId = publishingJob.CampaignId,
                MarketingPackId = publishingJob.MarketingPackId,
                GeneratedCopyId = publishingJob.GeneratedCopyId,
                Channel = publishingJob.Channel,
                Status = publishingJob.Status,
                PublishedDate = publishingJob.PublishedDate,
                PublishedUrl = publishingJob.PublishedUrl,
                ExternalPostId = publishingJob.ExternalPostId,
                CreatedAt = publishingJob.CreatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al guardar PublishingJob para Tenant {TenantId}",
                request.TenantId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "Internal server error",
                    message = "Failed to save publishing job"
                });
        }
    }
}

/// <summary>
/// Request para crear PublishingJob.
/// </summary>
public class CreatePublishingJobRequest
{
    public Guid? Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CampaignId { get; set; }
    public Guid? MarketingPackId { get; set; }
    public Guid? GeneratedCopyId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }
    public string? PublishedUrl { get; set; }
    public string? ExternalPostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Hashtags { get; set; }
    public string? MediaUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Payload { get; set; }
    public string? Metadata { get; set; }
}

/// <summary>
/// Respuesta del endpoint de PublishingJob.
/// </summary>
public class PublishingJobResponse
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CampaignId { get; set; }
    public Guid? MarketingPackId { get; set; }
    public Guid? GeneratedCopyId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishedDate { get; set; }
    public string? PublishedUrl { get; set; }
    public string? ExternalPostId { get; set; }
    public DateTime CreatedAt { get; set; }
}

