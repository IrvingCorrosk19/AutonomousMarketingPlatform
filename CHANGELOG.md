# CHANGELOG

Todos los cambios notables en este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

---

## [Unreleased]

### 🔧 Fixed - Auditoría Final Fase 3 (2025-01-01)

#### Correcciones Aplicadas

- **Eliminada conexión muerta a nodo inexistente**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Problema:** El nodo `Consolidate Publish Results` tenía una conexión a `HTTP Request - Save Learning` que no existe en `nodes`
  - **Corrección:** Eliminada la conexión muerta de `connections["Consolidate Publish Results"]`
  - **Impacto:** Evita errores en tiempo de ejecución cuando el flujo intenta ejecutar un nodo inexistente

- **Agregada guarda para `.toFixed()` en evaluación de precisión**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Nodo:** `OpenAI - Generate Evaluation Summary`
  - **Problema:** `$json.confidenceAccuracy.toFixed(2)` podría fallar si `confidenceAccuracy` es `undefined`
  - **Corrección:** Agregada validación: `(typeof $json.confidenceAccuracy === 'number' ? $json.confidenceAccuracy.toFixed(2) : '0.00')`
  - **Impacto:** Evita errores en tiempo de ejecución cuando `confidenceAccuracy` no está definido

- **Agregada guarda para acceso a array en métricas de publicación**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Nodo:** `HTTP Request - Save Job Metrics`
  - **Problema:** `$json.publishingJobIds[0]` podría fallar si `publishingJobIds` no es array o está vacío
  - **Corrección:** Agregada validación: `(Array.isArray($json.publishingJobIds) && $json.publishingJobIds.length > 0) ? $json.publishingJobIds[0] : ''`
  - **Impacto:** Evita errores en tiempo de ejecución cuando `publishingJobIds` no es un array válido

#### Bugs Identificados (Pendientes Corrección Futura)

- **Bug de determinismo en `Build Marketing Pack`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Nodo:** `Build Marketing Pack`
  - **Problema:** Usa `new Date().toISOString()` para `createdAt` y `generatedAt` en metadata, y `Date.now()` o `require('crypto').randomUUID()` para IDs
  - **Severidad:** MEDIA (no afecta decisiones cognitivas, pero afecta trazabilidad)
  - **Estado:** PENDIENTE CORRECCIÓN FUTURA
  - **Recomendación:** Usar `validatedData.requestId` para IDs determinísticos y `validatedData.receivedAt` para timestamps

- **Bug de determinismo en `Prepare Evaluation Times`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Nodo:** `Prepare Evaluation Times`
  - **Problema:** Usa `new Date()` para calcular `targetDate` y `now`
  - **Severidad:** MEDIA (afecta la determinación de qué eventos evaluar)
  - **Estado:** PENDIENTE CORRECCIÓN FUTURA
  - **Recomendación:** Usar timestamp del trigger del Cron o del evento evaluado

- **Bug de determinismo en `Calculate Block Status`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Nodo:** `Calculate Block Status`
  - **Problema:** Usa `new Date().toISOString()` como fallback para `referenceTimestamp`
  - **Severidad:** BAJA (solo afecta si falta el timestamp del evento)
  - **Estado:** PENDIENTE CORRECCIÓN FUTURA
  - **Recomendación:** Asegurar que siempre se pase `evaluationTime` del evento, y si falta, usar un timestamp fijo o lanzar error

#### Advertencias

- **Colisión de webhook paths**
  - **Problema:** `Load Marketing Memory.json` y `00-complete-marketing-flow.json` usan el mismo path `marketing-request`
  - **Impacto:** Solo uno puede estar activo a la vez en deployment
  - **Estado:** Requiere decisión de arquitectura (¿son flujos separados o deberían unificarse?)
  - **Severidad:** CRÍTICA para deployment, pero no es un bug de lógica

---

## [Fase 3] - Mejoras Cognitivas (2024-12-19)

### ✨ Added

- **Nodo `Register Human Override`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Propósito:** Detecta cuando `confidenceScore < 0.6` pero `requiresApproval = false` (human override)
  - **Funcionalidad:** Extrae patrón, crea `overrideData` estructurado, usa timestamps determinísticos

- **Nodo `HTTP Request - Save Override Memory`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Propósito:** Guarda override data en `PatternMemory`
  - **Endpoint:** `POST /api/memory/save`

- **Nodo `Calculate Escalated Penalty`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Propósito:** Calcula penalización escalada (Mild/Moderate/Severe/Critical) basada en desvíos CTR/Engagement y `failureCount`
  - **Funcionalidad:** Cálculos determinísticos con redondeo a 4 decimales

- **Nodo `Calculate Block Status`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Propósito:** Calcula estado de bloqueo escalado (warning/restriction/partial_block/permanent_block)
  - **Funcionalidad:** Basado en severidad y conteos de fallos, con ajuste temporal determinístico

- **Nodo `Check Override Result`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Propósito:** Verifica si la evaluación actual es para un override humano y ajusta penalización según resultado
  - **Funcionalidad:** Refuerzo positivo si override fue exitoso, penalización severa/crítica si falló

- **Nodo `HTTP Request - Load Pattern Memory`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Propósito:** Carga `PatternMemory` existente para cálculos de penalización y bloqueo
  - **Endpoint:** `GET /api/memory/save`

### 🔄 Modified

- **Nodo `Validate Confidence Score`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Cambio:** La rama "True" (cuando `confidenceScore < 0.6`) ahora conecta a `Register Human Override`

- **Nodo `Consolidate Advanced Memory`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Cambios:**
    - Agregada función determinística `daysSinceTimestamp`
    - Actualizada estructura `patternMemory` para incluir `severity`, `blockStatus`, y `patternTimestamps`
    - Redondeo consistente a 4 decimales para valores `penalty`
    - Uso de `referenceTimestamp` desde `validatedData.receivedAt` para cálculos temporales

- **Nodo `Cognitive Decision Engine`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Cambios:**
    - Reemplazado `new Date().toISOString()` con `validatedData.receivedAt` para `calculatedAt` (determinismo)
    - Incorporado `severity` y `blockStatus` de `advancedMemory.patternMemory` para influir en `confidenceScore` y `decisionRationale`
    - Aplicado ajuste temporal a `confidenceScore` basado en edad de `failedPatterns` y `successfulPatterns`

- **Nodo `Consolidate Metrics`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Cambios:**
    - Corregida duplicación de `isPositiveResult` y `result`
    - Corregido uso de `ctr` y `engagementRate` a `realCTR` y `realEngagementRate`
    - Agregado `patternMemory` desde `HTTP Request - Load Pattern Memory` al output

- **Nodo `Prepare Structured Learnings`**
  - **Archivo:** `workflows/n8n/12-feedback-learning-loop.json`
  - **Cambios:**
    - Actualizado para incluir `severity`, `blockStatus`, `overrideResult`, `failureCount`, y `timestamp` determinístico en `patternMemory.content`
    - `result` y `penalty` para `patternMemory` ahora se establecen condicionalmente basados en `overrideResult` si ocurrió un override

---

## [Fase 2] - Optimizaciones Seguras (2024-12-18)

### 🔄 Modified

- **Nodo `HTTP Request - Get Last Cognitive Version`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Cambio:** Movido para ejecutarse en paralelo con otros nodos de carga de memoria
  - **Impacto:** Reducción de latencia al paralelizar operaciones

- **Nodo `Consolidate Final Results`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Cambio:** Removida referencia a `memoryResult` y `learningSaved` (nodo `HTTP Request - Save Learning` fue eliminado)

### 🗑️ Removed

- **Nodo `HTTP Request - Save Learning`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Razón:** Paso innecesario de aprendizaje no estructurado, reemplazado por aprendizaje estructurado en Feedback Loop

---

## [Fase 1] - Correcciones Críticas (2024-12-17)

### 🔧 Fixed

- **Validación de `confidenceScore`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Nodo:** `Validate Confidence Score`
  - **Problema:** Uso de `||` para fallback numérico podría tratar `0` como falsy
  - **Corrección:** Reemplazado `||` con `??` para fallback numérico correcto

- **Validación de `requiresApproval`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Nodo:** `Validate Required Fields`
  - **Problema:** Validación ambigua de campo booleano
  - **Corrección:** Removida validación `isNotEmpty` para `requiresApproval`

- **Conexiones a nodos inexistentes**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Problema:** Conexiones a `Schedule Feedback Loop` y `HTTP Request - Save Feedback Schedule` que no existen
  - **Corrección:** Eliminadas conexiones muertas

- **Nodo `lastCognitiveVersion` indefinido**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Nodo:** `Consolidate Advanced Memory`
  - **Problema:** `lastCognitiveVersion` podría estar indefinido
  - **Corrección:** Agregado código para recuperar explícitamente `lastCognitiveVersion` desde `HTTP Request - Get Last Cognitive Version` con fallback seguro

### 🗑️ Removed

- **Nodo `Respond - Low Confidence`**
  - **Archivo:** `workflows/n8n/00-complete-marketing-flow.json`
  - **Razón:** Nodo no conectado, código muerto

---

## [Inicial] - Load Marketing Memory Bug Fix (2024-12-16)

### 🔧 Fixed

- **Validación de `requiresApproval`**
  - **Archivo:** `workflows/n8n/Load Marketing Memory.json`
  - **Nodo:** `Validate Required Fields`
  - **Problema:** Uso de `boolean → isNotEmpty` para `requiresApproval`, causando falsos errores cuando `requiresApproval = false`
  - **Corrección:** Eliminada condición `requiresApproval → boolean → isNotEmpty`, reemplazada con expresión: `{{ $json.requiresApproval === true || $json.requiresApproval === false }}`

- **Normalización de consents**
  - **Archivo:** `workflows/n8n/Load Marketing Memory.json`
  - **Nodo:** `Normalize Consents`
  - **Problema:** `aiConsent` y `publishingConsent` podrían no ser booleanos reales
  - **Corrección:** Agregado `Boolean()` para forzar tipo: `aiConsent: Boolean($json.aiConsent), publishingConsent: Boolean($json.publishingConsent)`

---

## Notas

- **Formato de versiones:** Se usa formato `[Fase X]` para agrupar cambios por fase de desarrollo
- **Fechas:** Formato `YYYY-MM-DD`
- **Severidad de bugs:** CRÍTICA, ALTA, MEDIA, BAJA
- **Estado de bugs:** PENDIENTE, EN PROGRESO, CORREGIDO

