using AutonomousMarketingPlatform.Application.Services;
using AutonomousMarketingPlatform.Domain.Interfaces;
using AutonomousMarketingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AutonomousMarketingPlatform.Infrastructure.Services.AI;

/// <summary>
/// Implementación de IAIProvider usando OpenAI API.
/// Obtiene la API key desde la base de datos (encriptada) o configuración como fallback.
/// </summary>
public class OpenAIProvider : IAIProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IEncryptionService _encryptionService;
    private readonly ITenantService _tenantService;
    private string? _apiKey;
    private string _model = "gpt-4";
    private bool _useMock = true;

    public OpenAIProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenAIProvider> logger,
        ApplicationDbContext dbContext,
        IEncryptionService encryptionService,
        ITenantService tenantService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _dbContext = dbContext;
        _encryptionService = encryptionService;
        _tenantService = tenantService;
        
        // Intentar obtener API key desde configuración (fallback)
        _apiKey = _configuration["AI:OpenAI:ApiKey"];
        _model = _configuration["AI:OpenAI:Model"] ?? "gpt-4";
        var useMockConfig = _configuration["AI:UseMock"];
        _useMock = string.IsNullOrEmpty(_apiKey) || (useMockConfig != null && bool.Parse(useMockConfig));

        // Configurar HttpClient base
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
    }

    /// <summary>
    /// Obtiene la API key desde la base de datos para el tenant actual.
    /// </summary>
    private async Task<string?> GetApiKeyFromDatabaseAsync()
    {
        try
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            if (!tenantId.HasValue)
            {
                _logger.LogWarning("No se pudo obtener TenantId para cargar configuración de IA");
                return null;
            }

            var config = await _dbContext.TenantAIConfigs
                .FirstOrDefaultAsync(c => c.TenantId == tenantId.Value && 
                                          c.Provider == "OpenAI" && 
                                          c.IsActive);

            if (config == null)
            {
                _logger.LogDebug("No se encontró configuración de IA en DB para tenant {TenantId}", tenantId);
                return null;
            }

            // Desencriptar API key
            var decryptedKey = _encryptionService.Decrypt(config.EncryptedApiKey);
            
            // Actualizar estadísticas de uso
            config.LastUsedAt = DateTime.UtcNow;
            config.UsageCount++;
            await _dbContext.SaveChangesAsync();

            _model = config.Model;
            return decryptedKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener API key desde base de datos");
            return null;
        }
    }

    /// <summary>
    /// Obtiene la API key (desde DB o configuración) y configura el HttpClient.
    /// </summary>
    private async Task EnsureApiKeyAsync()
    {
        if (!string.IsNullOrEmpty(_apiKey) && !_useMock)
            return;

        // Intentar obtener desde DB primero
        var dbKey = await GetApiKeyFromDatabaseAsync();
        if (!string.IsNullOrEmpty(dbKey))
        {
            _apiKey = dbKey;
            _useMock = false;
        }
        else if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("OpenAI API Key no configurada ni en DB ni en configuración. Usando modo mock.");
            _useMock = true;
        }

        // Configurar HttpClient si tenemos API key
        if (!_useMock && !string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }
    }

    public async Task<string> GenerateStrategyAsync(
        string contentDescription,
        string? userContext,
        string? campaignContext,
        CancellationToken cancellationToken = default)
    {
        await EnsureApiKeyAsync();

        if (_useMock)
        {
            return GenerateMockStrategy(contentDescription, userContext, campaignContext);
        }

        var prompt = BuildStrategyPrompt(contentDescription, userContext, campaignContext);
        return await CallOpenAIAsync(prompt, cancellationToken);
    }

    public async Task<GeneratedCopiesResult> GenerateCopiesAsync(
        string strategy,
        string contentDescription,
        string? userContext,
        CancellationToken cancellationToken = default)
    {
        await EnsureApiKeyAsync();

        if (_useMock)
        {
            return GenerateMockCopies(strategy, contentDescription);
        }

        var prompt = BuildCopiesPrompt(strategy, contentDescription, userContext);
        var response = await CallOpenAIAsync(prompt, cancellationToken);
        
        return ParseCopiesResponse(response);
    }

    public async Task<List<string>> GenerateHashtagsAsync(
        string copy,
        string? userContext,
        CancellationToken cancellationToken = default)
    {
        await EnsureApiKeyAsync();

        if (_useMock)
        {
            return GenerateMockHashtags(copy);
        }

        var prompt = BuildHashtagsPrompt(copy, userContext);
        var response = await CallOpenAIAsync(prompt, cancellationToken);
        
        return ParseHashtagsResponse(response);
    }

    public async Task<string> GenerateImagePromptAsync(
        string strategy,
        string contentDescription,
        string? userContext,
        CancellationToken cancellationToken = default)
    {
        await EnsureApiKeyAsync();

        if (_useMock)
        {
            return GenerateMockImagePrompt(strategy, contentDescription);
        }

        var prompt = BuildImagePromptPrompt(strategy, contentDescription, userContext);
        return await CallOpenAIAsync(prompt, cancellationToken);
    }

    public async Task<string> GenerateVideoPromptAsync(
        string strategy,
        string contentDescription,
        string? userContext,
        CancellationToken cancellationToken = default)
    {
        await EnsureApiKeyAsync();

        if (_useMock)
        {
            return GenerateMockVideoPrompt(strategy, contentDescription);
        }

        var prompt = BuildVideoPromptPrompt(strategy, contentDescription, userContext);
        return await CallOpenAIAsync(prompt, cancellationToken);
    }

    public async Task<PublicationChecklist> GeneratePublicationChecklistAsync(
        string channel,
        string copy,
        string? assetType,
        CancellationToken cancellationToken = default)
    {
        await EnsureApiKeyAsync();

        if (_useMock)
        {
            return GenerateMockChecklist(channel, copy, assetType);
        }

        var prompt = BuildChecklistPrompt(channel, copy, assetType);
        var response = await CallOpenAIAsync(prompt, cancellationToken);
        
        return ParseChecklistResponse(response, channel);
    }

    #region OpenAI API Calls

    private async Task<string> CallOpenAIAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            await EnsureApiKeyAsync();

            if (_useMock || string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API key no disponible");
            }

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "Eres un experto en marketing digital y generación de contenido." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            return responseObj.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al llamar a OpenAI API");
            throw;
        }
    }

    #endregion

    #region Prompt Builders

    private string BuildStrategyPrompt(string contentDescription, string? userContext, string? campaignContext)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Genera una estrategia de marketing estructurada basada en el siguiente contenido:");
        sb.AppendLine($"\nContenido: {SanitizeInput(contentDescription)}");
        
        if (!string.IsNullOrEmpty(userContext))
        {
            sb.AppendLine($"\nContexto del usuario:\n{SanitizeInput(userContext)}");
        }
        
        if (!string.IsNullOrEmpty(campaignContext))
        {
            sb.AppendLine($"\nContexto de campaña:\n{SanitizeInput(campaignContext)}");
        }
        
        sb.AppendLine("\nLa estrategia debe incluir: objetivos, audiencia objetivo, mensaje clave, tono y estilo.");
        
        return sb.ToString();
    }

    private string BuildCopiesPrompt(string strategy, string contentDescription, string? userContext)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Genera 3 versiones de copy publicitario basadas en la siguiente estrategia:");
        sb.AppendLine($"\nEstrategia: {SanitizeInput(strategy)}");
        sb.AppendLine($"\nContenido: {SanitizeInput(contentDescription)}");
        
        if (!string.IsNullOrEmpty(userContext))
        {
            sb.AppendLine($"\nContexto: {SanitizeInput(userContext)}");
        }
        
        sb.AppendLine("\nGenera:");
        sb.AppendLine("1. Copy corto (máximo 50 palabras)");
        sb.AppendLine("2. Copy medio (100-150 palabras)");
        sb.AppendLine("3. Copy largo (200-300 palabras)");
        sb.AppendLine("\nFormato: SHORT: [texto] | MEDIUM: [texto] | LONG: [texto]");
        
        return sb.ToString();
    }

    private string BuildHashtagsPrompt(string copy, string? userContext)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Genera 10-15 hashtags relevantes para el siguiente copy:");
        sb.AppendLine($"\nCopy: {SanitizeInput(copy)}");
        sb.AppendLine("\nFormato: lista separada por comas, sin #");
        
        return sb.ToString();
    }

    private string BuildImagePromptPrompt(string strategy, string contentDescription, string? userContext)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Genera un prompt detallado para un generador de imágenes (DALL-E, Midjourney) basado en:");
        sb.AppendLine($"\nEstrategia: {SanitizeInput(strategy)}");
        sb.AppendLine($"\nContenido: {SanitizeInput(contentDescription)}");
        sb.AppendLine("\nEl prompt debe ser descriptivo, incluir estilo visual, colores, composición.");
        
        return sb.ToString();
    }

    private string BuildVideoPromptPrompt(string strategy, string contentDescription, string? userContext)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Genera un prompt detallado para un generador de video/reel basado en:");
        sb.AppendLine($"\nEstrategia: {SanitizeInput(strategy)}");
        sb.AppendLine($"\nContenido: {SanitizeInput(contentDescription)}");
        sb.AppendLine("\nEl prompt debe incluir: escenas, transiciones, música sugerida, duración, estilo.");
        
        return sb.ToString();
    }

    private string BuildChecklistPrompt(string channel, string copy, string? assetType)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Genera un checklist de publicación para {channel} con el siguiente copy:");
        sb.AppendLine($"\nCopy: {SanitizeInput(copy)}");
        if (!string.IsNullOrEmpty(assetType))
        {
            sb.AppendLine($"\nTipo de asset: {assetType}");
        }
        sb.AppendLine("\nIncluye recomendaciones específicas del canal (formato, horarios, engagement, etc.)");
        sb.AppendLine("\nFormato: lista de items numerados");
        
        return sb.ToString();
    }

    #endregion

    #region Response Parsers

    private GeneratedCopiesResult ParseCopiesResponse(string response)
    {
        var result = new GeneratedCopiesResult();
        
        var shortMatch = Regex.Match(response, @"SHORT:\s*(.+?)(?:\||$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var mediumMatch = Regex.Match(response, @"MEDIUM:\s*(.+?)(?:\||$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var longMatch = Regex.Match(response, @"LONG:\s*(.+?)(?:\||$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        result.ShortCopy = shortMatch.Success ? shortMatch.Groups[1].Value.Trim() : "";
        result.MediumCopy = mediumMatch.Success ? mediumMatch.Groups[1].Value.Trim() : "";
        result.LongCopy = longMatch.Success ? longMatch.Groups[1].Value.Trim() : "";
        
        return result;
    }

    private List<string> ParseHashtagsResponse(string response)
    {
        return response.Split(',')
            .Select(h => h.Trim().TrimStart('#'))
            .Where(h => !string.IsNullOrEmpty(h))
            .Take(15)
            .ToList();
    }

    private PublicationChecklist ParseChecklistResponse(string response, string channel)
    {
        var checklist = new PublicationChecklist { Channel = channel };
        
        var items = response.Split('\n')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Trim().TrimStart('-', '*', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.'))
            .Where(item => !string.IsNullOrEmpty(item))
            .ToList();
        
        checklist.Items = items;
        
        return checklist;
    }

    #endregion

    #region Mock Implementations

    private string GenerateMockStrategy(string contentDescription, string? userContext, string? campaignContext)
    {
        return $@"Estrategia de Marketing Generada:

Objetivos:
- Aumentar engagement en redes sociales
- Generar leads cualificados
- Mejorar reconocimiento de marca

Audiencia Objetivo:
- Personas interesadas en {contentDescription}
- Edad: 25-45 años
- Intereses: Marketing digital, tecnología

Mensaje Clave:
Destacar los beneficios únicos del contenido presentado.

Tono y Estilo:
Profesional pero cercano, inspirador y orientado a resultados.";
    }

    private GeneratedCopiesResult GenerateMockCopies(string strategy, string contentDescription)
    {
        return new GeneratedCopiesResult
        {
            ShortCopy = $"Descubre cómo {contentDescription} puede transformar tu estrategia. ¡Empieza hoy!",
            MediumCopy = $"En el mundo del marketing digital, {contentDescription} representa una oportunidad única para destacar. Nuestra solución está diseñada para ayudarte a alcanzar tus objetivos de manera eficiente y efectiva. Únete a cientos de profesionales que ya están transformando su enfoque.",
            LongCopy = $"El marketing digital evoluciona constantemente, y {contentDescription} es la respuesta que estabas buscando. Con nuestra plataforma, podrás acceder a herramientas avanzadas que te permitirán crear campañas más efectivas, llegar a tu audiencia ideal y medir resultados en tiempo real. No importa si eres un emprendedor que está comenzando o una empresa establecida, nuestra solución se adapta a tus necesidades. Comienza tu transformación digital hoy mismo y descubre el potencial ilimitado que tienes al alcance de tus manos."
        };
    }

    private List<string> GenerateMockHashtags(string copy)
    {
        return new List<string> { "marketing", "digital", "estrategia", "contenido", "socialmedia", "branding", "growth", "engagement", "marketingdigital", "emprendimiento" };
    }

    private string GenerateMockImagePrompt(string strategy, string contentDescription)
    {
        return $"Professional marketing image featuring {contentDescription}, modern design, vibrant colors, clean composition, business style, high quality, 4K resolution";
    }

    private string GenerateMockVideoPrompt(string strategy, string contentDescription)
    {
        return $"Dynamic marketing video showcasing {contentDescription}, smooth transitions, upbeat music, 30 seconds duration, professional editing, engaging visuals, modern style";
    }

    private PublicationChecklist GenerateMockChecklist(string channel, string copy, string? assetType)
    {
        var items = new List<string>();
        
        switch (channel.ToLower())
        {
            case "instagram":
                items.AddRange(new[]
                {
                    "Formato: 1080x1080 (cuadrado) o 1080x1350 (vertical)",
                    "Horario óptimo: 11:00-13:00 o 19:00-21:00",
                    "Usar 5-10 hashtags relevantes",
                    "Incluir call-to-action claro",
                    "Etiquetar ubicación si aplica"
                });
                break;
            case "facebook":
                items.AddRange(new[]
                {
                    "Formato: 1200x630 (imagen) o 1080x1080 (video)",
                    "Horario óptimo: 13:00-15:00",
                    "Texto del post: máximo 40 caracteres para mejor engagement",
                    "Incluir enlace si aplica",
                    "Usar formato de video para mayor alcance"
                });
                break;
            case "tiktok":
                items.AddRange(new[]
                {
                    "Formato: 1080x1920 (vertical, 9:16)",
                    "Duración: 15-60 segundos",
                    "Música trending y relevante",
                    "Primeros 3 segundos deben captar atención",
                    "Usar hashtags trending pero relevantes"
                });
                break;
            default:
                items.Add("Revisar formato específico del canal");
                items.Add("Optimizar para audiencia objetivo");
                break;
        }
        
        return new PublicationChecklist
        {
            Channel = channel,
            Items = items,
            Recommendations = new Dictionary<string, string>
            {
                { "BestTime", "Horario de mayor engagement" },
                { "Hashtags", "Usar mix de trending y nicho" }
            }
        };
    }

    #endregion

    #region Security

    private string SanitizeInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        
        // Remover caracteres peligrosos y limitar longitud
        var sanitized = Regex.Replace(input, @"[<>""']", "");
        return sanitized.Length > 5000 ? sanitized.Substring(0, 5000) : sanitized;
    }

    #endregion
}

