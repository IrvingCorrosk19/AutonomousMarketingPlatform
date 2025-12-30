# Casos de Prueba - Multi-Tenant

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Multi-Tenant

---

## Índice

- [Resolución de Tenant](#resolución-de-tenant)
- [Validación de Tenant](#validación-de-tenant)
- [Aislamiento de Datos](#aislamiento-de-datos)
- [Super Admin](#super-admin)
- [Excepciones y Exclusiones](#excepciones-y-exclusiones)
- [Configuración Multi-Tenant](#configuración-multi-tenant)

---

## Resolución de Tenant

### TC-MT-001: Resolver tenant desde header X-Tenant-Id

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant existe y está activo
- Request incluye header `X-Tenant-Id` con GUID válido

**Pasos:**
1. Realizar request con header `X-Tenant-Id: {tenantGuid}`
2. Verificar que tenant se resuelve correctamente

**Resultado Esperado:**
- TenantId se resuelve desde header (prioridad más alta)
- TenantId se valida que existe en base de datos
- TenantId se agrega a `context.Items["ResolvedTenantId"]`
- Resolución funciona correctamente

---

### TC-MT-002: Resolver tenant desde subdomain

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant existe con Subdomain configurado
- Tenant está activo
- Request viene desde subdomain (ej: tenant1.miapp.com)

**Pasos:**
1. Realizar request desde subdomain: `https://tenant1.miapp.com/Home/Index`
2. Verificar que tenant se resuelve desde subdomain

**Resultado Esperado:**
- Subdomain se extrae correctamente (primer parte del host)
- Tenant se busca por Subdomain en base de datos
- TenantId se resuelve correctamente
- Si subdomain no existe o tenant está inactivo, no se resuelve

---

### TC-MT-003: Resolver tenant desde claim del usuario

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene claim "TenantId" con GUID válido
- No hay header X-Tenant-Id ni subdomain

**Pasos:**
1. Usuario autenticado hace request sin header ni subdomain
2. Verificar que tenant se resuelve desde claim

**Resultado Esperado:**
- TenantId se obtiene del claim "TenantId" del usuario
- Claim se parsea correctamente a GUID
- TenantId se resuelve como fallback (prioridad 3)
- Resolución funciona correctamente

---

### TC-MT-004: Prioridad de resolución: Header > Subdomain > Claim

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Request tiene header X-Tenant-Id, subdomain y claim con diferentes tenant IDs

**Pasos:**
1. Realizar request con:
   - Header: X-Tenant-Id = Tenant1
   - Subdomain: tenant2.miapp.com
   - Claim: TenantId = Tenant3
2. Verificar qué tenant se resuelve

**Resultado Esperado:**
- Header tiene prioridad más alta
- Tenant1 se resuelve (desde header)
- Subdomain y claim se ignoran cuando header está presente
- Prioridad se respeta correctamente

---

### TC-MT-005: Resolver tenant - Header inválido (GUID malformado)

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Request incluye header X-Tenant-Id con valor inválido (no GUID)

**Pasos:**
1. Realizar request con header `X-Tenant-Id: invalid-guid`
2. Verificar comportamiento

**Resultado Esperado:**
- Header se ignora (no se puede parsear a GUID)
- Sistema intenta siguiente método de resolución (subdomain o claim)
- No se genera error, se continúa con siguiente prioridad

---

### TC-MT-006: Resolver tenant - Subdomain no existe

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Request viene desde subdomain que no existe en base de datos

**Pasos:**
1. Realizar request desde subdomain inexistente: `https://nonexistent.miapp.com`
2. Verificar comportamiento

**Resultado Esperado:**
- Subdomain no se encuentra en base de datos
- Sistema intenta siguiente método (claim)
- No se genera error
- Si no hay claim, tenant no se resuelve

---

### TC-MT-007: Resolver tenant - Subdomain con tenant inactivo

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant existe pero IsActive = false
- Request viene desde subdomain de ese tenant

**Pasos:**
1. Realizar request desde subdomain de tenant inactivo
2. Verificar comportamiento

**Resultado Esperado:**
- Tenant se busca pero se valida que IsActive = true
- Tenant inactivo no se resuelve
- Sistema intenta siguiente método de resolución
- Validación funciona correctamente

---

## Validación de Tenant

### TC-MT-008: Validar tenant existe y está activo

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- TenantId se resolvió
- Tenant existe en base de datos
- Tenant está activo (IsActive = true)

**Pasos:**
1. Realizar request con TenantId válido y activo
2. Verificar que validación pasa

**Resultado Esperado:**
- `ValidateTenantAsync` retorna true
- Request continúa normalmente
- TenantId se agrega a `context.Items["TenantId"]`
- Validación funciona correctamente

---

### TC-MT-009: Validar tenant inexistente

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- TenantId resuelto no existe en base de datos

**Pasos:**
1. Realizar request con TenantId que no existe
2. Verificar comportamiento

**Resultado Esperado:**
- `ValidateTenantAsync` retorna false
- StatusCode 403 se retorna
- Error JSON: `{ error: { code: "INVALID_TENANT", message: "Tenant no válido o inactivo" } }`
- Warning se registra: "Intento de acceso con tenant inválido"
- Request NO continúa

---

### TC-MT-010: Validar tenant inactivo

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant existe pero IsActive = false

**Pasos:**
1. Realizar request con TenantId inactivo
2. Verificar comportamiento

**Resultado Esperado:**
- `ValidateTenantAsync` retorna false (porque IsActive = false)
- StatusCode 403 se retorna
- Error JSON se muestra
- Request NO continúa
- Tenants inactivos no pueden acceder

---

### TC-MT-011: Request sin TenantId - Usuario no autenticado

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario NO está autenticado
- No hay TenantId en header, subdomain ni claim

**Pasos:**
1. Realizar request sin TenantId y sin autenticación
2. Verificar comportamiento

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- Request NO continúa
- Redirección funciona correctamente

---

### TC-MT-012: Request sin TenantId - Usuario autenticado (no super admin)

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado pero NO es SuperAdmin
- No hay TenantId resuelto

**Pasos:**
1. Realizar request sin TenantId
2. Verificar comportamiento

**Resultado Esperado:**
- StatusCode 400 se retorna
- Error JSON: `{ error: { code: "MISSING_TENANT", message: "TenantId es requerido" } }`
- Warning se registra: "Request sin TenantId y no es super admin"
- Request NO continúa

---

## Aislamiento de Datos

### TC-MT-013: Datos se filtran por TenantId en todas las queries

**Módulo:** Multi-Tenant  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene campañas, contenido, publicaciones, métricas
- Tenant2 tiene datos diferentes
- Ambos tenants existen

**Pasos:**
1. Usuario de Tenant1 accede a diferentes módulos
2. Verificar que solo ve datos de Tenant1
3. Usuario de Tenant2 accede a mismos módulos
4. Verificar que solo ve datos de Tenant2

**Resultado Esperado:**
- Todas las queries filtran por TenantId:
  - Campañas: solo del tenant
  - Contenido: solo del tenant
  - Publicaciones: solo del tenant
  - Métricas: solo del tenant
  - Memorias: solo del tenant
  - Consentimientos: solo del tenant
  - Configuración IA: solo del tenant
- No hay fuga de datos entre tenants
- Aislamiento es completo

---

### TC-MT-014: Crear entidad se asigna al tenant correcto

**Módulo:** Multi-Tenant  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos para crear

**Pasos:**
1. Usuario de Tenant1 crea diferentes entidades (campaña, contenido, etc.)
2. Verificar en base de datos que TenantId es correcto

**Resultado Esperado:**
- Todas las entidades se crean con TenantId del usuario
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene
- Aplicable a: Campaign, Content, PublishingJob, CampaignMetrics, PublishingJobMetrics, MarketingMemory, Consent, TenantAIConfig

---

### TC-MT-015: Usuario no puede acceder a entidad de otro tenant

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1
- Usuario conoce el ID de la campaña de Tenant2

**Pasos:**
1. Usuario de Tenant1 intenta acceder a campaña de Tenant2 por ID
2. Verificar comportamiento

**Resultado Esperado:**
- Query filtra por TenantId del usuario
- Campaña NO se encuentra (porque pertenece a otro tenant)
- Sistema retorna NotFound()
- No hay fuga de información
- Seguridad se mantiene

---

### TC-MT-016: Usuario no puede modificar entidad de otro tenant

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1
- Usuario tiene permisos para editar

**Pasos:**
1. Usuario de Tenant1 intenta editar campaña de Tenant2

**Resultado Esperado:**
- Query filtra por TenantId
- Campaña NO se encuentra
- Sistema retorna NotFound()
- No se puede modificar entidad de otro tenant

---

### TC-MT-017: Usuario no puede eliminar entidad de otro tenant

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1
- Usuario tiene permisos para eliminar

**Pasos:**
1. Usuario de Tenant1 intenta eliminar campaña de Tenant2

**Resultado Esperado:**
- Query filtra por TenantId
- Campaña NO se encuentra
- Sistema retorna NotFound()
- No se puede eliminar entidad de otro tenant

---

## Super Admin

### TC-MT-018: Super Admin accede sin TenantId

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario es SuperAdmin (IsSuperAdmin = true)
- No hay TenantId resuelto

**Pasos:**
1. SuperAdmin realiza request sin TenantId
2. Verificar comportamiento

**Resultado Esperado:**
- Sistema detecta que es SuperAdmin
- Acceso se permite sin TenantId
- Information se registra: "SuperAdmin accediendo sin TenantId: Path={Path}"
- Request continúa normalmente
- SuperAdmin puede acceder a funcionalidades especiales

---

### TC-MT-019: Super Admin con TenantId = Guid.Empty

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario es SuperAdmin
- TenantId en claim es Guid.Empty

**Pasos:**
1. SuperAdmin realiza request con TenantId = Guid.Empty
2. Verificar comportamiento

**Resultado Esperado:**
- Guid.Empty se acepta para SuperAdmin
- Validación de tenant se omite
- Acceso se permite
- SuperAdmin funciona correctamente

---

## Excepciones y Exclusiones

### TC-MT-020: Archivos estáticos se excluyen de validación

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Archivos estáticos disponibles

**Pasos:**
1. Acceder a archivos estáticos:
   - `/css/custom.css`
   - `/js/site.js`
   - `/images/logo.png`
   - `/favicon.ico`
2. Verificar que validación se omite

**Resultado Esperado:**
- Archivos estáticos se sirven sin validación de tenant
- Validación se omite para:
  - `/css/`, `/js/`, `/images/`, `/img/`, `/fonts/`, `/lib/`
  - Extensiones: .css, .js, .png, .jpg, .jpeg, .gif, .svg, .ico, .woff, .woff2, .ttf, .eot
- Archivos se cargan correctamente

---

### TC-MT-021: Endpoints públicos se excluyen de validación

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Endpoints públicos configurados

**Pasos:**
1. Acceder a endpoints públicos:
   - `/health`
   - `/api/public/*`
   - `/Account/Login`
   - `/Account/AccessDenied`
2. Verificar que validación se omite

**Resultado Esperado:**
- Endpoints públicos se acceden sin validación de tenant
- Validación se omite correctamente
- Endpoints funcionan sin requerir tenant

---

### TC-MT-022: Validación puede estar deshabilitada

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Configuración `MultiTenant:ValidationEnabled = false`

**Pasos:**
1. Realizar request con validación deshabilitada
2. Verificar comportamiento

**Resultado Esperado:**
- Si `_validationEnabled = false`, validación se omite completamente
- Request continúa sin validar tenant
- Configuración funciona correctamente

---

## Configuración Multi-Tenant

### TC-MT-023: TenantId se agrega a context.Items después de validación

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- TenantId se resolvió y validó correctamente

**Pasos:**
1. Realizar request con tenant válido
2. Verificar que TenantId está en context.Items

**Resultado Esperado:**
- `context.Items["TenantId"]` contiene el TenantId validado
- Controladores pueden acceder a TenantId desde context.Items
- TenantId está disponible para todo el pipeline

---

### TC-MT-024: Resolución de tenant se cachea en HttpContext

**Módulo:** Multi-Tenant  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- TenantId se resolvió una vez

**Pasos:**
1. Resolver tenant en primer request
2. Verificar que se cachea en HttpContext.Items

**Resultado Esperado:**
- TenantId se agrega a `httpContext.Items["ResolvedTenantId"]`
- Resolución subsecuente puede usar valor cacheado
- Performance se optimiza

---

## Validación de Usuario Pertenece a Tenant

### TC-MT-025: Validar usuario pertenece al tenant

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Request tiene TenantId = Tenant1

**Pasos:**
1. Usuario realiza operación (crear, editar, etc.)
2. Sistema valida que usuario pertenece al tenant

**Resultado Esperado:**
- `ValidateUserBelongsToTenantAsync` valida correctamente
- Si usuario pertenece al tenant, operación continúa
- Validación se aplica en comandos críticos

---

### TC-MT-026: Usuario no pertenece al tenant - Operación rechazada

**Módulo:** Multi-Tenant  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Request intenta operar con Tenant2

**Pasos:**
1. Usuario de Tenant1 intenta crear entidad para Tenant2
2. Verificar validación

**Resultado Esperado:**
- `ValidateUserBelongsToTenantAsync` retorna false
- UnauthorizedAccessException se lanza
- Mensaje: "Usuario no pertenece a este tenant"
- Operación NO se ejecuta
- Seguridad se mantiene

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-MT-001 | Resolver desde header X-Tenant-Id | Funcional | Crítica | - |
| TC-MT-002 | Resolver desde subdomain | Funcional | Alta | - |
| TC-MT-003 | Resolver desde claim | Funcional | Crítica | - |
| TC-MT-004 | Prioridad de resolución | Funcional | Crítica | - |
| TC-MT-005 | Header inválido | Funcional | Alta | - |
| TC-MT-006 | Subdomain no existe | Funcional | Media | - |
| TC-MT-007 | Subdomain con tenant inactivo | Funcional | Alta | - |
| TC-MT-008 | Validar tenant existe y activo | Funcional | Crítica | - |
| TC-MT-009 | Validar tenant inexistente | Seguridad | Crítica | - |
| TC-MT-010 | Validar tenant inactivo | Seguridad | Crítica | - |
| TC-MT-011 | Request sin TenantId - No autenticado | Funcional | Alta | - |
| TC-MT-012 | Request sin TenantId - Autenticado | Funcional | Alta | - |
| TC-MT-013 | Datos filtran por TenantId | Multi-tenant | Crítica | - |
| TC-MT-014 | Crear entidad asignada al tenant | Multi-tenant | Crítica | - |
| TC-MT-015 | No acceder a entidad otro tenant | Seguridad | Crítica | - |
| TC-MT-016 | No modificar entidad otro tenant | Seguridad | Crítica | - |
| TC-MT-017 | No eliminar entidad otro tenant | Seguridad | Crítica | - |
| TC-MT-018 | Super Admin sin TenantId | Funcional | Alta | - |
| TC-MT-019 | Super Admin con Guid.Empty | Funcional | Alta | - |
| TC-MT-020 | Archivos estáticos excluidos | Funcional | Alta | - |
| TC-MT-021 | Endpoints públicos excluidos | Funcional | Alta | - |
| TC-MT-022 | Validación deshabilitada | Funcional | Media | - |
| TC-MT-023 | TenantId en context.Items | Funcional | Alta | - |
| TC-MT-024 | Resolución cacheada | Funcional | Media | - |
| TC-MT-025 | Validar usuario pertenece al tenant | Seguridad | Crítica | - |
| TC-MT-026 | Usuario no pertenece - Rechazado | Seguridad | Crítica | - |

**Total de casos:** 26  
**Críticos:** 15  
**Altos:** 9  
**Medios:** 2

