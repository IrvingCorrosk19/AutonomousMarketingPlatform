using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Application.UseCases.N8n;

/// <summary>
/// Command para actualizar la configuración de n8n en la base de datos.
/// </summary>
public class UpdateN8nConfigCommand : IRequest<bool>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public UpdateN8nConfigDto Config { get; set; } = null!;
    /// <summary>
    /// Si es true, permite actualizar configuraciones de cualquier tenant (para SuperAdmins).
    /// </summary>
    public bool IsSuperAdmin { get; set; } = false;
}

/// <summary>
/// Handler para actualizar la configuración de n8n en la base de datos.
/// </summary>
public class UpdateN8nConfigCommandHandler : IRequestHandler<UpdateN8nConfigCommand, bool>
{
    private readonly IRepository<TenantN8nConfig> _configRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateN8nConfigCommandHandler> _logger;

    public UpdateN8nConfigCommandHandler(
        IRepository<TenantN8nConfig> configRepository,
        IEncryptionService encryptionService,
        IAuditService auditService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateN8nConfigCommandHandler> logger)
    {
        _configRepository = configRepository;
        _encryptionService = encryptionService;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateN8nConfigCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Actualizando configuración de n8n para Tenant {TenantId}",
                request.TenantId);

            // Buscar configuración existente
            // Si es SuperAdmin, puede buscar cualquier tenant (usar Guid.Empty como filtro de tenant en el repositorio)
            var searchTenantId = request.IsSuperAdmin ? Guid.Empty : request.TenantId;
            
            var existingConfigs = await _configRepository.FindAsync(
                c => c.TenantId == request.TenantId,
                searchTenantId, // Usar Guid.Empty para SuperAdmin para evitar filtrado por tenant
                cancellationToken);

            var existingConfig = existingConfigs.FirstOrDefault();

            // Serializar webhook URLs a JSON
            var webhookUrlsJson = JsonSerializer.Serialize(request.Config.WebhookUrls);

            TenantN8nConfig config;

            if (existingConfig != null)
            {
                // Actualizar configuración existente
                existingConfig.UseMock = request.Config.UseMock;
                existingConfig.BaseUrl = request.Config.BaseUrl;
                existingConfig.ApiUrl = request.Config.ApiUrl;
                existingConfig.DefaultWebhookUrl = request.Config.DefaultWebhookUrl;
                existingConfig.WebhookUrlsJson = webhookUrlsJson;
                existingConfig.UpdatedAt = DateTime.UtcNow;
                existingConfig.IsActive = true;

                // Actualizar API Key solo si se proporcionó una nueva
                if (!string.IsNullOrWhiteSpace(request.Config.ApiKey))
                {
                    existingConfig.EncryptedApiKey = _encryptionService.Encrypt(request.Config.ApiKey);
                }

                await _configRepository.UpdateAsync(existingConfig, cancellationToken);
                config = existingConfig;

                _logger.LogInformation(
                    "Configuración de n8n actualizada: TenantId={TenantId}",
                    request.TenantId);
            }
            else
            {
                // Crear nueva configuración
                config = new TenantN8nConfig
                {
                    TenantId = request.TenantId,
                    UseMock = request.Config.UseMock,
                    BaseUrl = request.Config.BaseUrl,
                    ApiUrl = request.Config.ApiUrl,
                    DefaultWebhookUrl = request.Config.DefaultWebhookUrl,
                    WebhookUrlsJson = webhookUrlsJson,
                    IsActive = true
                };

                // Encriptar API Key si se proporcionó
                if (!string.IsNullOrWhiteSpace(request.Config.ApiKey))
                {
                    config.EncryptedApiKey = _encryptionService.Encrypt(request.Config.ApiKey);
                }

                await _configRepository.AddAsync(config, cancellationToken);

                _logger.LogInformation(
                    "Nueva configuración de n8n creada: TenantId={TenantId}",
                    request.TenantId);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Auditoría
            await _auditService.LogAsync(
                tenantId: request.TenantId,
                action: "N8nConfigUpdated",
                entityType: "TenantN8nConfig",
                entityId: config.Id,
                userId: request.UserId,
                newValues: $"UseMock={config.UseMock}, BaseUrl={config.BaseUrl}",
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al actualizar configuración de n8n para Tenant {TenantId}",
                request.TenantId);
            throw;
        }
    }
}

