# Casos de Prueba - Autenticación y Autorización

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Autenticación y Autorización

---

## Índice

- [Login Exitoso](#login-exitoso)
- [Validaciones de Login](#validaciones-de-login)
- [Bloqueo de Cuenta](#bloqueo-de-cuenta)
- [Logout](#logout)
- [Acceso Denegado](#acceso-denegado)
- [Multi-Tenant en Autenticación](#multi-tenant-en-autenticación)
- [Super Admin](#super-admin)
- [Remember Me](#remember-me)
- [Seguridad](#seguridad)

---

## Login Exitoso

### TC-AUTH-001: Login exitoso con credenciales válidas (Usuario normal)

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario existe en el sistema con email válido
- Usuario tiene TenantId asignado
- Usuario está activo (IsActive = true)
- Usuario no está bloqueado
- Contraseña cumple con políticas de seguridad

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email válido en campo "Email"
3. Ingresar contraseña correcta en campo "Password"
4. Dejar checkbox "Remember Me" sin marcar
5. Hacer clic en botón "Iniciar Sesión"

**Resultado Esperado:**
- Login se procesa exitosamente
- Usuario es redirigido a `/Home/Index` (Dashboard)
- Sesión se crea correctamente
- Claims se agregan al usuario: `FullName`, `TenantId`
- Campo `LastLoginAt` se actualiza en base de datos
- Campo `LastLoginIp` se registra
- Campo `FailedLoginAttempts` se resetea a 0
- Se registra evento de auditoría con estado "Success"
- Cookie de autenticación se establece con nombre "AutonomousMarketingPlatform.Auth"

---

### TC-AUTH-002: Login exitoso con Remember Me activado

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario existe y está activo
- Usuario tiene credenciales válidas

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email válido
3. Ingresar contraseña correcta
4. Marcar checkbox "Remember Me"
5. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Login exitoso
- Cookie de autenticación se establece con expiración extendida (24 horas)
- Cookie tiene `SlidingExpiration = true`
- Usuario permanece autenticado después de cerrar y reabrir navegador (dentro del período de expiración)

---

### TC-AUTH-003: Login exitoso con returnUrl válido

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario existe y está activo
- URL de retorno es local (no externa)

**Pasos:**
1. Navegar a `/Account/Login?returnUrl=/Campaigns/Index`
2. Ingresar credenciales válidas
3. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Login exitoso
- Usuario es redirigido a `/Campaigns/Index` (returnUrl)
- No se redirige a Dashboard por defecto

---

## Validaciones de Login

### TC-AUTH-004: Login fallido - Email vacío

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Navegador en página de login

**Pasos:**
1. Navegar a `/Account/Login`
2. Dejar campo "Email" vacío
3. Ingresar cualquier contraseña
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Formulario no se envía o se muestra error de validación
- Mensaje de error: "El correo electrónico es requerido."
- Usuario permanece en página de login
- No se crea sesión

---

### TC-AUTH-005: Login fallido - Contraseña vacía

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Navegador en página de login

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email válido
3. Dejar campo "Password" vacío
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Mensaje de error: "La contraseña es requerida."
- Usuario permanece en página de login
- No se crea sesión

---

### TC-AUTH-006: Login fallido - Credenciales inválidas (email incorrecto)

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Email no existe en el sistema

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email que no existe: `noexiste@test.com`
3. Ingresar cualquier contraseña
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Mensaje de error: "Intento de inicio de sesión no válido." o "Credenciales inválidas"
- Se muestra contador de intentos restantes (si aplica)
- Usuario permanece en página de login
- No se crea sesión
- Se registra evento de auditoría con estado "Failed"

---

### TC-AUTH-007: Login fallido - Credenciales inválidas (contraseña incorrecta)

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario existe en el sistema
- Usuario no está bloqueado
- Usuario tiene menos de 5 intentos fallidos previos

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email válido de usuario existente
3. Ingresar contraseña incorrecta
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Mensaje de error: "Credenciales inválidas. Intentos restantes: X" (donde X = 5 - intentos actuales)
- Campo `FailedLoginAttempts` se incrementa en 1
- Usuario permanece en página de login
- Se muestra `ViewData["RemainingAttempts"]` con intentos restantes
- Se registra evento de auditoría con estado "Failed"

---

### TC-AUTH-008: Login fallido - Usuario inactivo

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario existe con `IsActive = false`
- Credenciales son correctas

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email de usuario inactivo
3. Ingresar contraseña correcta
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Mensaje de error: "Cuenta desactivada"
- Usuario permanece en página de login
- No se crea sesión
- Se registra evento de auditoría con estado "Failed" y mensaje "Cuenta desactivada"

---

### TC-AUTH-009: Login fallido - Email con formato inválido

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Navegador en página de login

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email con formato inválido: `emailinvalido`
3. Ingresar cualquier contraseña
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Validación del lado del cliente o servidor rechaza el formato
- Mensaje de error apropiado
- Usuario permanece en página de login

---

## Bloqueo de Cuenta

### TC-AUTH-010: Bloqueo de cuenta después de 5 intentos fallidos

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario existe y está activo
- Usuario tiene 0 intentos fallidos previos

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email válido
3. Ingresar contraseña incorrecta
4. Hacer clic en "Iniciar Sesión"
5. Repetir pasos 2-4 cuatro veces más (total 5 intentos fallidos)

**Resultado Esperado:**
- Después del 5to intento fallido:
  - Campo `LockoutEndDate` se establece a `DateTime.UtcNow.AddMinutes(15)`
  - Mensaje de error: "Cuenta bloqueada por 15 minutos debido a múltiples intentos fallidos"
  - Se muestra `ViewData["LockoutEnd"]` con fecha/hora de desbloqueo
  - Usuario no puede iniciar sesión incluso con credenciales correctas
  - Se registra evento de auditoría con estado "Failed" y mensaje "Cuenta bloqueada por intentos fallidos"

---

### TC-AUTH-011: Intento de login con cuenta bloqueada

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene `LockoutEndDate` establecido y es mayor a `DateTime.UtcNow`
- Usuario tiene credenciales válidas

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email de usuario bloqueado
3. Ingresar contraseña correcta
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Mensaje de error: "Cuenta bloqueada hasta [fecha/hora]" (formato: g)
- Usuario no puede iniciar sesión
- Se muestra `ViewData["LockoutEnd"]` con fecha de desbloqueo
- Se registra evento de auditoría con estado "Failed" y mensaje "Cuenta bloqueada"

---

### TC-AUTH-012: Login exitoso después de expiración de bloqueo

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tenía cuenta bloqueada
- `LockoutEndDate` ha expirado (es menor a `DateTime.UtcNow`)
- Usuario tiene credenciales válidas

**Pasos:**
1. Esperar a que expire el período de bloqueo (15 minutos)
2. Navegar a `/Account/Login`
3. Ingresar email válido
4. Ingresar contraseña correcta
5. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Login exitoso
- Usuario es redirigido al Dashboard
- Campo `FailedLoginAttempts` se resetea a 0
- Campo `LockoutEndDate` se mantiene pero ya no bloquea (validación por fecha)

---

## Logout

### TC-AUTH-013: Logout exitoso

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado y tiene sesión activa
- Usuario está en cualquier página del sistema

**Pasos:**
1. Hacer clic en botón/menú "Cerrar Sesión" o "Logout"
2. Confirmar acción si se requiere

**Resultado Esperado:**
- Sesión se cierra correctamente
- Cookie de autenticación se elimina o invalida
- Usuario es redirigido a `/Account/Login`
- No se puede acceder a páginas protegidas sin autenticarse nuevamente
- Se registra evento de logout en logs con UserId y TenantId

---

### TC-AUTH-014: Acceso a página protegida después de logout

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario acaba de hacer logout

**Pasos:**
1. Después de logout exitoso
2. Intentar acceder directamente a URL protegida: `/Campaigns/Index`
3. O usar botón "Atrás" del navegador

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se puede acceder al contenido protegido
- Mensaje de login se muestra si aplica

---

## Acceso Denegado

### TC-AUTH-015: Acceso a página sin permisos suficientes

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado con rol "Marketer"
- Usuario intenta acceder a endpoint que requiere rol "Admin" o "Owner"

**Pasos:**
1. Usuario con rol "Marketer" autenticado
2. Intentar acceder a `/AIConfig/Index` (requiere Owner o Admin)

**Resultado Esperado:**
- Usuario es redirigido a `/Account/AccessDenied`
- Se muestra página de "Acceso Denegado"
- No se muestra contenido restringido

---

## Multi-Tenant en Autenticación

### TC-AUTH-016: Login exitoso con TenantId desde header

**Módulo:** Autenticación  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario existe con TenantId válido
- Tenant existe y está activo
- Request incluye header `X-Tenant-Id` con GUID válido

**Pasos:**
1. Realizar request POST a `/Account/Login` con header `X-Tenant-Id: [GUID válido]`
2. Incluir credenciales válidas del usuario
3. Enviar formulario

**Resultado Esperado:**
- TenantId se resuelve desde header
- Login exitoso si usuario pertenece a ese tenant
- Claim `TenantId` se establece correctamente
- Usuario solo puede acceder a datos de su tenant

---

### TC-AUTH-017: Login fallido - Usuario no pertenece al tenant

**Módulo:** Autenticación  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario A existe con TenantId = Tenant1
- Request incluye header `X-Tenant-Id: Tenant2` (diferente)

**Pasos:**
1. Realizar request POST a `/Account/Login` con header `X-Tenant-Id: [Tenant2 GUID]`
2. Ingresar credenciales de Usuario A (que pertenece a Tenant1)
3. Enviar formulario

**Resultado Esperado:**
- Login falla
- Mensaje de error: "Credenciales inválidas" o "Usuario no pertenece a este tenant"
- No se crea sesión
- Se registra evento de auditoría con estado "Failed" y mensaje "Usuario no pertenece a este tenant"

---

### TC-AUTH-018: Login sin TenantId - Usuario normal (no super admin)

**Módulo:** Autenticación  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario existe con TenantId válido (no Guid.Empty)
- Usuario no es SuperAdmin
- Request no incluye header `X-Tenant-Id`
- No hay subdomain en la URL

**Pasos:**
1. Navegar a `/Account/Login` sin header de tenant
2. Ingresar credenciales de usuario normal
3. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Sistema intenta obtener TenantId del usuario desde base de datos
- Si usuario tiene TenantId válido, login exitoso
- Si no se puede determinar tenant, mensaje: "No se pudo determinar el tenant. Por favor, use el header X-Tenant-Id o acceda desde el subdominio correcto."

---

## Super Admin

### TC-AUTH-019: Login exitoso como Super Admin

**Módulo:** Autenticación  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario existe con `TenantId = Guid.Empty`
- Usuario tiene rol "SuperAdmin" o "Owner" (a nivel sistema)
- Credenciales son válidas

**Pasos:**
1. Navegar a `/Account/Login`
2. Ingresar email de super admin
3. Ingresar contraseña correcta
4. Hacer clic en "Iniciar Sesión"

**Resultado Esperado:**
- Login exitoso
- Claim `TenantId` se establece como `Guid.Empty`
- Claim `IsSuperAdmin` se establece como "true"
- Usuario es redirigido a `/Home/Index`
- Usuario puede acceder a funcionalidades de super admin
- Se registra en logs: "Login de super admin (sin tenant)"

---

### TC-AUTH-020: Super Admin accede sin TenantId en request

**Módulo:** Autenticación  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario es SuperAdmin
- Request no incluye TenantId

**Pasos:**
1. SuperAdmin autenticado intenta acceder a cualquier endpoint
2. Request no incluye header `X-Tenant-Id`

**Resultado Esperado:**
- Acceso permitido
- `TenantValidationMiddleware` permite el acceso porque `IsSuperAdmin = true`
- Se registra en logs: "SuperAdmin accediendo sin TenantId: Path=[path]"

---

## Remember Me

### TC-AUTH-021: Cookie de Remember Me con configuración correcta

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Media

**Precondiciones:**
- Usuario hace login con Remember Me activado

**Pasos:**
1. Realizar login con Remember Me marcado
2. Inspeccionar cookie de autenticación en DevTools

**Resultado Esperado:**
- Cookie tiene nombre: "AutonomousMarketingPlatform.Auth"
- Cookie tiene `HttpOnly = true`
- Cookie tiene `Secure = true` en producción, `SameAsRequest` en desarrollo
- Cookie tiene `SameSite = Strict`
- Cookie tiene `ExpireTimeSpan = 24 horas`
- Cookie tiene `SlidingExpiration = true`

---

## Seguridad

### TC-AUTH-022: Protección CSRF en formulario de login

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Formulario de login está cargado

**Pasos:**
1. Inspeccionar formulario de login en DevTools
2. Verificar presencia de token anti-forgery

**Resultado Esperado:**
- Formulario incluye campo hidden con `__RequestVerificationToken`
- Endpoint POST requiere `[ValidateAntiForgeryToken]`
- Request sin token válido es rechazado

---

### TC-AUTH-023: Protección contra fuerza bruta - Rate limiting implícito

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Sistema tiene lockout configurado (5 intentos, 15 minutos)

**Pasos:**
1. Realizar múltiples intentos de login fallidos con diferentes usuarios
2. Verificar comportamiento del sistema

**Resultado Esperado:**
- Cada usuario tiene su propio contador de intentos fallidos
- Bloqueo se aplica por usuario, no globalmente
- Usuarios diferentes no se afectan entre sí

---

### TC-AUTH-024: Validación de contraseña según políticas

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Políticas de contraseña configuradas:
  - RequireDigit = true
  - RequireLowercase = true
  - RequireUppercase = true
  - RequireNonAlphanumeric = true
  - RequiredLength = 8

**Pasos:**
1. Intentar crear usuario o cambiar contraseña con valores que no cumplan políticas
2. Verificar validación

**Resultado Esperado:**
- Contraseñas que no cumplen políticas son rechazadas
- Mensajes de error específicos se muestran
- Sistema no permite contraseñas débiles

---

### TC-AUTH-025: Auditoría de eventos de autenticación

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Sistema de auditoría está activo

**Pasos:**
1. Realizar login exitoso
2. Realizar login fallido
3. Realizar logout
4. Consultar logs de auditoría

**Resultado Esperado:**
- Cada evento de login (exitoso/fallido) se registra en auditoría con:
  - TenantId
  - UserId
  - IP Address
  - Timestamp
  - Estado (Success/Failed)
  - Mensaje descriptivo
- Eventos de logout se registran en logs del sistema

---

### TC-AUTH-026: Protección de contraseña en logs

**Módulo:** Autenticación  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Sistema tiene logging activo

**Pasos:**
1. Realizar login
2. Revisar logs del sistema

**Resultado Esperado:**
- Contraseñas nunca aparecen en logs en texto plano
- Logs muestran "***" o "EMPTY" para campos de contraseña
- Solo se registra email y otros metadatos no sensibles

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-AUTH-001 | Login exitoso con credenciales válidas | Funcional | Crítica | - |
| TC-AUTH-002 | Login exitoso con Remember Me | Funcional | Alta | - |
| TC-AUTH-003 | Login exitoso con returnUrl válido | Funcional | Media | - |
| TC-AUTH-004 | Login fallido - Email vacío | Funcional | Alta | - |
| TC-AUTH-005 | Login fallido - Contraseña vacía | Funcional | Alta | - |
| TC-AUTH-006 | Login fallido - Email incorrecto | Funcional | Crítica | - |
| TC-AUTH-007 | Login fallido - Contraseña incorrecta | Funcional | Crítica | - |
| TC-AUTH-008 | Login fallido - Usuario inactivo | Funcional | Alta | - |
| TC-AUTH-009 | Login fallido - Email formato inválido | Funcional | Media | - |
| TC-AUTH-010 | Bloqueo después de 5 intentos | Seguridad | Crítica | - |
| TC-AUTH-011 | Intento de login con cuenta bloqueada | Seguridad | Crítica | - |
| TC-AUTH-012 | Login después de expiración de bloqueo | Funcional | Alta | - |
| TC-AUTH-013 | Logout exitoso | Funcional | Crítica | - |
| TC-AUTH-014 | Acceso después de logout | Seguridad | Alta | - |
| TC-AUTH-015 | Acceso denegado por permisos | Seguridad | Alta | - |
| TC-AUTH-016 | Login con TenantId desde header | Multi-tenant | Crítica | - |
| TC-AUTH-017 | Login fallido - Usuario no pertenece al tenant | Multi-tenant | Crítica | - |
| TC-AUTH-018 | Login sin TenantId - Usuario normal | Multi-tenant | Alta | - |
| TC-AUTH-019 | Login exitoso como Super Admin | Funcional | Crítica | - |
| TC-AUTH-020 | Super Admin sin TenantId | Multi-tenant | Alta | - |
| TC-AUTH-021 | Cookie Remember Me correcta | Seguridad | Media | - |
| TC-AUTH-022 | Protección CSRF | Seguridad | Crítica | - |
| TC-AUTH-023 | Protección fuerza bruta | Seguridad | Crítica | - |
| TC-AUTH-024 | Validación políticas de contraseña | Seguridad | Alta | - |
| TC-AUTH-025 | Auditoría de eventos | Seguridad | Alta | - |
| TC-AUTH-026 | Protección contraseña en logs | Seguridad | Crítica | - |

**Total de casos:** 26  
**Críticos:** 11  
**Altos:** 10  
**Medios:** 5

