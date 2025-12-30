using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Servicio para crear usuarios iniciales del sistema.
/// </summary>
public class UserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IDbContextFactory<Data.ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IDbContextFactory<Data.ApplicationDbContext> dbContextFactory,
        ILogger<UserSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Crea un usuario inicial para testing.
    /// </summary>
    public async Task<ApplicationUser?> CreateInitialUserAsync(
        string email,
        string password,
        string fullName,
        Guid tenantId,
        string roleName = "Owner")
    {
        try
        {
            // Verificar que el tenant existe
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            var tenant = await context.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogError("Tenant {TenantId} no encontrado", tenantId);
                return null;
            }

            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("Usuario con email {Email} ya existe", email);
                return existingUser;
            }

            // Crear usuario
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                TenantId = tenantId,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                _logger.LogError("Error al crear usuario: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return null;
            }

            // Asignar rol (si no lo tiene ya)
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (!userRoles.Contains(roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
                
                // Crear relación UserTenant solo si no existe
                var existingUserTenant = await context.UserTenants
                    .FirstOrDefaultAsync(ut => ut.UserId == user.Id && ut.TenantId == tenantId);
                
                if (existingUserTenant == null)
                {
                    var userTenant = new UserTenant
                    {
                        UserId = user.Id,
                        TenantId = tenantId,
                        RoleId = role.Id,
                        IsPrimary = true,
                        JoinedAt = DateTime.UtcNow
                    };

                    context.UserTenants.Add(userTenant);
                    await context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Usuario creado exitosamente: {Email}, Tenant: {TenantId}, Rol: {Role}", 
                email, tenantId, roleName);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario inicial");
            return null;
        }
    }

    /// <summary>
    /// Crea un tenant de prueba si no existe.
    /// </summary>
    public async Task<Tenant?> CreateTestTenantAsync(
        string name,
        string subdomain,
        string contactEmail)
    {
        try
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            
            var existingTenant = await context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain || t.Name == name);

            if (existingTenant != null)
            {
                _logger.LogInformation("Tenant {Name} ya existe con ID {TenantId}", name, existingTenant.Id);
                return existingTenant;
            }

            var tenant = new Tenant
            {
                Name = name,
                Subdomain = subdomain,
                ContactEmail = contactEmail,
                SubscriptionPlan = "Free",
                SubscriptionStartDate = DateTime.UtcNow,
                IsActive = true
            };

            context.Tenants.Add(tenant);
            await context.SaveChangesAsync();

            _logger.LogInformation("Tenant creado: {Name}, ID: {TenantId}", name, tenant.Id);
            return tenant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tenant de prueba");
            return null;
        }
    }
}

