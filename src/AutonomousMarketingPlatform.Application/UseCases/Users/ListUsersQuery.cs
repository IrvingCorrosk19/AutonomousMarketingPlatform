using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutonomousMarketingPlatform.Application.UseCases.Users;

/// <summary>
/// Query para listar usuarios.
/// </summary>
public class ListUsersQuery : IRequest<List<UserListDto>>
{
    public Guid TenantId { get; set; }
    public bool? IsActive { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// Handler para listar usuarios.
/// </summary>
public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, List<UserListDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITenantRepository _tenantRepository;

    public ListUsersQueryHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITenantRepository tenantRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tenantRepository = tenantRepository;
    }

    public async Task<List<UserListDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        // Filtrar por tenant
        if (request.TenantId != Guid.Empty)
        {
            query = query.Where(u => u.TenantId == request.TenantId);
        }

        // Filtrar por estado activo
        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var users = await query.ToListAsync(cancellationToken);
        var result = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            // Filtrar por rol si se especifica
            if (!string.IsNullOrEmpty(request.Role) && !roles.Contains(request.Role))
            {
                continue;
            }

            // Obtener tenant
            var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);

            result.Add(new UserListDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                TenantName = tenant?.Name ?? "N/A",
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                LastLoginAt = user.LastLoginAt
            });
        }

        return result;
    }
}

