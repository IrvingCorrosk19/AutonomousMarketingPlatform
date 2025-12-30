# Casos de Prueba - Memoria de Marketing

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Memoria de Marketing

---

## Índice

- [Listar Memoria de Marketing](#listar-memoria-de-marketing)
- [Filtros de Memoria](#filtros-de-memoria)
- [Memoria por Campaña](#memoria-por-campaña)
- [Contexto de Memoria para IA](#contexto-de-memoria-para-ia)
- [Tipos de Memoria](#tipos-de-memoria)
- [Multi-Tenant](#multi-tenant)
- [Ordenamiento y Límites](#ordenamiento-y-límites)

---

## Listar Memoria de Marketing

### TC-MEM-001: Listar todas las memorias del tenant

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId válido
- Tenant tiene memorias almacenadas

**Pasos:**
1. Navegar a `/Memory/Index`
2. Verificar que se muestra lista de memorias

**Resultado Esperado:**
- Lista muestra memorias del tenant
- Memorias se muestran con información relevante:
  - Tipo de memoria
  - Contenido
  - Tags
  - Relevancia (RelevanceScore)
  - Fecha de memoria
  - Asociación con campaña (si aplica)
- No se muestran memorias de otros tenants
- Vista se carga sin errores
- Máximo 50 memorias se muestran (Limit = 50)

---

### TC-MEM-002: Listar memorias sin memorias almacenadas

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant no tiene memorias almacenadas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/Index`
2. Verificar vista

**Resultado Esperado:**
- Lista se muestra vacía
- Mensaje informativo se muestra si aplica
- No se generan errores
- Vista se carga correctamente

---

### TC-MEM-003: Acceso a memoria sin autenticación

**Módulo:** Memoria de Marketing  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Memory/Index` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra contenido

---

### TC-MEM-004: Acceso a memoria sin TenantId

**Módulo:** Memoria de Marketing  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId válido
- Usuario NO es SuperAdmin

**Pasos:**
1. Intentar acceder a `/Memory/Index` sin TenantId

**Resultado Esperado:**
- Warning se registra en logs: "Usuario autenticado sin TenantId"
- Usuario es redirigido a `/Account/Login`
- No se muestran memorias

---

## Filtros de Memoria

### TC-MEM-005: Filtrar memorias por tipo

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene memorias de diferentes tipos
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/Index?type=Learning`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran memorias de tipo "Learning"
- Filtro se aplica correctamente
- `ViewBag.SelectedType` contiene el tipo seleccionado
- `ViewBag.MemoryTypes` contiene todos los tipos disponibles: Conversation, Decision, Learning, Feedback, Pattern, Preference

---

### TC-MEM-006: Filtrar memorias por tags

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene memorias con diferentes tags
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/Index?tags=marketing,estrategia`
2. Verificar resultados

**Resultado Esperado:**
- Solo se muestran memorias que contienen alguno de los tags especificados
- Tags se separan por coma
- Búsqueda es case-insensitive
- `ViewBag.SelectedTags` contiene los tags seleccionados

---

### TC-MEM-007: Filtrar memorias por tipo y tags simultáneamente

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene memorias variadas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/Index?type=Decision&tags=marketing`
2. Verificar resultados

**Resultado Esperado:**
- Se aplican ambos filtros simultáneamente
- Solo se muestran memorias que cumplen AMBOS criterios:
  - Tipo = "Decision"
  - Y contiene tag "marketing"
- Filtros se mantienen en ViewBag

---

### TC-MEM-008: Filtrar con tipo inexistente

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/Index?type=InvalidType`
2. Verificar resultados

**Resultado Esperado:**
- No se muestran memorias (ninguna coincide con tipo inválido)
- No se generan errores
- Lista se muestra vacía o con mensaje apropiado

---

## Memoria por Campaña

### TC-MEM-009: Ver memorias de una campaña específica

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe en el tenant
- Campaña tiene memorias asociadas
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/Campaign?campaignId={campaignId}`
2. Verificar que se muestran memorias de la campaña

**Resultado Esperado:**
- Solo se muestran memorias asociadas a la campaña especificada
- CampaignId se filtra correctamente
- Máximo 100 memorias se muestran (Limit = 100)
- Memorias se muestran con información completa

---

### TC-MEM-010: Ver memorias de campaña sin memorias

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Campaña existe pero NO tiene memorias asociadas
- Usuario está autenticado

**Pasos:**
1. Acceder a memorias de campaña sin memorias

**Resultado Esperado:**
- Lista se muestra vacía
- No se generan errores
- Vista se carga correctamente

---

### TC-MEM-011: Ver memorias de campaña inexistente

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Campaña con ID dado NO existe
- O campaña pertenece a otro tenant
- Usuario está autenticado

**Pasos:**
1. Intentar acceder a `/Memory/Campaign?campaignId={idInexistente}`

**Resultado Esperado:**
- No se muestran memorias (filtro no encuentra campaña)
- No se generan errores
- Lista se muestra vacía

---

### TC-MEM-012: Ver memorias de campaña de otro tenant

**Módulo:** Memoria de Marketing  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta ver memorias de campaña de Tenant2

**Resultado Esperado:**
- Query filtra por TenantId del usuario
- Campaña de otro tenant no se encuentra en el contexto del tenant del usuario
- No se muestran memorias de otro tenant
- No hay fuga de información

---

## Contexto de Memoria para IA

### TC-MEM-013: Ver contexto de memoria para IA

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId y UserId válidos
- Tenant tiene memorias almacenadas

**Pasos:**
1. Navegar a `/Memory/AIContext`
2. Verificar que se muestra contexto de memoria

**Resultado Esperado:**
- Contexto de memoria se muestra correctamente
- Contexto incluye:
  - UserPreferences
  - CampaignHistory (si aplica)
  - RecentDecisions
  - LearningInsights
  - Otros componentes del contexto
- Información es relevante para el usuario y tenant

---

### TC-MEM-014: Ver contexto de memoria para IA con campaña específica

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe en el tenant
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Memory/AIContext?campaignId={campaignId}`
2. Verificar contexto

**Resultado Esperado:**
- Contexto se filtra por campaña si se proporciona CampaignId
- Contexto incluye memorias relevantes a la campaña
- Información es contextualizada a la campaña

---

### TC-MEM-015: Ver contexto sin TenantId o UserId

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId o UserId válido

**Pasos:**
1. Intentar acceder a `/Memory/AIContext` sin TenantId o UserId

**Resultado Esperado:**
- Warning se registra: "Usuario autenticado sin TenantId o UserId"
- Usuario es redirigido a `/Account/Login`
- No se muestra contexto

---

### TC-MEM-016: Manejo de errores al cargar contexto

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Sistema tiene error al cargar contexto

**Pasos:**
1. Intentar acceder a contexto cuando hay error

**Resultado Esperado:**
- Error se captura
- Vista se muestra con `new MemoryContextForAI()` (vacío)
- Error se registra en logs
- No se crashea la aplicación

---

## Tipos de Memoria

### TC-MEM-017: Memorias de tipo Conversation

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene memorias de tipo "Conversation"
- Usuario está autenticado

**Pasos:**
1. Filtrar por tipo "Conversation"
2. Verificar que se muestran correctamente

**Resultado Esperado:**
- Memorias de tipo Conversation se muestran
- MemoryType = "Conversation"
- Contenido y contexto se muestran correctamente

---

### TC-MEM-018: Memorias de tipo Decision

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene memorias de tipo "Decision"
- Usuario está autenticado

**Pasos:**
1. Filtrar por tipo "Decision"
2. Verificar que se muestran correctamente

**Resultado Esperado:**
- Memorias de tipo Decision se muestran
- MemoryType = "Decision"
- Decisiones tomadas se muestran con contexto

---

### TC-MEM-019: Memorias de tipo Learning

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene memorias de tipo "Learning"
- Usuario está autenticado

**Pasos:**
1. Filtrar por tipo "Learning"
2. Verificar que se muestran correctamente

**Resultado Esperado:**
- Memorias de tipo Learning se muestran
- MemoryType = "Learning"
- Aprendizajes se muestran con información relevante

---

### TC-MEM-020: Memorias de tipo Feedback

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene memorias de tipo "Feedback"
- Usuario está autenticado

**Pasos:**
1. Filtrar por tipo "Feedback"
2. Verificar que se muestran correctamente

**Resultado Esperado:**
- Memorias de tipo Feedback se muestran
- MemoryType = "Feedback"
- Feedback se muestra con contexto

---

## Multi-Tenant

### TC-MEM-021: Memorias se filtran por TenantId

**Módulo:** Memoria de Marketing  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene 10 memorias
- Tenant2 tiene 15 memorias
- Ambos tenants existen

**Pasos:**
1. Usuario de Tenant1 accede a `/Memory/Index`
2. Verificar memorias mostradas
3. Usuario de Tenant2 accede a `/Memory/Index`
4. Verificar memorias mostradas

**Resultado Esperado:**
- Usuario de Tenant1 ve solo sus 10 memorias
- Usuario de Tenant2 ve solo sus 15 memorias
- No hay mezcla de datos entre tenants
- Query filtra correctamente por TenantId

---

### TC-MEM-022: Memorias asignadas al tenant correcto

**Módulo:** Memoria de Marketing  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Memorias se crean automáticamente por el sistema
- Usuario pertenece a Tenant1

**Pasos:**
1. Verificar que memorias creadas tienen TenantId correcto

**Resultado Esperado:**
- Memorias se crean con TenantId del contexto
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene

---

## Ordenamiento y Límites

### TC-MEM-023: Memorias ordenadas por relevancia y fecha

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene memorias con diferentes RelevanceScore y fechas
- Usuario está autenticado

**Pasos:**
1. Acceder a `/Memory/Index`
2. Verificar ordenamiento

**Resultado Esperado:**
- Memorias se ordenan por:
  1. RelevanceScore descendente (más relevantes primero)
  2. MemoryDate descendente (más recientes primero)
- Ordenamiento es correcto

---

### TC-MEM-024: Límite de 50 memorias en lista principal

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant tiene más de 50 memorias
- Usuario está autenticado

**Pasos:**
1. Acceder a `/Memory/Index`
2. Verificar cantidad de memorias mostradas

**Resultado Esperado:**
- Se muestran máximo 50 memorias
- Limit = 50 se aplica correctamente
- Las más relevantes y recientes se muestran primero

---

### TC-MEM-025: Límite de 100 memorias en vista de campaña

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Campaña tiene más de 100 memorias asociadas
- Usuario está autenticado

**Pasos:**
1. Acceder a `/Memory/Campaign?campaignId={id}`
2. Verificar cantidad de memorias mostradas

**Resultado Esperado:**
- Se muestran máximo 100 memorias
- Limit = 100 se aplica correctamente
- Las más relevantes y recientes se muestran primero

---

## Deserialización de Contexto JSON

### TC-MEM-026: Contexto JSON se deserializa correctamente

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Memoria tiene ContextJson con datos válidos
- Usuario está autenticado

**Pasos:**
1. Ver memoria con ContextJson
2. Verificar que contexto se muestra correctamente

**Resultado Esperado:**
- ContextJson se deserializa a Dictionary<string, object>
- Contexto se muestra en formato legible
- No se generan errores de deserialización
- Si ContextJson es null, Context es null

---

### TC-MEM-027: Tags se convierten a lista correctamente

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Memoria tiene Tags en formato string separado por comas
- Usuario está autenticado

**Pasos:**
1. Ver memoria con tags
2. Verificar que tags se muestran como lista

**Resultado Esperado:**
- Tags string se convierte a List<string>
- Tags se separan por coma
- Si Tags es null, se muestra lista vacía
- Tags se muestran correctamente en UI

---

## Manejo de Errores

### TC-MEM-028: Manejo de errores al cargar memorias

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error (BD, servicio, etc.)

**Pasos:**
1. Intentar acceder a `/Memory/Index` cuando hay error

**Resultado Esperado:**
- Error se captura
- Vista se muestra con `new List<MarketingMemoryDto>()` (vacío)
- Error se registra en logs: "Error al cargar memorias"
- No se crashea la aplicación

---

### TC-MEM-029: Manejo de errores al cargar memorias de campaña

**Módulo:** Memoria de Marketing  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Sistema tiene error

**Pasos:**
1. Intentar acceder a `/Memory/Campaign?campaignId={id}` cuando hay error

**Resultado Esperado:**
- Error se captura
- Vista se muestra con lista vacía
- Error se registra en logs: "Error al cargar memorias de campaña {CampaignId}"
- No se crashea

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-MEM-001 | Listar todas las memorias | Funcional | Crítica | - |
| TC-MEM-002 | Listar sin memorias | Funcional | Media | - |
| TC-MEM-003 | Acceso sin autenticación | Seguridad | Crítica | - |
| TC-MEM-004 | Acceso sin TenantId | Multi-tenant | Alta | - |
| TC-MEM-005 | Filtrar por tipo | Funcional | Alta | - |
| TC-MEM-006 | Filtrar por tags | Funcional | Alta | - |
| TC-MEM-007 | Filtrar por tipo y tags | Funcional | Media | - |
| TC-MEM-008 | Filtrar con tipo inexistente | Funcional | Media | - |
| TC-MEM-009 | Ver memorias de campaña | Funcional | Alta | - |
| TC-MEM-010 | Ver memorias de campaña sin memorias | Funcional | Media | - |
| TC-MEM-011 | Ver memorias de campaña inexistente | Funcional | Media | - |
| TC-MEM-012 | Ver memorias de campaña otro tenant | Multi-tenant | Crítica | - |
| TC-MEM-013 | Ver contexto para IA | Funcional | Alta | - |
| TC-MEM-014 | Ver contexto con campaña | Funcional | Alta | - |
| TC-MEM-015 | Ver contexto sin TenantId/UserId | Funcional | Alta | - |
| TC-MEM-016 | Manejo de errores en contexto | Funcional | Media | - |
| TC-MEM-017 | Memorias tipo Conversation | Funcional | Media | - |
| TC-MEM-018 | Memorias tipo Decision | Funcional | Media | - |
| TC-MEM-019 | Memorias tipo Learning | Funcional | Media | - |
| TC-MEM-020 | Memorias tipo Feedback | Funcional | Media | - |
| TC-MEM-021 | Memorias filtran por TenantId | Multi-tenant | Crítica | - |
| TC-MEM-022 | Memorias asignadas al tenant correcto | Multi-tenant | Crítica | - |
| TC-MEM-023 | Ordenamiento por relevancia y fecha | Funcional | Alta | - |
| TC-MEM-024 | Límite 50 en lista principal | Funcional | Media | - |
| TC-MEM-025 | Límite 100 en vista campaña | Funcional | Media | - |
| TC-MEM-026 | Deserialización ContextJson | Funcional | Alta | - |
| TC-MEM-027 | Tags convertidos a lista | Funcional | Media | - |
| TC-MEM-028 | Manejo de errores al cargar | Funcional | Alta | - |
| TC-MEM-029 | Manejo de errores campaña | Funcional | Media | - |

**Total de casos:** 29  
**Críticos:** 5  
**Altos:** 11  
**Medios:** 13

