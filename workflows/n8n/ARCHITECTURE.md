# Arquitectura de Workflows n8n

## Principios de Diseño

### 1. Modularidad
- Cada workflow es independiente y puede ejecutarse por separado
- Los workflows se comunican mediante HTTP Request/Response
- Cada workflow tiene una responsabilidad única y bien definida

### 2. Multi-Tenant Estricto
- Todos los workflows validan y respetan el `tenantId`
- Los datos se filtran automáticamente por tenant
- No hay fuga de datos entre tenants

### 3. Trazabilidad
- Cada solicitud incluye un `requestId` único
- Todas las decisiones se registran en la memoria de marketing
- Logs completos de ejecución para auditoría

### 4. Robustez
- Validación exhaustiva de campos
- Manejo de errores en cada paso
- Reintentos automáticos para operaciones críticas
- Timeouts configurados apropiadamente

### 5. Escalabilidad
- Diseñado para manejar múltiples solicitudes concurrentes
- Sin dependencias entre ejecuciones
- Uso eficiente de recursos

## Flujo de Datos

```
Backend (ASP.NET Core)
    ↓ HTTP POST
Webhook Trigger (n8n)
    ↓ Validación
Validation Node
    ├─ Error → Respond 400
    └─ Success → Continue
        ↓
Processing Nodes
    ↓
HTTP Request to Backend
    ↓
Response to Backend
```

## Estructura de Payload

### Entrada (Request)
```json
{
  "tenantId": "uuid",
  "userId": "uuid",
  "campaignId": "uuid (opcional)",
  "instruction": "string",
  "assets": ["url1", "url2"],
  "channels": ["instagram", "facebook"],
  "requiresApproval": true
}
```

### Salida (Response - Success)
```json
{
  "success": true,
  "message": "string",
  "data": {
    "tenantId": "uuid",
    "userId": "uuid",
    "campaignId": "uuid | null",
    "instruction": "string",
    "assets": ["url1"],
    "channels": ["instagram"],
    "requiresApproval": true,
    "timestamp": "ISO 8601",
    "requestId": "uuid"
  }
}
```

### Salida (Response - Error)
```json
{
  "success": false,
  "error": "string",
  "message": "string",
  "received": {
    "tenantId": "present | missing",
    "userId": "present | missing",
    "instruction": "present | missing",
    "channels": "present | missing",
    "requiresApproval": "present | missing"
  }
}
```

## Convenciones de Nomenclatura

### Nodos
- **Webhook**: `Webhook - [Descripción]`
- **Validación**: `Validate [Qué se valida]`
- **Procesamiento**: `[Acción] - [Objeto]`
- **Respuesta**: `Respond - [Tipo]`
- **Set de Datos**: `Set [Nombre del Dataset]`

### Workflows
- Formato: `[Número]-[Nombre-descriptivo].json`
- Ejemplo: `01-trigger-marketing-request.json`

### Variables
- Usar camelCase: `validatedData`, `requestId`, `tenantId`
- Prefijos descriptivos: `validated`, `processed`, `final`

## Manejo de Errores

### Niveles de Error
1. **Validación**: Error 400 - Campos faltantes o inválidos
2. **Procesamiento**: Error 500 - Error interno del workflow
3. **Timeout**: Error 504 - Timeout en operación externa
4. **Negocio**: Error 422 - Lógica de negocio fallida

### Estrategia de Reintentos
- Máximo 3 reintentos para operaciones críticas
- Backoff exponencial: 1s, 2s, 4s
- Solo reintentar errores transitorios (5xx)

## Seguridad

### Validación de Entrada
- Validar todos los campos requeridos
- Validar tipos de datos
- Validar formatos (UUIDs, URLs, etc.)
- Sanitizar strings para prevenir inyección

### Autenticación
- Usar tokens de API para comunicación backend ↔ n8n
- Validar tokens en cada request
- Rotar tokens periódicamente

### Aislamiento de Datos
- Nunca exponer datos de otros tenants
- Validar tenantId en cada operación
- Filtrar resultados por tenantId

## Performance

### Optimizaciones
- Usar HTTP Request en lugar de Function cuando sea posible
- Cachear resultados de consultas frecuentes
- Procesar en paralelo cuando sea posible
- Limitar tamaño de payloads

### Límites
- Timeout máximo: 30 segundos por operación
- Tamaño máximo de payload: 10MB
- Máximo de nodos por workflow: 50 (recomendado)

## Testing

### Casos de Prueba
1. **Happy Path**: Todos los campos válidos
2. **Campos Faltantes**: Falta cada campo requerido
3. **Tipos Inválidos**: Campos con tipos incorrectos
4. **Valores Vacíos**: Strings vacíos, arrays vacíos
5. **Multi-tenant**: Verificar aislamiento de datos

### Herramientas
- Postman/Insomnia para probar webhooks
- n8n Test Mode para probar workflows
- Logs de ejecución para debugging

## Monitoreo

### Métricas a Monitorear
- Tasa de éxito/fallo de workflows
- Tiempo de ejecución promedio
- Número de reintentos
- Errores por tipo

### Alertas
- Tasa de error > 5%
- Tiempo de ejecución > 30s
- Múltiples reintentos en corto tiempo

