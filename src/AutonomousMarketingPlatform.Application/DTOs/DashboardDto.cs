namespace AutonomousMarketingPlatform.Application.DTOs;

/// <summary>
/// DTO con todos los datos para el dashboard principal.
/// </summary>
public class DashboardDto
{
    public SystemStatusDto SystemStatus { get; set; } = new();
    public ContentSummaryDto ContentSummary { get; set; } = new();
    public AutomationStatusDto AutomationStatus { get; set; } = new();
    public List<RecentCampaignDto> RecentCampaigns { get; set; } = new();
    public MetricsDto Metrics { get; set; } = new();
}

/// <summary>
/// Estado del sistema.
/// </summary>
public class SystemStatusDto
{
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Paused, Maintenance
    public string StatusMessage { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; }
    public int ActiveUsers { get; set; }
}

/// <summary>
/// Resumen de contenido.
/// </summary>
public class ContentSummaryDto
{
    public int TotalFiles { get; set; }
    public int ImagesCount { get; set; }
    public int VideosCount { get; set; }
    public int AiGeneratedCount { get; set; }
    public int UserUploadedCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public List<RecentContentDto> RecentContent { get; set; } = new();
}

/// <summary>
/// Contenido reciente.
/// </summary>
public class RecentContentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public bool IsAiGenerated { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? PreviewUrl { get; set; }
}

/// <summary>
/// Estado de automatizaciones.
/// </summary>
public class AutomationStatusDto
{
    public int TotalAutomations { get; set; }
    public int RunningCount { get; set; }
    public int PausedCount { get; set; }
    public int CompletedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<AutomationDetailDto> ActiveAutomations { get; set; } = new();
    public DateTime? NextExecution { get; set; }
}

/// <summary>
/// Detalle de automatización.
/// </summary>
public class AutomationDetailDto
{
    public Guid Id { get; set; }
    public string AutomationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? LastExecutionAt { get; set; }
    public DateTime? NextExecutionAt { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string? LastError { get; set; }
}

/// <summary>
/// Campaña reciente.
/// </summary>
public class RecentCampaignDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public decimal? SpentAmount { get; set; }
    public int ContentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Métricas del sistema.
/// </summary>
public class MetricsDto
{
    public int TotalCampaigns { get; set; }
    public int ActiveCampaigns { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalContentGenerated { get; set; }
    public int TotalPublications { get; set; }
    public double AverageCampaignPerformance { get; set; }
    public DateTime MetricsDate { get; set; } = DateTime.UtcNow;
}

