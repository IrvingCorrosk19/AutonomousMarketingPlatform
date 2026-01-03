# Workflows n8n - Autonomous Marketing Platform

Este directorio contiene los workflows de n8n para la automatización del sistema de marketing autónomo.

## Estructura

Cada workflow es un archivo JSON independiente que puede ser importado directamente en n8n.

## Workflows Disponibles

### 01-trigger-marketing-request.json
**Trigger - Marketing Request**

Workflow de entrada que recibe y valida solicitudes de marketing desde el backend.

**Funcionalidad:**
- Recibe solicitudes POST mediante Webhook
- Valida campos requeridos: `tenantId`, `userId`, `instruction`, `channels`, `requiresApproval`
- Responde con error 400 si faltan campos
- Responde con éxito 200 si la validación es correcta
- Prepara los datos validados para el siguiente paso

**Endpoint:** `POST /webhook/marketing-request`

**Payload de Ejemplo:**
```json
{
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "660e8400-e29b-41d4-a716-446655440001",
  "campaignId": "770e8400-e29b-41d4-a716-446655440002",
  "instruction": "Crear una campaña de verano para Instagram y Facebook",
  "assets": ["https://example.com/image1.jpg", "https://example.com/image2.jpg"],
  "channels": ["instagram", "facebook"],
  "requiresApproval": true
}
```

**Respuesta de Éxito (200):**
```json
{
  "success": true,
  "message": "Request validated successfully",
  "data": {
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "instruction": "Crear una campaña de verano para Instagram y Facebook",
    "assets": ["https://example.com/image1.jpg"],
    "channels": ["instagram", "facebook"],
    "requiresApproval": true,
    "timestamp": "2025-01-01T12:00:00.000Z"
  }
}
```

**Respuesta de Error (400):**
```json
{
  "success": false,
  "error": "Missing required fields",
  "message": "The request must include: tenantId, userId, instruction, channels, and requiresApproval",
  "received": {
    "tenantId": "present",
    "userId": "missing",
    "instruction": "present",
    "channels": "present",
    "requiresApproval": "missing"
  }
}
```

## Instalación

1. Importar el archivo JSON en n8n:
   - Abrir n8n
   - Ir a "Workflows" > "Import from File"
   - Seleccionar el archivo JSON del workflow

2. Configurar el Webhook:
   - El webhook se activará automáticamente al activar el workflow
   - Copiar la URL del webhook generada
   - Configurar esta URL en el backend como endpoint para enviar solicitudes

3. Activar el workflow:
   - Activar el workflow en n8n
   - Verificar que el webhook esté activo

## Integración con Backend

El backend debe enviar solicitudes HTTP POST al endpoint del webhook con el payload especificado.

Ejemplo en C#:
```csharp
var payload = new
{
    tenantId = tenantId.ToString(),
    userId = userId.ToString(),
    campaignId = campaignId?.ToString(),
    instruction = instruction,
    assets = assets,
    channels = channels,
    requiresApproval = requiresApproval
};

var response = await httpClient.PostAsJsonAsync(webhookUrl, payload);
```

## Notas de Arquitectura

- **Multi-tenant estricto**: Todos los workflows validan y respetan el `tenantId`
- **Modularidad**: Cada workflow es independiente y puede ejecutarse por separado
- **Trazabilidad**: Cada solicitud incluye un `requestId` único para seguimiento
- **Robustez**: Validación exhaustiva de campos antes de procesar
- **Escalabilidad**: Diseñado para manejar múltiples solicitudes concurrentes

### 02-validate-consents.json
**Validate Consents**

Workflow que valida los consentimientos del usuario antes de continuar con el flujo de marketing.

**Funcionalidad:**
- Recibe `tenantId` y `userId` mediante Webhook POST
- Consulta al backend el estado de los consentimientos del usuario
- Valida que el usuario tenga:
  - Consentimiento para uso de IA (`aiConsent`)
  - Consentimiento para publicación (`publishingConsent`)
- Si alguno es falso, responde con error 403
- Si ambos son verdaderos, continúa el flujo

**Endpoint:** `POST /webhook/validate-consents`

**Payload de Entrada:**
```json
{
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "660e8400-e29b-41d4-a716-446655440001"
}
```

**Respuesta del Backend (GET /api/ConsentsApi/check):**
```json
{
  "aiConsent": true,
  "publishingConsent": true
}
```

**Respuesta de Éxito (200):**
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

**Respuesta de Error - Consentimientos Faltantes (403):**
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

**Respuesta de Error - Backend (500):**
```json
{
  "success": false,
  "error": "Backend error",
  "message": "Failed to check consents from backend",
  "details": {
    "statusCode": 500,
    "error": "Internal server error"
  }
}
```

**Notas:**
- El workflow consulta al backend mediante HTTP GET a `/api/ConsentsApi/check?tenantId=xxx&userId=yyy`
- Requiere que el backend tenga configurada la variable de entorno `BACKEND_URL` o usa `http://localhost:56610` por defecto
- Valida tanto la respuesta HTTP como los valores de los consentimientos

### 03-load-marketing-memory.json
**Load Marketing Memory**

Workflow que carga la memoria histórica de marketing de un tenant para usar en la generación de contenido.

**Funcionalidad:**
- Recibe `tenantId` mediante Webhook POST
- Consulta al backend el contexto de memoria del tenant
- Normaliza la respuesta para extraer:
  - Preferencias del usuario (tono, formatos, etc.)
  - Aprendizajes del sistema (canales que mejor funcionan)
  - Restricciones y limitaciones
- Retorna la memoria normalizada en formato estándar

**Endpoint:** `POST /webhook/load-marketing-memory`

**Payload de Entrada:**
```json
{
  "tenantId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Respuesta del Backend (GET /api/MemoryApi/context?tenantId=xxx):**
```json
{
  "preferences": {
    "preferredTone": "profesional",
    "dislikedFormats": ["texto largo"]
  },
  "learnings": {
    "bestPerformingChannels": ["instagram", "facebook"]
  },
  "restrictions": [],
  "userPreferences": [...],
  "recentConversations": [...],
  "campaignMemories": [...],
  "learningsList": [...],
  "summarizedContext": "..."
}
```

**Respuesta de Éxito (200):**
```json
{
  "success": true,
  "message": "Marketing memory loaded successfully",
  "data": {
    "preferredTone": "profesional",
    "dislikedFormats": ["texto largo"],
    "bestPerformingChannels": ["instagram"],
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

**Notas:**
- El workflow consulta al backend mediante HTTP GET a `/api/MemoryApi/context?tenantId=xxx`
- Normaliza la respuesta para facilitar su uso en workflows posteriores
- Extrae automáticamente preferencias, aprendizajes y restricciones de la memoria

### 04-analyze-instruction-ai.json
**Analyze Instruction AI**

Workflow que analiza la instrucción del usuario usando OpenAI para extraer información estructurada.

**Funcionalidad:**
- Recibe la instrucción del usuario mediante Webhook POST
- Analiza el texto usando OpenAI Chat Completion
- Extrae información estructurada:
  - Objetivo principal
  - Tono recomendado
  - Nivel de urgencia
  - Tipo de contenido recomendado
  - Audiencia objetivo (si se menciona)
  - Mensajes clave
  - Hashtags sugeridos
  - Canales recomendados
- Retorna un JSON estructurado, no texto libre

**Endpoint:** `POST /webhook/analyze-instruction`

**Payload de Entrada:**
```json
{
  "instruction": "Crear una campaña urgente para promocionar nuestro nuevo producto de verano en Instagram. Tono profesional pero amigable, dirigido a jóvenes de 18-35 años.",
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "660e8400-e29b-41d4-a716-446655440001"
}
```

**Respuesta de Éxito (200):**
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
    "originalInstruction": "Crear una campaña urgente...",
    "analyzedAt": "2025-01-01T12:00:00.000Z",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001"
  }
}
```

**Notas:**
- Usa OpenAI GPT-4 (configurable mediante variable de entorno `OPENAI_MODEL`)
- Requiere credenciales de OpenAI API configuradas en n8n
- El prompt está optimizado para devolver JSON estructurado
- Incluye manejo de errores y valores por defecto
- Parsea automáticamente la respuesta de OpenAI incluso si viene con markdown

### 05-generate-marketing-strategy.json
**Generate Marketing Strategy**

Workflow que genera una estrategia de marketing completa combinando el análisis de la instrucción del usuario con la memoria histórica del tenant.

**Funcionalidad:**
- Recibe el análisis de instrucción y la memoria de marketing mediante Webhook POST
- Genera una estrategia completa usando OpenAI
- Combina información del análisis y memoria histórica
- Retorna una estrategia estructurada con:
  - Mensaje principal
  - Call to Action (CTA)
  - Formato recomendado
  - Horario sugerido (días y horas óptimas)
  - Estructura de contenido (headline, body, hashtags, mentions)
  - Canales recomendados
  - Tono
  - Audiencia objetivo
  - Puntos clave

**Endpoint:** `POST /webhook/generate-marketing-strategy`

**Payload de Entrada:**
```json
{
  "analysis": {
    "objective": "Promocionar nuevo producto de verano",
    "tone": "profesional-amigable",
    "urgency": "high",
    "contentType": "post",
    "targetAudience": "jóvenes de 18-35 años",
    "keyMessages": ["Nuevo producto", "Verano"],
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
}
```

**Respuesta de Éxito (200):**
```json
{
  "success": true,
  "message": "Marketing strategy generated successfully",
  "data": {
    "mainMessage": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos",
    "cta": "Compra ahora y obtén 20% de descuento",
    "recommendedFormat": "post",
    "suggestedSchedule": {
      "bestDays": ["lunes", "miércoles", "viernes"],
      "bestTimes": ["09:00", "13:00", "18:00"],
      "timezone": "UTC"
    },
    "contentStructure": {
      "headline": "Nuevo Producto de Verano",
      "body": "Perfecto para jóvenes que buscan estilo y comodidad...",
      "hashtags": ["verano", "nuevoproducto", "moda", "jovenes"],
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
      "generatedAt": "2025-01-01T12:00:00.000Z",
      "basedOnAnalysis": {...},
      "basedOnMemory": {...}
    }
  }
}
```

**Notas:**
- Usa OpenAI GPT-4 (configurable mediante variable de entorno `OPENAI_MODEL`)
- Requiere credenciales de OpenAI API configuradas en n8n
- Combina inteligentemente el análisis de instrucción con la memoria histórica
- Genera horarios sugeridos basados en mejores prácticas de marketing
- Incluye estructura completa de contenido lista para usar
- Temperatura: 0.7 (balance entre creatividad y consistencia)

### 06-generate-marketing-copy.json
**Generate Marketing Copy**

Workflow que genera copy de marketing listo para publicar, incluyendo variantes A/B para testing.

**Funcionalidad:**
- Recibe la estrategia de marketing mediante Webhook POST
- Genera copy completo usando OpenAI
- Crea múltiples versiones:
  - Copy corto (máximo 125 caracteres, ideal para stories/tweets)
  - Copy largo (máximo 500 caracteres, ideal para posts)
  - Hashtags optimizados
  - Variantes A/B para testing
- Formatea el copy para diferentes canales (Instagram, Facebook, Twitter)
- Retorna copy listo para publicar sin ediciones

**Endpoint:** `POST /webhook/generate-marketing-copy`

**Payload de Entrada:**
```json
{
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
}
```

**Respuesta de Éxito (200):**
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
        "longCopy": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos que buscan estilo y comodidad. Perfecto para tus aventuras de verano. Compra ahora y obtén 20% de descuento en tu primera compra.",
        "hashtags": ["verano", "nuevoproducto", "moda"]
      },
      "variantB": {
        "shortCopy": "Nuevo producto de verano disponible. Oferta especial: 20% OFF",
        "longCopy": "Lanzamos nuestro nuevo producto de verano pensado en ti. Diseñado para jóvenes que valoran el estilo y la funcionalidad. No te pierdas esta oferta especial: 20% de descuento en tu primera compra.",
        "hashtags": ["verano", "oferta", "nuevo", "moda"]
      }
    },
    "headline": "Nuevo Producto de Verano - Estilo y Comodidad",
    "cta": "Compra ahora y obtén 20% de descuento",
    "emojiSuggestions": ["☀️", "🏖️", "👕"],
    "mentions": [],
    "publishFormat": {
      "instagram": {
        "caption": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos...\n\n#verano #nuevoproducto #moda #jovenes #estilo",
        "storyText": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento",
        "hashtags": ["verano", "nuevoproducto", "moda", "jovenes", "estilo"]
      },
      "facebook": {
        "post": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos...",
        "hashtags": ["verano", "nuevoproducto", "moda"]
      },
      "twitter": {
        "tweet": "Descubre nuestro nuevo producto de verano. Compra ahora con 20% de descuento",
        "thread": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos...",
        "hashtags": ["verano", "nuevoproducto"]
      }
    },
    "metadata": {
      "tenantId": "550e8400-e29b-41d4-a716-446655440000",
      "generatedAt": "2025-01-01T12:00:00.000Z",
      "format": "post",
      "channels": ["instagram", "facebook"],
      "tone": "profesional-amigable"
    }
  }
}
```

**Notas:**
- Usa OpenAI GPT-4 (configurable mediante variable de entorno `OPENAI_MODEL`)
- Requiere credenciales de OpenAI API configuradas en n8n
- Genera copy optimizado para engagement
- Incluye variantes A/B para testing de rendimiento
- Formatea automáticamente para diferentes canales
- Copy listo para copiar y pegar directamente
- Temperatura: 0.8 (más creativo para copy)

### 07-generate-visual-prompts.json
**Generate Visual Prompts**

Workflow que genera prompts optimizados para IA visual (imágenes y videos) usando el contexto de campaña y memoria.

**Funcionalidad:**
- Recibe estrategia, copy y memoria mediante Webhook POST
- Genera prompts optimizados para generación de imágenes y videos con IA
- Considera el contexto completo de la campaña
- Retorna prompts detallados y técnicos listos para usar en herramientas de IA visual

**Endpoint:** `POST /webhook/generate-visual-prompts`

**Payload de Entrada:**
```json
{
  "strategy": {
    "mainMessage": "Descubre nuestro nuevo producto de verano",
    "tone": "profesional-amigable",
    "targetAudience": "jóvenes de 18-35 años",
    "recommendedFormat": "post",
    "channels": ["instagram", "facebook"]
  },
  "copy": {
    "longCopy": "Descubre nuestro nuevo producto de verano...",
    "headline": "Nuevo Producto de Verano"
  },
  "memory": {
    "preferredTone": "profesional",
    "bestPerformingChannels": ["instagram"]
  },
  "tenantId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Respuesta de Éxito (200):**
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
    },
    "metadata": {
      "tenantId": "550e8400-e29b-41d4-a716-446655440000",
      "generatedAt": "2025-01-01T12:00:00.000Z",
      "format": "post",
      "channels": ["instagram", "facebook"],
      "tone": "profesional-amigable"
    }
  }
}
```

**Notas:**
- Usa OpenAI GPT-4 (configurable mediante variable de entorno `OPENAI_MODEL`)
- Requiere credenciales de OpenAI API configuradas en n8n
- Genera prompts detallados y técnicos optimizados para DALL-E, Midjourney, Stable Diffusion
- Incluye especificaciones técnicas (resolución, calidad, iluminación, composición)
- Considera el formato recomendado para determinar aspect ratio
- Temperatura: 0.7 (balance entre creatividad y precisión técnica)

### 08-build-marketing-pack.json
**Build Marketing Pack**

Workflow que unifica todos los componentes generados (estrategia, copy, prompts visuales, media, canales) en un MarketingPack completo listo para aprobación y publicación.

**Funcionalidad:**
- Recibe todos los componentes generados mediante Webhook POST
- Unifica estrategia, copy, prompts visuales, media y canales
- Construye un MarketingPack estructurado con:
  - Estrategia completa
  - GeneratedCopies (copy largo, corto, variantes A/B)
  - MarketingAssetPrompts (imagen y video)
  - Información de aprobación
  - Información de publicación
- Valida componentes mínimos requeridos
- Retorna MarketingPack listo para aprobación/publicación

**Endpoint:** `POST /webhook/build-marketing-pack`

**Payload de Entrada:**
```json
{
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
    "longCopy": "Descubre nuestro nuevo producto de verano diseñado para jóvenes activos...",
    "hashtags": ["verano", "nuevoproducto", "moda"],
    "variants": {
      "variantA": {
        "shortCopy": "...",
        "longCopy": "...",
        "hashtags": [...]
      },
      "variantB": {
        "shortCopy": "...",
        "longCopy": "...",
        "hashtags": [...]
      }
    },
    "publishFormat": {
      "instagram": {
        "caption": "...",
        "storyText": "..."
      }
    }
  },
  "visualPrompts": {
    "imagePrompt": "High-quality marketing image...",
    "videoPrompt": "Marketing video showcasing...",
    "imageStyle": "professional",
    "aspectRatio": "1:1"
  },
  "media": [
    "https://example.com/image1.jpg",
    "https://example.com/video1.mp4"
  ],
  "channels": ["instagram", "facebook"],
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "660e8400-e29b-41d4-a716-446655440001",
  "campaignId": "770e8400-e29b-41d4-a716-446655440002",
  "contentId": "880e8400-e29b-41d4-a716-446655440003",
  "requiresApproval": true
}
```

**Respuesta de Éxito (200):**
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
    "strategy": "{...}",
    "status": "Generated",
    "version": 1,
    "metadata": "{...}",
    "copies": [
      {
        "id": "...",
        "copyType": "long",
        "content": "Descubre nuestro nuevo producto...",
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
        "id": "...",
        "copyType": "variant-a",
        "content": "...",
        "hashtags": "...",
        "publicationChecklist": {
          "hasCopy": true,
          "isVariant": true,
          "variantType": "A"
        }
      }
    ],
    "assetPrompts": [
      {
        "id": "...",
        "assetType": "image",
        "prompt": "High-quality marketing image...",
        "parameters": {
          "style": "professional",
          "aspectRatio": "1:1"
        },
        "suggestedChannel": "instagram"
      },
      {
        "id": "...",
        "assetType": "video",
        "prompt": "Marketing video showcasing...",
        "parameters": {
          "aspectRatio": "16:9"
        }
      }
    ],
    "channels": ["instagram", "facebook"],
    "media": ["https://example.com/image1.jpg"],
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
          "caption": "...",
          "storyText": "..."
        }
      },
      "channels": [
        {
          "name": "instagram",
          "copy": "...",
          "format": "post",
          "ready": true
        }
      ]
    }
  }
}
```

**Notas:**
- Valida que existan estrategia y copy como componentes mínimos
- Genera IDs únicos para el pack y todos sus componentes
- Construye GeneratedCopies desde el copy (largo, corto, variantes A/B)
- Construye MarketingAssetPrompts desde los prompts visuales
- Incluye checklist de aprobación y publicación
- Status inicial: "Generated" si requiere aprobación, "Ready" si no
- Listo para enviar al backend para guardar en base de datos

### 09-human-approval-flow.json
**Human Approval Flow**

Workflow que maneja el flujo de aprobación humana del MarketingPack. Decide si el pack requiere aprobación o puede continuar directamente a publicación.

**Funcionalidad:**
- Recibe el MarketingPack generado mediante Webhook POST
- Verifica si `requiresApproval` es `true` o `false`
- Si `requiresApproval = true`:
  - Envía el pack al backend mediante HTTP POST
  - Marca el estado como "RequiresApproval"
  - Detiene el flujo (responde y termina)
- Si `requiresApproval = false`:
  - Prepara el pack para publicación
  - Continúa el flujo (responde con `nextStep: "publish"`)

**Endpoint:** `POST /webhook/human-approval-flow`

**Payload de Entrada:**
```json
{
  "marketingPack": {
    "id": "990e8400-e29b-41d4-a716-446655440004",
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "contentId": "880e8400-e29b-41d4-a716-446655440003",
    "campaignId": "770e8400-e29b-41d4-a716-446655440002",
    "strategy": "{...}",
    "status": "Generated",
    "copies": [...],
    "assetPrompts": [...],
    "requiresApproval": true
  }
}
```

**Respuesta - Aprobación Requerida (200):**
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

**Respuesta - Listo para Publicar (200):**
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
    "marketingPack": {...}
  }
}
```

**Notas:**
- Usa IF Node para bifurcar el flujo según `requiresApproval`
- Si requiere aprobación, envía al backend mediante HTTP POST a `/api/marketing-packs`
- El estado se marca como "RequiresApproval" en el backend
- Si no requiere aprobación, el pack se pasa al siguiente workflow para publicación
- Requiere que el backend tenga configurada la variable de entorno `BACKEND_URL` o usa `http://localhost:56610` por defecto

### 10-publish-content.json
**Publish Content**

Workflow que publica contenido en redes sociales (Instagram, Facebook, TikTok) y guarda el resultado en el backend.

**Funcionalidad:**
- Recibe solicitud de publicación mediante Webhook POST
- Valida datos requeridos (canal, contenido)
- Usa IF Nodes para bifurcar según el canal (Instagram, Facebook, TikTok)
- Publica en el canal correspondiente (simula si no hay credenciales)
- Procesa el resultado (real o simulado)
- Guarda el resultado en el backend mediante HTTP POST

**Endpoint:** `POST /webhook/publish-content`

**Payload de Entrada:**
```json
{
  "channel": "instagram",
  "content": "Descubre nuestro nuevo producto de verano",
  "hashtags": "#verano #nuevoproducto #moda",
  "mediaUrl": "https://cdn.example.com/images/product.jpg",
  "tenantId": "550e8400-e29b-41d4-a716-446655440000",
  "campaignId": "770e8400-e29b-41d4-a716-446655440002",
  "marketingPackId": "990e8400-e29b-41d4-a716-446655440004",
  "generatedCopyId": "aa0e8400-e29b-41d4-a716-446655440005",
  "scheduledDate": "2025-01-15T10:00:00Z"
}
```

**Respuesta de Éxito (200):**
```json
{
  "success": true,
  "message": "Content published successfully",
  "data": {
    "jobId": "bb0e8400-e29b-41d4-a716-446655440006",
    "channel": "instagram",
    "status": "Success",
    "publishedUrl": "https://instagram.com/posts/abc123",
    "externalPostId": "sim_1234567890_xyz",
    "simulated": true,
    "message": "Publication simulated for instagram (no credentials configured)"
  }
}
```

**Canales Soportados:**
- **Instagram**: Publica imágenes con caption y hashtags
- **Facebook**: Publica posts con mensaje y enlace
- **TikTok**: Publica videos con caption y hashtags

**Simulación:**
- Si no hay credenciales configuradas o la API falla, el workflow simula una publicación exitosa
- Genera IDs y URLs simulados para testing
- Marca el resultado como `simulated: true`

**Notas:**
- Usa IF Nodes para bifurcar según el canal
- Requiere variable de entorno `BACKEND_URL` (default: `http://localhost:56610`)
- Variables opcionales para APIs reales:
  - `INSTAGRAM_API_URL`, `INSTAGRAM_ACCESS_TOKEN`
  - `FACEBOOK_API_URL`, `FACEBOOK_ACCESS_TOKEN`
  - `TIKTOK_API_URL`, `TIKTOK_ACCESS_TOKEN`
- Guarda el resultado en `/api/publishing-jobs`

### 11-metrics-learning.json
**Metrics & Learning**

Workflow que guarda métricas iniciales y aprendizajes en memoria al final del proceso de marketing autónomo.

**Funcionalidad:**
- Recibe resultados del proceso completo mediante Webhook POST
- Guarda métricas iniciales de campaña
- Guarda métricas iniciales del publishing job
- Guarda aprendizajes en memoria de marketing
- Asocia resultados a tenant y campaña
- Consolida todos los resultados

**Endpoint:** `POST /webhook/metrics-learning`

**Payload de Entrada:**
```json
{
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
  "learning": "Content published successfully on Instagram. Engagement rate is 14.3% which is above average.",
  "context": {
    "channel": "instagram",
    "publishedAt": "2025-01-01T12:00:00Z"
  }
}
```

**Respuesta de Éxito (200):**
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

**Endpoints del Backend:**
- `POST /api/metrics/campaign` - Guarda métricas de campaña
- `POST /api/metrics/publishing-job` - Guarda métricas de publicación
- `POST /api/MemoryApi/save` - Guarda aprendizaje en memoria

**Notas:**
- Se ejecuta al final del proceso de marketing autónomo
- Guarda métricas iniciales (pueden actualizarse después con datos reales)
- Guarda aprendizajes para mejorar futuras generaciones
- Asocia todo a tenant y campaña para tracking
- Requiere variable de entorno `BACKEND_URL` (default: `http://localhost:56610`)

## Próximos Workflows

- 12-asset-creation.json - Creación de assets visuales usando los prompts
- 13-publishing-scheduler.json - Programación de publicaciones
- 14-workflow-orchestrator.json - Orquestador principal que conecta todos los workflows

