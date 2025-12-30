using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Application.UseCases.AI;

/// <summary>
/// Comando para configurar la API key de IA del tenant.
/// </summary>
public class ConfigureTenantAICommand : IRequest<TenantAIConfigDto>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4";
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Handler para configurar IA del tenant.
/// </summary>
public class ConfigureTenantAICommandHandler : IRequestHandler<ConfigureTenantAICommand, TenantAIConfigDto>
{
    private readonly IRepository<TenantAIConfig> _configRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ISecurityService _securityService;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ConfigureTenantAICommandHandler> _logger;

    public ConfigureTenantAICommandHandler(
        IRepository<TenantAIConfig> configRepository,
        IEncryptionService encryptionService,
        ISecurityService securityService,
        IAuditService auditService,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogger<ConfigureTenantAICommandHandler> logger)
    {
        _configRepository = configRepository;
        _encryptionService = encryptionService;
        _securityService = securityService;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<TenantAIConfigDto> Handle(ConfigureTenantAICommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el usuario pertenece al tenant
            var userBelongsToTenant = await _securityService.ValidateUserBelongsToTenantAsync(
                request.UserId, request.TenantId, cancellationToken);
            
            if (!userBelongsToTenant)
            {
                throw new UnauthorizedAccessException("Usuario no pertenece a este tenant");
            }

            // Validar que el usuario tiene permisos (Owner o Admin)
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var hasPermission = userRoles.Contains("Owner") || userRoles.Contains("Admin");
            
            if (!hasPermission)
            {
                _logger.LogWarning(
                    "Usuario {UserId} intentó configurar IA sin permisos. Roles: {Roles}",
                    request.UserId,
                    string.Join(", ", userRoles));
                throw new UnauthorizedAccessException("Solo usuarios con rol Owner o Admin pueden configurar IA");
            }

            // Buscar configuración existente
            var existingConfigs = await _configRepository.FindAsync(
                c => c.TenantId == request.TenantId && c.Provider == request.Provider,
                request.TenantId,
                cancellationToken);

            var existingConfig = existingConfigs.FirstOrDefault();

            TenantAIConfig config;

            if (existingConfig != null)
            {
                // Actualizar configuración existente
                existingConfig.EncryptedApiKey = _encryptionService.Encrypt(request.ApiKey);
                existingConfig.Model = request.Model;
                existingConfig.IsActive = request.IsActive;
                existingConfig.UpdatedAt = DateTime.UtcNow;
                
                await _configRepository.UpdateAsync(existingConfig, cancellationToken);
                config = existingConfig;
                
                _logger.LogInformation("Configuración de IA actualizada: TenantId={TenantId}, Provider={Provider}", 
                    request.TenantId, request.Provider);
            }
            else
            {
                // Crear nueva configuración
                config = new TenantAIConfig
                {
                    TenantId = request.TenantId,
                    Provider = request.Provider,
                    EncryptedApiKey = _encryptionService.Encrypt(request.ApiKey),
                    Model = request.Model,
                    IsActive = request.IsActive
                };

                await _configRepository.AddAsync(config, cancellationToken);
                
                _logger.LogInformation("Nueva configuración de IA creada: TenantId={TenantId}, Provider={Provider}", 
                    request.TenantId, request.Provider);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Auditoría
            await _auditService.LogAsync(
                request.TenantId,
                "ConfigureAI",
                "TenantAIConfig",
                config.Id,
                request.UserId,
                null,
                null,
                null,
                null,
                "Success",
                null,
                null,
                null,
                cancellationToken);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al configurar IA para tenant {TenantId}", request.TenantId);
            
            await _auditService.LogAsync(
                request.TenantId,
                "ConfigureAI",
                "TenantAIConfig",
                null,
                request.UserId,
                null,
                null,
                null,
                null,
                "Failed",
                ex.Message,
                null,
                null,
                cancellationToken);

            throw;
        }
    }
}

