using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;

namespace AutonomousMarketingPlatform.Application.UseCases.Tenants;

/// <summary>
/// Comando para crear tenant.
/// </summary>
public class CreateTenantCommand : IRequest<TenantDto>
{
    public string Name { get; set; } = string.Empty;
    public string? Subdomain { get; set; }
    public string? ContactEmail { get; set; }
}

/// <summary>
/// Handler para crear tenant.
/// </summary>
public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;

    public CreateTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya existe un tenant con el mismo subdomain
        if (!string.IsNullOrEmpty(request.Subdomain))
        {
            var existingBySubdomain = await _tenantRepository.GetBySubdomainAsync(request.Subdomain, cancellationToken);
            if (existingBySubdomain != null)
            {
                throw new InvalidOperationException($"Ya existe un tenant con subdomain {request.Subdomain}");
            }
        }

        var tenant = new Tenant
        {
            Name = request.Name,
            Subdomain = request.Subdomain ?? string.Empty,
            ContactEmail = request.ContactEmail ?? string.Empty,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _tenantRepository.AddAsync(tenant, cancellationToken);

        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Subdomain = tenant.Subdomain,
            ContactEmail = tenant.ContactEmail,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            UserCount = 0
        };
    }
}

