# AdminLTE como CMS Base - Configuración y Customización

## Objetivo

Usar AdminLTE como **base estructural** del CMS, pero transformarlo en un diseño moderno, sobrio y profesional que no se vea como una "plantilla gratis".

## Estrategia de Customización

### ✅ Qué Usar de AdminLTE

1. **Estructura HTML Base:**
   - Layout principal con sidebar y navbar
   - Sistema de grid y componentes base
   - JavaScript plugins útiles (charts, datatables, etc.)

2. **Componentes Funcionales:**
   - Sidebar navigation
   - Navbar con notificaciones
   - Cards y boxes
   - Modals y dropdowns
   - Form controls base

3. **Sistema de Iconos:**
   - Font Awesome (ya incluido en AdminLTE)

### ❌ Qué Ocultar/Eliminar

1. **Diseño Visual Default:**
   - Colores predeterminados (azul brillante)
   - Sombras excesivas
   - Bordes redondeados exagerados
   - Efectos hover llamativos

2. **Elementos Genéricos:**
   - Widgets de ejemplo
   - Gráficos de demostración
   - Menús innecesarios
   - Footer con branding de AdminLTE

3. **Componentes No Necesarios:**
   - Timeline genérico
   - Widgets de ejemplo
   - Páginas de demostración

## Estructura de Vistas Razor

### Layout Principal

```
Views/
├── Shared/
│   ├── _Layout.cshtml          # Layout principal con AdminLTE base
│   ├── _Sidebar.cshtml         # Sidebar customizado
│   ├── _Navbar.cshtml          # Navbar customizado
│   ├── _Footer.cshtml          # Footer minimalista
│   └── _Scripts.cshtml         # Scripts comunes
├── Home/
│   └── Index.cshtml            # Dashboard principal
└── ...
```

### Organización de Módulos

El sidebar se organizará por módulos funcionales:

1. **Dashboard**
   - Vista general del sistema
   - Métricas clave
   - Estado de automatizaciones

2. **Campañas**
   - Lista de campañas
   - Crear/Editar campaña
   - Análisis de campaña

3. **Contenido**
   - Biblioteca de contenido
   - Cargar imágenes/videos
   - Contenido generado por IA

4. **Automatizaciones**
   - Estado de automatizaciones
   - Configuración 24/7
   - Logs y resultados

5. **Preferencias**
   - Preferencias del usuario
   - Configuración de marca
   - Consentimientos

6. **Configuración**
   - Perfil de usuario
   - Configuración del tenant
   - Integraciones

## Sidebar Customizado

### Características

- **Minimalista:** Solo iconos y texto esencial
- **Jerárquico:** Agrupación lógica por módulos
- **Responsive:** Colapsable en móviles
- **Sin colores llamativos:** Paleta sobria y profesional

### Estructura

```html
<!-- Sidebar simplificado -->
<aside class="main-sidebar sidebar-dark-primary">
  <div class="sidebar">
    <nav class="mt-2">
      <ul class="nav nav-pills nav-sidebar flex-column">
        <!-- Dashboard -->
        <li class="nav-item">
          <a href="/" class="nav-link">
            <i class="nav-icon fas fa-tachometer-alt"></i>
            <p>Dashboard</p>
          </a>
        </li>
        
        <!-- Campañas -->
        <li class="nav-item">
          <a href="/campaigns" class="nav-link">
            <i class="nav-icon fas fa-bullhorn"></i>
            <p>Campañas</p>
          </a>
        </li>
        
        <!-- Más módulos... -->
      </ul>
    </nav>
  </div>
</aside>
```

## Navbar Customizado

### Características

- **Limpio:** Sin elementos innecesarios
- **Funcional:** Búsqueda, notificaciones, perfil
- **Branding:** Logo/nombre del tenant

### Elementos

- Logo/Nombre del tenant
- Búsqueda global (opcional)
- Notificaciones (estado de automatizaciones)
- Menú de usuario (perfil, configuración, logout)

## Dashboard

### Características

- **Enfoque en datos:** Métricas clave visibles
- **Cards informativos:** Estado de automatizaciones, campañas activas
- **Sin decoración excesiva:** Contenido sobre diseño

### Widgets Principales

1. **Resumen de Campañas**
   - Activas, pausadas, completadas
   - Presupuesto total vs gastado

2. **Estado de Automatizaciones**
   - Running, Paused, Errors
   - Próximas ejecuciones

3. **Contenido Reciente**
   - Últimos contenidos generados
   - Contenidos cargados

4. **Actividad Reciente**
   - Últimas acciones del sistema
   - Logs importantes

## Cards y Componentes

### Cards Personalizados

- **Sin sombras excesivas:** Sombras sutiles
- **Bordes limpios:** Bordes finos y discretos
- **Espaciado generoso:** Padding adecuado
- **Tipografía clara:** Jerarquía visual clara

### Estados Visuales

- **Success:** Verde suave (no brillante)
- **Warning:** Amarillo/ámbar discreto
- **Error:** Rojo sobrio
- **Info:** Azul profesional (no AdminLTE default)

## Responsive Design

- **Mobile-first:** Diseño pensado primero en móvil
- **Sidebar colapsable:** Se oculta en pantallas pequeñas
- **Tablas responsivas:** Scroll horizontal en móvil
- **Cards apilables:** Se apilan verticalmente en móvil

## Próximos Pasos

1. ✅ Estructura de vistas definida
2. ⏳ Crear layout base con AdminLTE
3. ⏳ Customizar sidebar y navbar
4. ⏳ Implementar dashboard
5. ⏳ Aplicar CSS personalizado (PROMPT 5)

