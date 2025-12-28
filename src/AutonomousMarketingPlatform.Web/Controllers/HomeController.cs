using AutonomousMarketingPlatform.Application.UseCases.Dashboard;
using AutonomousMarketingPlatform.Web.Attributes;
using AutonomousMarketingPlatform.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutonomousMarketingPlatform.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IMediator mediator, ILogger<HomeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var tenantId = UserHelper.GetTenantId(User);
            if (!tenantId.HasValue)
            {
                _logger.LogWarning("Usuario autenticado sin TenantId");
                return RedirectToAction("Login", "Account");
            }

            var query = new GetDashboardDataQuery
            {
                TenantId = tenantId.Value
            };

            var dashboardData = await _mediator.Send(query);
            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar datos del dashboard");
            return View(new Application.DTOs.DashboardDto());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }
}

