using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using ContentEntity = AutonomousMarketingPlatform.Domain.Entities.Content;
using CampaignEntity = AutonomousMarketingPlatform.Domain.Entities.Campaign;
using AutomationStateEntity = AutonomousMarketingPlatform.Domain.Entities.AutomationState;

namespace AutonomousMarketingPlatform.Application.UseCases.Dashboard;

/// <summary>
/// Query para obtener todos los datos del dashboard.
/// </summary>
public class GetDashboardDataQuery : IRequest<DashboardDto>
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
}

/// <summary>
/// Handler para obtener datos del dashboard.
/// </summary>
public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDto>
{
    private readonly IRepository<CampaignEntity> _campaignRepository;
    private readonly IRepository<ContentEntity> _contentRepository;
    private readonly IRepository<AutomationStateEntity> _automationRepository;
    private readonly IRepository<Domain.Entities.User> _userRepository;

    public GetDashboardDataQueryHandler(
        IRepository<CampaignEntity> campaignRepository,
        IRepository<ContentEntity> contentRepository,
        IRepository<AutomationStateEntity> automationRepository,
        IRepository<Domain.Entities.User> userRepository)
    {
        _campaignRepository = campaignRepository;
        _contentRepository = contentRepository;
        _automationRepository = automationRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var dashboard = new DashboardDto();

        // Estado del Sistema
        var activeUsers = await _userRepository.FindAsync(
            u => u.IsActive && u.LastLoginAt.HasValue && u.LastLoginAt.Value > DateTime.UtcNow.AddHours(-24),
            request.TenantId,
            cancellationToken);

        dashboard.SystemStatus = new SystemStatusDto
        {
            IsActive = true,
            Status = "Active",
            StatusMessage = "Sistema operativo y funcionando correctamente",
            LastActivity = DateTime.UtcNow,
            ActiveUsers = activeUsers.Count()
        };

        // Resumen de Contenido
        var allContent = await _contentRepository.GetAllAsync(request.TenantId, cancellationToken);
        var contentList = allContent.ToList();

        dashboard.ContentSummary = new ContentSummaryDto
        {
            TotalFiles = contentList.Count,
            ImagesCount = contentList.Count(c => c.ContentType == "Image"),
            VideosCount = contentList.Count(c => c.ContentType == "Video"),
            AiGeneratedCount = contentList.Count(c => c.IsAiGenerated),
            UserUploadedCount = contentList.Count(c => !c.IsAiGenerated),
            TotalSizeBytes = contentList.Where(c => c.FileSize.HasValue).Sum(c => c.FileSize ?? 0),
            RecentContent = contentList
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .Select(c => new RecentContentDto
                {
                    Id = c.Id,
                    FileName = c.OriginalFileName ?? "Sin nombre",
                    ContentType = c.ContentType,
                    IsAiGenerated = c.IsAiGenerated,
                    CreatedAt = c.CreatedAt
                })
                .ToList()
        };

        // Estado de Automatizaciones
        var allAutomations = await _automationRepository.GetAllAsync(request.TenantId, cancellationToken);
        var automationsList = allAutomations.ToList();

        dashboard.AutomationStatus = new AutomationStatusDto
        {
            TotalAutomations = automationsList.Count,
            RunningCount = automationsList.Count(a => a.Status == "Running"),
            PausedCount = automationsList.Count(a => a.Status == "Paused"),
            CompletedCount = automationsList.Count(a => a.Status == "Completed"),
            ErrorCount = automationsList.Count(a => a.Status == "Error"),
            ActiveAutomations = automationsList
                .Where(a => a.Status == "Running" || a.Status == "Scheduled")
                .OrderBy(a => a.NextExecutionAt)
                .Take(5)
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

        // Campañas Recientes
        var allCampaigns = await _campaignRepository.GetAllAsync(request.TenantId, cancellationToken);
        var campaignsList = allCampaigns.ToList();

        dashboard.RecentCampaigns = campaignsList
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new RecentCampaignDto
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Budget = c.Budget,
                SpentAmount = c.SpentAmount,
                ContentCount = c.Contents?.Count ?? 0,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        // Métricas
        dashboard.Metrics = new MetricsDto
        {
            TotalCampaigns = campaignsList.Count,
            ActiveCampaigns = campaignsList.Count(c => c.Status == "Active"),
            TotalBudget = campaignsList.Where(c => c.Budget.HasValue).Sum(c => c.Budget ?? 0),
            TotalSpent = campaignsList.Where(c => c.SpentAmount.HasValue).Sum(c => c.SpentAmount ?? 0),
            TotalContentGenerated = contentList.Count(c => c.IsAiGenerated),
            TotalPublications = 0, // TODO: Implementar cuando tengamos publicaciones
            AverageCampaignPerformance = campaignsList.Any() ? 85.5 : 0, // Placeholder
            MetricsDate = DateTime.UtcNow
        };

        return dashboard;
    }
}

