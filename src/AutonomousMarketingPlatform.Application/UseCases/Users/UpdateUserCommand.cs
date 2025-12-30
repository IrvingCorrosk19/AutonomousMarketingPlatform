using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutonomousMarketingPlatform.Application.UseCases.Users;

/// <summary>
/// Comando para actualizar usuario.
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public bool? IsActive { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// Handler para actualizar usuario.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRepository<UserTenant> _userTenantRepository;

    public UpdateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITenantRepository tenantRepository,
        IRepository<UserTenant> userTenantRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tenantRepository = tenantRepository;
        _userTenantRepository = userTenantRepository;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException($"Usuario {request.UserId} no encontrado");
        }

        // Actualizar campos
        if (!string.IsNullOrEmpty(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException($"Error al actualizar usuario: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
        }

        // Actualizar rol si se especifica
        if (!string.IsNullOrEmpty(request.Role))
        {
            var role = await _roleManager.FindByNameAsync(request.Role);
            if (role == null)
            {
                throw new InvalidOperationException($"Rol {request.Role} no encontrado");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);
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

