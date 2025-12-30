# Casos de Prueba - Consentimientos

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Gestión de Consentimientos

---

## Índice

- [Listar Consentimientos](#listar-consentimientos)
- [Otorgar Consentimiento](#otorgar-consentimiento)
- [Revocar Consentimiento](#revocar-consentimiento)
- [Validaciones](#validaciones)
- [Multi-Tenant](#multi-tenant)
- [Auditoría y Trazabilidad](#auditoría-y-trazabilidad)

---

## Listar Consentimientos

### TC-CONS-001: Listar consentimientos del usuario

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene UserId y TenantId válidos
- Usuario tiene consentimientos otorgados y/o revocados

**Pasos:**
1. Navegar a `/Consents/Index`
2. Verificar que se muestra lista de consentimientos

**Resultado Esperado:**
- Lista muestra todos los consentimientos del usuario
- Consentimientos se muestran con información relevante:
  - Tipo de consentimiento
  - Estado (otorgado/revocado)
  - Fecha de otorgamiento
  - Fecha de revocación (si aplica)
  - Versión del consentimiento
- Vista se carga sin errores

---

### TC-CONS-002: Listar consentimientos sin consentimientos

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario no tiene consentimientos registrados
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Consents/Index`
2. Verificar vista

**Resultado Esperado:**
- Lista se muestra vacía o con mensaje informativo
- No se generan errores
- Vista se carga correctamente

---

### TC-CONS-003: Acceso sin autenticación

**Módulo:** Consentimientos  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Consents/Index` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se muestra contenido

---

### TC-CONS-004: Acceso sin UserId o TenantId

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin UserId o TenantId válido

**Pasos:**
1. Intentar acceder a `/Consents/Index` sin UserId o TenantId

**Resultado Esperado:**
- Warning se registra: "Usuario autenticado sin UserId o TenantId"
- Usuario es redirigido a `/Account/Login`
- No se muestran consentimientos

---

## Otorgar Consentimiento

### TC-CONS-005: Otorgar consentimiento exitosamente

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene UserId y TenantId válidos
- Usuario no tiene consentimiento del tipo especificado otorgado

**Pasos:**
1. Navegar a `/Consents/Index`
2. Hacer clic en botón "Otorgar" para un tipo de consentimiento
3. O enviar POST a `/Consents/Grant` con:
   - ConsentType: "AIGeneration"
   - ConsentVersion: "1.0" (o dejar por defecto)
4. Confirmar otorgamiento

**Resultado Esperado:**
- Consentimiento se otorga exitosamente
- IsGranted se establece como true
- GrantedAt se establece a fecha/hora actual
- ConsentVersion se guarda (por defecto "1.0" si no se proporciona)
- IpAddress se registra desde HttpContext.Connection.RemoteIpAddress
- Usuario es redirigido a `/Consents/Index`
- TempData muestra: "Consentimiento 'AIGeneration' otorgado correctamente."

---

### TC-CONS-006: Otorgar consentimiento con versión específica

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Versión específica de consentimiento disponible

**Pasos:**
1. Otorgar consentimiento proporcionando ConsentVersion: "2.0"

**Resultado Esperado:**
- ConsentVersion se guarda como "2.0"
- Versión se registra correctamente
- Trazabilidad de versión se mantiene

---

### TC-CONS-007: Otorgar consentimiento sin UserId o TenantId

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin UserId o TenantId válido

**Pasos:**
1. Intentar otorgar consentimiento sin UserId o TenantId

**Resultado Esperado:**
- TempData muestra: "Error de autenticación. Por favor, inicie sesión nuevamente."
- Usuario es redirigido a `/Account/Login`
- Consentimiento NO se otorga

---

### TC-CONS-008: Otorgar consentimiento - Manejo de errores

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error (BD, servicio, etc.)

**Pasos:**
1. Intentar otorgar consentimiento cuando hay error de sistema

**Resultado Esperado:**
- Error se captura
- TempData muestra: "Error al otorgar el consentimiento. Por favor, intente nuevamente."
- Error se registra en logs: "Error al otorgar consentimiento {ConsentType} para usuario {UserId}"
- Usuario es redirigido a Index

---

## Revocar Consentimiento

### TC-CONS-009: Revocar consentimiento exitosamente

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene consentimiento otorgado
- Consentimiento NO es requerido (puede ser revocado)
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Consents/Index`
2. Hacer clic en botón "Revocar" para un consentimiento otorgado
3. O enviar POST a `/Consents/Revoke` con consentType
4. Confirmar revocación

**Resultado Esperado:**
- Consentimiento se revoca exitosamente
- IsGranted se establece como false
- RevokedAt se establece a fecha/hora actual
- GrantedAt se mantiene (historial)
- Usuario es redirigido a `/Consents/Index`
- TempData muestra: "Consentimiento '{consentType}' revocado correctamente."

---

### TC-CONS-010: Revocar consentimiento requerido (no permitido)

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene consentimiento otorgado
- Consentimiento es requerido y NO puede ser revocado
- Usuario está autenticado

**Pasos:**
1. Intentar revocar consentimiento requerido

**Resultado Esperado:**
- InvalidOperationException se lanza
- TempData muestra mensaje de error explicativo
- Consentimiento NO se revoca
- Warning se registra: "Intento de revocar consentimiento requerido {ConsentType}"
- Usuario es redirigido a Index

---

### TC-CONS-011: Revocar consentimiento sin UserId o TenantId

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin UserId o TenantId válido

**Pasos:**
1. Intentar revocar consentimiento sin UserId o TenantId

**Resultado Esperado:**
- TempData muestra: "Error de autenticación. Por favor, inicie sesión nuevamente."
- Usuario es redirigido a `/Account/Login`
- Consentimiento NO se revoca

---

### TC-CONS-012: Revocar consentimiento - Manejo de errores

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error

**Pasos:**
1. Intentar revocar consentimiento cuando hay error de sistema

**Resultado Esperado:**
- Error se captura
- TempData muestra: "Error al revocar el consentimiento. Por favor, intente nuevamente."
- Error se registra en logs: "Error al revocar consentimiento {ConsentType} para usuario {UserId}"
- Usuario es redirigido a Index

---

## Validaciones

### TC-CONS-013: Validación CSRF en otorgar consentimiento

**Módulo:** Consentimientos  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Formulario de otorgar consentimiento está cargado

**Pasos:**
1. Intentar otorgar consentimiento sin token anti-forgery válido

**Resultado Esperado:**
- Request es rechazado
- `[ValidateAntiForgeryToken]` valida token
- Consentimiento NO se otorga

---

### TC-CONS-014: Validación CSRF en revocar consentimiento

**Módulo:** Consentimientos  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Formulario de revocar consentimiento está cargado

**Pasos:**
1. Intentar revocar consentimiento sin token anti-forgery válido

**Resultado Esperado:**
- Request es rechazado
- `[ValidateAntiForgeryToken]` valida token
- Consentimiento NO se revoca

---

## Multi-Tenant

### TC-CONS-015: Consentimientos se filtran por UserId y TenantId

**Módulo:** Consentimientos  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario1 de Tenant1 tiene 3 consentimientos
- Usuario2 de Tenant1 tiene 2 consentimientos
- Usuario3 de Tenant2 tiene 4 consentimientos

**Pasos:**
1. Usuario1 de Tenant1 accede a `/Consents/Index`
2. Verificar consentimientos mostrados
3. Usuario2 de Tenant1 accede a `/Consents/Index`
4. Verificar consentimientos mostrados
5. Usuario3 de Tenant2 accede a `/Consents/Index`
6. Verificar consentimientos mostrados

**Resultado Esperado:**
- Usuario1 ve solo sus 3 consentimientos
- Usuario2 ve solo sus 2 consentimientos
- Usuario3 ve solo sus 4 consentimientos
- No hay mezcla de datos entre usuarios o tenants
- Query filtra correctamente por UserId y TenantId

---

### TC-CONS-016: Consentimiento otorgado se asigna al usuario y tenant correctos

**Módulo:** Consentimientos  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos

**Pasos:**
1. Usuario de Tenant1 otorga consentimiento
2. Verificar en base de datos que UserId y TenantId son correctos

**Resultado Esperado:**
- Consent se crea con UserId y TenantId del usuario
- No se puede asignar manualmente otros valores
- Integridad de datos multi-tenant se mantiene

---

### TC-CONS-017: Usuario no puede otorgar consentimiento para otro usuario

**Módulo:** Consentimientos  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario1 está autenticado
- Usuario1 intenta otorgar consentimiento para Usuario2

**Pasos:**
1. Usuario1 intenta otorgar consentimiento con UserId diferente al suyo

**Resultado Esperado:**
- Sistema usa UserId del usuario autenticado (desde claims)
- No se puede otorgar consentimiento para otro usuario
- Seguridad se mantiene

---

## Auditoría y Trazabilidad

### TC-CONS-018: IP Address se registra al otorgar consentimiento

**Módulo:** Consentimientos  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Request tiene IP address

**Pasos:**
1. Otorgar consentimiento
2. Verificar que IpAddress se registra

**Resultado Esperado:**
- IpAddress se obtiene de HttpContext.Connection.RemoteIpAddress
- IpAddress se guarda en Consent
- Trazabilidad se mantiene

---

### TC-CONS-019: Versión de consentimiento se registra

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Versión de consentimiento se proporciona

**Pasos:**
1. Otorgar consentimiento con versión específica
2. Verificar que versión se registra

**Resultado Esperado:**
- ConsentVersion se guarda correctamente
- Si no se proporciona, se usa "1.0" por defecto
- Versión permite rastrear qué versión del documento aceptó el usuario

---

### TC-CONS-020: Fechas de otorgamiento y revocación se registran

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Otorgar consentimiento
2. Verificar GrantedAt
3. Revocar consentimiento
4. Verificar RevokedAt

**Resultado Esperado:**
- GrantedAt se establece cuando se otorga
- RevokedAt se establece cuando se revoca
- Ambas fechas se mantienen para auditoría
- Historial completo se preserva

---

### TC-CONS-021: Historial de consentimiento se mantiene

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario otorga y luego revoca consentimiento

**Pasos:**
1. Otorgar consentimiento
2. Revocar consentimiento
3. Verificar que ambos eventos se registran

**Resultado Esperado:**
- Historial completo se mantiene:
  - IsGranted = false (después de revocar)
  - GrantedAt se mantiene (no se elimina)
  - RevokedAt se establece
- No se pierde información histórica

---

## Estados de Consentimiento

### TC-CONS-022: Consentimiento otorgado muestra estado correcto

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene consentimiento otorgado

**Pasos:**
1. Ver lista de consentimientos
2. Verificar estado de consentimiento otorgado

**Resultado Esperado:**
- IsGranted = true se muestra correctamente
- Estado se indica claramente en UI
- Fecha de otorgamiento se muestra

---

### TC-CONS-023: Consentimiento revocado muestra estado correcto

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene consentimiento revocado

**Pasos:**
1. Ver lista de consentimientos
2. Verificar estado de consentimiento revocado

**Resultado Esperado:**
- IsGranted = false se muestra correctamente
- Estado se indica claramente en UI
- Fecha de revocación se muestra
- Fecha de otorgamiento original se mantiene visible

---

## Tipos de Consentimiento

### TC-CONS-024: Otorgar consentimiento AIGeneration

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Otorgar consentimiento de tipo "AIGeneration"

**Resultado Esperado:**
- Consentimiento se otorga exitosamente
- ConsentType = "AIGeneration"
- Sistema puede usar IA para generar contenido

---

### TC-CONS-025: Otorgar consentimiento DataProcessing

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Otorgar consentimiento de tipo "DataProcessing"

**Resultado Esperado:**
- Consentimiento se otorga exitosamente
- ConsentType = "DataProcessing"
- Sistema puede procesar datos del usuario

---

### TC-CONS-026: Otorgar consentimiento AutoPublishing

**Módulo:** Consentimientos  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Otorgar consentimiento de tipo "AutoPublishing"

**Resultado Esperado:**
- Consentimiento se otorga exitosamente
- ConsentType = "AutoPublishing"
- Sistema puede publicar automáticamente

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-CONS-001 | Listar consentimientos del usuario | Funcional | Crítica | - |
| TC-CONS-002 | Listar sin consentimientos | Funcional | Media | - |
| TC-CONS-003 | Acceso sin autenticación | Seguridad | Crítica | - |
| TC-CONS-004 | Acceso sin UserId o TenantId | Funcional | Alta | - |
| TC-CONS-005 | Otorgar consentimiento exitosamente | Funcional | Crítica | - |
| TC-CONS-006 | Otorgar con versión específica | Funcional | Media | - |
| TC-CONS-007 | Otorgar sin UserId o TenantId | Funcional | Alta | - |
| TC-CONS-008 | Otorgar - Manejo de errores | Funcional | Alta | - |
| TC-CONS-009 | Revocar consentimiento exitosamente | Funcional | Crítica | - |
| TC-CONS-010 | Revocar consentimiento requerido | Funcional | Crítica | - |
| TC-CONS-011 | Revocar sin UserId o TenantId | Funcional | Alta | - |
| TC-CONS-012 | Revocar - Manejo de errores | Funcional | Alta | - |
| TC-CONS-013 | Validación CSRF otorgar | Seguridad | Crítica | - |
| TC-CONS-014 | Validación CSRF revocar | Seguridad | Crítica | - |
| TC-CONS-015 | Consentimientos filtran por UserId y TenantId | Multi-tenant | Crítica | - |
| TC-CONS-016 | Consentimiento asignado correctamente | Multi-tenant | Crítica | - |
| TC-CONS-017 | Usuario no puede otorgar para otro | Seguridad | Crítica | - |
| TC-CONS-018 | IP Address se registra | Seguridad | Alta | - |
| TC-CONS-019 | Versión se registra | Funcional | Alta | - |
| TC-CONS-020 | Fechas se registran | Funcional | Alta | - |
| TC-CONS-021 | Historial se mantiene | Funcional | Media | - |
| TC-CONS-022 | Estado otorgado correcto | Funcional | Media | - |
| TC-CONS-023 | Estado revocado correcto | Funcional | Media | - |
| TC-CONS-024 | Otorgar AIGeneration | Funcional | Alta | - |
| TC-CONS-025 | Otorgar DataProcessing | Funcional | Media | - |
| TC-CONS-026 | Otorgar AutoPublishing | Funcional | Media | - |

**Total de casos:** 26  
**Críticos:** 9  
**Altos:** 10  
**Medios:** 7

