using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutonomousMarketingPlatform.Application.UseCases.Users;

/// <summary>
/// Comando para crear usuario.
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string Role { get; set; } = "Marketer";
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Handler para crear usuario.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRepository<UserTenant> _userTenantRepository;

    public CreateUserCommandHandler(
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

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Obtener tenant
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant == null)
        {
            throw new InvalidOperationException($"Tenant {request.TenantId} no encontrado");
        }

        // Verificar que el rol existe
        var role = await _roleManager.FindByNameAsync(request.Role);
        if (role == null)
        {
            throw new InvalidOperationException($"Rol {request.Role} no encontrado");
        }

        // Verificar si el usuario ya existe
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Usuario con email {request.Email} ya existe");
        }

        // Crear usuario
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            FullName = request.FullName,
            TenantId = request.TenantId,
            IsActive = request.IsActive
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Error al crear usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // Asignar rol
        await _userManager.AddToRoleAsync(user, request.Role);

        // Crear relación UserTenant
        var userTenant = new UserTenant
        {
            TenantId = request.TenantId,
            UserId = user.Id,
            RoleId = role.Id,
            IsPrimary = true,
            JoinedAt = DateTime.UtcNow
        };

        await _userTenantRepository.AddAsync(userTenant, cancellationToken);

        // Obtener roles del usuario
        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            TenantId = user.TenantId,
            TenantName = tenant.Name,
            IsActive = user.IsActive,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LockoutEndDate = user.LockoutEndDate,
            LastLoginAt = user.LastLoginAt,
            LastLoginIp = user.LastLoginIp,
            Roles = roles.ToList(),
            CreatedAt = DateTime.UtcNow
        };
    }
}

