# Análisis: Payload Backend vs n8n Workflow

## 🔍 Problema
Hay errores en el workflow de n8n a pesar de que sabemos qué se envía. Necesitamos verificar la correspondencia exacta.

---

## 📤 LO QUE ENVÍA EL BACKEND

### Código: `ExternalAutomationService.TriggerAutomationAsync`

**Ubicación:** `src/AutonomousMarketingPlatform.Infrastructure/Services/ExternalAutomationService.cs:93-106`

```csharp
payload = new Dictionary<string, object>
{
    { "tenantId", tenantId.ToString() },                    // Guid → string
    { "userId", userId.Value.ToString() },                  // Guid → string
    { "instruction", eventDataDict.GetValueOrDefault("instruction")?.ToString() ?? "" },
    { "channels", eventDataDict.GetValueOrDefault("channels") ?? new List<string>() },
    { "requiresApproval", eventDataDict.GetValueOrDefault("requiresApproval") ?? false },
    { "campaignId", relatedEntityId?.ToString() ?? null },   // ⚠️ Puede ser null
    { "assets", eventDataDict.GetValueOrDefault("assets") ?? new List<string>() }
};
```

### Serialización JSON

**Línea 155-159:**
```csharp
var jsonContent = JsonSerializer.Serialize(dictPayload, new JsonSerializerOptions
{
    WriteIndented = false,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull  // ⚠️ Omite nulls
});
```

### Ejemplo de JSON Enviado

**Caso 1: Con campaignId**
```json
{
  "tenantId": "73f24df7-644a-4895-865b-0a507926b97e",
  "userId": "7022d212-35a5-4a12-8d52-f11f4f0cff4c",
  "instruction": "Crear contenido para Instagram",
  "channels": ["instagram"],
  "requiresApproval": false,
  "campaignId": "73f24df7-644a-4895-865b-0a507926b97e",
  "assets": []
}
```

**Caso 2: Sin campaignId (null se omite)**
```json
{
  "tenantId": "73f24df7-644a-4895-865b-0a507926b97e",
  "userId": "7022d212-35a5-4a12-8d52-f11f4f0cff4c",
  "instruction": "Crear contenido para Instagram",
  "channels": ["instagram"],
  "requiresApproval": false,
  "assets": []
}
```
⚠️ **Nota:** `campaignId` NO aparece en el JSON si es `null` debido a `WhenWritingNull`.

---

## 📥 LO QUE ESPERA N8N

### Estructura del Webhook en n8n

Cuando n8n recibe un POST con `Content-Type: application/json`, los datos están en:
- `$json.body` → Contiene el objeto JSON completo enviado

### Nodo "Normalize Payload" (Líneas 23-64)

```javascript
// Lee de $json.body.*
tenantId: $json.body.tenantId
userId: $json.body.userId
instruction: $json.body.instruction
campaignId: $json.body.campaignId || null  // ⚠️ Si no existe, usa null
requiresApproval: Boolean($json.body.requiresApproval)
channelsNormalized: Array.isArray($json.body.channels) ? $json.body.channels : ...
assets: Array.isArray($json.body.assets) ? $json.body.assets : ...
```

### Nodo "Validate Required Fields" (Líneas 84-130)

Valida:
1. `tenantId` → `notEmpty`
2. `userId` → `notEmpty`
3. `instruction` → `notEmpty`
4. `channelsNormalized.length` → `> 0` (number)
5. `requiresApproval` → `notEmpty` (como string)

---

## ⚠️ PROBLEMAS POTENCIALES IDENTIFICADOS

### 1. **CampaignId null se omite**
- **Backend:** Si `campaignId` es `null`, NO se incluye en el JSON (por `WhenWritingNull`)
- **n8n:** Lee `$json.body.campaignId || null`, lo cual debería funcionar
- **Estado:** ✅ OK (n8n maneja campos faltantes con `|| null`)

### 2. **Channels como array vacío**
- **Backend:** Puede enviar `"channels": []` si `model.Channels` es null o vacío
- **n8n:** Valida `channelsNormalized.length > 0`
- **Estado:** ⚠️ **PROBLEMA POTENCIAL** - Si `channels` es `[]`, la validación fallará

### 3. **RequiresApproval como boolean**
- **Backend:** Envía `"requiresApproval": false` (boolean)
- **n8n:** Convierte a boolean con `Boolean($json.body.requiresApproval)`, luego valida como string `String($json.requiresApproval)`
- **Estado:** ✅ OK (conversión correcta)

### 4. **Assets como array vacío**
- **Backend:** Puede enviar `"assets": []`
- **n8n:** No valida assets (es opcional)
- **Estado:** ✅ OK

---

## 🔧 VERIFICACIÓN NECESARIA

### ¿Qué errores específicos está dando n8n?

1. **Error de validación de channels?**
   - Si `channels` viene como `[]`, `channelsNormalized.length` será `0`
   - La validación `> 0` fallará
   - **Solución:** El backend debe garantizar que `channels` nunca sea vacío

2. **Error de tipo en channels?**
   - Si `channels` no es un array en el JSON recibido
   - **Solución:** Ya corregido con `channelsNormalized`

3. **Error en requiresApproval?**
   - Si `requiresApproval` no está presente
   - **Solución:** El backend siempre lo envía (default: `false`)

---

## ✅ RECOMENDACIONES

### 1. Garantizar que channels nunca sea vacío

**En `MarketingRequestController.cs` (línea 129-132):**
```csharp
// Validar que al menos un canal esté seleccionado
if (model.Channels == null || model.Channels.Count == 0)
{
    ModelState.AddModelError("Channels", "Debes seleccionar al menos un canal de publicación.");
}
```
✅ **Ya está validado en el frontend/backend**

### 2. Verificar que el payload se serializa correctamente

**Problema potencial:** `List<string>` puede no serializarse como array JSON si hay algún problema.

**Solución:** Agregar logging detallado (ya agregado en líneas 162-190)

### 3. Verificar estructura de n8n

**Pregunta:** ¿n8n envuelve el JSON en otra estructura?

**Respuesta:** No, si el Content-Type es `application/json`, los datos están directamente en `$json.body`.

---

## 🧪 PRUEBA MANUAL

Para verificar exactamente qué recibe n8n:

1. Enviar una solicitud de marketing
2. Revisar logs del backend (buscar `=== PAYLOAD ENVIADO A N8N ===`)
3. En n8n, agregar un nodo "Code" después del webhook para imprimir `$json`
4. Comparar ambos

---

## 📋 CHECKLIST DE VERIFICACIÓN

- [ ] ¿El backend envía `channels` como array? (Sí, `List<string>`)
- [ ] ¿El backend envía `channels` vacío? (Puede, pero está validado)
- [ ] ¿n8n recibe `channels` como array? (Depende de la serialización)
- [ ] ¿La validación de `channelsNormalized.length > 0` funciona? (Sí, si channels no está vacío)
- [ ] ¿`campaignId` null causa problemas? (No, n8n maneja campos faltantes)
- [ ] ¿`requiresApproval` se valida correctamente? (Sí, se convierte a boolean y luego a string)

---

## 🎯 CONCLUSIÓN

El problema más probable es:
1. **Channels vacío:** Si el backend envía `"channels": []`, la validación fallará
2. **Serialización incorrecta:** Si `List<string>` no se serializa como array JSON

**Siguiente paso:** Verificar los logs del backend para ver el payload exacto enviado.

