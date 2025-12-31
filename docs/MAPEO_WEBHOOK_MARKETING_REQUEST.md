# 🔄 Mapeo: Backend → n8n Webhook Marketing Request

## 📋 Resumen

Este documento muestra **qué espera n8n** y **qué enviamos desde el backend**, con el mapeo completo.

---

## 🎯 Qué Espera n8n (Workflow: `01-trigger-marketing-request.json`)

El workflow de n8n valida los siguientes campos en `$json.body`:

### Campos Requeridos (Validación: `notEmpty`)

| Campo | Tipo | Ubicación en n8n | Validación |
|-------|------|------------------|------------|
| `tenantId` | `string` | `$json.body.tenantId` | `notEmpty` (string) |
| `userId` | `string` | `$json.body.userId` | `notEmpty` (string) |
| `instruction` | `string` | `$json.body.instruction` | `notEmpty` (string) |
| `channels` | `array` | `$json.body.channels` | `notEmpty` (array) |
| `requiresApproval` | `boolean` | `$json.body.requiresApproval` | `notEmpty` (convertido a string) |

### Campos Opcionales

| Campo | Tipo | Ubicación en n8n | Notas |
|-------|------|------------------|-------|
| `campaignId` | `string` o `null` | `$json.body.campaignId` | Se usa `|| null` si no está presente |
| `assets` | `array` | `$json.body.assets` | Se usa `|| []` si no está presente |

### Respuesta de Éxito (200)

```json
{
  "success": true,
  "message": "Request validated successfully",
  "data": {
    "tenantId": "...",
    "userId": "...",
    "campaignId": "..." || null,
    "instruction": "...",
    "assets": [...] || [],
    "channels": [...],
    "requiresApproval": true/false,
    "timestamp": "..."
  }
}
```

### Respuesta de Error (400)

```json
{
  "success": false,
  "error": "Missing required fields",
  "message": "The request must include: tenantId, userId, instruction, channels, and requiresApproval",
  "received": {
    "tenantId": "present" | "missing",
    "userId": "present" | "missing",
    "instruction": "present" | "missing",
    "channels": "present" | "missing",
    "requiresApproval": "present" | "missing"
  }
}
```

---

## 📤 Qué Enviamos desde el Backend

### Código: `ExternalAutomationService.TriggerAutomationAsync`

**Ubicación:** `src/AutonomousMarketingPlatform.Infrastructure/Services/ExternalAutomationService.cs`

**Líneas:** 79-106

### Payload Construido

```csharp
payload = new Dictionary<string, object>
{
    { "tenantId", tenantId.ToString() },                    // Guid → string
    { "userId", userId.Value.ToString() },                  // Guid → string
    { "instruction", eventDataDict.GetValueOrDefault("instruction")?.ToString() ?? "" },
    { "channels", eventDataDict.GetValueOrDefault("channels") ?? new List<string>() },
    { "requiresApproval", eventDataDict.GetValueOrDefault("requiresApproval") ?? false },
    { "campaignId", relatedEntityId?.ToString() ?? null },   // Guid? → string o null
    { "assets", eventDataDict.GetValueOrDefault("assets") ?? new List<string>() }
};
```

### Origen de los Datos

| Campo | Origen | Tipo Original |
|-------|--------|---------------|
| `tenantId` | Parámetro `tenantId` (Guid) | `Guid` → `string` |
| `userId` | Parámetro `userId` (Guid?) | `Guid?` → `string` |
| `instruction` | `eventData["instruction"]` | `string` |
| `channels` | `eventData["channels"]` | `List<string>` |
| `requiresApproval` | `eventData["requiresApproval"]` | `bool` |
| `campaignId` | Parámetro `relatedEntityId` (Guid?) | `Guid?` → `string` o `null` |
| `assets` | `eventData["assets"]` | `List<string>` |

---

## ✅ Mapeo Completo

| Campo n8n | Campo Backend | Conversión | Estado |
|-----------|---------------|------------|--------|
| `body.tenantId` | `tenantId` (Guid) | `ToString()` | ✅ Correcto |
| `body.userId` | `userId` (Guid?) | `Value.ToString()` | ✅ Correcto |
| `body.instruction` | `eventData["instruction"]` | Directo | ✅ Correcto |
| `body.channels` | `eventData["channels"]` | Directo (List<string>) | ✅ Correcto |
| `body.requiresApproval` | `eventData["requiresApproval"]` | Directo (bool) | ✅ Correcto |
| `body.campaignId` | `relatedEntityId` (Guid?) | `ToString()` o `null` | ✅ Correcto |
| `body.assets` | `eventData["assets"]` | Directo (List<string>) | ✅ Correcto |

---

## 📝 Ejemplo de Payload Completo

### Ejemplo 1: Con Campaign ID (Datos Reales)

**Desde:** `N8nConfigController.TestWebhook` o `MarketingRequestController.Create`

```json
{
  "tenantId": "9629f563-c0b6-4570-816e-cdfb0d226167",
  "userId": "532b8976-25e8-4f84-953e-289cec40aebf",
  "campaignId": "73f24df7-644a-4895-865b-0a507926b97e",
  "instruction": "Campaña: swswswsws. wswswsws. Objetivos: 11, 11, Campaña. Audiencia objetivo: Edad: 18-35. Intereses: tecnilogia, maketing. Canales objetivo: instagram, facebook, tiktok",
  "channels": ["instagram", "facebook", "tiktok"],
  "requiresApproval": false,
  "assets": []
}
```

### Ejemplo 2: Sin Campaign ID (Prueba Genérica)

```json
{
  "tenantId": "9629f563-c0b6-4570-816e-cdfb0d226167",
  "userId": "532b8976-25e8-4f84-953e-289cec40aebf",
  "campaignId": null,
  "instruction": "Prueba de webhook desde frontend - Crear contenido de marketing para Instagram",
  "channels": ["instagram"],
  "requiresApproval": false,
  "assets": []
}
```

### Ejemplo 3: Con Assets

```json
{
  "tenantId": "9629f563-c0b6-4570-816e-cdfb0d226167",
  "userId": "532b8976-25e8-4f84-953e-289cec40aebf",
  "campaignId": "73f24df7-644a-4895-865b-0a507926b97e",
  "instruction": "Crear contenido promocional para Instagram",
  "channels": ["instagram"],
  "requiresApproval": true,
  "assets": [
    "https://example.com/image1.jpg",
    "https://example.com/video1.mp4"
  ]
}
```

---

## 🔍 Validaciones en n8n

### Nodo: "Validate Required Fields"

**Condiciones (AND):**

1. ✅ `$json.body.tenantId` → `notEmpty` (string)
2. ✅ `$json.body.userId` → `notEmpty` (string)
3. ✅ `$json.body.instruction` → `notEmpty` (string)
4. ✅ `$json.body.channels` → `notEmpty` (array)
5. ✅ `$json.body.requiresApproval` → `notEmpty` (convertido a string)

**Si todas pasan:** → "Respond - Validation Success" (200)  
**Si alguna falla:** → "Respond - Validation Error" (400)

---

## 🚨 Posibles Problemas y Soluciones

### Problema 1: `tenantId` o `userId` vacíos

**Error n8n:** `"tenantId": "missing"` o `"userId": "missing"`

**Causa:** El backend no está enviando estos campos o están vacíos.

**Solución:** Verificar que `UserHelper.GetTenantId(User)` y `UserHelper.GetUserId(User)` retornen valores válidos.

### Problema 2: `instruction` vacía

**Error n8n:** `"instruction": "missing"`

**Causa:** El `eventData` no contiene `"instruction"` o está vacío.

**Solución:** Asegurar que `eventData["instruction"]` tenga un valor no vacío.

### Problema 3: `channels` vacío o no es array

**Error n8n:** `"channels": "missing"`

**Causa:** El `eventData["channels"]` es `null` o no es un array.

**Solución:** Asegurar que `eventData["channels"]` sea `List<string>` con al menos un elemento.

### Problema 4: `requiresApproval` no es boolean

**Error n8n:** `"requiresApproval": "missing"`

**Causa:** El valor no es un boolean o no está presente.

**Solución:** Asegurar que `eventData["requiresApproval"]` sea `bool` (true/false).

---

## 📊 Flujo Completo

```
1. Usuario → MarketingRequestController.Create
   ↓
2. Construye eventData:
   {
     "instruction": "...",
     "channels": [...],
     "requiresApproval": false,
     "assets": [...]
   }
   ↓
3. Llama: TriggerAutomationAsync(
     tenantId: Guid,
     eventType: "marketing.request",
     eventData: Dictionary,
     userId: Guid?,
     relatedEntityId: Guid? (campaignId),
     ...
   )
   ↓
4. ExternalAutomationService construye payload:
   {
     "tenantId": "...",
     "userId": "...",
     "instruction": "...",
     "channels": [...],
     "requiresApproval": false,
     "campaignId": "..." | null,
     "assets": [...]
   }
   ↓
5. POST a n8n webhook URL
   ↓
6. n8n recibe en: $json.body.*
   ↓
7. n8n valida campos requeridos
   ↓
8. n8n responde: 200 (éxito) o 400 (error)
```

---

## ✅ Checklist de Verificación

- [x] `tenantId` se envía como string (Guid.ToString())
- [x] `userId` se envía como string (Guid.ToString())
- [x] `instruction` es string no vacío
- [x] `channels` es array de strings con al menos un elemento
- [x] `requiresApproval` es boolean (true/false)
- [x] `campaignId` es string o null (Guid?.ToString())
- [x] `assets` es array de strings (puede estar vacío)

---

## 🔗 Referencias

- **Workflow n8n:** `workflows/n8n/01-trigger-marketing-request.json`
- **Servicio Backend:** `src/AutonomousMarketingPlatform.Infrastructure/Services/ExternalAutomationService.cs`
- **Controlador:** `src/AutonomousMarketingPlatform.Web/Controllers/MarketingRequestController.cs`
- **Controlador Prueba:** `src/AutonomousMarketingPlatform.Web/Controllers/N8nConfigController.cs`

