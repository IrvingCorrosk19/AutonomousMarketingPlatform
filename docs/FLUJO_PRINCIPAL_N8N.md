# 🔄 Flujo Principal de Conexión n8n con el Sistema

## 📍 Flujo Principal: **"Trigger - Marketing Request"**

Este es el **primer workflow** que debes configurar porque es el **punto de entrada** desde tu sistema hacia n8n.

---

## 🎯 ¿Cómo Funciona la Conexión?

### 1. Tu Sistema → n8n

Tu sistema ASP.NET Core llama a n8n cuando:
- Un usuario solicita crear contenido de marketing
- Se dispara una automatización
- Se necesita generar estrategia de marketing

**Código en tu sistema:**
```csharp
// El sistema llama a n8n usando IExternalAutomationService
await _automationService.TriggerAutomationAsync(
    tenantId: tenantId,
    eventType: "marketing.request",  // ← Este mapea al webhook
    eventData: { ... },
    userId: userId
);
```

**Mapeo de Eventos:**
El sistema mapea `eventType` a URLs de webhooks en `appsettings.json`:

```json
{
  "N8n": {
    "WebhookUrls": {
      "MarketingRequest": "http://localhost:5678/webhook/XXXXX",
      "ValidateConsents": "http://localhost:5678/webhook/XXXXX",
      // ... otros
    }
  }
}
```

### 2. n8n → Tu Sistema

n8n llama de vuelta a tu sistema para:
- Validar consentimientos
- Cargar memoria de marketing
- Guardar MarketingPacks
- Guardar métricas
- Guardar resultados de publicaciones

**Endpoints que n8n llama:**
- `POST /api/consents/check` - Validar consentimientos
- `GET /api/memory/context` - Cargar memoria
- `POST /api/marketing-packs` - Guardar MarketingPack
- `POST /api/publishing-jobs` - Guardar publicación
- `POST /api/metrics/campaign` - Guardar métricas
- `POST /api/memory/save` - Guardar aprendizaje

---

## 🚀 Orden de Configuración Recomendado

### **Paso 1: Configurar el Trigger Principal** ⭐ (PRIMERO)

**Workflow:** `01-trigger-marketing-request.json`

**Por qué primero:**
- Es el punto de entrada desde tu sistema
- Valida que los datos lleguen correctamente
- Es el workflow que tu sistema llama directamente

**Pasos:**
1. Importar `01-trigger-marketing-request.json` en n8n
2. Activar el workflow
3. Copiar la URL del webhook (ej: `http://localhost:5678/webhook/abc123`)
4. Actualizar `appsettings.json`:
   ```json
   {
     "N8n": {
       "UseMock": false,
       "WebhookUrls": {
         "MarketingRequest": "http://localhost:5678/webhook/abc123"
       }
     }
   }
   ```

**Probar:**
```powershell
# Desde PowerShell, probar que el webhook funciona
curl -X POST http://localhost:5678/webhook/abc123 `
  -H "Content-Type: application/json" `
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Crear campaña de verano",
    "channels": ["instagram"],
    "requiresApproval": true
  }'
```

---

### **Paso 2: Configurar Workflows que n8n Llama al Sistema**

Estos workflows llaman a tu backend, así que tu backend debe estar corriendo.

#### **2.1: Validate Consents** 
**Workflow:** `02-validate-consents.json`

**Por qué:** Valida que el usuario tenga permisos antes de continuar

**Requiere:**
- Backend corriendo en `http://localhost:5000`
- Endpoint `/api/consents/check` funcionando

**Configurar:**
1. Importar `02-validate-consents.json`
2. En el nodo HTTP Request, verificar que la URL sea: `http://localhost:5000/api/consents/check`
3. Activar el workflow
4. Copiar URL del webhook y actualizar `appsettings.json`:
   ```json
   "ValidateConsents": "http://localhost:5678/webhook/xyz789"
   ```

#### **2.2: Load Marketing Memory**
**Workflow:** `03-load-marketing-memory.json`

**Por qué:** Carga el contexto histórico del tenant

**Requiere:**
- Endpoint `/api/memory/context` funcionando

**Configurar:**
1. Importar `03-load-marketing-memory.json`
2. Verificar URL en HTTP Request: `http://localhost:5000/api/memory/context`
3. Activar y copiar URL del webhook

---

### **Paso 3: Configurar Workflows de Generación (Opcional - Requieren OpenAI)**

Estos workflows usan OpenAI para generar contenido. Puedes configurarlos después.

- `04-analyze-instruction-ai.json` - Analiza instrucciones
- `05-generate-marketing-strategy.json` - Genera estrategia
- `06-generate-marketing-copy.json` - Genera copy
- `07-generate-visual-prompts.json` - Genera prompts visuales

**Nota:** Si no tienes OpenAI configurado, estos workflows simularán respuestas.

---

### **Paso 4: Configurar Workflows de Finalización**

Estos workflows guardan datos en tu backend.

#### **4.1: Build Marketing Pack**
**Workflow:** `08-build-marketing-pack.json`

**Requiere:**
- Endpoint `/api/marketing-packs` funcionando

#### **4.2: Human Approval Flow**
**Workflow:** `09-human-approval-flow.json`

**Requiere:**
- Endpoint `/api/marketing-packs` funcionando

#### **4.3: Publish Content**
**Workflow:** `10-publish-content.json`

**Requiere:**
- Endpoint `/api/publishing-jobs` funcionando

#### **4.4: Metrics & Learning**
**Workflow:** `11-metrics-learning.json`

**Requiere:**
- Endpoints `/api/metrics/campaign` y `/api/metrics/publishing-job` funcionando
- Endpoint `/api/memory/save` funcionando

---

## 📋 Checklist de Configuración Mínima

Para que el sistema básico funcione, necesitas al menos:

- [ ] **1. Trigger - Marketing Request** (OBLIGATORIO)
  - Importar workflow
  - Activar workflow
  - Copiar URL del webhook
  - Actualizar `appsettings.json` con la URL
  - Cambiar `UseMock: false`

- [ ] **2. Backend corriendo**
  - Sistema ASP.NET Core en `http://localhost:5000`
  - Endpoints API accesibles

- [ ] **3. Probar conexión**
  - Disparar desde el sistema
  - Ver ejecución en n8n
  - Verificar logs del backend

---

## 🔄 Flujo Completo de Ejecución

```
1. Usuario en tu sistema → Solicita crear contenido
   ↓
2. Tu sistema → Llama a n8n (Trigger - Marketing Request)
   POST http://localhost:5678/webhook/abc123
   ↓
3. n8n → Valida datos recibidos
   ↓
4. n8n → Llama a tu backend (Validate Consents)
   GET http://localhost:5000/api/consents/check?tenantId=...&userId=...
   ↓
5. n8n → Llama a tu backend (Load Memory)
   GET http://localhost:5000/api/memory/context?tenantId=...
   ↓
6. n8n → Genera estrategia y copy (usando OpenAI o simulación)
   ↓
7. n8n → Llama a tu backend (Build Marketing Pack)
   POST http://localhost:5000/api/marketing-packs
   ↓
8. n8n → Si requiere aprobación, guarda en estado "RequiresApproval"
   ↓
9. n8n → Publica contenido (si no requiere aprobación)
   ↓
10. n8n → Llama a tu backend (Save Metrics & Learning)
    POST http://localhost:5000/api/metrics/campaign
    POST http://localhost:5000/api/memory/save
```

---

## ⚠️ Importante

1. **El Trigger es el único workflow que tu sistema llama directamente**
   - Los demás workflows se llaman entre sí dentro de n8n
   - O se llaman desde el Trigger usando nodos HTTP Request

2. **Tu sistema NO necesita conocer las URLs de todos los workflows**
   - Solo necesita la URL del Trigger
   - Los demás workflows se conectan automáticamente dentro de n8n

3. **Configuración mínima para empezar:**
   - Solo necesitas el **Trigger - Marketing Request**
   - El resto puedes configurarlo gradualmente

---

## 🎯 Resumen

**Para empezar, configura SOLO este workflow:**

✅ **`01-trigger-marketing-request.json`** - El punto de entrada

Una vez que este funcione, puedes agregar los demás workflows uno por uno.

---

**¿Listo para configurar?** Sigue la guía: `docs/GUIA_CONFIGURACION_N8N_PASO_A_PASO.md`

