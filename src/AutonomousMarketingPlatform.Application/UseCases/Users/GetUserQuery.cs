using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutonomousMarketingPlatform.Application.UseCases.Users;

/// <summary>
/// Query para obtener un usuario por ID.
/// </summary>
public class GetUserQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}

/// <summary>
/// Handler para obtener un usuario por ID.
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRepository<UserTenant> _userTenantRepository;

    public GetUserQueryHandler(
        UserManager<ApplicationUser> userManager,
        ITenantRepository tenantRepository,
        IRepository<UserTenant> userTenantRepository)
    {
        _userManager = userManager;
        _tenantRepository = tenantRepository;
        _userTenantRepository = userTenantRepository;
    }

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException($"Usuario {request.UserId} no encontrado");
        }

        // Obtener tenant
        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        var roles = await _userManager.GetRolesAsync(user);

        // Obtener fecha de creación desde UserTenant primario (JoinedAt)
        var createdAt = DateTime.UtcNow; // Fallback
        try
        {
            var userTenants = await _userTenantRepository.FindAsync(
                ut => ut.UserId == user.Id && ut.IsPrimary,
                user.TenantId,
                cancellationToken);
            var primaryUserTenant = userTenants.FirstOrDefault();
            if (primaryUserTenant != null)
            {
                createdAt = primaryUserTenant.JoinedAt;
            }
        }
        catch
        {
            // Si no se puede obtener, usar fallback
            createdAt = DateTime.UtcNow;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            TenantId = user.TenantId,
            TenantName = tenant?.Name ?? "N/A",
            IsActive = user.IsActive,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LockoutEndDate = user.LockoutEndDate,
            LastLoginAt = user.LastLoginAt,
            LastLoginIp = user.LastLoginIp,
            Roles = roles.ToList(),
            CreatedAt = createdAt
        };
    }
}

