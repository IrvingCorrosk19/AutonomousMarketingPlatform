# Arquitectura del Sistema - Autonomous Marketing Platform

## Visión General

El sistema está construido siguiendo **Clean Architecture** con soporte nativo para **multi-tenant** desde el inicio. Esto garantiza:

- ✅ Aislamiento total de datos por empresa
- ✅ Escalabilidad horizontal
- ✅ Mantenibilidad del código
- ✅ Separación clara de responsabilidades

## Estructura de Capas

### 1. Domain (Capa de Dominio)

**Ubicación:** `src/AutonomousMarketingPlatform.Domain/`

**Responsabilidad:** Contiene las entidades del negocio, interfaces y reglas de dominio. Esta capa NO tiene dependencias externas.

**Componentes principales:**
- **Entities:** Entidades del dominio (Tenant, User, Campaign, Content, etc.)
- **Interfaces:** Contratos que definen los servicios y repositorios
- **Common:** Clases base y utilidades compartidas

**Entidades principales:**
- `Tenant`: Representa una empresa en el sistema multi-tenant
- `User`: Usuarios del sistema (vinculados a un tenant)
- `Campaign`: Campañas de marketing
- `Content`: Contenido cargado o generado por IA
- `Consent`: Consentimientos explícitos del usuario
- `UserPreference`: Preferencias del usuario
- `MarketingMemory`: Memoria del sistema sobre conversaciones y aprendizajes
- `AutomationState`: Estado de las automatizaciones 24/7

**Características clave:**
- Todas las entidades (excepto `Tenant`) implementan `ITenantEntity` para garantizar aislamiento
- Clase base `BaseEntity` con propiedades comunes (Id, CreatedAt, UpdatedAt, IsActive)

### 2. Application (Capa de Aplicación)

**Ubicación:** `src/AutonomousMarketingPlatform.Application/`

**Responsabilidad:** Contiene la lógica de casos de uso, DTOs, validaciones y mapeos. Orquesta las operaciones del dominio.

**Componentes:**
- **Use Cases:** Casos de uso específicos del negocio
- **DTOs:** Objetos de transferencia de datos
- **Mappings:** Configuración de AutoMapper
- **Validators:** Validaciones con FluentValidation

**Dependencias:**
- Domain (referencia directa)
- MediatR para CQRS
- FluentValidation para validaciones
- AutoMapper para mapeos

### 3. Infrastructure (Capa de Infraestructura)

**Ubicación:** `src/AutonomousMarketingPlatform.Infrastructure/`

**Responsabilidad:** Implementa las interfaces definidas en Domain. Maneja acceso a datos, servicios externos y configuraciones técnicas.

**Componentes principales:**
- **Data:** DbContext y configuraciones de Entity Framework
- **Repositories:** Implementaciones de repositorios
- **Services:** Servicios de infraestructura (TenantService, etc.)
- **Integrations:** Integraciones con APIs externas (IA, redes sociales, etc.)

**Características clave:**
- `ApplicationDbContext`: Contexto de EF Core con filtrado automático por tenant
- `BaseRepository<T>`: Repositorio genérico con soporte multi-tenant
- `TenantService`: Servicio para obtener y validar el tenant actual

### 4. Web (Capa de Presentación)

**Ubicación:** `src/AutonomousMarketingPlatform.Web/`

**Responsabilidad:** Controllers, Views, Razor Pages. NO contiene lógica de negocio, solo orquesta llamadas a la capa Application.

**Componentes:**
- **Controllers:** Controladores MVC
- **Views:** Vistas Razor
- **Pages:** Razor Pages
- **Middleware:** Middleware personalizado (autenticación, tenant resolution, etc.)

## Sistema Multi-Tenant

### Estrategia de Implementación

El sistema utiliza **filtrado por TenantId** en todas las consultas. Esto garantiza:

1. **Aislamiento de datos:** Cada tenant solo ve sus propios datos
2. **Seguridad:** Imposible acceder a datos de otros tenants
3. **Escalabilidad:** Fácil particionar datos en el futuro si es necesario

### Cómo Funciona

1. **Identificación del Tenant:**
   - Por subdominio (ej: `empresa1.plataforma.com`)
   - Por header HTTP (`X-Tenant-Id`)
   - Por claim del usuario autenticado

2. **Filtrado Automático:**
   - Todas las entidades implementan `ITenantEntity`
   - El `BaseRepository` filtra automáticamente por `TenantId`
   - El `ApplicationDbContext` valida que el `TenantId` esté asignado

3. **Validación:**
   - El `TenantService` valida que el tenant existe y está activo
   - Middleware verifica el tenant antes de procesar la solicitud

### Estructura de Base de Datos

Todas las tablas (excepto `Tenants`) tienen:
- `tenant_id` (UUID, NOT NULL, INDEXED)
- Foreign key constraint para garantizar integridad
- Índices para optimizar consultas filtradas por tenant

## Flujo de Datos

```
Request → Web Layer (Controller/Page)
    ↓
Application Layer (Use Case/Handler)
    ↓
Domain Layer (Entities/Business Logic)
    ↓
Infrastructure Layer (Repository/DbContext)
    ↓
PostgreSQL Database
```

## Seguridad Multi-Tenant

### Garantías de Aislamiento

1. **Nivel de Repositorio:**
   - Todos los métodos del repositorio requieren `tenantId`
   - Filtrado automático en todas las consultas LINQ

2. **Nivel de DbContext:**
   - Validación en `SaveChangesAsync` para asegurar `TenantId`
   - No se pueden insertar registros sin `TenantId`

3. **Nivel de Middleware:**
   - Validación del tenant antes de procesar la solicitud
   - Rechazo automático si el tenant no existe o está inactivo

## Próximos Pasos

1. ✅ Estructura base de Clean Architecture
2. ✅ Sistema multi-tenant implementado
3. ✅ Modelo de datos completo
4. ⏳ Implementar casos de uso en Application Layer
5. ⏳ Configurar AdminLTE como base del CMS
6. ⏳ Crear diseño CSS personalizado
7. ⏳ Implementar autenticación y autorización
8. ⏳ Integraciones con servicios de IA

