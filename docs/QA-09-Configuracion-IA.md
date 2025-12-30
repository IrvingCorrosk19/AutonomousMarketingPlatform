# Casos de Prueba - Configuración de IA

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Configuración de IA

---

## Índice

- [Ver Configuración de IA](#ver-configuración-de-ia)
- [Guardar Configuración de IA](#guardar-configuración-de-ia)
- [Encriptación de API Key](#encriptación-de-api-key)
- [Roles y Permisos](#roles-y-permisos)
- [Multi-Tenant](#multi-tenant)
- [Validaciones](#validaciones)
- [Proveedores y Modelos](#proveedores-y-modelos)

---

## Ver Configuración de IA

### TC-AI-001: Ver configuración de IA existente

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Owner o Admin
- Tenant tiene configuración de IA guardada
- Usuario tiene TenantId válido

**Pasos:**
1. Navegar a `/AIConfig/Index`
2. Verificar que se muestra configuración actual

**Resultado Esperado:**
- Configuración se carga correctamente
- Se muestran:
  - Provider (ej: "OpenAI")
  - Model (ej: "gpt-4")
  - IsActive (estado activo/inactivo)
  - LastUsedAt (última vez usado)
  - UsageCount (número de usos)
- API Key NO se muestra (seguridad)
- Formulario se pre-llena con valores actuales

---

### TC-AI-002: Ver configuración sin configuración existente

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant no tiene configuración de IA guardada
- Usuario tiene permisos

**Pasos:**
1. Navegar a `/AIConfig/Index`
2. Verificar vista

**Resultado Esperado:**
- Vista se carga con `new TenantAIConfigDto()` (vacío)
- Formulario se muestra vacío listo para configurar
- No se generan errores
- Valores por defecto se muestran si aplica

---

### TC-AI-003: Ver configuración sin permisos

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado sin rol Owner o Admin

**Pasos:**
1. Intentar acceder a `/AIConfig/Index` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Usuario es redirigido a `/Account/AccessDenied`
- No se puede ver configuración

---

### TC-AI-004: Ver configuración sin TenantId

**Módulo:** Configuración de IA  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId válido

**Pasos:**
1. Intentar acceder a `/AIConfig/Index` sin TenantId

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra configuración

---

### TC-AI-005: Ver configuración - Manejo de errores

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error al cargar configuración

**Pasos:**
1. Intentar acceder a configuración cuando hay error

**Resultado Esperado:**
- Error se captura
- Vista se muestra con `new TenantAIConfigDto()` (vacío)
- Error se registra en logs: "Error al cargar configuración de IA"
- No se crashea la aplicación

---

## Guardar Configuración de IA

### TC-AI-006: Guardar configuración de IA exitosamente

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Owner o Admin
- Usuario tiene UserId y TenantId válidos

**Pasos:**
1. Navegar a `/AIConfig/Index`
2. Completar formulario:
   - Provider: "OpenAI"
   - ApiKey: "sk-..." (API key válida)
   - Model: "gpt-4"
   - IsActive: true
3. Hacer clic en "Guardar"

**Resultado Esperado:**
- Configuración se guarda exitosamente
- API Key se encripta antes de guardar
- EncryptedApiKey se almacena en base de datos
- Provider, Model, IsActive se guardan correctamente
- TenantId se asigna automáticamente del usuario
- Usuario es redirigido a `/AIConfig/Index`
- TempData muestra: "API key de IA configurada correctamente."

---

### TC-AI-007: Actualizar configuración existente

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Tenant ya tiene configuración de IA guardada
- Usuario tiene permisos

**Pasos:**
1. Acceder a configuración existente
2. Modificar valores (ej: cambiar Model o ApiKey)
3. Guardar cambios

**Resultado Esperado:**
- Configuración existente se actualiza
- Nuevos valores se guardan correctamente
- API Key se re-encripta si se cambió
- Configuración anterior se sobrescribe

---

### TC-AI-008: Guardar configuración con IsActive = false

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Guardar configuración con IsActive = false
2. Verificar que se guarda correctamente

**Resultado Esperado:**
- IsActive se guarda como false
- Configuración se desactiva
- Sistema no usa esta configuración para generar contenido

---

### TC-AI-009: Guardar configuración sin UserId o TenantId

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin UserId o TenantId válido

**Pasos:**
1. Intentar guardar configuración sin UserId o TenantId

**Resultado Esperado:**
- Unauthorized se retorna
- Error: "Usuario no autenticado correctamente"
- Configuración NO se guarda

---

### TC-AI-010: Guardar configuración - Manejo de errores

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error (BD, encriptación, etc.)

**Pasos:**
1. Intentar guardar configuración cuando hay error

**Resultado Esperado:**
- Error se captura
- TempData muestra: "Error al guardar la configuración. Por favor, intente nuevamente."
- Error se registra en logs: "Error al guardar configuración de IA"
- Usuario es redirigido a Index

---

### TC-AI-011: Guardar configuración - UnauthorizedAccessException

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario intenta guardar configuración pero no tiene permisos

**Pasos:**
1. Intentar guardar configuración sin permisos suficientes

**Resultado Esperado:**
- UnauthorizedAccessException se lanza
- Warning se registra: "Intento no autorizado de configurar IA"
- TempData muestra mensaje de error
- Configuración NO se guarda

---

## Encriptación de API Key

### TC-AI-012: API Key se encripta antes de guardar

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene permisos
- API Key válida disponible

**Pasos:**
1. Guardar configuración con API Key
2. Verificar en base de datos que EncryptedApiKey está encriptado

**Resultado Esperado:**
- API Key se encripta usando IEncryptionService
- EncryptedApiKey se guarda en lugar de ApiKey en texto plano
- API Key original NO se almacena en texto plano
- Encriptación es reversible (para uso posterior)

---

### TC-AI-013: API Key NO se muestra en DTO de respuesta

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant tiene configuración con API Key guardada
- Usuario tiene permisos

**Pasos:**
1. Ver configuración de IA
2. Verificar que API Key no se muestra en respuesta

**Resultado Esperado:**
- TenantAIConfigDto NO incluye campo ApiKey
- API Key nunca se expone en respuestas
- Solo se muestran Provider, Model, IsActive, LastUsedAt, UsageCount
- Seguridad se mantiene

---

### TC-AI-014: API Key se desencripta para uso interno

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Configuración tiene API Key encriptada guardada
- Sistema necesita usar API Key para llamadas a IA

**Pasos:**
1. Sistema intenta usar configuración de IA
2. Verificar que API Key se desencripta correctamente

**Resultado Esperado:**
- API Key se desencripta usando IEncryptionService
- Desencriptación es exitosa
- API Key se usa para llamadas a proveedor de IA
- No se expone en logs o respuestas

---

## Roles y Permisos

### TC-AI-015: Usuario Owner puede configurar IA

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Owner"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a `/AIConfig/Index` (debe permitir)
2. Verificar acceso a guardar configuración (debe permitir)

**Resultado Esperado:**
- Owner puede ver configuración
- Owner puede guardar/actualizar configuración
- Permisos se aplican correctamente según `[AuthorizeRole("Owner", "Admin")]`

---

### TC-AI-016: Usuario Admin puede configurar IA

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Admin"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a configuración (debe permitir)
2. Verificar acceso a guardar (debe permitir)

**Resultado Esperado:**
- Admin puede ver configuración
- Admin puede guardar/actualizar configuración
- Permisos funcionan correctamente

---

### TC-AI-017: Usuario Marketer NO puede configurar IA

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Marketer" (no Owner ni Admin)

**Pasos:**
1. Intentar acceder a `/AIConfig/Index` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Redirección a `/Account/AccessDenied`
- Marketer no puede ver ni modificar configuración de IA

---

## Multi-Tenant

### TC-AI-018: Configuración se asigna al tenant correcto

**Módulo:** Configuración de IA  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos

**Pasos:**
1. Usuario de Tenant1 guarda configuración de IA
2. Verificar en base de datos que TenantId es Tenant1

**Resultado Esperado:**
- TenantAIConfig se crea con `TenantId = Tenant1` (del usuario)
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene

---

### TC-AI-019: Cada tenant tiene su propia configuración

**Módulo:** Configuración de IA  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene configuración con Provider = "OpenAI", Model = "gpt-4"
- Tenant2 tiene configuración con Provider = "OpenAI", Model = "gpt-3.5-turbo"
- Ambos tenants existen

**Pasos:**
1. Usuario de Tenant1 accede a configuración
2. Verificar configuración mostrada
3. Usuario de Tenant2 accede a configuración
4. Verificar configuración mostrada

**Resultado Esperado:**
- Usuario de Tenant1 ve su configuración (gpt-4)
- Usuario de Tenant2 ve su configuración (gpt-3.5-turbo)
- No hay mezcla de configuraciones entre tenants
- Query filtra correctamente por TenantId

---

### TC-AI-020: Configuración se filtra por Provider y TenantId

**Módulo:** Configuración de IA  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Tenant tiene múltiples configuraciones (diferentes providers)
- Usuario tiene permisos

**Pasos:**
1. Acceder a configuración con Provider = "OpenAI"
2. Verificar que solo se muestra configuración de OpenAI

**Resultado Esperado:**
- Query filtra por TenantId y Provider
- Solo configuración del provider especificado se muestra
- Filtrado funciona correctamente

---

## Validaciones

### TC-AI-021: Validación CSRF en guardar configuración

**Módulo:** Configuración de IA  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Formulario de configuración está cargado

**Pasos:**
1. Intentar guardar configuración sin token anti-forgery válido

**Resultado Esperado:**
- Request es rechazado
- `[ValidateAntiForgeryToken]` valida token
- Configuración NO se guarda

---

### TC-AI-022: Validación de campos requeridos

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Intentar guardar configuración sin completar campos requeridos (ApiKey, Provider)

**Resultado Esperado:**
- Validación falla
- Mensajes de error se muestran
- Configuración NO se guarda
- Usuario puede corregir y reintentar

---

## Proveedores y Modelos

### TC-AI-023: Configurar con Provider OpenAI

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- API Key de OpenAI válida disponible

**Pasos:**
1. Guardar configuración con Provider = "OpenAI"
2. Verificar que se guarda correctamente

**Resultado Esperado:**
- Provider = "OpenAI" se guarda
- Configuración se asocia a proveedor OpenAI
- Sistema puede usar OpenAI para generar contenido

---

### TC-AI-024: Configurar con Model gpt-4

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Guardar configuración con Model = "gpt-4"
2. Verificar que se guarda

**Resultado Esperado:**
- Model = "gpt-4" se guarda
- Modelo se usa para generación de contenido
- Valor por defecto es "gpt-4" si no se especifica

---

### TC-AI-025: Configurar con Model gpt-3.5-turbo

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Guardar configuración con Model = "gpt-3.5-turbo"
2. Verificar que se guarda

**Resultado Esperado:**
- Model = "gpt-3.5-turbo" se guarda
- Modelo se usa para generación
- Sistema acepta diferentes modelos

---

### TC-AI-026: Provider Anthropic está deshabilitado

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- UI muestra Anthropic como deshabilitado

**Pasos:**
1. Verificar que opción Anthropic está deshabilitada en formulario

**Resultado Esperado:**
- Opción "Anthropic" se muestra como disabled
- Mensaje: "Anthropic (Próximamente)"
- No se puede seleccionar Anthropic
- Solo OpenAI está disponible

---

## Uso de Configuración

### TC-AI-027: LastUsedAt se actualiza al usar configuración

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Configuración existe y está activa
- Sistema usa configuración para generar contenido

**Pasos:**
1. Sistema usa configuración de IA
2. Verificar que LastUsedAt se actualiza

**Resultado Esperado:**
- LastUsedAt se actualiza a fecha/hora actual cuando se usa
- Trazabilidad de uso se mantiene

---

### TC-AI-028: UsageCount se incrementa al usar configuración

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Configuración existe con UsageCount = 0
- Sistema usa configuración

**Pasos:**
1. Sistema usa configuración de IA
2. Verificar que UsageCount se incrementa

**Resultado Esperado:**
- UsageCount se incrementa en 1 cada vez que se usa
- Contador se mantiene actualizado
- Permite tracking de uso

---

### TC-AI-029: Configuración inactiva no se usa

**Módulo:** Configuración de IA  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Configuración existe con IsActive = false

**Pasos:**
1. Sistema intenta usar configuración inactiva
2. Verificar comportamiento

**Resultado Esperado:**
- Configuración inactiva NO se usa para generar contenido
- Sistema busca otra configuración activa o usa fallback
- IsActive controla si configuración está disponible

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-AI-001 | Ver configuración existente | Funcional | Crítica | - |
| TC-AI-002 | Ver sin configuración | Funcional | Media | - |
| TC-AI-003 | Ver sin permisos | Seguridad | Crítica | - |
| TC-AI-004 | Ver sin TenantId | Multi-tenant | Alta | - |
| TC-AI-005 | Manejo de errores al ver | Funcional | Alta | - |
| TC-AI-006 | Guardar configuración exitosamente | Funcional | Crítica | - |
| TC-AI-007 | Actualizar configuración existente | Funcional | Alta | - |
| TC-AI-008 | Guardar con IsActive = false | Funcional | Media | - |
| TC-AI-009 | Guardar sin UserId o TenantId | Funcional | Alta | - |
| TC-AI-010 | Guardar - Manejo de errores | Funcional | Alta | - |
| TC-AI-011 | Guardar - UnauthorizedAccessException | Seguridad | Crítica | - |
| TC-AI-012 | API Key se encripta | Seguridad | Crítica | - |
| TC-AI-013 | API Key NO se muestra en DTO | Seguridad | Crítica | - |
| TC-AI-014 | API Key se desencripta para uso | Funcional | Alta | - |
| TC-AI-015 | Permisos Owner | Seguridad | Crítica | - |
| TC-AI-016 | Permisos Admin | Seguridad | Crítica | - |
| TC-AI-017 | Permisos Marketer | Seguridad | Crítica | - |
| TC-AI-018 | Configuración asignada al tenant correcto | Multi-tenant | Crítica | - |
| TC-AI-019 | Cada tenant tiene su configuración | Multi-tenant | Crítica | - |
| TC-AI-020 | Filtrado por Provider y TenantId | Multi-tenant | Alta | - |
| TC-AI-021 | Validación CSRF | Seguridad | Crítica | - |
| TC-AI-022 | Validación campos requeridos | Funcional | Alta | - |
| TC-AI-023 | Configurar con OpenAI | Funcional | Alta | - |
| TC-AI-024 | Configurar con gpt-4 | Funcional | Media | - |
| TC-AI-025 | Configurar con gpt-3.5-turbo | Funcional | Media | - |
| TC-AI-026 | Provider Anthropic deshabilitado | Funcional | Media | - |
| TC-AI-027 | LastUsedAt se actualiza | Funcional | Media | - |
| TC-AI-028 | UsageCount se incrementa | Funcional | Media | - |
| TC-AI-029 | Configuración inactiva no se usa | Funcional | Alta | - |

**Total de casos:** 29  
**Críticos:** 12  
**Altos:** 10  
**Medios:** 7

