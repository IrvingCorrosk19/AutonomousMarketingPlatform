# Casos de Prueba - Navegación y UI

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Navegación y Interfaz de Usuario

---

## Índice

- [Sidebar y Menú de Navegación](#sidebar-y-menú-de-navegación)
- [Navbar Superior](#navbar-superior)
- [Navegación entre Módulos](#navegación-entre-módulos)
- [Breadcrumbs y Rutas](#breadcrumbs-y-rutas)
- [Mensajes y Notificaciones](#mensajes-y-notificaciones)
- [Formularios y Validación Visual](#formularios-y-validación-visual)
- [Accesibilidad](#accesibilidad)

---

## Sidebar y Menú de Navegación

### TC-UI-001: Sidebar se muestra correctamente

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Layout principal se carga

**Pasos:**
1. Acceder a cualquier página después de login
2. Verificar que sidebar se muestra

**Resultado Esperado:**
- Sidebar se muestra en lado izquierdo
- Brand logo/texto "Marketing Platform" se muestra
- Menú de navegación está visible
- Estilos de AdminLTE 3 aplicados correctamente

---

### TC-UI-002: Menú de navegación muestra todos los módulos

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Verificar sidebar
2. Revisar todos los elementos del menú

**Resultado Esperado:**
- Menú muestra todos los módulos:
  - Dashboard (icono: tachometer-alt)
  - Campañas (icono: bullhorn)
  - Publicaciones (icono: paper-plane)
  - Métricas (icono: chart-line)
  - Cargar Contenido (icono: images)
  - Memoria (icono: brain)
  - Consentimientos (icono: shield-alt)
  - Configuración IA (icono: robot)
- Iconos FontAwesome se muestran correctamente
- Enlaces apuntan a rutas correctas

---

### TC-UI-003: Navegación a Dashboard desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en "Dashboard" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Home/Index`
- Dashboard se carga correctamente
- Link funciona: `/Home/Index`

---

### TC-UI-004: Navegación a Campañas desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en "Campañas" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Campaigns`
- Lista de campañas se carga
- Link funciona correctamente

---

### TC-UI-005: Navegación a Publicaciones desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en "Publicaciones" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Publishing`
- Lista de publicaciones se carga
- Link funciona correctamente

---

### TC-UI-006: Navegación a Métricas desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en "Métricas" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Metrics`
- Métricas se cargan
- Link funciona correctamente

---

### TC-UI-007: Navegación a Cargar Contenido desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado con permisos

**Pasos:**
1. Hacer clic en "Cargar Contenido" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Content/Upload`
- Formulario de carga se muestra
- Link funciona correctamente

---

### TC-UI-008: Navegación a Memoria desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en "Memoria" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Memory`
- Lista de memorias se carga
- Link funciona correctamente

---

### TC-UI-009: Navegación a Consentimientos desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en "Consentimientos" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/Consents`
- Lista de consentimientos se carga
- Link funciona correctamente

---

### TC-UI-010: Navegación a Configuración IA desde sidebar

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado con rol Owner o Admin

**Pasos:**
1. Hacer clic en "Configuración IA" en sidebar
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a `/AIConfig`
- Configuración de IA se carga
- Link funciona correctamente
- Si no tiene permisos, acceso denegado

---

### TC-UI-011: Sidebar colapsable (pushmenu)

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario está autenticado
- Navbar tiene botón de menú

**Pasos:**
1. Hacer clic en botón de menú (hamburger) en navbar
2. Verificar que sidebar se colapsa/expande

**Resultado Esperado:**
- Sidebar se colapsa o expande al hacer clic
- Funcionalidad pushmenu de AdminLTE funciona
- Estado se mantiene durante la sesión

---

### TC-UI-012: Item activo se resalta en sidebar

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario está en una página específica

**Pasos:**
1. Navegar a `/Campaigns/Index`
2. Verificar que item "Campañas" se resalta en sidebar

**Resultado Esperado:**
- Item del menú correspondiente a la página actual se marca como activo
- Clase `active` se aplica al nav-link
- Estilos de activo se aplican (background, color, font-weight)

---

## Navbar Superior

### TC-UI-013: Navbar se muestra correctamente

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Acceder a cualquier página
2. Verificar navbar superior

**Resultado Esperado:**
- Navbar se muestra en parte superior
- Contiene:
  - Botón de menú (hamburger)
  - Notificaciones dropdown
  - Información de tenant
  - Dropdown de usuario
- Estilos se aplican correctamente

---

### TC-UI-014: Botón de menú (pushmenu) funciona

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en botón de menú (icono bars)
2. Verificar que sidebar se colapsa/expande

**Resultado Esperado:**
- Botón funciona correctamente
- Sidebar responde al clic
- Icono FontAwesome se muestra

---

### TC-UI-015: Dropdown de notificaciones se muestra

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en icono de campana (notificaciones)
2. Verificar dropdown

**Resultado Esperado:**
- Dropdown de notificaciones se abre
- Muestra header "Notificaciones"
- Muestra notificaciones disponibles
- Badge con contador se muestra (inicialmente 0)

---

### TC-UI-016: Información de tenant se muestra en navbar

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado
- Usuario tiene TenantId en claims

**Pasos:**
1. Verificar navbar
2. Buscar información de tenant

**Resultado Esperado:**
- Información de tenant se muestra:
  - Icono: building
  - Texto: "Tenant:"
  - Valor: TenantId del usuario (GUID)
- Solo se muestra si usuario está autenticado
- Se oculta en pantallas pequeñas (d-none d-sm-inline-block)

---

### TC-UI-017: Dropdown de usuario se muestra

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en dropdown de usuario
2. Verificar opciones

**Resultado Esperado:**
- Dropdown se abre correctamente
- Muestra:
  - Header con nombre completo del usuario (FullName o Name)
  - Opción "Perfil"
  - Opción "Configuración"
  - Separador
  - Opción "Cerrar Sesión" (formulario POST)
- Nombre se obtiene de claim "FullName" o User.Identity.Name

---

### TC-UI-018: Cerrar sesión desde dropdown de usuario

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en dropdown de usuario
2. Hacer clic en "Cerrar Sesión"
3. Confirmar si se requiere

**Resultado Esperado:**
- Formulario POST se envía a `/Account/Logout`
- Token anti-forgery se incluye
- Sesión se cierra
- Usuario es redirigido a login

---

### TC-UI-019: Navbar muestra "Iniciar Sesión" cuando no autenticado

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario NO está autenticado

**Pasos:**
1. Acceder a página sin autenticación
2. Verificar navbar

**Resultado Esperado:**
- Link "Iniciar Sesión" se muestra en navbar
- Link apunta a `/Account/Login`
- Icono FontAwesome se muestra
- Dropdown de usuario NO se muestra

---

## Navegación entre Módulos

### TC-UI-020: Navegación directa por URL funciona

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Escribir URL directamente en navegador: `/Campaigns/Index`
2. Verificar que página se carga

**Resultado Esperado:**
- Página se carga correctamente
- Navegación directa funciona
- Layout se aplica correctamente

---

### TC-UI-021: Botones de acción en listas funcionan

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en lista de campañas
- Campañas existen

**Pasos:**
1. Hacer clic en botón "Ver" o "Detalles" de una campaña
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a página de detalles
- URL cambia correctamente
- Detalles se muestran

---

### TC-UI-022: Botones de editar funcionan

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos para editar
- Entidad existe

**Pasos:**
1. Hacer clic en botón "Editar"
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a página de edición
- Formulario se carga con datos actuales
- Navegación funciona correctamente

---

### TC-UI-023: Botones de crear funcionan

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario tiene permisos para crear

**Pasos:**
1. Hacer clic en botón "Crear" o "Nuevo"
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a página de creación
- Formulario vacío se muestra
- Navegación funciona correctamente

---

### TC-UI-024: Botón "Atrás" o "Volver" funciona

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario está en página de detalles o edición

**Pasos:**
1. Hacer clic en botón "Atrás" o "Volver"
2. Verificar navegación

**Resultado Esperado:**
- Usuario regresa a página anterior (lista)
- Navegación funciona correctamente
- O usa botón "Atrás" del navegador

---

## Mensajes y Notificaciones

### TC-UI-025: Mensaje de éxito se muestra (TempData)

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Operación exitosa se completa (ej: crear campaña)

**Pasos:**
1. Completar operación exitosa
2. Verificar que mensaje se muestra

**Resultado Esperado:**
- Mensaje de éxito se muestra en TempData
- Alert se muestra con clase "alert-success"
- Mensaje es claro y descriptivo
- Mensaje se puede cerrar (dismissible)

---

### TC-UI-026: Mensaje de error se muestra (TempData)

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Operación falla

**Pasos:**
1. Completar operación que falla
2. Verificar que mensaje de error se muestra

**Resultado Esperado:**
- Mensaje de error se muestra en TempData
- Alert se muestra con clase "alert-danger"
- Mensaje es claro y descriptivo
- Mensaje se puede cerrar

---

### TC-UI-027: Mensajes de validación se muestran en formularios

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario intenta enviar formulario con datos inválidos

**Pasos:**
1. Enviar formulario con datos inválidos
2. Verificar mensajes de validación

**Resultado Esperado:**
- Mensajes de validación se muestran por campo
- Mensajes son claros y específicos
- Campos con error se resaltan
- Usuario puede corregir y reintentar

---

## Formularios y Validación Visual

### TC-UI-028: Formularios se muestran correctamente

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario accede a formulario (crear, editar)

**Pasos:**
1. Acceder a formulario
2. Verificar presentación

**Resultado Esperado:**
- Formulario se muestra con estilo AdminLTE
- Campos se organizan correctamente
- Labels son claros
- Botones de acción están visibles

---

### TC-UI-029: Campos requeridos se marcan visualmente

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Formulario tiene campos requeridos

**Pasos:**
1. Ver formulario
2. Verificar que campos requeridos se marcan

**Resultado Esperado:**
- Campos requeridos se marcan con asterisco (*) o indicador visual
- Usuario puede identificar fácilmente campos obligatorios

---

### TC-UI-030: Botones de formulario funcionan

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en formulario

**Pasos:**
1. Completar formulario
2. Hacer clic en botón "Guardar" o "Crear"
3. Verificar que formulario se envía

**Resultado Esperado:**
- Botón funciona correctamente
- Formulario se envía (POST)
- Token anti-forgery se incluye
- Validación se ejecuta

---

### TC-UI-031: Botón "Cancelar" funciona

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario está en formulario

**Pasos:**
1. Hacer clic en botón "Cancelar"
2. Verificar navegación

**Resultado Esperado:**
- Usuario regresa a página anterior (lista)
- Cambios no guardados se descartan
- Navegación funciona correctamente

---

## Accesibilidad

### TC-UI-032: Iconos tienen texto alternativo o aria-labels

**Módulo:** Navegación y UI  
**Tipo:** Accesibilidad  
**Prioridad:** Media

**Precondiciones:**
- Página se carga

**Pasos:**
1. Inspeccionar iconos en la página
2. Verificar accesibilidad

**Resultado Esperado:**
- Iconos tienen aria-label o texto descriptivo
- Screen readers pueden interpretar iconos
- Accesibilidad se mantiene

---

### TC-UI-033: Enlaces tienen texto descriptivo

**Módulo:** Navegación y UI  
**Tipo:** Accesibilidad  
**Prioridad:** Media

**Precondiciones:**
- Página se carga

**Pasos:**
1. Revisar enlaces en la página
2. Verificar que tienen texto descriptivo

**Resultado Esperado:**
- Enlaces tienen texto claro (no solo iconos)
- Texto describe la acción o destino
- Accesibilidad se mantiene

---

### TC-UI-034: Formularios tienen labels asociados

**Módulo:** Navegación y UI  
**Tipo:** Accesibilidad  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en formulario

**Pasos:**
1. Revisar campos del formulario
2. Verificar que tienen labels

**Resultado Esperado:**
- Cada campo tiene label asociado
- Labels usan atributo `for` o están dentro de `<label>`
- Screen readers pueden asociar labels con campos

---

## Brand y Logo

### TC-UI-035: Brand logo/texto se muestra en sidebar

**Módulo:** Navegación y UI  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Verificar sidebar
2. Buscar brand logo/texto

**Resultado Esperado:**
- Brand link se muestra en parte superior del sidebar
- Texto: "Marketing Platform"
- Link apunta a "/" (home)
- Estilos se aplican correctamente

---

### TC-UI-036: Brand link navega a home

**Módulo:** Navegación y UI  
**Tipo:** Funcional  
**Prioridad:** Media

**Precondiciones:**
- Usuario está autenticado

**Pasos:**
1. Hacer clic en brand logo/texto
2. Verificar navegación

**Resultado Esperado:**
- Usuario es redirigido a "/" (home/dashboard)
- Navegación funciona correctamente

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-UI-001 | Sidebar se muestra correctamente | UI | Crítica | - |
| TC-UI-002 | Menú muestra todos los módulos | UI | Crítica | - |
| TC-UI-003 | Navegación a Dashboard | Funcional | Alta | - |
| TC-UI-004 | Navegación a Campañas | Funcional | Alta | - |
| TC-UI-005 | Navegación a Publicaciones | Funcional | Alta | - |
| TC-UI-006 | Navegación a Métricas | Funcional | Alta | - |
| TC-UI-007 | Navegación a Cargar Contenido | Funcional | Alta | - |
| TC-UI-008 | Navegación a Memoria | Funcional | Alta | - |
| TC-UI-009 | Navegación a Consentimientos | Funcional | Alta | - |
| TC-UI-010 | Navegación a Configuración IA | Funcional | Alta | - |
| TC-UI-011 | Sidebar colapsable | UI | Media | - |
| TC-UI-012 | Item activo se resalta | UI | Media | - |
| TC-UI-013 | Navbar se muestra correctamente | UI | Crítica | - |
| TC-UI-014 | Botón de menú funciona | UI | Media | - |
| TC-UI-015 | Dropdown de notificaciones | UI | Media | - |
| TC-UI-016 | Información de tenant en navbar | UI | Alta | - |
| TC-UI-017 | Dropdown de usuario | UI | Alta | - |
| TC-UI-018 | Cerrar sesión desde dropdown | Funcional | Crítica | - |
| TC-UI-019 | Navbar muestra Iniciar Sesión | UI | Media | - |
| TC-UI-020 | Navegación directa por URL | Funcional | Alta | - |
| TC-UI-021 | Botones de acción en listas | Funcional | Alta | - |
| TC-UI-022 | Botones de editar | Funcional | Alta | - |
| TC-UI-023 | Botones de crear | Funcional | Alta | - |
| TC-UI-024 | Botón Atrás/Volver | Funcional | Media | - |
| TC-UI-025 | Mensaje de éxito se muestra | UI | Alta | - |
| TC-UI-026 | Mensaje de error se muestra | UI | Alta | - |
| TC-UI-027 | Mensajes de validación | UI | Alta | - |
| TC-UI-028 | Formularios se muestran correctamente | UI | Alta | - |
| TC-UI-029 | Campos requeridos marcados | UI | Media | - |
| TC-UI-030 | Botones de formulario funcionan | Funcional | Alta | - |
| TC-UI-031 | Botón Cancelar funciona | Funcional | Media | - |
| TC-UI-032 | Iconos accesibles | Accesibilidad | Media | - |
| TC-UI-033 | Enlaces descriptivos | Accesibilidad | Media | - |
| TC-UI-034 | Formularios con labels | Accesibilidad | Alta | - |
| TC-UI-035 | Brand logo/texto se muestra | UI | Media | - |
| TC-UI-036 | Brand link navega a home | Funcional | Media | - |

**Total de casos:** 36  
**Críticos:** 4  
**Altos:** 20  
**Medios:** 12

