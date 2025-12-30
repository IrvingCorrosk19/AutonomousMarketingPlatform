# Configuración de Integración con n8n

## 📋 Resumen

Este documento explica cómo configurar la integración entre el sistema **Autonomous Marketing Platform** y **n8n** para automatizar los flujos de marketing.

---

## 🔄 Flujo de Comunicación

### Sistema → n8n (Trigger)
El sistema ASP.NET Core llama a n8n mediante **HTTP POST** a webhooks específicos.

### n8n → Sistema (Callbacks)
n8n llama de vuelta al sistema mediante **HTTP POST** a endpoints API del backend.

```
┌─────────────┐                    ┌─────┐
│   Sistema   │ ──HTTP POST──>     │ n8n │
│  (ASP.NET)  │ <──HTTP POST──     │     │
└─────────────┘                    └─────┘
     │                                    │
     │ 1. Trigger Workflow                │
     │    POST /webhook/...               │
     │                                    │
     │                                    │ 2. Callback
     │ <──────────────────────────────────│
     │    POST /api/...                   │
```

---

## ⚙️ Configuración en appsettings.json

### 1. Configuración Básica

```json
{
  "N8n": {
    "UseMock": false,
    "BaseUrl": "http://localhost:5678",
    "ApiUrl": "http://localhost:5678/api/v1",
    "ApiKey": "tu-api-key-de-n8n"
  }
}
```

### 2. URLs de Webhooks

Después de importar los workflows en n8n, copia las URLs de los webhooks y configúralas:

```json
{
  "N8n": {
    "WebhookUrls": {
      "MarketingRequest": "http://localhost:5678/webhook/trigger-marketing-request",
      "ValidateConsents": "http://localhost:5678/webhook/validate-consents",
      "LoadMemory": "http://localhost:5678/webhook/load-marketing-memory",
      "AnalyzeInstruction": "http://localhost:5678/webhook/analyze-instruction-ai",
      "GenerateStrategy": "http://localhost:5678/webhook/generate-marketing-strategy",
      "GenerateCopy": "http://localhost:5678/webhook/generate-marketing-copy",
      "GenerateVisualPrompts": "http://localhost:5678/webhook/generate-visual-prompts",
      "BuildMarketingPack": "http://localhost:5678/webhook/build-marketing-pack",
      "HumanApproval": "http://localhost:5678/webhook/human-approval-flow",
      "PublishContent": "http://localhost:5678/webhook/publish-content",
      "MetricsLearning": "http://localhost:5678/webhook/metrics-learning"
    }
  }
}
```

---

## 🚀 Pasos de Configuración

### Paso 1: Instalar y Configurar n8n

1. **Instalar n8n:**
   ```bash
   npm install n8n -g
   # O usando Docker
   docker run -it --rm --name n8n -p 5678:5678 n8nio/n8n
   ```

2. **Acceder a n8n:**
   - Abrir navegador en `http://localhost:5678`
   - Crear cuenta inicial

### Paso 2: Importar Workflows

1. En n8n, ir a **Workflows** → **Import from File**
2. Importar cada archivo JSON de `workflows/n8n/`:
   - `01-trigger-marketing-request.json`
   - `02-validate-consents.json`
   - `03-load-marketing-memory.json`
   - ... (todos los workflows)

### Paso 3: Obtener URLs de Webhooks

1. Para cada workflow importado:
   - Abrir el workflow
   - Hacer clic en el nodo **Webhook**
   - Copiar la **Production URL** (ej: `http://localhost:5678/webhook/trigger-marketing-request`)
   - Activar el workflow (toggle en la esquina superior derecha)

2. Actualizar `appsettings.json` con las URLs reales

### Paso 4: Configurar Credenciales en n8n

1. **OpenAI (si usas OpenAI):**
   - Settings → Credentials → Add Credential
   - Seleccionar "OpenAI API"
   - Ingresar API Key

2. **Variables de Entorno (opcional):**
   - Settings → Environment Variables
   - Agregar:
     - `BACKEND_URL`: `http://localhost:5000` (URL de tu backend)
     - `OPENAI_MODEL`: `gpt-4` (modelo a usar)

### Paso 5: Configurar API Key de n8n (Opcional)

Si quieres consultar el estado de ejecuciones desde el sistema:

1. En n8n: Settings → API
2. Generar API Key
3. Copiar y pegar en `appsettings.json`:
   ```json
   {
     "N8n": {
       "ApiKey": "tu-api-key-aqui"
     }
   }
   ```

---

## 🔧 Modo Mock vs Producción

### Modo Mock (Desarrollo)
```json
{
  "N8n": {
    "UseMock": true
  }
}
```
- No requiere n8n corriendo
- Simula respuestas
- Útil para desarrollo sin n8n

### Modo Producción
```json
{
  "N8n": {
    "UseMock": false,
    "BaseUrl": "https://n8n.tudominio.com",
    "ApiUrl": "https://n8n.tudominio.com/api/v1",
    "ApiKey": "tu-api-key"
  }
}
```
- Requiere n8n corriendo y accesible
- Hace llamadas HTTP reales
- Usa URLs de producción

---

## 📡 Endpoints del Sistema que n8n Llama

El sistema expone estos endpoints que n8n usa:

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/api/consents/check` | GET | Validar consentimientos |
| `/api/memory/context` | GET | Obtener contexto de memoria |
| `/api/marketing-packs` | POST | Guardar MarketingPack |
| `/api/publishing-jobs` | POST | Guardar resultado de publicación |
| `/api/metrics/campaign` | POST | Guardar métricas de campaña |
| `/api/metrics/publishing-job` | POST | Guardar métricas de publicación |
| `/api/memory/save` | POST | Guardar aprendizaje en memoria |

**Nota:** Estos endpoints tienen `[AllowAnonymous]` para desarrollo. En producción, deberías agregar autenticación por API key.

---

## 🔐 Seguridad en Producción

### 1. Autenticación por API Key

Crear middleware para validar API key:

```csharp
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string API_KEY_HEADER = "X-API-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var apiKey = context.Request.Headers[API_KEY_HEADER].FirstOrDefault();
            var validApiKey = context.RequestServices
                .GetRequiredService<IConfiguration>()["N8n:ApiKey"];

            if (apiKey != validApiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }

        await _next(context);
    }
}
```

### 2. Red Privada

En producción, ejecutar n8n y el backend en la misma red privada (VPC) para evitar exponer endpoints públicamente.

---

## 🧪 Testing

### Probar Trigger desde el Sistema

```csharp
var command = new TriggerExternalAutomationCommand
{
    TenantId = tenantId,
    EventType = "marketing.request",
    EventData = new AutomationEventDto
    {
        // ... datos del evento
    }
};

var requestId = await _mediator.Send(command);
```

### Probar Webhook de n8n

```bash
curl -X POST http://localhost:5678/webhook/trigger-marketing-request \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Crear campaña de verano",
    "channels": ["instagram", "facebook"],
    "requiresApproval": true
  }'
```

---

## 📝 Checklist de Configuración

- [ ] n8n instalado y corriendo
- [ ] Workflows importados en n8n
- [ ] Workflows activados en n8n
- [ ] URLs de webhooks copiadas
- [ ] `appsettings.json` actualizado con URLs
- [ ] Credenciales configuradas en n8n (OpenAI, etc.)
- [ ] Variables de entorno configuradas en n8n
- [ ] `UseMock: false` en producción
- [ ] API Key configurada (opcional)
- [ ] Endpoints del backend accesibles desde n8n
- [ ] Autenticación configurada (producción)

---

## 🐛 Troubleshooting

### Error: "No se puede conectar a n8n"
- Verificar que n8n esté corriendo
- Verificar `BaseUrl` en `appsettings.json`
- Verificar firewall/red

### Error: "Webhook not found"
- Verificar que el workflow esté activado en n8n
- Verificar que la URL en `appsettings.json` coincida con la de n8n
- Verificar que el path del webhook sea correcto

### Error: "Timeout"
- Aumentar timeout en `HttpClient` (actualmente 30 segundos)
- Verificar que n8n no esté sobrecargado

---

**Última Actualización:** 2025-01-01

