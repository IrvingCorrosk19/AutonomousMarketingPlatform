namespace AutonomousMarketingPlatform.Domain.Interfaces;

/// <summary>
/// Interfaz para proveedores de IA (OpenAI, Anthropic, etc.).
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Genera una estrategia de marketing a partir de contenido y contexto.
    /// </summary>
    Task<string> GenerateStrategyAsync(
        string contentDescription,
        string? userContext,
        string? campaignContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera copies de marketing (corto, medio, largo).
    /// </summary>
    Task<GeneratedCopiesResult> GenerateCopiesAsync(
        string strategy,
        string contentDescription,
        string? userContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera hashtags sugeridos para un copy.
    /// </summary>
    Task<List<string>> GenerateHashtagsAsync(
        string copy,
        string? userContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera un prompt para generador de imágenes.
    /// </summary>
    Task<string> GenerateImagePromptAsync(
        string strategy,
        string contentDescription,
        string? userContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera un prompt para generador de video/reel.
    /// </summary>
    Task<string> GenerateVideoPromptAsync(
        string strategy,
        string contentDescription,
        string? userContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera checklist de publicación para un canal específico.
    /// </summary>
    Task<PublicationChecklist> GeneratePublicationChecklistAsync(
        string channel,
        string copy,
        string? assetType,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado de generación de copies.
/// </summary>
public class GeneratedCopiesResult
{
    public string ShortCopy { get; set; } = string.Empty;
    public string MediumCopy { get; set; } = string.Empty;
    public string LongCopy { get; set; } = string.Empty;
}

/// <summary>
/// Checklist de publicación para un canal.
/// </summary>
public class PublicationChecklist
{
    public string Channel { get; set; } = string.Empty;
    public List<string> Items { get; set; } = new();
    public Dictionary<string, string> Recommendations { get; set; } = new();
}

