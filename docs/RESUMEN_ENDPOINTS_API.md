# 📊 RESUMEN DE ENDPOINTS API

**Fecha:** 2025-01-01  
**Total de Endpoints API:** 11

---

## 🎯 ENDPOINTS API (Controllers en `/Controllers/Api/`)

### 1. **ConsentsApiController** (`/api/ConsentsApi`)
- **GET** `/api/ConsentsApi/check` - Verificar consentimientos del usuario
  - Query params: `tenantId`, `userId`
  - Retorna: `{ aiConsent, publishingConsent }`

**Total:** 1 endpoint

---

### 2. **MemoryApiController** (`/api/memory`)
- **GET** `/api/memory` - Obtener memorias por tipo
  - Query params: `tenantId` (requerido), `memoryType` (opcional), `tags` (opcional), `limit` (opcional)
  - Retorna: Lista de `MarketingMemoryDto`
  
- **GET** `/api/memory/context` - Obtener contexto completo de memoria
  - Query params: `tenantId` (requerido), `userId` (opcional), `campaignId` (opcional), `memoryType` (opcional)
  - Retorna: `MemoryContextResponse` con preferences, learnings, restrictions
  
- **POST** `/api/memory/save` - Guardar nueva memoria
  - Body: `SaveMemoryRequest` con `tenantId`, `memoryType`, `content`, `context`, `tags`, etc.
  - Retorna: `{ id, memoryType, tenantId, message }`

**Total:** 3 endpoints

---

### 3. **MarketingPacksApiController** (`/api/marketing-packs`)
- **GET** `/api/marketing-packs` - Obtener marketing packs
  - Query params: `id` (opcional), `tenantId` (requerido si no hay id), `orderBy` (opcional), `limit` (opcional)
  - Retorna: `MarketingPackResponse` o lista de `MarketingPackResponse`
  - Soporta: filtrado por ID, ordenamiento por `cognitiveVersion` o `createdAt`
  
- **POST** `/api/marketing-packs` - Crear o actualizar marketing pack
  - Body: `CreateMarketingPackRequest` con `tenantId`, `userId`, `strategy`, `copies`, `assetPrompts`, etc.
  - Retorna: `MarketingPackResponse`

**Total:** 2 endpoints

---

### 4. **PublishingJobsApiController** (`/api/publishing-jobs`)
- **GET** `/api/publishing-jobs` - Obtener publishing jobs con filtros
  - Query params: `tenantId` (requerido), `publishedAfter` (opcional), `status` (opcional), `campaignId` (opcional)
  - Retorna: Lista de `PublishingJobResponse`
  - Ordena por fecha de publicación descendente
  
- **POST** `/api/publishing-jobs` - Crear publishing job
  - Body: `CreatePublishingJobRequest` con `tenantId`, `campaignId`, `channel`, `content`, etc.
  - Retorna: `PublishingJobResponse`

**Total:** 2 endpoints

---

### 5. **MetricsApiController** (`/api/metrics`)
- **GET** `/api/metrics/publishing-job` - Obtener métricas de publishing job
  - Query params: `publishingJobId` (requerido), `fromDate` (opcional), `toDate` (opcional)
  - Retorna: Lista de `PublishingJobMetricsResponse`
  
- **POST** `/api/metrics/campaign` - Guardar métricas de campaña
  - Body: `SaveCampaignMetricsRequest` con `tenantId`, `campaignId`, `impressions`, `clicks`, etc.
  - Retorna: `CampaignMetricsResponse`
  
- **POST** `/api/metrics/publishing-job` - Guardar métricas de publishing job
  - Body: `SavePublishingJobMetricsRequest` con `tenantId`, `publishingJobId`, `impressions`, `clicks`, etc.
  - Retorna: `PublishingJobMetricsResponse`

**Total:** 3 endpoints

---

## 📊 RESUMEN POR MÉTODO HTTP

| Método | Cantidad | Endpoints |
|--------|----------|-----------|
| **GET** | 6 | `/api/ConsentsApi/check`, `/api/memory`, `/api/memory/context`, `/api/marketing-packs`, `/api/publishing-jobs`, `/api/metrics/publishing-job` |
| **POST** | 5 | `/api/memory/save`, `/api/marketing-packs`, `/api/publishing-jobs`, `/api/metrics/campaign`, `/api/metrics/publishing-job` |

**Total:** 11 endpoints

---

## 🎯 ENDPOINTS POR FUNCIONALIDAD

### Gestión de Memoria (3 endpoints)
- GET `/api/memory` - Listar memorias
- GET `/api/memory/context` - Contexto completo
- POST `/api/memory/save` - Guardar memoria

### Gestión de Marketing Packs (2 endpoints)
- GET `/api/marketing-packs` - Obtener packs
- POST `/api/marketing-packs` - Crear/actualizar pack

### Gestión de Publishing Jobs (2 endpoints)
- GET `/api/publishing-jobs` - Listar jobs
- POST `/api/publishing-jobs` - Crear job

### Gestión de Métricas (3 endpoints)
- GET `/api/metrics/publishing-job` - Obtener métricas
- POST `/api/metrics/campaign` - Guardar métricas de campaña
- POST `/api/metrics/publishing-job` - Guardar métricas de job

### Validación de Consentimientos (1 endpoint)
- GET `/api/ConsentsApi/check` - Verificar consents

---

## ✅ ESTADO DE IMPLEMENTACIÓN

**Todos los endpoints están implementados y funcionando correctamente.**

- ✅ 11 endpoints API implementados
- ✅ Todos los endpoints requeridos por workflows n8n existen
- ✅ Filtrado y ordenamiento funcionando
- ✅ Validaciones implementadas
- ✅ Manejo de errores implementado

---

**Última actualización:** 2025-01-01

