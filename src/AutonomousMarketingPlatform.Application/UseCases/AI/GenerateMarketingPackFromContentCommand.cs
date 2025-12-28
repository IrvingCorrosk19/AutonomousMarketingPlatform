using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Application.UseCases.Memory;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using ContentEntity = AutonomousMarketingPlatform.Domain.Entities.Content;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Application.UseCases.AI;

/// <summary>
/// Comando para generar un MarketingPack completo a partir de contenido cargado.
/// </summary>
public class GenerateMarketingPackFromContentCommand : IRequest<MarketingPackDto>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid ContentId { get; set; }
    public Guid? CampaignId { get; set; }
}

/// <summary>
/// Handler para generar MarketingPack.
/// </summary>
public class GenerateMarketingPackFromContentCommandHandler : IRequestHandler<GenerateMarketingPackFromContentCommand, MarketingPackDto>
{
    private readonly IRepository<ContentEntity> _contentRepository;
    private readonly IRepository<MarketingPack> _marketingPackRepository;
    private readonly IRepository<GeneratedCopy> _copyRepository;
    private readonly IRepository<MarketingAssetPrompt> _promptRepository;
    private readonly IRepository<CampaignDraft> _draftRepository;
    private readonly IConsentValidationService _consentService;
    private readonly ISecurityService _securityService;
    private readonly IMarketingMemoryService _memoryService;
    private readonly IAIProvider _aiProvider;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<GenerateMarketingPackFromContentCommandHandler> _logger;

    public GenerateMarketingPackFromContentCommandHandler(
        IRepository<ContentEntity> contentRepository,
        IRepository<MarketingPack> marketingPackRepository,
        IRepository<GeneratedCopy> copyRepository,
        IRepository<MarketingAssetPrompt> promptRepository,
        IRepository<CampaignDraft> draftRepository,
        IConsentValidationService consentService,
        ISecurityService securityService,
        IMarketingMemoryService memoryService,
        IAIProvider aiProvider,
        IAuditService auditService,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<GenerateMarketingPackFromContentCommandHandler> logger)
    {
        _contentRepository = contentRepository;
        _marketingPackRepository = marketingPackRepository;
        _copyRepository = copyRepository;
        _promptRepository = promptRepository;
        _draftRepository = draftRepository;
        _consentService = consentService;
        _securityService = securityService;
        _memoryService = memoryService;
        _aiProvider = aiProvider;
        _auditService = auditService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<MarketingPackDto> Handle(GenerateMarketingPackFromContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar tenant_id y user_id
            var userBelongsToTenant = await _securityService.ValidateUserBelongsToTenantAsync(
                request.UserId, request.TenantId, cancellationToken);
            
            if (!userBelongsToTenant)
            {
                _logger.LogWarning("Usuario {UserId} no pertenece al tenant {TenantId}", 
                    request.UserId, request.TenantId);
                throw new UnauthorizedAccessException("Usuario no pertenece a este tenant");
            }

            // 2. Validar consentimiento vigente
            var hasConsent = await _consentService.ValidateConsentAsync(
                request.UserId, request.TenantId, "AIGeneration", cancellationToken);
            
            if (!hasConsent)
            {
                _logger.LogWarning("Usuario {UserId} no tiene consentimiento para generación IA", request.UserId);
                throw new InvalidOperationException("Consentimiento para generación IA requerido");
            }

            // 3. Validar que el contenido existe y pertenece al tenant
            var content = await _contentRepository.GetByIdAsync(request.ContentId, request.TenantId, cancellationToken);
            if (content == null)
            {
                _logger.LogWarning("Contenido {ContentId} no encontrado o no pertenece al tenant {TenantId}", 
                    request.ContentId, request.TenantId);
                throw new NotFoundException($"Contenido {request.ContentId} no encontrado");
            }

            // 4. Obtener memoria de marketing del usuario/tenant
            var memoryContextQuery = new GetMemoryContextForAIQuery
            {
                TenantId = request.TenantId,
                UserId = request.UserId,
                CampaignId = request.CampaignId,
                RelevantTags = content.Tags?.Split(',').Select(t => t.Trim()).ToList()
            };
            var memoryContext = await _mediator.Send(memoryContextQuery, cancellationToken);

            // 5. Preparar descripción del contenido
            var contentDescription = $"{content.Description ?? "Contenido sin descripción"} " +
                                   $"Tipo: {content.ContentType} " +
                                   $"Tags: {content.Tags ?? "N/A"}";

            // Preparar contexto de memoria para IA (sin datos sensibles)
            var userContext = memoryContext.SummarizedContext;
            var campaignContext = request.CampaignId.HasValue 
                ? string.Join("\n", memoryContext.CampaignMemories.Take(3).Select(m => m.Content))
                : null;

            // 6. Generar estrategia
            _logger.LogInformation("Generando estrategia para contenido {ContentId}", request.ContentId);
            var strategy = await _aiProvider.GenerateStrategyAsync(
                contentDescription,
                userContext,
                campaignContext,
                cancellationToken);

            // 7. Generar copies
            _logger.LogInformation("Generando copies para contenido {ContentId}", request.ContentId);
            var copiesResult = await _aiProvider.GenerateCopiesAsync(
                strategy,
                contentDescription,
                userContext,
                cancellationToken);

            // 8. Generar hashtags para cada copy
            var shortHashtags = await _aiProvider.GenerateHashtagsAsync(
                copiesResult.ShortCopy, userContext, cancellationToken);
            var mediumHashtags = await _aiProvider.GenerateHashtagsAsync(
                copiesResult.MediumCopy, userContext, cancellationToken);
            var longHashtags = await _aiProvider.GenerateHashtagsAsync(
                copiesResult.LongCopy, userContext, cancellationToken);

            // 9. Generar prompts para assets
            var imagePrompt = await _aiProvider.GenerateImagePromptAsync(
                strategy, contentDescription, userContext, cancellationToken);
            var videoPrompt = await _aiProvider.GenerateVideoPromptAsync(
                strategy, contentDescription, userContext, cancellationToken);

            // 10. Generar checklists de publicación
            var instagramChecklist = await _aiProvider.GeneratePublicationChecklistAsync(
                "Instagram", copiesResult.MediumCopy, "Image", cancellationToken);
            var facebookChecklist = await _aiProvider.GeneratePublicationChecklistAsync(
                "Facebook", copiesResult.MediumCopy, "Image", cancellationToken);
            var tiktokChecklist = await _aiProvider.GeneratePublicationChecklistAsync(
                "TikTok", copiesResult.LongCopy, "Video", cancellationToken);

            // 11. Persistir MarketingPack
            var marketingPack = new MarketingPack
            {
                TenantId = request.TenantId,
                UserId = request.UserId,
                ContentId = request.ContentId,
                CampaignId = request.CampaignId,
                Strategy = strategy,
                Status = "Generated",
                Version = 1
            };

            await _marketingPackRepository.AddAsync(marketingPack, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 12. Persistir Copies
            var copies = new List<GeneratedCopy>
            {
                new GeneratedCopy
                {
                    TenantId = request.TenantId,
                    MarketingPackId = marketingPack.Id,
                    CopyType = "Short",
                    Content = copiesResult.ShortCopy,
                    Hashtags = string.Join(" ", shortHashtags),
                    SuggestedChannel = "Instagram",
                    PublicationChecklist = JsonSerializer.Serialize(instagramChecklist)
                },
                new GeneratedCopy
                {
                    TenantId = request.TenantId,
                    MarketingPackId = marketingPack.Id,
                    CopyType = "Medium",
                    Content = copiesResult.MediumCopy,
                    Hashtags = string.Join(" ", mediumHashtags),
                    SuggestedChannel = "Facebook",
                    PublicationChecklist = JsonSerializer.Serialize(facebookChecklist)
                },
                new GeneratedCopy
                {
                    TenantId = request.TenantId,
                    MarketingPackId = marketingPack.Id,
                    CopyType = "Long",
                    Content = copiesResult.LongCopy,
                    Hashtags = string.Join(" ", longHashtags),
                    SuggestedChannel = "TikTok",
                    PublicationChecklist = JsonSerializer.Serialize(tiktokChecklist)
                }
            };

            foreach (var copy in copies)
            {
                await _copyRepository.AddAsync(copy, cancellationToken);
            }

            // 13. Persistir Asset Prompts
            var assetPrompts = new List<MarketingAssetPrompt>
            {
                new MarketingAssetPrompt
                {
                    TenantId = request.TenantId,
                    MarketingPackId = marketingPack.Id,
                    AssetType = "Image",
                    Prompt = imagePrompt,
                    SuggestedChannel = "Instagram"
                },
                new MarketingAssetPrompt
                {
                    TenantId = request.TenantId,
                    MarketingPackId = marketingPack.Id,
                    AssetType = "Video",
                    Prompt = videoPrompt,
                    SuggestedChannel = "TikTok"
                }
            };

            foreach (var prompt in assetPrompts)
            {
                await _promptRepository.AddAsync(prompt, cancellationToken);
            }

            // 14. Crear CampaignDraft opcional
            var campaignDraft = new CampaignDraft
            {
                TenantId = request.TenantId,
                UserId = request.UserId,
                MarketingPackId = marketingPack.Id,
                Name = $"Campaña generada - {content.Description ?? "Sin nombre"}",
                Description = $"Campaña generada automáticamente a partir del contenido {request.ContentId}",
                Status = "Draft",
                IsConverted = false,
                SuggestedChannels = JsonSerializer.Serialize(new[] { "Instagram", "Facebook", "TikTok" })
            };

            await _draftRepository.AddAsync(campaignDraft, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 15. Auditoría
            await _auditService.LogAsync(
                request.TenantId,
                "GenerateMarketingPack",
                "MarketingPack",
                marketingPack.Id,
                null,
                null,
                null,
                null,
                null,
                "Success",
                null,
                null,
                null,
                cancellationToken);

            _logger.LogInformation("MarketingPack generado exitosamente: {MarketingPackId}", marketingPack.Id);

            // 16. Mapear a DTO
            return new MarketingPackDto
            {
                Id = marketingPack.Id,
                ContentId = marketingPack.ContentId,
                CampaignId = marketingPack.CampaignId,
                Strategy = marketingPack.Strategy,
                Status = marketingPack.Status,
                Version = marketingPack.Version,
                CreatedAt = marketingPack.CreatedAt,
                Copies = copies.Select(c => new GeneratedCopyDto
                {
                    Id = c.Id,
                    CopyType = c.CopyType,
                    Content = c.Content,
                    Hashtags = c.Hashtags,
                    SuggestedChannel = c.SuggestedChannel,
                    PublicationChecklist = c.PublicationChecklist != null 
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(c.PublicationChecklist)
                        : null
                }).ToList(),
                AssetPrompts = assetPrompts.Select(p => new MarketingAssetPromptDto
                {
                    Id = p.Id,
                    AssetType = p.AssetType,
                    Prompt = p.Prompt,
                    NegativePrompt = p.NegativePrompt,
                    Parameters = p.Parameters != null
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(p.Parameters)
                        : null,
                    SuggestedChannel = p.SuggestedChannel
                }).ToList(),
                CampaignDraft = new CampaignDraftDto
                {
                    Id = campaignDraft.Id,
                    Name = campaignDraft.Name,
                    Description = campaignDraft.Description,
                    Objectives = campaignDraft.Objectives != null
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(campaignDraft.Objectives)
                        : null,
                    TargetAudience = campaignDraft.TargetAudience != null
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(campaignDraft.TargetAudience)
                        : null,
                    SuggestedChannels = campaignDraft.SuggestedChannels != null
                        ? JsonSerializer.Deserialize<List<string>>(campaignDraft.SuggestedChannels)
                        : null,
                    Status = campaignDraft.Status,
                    IsConverted = campaignDraft.IsConverted,
                    ConvertedCampaignId = campaignDraft.ConvertedCampaignId
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar MarketingPack para contenido {ContentId}", request.ContentId);
            
            await _auditService.LogAsync(
                request.TenantId,
                "GenerateMarketingPack",
                "MarketingPack",
                null,
                null,
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

/// <summary>
/// Excepción personalizada para recursos no encontrados.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

