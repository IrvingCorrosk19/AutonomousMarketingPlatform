using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Application.UseCases.AI;

/// <summary>
/// Query para obtener un MarketingPack por ID.
/// </summary>
public class GetMarketingPackQuery : IRequest<MarketingPackDto?>
{
    public Guid TenantId { get; set; }
    public Guid MarketingPackId { get; set; }
}

/// <summary>
/// Handler para obtener MarketingPack.
/// </summary>
public class GetMarketingPackQueryHandler : IRequestHandler<GetMarketingPackQuery, MarketingPackDto?>
{
    private readonly IRepository<MarketingPack> _marketingPackRepository;
    private readonly ILogger<GetMarketingPackQueryHandler> _logger;

    public GetMarketingPackQueryHandler(
        IRepository<MarketingPack> marketingPackRepository,
        ILogger<GetMarketingPackQueryHandler> logger)
    {
        _marketingPackRepository = marketingPackRepository;
        _logger = logger;
    }

    public async Task<MarketingPackDto?> Handle(GetMarketingPackQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var marketingPack = await _marketingPackRepository.GetByIdAsync(
                request.MarketingPackId,
                request.TenantId,
                cancellationToken);

            if (marketingPack == null || !marketingPack.IsActive)
            {
                _logger.LogWarning(
                    "MarketingPack {MarketingPackId} no encontrado o inactivo para Tenant {TenantId}",
                    request.MarketingPackId,
                    request.TenantId);
                return null;
            }

            // Obtener copies y asset prompts relacionados
            var copies = marketingPack.Copies
                .Select(c => new GeneratedCopyDto
                {
                    Id = c.Id,
                    CopyType = c.CopyType,
                    Content = c.Content,
                    Hashtags = c.Hashtags,
                    SuggestedChannel = c.SuggestedChannel,
                    PublicationChecklist = !string.IsNullOrWhiteSpace(c.PublicationChecklist)
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(c.PublicationChecklist)
                        : null
                })
                .ToList();

            var assetPrompts = marketingPack.AssetPrompts
                .Select(p => new MarketingAssetPromptDto
                {
                    Id = p.Id,
                    AssetType = p.AssetType,
                    Prompt = p.Prompt,
                    NegativePrompt = p.NegativePrompt,
                    Parameters = !string.IsNullOrWhiteSpace(p.Parameters)
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(p.Parameters)
                        : null,
                    SuggestedChannel = p.SuggestedChannel
                })
                .ToList();

            return new MarketingPackDto
            {
                Id = marketingPack.Id,
                ContentId = marketingPack.ContentId,
                CampaignId = marketingPack.CampaignId,
                Strategy = marketingPack.Strategy,
                Status = marketingPack.Status,
                Version = marketingPack.Version,
                CreatedAt = marketingPack.CreatedAt,
                Copies = copies,
                AssetPrompts = assetPrompts
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al obtener MarketingPack {MarketingPackId} para Tenant {TenantId}",
                request.MarketingPackId,
                request.TenantId);
            throw;
        }
    }
}

