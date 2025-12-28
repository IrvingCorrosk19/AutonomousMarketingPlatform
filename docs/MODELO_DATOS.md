# Modelo de Datos - PostgreSQL Multi-Empresa

## Visión General

El modelo de datos está diseñado para soportar un sistema SaaS multi-tenant con aislamiento total de datos por empresa. Todas las tablas (excepto `Tenants`) están asociadas a un `tenant_id` obligatorio.

## Diagrama de Entidades

```
Tenants (1) ──< (N) Users
Tenants (1) ──< (N) Campaigns
Users (1) ──< (N) Consents
Users (1) ──< (N) UserPreferences
Campaigns (1) ──< (N) Contents
Campaigns (1) ──< (N) MarketingMemories
Campaigns (1) ──< (N) AutomationStates
```

## Tablas del Sistema

### 1. Tenants

**Propósito:** Representa una empresa (tenant) en el sistema multi-tenant.

**Campos principales:**
- `id` (UUID, PK): Identificador único del tenant
- `name` (VARCHAR 200): Nombre de la empresa
- `subdomain` (VARCHAR 100, UNIQUE): Subdominio único para acceso
- `contact_email` (VARCHAR 255): Email de contacto
- `subscription_plan` (VARCHAR 50): Plan de suscripción (Free, Basic, Pro, Enterprise)
- `is_active` (BOOLEAN): Indica si el tenant está activo
- `subscription_start_date` (TIMESTAMP): Fecha de inicio de suscripción
- `subscription_end_date` (TIMESTAMP, NULL): Fecha de expiración
- `created_at`, `updated_at`, `is_active` (heredados de BaseEntity)

**Índices:**
- `subdomain` (UNIQUE): Para búsqueda rápida por subdominio

**Razón de existencia:**
- Es la entidad raíz del sistema multi-tenant
- Permite identificar y aislar datos por empresa
- Soporta diferentes planes de suscripción

---

### 2. Users

**Propósito:** Usuarios del sistema, cada uno pertenece a un tenant específico.

**Campos principales:**
- `id` (UUID, PK): Identificador único del usuario
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO** - Identificador del tenant
- `email` (VARCHAR 255, UNIQUE con tenant_id): Email del usuario
- `password_hash` (VARCHAR): Hash de la contraseña
- `full_name` (VARCHAR 200): Nombre completo
- `role` (VARCHAR 50): Rol (Admin, Manager, User)
- `email_verified` (BOOLEAN): Si el email está verificado
- `email_verification_token` (VARCHAR, NULL): Token de verificación
- `last_login_at` (TIMESTAMP, NULL): Último inicio de sesión

**Índices:**
- `(tenant_id, email)` (UNIQUE): Garantiza emails únicos por tenant
- `tenant_id`: Para consultas rápidas por tenant

**Foreign Keys:**
- `tenant_id` → `Tenants.id` (ON DELETE RESTRICT)

**Razón de existencia:**
- Gestiona usuarios del sistema
- Permite autenticación y autorización por tenant
- Soporta roles y permisos

---

### 3. Consents

**Propósito:** Consentimientos explícitos del usuario para uso de IA y automatizaciones.

**Campos principales:**
- `id` (UUID, PK)
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO**
- `user_id` (UUID, FK → Users.id): Usuario que otorga el consentimiento
- `consent_type` (VARCHAR 100): Tipo (AIGeneration, DataProcessing, AutoPublishing)
- `is_granted` (BOOLEAN): Si está otorgado
- `granted_at` (TIMESTAMP, NULL): Fecha de otorgamiento
- `revoked_at` (TIMESTAMP, NULL): Fecha de revocación
- `consent_version` (VARCHAR, NULL): Versión del documento aceptado
- `ip_address` (VARCHAR, NULL): IP desde donde se otorgó

**Índices:**
- `(tenant_id, user_id, consent_type)`: Para búsqueda rápida
- `tenant_id`: Para consultas por tenant

**Foreign Keys:**
- `tenant_id` → `Tenants.id`
- `user_id` → `Users.id` (ON DELETE CASCADE)

**Razón de existencia:**
- Cumplimiento legal (GDPR, etc.)
- Requisito antes de usar IA
- Trazabilidad de consentimientos

---

### 4. Campaigns

**Propósito:** Campañas de marketing generadas por el sistema.

**Campos principales:**
- `id` (UUID, PK)
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO**
- `name` (VARCHAR 200): Nombre de la campaña
- `description` (TEXT, NULL): Descripción
- `status` (VARCHAR 50): Estado (Draft, Active, Paused, Completed, Archived)
- `marketing_strategy` (TEXT, NULL): Estrategia generada por IA
- `start_date` (TIMESTAMP, NULL): Fecha de inicio
- `end_date` (TIMESTAMP, NULL): Fecha de fin
- `budget` (DECIMAL, NULL): Presupuesto asignado
- `spent_amount` (DECIMAL, NULL): Presupuesto gastado

**Índices:**
- `tenant_id`: Para consultas por tenant
- `(tenant_id, status)`: Para filtrar campañas activas

**Foreign Keys:**
- `tenant_id` → `Tenants.id` (ON DELETE RESTRICT)

**Razón de existencia:**
- Organiza las campañas de marketing
- Permite rastrear estrategias y resultados
- Soporta presupuestos y análisis

---

### 5. Contents

**Propósito:** Contenido cargado por el usuario o generado por IA (imágenes, videos, texto).

**Campos principales:**
- `id` (UUID, PK)
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO**
- `campaign_id` (UUID, FK → Campaigns.id, NULL): Campaña asociada (opcional)
- `content_type` (VARCHAR 50): Tipo (Image, Video, Text, ReferenceImage, ReferenceVideo)
- `file_url` (VARCHAR 1000): URL o ruta del archivo
- `original_file_name` (VARCHAR, NULL): Nombre original
- `file_size` (BIGINT, NULL): Tamaño en bytes
- `mime_type` (VARCHAR, NULL): MIME type
- `is_ai_generated` (BOOLEAN): Si fue generado por IA
- `description` (TEXT, NULL): Descripción
- `tags` (VARCHAR, NULL): Tags separados por comas

**Índices:**
- `tenant_id`: Para consultas por tenant
- `campaign_id`: Para consultas por campaña
- `(tenant_id, content_type)`: Para filtrar por tipo

**Foreign Keys:**
- `tenant_id` → `Tenants.id`
- `campaign_id` → `Campaigns.id` (ON DELETE SET NULL)

**Razón de existencia:**
- Almacena todo el contenido del sistema
- Permite rastrear origen (usuario vs IA)
- Soporta organización por campañas

---

### 6. UserPreferences

**Propósito:** Preferencias del usuario que el sistema aprende y recuerda.

**Campos principales:**
- `id` (UUID, PK)
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO**
- `user_id` (UUID, FK → Users.id): Usuario propietario
- `preference_key` (VARCHAR 100): Clave (Tone, Style, TargetAudience, BrandColors)
- `preference_value` (TEXT): Valor (puede ser JSON para valores complejos)
- `category` (VARCHAR, NULL): Categoría (Marketing, Design, Publishing)
- `last_updated` (TIMESTAMP): Última actualización

**Índices:**
- `(tenant_id, user_id, preference_key)` (UNIQUE): Una preferencia por clave por usuario
- `tenant_id`: Para consultas por tenant

**Foreign Keys:**
- `tenant_id` → `Tenants.id`
- `user_id` → `Users.id` (ON DELETE CASCADE)

**Razón de existencia:**
- Personaliza el comportamiento del sistema
- Permite aprendizaje continuo
- Mejora la experiencia del usuario

---

### 7. MarketingMemories

**Propósito:** Memoria del sistema sobre conversaciones, decisiones y aprendizajes.

**Campos principales:**
- `id` (UUID, PK)
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO**
- `campaign_id` (UUID, FK → Campaigns.id, NULL): Campaña asociada
- `memory_type` (VARCHAR 50): Tipo (Conversation, Decision, Learning, Feedback)
- `content` (TEXT): Contenido de la memoria
- `context_json` (JSONB, NULL): Contexto adicional en JSON
- `tags` (VARCHAR, NULL): Tags para búsqueda
- `relevance_score` (INTEGER): Relevancia (1-10)
- `memory_date` (TIMESTAMP): Fecha del evento

**Índices:**
- `tenant_id`: Para consultas por tenant
- `campaign_id`: Para consultas por campaña
- `(tenant_id, memory_type)`: Para filtrar por tipo
- `context_json` (GIN): Para búsquedas en JSON (PostgreSQL)

**Foreign Keys:**
- `tenant_id` → `Tenants.id`
- `campaign_id` → `Campaigns.id` (ON DELETE SET NULL)

**Razón de existencia:**
- Permite que el sistema "recuerde" contexto
- Mejora la generación de contenido con IA
- Soporta aprendizaje continuo

---

### 8. AutomationStates

**Propósito:** Estado de las automatizaciones del sistema (marketing autónomo 24/7).

**Campos principales:**
- `id` (UUID, PK)
- `tenant_id` (UUID, FK → Tenants.id): **OBLIGATORIO**
- `campaign_id` (UUID, FK → Campaigns.id, NULL): Campaña asociada
- `automation_type` (VARCHAR 50): Tipo (ContentGeneration, Publishing, Analytics, StrategyUpdate)
- `status` (VARCHAR 50): Estado (Running, Paused, Completed, Error, Scheduled)
- `configuration_json` (JSONB, NULL): Configuración en JSON
- `last_execution_at` (TIMESTAMP, NULL): Última ejecución
- `next_execution_at` (TIMESTAMP, NULL): Próxima ejecución
- `execution_frequency` (VARCHAR, NULL): Frecuencia (Hourly, Daily, Weekly, Custom)
- `last_execution_result` (TEXT, NULL): Resultado de última ejecución
- `error_message` (TEXT, NULL): Mensaje de error si falló
- `success_count` (INTEGER): Ejecuciones exitosas
- `failure_count` (INTEGER): Ejecuciones fallidas

**Índices:**
- `tenant_id`: Para consultas por tenant
- `(tenant_id, status)`: Para filtrar automatizaciones activas
- `next_execution_at`: Para programar ejecuciones

**Foreign Keys:**
- `tenant_id` → `Tenants.id`
- `campaign_id` → `Campaigns.id` (ON DELETE SET NULL)

**Razón de existencia:**
- Controla el marketing autónomo 24/7
- Permite rastrear y depurar automatizaciones
- Soporta programación y monitoreo

---

## Reglas de Diseño

### 1. Tenant ID Obligatorio

**TODAS** las tablas (excepto `Tenants`) deben tener:
- Campo `tenant_id` (UUID, NOT NULL)
- Foreign key a `Tenants.id`
- Índice en `tenant_id` para rendimiento

### 2. Claves Primarias

- Todas las tablas usan UUID como clave primaria
- Ventajas: Únicas globalmente, no revelan información, seguras

### 3. Soft Delete

- Todas las entidades heredan `IsActive` de `BaseEntity`
- No se eliminan físicamente, se marcan como inactivas
- Permite auditoría y recuperación

### 4. Auditoría

- `CreatedAt`: Fecha de creación (automático)
- `UpdatedAt`: Fecha de última actualización (automático)
- `IsActive`: Soft delete flag

### 5. Índices para Rendimiento

- Índice en `tenant_id` en todas las tablas
- Índices compuestos para consultas frecuentes
- Índice GIN en campos JSONB para búsquedas rápidas

## Consideraciones para Crecimiento

### Particionamiento

En el futuro, si el sistema crece mucho, se puede particionar por `tenant_id`:
- Cada tenant en su propia partición
- Mejor rendimiento y mantenimiento
- Escalabilidad horizontal

### Analítica

El modelo soporta analítica futura:
- Campos de fecha para análisis temporal
- Campos numéricos para métricas (budget, spent_amount, success_count)
- JSONB para datos flexibles y consultables

### Escalabilidad

- Índices optimizados para consultas multi-tenant
- Foreign keys para integridad referencial
- Soft delete para no perder datos históricos

