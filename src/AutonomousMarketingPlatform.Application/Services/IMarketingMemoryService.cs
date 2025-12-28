namespace AutonomousMarketingPlatform.Application.Services;

/// <summary>
/// Servicio para gestión de memoria de marketing.
/// Proporciona métodos de alto nivel para guardar y consultar memoria.
/// </summary>
public interface IMarketingMemoryService
{
    /// <summary>
    /// Guarda una memoria de conversación.
    /// </summary>
    Task<Guid> SaveConversationMemoryAsync(
        Guid tenantId, 
        Guid? userId, 
        string conversation, 
        Dictionary<string, object>? context = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Guarda una decisión tomada por el usuario.
    /// </summary>
    Task<Guid> SaveDecisionMemoryAsync(
        Guid tenantId,
        Guid? userId,
        Guid? campaignId,
        string decision,
        Dictionary<string, object>? context = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Guarda un aprendizaje del sistema.
    /// </summary>
    Task<Guid> SaveLearningMemoryAsync(
        Guid tenantId,
        string learning,
        List<string>? tags = null,
        int relevanceScore = 7,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Guarda feedback del usuario sobre contenido generado.
    /// </summary>
    Task<Guid> SaveFeedbackMemoryAsync(
        Guid tenantId,
        Guid userId,
        Guid? contentId,
        string feedback,
        bool isPositive,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene contexto de memoria para generar contenido con IA.
    /// </summary>
    Task<DTOs.MemoryContextForAI> GetMemoryContextForAIAsync(
        Guid tenantId,
        Guid? userId = null,
        Guid? campaignId = null,
        List<string>? relevantTags = null,
        CancellationToken cancellationToken = default);
}

