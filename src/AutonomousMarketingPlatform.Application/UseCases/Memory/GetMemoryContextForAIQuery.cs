using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Application.UseCases.Memory;
using MediatR;

namespace AutonomousMarketingPlatform.Application.UseCases.Memory;

/// <summary>
/// Query para obtener contexto de memoria para generar contenido con IA.
/// Este es el query principal que se usa antes de generar contenido.
/// </summary>
public class GetMemoryContextForAIQuery : IRequest<MemoryContextForAI>
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CampaignId { get; set; }
    public List<string>? RelevantTags { get; set; }
}

/// <summary>
/// Handler para obtener contexto de memoria para IA.
/// </summary>
public class GetMemoryContextForAIQueryHandler : IRequestHandler<GetMemoryContextForAIQuery, MemoryContextForAI>
{
    private readonly IMediator _mediator;

    public GetMemoryContextForAIQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<MemoryContextForAI> Handle(GetMemoryContextForAIQuery request, CancellationToken cancellationToken)
    {
        var context = new MemoryContextForAI();

        // 1. Obtener preferencias del usuario (si hay userId)
        if (request.UserId.HasValue)
        {
            var preferencesQuery = new QueryMemoryQuery
            {
                TenantId = request.TenantId,
                UserId = request.UserId,
                MemoryTypes = new List<string> { "Preference" },
                MinRelevanceScore = 5,
                Limit = 20
            };
            context.UserPreferences = await _mediator.Send(preferencesQuery, cancellationToken);
        }

        // 2. Obtener conversaciones recientes (últimos 7 días)
        var conversationsQuery = new QueryMemoryQuery
        {
            TenantId = request.TenantId,
            UserId = request.UserId,
            MemoryTypes = new List<string> { "Conversation" },
            FromDate = DateTime.UtcNow.AddDays(-7),
            MinRelevanceScore = 4,
            Limit = 10
        };
        context.RecentConversations = await _mediator.Send(conversationsQuery, cancellationToken);

        // 3. Obtener memorias de campaña (si hay campaignId)
        if (request.CampaignId.HasValue)
        {
            var campaignQuery = new QueryMemoryQuery
            {
                TenantId = request.TenantId,
                CampaignId = request.CampaignId,
                MinRelevanceScore = 5,
                Limit = 15
            };
            context.CampaignMemories = await _mediator.Send(campaignQuery, cancellationToken);
        }

        // 4. Obtener aprendizajes relevantes
        var learningsQuery = new QueryMemoryQuery
        {
            TenantId = request.TenantId,
            MemoryTypes = new List<string> { "Learning", "Pattern" },
            Tags = request.RelevantTags,
            MinRelevanceScore = 6, // Solo aprendizajes muy relevantes
            Limit = 10
        };
        context.Learnings = await _mediator.Send(learningsQuery, cancellationToken);

        // 5. Generar resumen de contexto para IA
        context.SummarizedContext = BuildSummarizedContext(context);

        return context;
    }

    /// <summary>
    /// Construye un resumen del contexto para incluir en el prompt de IA.
    /// </summary>
    private string BuildSummarizedContext(MemoryContextForAI context)
    {
        var summary = new System.Text.StringBuilder();

        // Preferencias del usuario
        if (context.UserPreferences.Any())
        {
            summary.AppendLine("### Preferencias del Usuario:");
            foreach (var pref in context.UserPreferences.Take(5))
            {
                summary.AppendLine($"- {pref.Content}");
            }
        }

        // Conversaciones recientes relevantes
        if (context.RecentConversations.Any())
        {
            summary.AppendLine("\n### Contexto de Conversaciones Recientes:");
            foreach (var conv in context.RecentConversations.Take(3))
            {
                summary.AppendLine($"- {conv.Content}");
            }
        }

        // Aprendizajes aplicables
        if (context.Learnings.Any())
        {
            summary.AppendLine("\n### Aprendizajes del Sistema:");
            foreach (var learning in context.Learnings.Take(3))
            {
                summary.AppendLine($"- {learning.Content}");
            }
        }

        // Memorias de campaña
        if (context.CampaignMemories.Any())
        {
            summary.AppendLine("\n### Contexto de Campaña:");
            foreach (var mem in context.CampaignMemories.Take(3))
            {
                summary.AppendLine($"- {mem.Content}");
            }
        }

        return summary.ToString();
    }
}

