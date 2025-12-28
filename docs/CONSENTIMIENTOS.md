# Sistema de Consentimientos y Autorización - PROMPT 6

## Visión General

Sistema completo de gestión de consentimientos explícitos del usuario para cumplimiento legal (GDPR, etc.) y control de acceso a funcionalidades de IA.

## Características Implementadas

### 1. Casos de Uso (Application Layer)

#### GetUserConsentsQuery
- Obtiene todos los consentimientos de un usuario
- Identifica consentimientos requeridos vs opcionales
- Muestra estado de cada consentimiento (otorgado/revocado)
- Calcula si todos los consentimientos requeridos están otorgados

#### GrantConsentCommand
- Otorga un consentimiento específico
- Registra IP, fecha y versión del documento
- Actualiza consentimiento existente o crea uno nuevo
- Valida que el usuario pertenece al tenant

#### RevokeConsentCommand
- Revoca un consentimiento (solo opcionales)
- Previene revocación de consentimientos requeridos
- Registra fecha de revocación

#### ValidateConsentQuery
- Valida si un usuario tiene un consentimiento específico otorgado
- Usado por servicios que requieren consentimiento

### 2. Servicio de Validación

#### IConsentValidationService
- `ValidateConsentAsync`: Valida un consentimiento específico
- `ValidateConsentsAsync`: Valida múltiples consentimientos
- `GetMissingConsentsAsync`: Obtiene lista de consentimientos faltantes

### 3. Tipos de Consentimiento

#### Requeridos (No pueden ser revocados)
- **AIGeneration**: Generación de contenido con IA
- **DataProcessing**: Procesamiento de datos

#### Opcionales (Pueden ser revocados)
- **AutoPublishing**: Publicación automática en redes
- **Analytics**: Análisis y métricas

### 4. Interfaz de Usuario

#### Vista de Consentimientos (`/Consents`)
- Lista todos los consentimientos disponibles
- Muestra estado actual (otorgado/no otorgado)
- Permite otorgar/revocar consentimientos
- Alerta sobre consentimientos requeridos faltantes
- Información legal y explicación de cada consentimiento

### 5. Middleware de Validación

#### ConsentValidationMiddleware
- Intercepta solicitudes a rutas que requieren consentimientos
- Valida que el usuario tenga los consentimientos necesarios
- Redirige a página de consentimientos si faltan
- Configurable por ruta

## Flujo de Trabajo

### Otorgar Consentimiento

1. Usuario accede a `/Consents`
2. Ve lista de consentimientos disponibles
3. Hace clic en "Otorgar Consentimiento"
4. Sistema registra:
   - Fecha y hora
   - IP del usuario
   - Versión del documento
   - Tenant y Usuario

### Validar Consentimiento

1. Usuario intenta acceder a funcionalidad que requiere consentimiento
2. Middleware intercepta la solicitud
3. Valida consentimientos requeridos
4. Si faltan, redirige a `/Consents`
5. Si están otorgados, permite continuar

### Revocar Consentimiento

1. Usuario accede a `/Consents`
2. Ve consentimientos otorgados
3. Hace clic en "Revocar" (solo opcionales)
4. Sistema registra fecha de revocación
5. Funcionalidades relacionadas dejan de estar disponibles

## Cumplimiento Legal

### GDPR Compliance

- ✅ Consentimiento explícito requerido
- ✅ Registro de IP y fecha
- ✅ Versión de documento de consentimiento
- ✅ Capacidad de revocación (donde aplica)
- ✅ Trazabilidad completa

### Información al Usuario

- ✅ Descripción clara de cada consentimiento
- ✅ Explicación de por qué se necesita
- ✅ Información sobre derechos del usuario
- ✅ Diferenciación entre requeridos y opcionales

## Integración con Otras Funcionalidades

### Antes de Usar IA

Cualquier operación que requiera IA debe validar:
```csharp
var hasConsent = await _consentValidationService.ValidateConsentAsync(
    userId, tenantId, "AIGeneration", cancellationToken);

if (!hasConsent)
{
    throw new UnauthorizedAccessException("Consentimiento requerido no otorgado.");
}
```

### Middleware Automático

El middleware valida automáticamente rutas configuradas:
- `/campaigns/create` → Requiere AIGeneration, DataProcessing
- `/campaigns/generate` → Requiere AIGeneration
- `/content/generate` → Requiere AIGeneration
- `/automations` → Requiere AIGeneration, DataProcessing, AutoPublishing

## Próximos Pasos

1. ✅ Casos de uso implementados
2. ✅ Servicio de validación implementado
3. ✅ Vistas y controladores creados
4. ✅ Middleware de validación creado
5. ⏳ Integrar con sistema de autenticación (obtener UserId real)
6. ⏳ Agregar más rutas al middleware según necesidad
7. ⏳ Crear documentos legales de consentimiento (PDFs)
8. ⏳ Agregar notificaciones cuando falten consentimientos

## Archivos Creados

### Application Layer
- `DTOs/ConsentDto.cs`
- `UseCases/Consents/GetUserConsentsQuery.cs`
- `UseCases/Consents/GrantConsentCommand.cs`
- `UseCases/Consents/RevokeConsentCommand.cs`
- `UseCases/Consents/ValidateConsentQuery.cs`
- `Services/IConsentValidationService.cs`

### Infrastructure Layer
- `Services/ConsentValidationService.cs`

### Web Layer
- `Controllers/ConsentsController.cs`
- `Views/Consents/Index.cshtml`
- `Middleware/ConsentValidationMiddleware.cs`

