using AutonomousMarketingPlatform.Application.DTOs;
using AutonomousMarketingPlatform.Domain.Entities;
using AutonomousMarketingPlatform.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutonomousMarketingPlatform.Application.UseCases.Memory;

/// <summary>
/// Comando para guardar una nueva memoria de marketing.
/// </summary>
public class SaveMemoryCommand : IRequest<MarketingMemoryDto>
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? CampaignId { get; set; }
    public string MemoryType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object>? Context { get; set; }
    public List<string>? Tags { get; set; }
    public int RelevanceScore { get; set; } = 5;
}

/// <summary>
/// Handler para guardar memoria.
/// </summary>
public class SaveMemoryCommandHandler : IRequestHandler<SaveMemoryCommand, MarketingMemoryDto>
{
    private readonly IRepository<MarketingMemory> _memoryRepository;
    private readonly ILogger<SaveMemoryCommandHandler> _logger;

    // Tipos de memoria permitidos
    private static readonly string[] AllowedMemoryTypes = 
    {
        "Conversation", "Decision", "Learning", "Feedback", "Pattern", "Preference"
    };

    public SaveMemoryCommandHandler(
        IRepository<MarketingMemory> memoryRepository,
        ILogger<SaveMemoryCommandHandler> logger)
    {
        _memoryRepository = memoryRepository;
        _logger = logger;
    }

    public async Task<MarketingMemoryDto> Handle(SaveMemoryCommand request, CancellationToken cancellationToken)
    {
        // Validar tipo de memoria
        if (!AllowedMemoryTypes.Contains(request.MemoryType))
        {
            throw new ArgumentException($"Tipo de memoria no permitido: {request.MemoryType}");
        }

        // Validar relevance score
        if (request.RelevanceScore < 1 || request.RelevanceScore > 10)
        {
            request.RelevanceScore = 5; // Default
        }

        // Validar y limpiar contenido (no guardar datos sensibles)
        var cleanedContent = CleanSensitiveData(request.Content);

        // Serializar contexto a JSON
        string? contextJson = null;
        if (request.Context != null && request.Context.Any())
        {
            var cleanedContext = CleanSensitiveDataFromContext(request.Context);
            contextJson = JsonSerializer.Serialize(cleanedContext);
        }

        // Crear memoria
        var memory = new MarketingMemory
        {
            TenantId = request.TenantId,
            CampaignId = request.CampaignId,
            MemoryType = request.MemoryType,
            Content = cleanedContent,
            ContextJson = contextJson,
            Tags = request.Tags != null ? string.Join(",", request.Tags) : null,
            RelevanceScore = request.RelevanceScore,
            MemoryDate = DateTime.UtcNow
        };

        await _memoryRepository.AddAsync(memory, cancellationToken);

        _logger.LogInformation(
            "Memoria guardada: Type={MemoryType}, TenantId={TenantId}, Relevance={RelevanceScore}",
            request.MemoryType, request.TenantId, request.RelevanceScore);

        return new MarketingMemoryDto
        {
            Id = memory.Id,
            CampaignId = memory.CampaignId,
            MemoryType = memory.MemoryType,
            Content = memory.Content,
            Context = request.Context,
            Tags = request.Tags ?? new List<string>(),
            RelevanceScore = memory.RelevanceScore,
            MemoryDate = memory.MemoryDate,
            CreatedAt = memory.CreatedAt
        };
    }

    /// <summary>
    /// Limpia datos sensibles del contenido.
    /// </summary>
    private string CleanSensitiveData(string content)
    {
        // Patrones de datos sensibles a detectar y eliminar
        var sensitivePatterns = new[]
        {
            @"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", // Tarjetas de crédito
            @"\b\d{3}-\d{2}-\d{4}\b", // SSN
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b" // Emails (opcional, puede ser necesario)
        };

        var cleaned = content;
        foreach (var pattern in sensitivePatterns)
        {
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, pattern, "[REDACTED]");
        }

        return cleaned;
    }

    /// <summary>
    /// Limpia datos sensibles del contexto.
    /// </summary>
    private Dictionary<string, object> CleanSensitiveDataFromContext(Dictionary<string, object> context)
    {
        var cleaned = new Dictionary<string, object>();
        var sensitiveKeys = new[] { "password", "creditCard", "ssn", "bankAccount" };

        foreach (var kvp in context)
        {
            if (!sensitiveKeys.Contains(kvp.Key.ToLower()))
            {
                cleaned[kvp.Key] = kvp.Value;
            }
        }

        return cleaned;
    }
}

