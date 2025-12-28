using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.Content;
using AutonomousMarketingPlatform.Web.Attributes;
using AutonomousMarketingPlatform.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para gestión de contenido (imágenes y videos).
/// Requiere rol Marketer, Admin o Owner para cargar contenido.
/// </summary>
[Authorize]
[AuthorizeRole("Marketer", "Admin", "Owner")]
public class ContentController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContentController> _logger;

    public ContentController(IMediator mediator, ILogger<ContentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Vista principal para cargar archivos.
    /// </summary>
    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    /// <summary>
    /// Endpoint para cargar múltiples archivos.
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromForm] Guid? campaignId, [FromForm] string? description, [FromForm] string? tags)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { error = "No se proporcionaron archivos." });
        }

        try
        {
            var userId = UserHelper.GetUserId(User);
            var tenantId = UserHelper.GetTenantId(User);

            if (!userId.HasValue || !tenantId.HasValue)
            {
                return BadRequest(new { error = "Error de autenticación. Por favor, inicie sesión nuevamente." });
            }

            var command = new UploadFilesCommand
            {
                UserId = userId.Value,
                TenantId = tenantId.Value,
                Files = files,
                CampaignId = campaignId,
                Description = description,
                Tags = tags
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar archivos");
            return StatusCode(500, new { error = "Error al procesar los archivos. Por favor, intente nuevamente." });
        }
    }

    /// <summary>
    /// Vista para listar contenido cargado.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}

