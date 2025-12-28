using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AutonomousMarketingPlatform.Infrastructure.Data;

/// <summary>
/// Factory para crear ApplicationDbContext en tiempo de diseño (migraciones).
/// Resuelve el problema de dependencia circular con TenantService.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Obtener el directorio del proyecto Web donde está appsettings.json
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AutonomousMarketingPlatform.Web");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        optionsBuilder.UseNpgsql(connectionString);

        // Crear un TenantService mock para tiempo de diseño
        var mockTenantService = new DesignTimeTenantService();

        return new ApplicationDbContext(optionsBuilder.Options, mockTenantService);
    }
}

/// <summary>
/// Implementación mock de ITenantService para tiempo de diseño.
/// </summary>
internal class DesignTimeTenantService : Domain.Interfaces.ITenantService
{
    public Guid? GetCurrentTenantId() => null;

    public Task<bool> ValidateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<Guid?> GetTenantIdBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
        => Task.FromResult<Guid?>(null);
}

