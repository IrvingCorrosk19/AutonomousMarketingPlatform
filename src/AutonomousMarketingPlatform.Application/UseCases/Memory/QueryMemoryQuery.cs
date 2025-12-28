using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Application.UseCases.Memory;

/// <summary>
/// Query para consultar memorias de marketing relevantes.
/// </summary>
public class QueryMemoryQuery : IRequest<List<MarketingMemoryDto>>
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CampaignId { get; set; }
    public List<string>? MemoryTypes { get; set; }
    public List<string>? Tags { get; set; }
    public int? MinRelevanceScore { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Limit { get; set; } = 10;
}

/// <summary>
/// Handler para consultar memoria.
/// </summary>
public class QueryMemoryQueryHandler : IRequestHandler<QueryMemoryQuery, List<MarketingMemoryDto>>
{
    private readonly IRepository<MarketingMemory> _memoryRepository;

    public QueryMemoryQueryHandler(IRepository<MarketingMemory> memoryRepository)
    {
        _memoryRepository = memoryRepository;
    }

    public async Task<List<MarketingMemoryDto>> Handle(QueryMemoryQuery request, CancellationToken cancellationToken)
    {
        // Obtener todas las memorias del tenant
        var allMemories = await _memoryRepository.GetAllAsync(request.TenantId, cancellationToken);
        var memories = allMemories.ToList();

        // Aplicar filtros
        if (request.CampaignId.HasValue)
        {
            memories = memories.Where(m => m.CampaignId == request.CampaignId).ToList();
        }

        if (request.MemoryTypes != null && request.MemoryTypes.Any())
        {
            memories = memories.Where(m => request.MemoryTypes.Contains(m.MemoryType)).ToList();
        }

        if (request.Tags != null && request.Tags.Any())
        {
            memories = memories.Where(m => 
                m.Tags != null && 
                request.Tags.Any(tag => m.Tags.Contains(tag, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        if (request.MinRelevanceScore.HasValue)
        {
            memories = memories.Where(m => m.RelevanceScore >= request.MinRelevanceScore.Value).ToList();
        }

        if (request.FromDate.HasValue)
        {
            memories = memories.Where(m => m.MemoryDate >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            memories = memories.Where(m => m.MemoryDate <= request.ToDate.Value).ToList();
        }

        // Ordenar por relevancia y fecha (más relevantes y recientes primero)
        var sortedMemories = memories
            .OrderByDescending(m => m.RelevanceScore)
            .ThenByDescending(m => m.MemoryDate)
            .Take(request.Limit)
            .ToList();

        // Convertir a DTOs
        return sortedMemories.Select(m => new MarketingMemoryDto
        {
            Id = m.Id,
            CampaignId = m.CampaignId,
            MemoryType = m.MemoryType,
            MemoryTypeDisplayName = GetMemoryTypeDisplayName(m.MemoryType),
            Content = m.Content,
            Context = m.ContextJson != null 
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(m.ContextJson) 
                : null,
            Tags = m.Tags != null ? m.Tags.Split(',').ToList() : new List<string>(),
            RelevanceScore = m.RelevanceScore,
            MemoryDate = m.MemoryDate,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    private string GetMemoryTypeDisplayName(string memoryType)
    {
        return memoryType switch
        {
            "Conversation" => "Conversación",
            "Decision" => "Decisión",
            "Learning" => "Aprendizaje",
            "Feedback" => "Feedback",
            "Pattern" => "Patrón",
            "Preference" => "Preferencia",
            _ => memoryType
        };
    }
}

