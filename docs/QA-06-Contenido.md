# Casos de Prueba - Contenido (Carga de Archivos)

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Gestión de Contenido

---

## Índice

- [Cargar Archivos](#cargar-archivos)
- [Validaciones de Archivos](#validaciones-de-archivos)
- [Listar Contenido](#listar-contenido)
- [Asociación con Campañas](#asociación-con-campañas)
- [Tipos de Archivo](#tipos-de-archivo)
- [Límites de Tamaño](#límites-de-tamaño)
- [Roles y Permisos](#roles-y-permisos)
- [Multi-Tenant](#multi-tenant)
- [Manejo de Errores](#manejo-de-errores)

---

## Cargar Archivos

### TC-CONT-001: Cargar archivo único exitosamente (imagen)

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con rol Marketer, Admin o Owner
- Usuario tiene TenantId válido
- Archivo de imagen válido disponible (JPEG, PNG, GIF o WEBP)

**Pasos:**
1. Navegar a `/Content/Upload`
2. Seleccionar archivo de imagen válido (ej: imagen.jpg)
3. Opcionalmente asociar a campaña
4. Opcionalmente agregar descripción y tags
5. Hacer clic en "Cargar" o "Upload"

**Resultado Esperado:**
- Archivo se carga exitosamente
- Archivo se guarda en almacenamiento (FileStorageService)
- Registro de Content se crea en base de datos con:
  - TenantId del usuario
  - ContentType = "Image"
  - OriginalFileName preservado
  - FilePath generado
  - FileSize registrado
  - IsAiGenerated = false
- Respuesta JSON con información del archivo cargado
- Vista previa se muestra si aplica

---

### TC-CONT-002: Cargar múltiples archivos simultáneamente

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Múltiples archivos válidos disponibles (imágenes y/o videos)

**Pasos:**
1. Navegar a `/Content/Upload`
2. Seleccionar múltiples archivos (ej: 3 imágenes y 1 video)
3. Hacer clic en "Cargar"

**Resultado Esperado:**
- Todos los archivos se cargan exitosamente
- Cada archivo se procesa y valida individualmente
- Se crean registros de Content para cada archivo
- Respuesta incluye información de todos los archivos cargados
- Archivos que fallan no afectan a los exitosos

---

### TC-CONT-003: Cargar archivo de video exitosamente

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo de video válido disponible (MP4, MPEG, MOV, AVI o WEBM)

**Pasos:**
1. Navegar a `/Content/Upload`
2. Seleccionar archivo de video válido (ej: video.mp4)
3. Cargar archivo

**Resultado Esperado:**
- Video se carga exitosamente
- ContentType se establece como "Video"
- Archivo se guarda correctamente
- Registro de Content se crea con información del video

---

### TC-CONT-004: Cargar archivo asociado a campaña

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Campaña existe en el tenant
- Archivo válido disponible

**Pasos:**
1. Navegar a `/Content/Upload`
2. Seleccionar archivo
3. Seleccionar campaña del dropdown o proporcionar CampaignId
4. Cargar archivo

**Resultado Esperado:**
- Archivo se carga exitosamente
- Content se asocia a la campaña (CampaignId se guarda)
- Relación se establece correctamente en base de datos
- Archivo aparece en lista de contenido de la campaña

---

### TC-CONT-005: Cargar archivo con descripción y tags

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo válido disponible

**Pasos:**
1. Navegar a `/Content/Upload`
2. Seleccionar archivo
3. Ingresar descripción: "Imagen promocional Q1 2024"
4. Ingresar tags: "promocional, verano, producto"
5. Cargar archivo

**Resultado Esperado:**
- Archivo se carga exitosamente
- Descripción se guarda en Content
- Tags se guardan en Content
- Metadatos se preservan correctamente

---

### TC-CONT-006: Cargar archivo sin permisos

**Módulo:** Contenido  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado sin rol Marketer, Admin o Owner

**Pasos:**
1. Intentar acceder a `/Content/Upload` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Usuario es redirigido a `/Account/AccessDenied`
- No se puede cargar contenido

---

### TC-CONT-007: Cargar archivo sin autenticación

**Módulo:** Contenido  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Intentar acceder a `/Content/Upload` sin sesión

**Resultado Esperado:**
- Usuario es redirigido a `/Account/Login`
- No se puede cargar contenido

---

## Validaciones de Archivos

### TC-CONT-008: Validación - Sin archivos proporcionados

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos

**Pasos:**
1. Navegar a `/Content/Upload`
2. Intentar cargar sin seleccionar archivos
3. Hacer clic en "Cargar"

**Resultado Esperado:**
- Validación falla
- Respuesta BadRequest con error: "No se proporcionaron archivos."
- No se procesa ningún archivo
- Mensaje de error se muestra al usuario

---

### TC-CONT-009: Validación - Archivo vacío (tamaño 0)

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo vacío disponible (0 bytes)

**Pasos:**
1. Seleccionar archivo vacío
2. Intentar cargar

**Resultado Esperado:**
- Validación falla
- Error: "El archivo está vacío."
- Archivo NO se carga
- Validación se realiza tanto en cliente como servidor

---

### TC-CONT-010: Validación - Archivo excede tamaño máximo (100 MB)

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo mayor a 100 MB disponible

**Pasos:**
1. Seleccionar archivo mayor a 100 MB
2. Intentar cargar

**Resultado Esperado:**
- Validación falla
- Error: "El archivo excede el tamaño máximo permitido (100 MB)."
- Archivo NO se carga
- Límite se aplica: `[RequestSizeLimit(100 * 1024 * 1024)]`
- Límite también en `RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)`

---

### TC-CONT-011: Validación - Tipo MIME no permitido

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo con tipo MIME no permitido (ej: application/pdf, text/plain)

**Pasos:**
1. Seleccionar archivo con tipo MIME no permitido
2. Intentar cargar

**Resultado Esperado:**
- Validación falla
- Error: "Tipo de archivo no permitido. Tipos permitidos: imágenes (JPEG, PNG, GIF, WEBP) y videos (MP4, MPEG, MOV, AVI, WEBM)."
- Archivo NO se carga
- Tipos permitidos:
  - Imágenes: image/jpeg, image/jpg, image/png, image/gif, image/webp
  - Videos: video/mp4, video/mpeg, video/quicktime, video/x-msvideo, video/webm

---

### TC-CONT-012: Validación - Extensión de archivo no permitida

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo con extensión no permitida (ej: .pdf, .txt, .docx)

**Pasos:**
1. Seleccionar archivo con extensión no permitida
2. Intentar cargar

**Resultado Esperado:**
- Validación falla
- Error: "Extensión de archivo no permitida."
- Archivo NO se carga
- Extensiones permitidas: .jpg, .jpeg, .png, .gif, .webp, .mp4, .mpeg, .mov, .avi, .webm

---

### TC-CONT-013: Validación - Archivo con extensión permitida pero tipo MIME incorrecto

**Módulo:** Contenido  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo con extensión .jpg pero tipo MIME application/octet-stream

**Pasos:**
1. Seleccionar archivo con extensión válida pero tipo MIME inválido
2. Intentar cargar

**Resultado Esperado:**
- Validación falla por tipo MIME
- Sistema valida tanto extensión como tipo MIME
- Archivo NO se carga
- Seguridad se mantiene

---

### TC-CONT-014: Cargar archivo en límite de tamaño (100 MB exacto)

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario autenticado con permisos
- Archivo de exactamente 100 MB disponible

**Pasos:**
1. Seleccionar archivo de 100 MB exactos
2. Cargar archivo

**Resultado Esperado:**
- Archivo se carga exitosamente
- Validación acepta archivo de 100 MB (límite inclusivo)
- No se genera error

---

## Listar Contenido

### TC-CONT-015: Listar contenido cargado

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado
- Tenant tiene contenido cargado (imágenes y videos)

**Pasos:**
1. Navegar a `/Content/Index`
2. Verificar que se muestra lista de contenido

**Resultado Esperado:**
- Lista muestra contenido del tenant
- Contenido se muestra con información relevante:
  - Nombre de archivo
  - Tipo (Image/Video)
  - Tamaño
  - Fecha de carga
  - Asociación con campaña (si aplica)
- No se muestra contenido de otros tenants
- Vista se carga sin errores

---

### TC-CONT-016: Listar contenido sin contenido cargado

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Tenant no tiene contenido cargado
- Usuario está autenticado

**Pasos:**
1. Navegar a `/Content/Index`
2. Verificar vista

**Resultado Esperado:**
- Lista se muestra vacía
- Mensaje informativo se muestra si aplica
- No se generan errores
- Botón "Cargar Contenido" está disponible

---

## Asociación con Campañas

### TC-CONT-017: Asociar contenido a campaña existente

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Campaña existe en el tenant
- Usuario tiene permisos

**Pasos:**
1. Cargar archivo seleccionando campaña del dropdown
2. Verificar asociación

**Resultado Esperado:**
- Content se asocia correctamente a la campaña
- CampaignId se guarda en Content
- Relación se establece en base de datos
- Contenido aparece en lista de contenido de la campaña

---

### TC-CONT-018: Cargar contenido sin asociar a campaña

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos

**Pasos:**
1. Cargar archivo sin seleccionar campaña
2. Verificar que se guarda sin CampaignId

**Resultado Esperado:**
- Archivo se carga exitosamente
- CampaignId es null
- Contenido se puede asociar a campaña posteriormente
- No se generan errores

---

### TC-CONT-019: Asociar contenido a campaña de otro tenant

**Módulo:** Contenido  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Campaña pertenece a Tenant2
- Usuario pertenece a Tenant1

**Pasos:**
1. Usuario de Tenant1 intenta cargar contenido asociado a campaña de Tenant2

**Resultado Esperado:**
- Sistema valida que campaña pertenece al tenant del usuario
- Si no pertenece, error apropiado
- Contenido NO se asocia a campaña de otro tenant
- Integridad multi-tenant se mantiene

---

## Tipos de Archivo

### TC-CONT-020: Cargar imagen JPEG

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Archivo JPEG válido disponible

**Pasos:**
1. Cargar archivo .jpg o .jpeg
2. Verificar que ContentType = "Image"

**Resultado Esperado:**
- Archivo se carga exitosamente
- ContentType se determina correctamente como "Image"
- Tipo MIME se valida: image/jpeg o image/jpg

---

### TC-CONT-021: Cargar imagen PNG

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Archivo PNG válido disponible

**Pasos:**
1. Cargar archivo .png
2. Verificar procesamiento

**Resultado Esperado:**
- Archivo se carga exitosamente
- ContentType = "Image"
- Tipo MIME: image/png

---

### TC-CONT-022: Cargar imagen GIF

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Archivo GIF válido disponible

**Pasos:**
1. Cargar archivo .gif
2. Verificar procesamiento

**Resultado Esperado:**
- Archivo se carga exitosamente
- ContentType = "Image"
- Tipo MIME: image/gif

---

### TC-CONT-023: Cargar imagen WEBP

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Archivo WEBP válido disponible

**Pasos:**
1. Cargar archivo .webp
2. Verificar procesamiento

**Resultado Esperado:**
- Archivo se carga exitosamente
- ContentType = "Image"
- Tipo MIME: image/webp

---

### TC-CONT-024: Cargar video MP4

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Archivo MP4 válido disponible

**Pasos:**
1. Cargar archivo .mp4
2. Verificar que ContentType = "Video"

**Resultado Esperado:**
- Archivo se carga exitosamente
- ContentType se determina correctamente como "Video"
- Tipo MIME: video/mp4

---

### TC-CONT-025: Cargar video MOV

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Archivo MOV válido disponible

**Pasos:**
1. Cargar archivo .mov
2. Verificar procesamiento

**Resultado Esperado:**
- Archivo se carga exitosamente
- ContentType = "Video"
- Tipo MIME: video/quicktime

---

## Límites de Tamaño

### TC-CONT-026: Cargar archivo pequeño (< 1 MB)

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- Archivo pequeño disponible

**Pasos:**
1. Cargar archivo pequeño
2. Verificar procesamiento

**Resultado Esperado:**
- Archivo se carga exitosamente
- No hay problemas con archivos pequeños
- FileSize se registra correctamente

---

### TC-CONT-027: Cargar archivo grande (cercano a 100 MB)

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Archivo de ~99 MB disponible

**Pasos:**
1. Cargar archivo grande
2. Verificar procesamiento

**Resultado Esperado:**
- Archivo se carga exitosamente
- Tiempo de carga puede ser mayor pero funciona
- FileSize se registra correctamente
- No hay timeouts inesperados

---

## Roles y Permisos

### TC-CONT-028: Usuario Marketer puede cargar contenido

**Módulo:** Contenido  
**Tipo:** Seguridad  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario tiene rol "Marketer"
- Usuario está autenticado

**Pasos:**
1. Verificar acceso a `/Content/Upload` (debe permitir)
2. Verificar acceso a `/Content/Index` (debe permitir)

**Resultado Esperado:**
- Marketer puede cargar contenido
- Marketer puede ver lista de contenido
- Permisos se aplican correctamente según `[AuthorizeRole]`

---

### TC-CONT-029: Usuario Viewer NO puede cargar contenido

**Módulo:** Contenido  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene rol que NO incluye Marketer, Admin o Owner

**Pasos:**
1. Intentar acceder a `/Content/Upload` sin permisos

**Resultado Esperado:**
- Acceso denegado
- Redirección a `/Account/AccessDenied`
- No se puede cargar contenido

---

## Multi-Tenant

### TC-CONT-030: Contenido se asigna al tenant correcto

**Módulo:** Contenido  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario pertenece a Tenant1
- Usuario tiene permisos

**Pasos:**
1. Usuario de Tenant1 carga archivo
2. Verificar en base de datos que TenantId del Content es Tenant1

**Resultado Esperado:**
- Content se crea con `TenantId = Tenant1` (del usuario)
- No se puede asignar manualmente otro TenantId
- Integridad de datos multi-tenant se mantiene

---

### TC-CONT-031: Lista de contenido filtra por TenantId

**Módulo:** Contenido  
**Tipo:** Multi-tenant  
**Prioridad:** Crítica

**Precondiciones:**
- Tenant1 tiene 5 archivos
- Tenant2 tiene 10 archivos

**Pasos:**
1. Usuario de Tenant1 accede a `/Content/Index`
2. Verificar contenido mostrado
3. Usuario de Tenant2 accede a `/Content/Index`
4. Verificar contenido mostrado

**Resultado Esperado:**
- Usuario de Tenant1 ve solo sus 5 archivos
- Usuario de Tenant2 ve solo sus 10 archivos
- No hay mezcla de datos entre tenants
- Filtrado por TenantId funciona correctamente

---

### TC-CONT-032: Acceso a contenido sin TenantId

**Módulo:** Contenido  
**Tipo:** Multi-tenant  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin TenantId válido
- Usuario NO es SuperAdmin

**Pasos:**
1. Intentar acceder a `/Content/Upload` sin TenantId

**Resultado Esperado:**
- Si no hay TenantId, se retorna BadRequest
- Error: "Error de autenticación. Por favor, inicie sesión nuevamente."
- No se puede cargar contenido

---

## Manejo de Errores

### TC-CONT-033: Manejo de errores al cargar archivo

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Sistema tiene error (almacenamiento, BD, etc.)

**Pasos:**
1. Intentar cargar archivo cuando hay error de sistema
2. Verificar manejo

**Resultado Esperado:**
- Error se captura
- Respuesta StatusCode(500) con error: "Error al procesar los archivos. Por favor, intente nuevamente."
- Error se registra en logs
- Usuario recibe mensaje apropiado

---

### TC-CONT-034: Cargar múltiples archivos con algunos inválidos

**Módulo:** Contenido  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos
- Múltiples archivos: algunos válidos, algunos inválidos

**Pasos:**
1. Seleccionar 3 archivos: 2 válidos, 1 inválido (tipo no permitido)
2. Intentar cargar todos

**Resultado Esperado:**
- Archivos válidos se cargan exitosamente
- Archivo inválido se rechaza con error específico
- Respuesta incluye información de éxitos y fallos
- No se bloquea todo el proceso por un archivo inválido

---

### TC-CONT-035: Error de autenticación durante carga

**Módulo:** Contenido  
**Tipo:** Seguridad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario autenticado pero sin UserId o TenantId válido

**Pasos:**
1. Intentar cargar archivo cuando UserId o TenantId son null

**Resultado Esperado:**
- BadRequest se retorna
- Error: "Error de autenticación. Por favor, inicie sesión nuevamente."
- Archivo NO se carga

---

## Vista Previa

### TC-CONT-036: Vista previa de imágenes antes de cargar

**Módulo:** Contenido  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- JavaScript de content-upload.js está activo

**Pasos:**
1. Seleccionar archivo de imagen
2. Verificar que se muestra vista previa antes de cargar

**Resultado Esperado:**
- Vista previa de imagen se muestra
- Nombre de archivo se muestra
- Tamaño se muestra
- Usuario puede verificar antes de cargar

---

### TC-CONT-037: Vista previa de videos antes de cargar

**Módulo:** Contenido  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario tiene permisos
- JavaScript activo

**Pasos:**
1. Seleccionar archivo de video
2. Verificar que se muestra información del video

**Resultado Esperado:**
- Información del video se muestra (nombre, tamaño)
- Vista previa puede mostrar thumbnail si está disponible
- Usuario puede verificar antes de cargar

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-CONT-001 | Cargar archivo único (imagen) | Funcional | Crítica | - |
| TC-CONT-002 | Cargar múltiples archivos | Funcional | Alta | - |
| TC-CONT-003 | Cargar archivo de video | Funcional | Alta | - |
| TC-CONT-004 | Cargar asociado a campaña | Funcional | Alta | - |
| TC-CONT-005 | Cargar con descripción y tags | Funcional | Media | - |
| TC-CONT-006 | Cargar sin permisos | Seguridad | Crítica | - |
| TC-CONT-007 | Cargar sin autenticación | Seguridad | Crítica | - |
| TC-CONT-008 | Validación sin archivos | Funcional | Alta | - |
| TC-CONT-009 | Validación archivo vacío | Funcional | Alta | - |
| TC-CONT-010 | Validación tamaño > 100 MB | Funcional | Crítica | - |
| TC-CONT-011 | Validación tipo MIME no permitido | Funcional | Alta | - |
| TC-CONT-012 | Validación extensión no permitida | Funcional | Alta | - |
| TC-CONT-013 | Validación extensión válida pero MIME incorrecto | Seguridad | Alta | - |
| TC-CONT-014 | Cargar archivo de 100 MB exacto | Funcional | Media | - |
| TC-CONT-015 | Listar contenido cargado | Funcional | Alta | - |
| TC-CONT-016 | Listar sin contenido | Funcional | Media | - |
| TC-CONT-017 | Asociar a campaña existente | Funcional | Alta | - |
| TC-CONT-018 | Cargar sin asociar a campaña | Funcional | Media | - |
| TC-CONT-019 | Asociar a campaña otro tenant | Multi-tenant | Crítica | - |
| TC-CONT-020 | Cargar imagen JPEG | Funcional | Media | - |
| TC-CONT-021 | Cargar imagen PNG | Funcional | Media | - |
| TC-CONT-022 | Cargar imagen GIF | Funcional | Media | - |
| TC-CONT-023 | Cargar imagen WEBP | Funcional | Media | - |
| TC-CONT-024 | Cargar video MP4 | Funcional | Alta | - |
| TC-CONT-025 | Cargar video MOV | Funcional | Media | - |
| TC-CONT-026 | Cargar archivo pequeño | Funcional | Media | - |
| TC-CONT-027 | Cargar archivo grande | Funcional | Alta | - |
| TC-CONT-028 | Permisos Marketer | Seguridad | Crítica | - |
| TC-CONT-029 | Permisos Viewer | Seguridad | Alta | - |
| TC-CONT-030 | Contenido asignado al tenant correcto | Multi-tenant | Crítica | - |
| TC-CONT-031 | Lista filtra por TenantId | Multi-tenant | Crítica | - |
| TC-CONT-032 | Acceso sin TenantId | Multi-tenant | Alta | - |
| TC-CONT-033 | Manejo de errores al cargar | Funcional | Alta | - |
| TC-CONT-034 | Cargar múltiples con algunos inválidos | Funcional | Alta | - |
| TC-CONT-035 | Error de autenticación durante carga | Seguridad | Alta | - |
| TC-CONT-036 | Vista previa de imágenes | UI | Media | - |
| TC-CONT-037 | Vista previa de videos | UI | Media | - |

**Total de casos:** 37  
**Críticos:** 8  
**Altos:** 18  
**Medios:** 11

