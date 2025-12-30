# Ejemplos de Uso - Workflows n8n

## Workflow 01: Trigger - Marketing Request

### Ejemplo 1: Solicitud Completa Válida

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/marketing-request \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "instruction": "Crear una campaña de verano para promocionar nuestros nuevos productos de playa. Enfocarse en público joven de 18-35 años.",
    "assets": [
      "https://cdn.example.com/images/beach-product-1.jpg",
      "https://cdn.example.com/images/beach-product-2.jpg"
    ],
    "channels": ["instagram", "facebook"],
    "requiresApproval": true
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Request validated successfully",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "instruction": "Crear una campaña de verano para promocionar nuestros nuevos productos de playa. Enfocarse en público joven de 18-35 años.",
    "assets": [
      "https://cdn.example.com/images/beach-product-1.jpg",
      "https://cdn.example.com/images/beach-product-2.jpg"
    ],
    "channels": ["instagram", "facebook"],
    "requiresApproval": true,
    "timestamp": "2025-01-01T12:00:00.000Z"
  }
}
```

### Ejemplo 2: Solicitud sin campaignId (Válida)

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/marketing-request \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Generar contenido para redes sociales sobre nuestro nuevo servicio",
    "assets": [],
    "channels": ["instagram"],
    "requiresApproval": false
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Request validated successfully",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "campaignId": null,
    "instruction": "Generar contenido para redes sociales sobre nuestro nuevo servicio",
    "assets": [],
    "channels": ["instagram"],
    "requiresApproval": false,
    "timestamp": "2025-01-01T12:00:00.000Z"
  }
}
```

### Ejemplo 3: Error - Falta tenantId

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/marketing-request \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Crear campaña",
    "channels": ["instagram"],
    "requiresApproval": true
  }'
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "error": "Missing required fields",
  "message": "The request must include: tenantId, userId, instruction, channels, and requiresApproval",
  "received": {
    "tenantId": "missing",
    "userId": "present",
    "instruction": "present",
    "channels": "present",
    "requiresApproval": "present"
  }
}
```

### Ejemplo 4: Error - Falta channels

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/marketing-request \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Crear campaña",
    "requiresApproval": true
  }'
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "error": "Missing required fields",
  "message": "The request must include: tenantId, userId, instruction, channels, and requiresApproval",
  "received": {
    "tenantId": "present",
    "userId": "present",
    "instruction": "present",
    "channels": "missing",
    "requiresApproval": "present"
  }
}
```

### Ejemplo 5: Error - channels vacío

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/marketing-request \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Crear campaña",
    "channels": [],
    "requiresApproval": true
  }'
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "error": "Missing required fields",
  "message": "The request must include: tenantId, userId, instruction, channels, and requiresApproval",
  "received": {
    "tenantId": "present",
    "userId": "present",
    "instruction": "present",
    "channels": "present",
    "requiresApproval": "present"
  }
}
```

## Workflow 02: Validate Consents

### Ejemplo 1: Validación Exitosa

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/validate-consents \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001"
  }'
```

**Backend Response (GET /api/consents/check?tenantId=xxx&userId=yyy):**
```json
{
  "aiConsent": true,
  "publishingConsent": true
}
```

**n8n Response (200 OK):**
```json
{
  "success": true,
  "message": "All required consents validated",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "aiConsent": true,
    "publishingConsent": true,
    "validatedAt": "2025-01-01T12:00:00.000Z"
  }
}
```

### Ejemplo 2: Error - Falta Consentimiento de IA

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/validate-consents \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001"
  }'
```

**Backend Response:**
```json
{
  "aiConsent": false,
  "publishingConsent": true
}
```

**n8n Response (403 Forbidden):**
```json
{
  "success": false,
  "error": "Missing required consents",
  "message": "User does not have required consents to proceed",
  "details": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "aiConsent": false,
    "publishingConsent": true,
    "missingConsents": ["AIGeneration"]
  }
}
```

## Workflow 03: Load Marketing Memory

### Ejemplo 1: Carga Exitosa

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/load-marketing-memory \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing memory loaded successfully",
  "data": {
    "preferredTone": "profesional",
    "dislikedFormats": ["texto largo"],
    "bestPerformingChannels": ["instagram", "facebook"],
    "restrictions": [],
    "userPreferences": [...],
    "recentConversations": [...],
    "campaignMemories": [...],
    "learnings": {...},
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "loadedAt": "2025-01-01T12:00:00.000Z"
  }
}
```

## Workflow 04: Analyze Instruction AI

### Ejemplo 1: Análisis Exitoso

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/analyze-instruction \
  -H "Content-Type: application/json" \
  -d '{
    "instruction": "Crear una campaña urgente para promocionar nuestro nuevo producto de verano en Instagram. Tono profesional pero amigable, dirigido a jóvenes de 18-35 años.",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Instruction analyzed successfully",
  "data": {
    "objective": "Promocionar nuevo producto de verano",
    "tone": "profesional-amigable",
    "urgency": "high",
    "contentType": "post",
    "targetAudience": "jóvenes de 18-35 años",
    "keyMessages": [
      "Nuevo producto de verano",
      "Promoción especial",
      "Dirigido a jóvenes"
    ],
    "hashtags": [
      "verano",
      "nuevoproducto",
      "promocion"
    ],
    "channels": ["instagram"],
    "originalInstruction": "Crear una campaña urgente para promocionar nuestro nuevo producto de verano en Instagram. Tono profesional pero amigable, dirigido a jóvenes de 18-35 años.",
    "analyzedAt": "2025-01-01T12:00:00.000Z",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001"
  }
}
```

### Ejemplo 2: Instrucción con Múltiples Canales

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/analyze-instruction \
  -H "Content-Type: application/json" \
  -d '{
    "instruction": "Necesito contenido para Facebook e Instagram sobre nuestro evento de lanzamiento. Tono formal, urgencia media.",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Instruction analyzed successfully",
  "data": {
    "objective": "Promocionar evento de lanzamiento",
    "tone": "formal",
    "urgency": "medium",
    "contentType": "post",
    "targetAudience": "",
    "keyMessages": [
      "Evento de lanzamiento",
      "Facebook e Instagram"
    ],
    "hashtags": [
      "lanzamiento",
      "evento"
    ],
    "channels": ["facebook", "instagram"],
    "originalInstruction": "Necesito contenido para Facebook e Instagram...",
    "analyzedAt": "2025-01-01T12:00:00.000Z",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": ""
  }
}
```

### Ejemplo 3: Instrucción con Urgencia Crítica

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/analyze-instruction \
  -H "Content-Type: application/json" \
  -d '{
    "instruction": "URGENTE: Necesito un post para Instagram AHORA sobre la oferta flash que termina hoy. Tono casual y divertido.",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Instruction analyzed successfully",
  "data": {
    "objective": "Promocionar oferta flash que termina hoy",
    "tone": "casual",
    "urgency": "critical",
    "contentType": "story",
    "targetAudience": "",
    "keyMessages": [
      "Oferta flash",
      "Termina hoy",
      "Urgente"
    ],
    "hashtags": [
      "oferta",
      "flash",
      "urgente"
    ],
    "channels": ["instagram"],
    "originalInstruction": "URGENTE: Necesito un post para Instagram AHORA...",
    "analyzedAt": "2025-01-01T12:00:00.000Z",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": ""
  }
}
```

## Integración desde C# (ASP.NET Core)

### Ejemplo de Servicio

```csharp
using System.Net.Http.Json;

public class N8nWorkflowService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<N8nWorkflowService> _logger;
    private readonly string _webhookUrl;

    public N8nWorkflowService(
        HttpClient httpClient,
        ILogger<N8nWorkflowService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _webhookUrl = configuration["N8n:WebhookUrl"];
    }

    public async Task<MarketingRequestResponse> TriggerMarketingRequestAsync(
        Guid tenantId,
        Guid userId,
        Guid? campaignId,
        string instruction,
        List<string> assets,
        List<string> channels,
        bool requiresApproval,
        CancellationToken cancellationToken = default)
    {
        var request = new MarketingRequest
        {
            TenantId = tenantId.ToString(),
            UserId = userId.ToString(),
            CampaignId = campaignId?.ToString(),
            Instruction = instruction,
            Assets = assets ?? new List<string>(),
            Channels = channels,
            RequiresApproval = requiresApproval
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                _webhookUrl,
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<MarketingRequestResponse>(
                cancellationToken);

            if (result?.Success == true)
            {
                _logger.LogInformation(
                    "Marketing request triggered successfully. RequestId: {RequestId}",
                    result.Data?.RequestId);
            }

            return result ?? new MarketingRequestResponse { Success = false };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error triggering marketing request");
            throw;
        }
    }

    public async Task<InstructionAnalysisResponse> AnalyzeInstructionAsync(
        Guid tenantId,
        Guid? userId,
        string instruction,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            tenantId = tenantId.ToString(),
            userId = userId?.ToString(),
            instruction = instruction
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_webhookUrl}/analyze-instruction",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<InstructionAnalysisResponse>(
                cancellationToken);

            return result ?? new InstructionAnalysisResponse { Success = false };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error analyzing instruction");
            throw;
        }
    }
}

public class InstructionAnalysisResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public InstructionAnalysisData? Data { get; set; }
}

public class InstructionAnalysisData
{
    public string Objective { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public List<string> KeyMessages { get; set; } = new();
    public List<string> Hashtags { get; set; } = new();
    public List<string> Channels { get; set; } = new();
}
```

## Configuración en appsettings.json

```json
{
  "N8n": {
    "WebhookUrl": "https://your-n8n-instance.com/webhook",
    "Timeout": 30
  }
}
```

## Workflow 05: Generate Marketing Strategy

### Ejemplo 1: Generación de Estrategia Completa

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-marketing-strategy \
  -H "Content-Type: application/json" \
  -d '{
    "analysis": {
      "objective": "Promocionar nuevo producto de verano",
      "tone": "profesional-amigable",
      "urgency": "high",
      "contentType": "post",
      "targetAudience": "jóvenes de 18-35 años",
      "keyMessages": ["Nuevo producto", "Verano", "Oferta especial"],
      "hashtags": ["verano", "nuevoproducto"],
      "channels": ["instagram"]
    },
    "memory": {
      "preferredTone": "profesional",
      "dislikedFormats": ["texto largo"],
      "bestPerformingChannels": ["instagram", "facebook"],
      "restrictions": []
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing strategy generated successfully",
  "data": {
    "mainMessage": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos que buscan estilo y comodidad",
    "cta": "Compra ahora y obtén 20% de descuento en tu primera compra",
    "recommendedFormat": "post",
    "suggestedSchedule": {
      "bestDays": ["lunes", "miércoles", "viernes"],
      "bestTimes": ["09:00", "13:00", "18:00"],
      "timezone": "UTC"
    },
    "contentStructure": {
      "headline": "Nuevo Producto de Verano - Estilo y Comodidad",
      "body": "Perfecto para jóvenes que buscan combinar estilo y funcionalidad. Diseñado pensando en ti.",
      "hashtags": ["verano", "nuevoproducto", "moda", "jovenes", "estilo"],
      "mentions": []
    },
    "channels": ["instagram", "facebook"],
    "tone": "profesional-amigable",
    "targetAudience": "jóvenes de 18-35 años",
    "keyPoints": [
      "Nuevo producto de verano",
      "Dirigido a jóvenes activos",
      "Oferta especial de lanzamiento"
    ],
    "metadata": {
      "tenantId": "550e8400-e29b-41d4-a716-446655440000",
      "generatedAt": "2025-01-01T12:00:00.000Z"
    }
  }
}
```

### Ejemplo 2: Estrategia para Story Urgente

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-marketing-strategy \
  -H "Content-Type: application/json" \
  -d '{
    "analysis": {
      "objective": "Promocionar oferta flash",
      "tone": "casual",
      "urgency": "critical",
      "contentType": "story",
      "keyMessages": ["Oferta flash", "Termina hoy"],
      "channels": ["instagram"]
    },
    "memory": {
      "preferredTone": "casual",
      "bestPerformingChannels": ["instagram"]
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing strategy generated successfully",
  "data": {
    "mainMessage": "Oferta flash exclusiva - Solo hoy",
    "cta": "Compra ahora antes de que termine",
    "recommendedFormat": "story",
    "suggestedSchedule": {
      "bestDays": ["hoy"],
      "bestTimes": ["09:00", "12:00", "15:00", "18:00", "21:00"],
      "timezone": "UTC"
    },
    "contentStructure": {
      "headline": "Oferta Flash - Solo Hoy",
      "body": "No te pierdas esta oportunidad única",
      "hashtags": ["oferta", "flash", "urgente"],
      "mentions": []
    },
    "channels": ["instagram"],
    "tone": "casual",
    "targetAudience": "",
    "keyPoints": ["Oferta flash", "Termina hoy", "Urgente"]
  }
}
```

### Ejemplo 3: Estrategia Multi-Canal

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-marketing-strategy \
  -H "Content-Type: application/json" \
  -d '{
    "analysis": {
      "objective": "Evento de lanzamiento",
      "tone": "formal",
      "urgency": "medium",
      "contentType": "post",
      "channels": ["facebook", "instagram", "linkedin"]
    },
    "memory": {
      "preferredTone": "formal",
      "bestPerformingChannels": ["facebook", "instagram"]
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing strategy generated successfully",
  "data": {
    "mainMessage": "Te invitamos a nuestro evento de lanzamiento exclusivo",
    "cta": "Reserva tu lugar ahora",
    "recommendedFormat": "post",
    "suggestedSchedule": {
      "bestDays": ["martes", "miércoles", "jueves"],
      "bestTimes": ["10:00", "14:00", "17:00"],
      "timezone": "UTC"
    },
    "contentStructure": {
      "headline": "Evento de Lanzamiento Exclusivo",
      "body": "Únete a nosotros en este evento especial donde presentaremos nuestras novedades",
      "hashtags": ["evento", "lanzamiento", "exclusivo"],
      "mentions": []
    },
    "channels": ["facebook", "instagram", "linkedin"],
    "tone": "formal",
    "targetAudience": "",
    "keyPoints": ["Evento de lanzamiento", "Exclusivo", "Reserva tu lugar"]
  }
}
```

## Workflow 06: Generate Marketing Copy

### Ejemplo 1: Generación de Copy Completo

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-marketing-copy \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Descubre nuestro nuevo producto de verano",
      "cta": "Compra ahora y obtén 20% de descuento",
      "recommendedFormat": "post",
      "contentStructure": {
        "headline": "Nuevo Producto de Verano",
        "body": "Perfecto para jóvenes activos",
        "hashtags": ["verano", "nuevoproducto"]
      },
      "channels": ["instagram", "facebook"],
      "tone": "profesional-amigable",
      "targetAudience": "jóvenes de 18-35 años"
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing copy generated successfully",
  "data": {
    "shortCopy": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento",
    "longCopy": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos que buscan estilo y comodidad. Perfecto para tus aventuras de verano. Compra ahora y obtén 20% de descuento en tu primera compra.",
    "hashtags": ["verano", "nuevoproducto", "moda", "jovenes", "estilo"],
    "variants": {
      "variantA": {
        "shortCopy": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento",
        "longCopy": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos...",
        "hashtags": ["verano", "nuevoproducto", "moda"]
      },
      "variantB": {
        "shortCopy": "Nuevo producto de verano disponible. Oferta especial: 20% OFF",
        "longCopy": "Lanzamos nuestro nuevo producto de verano pensado en ti...",
        "hashtags": ["verano", "oferta", "nuevo", "moda"]
      }
    },
    "headline": "Nuevo Producto de Verano - Estilo y Comodidad",
    "cta": "Compra ahora y obtén 20% de descuento",
    "emojiSuggestions": ["☀️", "🏖️", "👕"],
    "publishFormat": {
      "instagram": {
        "caption": "Descubre nuestro nuevo producto de verano...\n\n#verano #nuevoproducto #moda #jovenes #estilo",
        "storyText": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento"
      },
      "facebook": {
        "post": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos..."
      },
      "twitter": {
        "tweet": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento"
      }
    }
  }
}
```

### Ejemplo 2: Copy para Story Urgente

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-marketing-copy \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Oferta flash exclusiva - Solo hoy",
      "cta": "Compra ahora antes de que termine",
      "recommendedFormat": "story",
      "urgency": "critical",
      "channels": ["instagram"]
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing copy generated successfully",
  "data": {
    "shortCopy": "Oferta flash exclusiva - Solo hoy. Compra ahora antes de que termine",
    "longCopy": "No te pierdas esta oportunidad única. Oferta flash exclusiva disponible solo hoy. Compra ahora antes de que termine y aprovecha los mejores descuentos.",
    "hashtags": ["oferta", "flash", "urgente", "hoy", "descuento"],
    "variants": {
      "variantA": {
        "shortCopy": "Oferta flash exclusiva - Solo hoy. Compra ahora",
        "longCopy": "No te pierdas esta oportunidad única...",
        "hashtags": ["oferta", "flash", "urgente"]
      },
      "variantB": {
        "shortCopy": "Últimas horas. Oferta especial termina hoy",
        "longCopy": "Aprovecha ahora mismo esta oferta especial que termina hoy...",
        "hashtags": ["oferta", "hoy", "ultimashoras"]
      }
    },
    "publishFormat": {
      "instagram": {
        "storyText": "Oferta flash exclusiva - Solo hoy. Compra ahora antes de que termine"
      }
    }
  }
}
```

### Ejemplo 3: Copy Multi-Canal

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-marketing-copy \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Evento de lanzamiento exclusivo",
      "cta": "Reserva tu lugar ahora",
      "recommendedFormat": "post",
      "channels": ["facebook", "instagram", "linkedin"],
      "tone": "formal"
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing copy generated successfully",
  "data": {
    "shortCopy": "Te invitamos a nuestro evento de lanzamiento exclusivo. Reserva tu lugar ahora",
    "longCopy": "Te invitamos a nuestro evento de lanzamiento exclusivo donde presentaremos nuestras novedades más importantes. Únete a nosotros en este evento especial. Reserva tu lugar ahora y sé parte de esta experiencia única.",
    "hashtags": ["evento", "lanzamiento", "exclusivo", "networking"],
    "publishFormat": {
      "instagram": {
        "caption": "Te invitamos a nuestro evento de lanzamiento exclusivo...\n\n#evento #lanzamiento #exclusivo #networking"
      },
      "facebook": {
        "post": "Te invitamos a nuestro evento de lanzamiento exclusivo donde presentaremos nuestras novedades más importantes..."
      },
      "twitter": {
        "tweet": "Te invitamos a nuestro evento de lanzamiento exclusivo. Reserva tu lugar ahora"
      }
    }
  }
}
```

## Workflow 07: Generate Visual Prompts

### Ejemplo 1: Generación de Prompts para Post

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-visual-prompts \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Descubre nuestro nuevo producto de verano",
      "tone": "profesional-amigable",
      "targetAudience": "jóvenes de 18-35 años",
      "recommendedFormat": "post",
      "channels": ["instagram", "facebook"]
    },
    "copy": {
      "longCopy": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos",
      "headline": "Nuevo Producto de Verano"
    },
    "memory": {
      "preferredTone": "profesional",
      "bestPerformingChannels": ["instagram"]
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Visual prompts generated successfully",
  "data": {
    "imagePrompt": "High-quality marketing image featuring a vibrant summer product display. Professional photography style with natural lighting, modern composition, warm color palette with blues and yellows, engaging visual that appeals to young adults aged 18-35. The image should convey energy, style, and summer vibes. Optimized for Instagram and Facebook, 1:1 aspect ratio, professional quality, balanced composition with product as focal point",
    "videoPrompt": "Marketing video showcasing summer product in dynamic scenes. Smooth camera movements with gentle pans and zooms, professional cinematography style, vibrant color grading with summer tones, engaging visual storytelling that captures attention. Include product close-ups, lifestyle shots, and energetic transitions. Optimized for social media, 9:16 aspect ratio for stories, professional quality, natural lighting with golden hour aesthetic",
    "imageStyle": "professional",
    "colorPalette": ["vibrant", "warm", "summer", "blue", "yellow"],
    "mood": "energetic",
    "aspectRatio": "1:1",
    "technicalSpecs": {
      "resolution": "high",
      "quality": "professional",
      "lighting": "natural",
      "composition": "balanced"
    }
  }
}
```

### Ejemplo 2: Prompts para Story/Reel

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-visual-prompts \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Oferta flash exclusiva",
      "tone": "casual",
      "recommendedFormat": "story",
      "urgency": "critical",
      "channels": ["instagram"]
    },
    "copy": {
      "shortCopy": "Oferta flash exclusiva - Solo hoy"
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Visual prompts generated successfully",
  "data": {
    "imagePrompt": "Urgent marketing image for flash sale. Bold, eye-catching design with vibrant colors, dynamic composition, high contrast, attention-grabbing visual style. Include text overlay space, energetic mood, optimized for Instagram stories, 9:16 aspect ratio, professional quality",
    "videoPrompt": "Dynamic marketing video for flash sale. Fast-paced editing, energetic transitions, bold graphics, vibrant colors, urgent and exciting mood. Include countdown elements, product highlights, and call-to-action visuals. Optimized for Instagram stories and reels, 9:16 aspect ratio, professional quality",
    "imageStyle": "vibrant",
    "colorPalette": ["bold", "vibrant", "high-contrast"],
    "mood": "urgent",
    "aspectRatio": "9:16",
    "technicalSpecs": {
      "resolution": "high",
      "quality": "professional",
      "lighting": "dramatic",
      "composition": "dynamic"
    }
  }
}
```

### Ejemplo 3: Prompts para Contenido Formal

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/generate-visual-prompts \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Evento de lanzamiento exclusivo",
      "tone": "formal",
      "recommendedFormat": "post",
      "channels": ["linkedin", "facebook"]
    },
    "copy": {
      "headline": "Evento de Lanzamiento Exclusivo"
    },
    "tenantId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Visual prompts generated successfully",
  "data": {
    "imagePrompt": "Elegant and sophisticated marketing image for exclusive launch event. Professional corporate photography style, refined composition, sophisticated color palette with deep blues and golds, formal and prestigious mood. Clean, minimalist design with emphasis on professionalism. Optimized for LinkedIn and Facebook, 16:9 aspect ratio, high-end quality, balanced composition",
    "videoPrompt": "Professional marketing video for exclusive launch event. Smooth, elegant camera movements, sophisticated cinematography, refined color grading, formal and prestigious aesthetic. Include event venue shots, product reveals, and professional transitions. Optimized for professional networks, 16:9 aspect ratio, cinematic quality",
    "imageStyle": "sophisticated",
    "colorPalette": ["deep blue", "gold", "elegant"],
    "mood": "prestigious",
    "aspectRatio": "16:9",
    "technicalSpecs": {
      "resolution": "high",
      "quality": "cinematic",
      "lighting": "studio",
      "composition": "refined"
    }
  }
}
```

## Workflow 08: Build Marketing Pack

### Ejemplo 1: Construcción de MarketingPack Completo

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/build-marketing-pack \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {
      "mainMessage": "Descubre nuestro nuevo producto de verano",
      "cta": "Compra ahora y obtén 20% de descuento",
      "recommendedFormat": "post",
      "tone": "profesional-amigable",
      "targetAudience": "jóvenes de 18-35 años",
      "keyPoints": ["Nuevo producto", "Oferta especial"],
      "suggestedSchedule": {
        "bestDays": ["lunes", "miércoles", "viernes"],
        "bestTimes": ["09:00", "13:00", "18:00"]
      },
      "channels": ["instagram", "facebook"]
    },
    "copy": {
      "shortCopy": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento",
      "longCopy": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos que buscan estilo y comodidad.",
      "hashtags": ["verano", "nuevoproducto", "moda"],
      "variants": {
        "variantA": {
          "shortCopy": "Nuevo producto de verano disponible. Oferta especial: 20% OFF",
          "longCopy": "Lanzamos nuestro nuevo producto de verano pensado en ti...",
          "hashtags": ["verano", "oferta", "nuevo"]
        }
      },
      "publishFormat": {
        "instagram": {
          "caption": "Descubre nuestro nuevo producto...\n\n#verano #nuevoproducto #moda"
        }
      }
    },
    "visualPrompts": {
      "imagePrompt": "High-quality marketing image featuring summer product...",
      "videoPrompt": "Marketing video showcasing summer product...",
      "imageStyle": "professional",
      "aspectRatio": "1:1"
    },
    "media": [
      "https://cdn.example.com/images/summer-product.jpg"
    ],
    "channels": ["instagram", "facebook"],
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "contentId": "880e8400-e29b-41d4-a716-446655440003",
    "requiresApproval": true
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing pack built successfully",
  "data": {
    "id": "990e8400-e29b-41d4-a716-446655440004",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "contentId": "880e8400-e29b-41d4-a716-446655440003",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "strategy": "{\"mainMessage\":\"Descubre nuestro nuevo producto de verano\",...}",
    "status": "Generated",
    "version": 1,
    "copies": [
      {
        "id": "copy-1",
        "copyType": "long",
        "content": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos...",
        "hashtags": "verano, nuevoproducto, moda",
        "suggestedChannel": "instagram",
        "publicationChecklist": {
          "hasCopy": true,
          "hasHashtags": true,
          "hasMedia": true,
          "readyForPublication": true
        }
      },
      {
        "id": "copy-2",
        "copyType": "short",
        "content": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento",
        "hashtags": "verano, nuevoproducto, moda",
        "suggestedChannel": "instagram"
      },
      {
        "id": "copy-3",
        "copyType": "variant-a",
        "content": "Lanzamos nuestro nuevo producto de verano pensado en ti...",
        "hashtags": "verano, oferta, nuevo",
        "publicationChecklist": {
          "hasCopy": true,
          "isVariant": true,
          "variantType": "A"
        }
      }
    ],
    "assetPrompts": [
      {
        "id": "prompt-img-1",
        "assetType": "image",
        "prompt": "High-quality marketing image featuring summer product...",
        "parameters": {
          "style": "professional",
          "aspectRatio": "1:1",
          "colorPalette": ["vibrant", "warm"],
          "mood": "energetic"
        },
        "suggestedChannel": "instagram"
      },
      {
        "id": "prompt-vid-1",
        "assetType": "video",
        "prompt": "Marketing video showcasing summer product...",
        "parameters": {
          "aspectRatio": "16:9"
        }
      }
    ],
    "channels": ["instagram", "facebook"],
    "media": ["https://cdn.example.com/images/summer-product.jpg"],
    "requiresApproval": true,
    "approvalInfo": {
      "requiresApproval": true,
      "readyForApproval": true,
      "readyForPublication": false,
      "approvalChecklist": {
        "hasStrategy": true,
        "hasCopy": true,
        "hasVisualPrompts": true,
        "hasChannels": true,
        "hasMedia": true
      }
    },
    "publicationInfo": {
      "suggestedSchedule": {
        "bestDays": ["lunes", "miércoles", "viernes"],
        "bestTimes": ["09:00", "13:00", "18:00"]
      },
      "publishFormats": {
        "instagram": {
          "caption": "Descubre nuestro nuevo producto...\n\n#verano #nuevoproducto #moda"
        }
      },
      "channels": [
        {
          "name": "instagram",
          "copy": "Descubre nuestro nuevo producto...",
          "format": "post",
          "ready": true
        },
        {
          "name": "facebook",
          "copy": "Descubre nuestro nuevo producto...",
          "format": "post",
          "ready": true
        }
      ]
    }
  }
}
```

### Ejemplo 2: MarketingPack sin Aprobación

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/build-marketing-pack \
  -H "Content-Type: application/json" \
  -d '{
    "strategy": {...},
    "copy": {...},
    "visualPrompts": {...},
    "requiresApproval": false
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing pack built successfully",
  "data": {
    "id": "...",
    "status": "Ready",
    "requiresApproval": false,
    "approvalInfo": {
      "requiresApproval": false,
      "readyForApproval": true,
      "readyForPublication": true
    }
  }
}
```

### Ejemplo 3: Error - Componentes Faltantes

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/build-marketing-pack \
  -H "Content-Type: application/json" \
  -d '{
    "copy": {
      "longCopy": "Copy sin estrategia"
    }
  }'
```

**Response (500 Internal Server Error):**
```json
{
  "success": false,
  "error": "Marketing pack build error",
  "message": "Failed to build marketing pack",
  "details": {
    "error": "Missing required components"
  }
}
```

## Workflow 09: Human Approval Flow

### Ejemplo 1: Pack que Requiere Aprobación

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/human-approval-flow \
  -H "Content-Type: application/json" \
  -d '{
    "marketingPack": {
      "id": "990e8400-e29b-41d4-a716-446655440004",
      "tenantId": "550e8400-e29b-41d4-a716-446655440000",
      "userId": "660e8400-e29b-41d4-a716-446655440001",
      "contentId": "880e8400-e29b-41d4-a716-446655440003",
      "campaignId": "770e8400-e29b-41d4-a716-446655440002",
      "strategy": "{\"mainMessage\":\"...\"}",
      "status": "Generated",
      "version": 1,
      "copies": [...],
      "assetPrompts": [...],
      "requiresApproval": true
    }
  }'
```

**Backend Request (POST /api/marketing-packs):**
```json
{
  "id": "990e8400-e29b-41d4-a716-446655440004",
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "660e8400-e29b-41d4-a716-446655440001",
  "contentId": "880e8400-e29b-41d4-a716-446655440003",
  "campaignId": "770e8400-e29b-41d4-a716-446655440002",
  "strategy": "{\"mainMessage\":\"...\"}",
  "status": "RequiresApproval",
  "version": 1,
  "metadata": "{...}",
  "copies": [...],
  "assetPrompts": [...]
}
```

**n8n Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing pack sent for approval",
  "data": {
    "packId": "990e8400-e29b-41d4-a716-446655440004",
    "status": "RequiresApproval",
    "requiresApproval": true,
    "message": "Pack has been saved and is waiting for human approval",
    "nextStep": "approval"
  }
}
```

**Flujo:** El workflow se detiene aquí. El pack queda guardado en el backend con estado "RequiresApproval" esperando aprobación humana.

### Ejemplo 2: Pack Listo para Publicar (Sin Aprobación)

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/human-approval-flow \
  -H "Content-Type: application/json" \
  -d '{
    "marketingPack": {
      "id": "990e8400-e29b-41d4-a716-446655440004",
      "tenantId": "550e8400-e29b-41d4-a716-446655440000",
      "userId": "660e8400-e29b-41d4-a716-446655440001",
      "contentId": "880e8400-e29b-41d4-a716-446655440003",
      "strategy": "{\"mainMessage\":\"...\"}",
      "status": "Generated",
      "copies": [...],
      "assetPrompts": [...],
      "requiresApproval": false
    }
  }'
```

**n8n Response (200 OK):**
```json
{
  "success": true,
  "message": "Marketing pack ready for publication",
  "data": {
    "packId": "990e8400-e29b-41d4-a716-446655440004",
    "status": "Ready",
    "requiresApproval": false,
    "message": "Pack does not require approval, ready to publish",
    "nextStep": "publish",
    "marketingPack": {
      "id": "990e8400-e29b-41d4-a716-446655440004",
      "tenantId": "550e8400-e29b-41d4-a716-446655440000",
      "userId": "660e8400-e29b-41d4-a716-446655440001",
      "contentId": "880e8400-e29b-41d4-a716-446655440003",
      "strategy": "{\"mainMessage\":\"...\"}",
      "copies": [...],
      "assetPrompts": [...],
      "channels": ["instagram", "facebook"],
      "publicationInfo": {...}
    }
  }
}
```

**Flujo:** El workflow continúa. El pack está listo para pasar al siguiente workflow de publicación.

### Ejemplo 3: Error al Guardar en Backend

**n8n Response (500 Internal Server Error):**
```json
{
  "success": false,
  "error": "Backend error",
  "message": "Failed to save marketing pack to backend",
  "details": {
    "statusCode": "500",
    "error": "Internal server error"
  }
}
```

## Workflow 10: Publish Content

### Ejemplo 1: Publicar en Instagram

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/publish-content \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "instagram",
    "content": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos",
    "hashtags": "#verano #nuevoproducto #moda #estilo",
    "mediaUrl": "https://cdn.example.com/images/summer-product.jpg",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "marketingPackId": "990e8400-e29b-41d4-a716-446655440004",
    "generatedCopyId": "aa0e8400-e29b-41d4-a716-446655440005"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Content published successfully",
  "data": {
    "jobId": "bb0e8400-e29b-41d4-a716-446655440006",
    "channel": "instagram",
    "status": "Success",
    "publishedUrl": "https://instagram.com/posts/sim_1234567890_xyz",
    "externalPostId": "sim_1234567890_xyz",
    "simulated": true,
    "message": "Publication simulated for instagram (no credentials configured)"
  }
}
```

### Ejemplo 2: Publicar en Facebook

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/publish-content \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "facebook",
    "content": "Descubre nuestro nuevo producto de verano",
    "hashtags": "#verano #nuevoproducto",
    "mediaUrl": "https://cdn.example.com/images/product.jpg",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Content published successfully",
  "data": {
    "jobId": "cc0e8400-e29b-41d4-a716-446655440007",
    "channel": "facebook",
    "status": "Success",
    "publishedUrl": "https://facebook.com/posts/sim_1234567891_abc",
    "externalPostId": "sim_1234567891_abc",
    "simulated": true,
    "message": "Publication simulated for facebook (no credentials configured)"
  }
}
```

### Ejemplo 3: Publicar en TikTok

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/publish-content \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "tiktok",
    "content": "Nuevo producto de verano disponible ahora",
    "hashtags": "#verano #nuevoproducto #moda #tiktok",
    "mediaUrl": "https://cdn.example.com/videos/product-video.mp4",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Content published successfully",
  "data": {
    "jobId": "dd0e8400-e29b-41d4-a716-446655440008",
    "channel": "tiktok",
    "status": "Success",
    "publishedUrl": "https://tiktok.com/posts/sim_1234567892_def",
    "externalPostId": "sim_1234567892_def",
    "simulated": true,
    "message": "Publication simulated for tiktok (no credentials configured)"
  }
}
```

### Ejemplo 4: Publicación Real con Credenciales

**Request (con credenciales configuradas):**
```bash
curl -X POST https://your-n8n-instance.com/webhook/publish-content \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "instagram",
    "content": "Descubre nuestro nuevo producto",
    "hashtags": "#nuevo #producto",
    "mediaUrl": "https://cdn.example.com/images/product.jpg",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002"
  }'
```

**Response (200 OK) - Con credenciales reales:**
```json
{
  "success": true,
  "message": "Content published successfully",
  "data": {
    "jobId": "ee0e8400-e29b-41d4-a716-446655440009",
    "channel": "instagram",
    "status": "Success",
    "publishedUrl": "https://instagram.com/p/ABC123XYZ",
    "externalPostId": "17841405309211850",
    "simulated": false,
    "message": "Successfully published to instagram"
  }
}
```

### Ejemplo 5: Error - Datos Faltantes

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/publish-content \
  -H "Content-Type: application/json" \
  -d '{
    "channel": "instagram"
  }'
```

**Response (500 Internal Server Error):**
```json
{
  "success": false,
  "error": "Publishing error",
  "message": "Failed to publish content",
  "details": {
    "channel": "instagram",
    "error": "Missing required data"
  }
}
```

## Workflow 11: Metrics & Learning

### Ejemplo 1: Guardar Métricas y Aprendizaje Completo

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/metrics-learning \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "marketingPackId": "990e8400-e29b-41d4-a716-446655440004",
    "publishingJobId": "bb0e8400-e29b-41d4-a716-446655440006",
    "channel": "instagram",
    "metrics": {
      "impressions": 1000,
      "clicks": 50,
      "likes": 120,
      "comments": 15,
      "shares": 8,
      "source": "n8n",
      "notes": "Initial metrics from automated publishing"
    },
    "learning": "Content published successfully on Instagram. Engagement rate is 14.3% which is above average. Best posting time appears to be 10:00 AM.",
    "context": {
      "channel": "instagram",
      "publishedAt": "2025-01-01T12:00:00Z",
      "engagementRate": 14.3
    }
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Metrics and learning saved successfully",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "marketingPackId": "990e8400-e29b-41d4-a716-446655440004",
    "publishingJobId": "bb0e8400-e29b-41d4-a716-446655440006",
    "channel": "instagram",
    "metricsSaved": true,
    "jobMetricsSaved": true,
    "learningSaved": true,
    "metricsId": "cc0e8400-e29b-41d4-a716-446655440007",
    "jobMetricsId": "dd0e8400-e29b-41d4-a716-446655440008",
    "memoryId": "ee0e8400-e29b-41d4-a716-446655440009",
    "success": true,
    "message": "Metrics and learning saved successfully"
  }
}
```

### Ejemplo 2: Solo Métricas Iniciales (Sin Aprendizaje)

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/metrics-learning \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "publishingJobId": "bb0e8400-e29b-41d4-a716-446655440006",
    "channel": "facebook",
    "metrics": {
      "impressions": 500,
      "clicks": 25,
      "likes": 40,
      "comments": 5,
      "shares": 3
    }
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Metrics and learning saved successfully",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "publishingJobId": "bb0e8400-e29b-41d4-a716-446655440006",
    "channel": "facebook",
    "metricsSaved": true,
    "jobMetricsSaved": true,
    "learningSaved": false,
    "metricsId": "ff0e8400-e29b-41d4-a716-446655440010",
    "jobMetricsId": "110e8400-e29b-41d4-a716-446655440011",
    "memoryId": null,
    "success": true,
    "message": "Metrics and learning saved successfully"
  }
}
```

### Ejemplo 3: Solo Aprendizaje (Sin Métricas)

**Request:**
```bash
curl -X POST https://your-n8n-instance.com/webhook/metrics-learning \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "learning": "TikTok videos perform better with shorter captions (under 100 characters). Hashtags should be limited to 3-5 for better reach.",
    "context": {
      "channel": "tiktok",
      "insight": "caption-length"
    }
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Metrics and learning saved successfully",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "metricsSaved": false,
    "jobMetricsSaved": false,
    "learningSaved": true,
    "memoryId": "220e8400-e29b-41d4-a716-446655440012",
    "success": true,
    "message": "Metrics and learning saved successfully"
  }
}
```

## Próximos Pasos

Después de guardar métricas y aprendizajes:
1. Las métricas quedan registradas en el backend para análisis
2. Los aprendizajes se guardan en memoria para mejorar futuras generaciones
3. Todo queda asociado a tenant y campaña para tracking completo
4. El sistema puede usar estos aprendizajes en próximas generaciones de contenido
5. Las métricas pueden actualizarse posteriormente con datos reales de las APIs de redes sociales
