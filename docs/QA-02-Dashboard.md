# Casos de Prueba - Dashboard Ejecutivo

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Dashboard Ejecutivo

---

## Índice

- [Acceso al Dashboard](#acceso-al-dashboard)
- [Visualización de Datos](#visualización-de-datos)
- [Estado del Sistema](#estado-del-sistema)
- [Resumen de Contenido](#resumen-de-contenido)
- [Estado de Automatizaciones](#estado-de-automatizaciones)
- [Campañas Recientes](#campañas-recientes)
- [Métricas Generales](#métricas-generales)
- [Multi-Tenant en Dashboard](#multi-tenant-en-dashboard)
- [Super Admin Dashboard](#super-admin-dashboard)
- [Manejo de Errores](#manejo-de-errores)

---

## Acceso al Dashboard

### TC-DASH-001: Acceso exitoso al dashboard después de login

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId válido
- Usuario tiene permisos para ver dashboard

**Pasos:**
1. Realizar login exitoso
2. Ser redirigido automáticamente a `/Home/Index`
3. O navegar manualmente a `/Home/Index`

**Resultado Esperado:**
- Dashboard se carga correctamente
- No se muestra error
- Todos los componentes del dashboard son visibles
- Datos se muestran según el tenant del usuario

---

### TC-DASH-002: Acceso al dashboard sin autenticación

**Módulo:** Dashboard  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado
- Sesión no existe o ha expirado

**Pasos:**
1. Cerrar sesión o eliminar cookies
2. Intentar acceder directamente a `/Home/Index`
3. O usar URL directa en navegador

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra contenido del dashboard
- Mensaje de login se presenta si aplica

---

### TC-DASH-003: Acceso al dashboard sin TenantId (usuario normal)

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado pero sin TenantId válido
- Usuario NO es SuperAdmin

**Pasos:**
1. Usuario autenticado sin TenantId en claims
2. Intentar acceder a `/Home/Index`

**Resultado Esperado:**
- Sistema detecta ausencia de TenantId
- Usuario es redirigido a `/Account/Login`
- Se registra warning en logs: "Usuario autenticado sin TenantId y no es super admin"
- No se muestra dashboard

---

## Visualización de Datos

### TC-DASH-004: Dashboard muestra datos correctos del tenant

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con TenantId válido
- Tenant tiene datos: campañas, contenido, automatizaciones
- Base de datos contiene información del tenant

**Pasos:**
1. Acceder a `/Home/Index`
2. Verificar que se cargan todos los componentes
3. Verificar que los datos mostrados corresponden al tenant del usuario

**Resultado Esperado:**
- Dashboard muestra:
  - SystemStatus con información correcta
  - ContentSummary con archivos del tenant
  - AutomationStatus con automatizaciones del tenant
  - RecentCampaigns con campañas del tenant (máximo 5)
  - Metrics con métricas agregadas del tenant
- No se muestran datos de otros tenants
- Datos se actualizan en tiempo real según estado actual

---

### TC-DASH-005: Dashboard con tenant sin datos

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con TenantId válido
- Tenant NO tiene campañas, contenido ni automatizaciones
- Tenant es nuevo o vacío

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar todos los componentes del dashboard

**Resultado Esperado:**
- Dashboard se carga sin errores
- Métricas muestran valores en cero:
  - TotalCampaigns = 0
  - ActiveCampaigns = 0
  - TotalBudget = 0
  - TotalSpent = 0
  - TotalContentGenerated = 0
- ContentSummary muestra:
  - TotalFiles = 0
  - ImagesCount = 0
  - VideosCount = 0
- RecentCampaigns muestra lista vacía
- AutomationStatus muestra contadores en cero
- Mensajes informativos se muestran si aplica (ej: "No hay campañas aún")

---

### TC-DASH-006: Dashboard con datos parciales

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene algunas campañas pero no contenido
- O tiene contenido pero no campañas
- O tiene automatizaciones pero no campañas

**Pasos:**
1. Acceder a `/Home/Index`
2. Verificar que cada sección muestra datos correctos según disponibilidad

**Resultado Esperado:**
- Secciones con datos se muestran correctamente
- Secciones sin datos muestran cero o lista vacía
- No se generan errores por datos faltantes
- Dashboard se renderiza completamente

---

## Estado del Sistema

### TC-DASH-007: SystemStatus muestra información correcta

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado
- Tenant tiene usuarios activos

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar sección SystemStatus

**Resultado Esperado:**
- SystemStatus muestra:
  - `IsActive = true`
  - `Status = "Active"`
  - `StatusMessage = "Sistema operativo y funcionando correctamente"`
  - `LastActivity` con timestamp actual
  - `ActiveUsers` con conteo de usuarios que hicieron login en las últimas 24 horas
- Información se actualiza correctamente

---

### TC-DASH-008: ActiveUsers cuenta solo usuarios del tenant

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant A tiene 3 usuarios activos (login en últimas 24h)
- Tenant B tiene 5 usuarios activos
- Ambos tenants existen en el sistema

**Pasos:**
1. Usuario de Tenant A accede a dashboard
2. Verificar contador ActiveUsers
3. Usuario de Tenant B accede a dashboard
4. Verificar contador ActiveUsers

**Resultado Esperado:**
- Usuario de Tenant A ve ActiveUsers = 3
- Usuario de Tenant B ve ActiveUsers = 5
- No se mezclan conteos entre tenants
- Solo se cuentan usuarios con `LastLoginAt > DateTime.UtcNow.AddHours(-24)`

---

## Resumen de Contenido

### TC-DASH-009: ContentSummary muestra estadísticas correctas

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene contenido cargado:
  - 10 imágenes
  - 5 videos
  - 8 archivos generados por IA
  - 7 archivos subidos por usuarios
  - Tamaño total conocido

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar sección ContentSummary

**Resultado Esperado:**
- ContentSummary muestra:
  - `TotalFiles = 15` (10 imágenes + 5 videos)
  - `ImagesCount = 10`
  - `VideosCount = 5`
  - `AiGeneratedCount = 8`
  - `UserUploadedCount = 7`
  - `TotalSizeBytes` con suma correcta de todos los archivos
- Cálculos son precisos

---

### TC-DASH-010: RecentContent muestra máximo 6 elementos más recientes

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene más de 6 archivos de contenido
- Archivos tienen diferentes fechas de creación

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar lista RecentContent

**Resultado Esperado:**
- Se muestran máximo 6 elementos
- Elementos están ordenados por `CreatedAt` descendente (más recientes primero)
- Cada elemento muestra:
  - Id
  - FileName (o "Sin nombre" si es null)
  - ContentType
  - IsAiGenerated
  - CreatedAt
- No se muestran más de 6 elementos aunque existan más

---

### TC-DASH-011: ContentSummary solo muestra contenido del tenant

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant A tiene 5 archivos
- Tenant B tiene 10 archivos
- Ambos tenants existen

**Pasos:**
1. Usuario de Tenant A accede a dashboard
2. Verificar ContentSummary
3. Usuario de Tenant B accede a dashboard
4. Verificar ContentSummary

**Resultado Esperado:**
- Usuario de Tenant A ve TotalFiles = 5
- Usuario de Tenant B ve TotalFiles = 10
- No se muestran archivos de otros tenants
- Filtrado por TenantId funciona correctamente

---

## Estado de Automatizaciones

### TC-DASH-012: AutomationStatus muestra contadores correctos

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene automatizaciones con diferentes estados:
  - 3 Running
  - 2 Paused
  - 5 Completed
  - 1 Error

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar sección AutomationStatus

**Resultado Esperado:**
- AutomationStatus muestra:
  - `RunningCount = 3`
  - `PausedCount = 2`
  - `CompletedCount = 5`
  - `ErrorCount = 1`
- Contadores son precisos
- ActiveAutomations lista muestra automatizaciones activas

---

### TC-DASH-013: NextExecution muestra próxima ejecución

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene automatizaciones programadas
- Al menos una automatización tiene `NextExecutionAt` definido

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar campo NextExecution en AutomationStatus

**Resultado Esperado:**
- `NextExecution` muestra la fecha/hora de la próxima ejecución programada
- O `null` si no hay ejecuciones programadas
- Fecha se muestra en formato legible

---

## Campañas Recientes

### TC-DASH-014: RecentCampaigns muestra máximo 5 campañas más recientes

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene más de 5 campañas
- Campañas tienen diferentes fechas de creación

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar lista RecentCampaigns

**Resultado Esperado:**
- Se muestran máximo 5 campañas
- Campañas están ordenadas por `CreatedAt` descendente
- Cada campaña muestra:
  - Id
  - Name
  - Status
  - StartDate
  - EndDate
  - Budget
  - SpentAmount
  - ContentCount
  - CreatedAt
- No se muestran más de 5 aunque existan más

---

### TC-DASH-015: RecentCampaigns solo muestra campañas del tenant

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant A tiene 3 campañas
- Tenant B tiene 8 campañas

**Pasos:**
1. Usuario de Tenant A accede a dashboard
2. Verificar RecentCampaigns
3. Usuario de Tenant B accede a dashboard
4. Verificar RecentCampaigns

**Resultado Esperado:**
- Usuario de Tenant A ve máximo 3 campañas (las que tiene)
- Usuario de Tenant B ve máximo 5 campañas (las 5 más recientes)
- No se muestran campañas de otros tenants
- Filtrado por TenantId funciona correctamente

---

### TC-DASH-016: RecentCampaigns con campañas sin presupuesto

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene campañas donde algunas no tienen Budget o SpentAmount

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar RecentCampaigns con campañas que tienen Budget null

**Resultado Esperado:**
- Campañas sin Budget muestran null o valor por defecto
- Campañas sin SpentAmount muestran null o 0
- No se generan errores por valores null
- Dashboard se renderiza correctamente

---

## Métricas Generales

### TC-DASH-017: Metrics muestra agregaciones correctas

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant tiene:
  - 10 campañas totales
  - 4 campañas activas (Status = "Active")
  - Presupuesto total: $50,000
  - Gasto total: $25,000
  - 15 archivos generados por IA

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar sección Metrics

**Resultado Esperado:**
- Metrics muestra:
  - `TotalCampaigns = 10`
  - `ActiveCampaigns = 4`
  - `TotalBudget = 50000` (suma de todos los Budget no null)
  - `TotalSpent = 25000` (suma de todos los SpentAmount no null)
  - `TotalContentGenerated = 15`
  - `TotalPublications = 0` (o valor real si está implementado)
  - `AverageCampaignPerformance = 85.5` (o valor calculado)
  - `MetricsDate` con timestamp actual
- Cálculos son precisos

---

### TC-DASH-018: Metrics con valores null en presupuestos

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene campañas donde algunas tienen Budget null
- Algunas tienen SpentAmount null

**Pasos:**
1. Acceder a `/Home/Index`
2. Revisar Metrics

**Resultado Esperado:**
- `TotalBudget` suma solo campañas con Budget no null
- `TotalSpent` suma solo campañas con SpentAmount no null
- Campañas con valores null no afectan los cálculos
- No se generan errores

---

### TC-DASH-019: Metrics solo agrega datos del tenant

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant A: 5 campañas, Budget total $10,000
- Tenant B: 8 campañas, Budget total $20,000

**Pasos:**
1. Usuario de Tenant A accede a dashboard
2. Verificar Metrics
3. Usuario de Tenant B accede a dashboard
4. Verificar Metrics

**Resultado Esperado:**
- Usuario de Tenant A ve TotalCampaigns = 5, TotalBudget = 10000
- Usuario de Tenant B ve TotalCampaigns = 8, TotalBudget = 20000
- No se mezclan datos entre tenants
- Agregaciones son correctas por tenant

---

## Multi-Tenant en Dashboard

### TC-DASH-020: Dashboard filtra todos los datos por TenantId

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Múltiples tenants existen con datos similares
- Cada tenant tiene su propio conjunto de datos

**Pasos:**
1. Usuario de Tenant A accede a dashboard
2. Verificar todas las secciones
3. Comparar con datos reales del tenant en base de datos

**Resultado Esperado:**
- Todas las consultas filtran por TenantId:
  - Campañas: solo del tenant
  - Contenido: solo del tenant
  - Automatizaciones: solo del tenant
  - Usuarios activos: solo del tenant
- No hay fuga de datos entre tenants
- Datos mostrados coinciden exactamente con datos del tenant en BD

---

### TC-DASH-021: Dashboard con TenantId inválido

**Módulo:** Dashboard  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene TenantId que no existe en base de datos
- O TenantId corresponde a tenant inactivo

**Pasos:**
1. Usuario con TenantId inválido intenta acceder a dashboard
2. Verificar comportamiento

**Resultado Esperado:**
- Sistema valida tenant antes de mostrar dashboard
- Si tenant no existe o está inactivo, se muestra error o redirección
- No se muestran datos
- Se registra intento en logs

---

## Super Admin Dashboard

### TC-DASH-022: Super Admin accede a dashboard sin TenantId

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario es SuperAdmin (IsSuperAdmin = true)
- Usuario tiene TenantId = Guid.Empty

**Pasos:**
1. SuperAdmin autenticado accede a `/Home/Index`
2. Verificar que dashboard se carga

**Resultado Esperado:**
- Dashboard se carga correctamente
- `effectiveTenantId = Guid.Empty` se usa en la query
- Dashboard puede mostrar datos agregados de todos los tenants o vista especial
- No se genera error por falta de TenantId
- Se permite acceso

---

### TC-DASH-023: Super Admin ve datos agregados (si aplica)

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- SuperAdmin accede a dashboard
- Sistema tiene lógica para mostrar datos agregados para super admin

**Pasos:**
1. SuperAdmin accede a `/Home/Index`
2. Revisar métricas y datos mostrados

**Resultado Esperado:**
- Si el sistema soporta vista agregada para super admin, se muestran datos de todos los tenants
- O se muestra vista especial indicando que es super admin
- Dashboard funciona sin errores

---

## Manejo de Errores

### TC-DASH-024: Dashboard maneja errores de base de datos gracefully

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Base de datos no está disponible
- O hay error de conexión
- O hay error en query

**Pasos:**
1. Simular error de base de datos
2. Intentar acceder a `/Home/Index`

**Resultado Esperado:**
- Error se captura en try-catch
- Dashboard muestra `new DashboardDto()` (vacío) en lugar de crashear
- Se registra error en logs: "Error al cargar datos del dashboard"
- Usuario ve dashboard vacío o mensaje de error amigable
- No se muestra stack trace al usuario

---

### TC-DASH-025: Dashboard con excepciones en queries individuales

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Una de las queries falla (ej: ContentSummary)
- Otras queries funcionan correctamente

**Pasos:**
1. Simular error en una sección específica
2. Acceder a dashboard

**Resultado Esperado:**
- Sistema maneja error sin afectar otras secciones
- Secciones que funcionan se muestran correctamente
- Sección con error muestra valores por defecto o se omite
- Error se registra en logs

---

### TC-DASH-026: Dashboard con datos corruptos o inconsistentes

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Base de datos tiene datos inconsistentes:
  - Campaña con TenantId null
  - Contenido con referencias inválidas
  - Valores fuera de rango esperado

**Pasos:**
1. Acceder a dashboard con datos inconsistentes
2. Verificar que dashboard se renderiza

**Resultado Esperado:**
- Dashboard se carga sin crashear
- Datos inconsistentes se filtran o manejan apropiadamente
- No se muestran datos corruptos
- Sistema es resiliente a datos malformados

---

## Rendimiento

### TC-DASH-027: Dashboard carga en tiempo razonable con muchos datos

**Módulo:** Dashboard  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene:
  - 100+ campañas
  - 500+ archivos de contenido
  - 50+ automatizaciones

**Pasos:**
1. Acceder a `/Home/Index`
2. Medir tiempo de carga

**Resultado Esperado:**
- Dashboard se carga en menos de 3 segundos
- Queries están optimizadas (usando índices, límites apropiados)
- Paginación o límites se aplican donde corresponde (ej: RecentCampaigns máximo 5)
- Experiencia de usuario es fluida

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-DASH-001 | Acceso exitoso al dashboard | Funcional | Crítica | - |
| TC-DASH-002 | Acceso sin autenticación | Seguridad | Crítica | - |
| TC-DASH-003 | Acceso sin TenantId (usuario normal) | Multi-tenant | Alta | - |
| TC-DASH-004 | Dashboard muestra datos correctos | Funcional | Crítica | - |
| TC-DASH-005 | Dashboard con tenant sin datos | Funcional | Alta | - |
| TC-DASH-006 | Dashboard con datos parciales | Funcional | Media | - |
| TC-DASH-007 | SystemStatus muestra información correcta | Funcional | Alta | - |
| TC-DASH-008 | ActiveUsers cuenta solo del tenant | Multi-tenant | Crítica | - |
| TC-DASH-009 | ContentSummary muestra estadísticas correctas | Funcional | Alta | - |
| TC-DASH-010 | RecentContent muestra máximo 6 elementos | Funcional | Media | - |
| TC-DASH-011 | ContentSummary solo del tenant | Multi-tenant | Crítica | - |
| TC-DASH-012 | AutomationStatus muestra contadores correctos | Funcional | Alta | - |
| TC-DASH-013 | NextExecution muestra próxima ejecución | Funcional | Media | - |
| TC-DASH-014 | RecentCampaigns muestra máximo 5 | Funcional | Alta | - |
| TC-DASH-015 | RecentCampaigns solo del tenant | Multi-tenant | Crítica | - |
| TC-DASH-016 | RecentCampaigns con valores null | Funcional | Media | - |
| TC-DASH-017 | Metrics muestra agregaciones correctas | Funcional | Crítica | - |
| TC-DASH-018 | Metrics con valores null | Funcional | Media | - |
| TC-DASH-019 | Metrics solo del tenant | Multi-tenant | Crítica | - |
| TC-DASH-020 | Dashboard filtra todos los datos por TenantId | Multi-tenant | Crítica | - |
| TC-DASH-021 | Dashboard con TenantId inválido | Multi-tenant | Alta | - |
| TC-DASH-022 | Super Admin accede sin TenantId | Funcional | Alta | - |
| TC-DASH-023 | Super Admin ve datos agregados | Funcional | Media | - |
| TC-DASH-024 | Manejo de errores de BD | Funcional | Alta | - |
| TC-DASH-025 | Dashboard con excepciones en queries | Funcional | Media | - |
| TC-DASH-026 | Dashboard con datos corruptos | Funcional | Media | - |
| TC-DASH-027 | Rendimiento con muchos datos | Funcional | Media | - |

**Total de casos:** 27  
**Críticos:** 8  
**Altos:** 10  
**Medios:** 9

