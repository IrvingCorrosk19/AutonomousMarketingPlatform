# Sistema de Memoria de Marketing

## Visión General

El sistema de memoria permite que la plataforma "recuerde" contexto, preferencias, conversaciones y aprendizajes para mejorar continuamente la generación de contenido con IA.

## Tipos de Memoria

### 1. Memoria de Usuario

**Qué se guarda:**
- ✅ Preferencias de estilo (tono, voz, formato)
- ✅ Preferencias de contenido (temas favoritos, formatos preferidos)
- ✅ Feedback sobre contenido generado (me gusta, no me gusta)
- ✅ Patrones de uso (horarios, frecuencia)
- ✅ Configuraciones de marca (colores, estilos visuales)

**Qué NO se guarda:**
- ❌ Información personal sensible (datos bancarios, documentos)
- ❌ Contraseñas o credenciales
- ❌ Información de contacto de clientes (a menos que sea explícitamente autorizado)
- ❌ Conversaciones privadas fuera del contexto de marketing

### 2. Memoria de Conversación

**Qué se guarda:**
- ✅ Contexto de conversaciones sobre campañas
- ✅ Decisiones tomadas por el usuario
- ✅ Instrucciones específicas dadas a la IA
- ✅ Correcciones y ajustes solicitados
- ✅ Referencias a contenido anterior

**Qué NO se guarda:**
- ❌ Conversaciones fuera del contexto de marketing
- ❌ Información personal no relacionada con marketing
- ❌ Datos sensibles mencionados en conversaciones

### 3. Memoria de Campañas

**Qué se guarda:**
- ✅ Estrategias que funcionaron bien
- ✅ Estrategias que no funcionaron
- ✅ Métricas de rendimiento
- ✅ Ajustes realizados durante campañas
- ✅ Lecciones aprendidas por campaña

**Qué NO se guarda:**
- ❌ Datos de clientes individuales (a menos que sea necesario para la campaña)
- ❌ Información financiera detallada
- ❌ Datos de competencia obtenidos de forma no ética

### 4. Memoria de Aprendizaje

**Qué se guarda:**
- ✅ Patrones de éxito identificados
- ✅ Mejores prácticas descubiertas
- ✅ Optimizaciones que mejoraron resultados
- ✅ Correlaciones entre variables (ej: horario vs engagement)
- ✅ Preferencias del público objetivo (si es anónimo y agregado)

**Qué NO se guarda:**
- ❌ Datos personales identificables de audiencia
- ❌ Información que viole privacidad
- ❌ Datos obtenidos sin consentimiento

## Estructura de Almacenamiento

### Tabla: MarketingMemories

```sql
- id (UUID, PK)
- tenant_id (UUID, FK) - OBLIGATORIO
- campaign_id (UUID, FK, NULL) - Opcional
- memory_type (VARCHAR) - Conversation, Decision, Learning, Feedback
- content (TEXT) - Contenido de la memoria
- context_json (JSONB) - Contexto adicional estructurado
- tags (VARCHAR) - Tags para búsqueda
- relevance_score (INTEGER) - Relevancia 1-10
- memory_date (TIMESTAMP) - Fecha del evento
- created_at, updated_at, is_active
```

### Tipos de Memoria (memory_type)

1. **Conversation**: Conversaciones con el usuario
2. **Decision**: Decisiones tomadas
3. **Learning**: Aprendizajes del sistema
4. **Feedback**: Feedback del usuario sobre contenido
5. **Pattern**: Patrones identificados
6. **Preference**: Preferencias detectadas

## Flujo de Consulta Antes de Generar Contenido

### Proceso de Generación con Memoria

```
1. Usuario solicita generar contenido
   ↓
2. Sistema consulta memoria relevante:
   - Preferencias del usuario
   - Conversaciones recientes relacionadas
   - Campañas similares exitosas
   - Aprendizajes aplicables
   ↓
3. Sistema agrega contexto de memoria al prompt de IA
   ↓
4. IA genera contenido usando contexto + memoria
   ↓
5. Sistema guarda nueva memoria:
   - Contenido generado
   - Contexto usado
   - Resultado (si hay feedback)
```

### Consulta de Memoria

**Criterios de búsqueda:**
- TenantId (obligatorio - aislamiento)
- UserId (opcional - memoria específica del usuario)
- CampaignId (opcional - memoria de campaña específica)
- MemoryType (opcional - tipo de memoria)
- Tags (opcional - búsqueda por tags)
- RelevanceScore (mínimo - solo memorias relevantes)
- Fecha (rango - memorias recientes tienen más peso)

**Orden de relevancia:**
1. Memorias con mayor relevance_score
2. Memorias más recientes
3. Memorias del mismo usuario
4. Memorias de campañas similares

## Privacidad y Seguridad

### Aislamiento por Tenant
- ✅ Todas las memorias están asociadas a tenant_id
- ✅ Imposible acceder a memorias de otros tenants
- ✅ Consultas automáticamente filtradas por tenant

### Aislamiento por Usuario (Opcional)
- ✅ Memorias pueden estar asociadas a userId
- ✅ Usuarios solo ven sus propias memorias (a menos que sean Admin)
- ✅ Admins pueden ver todas las memorias del tenant

### Datos Sensibles
- ❌ NO se guardan datos personales identificables sin consentimiento
- ❌ NO se guardan credenciales o información financiera
- ❌ NO se comparten memorias entre tenants
- ✅ Se puede eliminar memoria bajo solicitud (GDPR)

### Anonimización
- ✅ Datos agregados se anonimizan
- ✅ Patrones se guardan sin identificar individuos
- ✅ Feedback se guarda sin datos personales

## Consulta de Memoria para IA

### Antes de Generar Contenido

```csharp
// 1. Obtener preferencias del usuario
var userPreferences = await GetUserMarketingMemories(userId, tenantId, "Preference");

// 2. Obtener conversaciones recientes
var recentConversations = await GetRecentConversations(userId, tenantId, days: 7);

// 3. Obtener aprendizajes relevantes
var learnings = await GetRelevantLearnings(tenantId, tags: ["content-generation"]);

// 4. Obtener campañas similares exitosas
var successfulCampaigns = await GetSuccessfulCampaignMemories(tenantId, similarContext);

// 5. Construir contexto para IA
var aiContext = BuildAIContext(userPreferences, recentConversations, learnings, successfulCampaigns);

// 6. Generar contenido con contexto
var content = await GenerateContentWithContext(prompt, aiContext);
```

## Visualización (Solo Lectura)

### Frontend
- ✅ Vista de memorias del usuario
- ✅ Vista de memorias de campaña
- ✅ Vista de aprendizajes del sistema
- ✅ Búsqueda y filtrado
- ❌ NO permite editar memorias directamente
- ❌ NO permite eliminar memorias (solo Admin)

### Permisos
- **Usuario**: Ve solo sus propias memorias
- **Manager**: Ve memorias de su equipo
- **Admin**: Ve todas las memorias del tenant

## Retención de Memoria

### Políticas
- Memorias con relevance_score < 3: Se archivan después de 90 días
- Memorias de conversación: Se mantienen 1 año
- Memorias de aprendizaje: Se mantienen indefinidamente (son valiosas)
- Memorias de feedback: Se mantienen 2 años

### Limpieza Automática
- Sistema limpia memorias archivadas periódicamente
- Memorias eliminadas se marcan como is_active = false (soft delete)
- Admins pueden restaurar memorias eliminadas

