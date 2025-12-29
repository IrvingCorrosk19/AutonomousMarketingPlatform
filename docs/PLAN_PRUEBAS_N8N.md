# Plan de Pruebas y Integración con n8n

## 📋 Resumen Ejecutivo

Este documento detalla:
1. **Pruebas que se pueden hacer AHORA** (sin n8n)
2. **Lo que falta para integrar con n8n**
3. **Pruebas post-integración con n8n**

---

## ✅ PRUEBAS QUE SE PUEDEN HACER AHORA

### 1. **Autenticación y Autorización**

#### 1.1 Login/Logout
- ✅ **Login con credenciales válidas**
  - Email: `admin@test.com` / Password: `Admin123!`
  - Email: `marketer@test.com` / Password: `Marketer123!`
  - Verificar redirección al dashboard
  - Verificar claims de usuario y tenant

- ✅ **Login sin tenant resuelto**
  - Acceder desde `localhost` sin header `X-Tenant-Id`
  - Sistema debe usar TenantId del usuario
  - Verificar que funciona correctamente

- ✅ **Login con credenciales inválidas**
  - Email incorrecto
  - Password incorrecto
  - Verificar mensajes de error

- ✅ **Logout**
  - Cerrar sesión
  - Verificar redirección a login
  - Verificar que no se puede acceder a rutas protegidas

#### 1.2 Autorización por Roles
- ✅ **Acceso Owner/Admin**
  - Verificar acceso a `/AIConfig`
  - Verificar acceso a creación de campañas
  - Verificar acceso a todas las funcionalidades

- ✅ **Acceso Marketer**
  - Verificar acceso limitado
  - Verificar que NO puede acceder a `/AIConfig`
  - Verificar que SÍ puede crear campañas

- ✅ **Acceso Viewer**
  - Verificar acceso de solo lectura
  - Verificar que NO puede crear/editar

---

### 2. **Gestión de Campañas**

#### 2.1 CRUD de Campañas
- ✅ **Crear Campaña**
  - `POST /Campaigns/Create`
  - Validar campos requeridos
  - Verificar que se guarda con TenantId correcto
  - Verificar estado inicial (Draft)

- ✅ **Listar Campañas**
  - `GET /Campaigns`
  - Verificar filtrado por tenant
  - Verificar filtrado por status
  - Verificar paginación (si existe)

- ✅ **Ver Detalle de Campaña**
  - `GET /Campaigns/Details/{id}`
  - Verificar que muestra datos correctos
  - Verificar botón "Ver Métricas"
  - Verificar que NO muestra campañas de otros tenants

- ✅ **Editar Campaña**
  - `GET /Campaigns/Edit/{id}`
  - `POST /Campaigns/Edit`
  - Verificar actualización de datos
  - Verificar validaciones

- ✅ **Activar/Desactivar Campaña**
  - Cambiar estado a "Active"
  - Cambiar estado a "Paused"
  - Cambiar estado a "Completed"
  - Verificar cambios en BD

#### 2.2 Validaciones
- ✅ **Fechas**
  - StartDate < EndDate
  - EndDate > StartDate
  - Fechas en el pasado (si está permitido)

- ✅ **Presupuesto**
  - Budget > 0
  - SpentAmount <= Budget

- ✅ **Multi-tenant**
  - No se puede acceder a campañas de otros tenants
  - No se puede crear campaña con TenantId incorrecto

---

### 3. **Gestión de Contenido**

#### 3.1 Carga de Archivos
- ✅ **Cargar Imágenes**
  - `POST /Content/Upload`
  - Múltiples archivos
  - Validar tipos permitidos (jpg, png, gif, webp)
  - Validar tamaño máximo
  - Verificar preview

- ✅ **Cargar Videos**
  - Validar tipos permitidos (mp4, avi, mov)
  - Validar tamaño máximo
  - Verificar preview

- ✅ **Validaciones**
  - Archivo muy grande (debe rechazar)
  - Tipo no permitido (debe rechazar)
  - Sin archivo (debe rechazar)

- ✅ **Listar Contenido**
  - `GET /Content`
  - Verificar filtrado por tenant
  - Verificar que muestra archivos cargados

#### 3.2 Generación con IA
- ✅ **Generar MarketingPack**
  - `POST /AI/GenerateMarketingPack`
  - Con ContentId válido
  - Con CampaignId válido
  - Verificar respuesta
  - Verificar que se guarda en BD

- ⚠️ **Nota**: Requiere API key de OpenAI configurada en `/AIConfig`

---

### 4. **Gestión de Publicaciones**

#### 4.1 Publicaciones
- ✅ **Listar Publicaciones**
  - `GET /Publishing`
  - Verificar filtrado por tenant
  - Verificar estados (Pending, Published, Failed)

- ✅ **Ver Detalle de Publicación**
  - `GET /Publishing/Details/{id}`
  - Verificar datos mostrados
  - Verificar botón "Ver Métricas"
  - Verificar estado actual

- ✅ **Crear Publicación Manual**
  - `POST /Publishing/Create`
  - Con MarketingPackId válido
  - Seleccionar canal (Instagram, Facebook, Twitter)
  - Programar fecha
  - Verificar que se guarda

#### 4.2 Estados de Publicación
- ✅ **Pending → Processing**
  - Verificar cambio de estado
  - Verificar que se procesa

- ✅ **Processing → Published**
  - Verificar publicación exitosa
  - Verificar URL generada

- ✅ **Processing → Failed**
  - Simular error
  - Verificar mensaje de error
  - Verificar reintentos

---

### 5. **Métricas y Analytics**

#### 5.1 Métricas de Campaña
- ✅ **Ver Métricas de Campaña**
  - `GET /Metrics/Campaign/{campaignId}`
  - Verificar gráficos
  - Verificar datos mostrados
  - Verificar filtrado por fechas

- ✅ **Registrar Métricas**
  - `POST /Metrics/Campaign`
  - Verificar que se guarda
  - Verificar que dispara aprendizaje automático

#### 5.2 Métricas de Publicación
- ✅ **Ver Métricas de Publicación**
  - `GET /Metrics/PublishingJob/{publishingJobId}`
  - Verificar datos mostrados
  - Verificar gráficos

- ✅ **Registrar Métricas de Publicación**
  - `POST /Metrics/PublishingJob`
  - Verificar que se guarda
  - Verificar que dispara aprendizaje automático

#### 5.3 Aprendizaje Automático
- ✅ **Trigger Automático**
  - Registrar métricas
  - Verificar en logs que se dispara aprendizaje
  - Verificar que no bloquea la respuesta

- ✅ **Background Service**
  - Verificar que corre diariamente
  - Verificar logs de ejecución
  - Verificar que procesa todos los tenants activos

---

### 6. **Memoria de Marketing**

#### 6.1 Visualización
- ✅ **Listar Memoria**
  - `GET /Memory`
  - Verificar filtrado por tipo
  - Verificar filtrado por tags
  - Verificar paginación

- ✅ **Ver Detalle de Memoria**
  - `GET /Memory/Details/{id}`
  - Verificar datos mostrados
  - Verificar que es solo lectura

#### 6.2 Consulta Automática
- ⚠️ **Nota**: La consulta automática se hace internamente antes de generar contenido
- ✅ **Verificar en Logs**
  - Al generar MarketingPack
  - Verificar que se consulta memoria
  - Verificar que se usa en contexto de IA

---

### 7. **Configuración de IA**

#### 7.1 API Key
- ✅ **Configurar API Key**
  - `GET /AIConfig`
  - `POST /AIConfig/Save`
  - Ingresar API key de OpenAI
  - Verificar que se encripta
  - Verificar que se guarda en BD

- ✅ **Verificar Uso**
  - Verificar estadísticas de uso
  - Verificar último uso
  - Verificar veces usado

- ✅ **Solo Owner/Admin**
  - Verificar que Marketer NO puede acceder
  - Verificar que Viewer NO puede acceder

---

### 8. **Consentimientos**

#### 8.1 CRUD de Consentimientos
- ✅ **Crear Consentimiento**
  - `POST /Consents/Create`
  - Verificar validaciones
  - Verificar que se guarda

- ✅ **Listar Consentimientos**
  - `GET /Consents`
  - Verificar filtrado por tenant
  - Verificar estados

- ✅ **Validación de Consentimientos**
  - Verificar middleware
  - Verificar que bloquea acciones sin consentimiento

---

### 9. **Multi-Tenant**

#### 9.1 Aislamiento de Datos
- ✅ **Verificar Filtrado**
  - Crear datos en Tenant A
  - Verificar que Tenant B NO los ve
  - Verificar en todas las entidades

#### 9.2 Resolución de Tenant
- ✅ **Por Header**
  - Enviar `X-Tenant-Id` en request
  - Verificar que se resuelve correctamente

- ✅ **Por Subdominio**
  - Acceder desde `test.localhost`
  - Verificar que resuelve tenant "test"

- ✅ **Sin Tenant Resuelto**
  - Acceder desde `localhost` sin header
  - Verificar que usa TenantId del usuario

---

### 10. **Dashboard**

#### 10.1 Vista Principal
- ✅ **Cargar Dashboard**
  - `GET /Home`
  - Verificar widgets
  - Verificar métricas
  - Verificar auto-refresh

#### 10.2 Datos Mostrados
- ✅ **Campañas Recientes**
  - Verificar que muestra campañas del tenant
  - Verificar orden (más recientes primero)

- ✅ **Contenido Reciente**
  - Verificar que muestra contenido del tenant
  - Verificar orden

- ✅ **Métricas**
  - Verificar cálculos
  - Verificar que son del tenant correcto

---

## 🔧 LO QUE FALTA PARA INTEGRAR CON N8N

### 1. **Implementación del Servicio de Automatización**

#### 1.1 ExternalAutomationService (Actual: MOCK)
**Ubicación**: `src/AutonomousMarketingPlatform.Infrastructure/Services/ExternalAutomationService.cs`

**Cambios Necesarios**:

```csharp
// ACTUAL (Mock):
public async Task<string> TriggerAutomationAsync(...)
{
    _logger.LogInformation("Triggering external automation...");
    // TODO: Implementar llamada real a n8n
    await Task.Delay(100, cancellationToken); // Simulación
    return Guid.NewGuid().ToString(); // Retornar request ID mock
}

// DEBE SER (Real):
public async Task<string> TriggerAutomationAsync(...)
{
    var n8nUrl = _configuration["N8N:WebhookUrl"];
    var n8nApiKey = _configuration["N8N:ApiKey"];
    
    var payload = new {
        tenantId = tenantId,
        eventType = eventType,
        eventData = eventData,
        userId = userId,
        relatedEntityId = relatedEntityId,
        additionalContext = additionalContext,
        timestamp = DateTime.UtcNow
    };
    
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("X-API-Key", n8nApiKey);
    
    var response = await httpClient.PostAsJsonAsync(n8nUrl, payload, cancellationToken);
    response.EnsureSuccessStatusCode();
    
    var result = await response.Content.ReadFromJsonAsync<N8nResponse>();
    return result.RequestId;
}
```

**Tareas**:
- [ ] Implementar `TriggerAutomationAsync` con HTTP client real
- [ ] Implementar `GetExecutionStatusAsync` con polling a n8n API
- [ ] Implementar `CancelExecutionAsync` con llamada a n8n API
- [ ] Implementar `ProcessWebhookResponseAsync` para guardar en BD
- [ ] Agregar configuración en `appsettings.json`:
  ```json
  "N8N": {
    "WebhookUrl": "https://tu-n8n-instance.com/webhook/trigger",
    "ApiUrl": "https://tu-n8n-instance.com/api/v1",
    "ApiKey": "tu-api-key",
    "TimeoutSeconds": 300,
    "RetryAttempts": 3
  }
  ```

---

### 2. **Controller para Webhooks de n8n**

#### 2.1 Crear WebhookController
**Ubicación**: `src/AutonomousMarketingPlatform.Web/Controllers/WebhookController.cs`

**Implementación Necesaria**:

```csharp
[ApiController]
[Route("api/webhooks")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebhookController> _logger;

    [HttpPost("n8n")]
    [AllowAnonymous] // O con autenticación por token
    public async Task<IActionResult> ReceiveN8nWebhook(
        [FromBody] WebhookResponseData responseData,
        [FromHeader(Name = "X-Tenant-Id")] string? tenantIdHeader,
        [FromHeader(Name = "X-Request-Id")] string? requestIdHeader)
    {
        // Validar autenticación (token o signature)
        // Resolver tenant
        // Procesar respuesta
        var command = new ProcessWebhookResponseCommand
        {
            TenantId = Guid.Parse(tenantIdHeader),
            RequestId = requestIdHeader ?? responseData.RequestId,
            ResponseData = responseData
        };
        
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }
}
```

**Tareas**:
- [ ] Crear `WebhookController.cs`
- [ ] Implementar endpoint `POST /api/webhooks/n8n`
- [ ] Agregar autenticación por token/signature
- [ ] Validar datos recibidos
- [ ] Procesar respuesta con `ProcessWebhookResponseCommand`

---

### 3. **Persistencia de Ejecuciones**

#### 3.1 AutomationExecution Entity
**Ubicación**: `src/AutonomousMarketingPlatform.Domain/Entities/AutomationExecution.cs`

**Estado Actual**: ✅ Ya existe

**Verificar**:
- [ ] Migración aplicada
- [ ] Tabla `AutomationExecutions` existe en BD
- [ ] Índices creados

#### 3.2 Repository/Service para AutomationExecution
**Tareas**:
- [ ] Crear `IAutomationExecutionRepository`
- [ ] Implementar `AutomationExecutionRepository`
- [ ] Registrar en DI container
- [ ] Usar en `ExternalAutomationService` para guardar ejecuciones

---

### 4. **Disparadores de Automatizaciones**

#### 4.1 Eventos que Deben Disparar n8n

**Ubicación**: En los handlers de comandos correspondientes

**Eventos a Implementar**:

1. **Nueva Campaña Creada**
   - `CampaignsController.Create` (POST)
   - Después de guardar campaña exitosamente
   ```csharp
   await _externalAutomationService.TriggerAutomationAsync(
       tenantId,
       "Campaign.Created",
       new { CampaignId = campaign.Id, ... },
       userId,
       campaign.Id
   );
   ```

2. **Campaña Activada**
   - `CampaignsController.Activate` (POST)
   - Cuando estado cambia a "Active"
   ```csharp
   await _externalAutomationService.TriggerAutomationAsync(
       tenantId,
       "Campaign.Activated",
       new { CampaignId = campaign.Id, ... },
       userId,
       campaign.Id
   );
   ```

3. **Nuevo Contenido Cargado**
   - `ContentController.Upload` (POST)
   - Después de guardar archivo exitosamente
   ```csharp
   await _externalAutomationService.TriggerAutomationAsync(
       tenantId,
       "Content.Uploaded",
       new { ContentId = content.Id, ... },
       userId,
       content.Id
   );
   ```

4. **MarketingPack Generado**
   - `AIController.GenerateMarketingPack` (POST)
   - Después de generar exitosamente
   ```csharp
   await _externalAutomationService.TriggerAutomationAsync(
       tenantId,
       "MarketingPack.Generated",
       new { MarketingPackId = pack.Id, ... },
       userId,
       pack.Id
   );
   ```

5. **Publicación Creada**
   - `PublishingController.Create` (POST)
   - Después de crear publicación
   ```csharp
   await _externalAutomationService.TriggerAutomationAsync(
       tenantId,
       "PublishingJob.Created",
       new { PublishingJobId = job.Id, ... },
       userId,
       job.Id
   );
   ```

**Tareas**:
- [ ] Inyectar `IExternalAutomationService` en handlers
- [ ] Agregar llamadas después de operaciones exitosas
- [ ] Manejar errores (no debe fallar la operación principal)
- [ ] Logging adecuado

---

### 5. **Configuración de n8n**

#### 5.1 Variables de Entorno
**Agregar a `appsettings.json`**:
```json
{
  "N8N": {
    "WebhookUrl": "https://tu-n8n-instance.com/webhook/autonomous-marketing",
    "ApiUrl": "https://tu-n8n-instance.com/api/v1",
    "ApiKey": "tu-api-key-aqui",
    "WebhookSecret": "secret-para-validar-webhooks",
    "TimeoutSeconds": 300,
    "RetryAttempts": 3,
    "RetryDelaySeconds": 60
  }
}
```

#### 5.2 Configuración por Tenant (Opcional)
**Si cada tenant tiene su propia instancia de n8n**:
- [ ] Crear tabla `TenantN8nConfig`
- [ ] UI para configurar URL/API key por tenant
- [ ] Usar configuración del tenant en `ExternalAutomationService`

---

### 6. **Workflows de n8n**

#### 6.1 Workflows Necesarios

**1. Procesamiento de Contenido**
- **Trigger**: Webhook `Content.Uploaded`
- **Acciones**:
  - Analizar imagen/video con IA
  - Generar tags automáticos
  - Optimizar imagen
  - Enviar resultado de vuelta al sistema

**2. Análisis de Campaña**
- **Trigger**: Webhook `Campaign.Created`
- **Acciones**:
  - Analizar estrategia de campaña
  - Generar recomendaciones
  - Enviar resultado de vuelta al sistema

**3. Publicación Automática**
- **Trigger**: Webhook `Campaign.Activated`
- **Acciones**:
  - Programar publicaciones
  - Publicar en redes sociales
  - Enviar confirmación al sistema

**4. Análisis de Métricas**
- **Trigger**: Webhook `Metrics.Registered`
- **Acciones**:
  - Analizar métricas
  - Generar insights
  - Enviar recomendaciones al sistema

**Tareas**:
- [ ] Crear workflows en n8n
- [ ] Configurar webhooks de entrada
- [ ] Configurar webhooks de salida (de vuelta al sistema)
- [ ] Probar cada workflow individualmente

---

## 🧪 PRUEBAS POST-INTEGRACIÓN CON N8N

### 1. **Pruebas de Disparo de Automatizaciones**

#### 1.1 Crear Campaña → Dispara n8n
- ✅ Crear campaña
- ✅ Verificar en logs que se dispara automatización
- ✅ Verificar que se guarda `AutomationExecution` en BD
- ✅ Verificar que n8n recibe el webhook
- ✅ Verificar respuesta de n8n
- ✅ Verificar que se actualiza estado en BD

#### 1.2 Activar Campaña → Dispara n8n
- ✅ Activar campaña
- ✅ Verificar disparo de automatización
- ✅ Verificar procesamiento en n8n
- ✅ Verificar respuesta

#### 1.3 Cargar Contenido → Dispara n8n
- ✅ Cargar imagen/video
- ✅ Verificar disparo de automatización
- ✅ Verificar procesamiento en n8n
- ✅ Verificar que se actualiza contenido con resultados

### 2. **Pruebas de Webhooks de Respuesta**

#### 2.1 Recibir Respuesta Exitosa
- ✅ n8n envía webhook de éxito
- ✅ Sistema procesa respuesta
- ✅ Verificar que se actualiza `AutomationExecution`
- ✅ Verificar que se guardan datos recibidos

#### 2.2 Recibir Respuesta de Error
- ✅ n8n envía webhook de error
- ✅ Sistema procesa error
- ✅ Verificar que se marca como `Failed`
- ✅ Verificar que se guarda mensaje de error

#### 2.3 Recibir Respuesta de Progreso
- ✅ n8n envía webhook de progreso
- ✅ Sistema actualiza progreso
- ✅ Verificar que se muestra en UI (si existe)

### 3. **Pruebas de Reintentos**

#### 3.1 Timeout
- ✅ Simular timeout (n8n no responde)
- ✅ Verificar que se marca como `Timeout`
- ✅ Verificar que se programa reintento
- ✅ Verificar reintento automático

#### 3.2 Error Recuperable
- ✅ Simular error temporal
- ✅ Verificar reintento
- ✅ Verificar límite de reintentos

### 4. **Pruebas de Seguridad**

#### 4.1 Autenticación de Webhooks
- ✅ Verificar que webhooks sin token son rechazados
- ✅ Verificar que webhooks con token válido son aceptados
- ✅ Verificar validación de signature

#### 4.2 Multi-Tenant
- ✅ Verificar que webhook de Tenant A no afecta Tenant B
- ✅ Verificar que `TenantId` en webhook es validado

### 5. **Pruebas de Performance**

#### 5.1 Múltiples Automatizaciones Simultáneas
- ✅ Disparar 10 automatizaciones simultáneas
- ✅ Verificar que todas se procesan
- ✅ Verificar que no hay bloqueos

#### 5.2 Automatizaciones de Larga Duración
- ✅ Disparar automatización que tarda 5 minutos
- ✅ Verificar que no hay timeout prematuro
- ✅ Verificar que se recibe respuesta correctamente

---

## 📊 CHECKLIST DE INTEGRACIÓN

### Fase 1: Preparación
- [ ] Implementar `ExternalAutomationService` real
- [ ] Crear `WebhookController`
- [ ] Agregar configuración en `appsettings.json`
- [ ] Verificar migración de `AutomationExecutions`

### Fase 2: Disparadores
- [ ] Agregar disparador en `CampaignsController.Create`
- [ ] Agregar disparador en `CampaignsController.Activate`
- [ ] Agregar disparador en `ContentController.Upload`
- [ ] Agregar disparador en `AIController.GenerateMarketingPack`
- [ ] Agregar disparador en `PublishingController.Create`

### Fase 3: Workflows en n8n
- [ ] Crear workflow "Procesamiento de Contenido"
- [ ] Crear workflow "Análisis de Campaña"
- [ ] Crear workflow "Publicación Automática"
- [ ] Crear workflow "Análisis de Métricas"
- [ ] Configurar webhooks de salida en cada workflow

### Fase 4: Pruebas
- [ ] Pruebas de disparo de automatizaciones
- [ ] Pruebas de recepción de webhooks
- [ ] Pruebas de reintentos
- [ ] Pruebas de seguridad
- [ ] Pruebas de performance

### Fase 5: Producción
- [ ] Configurar URLs de producción
- [ ] Configurar API keys de producción
- [ ] Monitoreo y alertas
- [ ] Documentación para usuarios

---

## 🎯 PRÓXIMOS PASOS RECOMENDADOS

1. **Semana 1**: Implementar `ExternalAutomationService` real y `WebhookController`
2. **Semana 2**: Agregar disparadores en los controladores principales
3. **Semana 3**: Crear workflows básicos en n8n y probar integración
4. **Semana 4**: Pruebas completas y ajustes finales

---

## 📝 NOTAS IMPORTANTES

- **No bloquear operaciones principales**: Los disparadores de n8n deben ser asíncronos y no deben fallar si n8n no está disponible
- **Logging exhaustivo**: Registrar todos los eventos de automatización para debugging
- **Manejo de errores**: Implementar reintentos y manejo de errores robusto
- **Seguridad**: Validar siempre autenticación de webhooks
- **Multi-tenant**: Asegurar que cada tenant solo puede disparar/recibir sus propias automatizaciones

