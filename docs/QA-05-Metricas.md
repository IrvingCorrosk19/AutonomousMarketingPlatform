# Casos de Prueba - Métricas

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Gestión de Métricas

---

## Índice

- [Listar Métricas de Campañas](#listar-métricas-de-campañas)
- [Métricas Detalladas de Campaña](#métricas-detalladas-de-campaña)
- [Métricas de Publicación](#métricas-de-publicación)
- [Registrar Métricas de Campaña](#registrar-métricas-de-campaña)
- [Registrar Métricas de Publicación](#registrar-métricas-de-publicación)
- [Cálculos Automáticos](#cálculos-automáticos)
- [Validaciones](#validaciones)
- [Filtros de Fecha](#filtros-de-fecha)
- [Roles y Permisos](#roles-y-permisos)
- [Multi-Tenant](#multi-tenant)

---

## Listar Métricas de Campañas

### TC-MET-001: Listar métricas de todas las campañas

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId válido
- Tenant tiene múltiples campañas con métricas registradas

**Pasos:**
1. Navegar a `/Metrics/Index`
2. Verificar que se muestra lista de métricas

**Resultado Esperado:**
- Lista muestra métricas de todas las campañas del tenant
- Métricas se agrupan o muestran por campaña
- No se muestran métricas de otros tenants
- Vista se carga sin errores

---

### TC-MET-002: Listar métricas con filtro de fecha desde

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/Index?fromDate=2024-12-01`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran métricas desde la fecha especificada
- Filtro se aplica correctamente
- `ViewBag.FromDate` contiene la fecha
- Métricas anteriores a fromDate no se muestran

---

### TC-MET-003: Listar métricas con filtro de fecha hasta

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/Index?toDate=2024-12-31`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran métricas hasta la fecha especificada
- Si toDate es null, se usa `DateTime.UtcNow` por defecto
- Filtro se aplica correctamente
- `ViewBag.ToDate` contiene la fecha

---

### TC-MET-004: Listar métricas con rango de fechas

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/Index?fromDate=2024-12-01&toDate=2024-12-31`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran métricas dentro del rango de fechas
- Ambos filtros se aplican simultáneamente
- Métricas fuera del rango no se muestran

---

### TC-MET-005: Listar métricas sin métricas registradas

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant no tiene métricas registradas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/Index`
2. Verificar vista

**Resultado Esperado:**
- Lista se muestra vacía
- Mensaje informativo se muestra si aplica
- No se generan errores

---

### TC-MET-006: Acceso a lista sin autenticación

**Módulo:** Métricas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Metrics/Index` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra contenido

---

## Métricas Detalladas de Campaña

### TC-MET-007: Ver métricas detalladas de campaña

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña existe en el tenant
- Campaña tiene métricas registradas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/Campaign?campaignId={campaignId}`
2. Verificar que se muestran métricas detalladas

**Resultado Esperado:**
- Resumen de métricas se muestra correctamente
- Métricas incluyen:
  - Impressions, Clicks, Engagement
  - Likes, Comments, Shares
  - ClickThroughRate, EngagementRate
  - ActivePosts
  - IsManualEntry, Source, Notes
- Datos son precisos y completos

---

### TC-MET-008: Ver métricas de campaña con filtro de fecha

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/Campaign?campaignId={id}&fromDate=2024-12-01&toDate=2024-12-31`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran métricas dentro del rango de fechas
- Resumen se calcula solo con métricas del rango
- Filtros se mantienen en ViewBag

---

### TC-MET-009: Ver métricas de campaña inexistente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña con ID dado NO existe
- O campaña pertenece a otro tenant
- Usuario está autenticado

**Pasos:**
1. Intentar acceder a `/Metrics/Campaign?campaignId={idInexistente}`

**Resultado Esperado:**
- Sistema maneja apropiadamente
- No se muestran métricas
- Error se maneja sin crashear

---

### TC-MET-010: Ver métricas de campaña sin métricas

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Campaña existe pero NO tiene métricas registradas
- Usuario está autenticado

**Pasos:**
1. Acceder a métricas de campaña sin métricas

**Resultado Esperado:**
- Vista se carga sin errores
- Resumen muestra valores en cero o null
- Mensaje informativo se muestra si aplica

---

## Métricas de Publicación

### TC-MET-011: Ver métricas detalladas de publicación

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación existe en el tenant
- Publicación tiene métricas registradas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Metrics/PublishingJob?publishingJobId={id}`
2. Verificar que se muestran métricas detalladas

**Resultado Esperado:**
- Resumen de métricas se muestra correctamente
- Métricas incluyen:
  - PublishingJobId, PublishingJobContent, Channel
  - MetricDate
  - Impressions, Clicks, Engagement
  - Likes, Comments, Shares
  - ClickThroughRate, EngagementRate
  - IsManualEntry, Source, Notes
- Datos son precisos y completos

---

### TC-MET-012: Ver métricas de publicación inexistente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación con ID dado NO existe
- O publicación pertenece a otro tenant

**Pasos:**
1. Intentar acceder a `/Metrics/PublishingJob?publishingJobId={idInexistente}`

**Resultado Esperado:**
- Sistema maneja apropiadamente
- No se muestran métricas
- Error se maneja sin crashear

---

## Registrar Métricas de Campaña

### TC-MET-013: Registrar métricas de campaña exitosamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Owner, Admin o Marketer
- Campaña existe en el tenant
- Usuario tiene TenantId válido

**Pasos:**
1. Navegar a `/Metrics/RegisterCampaign?campaignId={campaignId}`
2. Verificar que formulario se carga con MetricDate = DateTime.UtcNow.Date por defecto
3. Completar campos:
   - MetricDate: fecha actual
   - Impressions: 1000
   - Clicks: 50
   - Likes: 100
   - Comments: 20
   - Shares: 10
   - Source: "Manual"
   - Notes: "Métricas del día"
4. Hacer clic en "Registrar"

**Resultado Esperado:**
- Métricas se registran exitosamente
- Engagement se calcula automáticamente: 100 + 20 + 10 = 130
- ClickThroughRate se calcula: (50 / 1000) * 100 = 5%
- EngagementRate se calcula: (130 / 1000) * 100 = 13%
- IsManualEntry se establece como true
- Usuario es redirigido a `/Metrics/Campaign?campaignId={id}`
- TempData muestra: "Métricas registradas correctamente"

---

### TC-MET-014: Registrar métricas de campaña con todos los campos

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Campaña existe

**Pasos:**
1. Registrar métricas completando todos los campos opcionales:
   - Source: "API Externa"
   - Notes: "Notas detalladas sobre las métricas"
2. Guardar

**Resultado Esperado:**
- Todos los campos se guardan correctamente
- Source y Notes se persisten
- Métricas se registran exitosamente

---

### TC-MET-015: Actualizar métricas existentes de campaña (misma fecha)

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña ya tiene métricas registradas para una fecha específica
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas para la misma fecha de métricas existentes
2. Verificar comportamiento

**Resultado Esperado:**
- Sistema detecta métricas existentes para esa fecha
- Métricas se actualizan en lugar de crear duplicados
- Valores anteriores se sobrescriben
- No se crean registros duplicados

---

### TC-MET-016: Registrar métricas sin permisos

**Módulo:** Métricas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado sin rol Owner, Admin o Marketer

**Pasos:**
1. Intentar acceder a `/Metrics/RegisterCampaign?campaignId={id}` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Usuario es redirigido a `/Account/AccessDenied`

---

### TC-MET-017: Registrar métricas - Validación CampaignId vacío

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas con CampaignId = Guid.Empty
2. Verificar validación

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "El ID de la campaña es obligatorio."
- Métricas NO se registran

---

### TC-MET-018: Registrar métricas - Validación fecha futura

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas con MetricDate = fecha futura (más de 1 día)
2. Verificar validación

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "La fecha no puede ser futura."
- Métricas NO se registran
- Fecha debe ser <= DateTime.UtcNow.Date.AddDays(1)

---

### TC-MET-019: Registrar métricas - Validación valores negativos

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas con valores negativos:
   - Impressions: -100
   - Clicks: -50
   - Likes: -10
2. Verificar validación

**Resultado Esperado:**
- Validaciones fallan
- Mensajes de error específicos:
  - "Las impresiones no pueden ser negativas."
  - "Los clics no pueden ser negativos."
  - "Los likes no pueden ser negativos."
- Métricas NO se registran

---

### TC-MET-020: Registrar métricas - Validación Source muy largo

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas con Source de más de 50 caracteres
2. Verificar validación

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "La fuente no puede exceder 50 caracteres."
- Métricas NO se registran

---

### TC-MET-021: Registrar métricas - Validación Notes muy largo

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas con Notes de más de 2000 caracteres
2. Verificar validación

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "Las notas no pueden exceder 2000 caracteres."
- Métricas NO se registran

---

### TC-MET-022: Registrar métricas - Manejo de errores

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error (BD, servicio, etc.)

**Pasos:**
1. Intentar registrar métricas cuando hay error de sistema
2. Verificar manejo

**Resultado Esperado:**
- Error se captura
- Mensaje de error: "Error al registrar métricas. Por favor, intente nuevamente."
- Error se registra en logs
- Usuario permanece en formulario con datos ingresados

---

## Registrar Métricas de Publicación

### TC-MET-023: Registrar métricas de publicación exitosamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Owner, Admin o Marketer
- Publicación existe en el tenant
- Usuario tiene TenantId válido

**Pasos:**
1. Navegar a `/Metrics/RegisterPublishingJob?publishingJobId={id}`
2. Verificar que formulario se carga con MetricDate = DateTime.UtcNow.Date por defecto
3. Completar campos:
   - MetricDate: fecha actual
   - Impressions: 500
   - Clicks: 25
   - Likes: 50
   - Comments: 10
   - Shares: 5
   - Source: "Manual"
   - Notes: "Métricas del post"
4. Hacer clic en "Registrar"

**Resultado Esperado:**
- Métricas se registran exitosamente
- Engagement se calcula automáticamente: 50 + 10 + 5 = 65
- ClickThroughRate se calcula: (25 / 500) * 100 = 5%
- EngagementRate se calcula: (65 / 500) * 100 = 13%
- IsManualEntry se establece como true
- Usuario es redirigido a `/Metrics/PublishingJob?publishingJobId={id}`
- TempData muestra: "Métricas registradas correctamente"

---

### TC-MET-024: Actualizar métricas existentes de publicación (misma fecha)

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación ya tiene métricas registradas para una fecha específica
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas para la misma fecha de métricas existentes
2. Verificar comportamiento

**Resultado Esperado:**
- Sistema detecta métricas existentes para esa fecha
- Métricas se actualizan en lugar de crear duplicados
- Valores anteriores se sobrescriben
- No se crean registros duplicados

---

### TC-MET-025: Registrar métricas de publicación - Validaciones aplican igual que campaña

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas con datos inválidos (fecha futura, valores negativos, etc.)
2. Verificar validaciones

**Resultado Esperado:**
- Mismas validaciones que métricas de campaña se aplican:
  - PublishingJobId obligatorio
  - Fecha no futura
  - Valores no negativos
  - Source máximo 50 caracteres
  - Notes máximo 2000 caracteres
- Errores se muestran apropiadamente

---

## Cálculos Automáticos

### TC-MET-026: Engagement se calcula automáticamente (Likes + Comments + Shares)

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas con:
   - Likes: 100
   - Comments: 20
   - Shares: 10
2. Verificar cálculo de Engagement

**Resultado Esperado:**
- Engagement se calcula automáticamente: 100 + 20 + 10 = 130
- No se requiere ingresar Engagement manualmente
- Cálculo es preciso

---

### TC-MET-027: ClickThroughRate se calcula automáticamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas con:
   - Impressions: 1000
   - Clicks: 50
2. Verificar cálculo de CTR

**Resultado Esperado:**
- ClickThroughRate se calcula: (50 / 1000) * 100 = 5%
- Si Impressions = 0, CTR es null
- Cálculo es preciso con decimales

---

### TC-MET-028: EngagementRate se calcula automáticamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas con:
   - Impressions: 1000
   - Engagement: 130
2. Verificar cálculo de EngagementRate

**Resultado Esperado:**
- EngagementRate se calcula: (130 / 1000) * 100 = 13%
- Si Impressions = 0, EngagementRate es null
- Cálculo es preciso con decimales

---

### TC-MET-029: CTR y EngagementRate con Impressions = 0

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas con Impressions = 0
2. Verificar cálculos

**Resultado Esperado:**
- ClickThroughRate es null (no se divide por cero)
- EngagementRate es null (no se divide por cero)
- No se generan errores de división por cero

---

## Validaciones

### TC-MET-030: Validación ModelState en formularios

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar registrar métricas sin completar campos requeridos
2. Verificar validación

**Resultado Esperado:**
- Si ModelState no es válido, se muestra formulario con errores
- Usuario permanece en página de registro
- Métricas NO se registran
- Errores se muestran claramente

---

## Filtros de Fecha

### TC-MET-031: Filtro de fecha desde se aplica correctamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Acceder a métricas con fromDate específico
2. Verificar que solo se muestran métricas desde esa fecha

**Resultado Esperado:**
- Filtro se aplica correctamente en queries
- Métricas anteriores a fromDate no se incluyen
- Resúmenes se calculan solo con métricas del rango

---

### TC-MET-032: Filtro de fecha hasta se aplica correctamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Acceder a métricas con toDate específico
2. Verificar que solo se muestran métricas hasta esa fecha

**Resultado Esperado:**
- Filtro se aplica correctamente
- Si toDate es null, se usa DateTime.UtcNow por defecto
- Métricas posteriores a toDate no se incluyen

---

### TC-MET-033: Rango de fechas se aplica correctamente

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene métricas en diferentes fechas
- Usuario está autenticado

**Pasos:**
1. Acceder a métricas con fromDate y toDate
2. Verificar que solo se muestran métricas dentro del rango

**Resultado Esperado:**
- Ambos filtros se aplican simultáneamente
- Solo métricas dentro del rango se muestran
- Resúmenes se calculan correctamente

---

## Roles y Permisos

### TC-MET-034: Usuario Marketer puede registrar métricas

**Módulo:** Métricas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Marketer"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a registrar métricas de campaña (debe permitir)
2. Verificar acceso a registrar métricas de publicación (debe permitir)
3. Verificar acceso a ver métricas (debe permitir)

**Resultado Esperado:**
- Marketer puede registrar métricas de campaña
- Marketer puede registrar métricas de publicación
- Marketer puede ver todas las métricas
- Permisos se aplican correctamente según `[AuthorizeRole]`

---

### TC-MET-035: Usuario Viewer solo puede ver métricas (no registrar)

**Módulo:** Métricas  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene rol que NO incluye Owner, Admin o Marketer

**Pasos:**
1. Intentar acceder a registrar métricas (debe denegar)
2. Verificar acceso a ver métricas (debe permitir)

**Resultado Esperado:**
- Viewer NO puede registrar métricas
- Viewer PUEDE ver métricas (solo lectura)
- Permisos funcionan correctamente

---

## Multi-Tenant

### TC-MET-036: Métricas se filtran por TenantId

**Módulo:** Métricas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene métricas de campañas y publicaciones
- Tenant2 tiene métricas diferentes
- Ambos tenants existen

**Pasos:**
1. Usuario de Tenant1 accede a métricas
2. Verificar métricas mostradas
3. Usuario de Tenant2 accede a métricas
4. Verificar métricas mostradas

**Resultado Esperado:**
- Usuario de Tenant1 ve solo sus métricas
- Usuario de Tenant2 ve solo sus métricas
- No hay mezcla de datos entre tenants
- Queries filtran correctamente por TenantId

---

### TC-MET-037: Métricas registradas se asignan al tenant correcto

**Módulo:** Métricas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos

**Pasos:**
1. Usuario de Tenant1 registra métricas
2. Verificar en base de datos que TenantId de las métricas es Tenant1

**Resultado Esperado:**
- Métricas se crean con `TenantId = Tenant1` (del usuario)
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene

---

### TC-MET-038: Acceso a métricas sin TenantId

**Módulo:** Métricas  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId válido
- Usuario NO es SuperAdmin

**Pasos:**
1. Intentar acceder a `/Metrics/Index` sin TenantId

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestran métricas

---

### TC-MET-039: Registrar métricas de campaña de otro tenant

**Módulo:** Métricas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta registrar métricas de campaña de Tenant2

**Resultado Esperado:**
- Sistema valida que campaña pertenece al tenant del usuario
- Si no pertenece, error apropiado
- Métricas NO se registran para campaña de otro tenant

---

### TC-MET-040: Registrar métricas de publicación de otro tenant

**Módulo:** Métricas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta registrar métricas de publicación de Tenant2

**Resultado Esperado:**
- Sistema valida que publicación pertenece al tenant del usuario
- Si no pertenece, error apropiado
- Métricas NO se registran para publicación de otro tenant

---

## Aprendizaje Automático

### TC-MET-041: Aprendizaje automático se dispara al registrar métricas

**Módulo:** Métricas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Sistema tiene servicio de aprendizaje activo
- Usuario tiene permisos

**Pasos:**
1. Registrar métricas de campaña
2. Verificar en logs que aprendizaje se dispara

**Resultado Esperado:**
- Al registrar métricas, se dispara aprendizaje automático en background
- Aprendizaje no bloquea la respuesta del usuario
- Se registra en logs: "Aprendizaje automático completado"
- Si hay error en aprendizaje, se registra pero no afecta el registro de métricas

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-MET-001 | Listar métricas de todas las campañas | Funcional | Crítica | - |
| TC-MET-002 | Listar con filtro desde | Funcional | Alta | - |
| TC-MET-003 | Listar con filtro hasta | Funcional | Alta | - |
| TC-MET-004 | Listar con rango de fechas | Funcional | Alta | - |
| TC-MET-005 | Listar sin métricas | Funcional | Media | - |
| TC-MET-006 | Acceso sin autenticación | Seguridad | Crítica | - |
| TC-MET-007 | Ver métricas detalladas de campaña | Funcional | Crítica | - |
| TC-MET-008 | Ver métricas con filtro de fecha | Funcional | Alta | - |
| TC-MET-009 | Ver métricas de campaña inexistente | Funcional | Alta | - |
| TC-MET-010 | Ver métricas sin métricas | Funcional | Media | - |
| TC-MET-011 | Ver métricas de publicación | Funcional | Crítica | - |
| TC-MET-012 | Ver métricas de publicación inexistente | Funcional | Alta | - |
| TC-MET-013 | Registrar métricas de campaña | Funcional | Crítica | - |
| TC-MET-014 | Registrar con todos los campos | Funcional | Alta | - |
| TC-MET-015 | Actualizar métricas existentes campaña | Funcional | Alta | - |
| TC-MET-016 | Registrar sin permisos | Seguridad | Crítica | - |
| TC-MET-017 | Validación CampaignId vacío | Funcional | Alta | - |
| TC-MET-018 | Validación fecha futura | Funcional | Alta | - |
| TC-MET-019 | Validación valores negativos | Funcional | Alta | - |
| TC-MET-020 | Validación Source muy largo | Funcional | Media | - |
| TC-MET-021 | Validación Notes muy largo | Funcional | Media | - |
| TC-MET-022 | Manejo de errores al registrar | Funcional | Alta | - |
| TC-MET-023 | Registrar métricas de publicación | Funcional | Crítica | - |
| TC-MET-024 | Actualizar métricas existentes publicación | Funcional | Alta | - |
| TC-MET-025 | Validaciones de publicación | Funcional | Alta | - |
| TC-MET-026 | Cálculo Engagement automático | Funcional | Crítica | - |
| TC-MET-027 | Cálculo ClickThroughRate | Funcional | Alta | - |
| TC-MET-028 | Cálculo EngagementRate | Funcional | Alta | - |
| TC-MET-029 | CTR y ER con Impressions = 0 | Funcional | Media | - |
| TC-MET-030 | Validación ModelState | Funcional | Alta | - |
| TC-MET-031 | Filtro fecha desde | Funcional | Alta | - |
| TC-MET-032 | Filtro fecha hasta | Funcional | Alta | - |
| TC-MET-033 | Rango de fechas | Funcional | Alta | - |
| TC-MET-034 | Permisos Marketer | Seguridad | Crítica | - |
| TC-MET-035 | Permisos Viewer | Seguridad | Alta | - |
| TC-MET-036 | Métricas filtran por TenantId | Multi-tenant | Crítica | - |
| TC-MET-037 | Métricas asignadas al tenant correcto | Multi-tenant | Crítica | - |
| TC-MET-038 | Acceso sin TenantId | Multi-tenant | Alta | - |
| TC-MET-039 | Registrar métricas campaña otro tenant | Multi-tenant | Crítica | - |
| TC-MET-040 | Registrar métricas publicación otro tenant | Multi-tenant | Crítica | - |
| TC-MET-041 | Aprendizaje automático se dispara | Funcional | Media | - |

**Total de casos:** 41  
**Críticos:** 13  
**Altos:** 22  
**Medios:** 6

