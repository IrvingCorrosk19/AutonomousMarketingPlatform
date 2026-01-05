using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<CreateTenantCommandHandler> _logger;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepository,
        ILogger<CreateTenantCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("=== [CreateTenantCommandHandler.Handle] INICIADO ===");
        _logger.LogInformation("[CreateTenantCommandHandler] Name: {Name}", request.Name);
        _logger.LogInformation("[CreateTenantCommandHandler] Subdomain: {Subdomain}", request.Subdomain ?? "NULL");
        _logger.LogInformation("[CreateTenantCommandHandler] ContactEmail: {ContactEmail}", request.ContactEmail ?? "NULL");

        // Verificar si ya existe un tenant con el mismo subdomain (activo o inactivo)
        if (!string.IsNullOrEmpty(request.Subdomain))
        {
            _logger.LogInformation("[CreateTenantCommandHandler] Verificando subdomain único: {Subdomain}", request.Subdomain);
            var existingBySubdomain = await _tenantRepository.GetBySubdomainAnyAsync(request.Subdomain, cancellationToken);
            if (existingBySubdomain != null)
            {
                _logger.LogWarning("[CreateTenantCommandHandler] Ya existe un tenant con subdomain: {Subdomain}", request.Subdomain);
                throw new InvalidOperationException($"Ya existe un tenant con subdomain {request.Subdomain}");
            }
            _logger.LogInformation("[CreateTenantCommandHandler] Subdomain disponible: {Subdomain}", request.Subdomain);
        }
        else
        {
            _logger.LogInformation("[CreateTenantCommandHandler] Subdomain vacío, omitiendo validación");
        }

        _logger.LogInformation("[CreateTenantCommandHandler] Creando entidad Tenant...");
        var tenant = new Tenant
        {
            Name = request.Name,
            Subdomain = request.Subdomain ?? string.Empty,
            ContactEmail = request.ContactEmail ?? string.Empty,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("[CreateTenantCommandHandler] Tenant creado - Id: {Id}, Name: {Name}, Subdomain: {Subdomain}",
            tenant.Id, tenant.Name, tenant.Subdomain);

        _logger.LogInformation("[CreateTenantCommandHandler] Guardando tenant en repositorio...");
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        _logger.LogInformation("[CreateTenantCommandHandler] Tenant guardado exitosamente - Id: {Id}", tenant.Id);

        var result = new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Subdomain = tenant.Subdomain,
            ContactEmail = tenant.ContactEmail,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            UserCount = 0
        };

        _logger.LogInformation("[CreateTenantCommandHandler] DTO creado - Id: {Id}, Name: {Name}", result.Id, result.Name);
        _logger.LogInformation("=== [CreateTenantCommandHandler.Handle] COMPLETADO ===");

        return result;
    }
}

