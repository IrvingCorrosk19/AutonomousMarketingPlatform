using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Application.UseCases.Memory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutonomousMarketingPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de memoria de marketing.
/// </summary>
public class MarketingMemoryService : IMarketingMemoryService
{
    private readonly IMediator _mediator;
    private readonly ILogger<MarketingMemoryService> _logger;

    public MarketingMemoryService(IMediator mediator, ILogger<MarketingMemoryService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Guid> SaveConversationMemoryAsync(
        Guid tenantId,
        Guid? userId,
        string conversation,
        Dictionary<string, object>? context = null,
        CancellationToken cancellationToken = default)
    {
        var command = new SaveMemoryCommand
        {
            TenantId = tenantId,
            UserId = userId,
            MemoryType = "Conversation",
            Content = conversation,
            Context = context,
            RelevanceScore = 5,
            Tags = new List<string> { "conversation", "user-interaction" }
        };

        var result = await _mediator.Send(command, cancellationToken);
        return result.Id;
    }

    public async Task<Guid> SaveDecisionMemoryAsync(
        Guid tenantId,
        Guid? userId,
        Guid? campaignId,
        string decision,
        Dictionary<string, object>? context = null,
        CancellationToken cancellationToken = default)
    {
        var command = new SaveMemoryCommand
        {
            TenantId = tenantId,
            UserId = userId,
            CampaignId = campaignId,
            MemoryType = "Decision",
            Content = decision,
            Context = context,
            RelevanceScore = 7, // Decisiones son más relevantes
            Tags = new List<string> { "decision", "user-choice" }
        };

        var result = await _mediator.Send(command, cancellationToken);
        return result.Id;
    }

    public async Task<Guid> SaveLearningMemoryAsync(
        Guid tenantId,
        string learning,
        List<string>? tags = null,
        int relevanceScore = 7,
        CancellationToken cancellationToken = default)
    {
        var allTags = new List<string> { "learning", "system-improvement" };
        if (tags != null)
        {
            allTags.AddRange(tags);
        }

        var command = new SaveMemoryCommand
        {
            TenantId = tenantId,
            MemoryType = "Learning",
            Content = learning,
            RelevanceScore = relevanceScore,
            Tags = allTags
        };

        var result = await _mediator.Send(command, cancellationToken);
        return result.Id;
    }

    public async Task<Guid> SaveFeedbackMemoryAsync(
        Guid tenantId,
        Guid userId,
        Guid? contentId,
        string feedback,
        bool isPositive,
        CancellationToken cancellationToken = default)
    {
        var context = new Dictionary<string, object>
        {
            { "isPositive", isPositive },
            { "contentId", contentId?.ToString() ?? "" }
        };

        var command = new SaveMemoryCommand
        {
            TenantId = tenantId,
            UserId = userId,
            MemoryType = "Feedback",
            Content = feedback,
            Context = context,
            RelevanceScore = isPositive ? 8 : 6, // Feedback positivo es más relevante
            Tags = new List<string> { "feedback", isPositive ? "positive" : "negative" }
        };

        var result = await _mediator.Send(command, cancellationToken);
        return result.Id;
    }

    public async Task<MemoryContextForAI> GetMemoryContextForAIAsync(
        Guid tenantId,
        Guid? userId = null,
        Guid? campaignId = null,
        List<string>? relevantTags = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMemoryContextForAIQuery
        {
            TenantId = tenantId,
            UserId = userId,
            CampaignId = campaignId,
            RelevantTags = relevantTags
        };

        return await _mediator.Send(query, cancellationToken);
    }
}

