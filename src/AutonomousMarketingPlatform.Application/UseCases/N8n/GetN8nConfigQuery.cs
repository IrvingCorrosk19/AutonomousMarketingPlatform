using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Application.UseCases.N8n;

/// <summary>
/// Query para obtener la configuración actual de n8n.
/// </summary>
public class GetN8nConfigQuery : IRequest<N8nConfigDto>
{
    public Guid TenantId { get; set; }
    /// <summary>
    /// Si es true, permite buscar configuraciones de cualquier tenant (para SuperAdmins).
    /// </summary>
    public bool IsSuperAdmin { get; set; } = false;
}

/// <summary>
/// Handler para obtener la configuración de n8n desde la base de datos.
/// </summary>
public class GetN8nConfigQueryHandler : IRequestHandler<GetN8nConfigQuery, N8nConfigDto>
{
    private readonly IRepository<Domain.Entities.TenantN8nConfig> _configRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetN8nConfigQueryHandler> _logger;

    public GetN8nConfigQueryHandler(
        IRepository<Domain.Entities.TenantN8nConfig> configRepository,
        IConfiguration configuration,
        ILogger<GetN8nConfigQueryHandler> logger)
    {
        _configRepository = configRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<N8nConfigDto> Handle(GetN8nConfigQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Intentar obtener configuración de la base de datos
            // Si es SuperAdmin, puede buscar cualquier tenant (usar Guid.Empty como filtro de tenant en el repositorio)
            var searchTenantId = request.IsSuperAdmin ? Guid.Empty : request.TenantId;
            
            var configs = await _configRepository.FindAsync(
                c => c.TenantId == request.TenantId && c.IsActive,
                searchTenantId, // Usar Guid.Empty para SuperAdmin para evitar filtrado por tenant
                cancellationToken);

            var dbConfig = configs.FirstOrDefault();

            if (dbConfig != null)
            {
                // Deserializar webhook URLs desde JSON
                Dictionary<string, string> webhookUrls = new();
                if (!string.IsNullOrWhiteSpace(dbConfig.WebhookUrlsJson))
                {
                    try
                    {
                        webhookUrls = JsonSerializer.Deserialize<Dictionary<string, string>>(dbConfig.WebhookUrlsJson) 
                            ?? new Dictionary<string, string>();
                        
                        // Log para debugging
                        _logger.LogInformation(
                            "Configuración cargada desde BD para Tenant {TenantId}. MarketingRequest URL: {Url}",
                            request.TenantId,
                            webhookUrls.GetValueOrDefault("MarketingRequest") ?? "NO CONFIGURADA");
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Error al deserializar WebhookUrlsJson para Tenant {TenantId}", request.TenantId);
                    }
                }

                return new N8nConfigDto
                {
                    UseMock = dbConfig.UseMock,
                    BaseUrl = dbConfig.BaseUrl,
                    ApiUrl = dbConfig.ApiUrl,
                    ApiKey = null, // No devolver la API key por seguridad (está encriptada)
                    DefaultWebhookUrl = dbConfig.DefaultWebhookUrl,
                    WebhookUrls = webhookUrls
                };
            }

            // Si no hay configuración en BD, usar valores por defecto de appsettings.json
            _logger.LogInformation("No se encontró configuración de n8n en BD para Tenant {TenantId}, usando valores por defecto", request.TenantId);
            
            return new N8nConfigDto
            {
                UseMock = bool.TryParse(_configuration["N8n:UseMock"], out var useMock) ? useMock : true,
                BaseUrl = _configuration["N8n:BaseUrl"] ?? "http://localhost:5678",
                ApiUrl = _configuration["N8n:ApiUrl"] ?? "http://localhost:5678/api/v1",
                ApiKey = null,
                DefaultWebhookUrl = _configuration["N8n:DefaultWebhookUrl"] ?? "http://localhost:5678/webhook",
                WebhookUrls = new Dictionary<string, string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración de n8n para Tenant {TenantId}", request.TenantId);
            throw;
        }
    }
}

