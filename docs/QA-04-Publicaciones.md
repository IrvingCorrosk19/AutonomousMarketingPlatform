# Casos de Prueba - Publicaciones

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Gestión de Publicaciones

---

## Índice

- [Listar Publicaciones](#listar-publicaciones)
- [Generar Publicación](#generar-publicación)
- [Ver Detalles de Publicación](#ver-detalles-de-publicación)
- [Descargar Paquete de Publicación](#descargar-paquete-de-publicación)
- [Aprobar Publicación](#aprobar-publicación)
- [Estados de Publicación](#estados-de-publicación)
- [Filtros y Búsqueda](#filtros-y-búsqueda)
- [Roles y Permisos](#roles-y-permisos)
- [Multi-Tenant](#multi-tenant)
- [Workflow de Publicación](#workflow-de-publicación)

---

## Listar Publicaciones

### TC-PUB-001: Listar todas las publicaciones del tenant

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId válido
- Tenant tiene múltiples publicaciones con diferentes estados y canales

**Pasos:**
1. Navegar a `/Publishing/Index`
2. Verificar que se muestra lista de publicaciones

**Resultado Esperado:**
- Lista muestra todas las publicaciones del tenant
- Publicaciones se muestran con información relevante (canal, estado, fechas, URL)
- No se muestran publicaciones de otros tenants
- Vista se carga sin errores

---

### TC-PUB-002: Filtrar publicaciones por campaña

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene publicaciones asociadas a diferentes campañas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Index?campaignId={campaignId}`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran publicaciones de la campaña especificada
- Filtro se aplica correctamente
- `ViewBag.CampaignId` contiene el ID de la campaña
- URL refleja el filtro aplicado

---

### TC-PUB-003: Filtrar publicaciones por estado

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene publicaciones con diferentes estados: Pending, Processing, Success, Failed, RequiresApproval, Cancelled
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Index?status=RequiresApproval`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran publicaciones con el estado seleccionado
- Filtro se aplica correctamente
- `ViewBag.StatusFilter` contiene el estado seleccionado

---

### TC-PUB-004: Filtrar publicaciones por canal

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene publicaciones en diferentes canales (Instagram, Facebook, TikTok, etc.)
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Index?channel=Instagram`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran publicaciones del canal especificado
- Filtro se aplica correctamente
- `ViewBag.ChannelFilter` contiene el canal seleccionado

---

### TC-PUB-005: Filtrar publicaciones con múltiples filtros

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene publicaciones variadas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Index?campaignId={id}&status=Success&channel=Instagram`
2. Verificar resultados

**Resultado Esperado:**
- Se aplican todos los filtros simultáneamente
- Solo se muestran publicaciones que cumplen TODOS los criterios
- Filtros se mantienen en ViewBag

---

### TC-PUB-006: Listar publicaciones con tenant sin publicaciones

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant no tiene publicaciones creadas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Index`
2. Verificar vista

**Resultado Esperado:**
- Lista se muestra vacía
- Mensaje informativo se muestra si aplica
- No se generan errores
- Botón "Generar Publicación" está disponible

---

### TC-PUB-007: Acceso a lista sin autenticación

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Publishing/Index` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra contenido

---

## Generar Publicación

### TC-PUB-008: Generar publicación exitosamente con datos mínimos

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Owner, Admin o Marketer
- Campaña existe en el tenant
- Usuario tiene TenantId válido

**Pasos:**
1. Navegar a `/Publishing/Generate`
2. Verificar que formulario se carga con `RequiresApproval = true` por defecto
3. Seleccionar CampaignId
4. Seleccionar Channel (ej: "Instagram")
5. Dejar otros campos opcionales
6. Hacer clic en "Generar"

**Resultado Esperado:**
- Publicación se genera exitosamente
- Status se establece como "Pending"
- RequiresApproval se establece como true por defecto
- TenantId se asigna automáticamente del usuario
- Usuario es redirigido a `/Publishing/Details/{id}`
- TempData muestra: "Publicación generada exitosamente. Se procesará en breve."
- PublishingJob se guarda en base de datos

---

### TC-PUB-009: Generar publicación con todos los campos completos

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Campaña existe
- MarketingPack existe (opcional)
- GeneratedCopy existe (opcional)

**Pasos:**
1. Navegar a `/Publishing/Generate?campaignId={id}&marketingPackId={id}`
2. Completar todos los campos:
   - CampaignId: seleccionado
   - MarketingPackId: seleccionado
   - GeneratedCopyId: seleccionado (opcional)
   - Channel: "Instagram"
   - ScheduledDate: fecha futura
   - RequiresApproval: true
3. Hacer clic en "Generar"

**Resultado Esperado:**
- Publicación se genera con todos los campos
- Fecha programada se guarda correctamente
- Relaciones con Campaign, MarketingPack, GeneratedCopy se establecen
- Redirección a detalles exitosa

---

### TC-PUB-010: Generar publicación desde campaña específica

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe
- Usuario tiene permisos

**Pasos:**
1. Navegar a `/Publishing/Generate?campaignId={campaignId}`
2. Verificar que CampaignId se pre-llena en formulario
3. Completar otros campos requeridos
4. Generar publicación

**Resultado Esperado:**
- CampaignId se pre-llena correctamente
- `ViewBag.CampaignId` contiene el ID
- Publicación se asocia correctamente a la campaña

---

### TC-PUB-011: Generar publicación desde MarketingPack

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- MarketingPack existe
- Usuario tiene permisos

**Pasos:**
1. Navegar a `/Publishing/Generate?marketingPackId={marketingPackId}`
2. Verificar que MarketingPackId se pre-llena
3. Completar otros campos
4. Generar publicación

**Resultado Esperado:**
- MarketingPackId se pre-llena correctamente
- `ViewBag.MarketingPackId` contiene el ID
- Publicación se asocia correctamente al MarketingPack

---

### TC-PUB-012: Generar publicación sin permisos

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado sin rol Owner, Admin o Marketer

**Pasos:**
1. Intentar acceder a `/Publishing/Generate` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Usuario es redirigido a `/Account/AccessDenied`

---

### TC-PUB-013: Generar publicación - Validación ModelState

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Navegar a `/Publishing/Generate`
2. Intentar generar sin completar campos requeridos
3. Verificar validación

**Resultado Esperado:**
- Si ModelState no es válido, se muestra formulario con errores
- Usuario permanece en página de generación
- Publicación NO se crea

---

### TC-PUB-014: Generar publicación - Manejo de errores

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error (BD, servicio, etc.)

**Pasos:**
1. Intentar generar publicación cuando hay error de sistema
2. Verificar manejo

**Resultado Esperado:**
- Error se captura
- Mensaje de error se muestra: "Error al generar la publicación: {mensaje}"
- Error se registra en logs
- Usuario permanece en formulario

---

## Ver Detalles de Publicación

### TC-PUB-015: Ver detalles de publicación existente

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación existe en el tenant del usuario
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Details/{jobId}`
2. Verificar que se muestran todos los detalles

**Resultado Esperado:**
- Detalles de publicación se muestran correctamente:
  - ID, CampaignId, MarketingPackId, GeneratedCopyId
  - Channel, Status
  - ScheduledDate, ProcessedAt, PublishedDate
  - Content, Hashtags, MediaUrl
  - PublishedUrl, ExternalPostId
  - RequiresApproval, DownloadUrl
  - ErrorMessage (si aplica)
  - RetryCount
  - Fechas de creación y actualización
- Información es precisa y completa

---

### TC-PUB-016: Ver detalles de publicación inexistente

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación con ID dado NO existe
- Usuario está autenticado

**Pasos:**
1. Intentar acceder a `/Publishing/Details/{idInexistente}`

**Resultado Esperado:**
- Sistema retorna `NotFound()` (404)
- Página 404 se muestra

---

### TC-PUB-017: Ver detalles de publicación de otro tenant

**Módulo:** Publicaciones  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación existe pero pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta acceder a `/Publishing/Details/{jobIdDeTenant2}`

**Resultado Esperado:**
- Query filtra por TenantId del usuario
- Publicación NO se encuentra
- Sistema retorna `NotFound()`
- No hay fuga de información entre tenants

---

### TC-PUB-018: Ver detalles sin autenticación

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Publishing/Details/{id}` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestran detalles

---

## Descargar Paquete de Publicación

### TC-PUB-019: Descargar paquete de publicación (Fase A)

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación existe con Status = "RequiresApproval"
- Publicación tiene DownloadUrl generado (formato data URI base64)
- Usuario tiene rol Owner, Admin o Marketer
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Details/{jobId}`
2. Hacer clic en botón "Descargar Paquete" o acceder a `/Publishing/DownloadPackage/{jobId}`
3. Verificar descarga

**Resultado Esperado:**
- Archivo JSON se descarga correctamente
- Nombre de archivo: `publicacion-{id}.json`
- Contenido es JSON válido (deserializado desde base64)
- Content-Type: `application/json`
- Archivo contiene payload completo de la publicación

---

### TC-PUB-020: Descargar paquete sin DownloadUrl

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Publicación existe pero NO tiene DownloadUrl
- O DownloadUrl está vacío

**Pasos:**
1. Intentar descargar paquete de publicación sin DownloadUrl

**Resultado Esperado:**
- Sistema retorna `NotFound()`
- No se descarga archivo
- Mensaje apropiado se muestra

---

### TC-PUB-021: Descargar paquete con formato incorrecto

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Publicación tiene DownloadUrl pero no en formato esperado
- DownloadUrl no comienza con "data:application/json;base64,"

**Pasos:**
1. Intentar descargar paquete con formato incorrecto

**Resultado Esperado:**
- Sistema retorna `NotFound()`
- Error se maneja apropiadamente
- No se descarga archivo corrupto

---

### TC-PUB-022: Descargar paquete sin permisos

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado sin rol Owner, Admin o Marketer

**Pasos:**
1. Intentar acceder a `/Publishing/DownloadPackage/{id}` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Redirección a `/Account/AccessDenied`

---

### TC-PUB-023: Descargar paquete de otro tenant

**Módulo:** Publicaciones  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta descargar paquete de Tenant2

**Resultado Esperado:**
- Query filtra por TenantId
- Publicación NO se encuentra
- Sistema retorna `NotFound()`
- No hay acceso cruzado

---

## Aprobar Publicación

### TC-PUB-024: Aprobar publicación exitosamente

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación existe con Status = "RequiresApproval"
- Usuario tiene rol Owner, Admin o Marketer
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Publishing/Details/{jobId}`
2. Hacer clic en botón "Aprobar" o enviar POST a `/Publishing/Approve/{jobId}`
3. Opcionalmente proporcionar PublishedUrl y ExternalPostId
4. Confirmar aprobación

**Resultado Esperado:**
- Publicación se aprueba exitosamente
- Status se actualiza a "Success" (o estado apropiado)
- ApprovedAt se establece a fecha/hora actual
- ApprovedBy se establece al UserId del usuario
- Si se proporcionan, PublishedUrl y ExternalPostId se guardan
- Usuario es redirigido a `/Publishing/Details/{id}`
- TempData muestra: "Publicación aprobada y marcada como publicada."

---

### TC-PUB-025: Aprobar publicación con URL y PostId

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación existe con Status = "RequiresApproval"
- Usuario tiene permisos

**Pasos:**
1. Aprobar publicación proporcionando:
   - PublishedUrl: "https://instagram.com/p/abc123"
   - ExternalPostId: "123456789"

**Resultado Esperado:**
- PublishedUrl se guarda correctamente
- ExternalPostId se guarda correctamente
- Ambos campos se muestran en detalles después de aprobar

---

### TC-PUB-026: Aprobar publicación sin permisos

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado sin rol Owner, Admin o Marketer

**Pasos:**
1. Intentar aprobar publicación sin permisos

**Resultado Esperado:**
- Acceso denegado
- Redirección a `/Account/AccessDenied`

---

### TC-PUB-027: Aprobar publicación inexistente

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Publicación con ID dado NO existe
- Usuario tiene permisos

**Pasos:**
1. Intentar aprobar publicación inexistente

**Resultado Esperado:**
- Error se maneja apropiadamente
- TempData muestra mensaje de error
- Usuario es redirigido a detalles (o página apropiada)

---

### TC-PUB-028: Aprobar publicación de otro tenant

**Módulo:** Publicaciones  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Publicación pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta aprobar publicación de Tenant2

**Resultado Esperado:**
- Query filtra por TenantId
- Publicación NO se encuentra
- No se aprueba publicación de otro tenant
- Error se maneja apropiadamente

---

### TC-PUB-029: Aprobar publicación - Manejo de errores

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación existe
- Sistema tiene error al procesar aprobación

**Pasos:**
1. Intentar aprobar publicación cuando hay error de sistema
2. Verificar manejo

**Resultado Esperado:**
- Error se captura
- TempData muestra: "Error al aprobar la publicación: {mensaje}"
- Error se registra en logs
- Usuario es redirigido a detalles

---

## Estados de Publicación

### TC-PUB-030: Publicación en estado Pending

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación recién creada
- Status = "Pending"

**Pasos:**
1. Crear nueva publicación
2. Verificar estado inicial

**Resultado Esperado:**
- Status se establece como "Pending"
- Publicación está lista para ser procesada
- Estado se muestra correctamente en detalles

---

### TC-PUB-031: Publicación en estado Processing

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Publicación está siendo procesada por background service
- Status = "Processing"

**Pasos:**
1. Verificar publicación en estado Processing
2. Verificar que ProcessedAt se establece

**Resultado Esperado:**
- Status = "Processing"
- ProcessedAt se establece cuando comienza procesamiento
- No se puede procesar concurrentemente (validación en servicio)

---

### TC-PUB-032: Publicación en estado RequiresApproval

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación fue procesada y requiere aprobación manual
- Status = "RequiresApproval"
- DownloadUrl está disponible

**Pasos:**
1. Verificar publicación en estado RequiresApproval
2. Verificar que DownloadUrl está disponible

**Resultado Esperado:**
- Status = "RequiresApproval"
- DownloadUrl contiene paquete JSON en formato data URI base64
- Publicación está lista para aprobación manual
- Botón "Aprobar" está disponible

---

### TC-PUB-033: Publicación en estado Success

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación fue aprobada o publicada exitosamente
- Status = "Success"

**Pasos:**
1. Verificar publicación en estado Success
2. Verificar campos relacionados

**Resultado Esperado:**
- Status = "Success"
- PublishedDate se establece
- PublishedUrl y ExternalPostId pueden estar presentes
- Publicación se muestra como completada

---

### TC-PUB-034: Publicación en estado Failed

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Publicación falló durante procesamiento o publicación
- Status = "Failed"

**Pasos:**
1. Verificar publicación en estado Failed
2. Verificar ErrorMessage

**Resultado Esperado:**
- Status = "Failed"
- ErrorMessage contiene descripción del error
- Publicación puede ser reintentada si RetryCount < MaxRetries
- Error se muestra en detalles

---

### TC-PUB-035: Publicación en estado Cancelled

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Publicación fue cancelada
- Status = "Cancelled"

**Pasos:**
1. Verificar publicación en estado Cancelled

**Resultado Esperado:**
- Status = "Cancelled"
- Publicación no se procesa más
- Estado se muestra correctamente

---

## Filtros y Búsqueda

### TC-PUB-036: Filtrar publicaciones por múltiples criterios simultáneos

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene publicaciones variadas
- Usuario está autenticado

**Pasos:**
1. Aplicar filtros combinados: campaña + estado + canal
2. Verificar resultados

**Resultado Esperado:**
- Todos los filtros se aplican correctamente
- Solo se muestran publicaciones que cumplen TODOS los criterios
- Filtros se mantienen en ViewBag para mostrar en UI

---

## Roles y Permisos

### TC-PUB-037: Usuario Marketer puede generar, descargar y aprobar

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Marketer"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a `/Publishing/Generate` (debe permitir)
2. Verificar acceso a descargar paquete (debe permitir)
3. Verificar acceso a aprobar (debe permitir)
4. Verificar acceso a lista y detalles (debe permitir)

**Resultado Esperado:**
- Marketer puede generar publicaciones
- Marketer puede descargar paquetes
- Marketer puede aprobar publicaciones
- Marketer puede ver lista y detalles
- Permisos se aplican correctamente según `[AuthorizeRole]`

---

### TC-PUB-038: Usuario Viewer solo puede ver (no generar/aprobar)

**Módulo:** Publicaciones  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene rol que NO incluye Owner, Admin o Marketer

**Pasos:**
1. Intentar acceder a generar (debe denegar)
2. Intentar acceder a descargar (debe denegar)
3. Intentar acceder a aprobar (debe denegar)
4. Verificar acceso a lista y detalles (debe permitir)

**Resultado Esperado:**
- Viewer NO puede generar publicaciones
- Viewer NO puede descargar paquetes
- Viewer NO puede aprobar publicaciones
- Viewer PUEDE ver lista y detalles (solo lectura)

---

## Multi-Tenant

### TC-PUB-039: Lista de publicaciones filtra por TenantId

**Módulo:** Publicaciones  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene 5 publicaciones
- Tenant2 tiene 10 publicaciones

**Pasos:**
1. Usuario de Tenant1 accede a `/Publishing/Index`
2. Verificar publicaciones mostradas
3. Usuario de Tenant2 accede a `/Publishing/Index`
4. Verificar publicaciones mostradas

**Resultado Esperado:**
- Usuario de Tenant1 ve solo sus 5 publicaciones
- Usuario de Tenant2 ve solo sus 10 publicaciones
- No hay mezcla de datos entre tenants
- Query filtra correctamente por TenantId

---

### TC-PUB-040: Publicación generada se asigna al tenant correcto

**Módulo:** Publicaciones  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos

**Pasos:**
1. Usuario de Tenant1 genera nueva publicación
2. Verificar en base de datos que TenantId de la publicación es Tenant1

**Resultado Esperado:**
- Publicación se crea con `TenantId = Tenant1` (del usuario)
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene

---

### TC-PUB-041: Acceso a publicaciones sin TenantId

**Módulo:** Publicaciones  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId válido
- Usuario NO es SuperAdmin

**Pasos:**
1. Intentar acceder a `/Publishing/Index` sin TenantId

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestran publicaciones

---

## Workflow de Publicación

### TC-PUB-042: Workflow completo Fase A (Manual)

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene permisos
- Campaña existe
- RequiresApproval = true

**Pasos:**
1. Generar publicación con RequiresApproval = true
2. Verificar que Status = "Pending"
3. Esperar procesamiento por background service
4. Verificar que Status cambia a "RequiresApproval"
5. Verificar que DownloadUrl está disponible
6. Descargar paquete
7. Aprobar publicación con PublishedUrl y ExternalPostId
8. Verificar que Status cambia a "Success"

**Resultado Esperado:**
- Flujo completo funciona correctamente:
  - Generate → Pending
  - Processing → RequiresApproval (con DownloadUrl)
  - Approve → Success (con PublishedUrl y ExternalPostId)
- Cada paso se ejecuta correctamente
- Datos se persisten en cada etapa

---

### TC-PUB-043: Publicación programada (ScheduledDate)

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Fecha programada es futura

**Pasos:**
1. Generar publicación con ScheduledDate en el futuro
2. Verificar que ScheduledDate se guarda correctamente
3. Verificar que publicación se programa para esa fecha

**Resultado Esperado:**
- ScheduledDate se guarda correctamente
- Background service procesa publicación en la fecha programada
- Publicación no se procesa antes de la fecha

---

### TC-PUB-044: Publicación sin aprobación requerida (RequiresApproval = false)

**Módulo:** Publicaciones  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Adaptador puede publicar directamente

**Pasos:**
1. Generar publicación con RequiresApproval = false
2. Verificar procesamiento automático

**Resultado Esperado:**
- Si adaptador puede publicar, publicación se procesa automáticamente
- Si no puede publicar, se lanza InvalidOperationException
- Workflow se adapta según RequiresApproval

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-PUB-001 | Listar todas las publicaciones | Funcional | Crítica | - |
| TC-PUB-002 | Filtrar por campaña | Funcional | Alta | - |
| TC-PUB-003 | Filtrar por estado | Funcional | Alta | - |
| TC-PUB-004 | Filtrar por canal | Funcional | Alta | - |
| TC-PUB-005 | Filtrar con múltiples filtros | Funcional | Media | - |
| TC-PUB-006 | Listar sin publicaciones | Funcional | Media | - |
| TC-PUB-007 | Acceso sin autenticación | Seguridad | Crítica | - |
| TC-PUB-008 | Generar publicación exitosamente | Funcional | Crítica | - |
| TC-PUB-009 | Generar con todos los campos | Funcional | Alta | - |
| TC-PUB-010 | Generar desde campaña | Funcional | Alta | - |
| TC-PUB-011 | Generar desde MarketingPack | Funcional | Media | - |
| TC-PUB-012 | Generar sin permisos | Seguridad | Crítica | - |
| TC-PUB-013 | Validación ModelState | Funcional | Alta | - |
| TC-PUB-014 | Manejo de errores al generar | Funcional | Alta | - |
| TC-PUB-015 | Ver detalles de publicación | Funcional | Crítica | - |
| TC-PUB-016 | Ver detalles inexistente | Funcional | Alta | - |
| TC-PUB-017 | Ver detalles de otro tenant | Multi-tenant | Crítica | - |
| TC-PUB-018 | Ver detalles sin autenticación | Seguridad | Crítica | - |
| TC-PUB-019 | Descargar paquete exitosamente | Funcional | Alta | - |
| TC-PUB-020 | Descargar sin DownloadUrl | Funcional | Media | - |
| TC-PUB-021 | Descargar formato incorrecto | Funcional | Media | - |
| TC-PUB-022 | Descargar sin permisos | Seguridad | Alta | - |
| TC-PUB-023 | Descargar de otro tenant | Multi-tenant | Crítica | - |
| TC-PUB-024 | Aprobar publicación exitosamente | Funcional | Crítica | - |
| TC-PUB-025 | Aprobar con URL y PostId | Funcional | Alta | - |
| TC-PUB-026 | Aprobar sin permisos | Seguridad | Crítica | - |
| TC-PUB-027 | Aprobar publicación inexistente | Funcional | Media | - |
| TC-PUB-028 | Aprobar de otro tenant | Multi-tenant | Crítica | - |
| TC-PUB-029 | Aprobar - Manejo de errores | Funcional | Alta | - |
| TC-PUB-030 | Estado Pending | Funcional | Alta | - |
| TC-PUB-031 | Estado Processing | Funcional | Media | - |
| TC-PUB-032 | Estado RequiresApproval | Funcional | Alta | - |
| TC-PUB-033 | Estado Success | Funcional | Alta | - |
| TC-PUB-034 | Estado Failed | Funcional | Alta | - |
| TC-PUB-035 | Estado Cancelled | Funcional | Media | - |
| TC-PUB-036 | Filtros múltiples simultáneos | Funcional | Media | - |
| TC-PUB-037 | Permisos Marketer | Seguridad | Crítica | - |
| TC-PUB-038 | Permisos Viewer | Seguridad | Alta | - |
| TC-PUB-039 | Lista filtra por TenantId | Multi-tenant | Crítica | - |
| TC-PUB-040 | Publicación asignada al tenant correcto | Multi-tenant | Crítica | - |
| TC-PUB-041 | Acceso sin TenantId | Multi-tenant | Alta | - |
| TC-PUB-042 | Workflow completo Fase A | Funcional | Crítica | - |
| TC-PUB-043 | Publicación programada | Funcional | Alta | - |
| TC-PUB-044 | Publicación sin aprobación | Funcional | Media | - |

**Total de casos:** 44  
**Críticos:** 15  
**Altos:** 19  
**Medios:** 10

