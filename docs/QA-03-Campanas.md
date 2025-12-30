# Casos de Prueba - Campañas

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Gestión de Campañas

---

## Índice

- [Listar Campañas](#listar-campañas)
- [Crear Campaña](#crear-campaña)
- [Ver Detalles de Campaña](#ver-detalles-de-campaña)
- [Editar Campaña](#editar-campaña)
- [Eliminar Campaña](#eliminar-campaña)
- [Validaciones de Datos](#validaciones-de-datos)
- [Roles y Permisos](#roles-y-permisos)
- [Multi-Tenant](#multi-tenant)
- [Estados de Campaña](#estados-de-campaña)

---

## Listar Campañas

### TC-CAMP-001: Listar todas las campañas del tenant

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId válido
- Tenant tiene múltiples campañas con diferentes estados

**Pasos:**
1. Navegar a `/Campaigns/Index`
2. Verificar que se muestra lista de campañas

**Resultado Esperado:**
- Lista muestra todas las campañas del tenant
- Campañas se muestran con información relevante (nombre, estado, fechas, presupuesto)
- No se muestran campañas de otros tenants
- Vista se carga sin errores

---

### TC-CAMP-002: Filtrar campañas por estado

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene campañas con diferentes estados: Draft, Active, Paused, Archived
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Campaigns/Index`
2. Aplicar filtro por estado (ej: `?status=Active`)
3. Verificar resultados

**Resultado Esperado:**
- Solo se muestran campañas con el estado seleccionado
- Filtro se aplica correctamente
- `ViewBag.StatusFilter` contiene el estado seleccionado
- URL refleja el filtro aplicado

---

### TC-CAMP-003: Listar campañas con tenant sin campañas

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant no tiene campañas creadas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Campaigns/Index`
2. Verificar vista

**Resultado Esperado:**
- Lista se muestra vacía
- Mensaje informativo se muestra si aplica (ej: "No hay campañas")
- No se generan errores
- Botón "Crear Campaña" está disponible

---

### TC-CAMP-004: Acceso a lista sin autenticación

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Campaigns/Index` sin sesión
2. Verificar comportamiento

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra contenido de campañas

---

## Crear Campaña

### TC-CAMP-005: Crear campaña exitosamente con datos mínimos

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Owner, Admin o Marketer
- Usuario tiene TenantId válido

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Verificar que formulario se carga con Status = "Draft" por defecto
3. Ingresar nombre: "Campaña Test 2024"
4. Dejar otros campos opcionales vacíos
5. Hacer clic en "Crear" o "Guardar"

**Resultado Esperado:**
- Campaña se crea exitosamente
- Status se establece como "Draft"
- TenantId se asigna automáticamente del usuario
- Usuario es redirigido a `/Campaigns/Details/{id}`
- TempData muestra: "Campaña creada exitosamente."
- Campaña se guarda en base de datos con todos los campos correctos

---

### TC-CAMP-006: Crear campaña con todos los campos completos

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos para crear

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Completar todos los campos:
   - Name: "Campaña Completa Q1 2024"
   - Description: "Descripción detallada de la campaña"
   - Status: "Active"
   - StartDate: Fecha futura
   - EndDate: Fecha posterior a StartDate
   - Budget: 50000.00
   - Objectives: JSON válido
   - TargetAudience: JSON válido
   - TargetChannels: Lista de canales
   - Notes: "Notas internas"
3. Hacer clic en "Crear"

**Resultado Esperado:**
- Campaña se crea con todos los campos
- JSON fields (Objectives, TargetAudience, TargetChannels) se serializan correctamente
- Fechas se guardan correctamente
- Presupuesto se guarda con precisión decimal
- Redirección a detalles exitosa

---

### TC-CAMP-007: Crear campaña - Validación nombre vacío

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Dejar campo "Name" vacío
3. Completar otros campos opcionales
4. Intentar crear campaña

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "El nombre de la campaña es obligatorio."
- Formulario se mantiene con datos ingresados
- Campaña NO se crea
- Usuario permanece en página de creación

---

### TC-CAMP-008: Crear campaña - Validación nombre muy largo

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Ingresar nombre con más de 200 caracteres
3. Intentar crear campaña

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "El nombre no puede exceder 200 caracteres."
- Campaña NO se crea

---

### TC-CAMP-009: Crear campaña - Validación descripción muy larga

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Ingresar descripción con más de 1000 caracteres
3. Intentar crear campaña

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "La descripción no puede exceder 1000 caracteres."
- Campaña NO se crea

---

### TC-CAMP-010: Crear campaña - Validación EndDate anterior a StartDate

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Ingresar StartDate: 2024-12-31
3. Ingresar EndDate: 2024-12-01 (anterior a StartDate)
4. Intentar crear campaña

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "La fecha de fin debe ser posterior a la fecha de inicio."
- Campaña NO se crea

---

### TC-CAMP-011: Crear campaña - Validación presupuesto negativo

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Ingresar Budget: -1000
3. Intentar crear campaña

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "El presupuesto no puede ser negativo."
- Campaña NO se crea

---

### TC-CAMP-012: Crear campaña - Validación estado inválido

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Campaigns/Create`
2. Modificar valor de Status a valor inválido (ej: "InvalidStatus")
3. Intentar crear campaña

**Resultado Esperado:**
- Validación falla
- Mensaje de error: "El estado debe ser: Draft, Active, Paused o Archived."
- Campaña NO se crea

---

### TC-CAMP-013: Crear campaña sin permisos (rol Viewer)

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol que NO incluye Owner, Admin o Marketer
- Usuario intenta acceder a crear campaña

**Pasos:**
1. Usuario sin permisos intenta acceder a `/Campaigns/Create`
2. Verificar comportamiento

**Resultado Esperado:**
- Acceso denegado
- Usuario es redirigido a `/Account/AccessDenied`
- No se puede crear campaña

---

### TC-CAMP-014: Crear campaña - Usuario no pertenece al tenant

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario A pertenece a Tenant1
- Usuario intenta crear campaña pero TenantId en request no coincide

**Pasos:**
1. Usuario autenticado con TenantId = Tenant1
2. Intentar crear campaña (sistema valida que usuario pertenece al tenant)

**Resultado Esperado:**
- Si usuario no pertenece al tenant, se lanza `UnauthorizedAccessException`
- Mensaje: "Usuario no pertenece a este tenant"
- Campaña NO se crea
- Error se maneja apropiadamente

---

## Ver Detalles de Campaña

### TC-CAMP-015: Ver detalles de campaña existente

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña existe en el tenant del usuario
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Campaigns/Details/{campaignId}`
2. Verificar que se muestran todos los detalles

**Resultado Esperado:**
- Detalles de campaña se muestran correctamente:
  - Nombre, descripción, estado
  - Fechas de inicio y fin
  - Presupuesto y gasto
  - Objetivos, audiencia, canales (deserializados desde JSON)
  - Notas
  - Fechas de creación y actualización
- Información es precisa y completa

---

### TC-CAMP-016: Ver detalles de campaña inexistente

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña con ID dado NO existe
- Usuario está autenticado

**Pasos:**
1. Intentar acceder a `/Campaigns/Details/{idInexistente}`
2. Verificar comportamiento

**Resultado Esperado:**
- Sistema retorna `NotFound()` (404)
- Página 404 se muestra
- No se muestra información de campaña

---

### TC-CAMP-017: Ver detalles de campaña de otro tenant

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña existe pero pertenece a Tenant2
- Usuario pertenece a Tenant1
- Usuario conoce el ID de la campaña de Tenant2

**Pasos:**
1. Usuario de Tenant1 intenta acceder a `/Campaigns/Details/{campaignIdDeTenant2}`
2. Verificar comportamiento

**Resultado Esperado:**
- Query filtra por TenantId del usuario
- Campaña NO se encuentra (porque pertenece a otro tenant)
- Sistema retorna `NotFound()`
- No hay fuga de información entre tenants

---

### TC-CAMP-018: Ver detalles sin autenticación

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Campaigns/Details/{id}` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestran detalles

---

## Editar Campaña

### TC-CAMP-019: Editar campaña exitosamente

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña existe en el tenant del usuario
- Usuario tiene rol Owner, Admin o Marketer
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Campaigns/Edit/{campaignId}`
2. Verificar que formulario se carga con datos actuales
3. Modificar campos (ej: nombre, descripción, estado)
4. Hacer clic en "Guardar"

**Resultado Esperado:**
- Formulario se carga con datos actuales de la campaña
- Campos opcionales se manejan correctamente (null si no existen)
- Campaña se actualiza exitosamente
- `UpdatedAt` se actualiza a fecha/hora actual
- Usuario es redirigido a `/Campaigns/Details/{id}`
- TempData muestra: "Campaña actualizada exitosamente."
- Cambios se reflejan en base de datos

---

### TC-CAMP-020: Editar campaña - Cargar formulario con datos correctos

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe con todos los campos completos
- Usuario tiene permisos para editar

**Pasos:**
1. Navegar a `/Campaigns/Edit/{campaignId}`
2. Verificar que todos los campos se cargan correctamente

**Resultado Esperado:**
- Formulario muestra:
  - Name, Description, Status correctos
  - StartDate, EndDate correctos
  - Budget correcto
  - Objectives, TargetAudience, TargetChannels deserializados correctamente
  - Notes correctos
- TargetChannels se muestra como lista (no null, sino lista vacía si no hay)

---

### TC-CAMP-021: Editar campaña - Validaciones aplican igual que crear

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe
- Usuario tiene permisos

**Pasos:**
1. Navegar a `/Campaigns/Edit/{campaignId}`
2. Intentar guardar con datos inválidos (nombre vacío, fechas incorrectas, etc.)
3. Verificar validaciones

**Resultado Esperado:**
- Mismas validaciones que crear campaña se aplican:
  - Nombre obligatorio y máximo 200 caracteres
  - Descripción máximo 1000 caracteres
  - EndDate posterior a StartDate
  - Budget no negativo
  - Status válido
- Errores se muestran en ModelState
- Campaña NO se actualiza

---

### TC-CAMP-022: Editar campaña inexistente

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña con ID dado NO existe
- Usuario tiene permisos

**Pasos:**
1. Intentar acceder a `/Campaigns/Edit/{idInexistente}`

**Resultado Esperado:**
- Sistema retorna `NotFound()`
- Página 404 se muestra

---

### TC-CAMP-023: Editar campaña de otro tenant

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta editar campaña de Tenant2

**Resultado Esperado:**
- Query filtra por TenantId
- Campaña NO se encuentra
- Sistema retorna `NotFound()`
- No hay acceso cruzado entre tenants

---

### TC-CAMP-024: Editar campaña sin permisos

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado sin rol Owner, Admin o Marketer

**Pasos:**
1. Intentar acceder a `/Campaigns/Edit/{id}` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Redirección a `/Account/AccessDenied`

---

## Eliminar Campaña

### TC-CAMP-025: Eliminar campaña exitosamente (soft delete)

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe en el tenant
- Usuario tiene rol Owner o Admin
- Campaña puede ser eliminada (sin restricciones)

**Pasos:**
1. Navegar a `/Campaigns/Details/{campaignId}`
2. Hacer clic en botón "Eliminar" (si está disponible)
3. Confirmar eliminación si se requiere

**Resultado Esperado:**
- Campaña se elimina (soft delete)
- Usuario es redirigido a `/Campaigns/Index`
- TempData muestra: "Campaña eliminada exitosamente."
- Campaña ya no aparece en lista (o aparece marcada como eliminada según implementación)

---

### TC-CAMP-026: Eliminar campaña sin permisos (rol Marketer)

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol Marketer (puede crear/editar pero NO eliminar)
- Campaña existe

**Pasos:**
1. Usuario Marketer intenta eliminar campaña
2. Verificar comportamiento

**Resultado Esperado:**
- Acceso denegado
- Botón eliminar no está visible o está deshabilitado
- Si intenta por URL directa, redirección a `/Account/AccessDenied`

---

### TC-CAMP-027: Eliminar campaña con restricciones (InvalidOperationException)

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña tiene publicaciones asociadas
- O campaña tiene contenido asociado que impide eliminación
- Usuario tiene permisos para eliminar

**Pasos:**
1. Intentar eliminar campaña con restricciones
2. Verificar comportamiento

**Resultado Esperado:**
- `InvalidOperationException` se lanza
- TempData muestra mensaje de error explicativo
- Usuario es redirigido a `/Campaigns/Details/{id}`
- Campaña NO se elimina
- Mensaje explica por qué no se puede eliminar

---

### TC-CAMP-028: Eliminar campaña inexistente

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Campaña con ID dado NO existe
- Usuario tiene permisos

**Pasos:**
1. Intentar eliminar campaña inexistente

**Resultado Esperado:**
- `NotFoundException` se lanza
- Sistema retorna `NotFound()`
- Página 404 se muestra

---

### TC-CAMP-029: Eliminar campaña de otro tenant

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
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
- Sistema retorna `NotFound()`
- No hay eliminación cruzada entre tenants

---

## Validaciones de Datos

### TC-CAMP-030: Campaña con fechas null es válida

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Crear campaña sin StartDate ni EndDate
2. Verificar que se guarda correctamente

**Resultado Esperado:**
- Campaña se crea exitosamente
- StartDate y EndDate son null
- No se generan errores por fechas null

---

### TC-CAMP-031: Campaña con presupuesto null es válida

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Crear campaña sin presupuesto
2. Verificar que se guarda correctamente

**Resultado Esperado:**
- Campaña se crea exitosamente
- Budget es null
- No se generan errores

---

### TC-CAMP-032: JSON fields se serializan y deserializan correctamente

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Crear campaña con:
   - Objectives: objeto JSON complejo
   - TargetAudience: objeto JSON
   - TargetChannels: array JSON
2. Guardar campaña
3. Ver detalles de campaña
4. Editar campaña

**Resultado Esperado:**
- Al crear: JSON se serializa correctamente a string en BD
- Al ver detalles: JSON se deserializa correctamente
- Al editar: JSON se carga correctamente en formulario
- Datos se mantienen íntegros en todo el ciclo

---

## Roles y Permisos

### TC-CAMP-033: Usuario Marketer puede crear y editar pero no eliminar

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Marketer"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a `/Campaigns/Create` (debe permitir)
2. Verificar acceso a `/Campaigns/Edit/{id}` (debe permitir)
3. Verificar acceso a eliminar (debe denegar)

**Resultado Esperado:**
- Marketer puede crear campañas
- Marketer puede editar campañas
- Marketer NO puede eliminar campañas
- Permisos se aplican correctamente según `[AuthorizeRole]`

---

### TC-CAMP-034: Usuario Owner puede crear, editar y eliminar

**Módulo:** Campañas  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Owner"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a crear (debe permitir)
2. Verificar acceso a editar (debe permitir)
3. Verificar acceso a eliminar (debe permitir)

**Resultado Esperado:**
- Owner tiene acceso completo a todas las operaciones
- Permisos funcionan correctamente

---

## Multi-Tenant

### TC-CAMP-035: Lista de campañas filtra por TenantId

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene 5 campañas
- Tenant2 tiene 8 campañas
- Ambos tenants existen

**Pasos:**
1. Usuario de Tenant1 accede a `/Campaigns/Index`
2. Verificar campañas mostradas
3. Usuario de Tenant2 accede a `/Campaigns/Index`
4. Verificar campañas mostradas

**Resultado Esperado:**
- Usuario de Tenant1 ve solo sus 5 campañas
- Usuario de Tenant2 ve solo sus 8 campañas
- No hay mezcla de datos entre tenants
- Query filtra correctamente por TenantId

---

### TC-CAMP-036: Campaña creada se asigna al tenant correcto

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos

**Pasos:**
1. Usuario de Tenant1 crea nueva campaña
2. Verificar en base de datos que TenantId de la campaña es Tenant1

**Resultado Esperado:**
- Campaña se crea con `TenantId = Tenant1` (del usuario)
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene

---

### TC-CAMP-037: Acceso a campañas sin TenantId

**Módulo:** Campañas  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId válido
- Usuario NO es SuperAdmin

**Pasos:**
1. Intentar acceder a `/Campaigns/Index` sin TenantId

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestran campañas

---

## Estados de Campaña

### TC-CAMP-038: Crear campaña con estado Draft

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Crear campaña sin especificar estado
2. Verificar estado por defecto

**Resultado Esperado:**
- Estado se establece como "Draft" por defecto
- Campaña se crea correctamente

---

### TC-CAMP-039: Cambiar estado de campaña

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe con estado "Draft"
- Usuario tiene permisos para editar

**Pasos:**
1. Editar campaña
2. Cambiar estado de "Draft" a "Active"
3. Guardar cambios

**Resultado Esperado:**
- Estado se actualiza correctamente
- Campaña refleja nuevo estado en detalles
- Cambio se persiste en base de datos

---

### TC-CAMP-040: Estados válidos son Draft, Active, Paused, Archived

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Crear/editar campaña con cada estado válido
2. Verificar que todos se aceptan

**Resultado Esperado:**
- Estados "Draft", "Active", "Paused", "Archived" son válidos
- Cualquier otro estado es rechazado
- Validación funciona correctamente

---

## Manejo de Errores

### TC-CAMP-041: Manejo de errores al crear campaña

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error de base de datos o conexión

**Pasos:**
1. Intentar crear campaña cuando hay error de sistema
2. Verificar manejo de error

**Resultado Esperado:**
- Error se captura en try-catch
- Mensaje genérico se muestra: "Error al crear la campaña. Por favor, intente nuevamente."
- Error se registra en logs
- Usuario permanece en formulario con datos ingresados
- No se muestra stack trace al usuario

---

### TC-CAMP-042: Manejo de ValidationException al crear

**Módulo:** Campañas  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario intenta crear campaña con datos inválidos

**Pasos:**
1. Crear campaña con datos que fallan validación de FluentValidation
2. Verificar manejo

**Resultado Esperado:**
- `ValidationException` se captura
- Errores se agregan a ModelState por PropertyName
- Mensajes de error específicos se muestran en formulario
- Usuario puede corregir y reintentar

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-CAMP-001 | Listar todas las campañas | Funcional | Crítica | - |
| TC-CAMP-002 | Filtrar campañas por estado | Funcional | Alta | - |
| TC-CAMP-003 | Listar con tenant sin campañas | Funcional | Media | - |
| TC-CAMP-004 | Acceso sin autenticación | Seguridad | Crítica | - |
| TC-CAMP-005 | Crear campaña con datos mínimos | Funcional | Crítica | - |
| TC-CAMP-006 | Crear campaña completa | Funcional | Alta | - |
| TC-CAMP-007 | Validación nombre vacío | Funcional | Alta | - |
| TC-CAMP-008 | Validación nombre muy largo | Funcional | Media | - |
| TC-CAMP-009 | Validación descripción muy larga | Funcional | Media | - |
| TC-CAMP-010 | Validación EndDate anterior a StartDate | Funcional | Alta | - |
| TC-CAMP-011 | Validación presupuesto negativo | Funcional | Alta | - |
| TC-CAMP-012 | Validación estado inválido | Funcional | Media | - |
| TC-CAMP-013 | Crear sin permisos | Seguridad | Crítica | - |
| TC-CAMP-014 | Crear - Usuario no pertenece al tenant | Multi-tenant | Crítica | - |
| TC-CAMP-015 | Ver detalles de campaña | Funcional | Crítica | - |
| TC-CAMP-016 | Ver detalles inexistente | Funcional | Alta | - |
| TC-CAMP-017 | Ver detalles de otro tenant | Multi-tenant | Crítica | - |
| TC-CAMP-018 | Ver detalles sin autenticación | Seguridad | Crítica | - |
| TC-CAMP-019 | Editar campaña exitosamente | Funcional | Crítica | - |
| TC-CAMP-020 | Cargar formulario edición | Funcional | Alta | - |
| TC-CAMP-021 | Validaciones al editar | Funcional | Alta | - |
| TC-CAMP-022 | Editar campaña inexistente | Funcional | Alta | - |
| TC-CAMP-023 | Editar campaña de otro tenant | Multi-tenant | Crítica | - |
| TC-CAMP-024 | Editar sin permisos | Seguridad | Crítica | - |
| TC-CAMP-025 | Eliminar campaña exitosamente | Funcional | Alta | - |
| TC-CAMP-026 | Eliminar sin permisos | Seguridad | Crítica | - |
| TC-CAMP-027 | Eliminar con restricciones | Funcional | Alta | - |
| TC-CAMP-028 | Eliminar campaña inexistente | Funcional | Media | - |
| TC-CAMP-029 | Eliminar campaña de otro tenant | Multi-tenant | Crítica | - |
| TC-CAMP-030 | Campaña con fechas null | Funcional | Media | - |
| TC-CAMP-031 | Campaña con presupuesto null | Funcional | Media | - |
| TC-CAMP-032 | JSON fields serialización | Funcional | Alta | - |
| TC-CAMP-033 | Permisos Marketer | Seguridad | Crítica | - |
| TC-CAMP-034 | Permisos Owner | Seguridad | Crítica | - |
| TC-CAMP-035 | Lista filtra por TenantId | Multi-tenant | Crítica | - |
| TC-CAMP-036 | Campaña asignada al tenant correcto | Multi-tenant | Crítica | - |
| TC-CAMP-037 | Acceso sin TenantId | Multi-tenant | Alta | - |
| TC-CAMP-038 | Crear con estado Draft | Funcional | Media | - |
| TC-CAMP-039 | Cambiar estado de campaña | Funcional | Alta | - |
| TC-CAMP-040 | Estados válidos | Funcional | Alta | - |
| TC-CAMP-041 | Manejo de errores al crear | Funcional | Alta | - |
| TC-CAMP-042 | Manejo de ValidationException | Funcional | Alta | - |

**Total de casos:** 42  
**Críticos:** 16  
**Altos:** 19  
**Medios:** 7

