# Orden de Ejecución de Workflows - Autonomous Marketing Platform

## Flujo Principal: Punto de Entrada

### 🎯 **00-complete-marketing-flow.json** (Complete Marketing Flow - Integrated)

**Este es el flujo que se carga primero y actúa como punto de entrada principal del sistema.**

**Webhook de Entrada:**
- **Path:** `/webhook/marketing-request`
- **Método:** POST
- **Nodo:** `Webhook - Receive Request`

---

## Orden de Ejecución en el Flujo Principal

### 1. **Webhook - Receive Request** (Punto de Entrada)
- Recibe la solicitud POST del backend
- Path: `marketing-request`

### 2. **Normalize Payload**
- Normaliza los datos del payload
- Extrae: `tenantId`, `userId`, `instruction`, `campaignId`, `requiresApproval`, `channels`, `assets`

### 3. **Validate Required Fields**
- Valida campos requeridos:
  - `tenantId` (string, notEmpty)
  - `userId` (string, notEmpty)
  - `instruction` (string, notEmpty)
  - `channels` (array, length > 0)
  - `requiresApproval` (boolean, === true || === false)

### 4. **Set Validated Data**
- Prepara datos validados para el siguiente paso
- Preserva `tenantId` y `userId` en nivel raíz

### 5. **HTTP Request - Check Consents**
- Consulta al backend: `GET /api/ConsentsApi/check?tenantId=xxx&userId=yyy`
- Verifica consentimientos del usuario

### 6. **Normalize Consents**
- Normaliza `aiConsent` y `publishingConsent` a booleanos reales
- Usa `Boolean()` para forzar tipo

### 7. **Validate Consents**
- Valida que ambos consents sean `true`
- Si alguno es `false`, responde con error 403

### 8. **HTTP Request - Load Marketing Memory** ⭐
- **AQUÍ ES DONDE SE CARGA LA MEMORIA**
- Consulta al backend: `GET /api/memory/context?tenantId=xxx`
- Carga memoria histórica del tenant

### 9. **Normalize Memory**
- Normaliza la memoria cargada
- Prepara para cargar memorias avanzadas en paralelo

### 10. **Carga de Memorias Avanzadas (Paralelo)**
- `HTTP Request - Load Preference Memory`
- `HTTP Request - Load Performance Memory`
- `HTTP Request - Load Constraint Memory`
- `HTTP Request - Load Pattern Memory`
- `HTTP Request - Get Last Cognitive Version`

### 11. **Consolidate Advanced Memory**
- Consolida todas las memorias avanzadas
- Calcula `confidenceWeights`
- Prepara contexto cognitivo

### 12. **Análisis y Generación (Secuencial)**
- `OpenAI - Analyze Instruction`
- `OpenAI - Generate Strategy`
- `OpenAI - Generate Copy`
- `OpenAI - Generate Visual Prompts`

### 13. **Cognitive Decision Engine**
- Calcula `confidenceScore`
- Ajusta decisiones basadas en memoria
- Determina `cognitiveVersion`

### 14. **Build Marketing Pack**
- Construye el MarketingPack completo
- Unifica todos los componentes generados

### 15. **Validate Confidence Score**
- Valida `confidenceScore >= 0.6`
- Si es bajo, puede requerir aprobación o registrar override

### 16. **Check Requires Approval Final**
- Decide si el pack requiere aprobación
- Bifurca el flujo según `requiresApproval`

### 17. **Save Pack**
- Guarda el MarketingPack en el backend
- Estado: "RequiresApproval" o "Ready"

### 18. **Prepare Publish Jobs** (si no requiere aprobación)
- Prepara trabajos de publicación
- Crea jobs por canal

### 19. **Publicación por Canal**
- `Publish - Instagram`
- `Publish - Facebook`
- `Publish - TikTok`

### 20. **Save Results**
- Guarda métricas de campaña
- Guarda métricas de publicación
- Guarda aprendizajes

### 21. **Respond - Final Success**
- Responde con éxito al backend

---

## Workflows Independientes

### **Load Marketing Memory.json**

**Propósito:** Workflow independiente para cargar solo la memoria de marketing.

**Webhook:**
- Path: `/webhook/load-marketing-memory` (diferente al principal)
- Puede ser llamado independientemente

**Flujo:**
1. Webhook - Receive Request
2. Normalize Payload
3. Validate Required Fields
4. Set Validated Data
5. HTTP Request - Check Consents
6. Normalize Consents
7. Validate Consents
8. Respond - Final Success

**Nota:** Este workflow NO carga la memoria completa, solo valida y responde. La carga de memoria real está integrada en el flujo principal.

---

### **12-feedback-learning-loop.json**

**Propósito:** Workflow de aprendizaje post-publicación.

**Trigger:**
- **Cron:** Se ejecuta cada hora automáticamente
- **NO es un punto de entrada manual**

**Flujo:**
1. Cron - Every Hour
2. Prepare Evaluation Times (24h y 48h)
3. HTTP Request - Get Publishing Jobs
4. Prepare Jobs for Evaluation
5. HTTP Request - Get Job Metrics
6. HTTP Request - Get Marketing Pack
7. HTTP Request - Load Pattern Memory
8. Consolidate Metrics
9. Calculate Escalated Penalty
10. Calculate Block Status
11. Check Override Result
12. OpenAI - Generate Evaluation Summary
13. Prepare Structured Learnings
14. Save Performance Memory
15. Save Pattern Memory
16. Check - Increment Version

---

## Resumen: ¿Qué Flujo Carga Primero?

### ✅ **Respuesta: `00-complete-marketing-flow.json`**

**Razones:**
1. Es el flujo principal integrado que contiene todo el proceso
2. Tiene el webhook principal: `/webhook/marketing-request`
3. Es el punto de entrada cuando el backend envía una solicitud de marketing
4. Integra todos los pasos: validación, consents, memoria, análisis, generación, decisión, publicación

**Orden de Carga de Memoria en el Flujo Principal:**
1. Primero se valida el payload
2. Luego se validan los consents
3. **Después se carga la memoria** (paso 8: `HTTP Request - Load Marketing Memory`)
4. Luego se cargan las memorias avanzadas en paralelo
5. Finalmente se consolida todo en `Consolidate Advanced Memory`

---

## Diagrama de Flujo Simplificado

```
Backend → POST /webhook/marketing-request
  ↓
00-complete-marketing-flow.json (FLUJO PRINCIPAL)
  ↓
1. Webhook - Receive Request
  ↓
2. Normalize Payload
  ↓
3. Validate Required Fields
  ↓
4. Set Validated Data
  ↓
5. HTTP Request - Check Consents
  ↓
6. Normalize Consents
  ↓
7. Validate Consents
  ↓
8. HTTP Request - Load Marketing Memory ⭐ (AQUÍ SE CARGA LA MEMORIA)
  ↓
9. Normalize Memory
  ↓
10. Carga Memorias Avanzadas (Paralelo)
  ↓
11. Consolidate Advanced Memory
  ↓
12-21. Análisis, Generación, Decisión, Publicación, Resultados
```

---

**Última actualización:** 2024-12-19
**Estado:** ✅ Documentación actualizada

