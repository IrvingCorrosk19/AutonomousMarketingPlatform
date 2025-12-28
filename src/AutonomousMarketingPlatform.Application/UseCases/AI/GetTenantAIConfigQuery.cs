using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;

namespace AutonomousMarketingPlatform.Application.UseCases.AI;

/// <summary>
/// Query para obtener la configuración de IA del tenant.
/// </summary>
public class GetTenantAIConfigQuery : IRequest<TenantAIConfigDto?>
{
    public Guid TenantId { get; set; }
    public string Provider { get; set; } = "OpenAI";
}

/// <summary>
/// Handler para obtener configuración de IA.
/// </summary>
public class GetTenantAIConfigQueryHandler : IRequestHandler<GetTenantAIConfigQuery, TenantAIConfigDto?>
{
    private readonly IRepository<Domain.Entities.TenantAIConfig> _configRepository;

    public GetTenantAIConfigQueryHandler(IRepository<Domain.Entities.TenantAIConfig> configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<TenantAIConfigDto?> Handle(GetTenantAIConfigQuery request, CancellationToken cancellationToken)
    {
        var configs = await _configRepository.FindAsync(
            c => c.TenantId == request.TenantId && 
                 c.Provider == request.Provider && 
                 c.IsActive,
            request.TenantId,
            cancellationToken);

        var config = configs.FirstOrDefault();
        
        if (config == null)
            return null;

        return new TenantAIConfigDto
        {
            Id = config.Id,
            Provider = config.Provider,
            Model = config.Model,
            IsActive = config.IsActive,
            LastUsedAt = config.LastUsedAt,
            UsageCount = config.UsageCount
        };
    }
}

