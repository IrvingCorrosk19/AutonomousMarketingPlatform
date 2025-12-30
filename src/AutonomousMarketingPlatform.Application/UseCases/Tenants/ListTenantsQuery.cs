using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutonomousMarketingPlatform.Application.UseCases.Tenants;

/// <summary>
/// Query para listar tenants.
/// </summary>
public class ListTenantsQuery : IRequest<List<TenantDto>>
{
}

/// <summary>
/// Handler para listar tenants.
/// </summary>
public class ListTenantsQueryHandler : IRequestHandler<ListTenantsQuery, List<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListTenantsQueryHandler(
        ITenantRepository tenantRepository,
        UserManager<ApplicationUser> userManager)
    {
        _tenantRepository = tenantRepository;
        _userManager = userManager;
    }

    public async Task<List<TenantDto>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
    {
        // Obtener todos los tenants
        var tenantsList = await _tenantRepository.GetAllAsync(cancellationToken);
        
        var result = new List<TenantDto>();
        foreach (var tenant in tenantsList)
        {
            var userCount = _userManager.Users.Count(u => u.TenantId == tenant.Id);
            
            result.Add(new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                ContactEmail = tenant.ContactEmail,
                IsActive = tenant.IsActive,
                CreatedAt = tenant.CreatedAt,
                UserCount = userCount
            });
        }
        
        return result;
    }
}

