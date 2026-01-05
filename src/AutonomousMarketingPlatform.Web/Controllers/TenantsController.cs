using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.Tenants;
using AutonomousMarketingPlatform.Web.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para gestión de tenants.
/// Requiere rol Owner o SuperAdmin.
/// </summary>
[Authorize]
[AuthorizeRole("Owner", "SuperAdmin")]
public class TenantsController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(IMediator mediator, ILogger<TenantsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista de tenants.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var query = new ListTenantsQuery();
            var tenants = await _mediator.Send(query);
            return View(tenants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar tenants");
            return View(new List<TenantDto>());
        }
    }

    /// <summary>
    /// Formulario para crear tenant.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateTenantDto());
    }

    /// <summary>
    /// Crear tenant.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateTenantDto model)
    {

       
        _logger.LogInformation("=== [TenantsController.Create] POST INICIADO ===");
        _logger.LogInformation("[TenantsController.Create] Request Path: {Path}", HttpContext.Request.Path);
        _logger.LogInformation("[TenantsController.Create] Request Method: {Method}", HttpContext.Request.Method);
        _logger.LogInformation("[TenantsController.Create] ContentType: {ContentType}", Request.ContentType);
        _logger.LogInformation("[TenantsController.Create] HasFormContentType: {HasFormContentType}", Request.HasFormContentType);
        _logger.LogInformation("[TenantsController.Create] Form.Count: {Count}", Request.Form?.Count ?? 0);
        _logger.LogInformation("[TenantsController.Create] Model recibido (antes de binding): {Model}", model != null ? "NOT NULL" : "NULL");

        // Loggear todos los valores del formulario
        if (Request.Form != null && Request.Form.Count > 0)
        {
            _logger.LogInformation("[TenantsController.Create] === VALORES DEL FORMULARIO ===");
            foreach (var key in Request.Form.Keys)
            {
                var value = Request.Form[key].ToString();
                _logger.LogInformation("[TenantsController.Create] Form[{Key}] = {Value}", key, value);
            }
        }

        // Si el modelo es null, crear uno nuevo
        if (model == null)
        {
            _logger.LogWarning("[TenantsController.Create] Model es NULL, creando nuevo modelo");
            model = new CreateTenantDto();
        }

        // Loggear el modelo recibido
        _logger.LogInformation("[TenantsController.Create] === MODELO RECIBIDO ===");
        _logger.LogInformation("[TenantsController.Create] Model.Name: {Name}", model.Name ?? "NULL");
        _logger.LogInformation("[TenantsController.Create] Model.Subdomain: {Subdomain}", model.Subdomain ?? "NULL");
        _logger.LogInformation("[TenantsController.Create] Model.ContactEmail: {ContactEmail}", model.ContactEmail ?? "NULL");

        // Loggear ModelState
        _logger.LogInformation("[TenantsController.Create] === MODELSTATE ===");
        _logger.LogInformation("[TenantsController.Create] ModelState.IsValid: {IsValid}", ModelState.IsValid);
        _logger.LogInformation("[TenantsController.Create] ModelState.ErrorCount: {ErrorCount}", ModelState.ErrorCount);
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[TenantsController.Create] ModelState NO ES VÁLIDO");
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state?.Errors != null && state.Errors.Count > 0)
                {
                    foreach (var error in state.Errors)
                    {
                        _logger.LogWarning("[TenantsController.Create] ModelState Error - Key: {Key}, Error: {Error}", 
                            key, error.ErrorMessage);
                    }
                }
            }
            return View(model);
        }

        _logger.LogInformation("[TenantsController.Create] ModelState ES VÁLIDO, procediendo a crear tenant");

        try
        {
            var command = new CreateTenantCommand
            {
                Name = model.Name,
                Subdomain = model.Subdomain,
                ContactEmail = model.ContactEmail
            };

            _logger.LogInformation("[TenantsController.Create] Comando creado - Name: {Name}, Subdomain: {Subdomain}, ContactEmail: {ContactEmail}",
                command.Name, command.Subdomain, command.ContactEmail);

            _logger.LogInformation("[TenantsController.Create] Enviando comando a Mediator...");
            var result = await _mediator.Send(command);
            _logger.LogInformation("[TenantsController.Create] Tenant creado exitosamente - Id: {Id}, Name: {Name}", 
                result.Id, result.Name);

            TempData["SuccessMessage"] = "Tenant creado exitosamente.";
            _logger.LogInformation("[TenantsController.Create] Redirigiendo a Index");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TenantsController.Create] ERROR al crear tenant");
            _logger.LogError("[TenantsController.Create] Exception Type: {Type}", ex.GetType().Name);
            _logger.LogError("[TenantsController.Create] Exception Message: {Message}", ex.Message);
            _logger.LogError("[TenantsController.Create] Exception StackTrace: {StackTrace}", ex.StackTrace);
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    /// <summary>
    /// Muestra el formulario para editar un tenant.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var query = new GetTenantQuery
            {
                TenantId = id
            };

            var tenant = await _mediator.Send(query);

            if (tenant == null)
            {
                TempData["ErrorMessage"] = "Tenant no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UpdateTenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                ContactEmail = tenant.ContactEmail,
                IsActive = tenant.IsActive
            };

            return View(updateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tenant para editar {TenantId}", id);
            TempData["ErrorMessage"] = "Error al cargar el tenant para editar.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Actualiza un tenant existente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateTenantDto model)
    {
        // Asegurar que el Id del modelo coincida con el parámetro de ruta
        model.Id = id;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new UpdateTenantCommand
            {
                TenantId = id,
                Tenant = model
            };

            await _mediator.Send(command);
            TempData["SuccessMessage"] = "Tenant actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tenant {TenantId}", id);
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}

