using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

/// <summary>
/// Controlador para gestión de automatizaciones.
/// </summary>
[Authorize]
public class AutomationsController : Controller
{
    private readonly IRepository<AutomationState> _automationRepository;
    private readonly ILogger<AutomationsController> _logger;

    public AutomationsController(
        IRepository<AutomationState> automationRepository,
        ILogger<AutomationsController> logger)
    {
        _automationRepository = automationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas las automatizaciones del tenant.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            var isSuperAdmin = User.HasClaim("IsSuperAdmin", "true");
            
            if (!isSuperAdmin && !tenantId.HasValue)
            {
                _logger.LogWarning("[AutomationsController.Index] No se pudo determinar el tenant");
                TempData["ErrorMessage"] = "No se pudo determinar el tenant. Por favor, contacte al administrador.";
                return RedirectToAction("Index", "Home");
            }

            // Para SuperAdmin, necesitamos obtener todas las automatizaciones de todos los tenants
            // Por ahora, si es SuperAdmin, usamos el tenant del usuario actual o mostramos vacío
            var effectiveTenantId = isSuperAdmin ? (tenantId ?? Guid.Empty) : (tenantId ?? Guid.Empty);
            
            IEnumerable<AutomationState> automations;
            if (isSuperAdmin && effectiveTenantId == Guid.Empty)
            {
                // SuperAdmin sin tenant específico - mostrar lista vacía por ahora
                // TODO: Implementar consulta que obtenga todas las automatizaciones de todos los tenants
                automations = new List<AutomationState>();
            }
            else
            {
                automations = await _automationRepository.GetAllAsync(effectiveTenantId);
            }
            
            var automationsList = automations.ToList();

            var automationStatus = new AutomationStatusDto
            {
                TotalAutomations = automationsList.Count,
                RunningCount = automationsList.Count(a => a.Status == "Running"),
                PausedCount = automationsList.Count(a => a.Status == "Paused"),
                CompletedCount = automationsList.Count(a => a.Status == "Completed"),
                ErrorCount = automationsList.Count(a => a.Status == "Error"),
                ActiveAutomations = automationsList
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new AutomationDetailDto
                    {
                        Id = a.Id,
                        AutomationType = a.AutomationType,
                        Status = a.Status,
                        LastExecutionAt = a.LastExecutionAt,
                        NextExecutionAt = a.NextExecutionAt,
                        SuccessCount = a.SuccessCount,
                        FailureCount = a.FailureCount,
                        LastError = a.ErrorMessage
                    })
                    .ToList(),
                NextExecution = automationsList
                    .Where(a => a.NextExecutionAt.HasValue && a.Status != "Paused")
                    .OrderBy(a => a.NextExecutionAt)
                    .Select(a => a.NextExecutionAt)
                    .FirstOrDefault()
            };

            return View(automationStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AutomationsController.Index] Error al listar automatizaciones");
            TempData["ErrorMessage"] = "Error al cargar las automatizaciones. Por favor, intente nuevamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}

