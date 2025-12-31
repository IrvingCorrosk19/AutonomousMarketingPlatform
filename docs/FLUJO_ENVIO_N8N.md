# 🔍 Flujo Completo: Envío de Datos a n8n

## 📍 Ubicación del Código

### 1. Punto de Entrada: `MarketingRequestController.Create` (POST)

**Archivo:** `src/AutonomousMarketingPlatform.Web/Controllers/MarketingRequestController.cs`  
**Líneas:** 208-229

```csharp
// Preparar datos para el webhook
var eventData = new Dictionary<string, object>
{
    { "instruction", model.Instruction.Trim() },
    { "channels", model.Channels ?? new List<string>() },  // ⚠️ List<string>
    { "requiresApproval", model.RequiresApproval },        // bool
    { "assets", assetsList }                                // List<string>
};

// Disparar webhook a n8n
var requestId = await _automationService.TriggerAutomationAsync(
    effectiveTenantId,
    "marketing.request",
    eventData,        // ← Dictionary<string, object>
    userId,
    model.CampaignId,
    null,
    CancellationToken.None);
```

---

### 2. Construcción del Payload: `ExternalAutomationService.TriggerAutomationAsync`

**Archivo:** `src/AutonomousMarketingPlatform.Infrastructure/Services/ExternalAutomationService.cs`  
**Líneas:** 80-133

```csharp
if (eventType == "marketing.request" && eventData != null)
{
    var eventDataDict = eventData as Dictionary<string, object>;
    if (eventDataDict != null)
    {
        // Obtener channels
        var channelsValue = eventDataDict.GetValueOrDefault("channels") ?? 
                           eventDataDict.GetValueOrDefault("Channels");
        
        // Convertir a List<string>
        List<string> channelsList;
        if (channelsValue is List<string> channels)
        {
            channelsList = channels;
        }
        else if (channelsValue is IEnumerable<string> enumerable)
        {
            channelsList = enumerable.ToList();
        }
        else
        {
            channelsList = new List<string>();
        }
        
        // Validar que no esté vacío
        if (channelsList.Count == 0)
        {
            throw new ArgumentException("channels no puede estar vacío...");
        }
        
        // ⚠️ AQUÍ SE CONSTRUYE EL PAYLOAD
        payload = new Dictionary<string, object>
        {
            { "tenantId", tenantId.ToString() },           // string
            { "userId", userId.Value.ToString() },          // string
            { "instruction", eventDataDict.GetValueOrDefault("instruction")?.ToString() ?? "" },
            { "channels", channelsList },                   // ⚠️ List<string> dentro de Dictionary<string, object>
            { "requiresApproval", eventDataDict.GetValueOrDefault("requiresApproval") ?? false },  // bool
            { "campaignId", relatedEntityId?.ToString() ?? null },  // string o null
            { "assets", eventDataDict.GetValueOrDefault("assets") ?? new List<string>() }  // List<string>
        };
    }
}
```

---

### 3. Serialización JSON: `ExternalAutomationService.TriggerAutomationAsync`

**Archivo:** `src/AutonomousMarketingPlatform.Infrastructure/Services/ExternalAutomationService.cs`  
**Líneas:** 210-218

```csharp
if (payload is Dictionary<string, object> dictPayload)
{
    // ⚠️ AQUÍ SE SERIALIZA
    var jsonContent = JsonSerializer.Serialize(dictPayload, new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    });
    
    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
    
    // ⚠️ AQUÍ SE ENVÍA
    response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);
}
```

---

## ⚠️ PROBLEMA IDENTIFICADO

### El Problema con `System.Text.Json` y `Dictionary<string, object>`

Cuando serializas un `Dictionary<string, object>` que contiene un `List<string>`, `System.Text.Json` puede:

1. **Serializar correctamente** el `List<string>` como array JSON `["item1", "item2"]`
2. **PERO** cuando n8n lo recibe y lo deserializa, puede convertirlo en un `JsonElement` en lugar de un array nativo

### Ejemplo del Problema

**Lo que enviamos:**
```json
{
  "channels": ["instagram", "facebook"]
}
```

**Lo que n8n puede recibir internamente:**
```javascript
{
  "channels": JsonElement { ValueKind: Array, ... }  // ⚠️ No es un array nativo
}
```

**Cuando n8n evalúa:**
```javascript
$json.body.channels.length  // ⚠️ Puede devolver undefined o un string
```

---

## ✅ SOLUCIÓN

### Opción 1: Usar un objeto anónimo en lugar de Dictionary

En lugar de:
```csharp
payload = new Dictionary<string, object> { ... }
```

Usar:
```csharp
payload = new
{
    tenantId = tenantId.ToString(),
    userId = userId.Value.ToString(),
    instruction = ...,
    channels = channelsList,  // ✅ Se serializa directamente como array
    requiresApproval = ...,
    campaignId = relatedEntityId?.ToString(),
    assets = ...
};
```

### Opción 2: Serializar manualmente asegurando tipos correctos

Usar `JsonSerializer.Serialize` con opciones que garanticen arrays correctos.

---

## 🧪 VERIFICACIÓN

Para verificar qué se está enviando exactamente:

1. Revisar los logs del backend (buscar `=== PAYLOAD ENVIADO A N8N ===`)
2. El log muestra el JSON exacto que se envía
3. Comparar con lo que n8n espera recibir

---

## 📋 CHECKLIST

- [ ] ¿`channels` se serializa como array JSON? (`["instagram"]`)
- [ ] ¿`requiresApproval` se serializa como boolean? (`true` o `false`)
- [ ] ¿`campaignId` se omite si es null? (Sí, por `WhenWritingNull`)
- [ ] ¿Los tipos son correctos en n8n después de deserializar?

