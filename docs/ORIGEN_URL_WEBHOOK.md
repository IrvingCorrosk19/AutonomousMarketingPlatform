# 🔍 Origen de la URL del Webhook - Flujo Completo

## 📊 Flujo de Obtención de la URL

### 1️⃣ **PRIORIDAD 1: Base de Datos (BD)**
**Tabla:** `TenantN8nConfigs`

**Campos relevantes:**
- `WebhookUrlsJson` → JSON con URLs específicas: `{"MarketingRequest": "https://..."}`
- `DefaultWebhookUrl` → URL genérica por defecto
- `BaseUrl` → URL base para construir URLs

**Código:** `ExternalAutomationService.GetN8nConfigAsync()` (línea 474-514)

```csharp
// 1. Buscar en BD
var configs = await _configRepository.FindAsync(
    c => c.TenantId == tenantId && c.IsActive,
    tenantId,
    cancellationToken);

var config = configs.FirstOrDefault();

if (config != null)
{
    // ✅ USA CONFIGURACIÓN DE BD
    return config;
}
```

### 2️⃣ **PRIORIDAD 2: appsettings.json**
**Solo si NO hay configuración en BD**

**Archivo:** `src/AutonomousMarketingPlatform.Web/appsettings.json`

**Sección:**
```json
"N8n": {
  "BaseUrl": "https://n8n.bashpty.com",
  "DefaultWebhookUrl": "https://n8n.bashpty.com/webhook",
  "WebhookUrls": {
    "MarketingRequest": "http://localhost:5678/webhook/trigger-marketing-request"
  }
}
```

**Código:** Línea 506-514
```csharp
// Si no hay configuración en BD, usar valores por defecto de appsettings.json
return new TenantN8nConfig
{
    BaseUrl = _configuration["N8n:BaseUrl"] ?? "https://n8n.bashpty.com",
    DefaultWebhookUrl = _configuration["N8n:DefaultWebhookUrl"] ?? "https://n8n.bashpty.com/webhook/marketing-request",
    WebhookUrlsJson = "{}"
};
```

### 3️⃣ **PRIORIDAD 3: Construcción de URL**
**Método:** `GetWebhookUrlForEventType()` (línea 535-602)

**Orden de prioridad:**
1. ✅ `WebhookUrlsJson["MarketingRequest"]` (de BD o appsettings)
2. ✅ `BuildWebhookUrl(BaseUrl, "marketing-request", DefaultWebhookUrl)` (construye desde BaseUrl)
3. ✅ `DefaultWebhookUrl` (fallback final)

**Código:**
```csharp
"marketing.request" => webhookUrls.GetValueOrDefault("MarketingRequest") 
    ?? BuildWebhookUrl(config.BaseUrl, "marketing-request", config.DefaultWebhookUrl),
```

**Método BuildWebhookUrl:**
```csharp
private string BuildWebhookUrl(string? baseUrl, string eventPath, string defaultWebhookUrl)
{
    if (baseUrl.Contains("n8n.bashpty.com"))
    {
        return $"{baseUrl}/webhook/{eventPath}"; // https://n8n.bashpty.com/webhook/marketing-request
    }
    return defaultWebhookUrl;
}
```

---

## 🎯 **ESTADO ACTUAL (Según logs de producción)**

```
✅ No se encontró configuración de n8n en BD para Tenant 00000000-0000-0000-0000-000000000000
✅ Usando valores por defecto de appsettings.json
✅ BaseUrl = "https://n8n.bashpty.com" (CORRECTO)
✅ BuildWebhookUrl construye: "https://n8n.bashpty.com/webhook/marketing-request" (CORRECTO)
✅ URL del webhook para evento marketing.request: https://n8n.bashpty.com/webhook/marketing-request (desde BuildWebhookUrl(BaseUrl))
✅ POST exitoso: StatusCode=OK, Respuesta 200
```

**✅ CONFIRMADO: El webhook funciona correctamente**

El sistema está usando `BuildWebhookUrl` que construye la URL correcta desde `BaseUrl` cuando no hay configuración en BD.

---

## ✅ **SOLUCIÓN: Configurar en BD**

La mejor solución es **configurar en la Base de Datos** para el tenant de la campaña:

### Opción 1: Desde la Interfaz Web
1. Ir a `/N8nConfig`
2. Seleccionar el tenant de la campaña
3. Configurar:
   - **BaseUrl:** `https://n8n.bashpty.com`
   - **DefaultWebhookUrl:** `https://n8n.bashpty.com/webhook/marketing-request`
   - **WebhookUrls → MarketingRequest:** `https://n8n.bashpty.com/webhook/marketing-request`

### Opción 2: SQL Directo
```sql
UPDATE "TenantN8nConfigs"
SET 
    "BaseUrl" = 'https://n8n.bashpty.com',
    "DefaultWebhookUrl" = 'https://n8n.bashpty.com/webhook/marketing-request',
    "WebhookUrlsJson" = jsonb_set(
        COALESCE("WebhookUrlsJson"::jsonb, '{}'::jsonb),
        '{MarketingRequest}',
        '"https://n8n.bashpty.com/webhook/marketing-request"'::jsonb
    )::text,
    "UpdatedAt" = NOW()
WHERE "TenantId" = '9629f563-c0b6-4570-816e-cdfb0d226167'; -- TenantId de tu campaña
```

---

## 🔍 **VERIFICACIÓN**

Para verificar qué está usando tu sistema, revisa los logs:

```
✅ "Configuración de n8n cargada desde BD" → Usa BD
❌ "No se encontró configuración de n8n en BD" → Usa appsettings.json
```

