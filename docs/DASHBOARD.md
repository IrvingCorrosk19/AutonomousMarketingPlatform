# Dashboard Principal - Diseño SaaS Ejecutivo

## Visión General

Dashboard principal del CMS diseñado para transmitir **control, confianza y automatización**. El diseño comunica claramente que "el sistema está trabajando solo" mientras el usuario mantiene control total.

## Objetivos del Diseño

### Mensajes Clave
- ✅ **"El sistema está trabajando solo"** - Automatización 24/7 visible
- ✅ **Control** - Usuario puede ver y gestionar todo
- ✅ **Confianza** - Transparencia en métricas y estado
- ✅ **Profesionalismo** - Diseño SaaS ejecutivo, no genérico

## Componentes del Dashboard

### 1. Estado del Sistema (Card Principal)

**Ubicación:** Parte superior, ancho completo

**Contenido:**
- Estado actual (Activo/Pausado)
- Usuarios activos en las últimas 24 horas
- Última actividad del sistema
- Mensaje de estado operativo

**Diseño:**
- 4 Info Boxes con iconos grandes
- Colores según estado (verde = activo, amarillo = pausado)
- Card con borde superior destacado
- Alertas informativas

### 2. Métricas Principales (Small Boxes)

**Ubicación:** Fila debajo del estado del sistema

**Métricas mostradas:**
- Total de Campañas
- Campañas Activas
- Contenido Generado por IA
- Automatizaciones Activas

**Diseño:**
- 4 Small Boxes con gradientes
- Iconos grandes y visibles
- Enlaces a secciones relacionadas
- Efecto hover con elevación

### 3. Automatizaciones 24/7

**Ubicación:** Columna izquierda, mitad inferior

**Contenido:**
- Tabla de automatizaciones activas
- Tipo de automatización
- Estado actual
- Próxima ejecución programada
- Rendimiento (tasa de éxito)

**Características:**
- Badges de estado con colores
- Indicador de próxima ejecución
- Botón para gestionar automatizaciones
- Mensaje claro: "El sistema está trabajando solo"

### 4. Contenido Reciente

**Ubicación:** Columna derecha, mitad inferior

**Contenido:**
- Tabla de últimos archivos cargados
- Nombre del archivo
- Tipo (Imagen/Video)
- Origen (IA/Usuario)
- Fecha de carga

**Características:**
- Badges diferenciados por tipo
- Indicador de origen (IA vs Usuario)
- Botón para cargar nuevo contenido
- Resumen de totales (imágenes/videos)

### 5. Campañas Recientes

**Ubicación:** Columna izquierda, parte inferior

**Contenido:**
- Tabla de últimas campañas
- Nombre y estado
- Presupuesto y gastado
- Cantidad de contenido asociado
- Fecha de creación

**Características:**
- Badges de estado de campaña
- Información de presupuesto visible
- Enlace a gestión de campañas
- Botón para crear nueva campaña

### 6. Métricas Rápidas

**Ubicación:** Columna derecha, parte inferior

**Contenido:**
- Presupuesto total (barra de progreso)
- Gastado vs Presupuesto (barra de progreso con colores)
- Rendimiento promedio (barra de progreso)
- Totales rápidos (archivos, automatizaciones)

**Características:**
- Progress bars con colores contextuales
- Descripción blocks para métricas
- Diseño compacto y claro

### 7. Indicador de Sistema Autónomo

**Ubicación:** Columna derecha, card destacado

**Contenido:**
- Icono de robot animado
- Mensaje: "El sistema está trabajando solo"
- Contador de automatizaciones activas
- Texto explicativo

**Características:**
- Card con borde verde (éxito)
- Animación sutil del icono
- Mensaje claro y directo
- Transmite confianza y automatización

## Paleta de Colores del Dashboard

### Estados
- **Activo/Éxito:** Verde (#27AE60)
- **Advertencia/Pausado:** Amarillo (#F39C12)
- **Error:** Rojo (#E74C3C)
- **Info:** Azul (#5DADE2)
- **Primario:** Gris oscuro (#2C3E50)

### Gradientes
- **Primary:** #2C3E50 → #34495E
- **Success:** #27AE60 → #229954
- **Info:** #5DADE2 → #3498DB
- **Warning:** #F39C12 → #E67E22

## Características de Diseño

### 1. Transparencia
- Todas las métricas son visibles
- Estado del sistema siempre visible
- Sin información oculta

### 2. Control Visual
- Botones de acción claros
- Enlaces a todas las secciones
- Fácil navegación

### 3. Confianza
- Datos actualizados
- Indicadores de estado claros
- Métricas precisas

### 4. Automatización Visible
- Contador de automatizaciones activas
- Próximas ejecuciones visibles
- Rendimiento de automatizaciones
- Card destacado "Sistema Autónomo"

## Actualización en Tiempo Real

### Auto-refresh
- El dashboard se actualiza automáticamente cada 30 segundos
- Solo si la pestaña está activa (no consume recursos innecesarios)
- Mantiene los datos siempre actualizados

### Indicadores Visuales
- Iconos animados (robot, engranajes)
- Badges de estado con colores
- Progress bars animadas
- Efectos hover sutiles

## Responsive Design

### Desktop (> 992px)
- Layout completo con todas las columnas
- Tablas completas
- Info boxes en fila

### Tablet (768px - 992px)
- Columnas se reorganizan
- Tablas con scroll horizontal
- Info boxes en 2 columnas

### Mobile (< 768px)
- Una columna
- Tablas apiladas
- Info boxes apilados
- Botones full-width

## Flujo de Datos

```
HomeController.Index()
    ↓
GetDashboardDataQuery
    ↓
GetDashboardDataQueryHandler
    ↓
Repositorios (Campaigns, Content, Automations, Users)
    ↓
DashboardDto (agregado)
    ↓
Vista Index.cshtml (renderizado)
```

## Próximos Pasos

1. ✅ Dashboard diseñado e implementado
2. ✅ Casos de uso creados
3. ✅ Vista completa con todos los widgets
4. ✅ Estilos CSS profesionales
5. ⏳ Integrar con autenticación (obtener UserId real)
6. ⏳ Agregar gráficos (charts) para métricas
7. ⏳ Notificaciones en tiempo real
8. ⏳ Filtros y búsqueda en tablas

## Archivos Creados

### Application Layer
- `DTOs/DashboardDto.cs`
- `UseCases/Dashboard/GetDashboardDataQuery.cs`

### Web Layer
- `Controllers/HomeController.cs` (actualizado)
- `Views/Home/Index.cshtml` (rediseñado)
- `wwwroot/css/dashboard.css` (nuevo)

El dashboard está listo y transmite profesionalismo, control y confianza.

