# Casos de Prueba - Responsive Design

**Versión:** 1.0  
**Fecha:** 2024-12-19  
**Sistema:** Autonomous Marketing Platform  
**Módulo:** Diseño Responsive

---

## Índice

- [Vista Desktop](#vista-desktop)
- [Vista Tablet](#vista-tablet)
- [Vista Mobile](#vista-mobile)
- [Sidebar Responsive](#sidebar-responsive)
- [Navbar Responsive](#navbar-responsive)
- [Tablas Responsive](#tablas-responsive)
- [Formularios Responsive](#formularios-responsive)
- [Cards y Contenedores](#cards-y-contenedores)

---

## Vista Desktop

### TC-RES-001: Aplicación se muestra correctamente en desktop (1920x1080)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Navegador en resolución 1920x1080

**Pasos:**
1. Acceder a aplicación en desktop
2. Verificar layout y elementos

**Resultado Esperado:**
- Layout se muestra correctamente
- Sidebar visible en lado izquierdo
- Contenido principal ocupa espacio restante
- Navbar visible en parte superior
- Todos los elementos son legibles y accesibles
- No hay elementos cortados o superpuestos

---

### TC-RES-002: Aplicación se muestra correctamente en desktop (1366x768)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado
- Navegador en resolución 1366x768

**Pasos:**
1. Acceder a aplicación
2. Verificar layout

**Resultado Esperado:**
- Layout se adapta correctamente
- Elementos se muestran sin problemas
- Sidebar y contenido se ajustan
- No hay scroll horizontal innecesario

---

### TC-RES-003: Aplicación se muestra correctamente en desktop (1280x720)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado
- Navegador en resolución 1280x720

**Pasos:**
1. Acceder a aplicación
2. Verificar layout

**Resultado Esperado:**
- Layout funciona correctamente
- Elementos se ajustan apropiadamente
- Contenido es legible

---

## Vista Tablet

### TC-RES-004: Aplicación se muestra correctamente en tablet (768x1024)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está autenticado
- Navegador en resolución tablet (768x1024)

**Pasos:**
1. Acceder a aplicación en tablet
2. Verificar layout y funcionalidad

**Resultado Esperado:**
- Layout se adapta a pantalla tablet
- Sidebar puede colapsarse automáticamente o ser accesible
- Contenido se ajusta correctamente
- Elementos son táctiles y accesibles
- No hay elementos cortados

---

### TC-RES-005: Sidebar en tablet se comporta correctamente

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en tablet
- Sidebar está presente

**Pasos:**
1. Verificar comportamiento del sidebar en tablet
2. Probar colapsar/expandir

**Resultado Esperado:**
- Sidebar se puede colapsar/expandir
- Menú sigue siendo accesible
- Navegación funciona correctamente
- Botón de menú (hamburger) está visible

---

### TC-RES-006: Formularios en tablet son usables

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en tablet
- Formulario está visible

**Pasos:**
1. Acceder a formulario en tablet
2. Verificar usabilidad

**Resultado Esperado:**
- Campos son lo suficientemente grandes para tocar
- Formularios se ajustan al ancho disponible
- Botones son accesibles
- No hay problemas de usabilidad

---

## Vista Mobile

### TC-RES-007: Aplicación se muestra correctamente en mobile (375x667 - iPhone)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Navegador en resolución mobile (375x667)

**Pasos:**
1. Acceder a aplicación en mobile
2. Verificar layout y funcionalidad

**Resultado Esperado:**
- Layout se adapta completamente a mobile
- Sidebar se colapsa automáticamente o se oculta
- Contenido ocupa ancho completo
- Elementos son táctiles y accesibles
- Texto es legible sin zoom
- No hay scroll horizontal

---

### TC-RES-008: Aplicación se muestra correctamente en mobile (360x640 - Android)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está autenticado
- Navegador en resolución mobile (360x640)

**Pasos:**
1. Acceder a aplicación
2. Verificar layout

**Resultado Esperado:**
- Layout funciona correctamente
- Adaptación a Android funciona
- Elementos se muestran correctamente

---

### TC-RES-009: Sidebar en mobile se colapsa automáticamente

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está en mobile
- Sidebar está presente

**Pasos:**
1. Acceder a aplicación en mobile
2. Verificar estado del sidebar

**Resultado Esperado:**
- Sidebar se colapsa automáticamente en mobile
- Botón de menú (hamburger) está visible
- Al hacer clic, sidebar se expande (overlay o slide)
- Navegación funciona correctamente

---

### TC-RES-010: Navbar en mobile se adapta correctamente

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile

**Pasos:**
1. Verificar navbar en mobile
2. Probar elementos del navbar

**Resultado Esperado:**
- Navbar se ajusta al ancho de pantalla
- Elementos se reorganizan si es necesario
- Dropdowns funcionan correctamente
- Información de tenant puede ocultarse (d-none d-sm-inline-block)
- Botón de menú está accesible

---

### TC-RES-011: Formularios en mobile son usables

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está en mobile
- Formulario está visible

**Pasos:**
1. Acceder a formulario en mobile
2. Probar completar formulario

**Resultado Esperado:**
- Campos ocupan ancho completo o apropiado
- Campos son lo suficientemente grandes para tocar (mínimo 44x44px)
- Labels y campos están bien espaciados
- Botones son accesibles y táctiles
- Teclado virtual no cubre campos importantes
- Formularios son completamente funcionales

---

### TC-RES-012: Tablas en mobile se adaptan o hacen scroll

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile
- Lista con tabla está visible

**Pasos:**
1. Acceder a lista con tabla en mobile
2. Verificar presentación

**Resultado Esperado:**
- Tablas se adaptan al ancho disponible
- O tablas tienen scroll horizontal si es necesario
- O tablas se convierten en cards/listas en mobile
- Contenido es legible y accesible
- No hay elementos cortados

---

### TC-RES-013: Cards en mobile se apilan verticalmente

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile
- Vista con cards está visible

**Pasos:**
1. Acceder a vista con cards (ej: dashboard)
2. Verificar presentación

**Resultado Esperado:**
- Cards se apilan verticalmente
- Cards ocupan ancho completo o apropiado
- Espaciado entre cards es adecuado
- Contenido es legible

---

### TC-RES-014: Botones en mobile son táctiles

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está en mobile

**Pasos:**
1. Verificar botones en mobile
2. Probar hacer clic/tocar

**Resultado Esperado:**
- Botones tienen tamaño mínimo táctil (44x44px recomendado)
- Botones tienen espaciado adecuado
- Botones son fáciles de tocar
- No hay botones muy pequeños o muy juntos

---

## Sidebar Responsive

### TC-RES-015: Sidebar se oculta en pantallas pequeñas

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en pantalla pequeña (< 768px)

**Pasos:**
1. Acceder a aplicación en pantalla pequeña
2. Verificar sidebar

**Resultado Esperado:**
- Sidebar se oculta automáticamente
- Botón de menú está visible para expandir
- Contenido principal ocupa ancho completo

---

### TC-RES-016: Sidebar overlay en mobile funciona

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile
- Sidebar está colapsado

**Pasos:**
1. Hacer clic en botón de menú
2. Verificar que sidebar se expande como overlay

**Resultado Esperado:**
- Sidebar se expande como overlay sobre contenido
- Contenido de fondo se oscurece o sidebar tiene sombra
- Sidebar se puede cerrar haciendo clic fuera o en botón
- Navegación funciona correctamente

---

## Navbar Responsive

### TC-RES-017: Información de tenant se oculta en mobile

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Usuario está en mobile
- Navbar muestra información de tenant

**Pasos:**
1. Verificar navbar en mobile
2. Buscar información de tenant

**Resultado Esperado:**
- Información de tenant se oculta en pantallas pequeñas
- Clase `d-none d-sm-inline-block` funciona correctamente
- Se muestra solo en pantallas más grandes

---

### TC-RES-018: Dropdowns en navbar funcionan en mobile

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile

**Pasos:**
1. Hacer clic en dropdown de notificaciones
2. Hacer clic en dropdown de usuario
3. Verificar funcionamiento

**Resultado Esperado:**
- Dropdowns se abren correctamente
- Dropdowns se posicionan apropiadamente (dropdown-menu-right)
- Dropdowns son táctiles y accesibles
- Dropdowns se cierran al hacer clic fuera

---

## Tablas Responsive

### TC-RES-019: Tablas tienen scroll horizontal en mobile si es necesario

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile
- Tabla con muchas columnas está visible

**Pasos:**
1. Acceder a tabla en mobile
2. Verificar scroll horizontal

**Resultado Esperado:**
- Tabla permite scroll horizontal si es necesario
- Headers se mantienen visibles si es posible
- Scroll funciona correctamente
- Contenido es accesible

---

### TC-RES-020: Tablas se convierten en cards en mobile (si aplica)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Sistema tiene lógica para convertir tablas a cards en mobile
- Usuario está en mobile

**Pasos:**
1. Acceder a tabla en mobile
2. Verificar presentación

**Resultado Esperado:**
- Si está implementado, tablas se convierten en cards
- Cada fila se muestra como card
- Información es legible
- Cards son táctiles

---

## Formularios Responsive

### TC-RES-021: Campos de formulario ocupan ancho apropiado en mobile

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Crítica

**Precondiciones:**
- Usuario está en mobile
- Formulario está visible

**Pasos:**
1. Acceder a formulario en mobile
2. Verificar campos

**Resultado Esperado:**
- Campos ocupan ancho completo o apropiado
- Campos no son demasiado estrechos
- Labels y campos están bien organizados
- Formulario es completamente funcional

---

### TC-RES-022: Botones de formulario en mobile son accesibles

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en mobile
- Formulario tiene botones

**Pasos:**
1. Verificar botones en formulario mobile
2. Probar hacer clic

**Resultado Esperado:**
- Botones tienen tamaño táctil adecuado
- Botones están bien espaciados
- Botones son fáciles de tocar
- Botones no están cortados o superpuestos

---

## Cards y Contenedores

### TC-RES-023: Cards se adaptan al ancho disponible

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Vista con cards está visible

**Pasos:**
1. Cambiar tamaño de ventana
2. Verificar que cards se adaptan

**Resultado Esperado:**
- Cards se ajustan al ancho disponible
- En desktop: múltiples columnas
- En tablet: 2 columnas o menos
- En mobile: 1 columna (apiladas)
- Grid system funciona correctamente

---

### TC-RES-024: Contenedores principales se ajustan

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Usuario está en diferentes resoluciones

**Pasos:**
1. Cambiar tamaño de ventana
2. Verificar contenedores

**Resultado Esperado:**
- Contenedores principales se ajustan al ancho
- Padding y márgenes se adaptan
- No hay overflow horizontal
- Contenido se mantiene legible

---

## Zoom y Escalado

### TC-RES-025: Aplicación funciona con zoom del navegador (125%)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Navegador tiene zoom al 125%

**Pasos:**
1. Configurar zoom al 125%
2. Acceder a aplicación
3. Verificar funcionalidad

**Resultado Esperado:**
- Aplicación funciona correctamente con zoom
- Elementos se escalan apropiadamente
- No hay elementos cortados o superpuestos
- Funcionalidad se mantiene

---

### TC-RES-026: Aplicación funciona con zoom del navegador (150%)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Navegador tiene zoom al 150%

**Pasos:**
1. Configurar zoom al 150%
2. Acceder a aplicación
3. Verificar funcionalidad

**Resultado Esperado:**
- Aplicación funciona con zoom alto
- Elementos siguen siendo accesibles
- Scroll funciona si es necesario
- Funcionalidad se mantiene

---

## Orientación

### TC-RES-027: Aplicación funciona en orientación landscape (tablet)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Dispositivo tablet en orientación landscape

**Pasos:**
1. Rotar tablet a landscape
2. Verificar layout

**Resultado Esperado:**
- Layout se adapta a orientación landscape
- Elementos se reorganizan apropiadamente
- Contenido es legible y accesible
- Funcionalidad se mantiene

---

### TC-RES-028: Aplicación funciona en orientación portrait (mobile)

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Media

**Precondiciones:**
- Dispositivo mobile en orientación portrait

**Pasos:**
1. Verificar aplicación en portrait
2. Probar funcionalidad

**Resultado Esperado:**
- Layout funciona en portrait
- Elementos se muestran correctamente
- Navegación funciona
- Formularios son usables

---

## Breakpoints

### TC-RES-029: Breakpoint 768px funciona correctamente

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Navegador cerca de breakpoint 768px

**Pasos:**
1. Cambiar ancho de ventana alrededor de 768px
2. Verificar cambios de layout

**Resultado Esperado:**
- Layout cambia apropiadamente en breakpoint
- Sidebar se comporta correctamente
- Elementos se reorganizan
- No hay saltos o problemas visuales

---

### TC-RES-030: Breakpoint 576px funciona correctamente

**Módulo:** Responsive Design  
**Tipo:** UI  
**Prioridad:** Alta

**Precondiciones:**
- Navegador cerca de breakpoint 576px

**Pasos:**
1. Cambiar ancho de ventana alrededor de 576px
2. Verificar cambios de layout

**Resultado Esperado:**
- Layout cambia apropiadamente
- Elementos se ajustan para mobile
- Contenido es accesible
- No hay problemas visuales

---

## Resumen de Casos

| ID | Nombre | Tipo | Prioridad | Estado |
|---|---|---|---|---|
| TC-RES-001 | Desktop 1920x1080 | UI | Crítica | - |
| TC-RES-002 | Desktop 1366x768 | UI | Alta | - |
| TC-RES-003 | Desktop 1280x720 | UI | Alta | - |
| TC-RES-004 | Tablet 768x1024 | UI | Alta | - |
| TC-RES-005 | Sidebar en tablet | UI | Alta | - |
| TC-RES-006 | Formularios en tablet | UI | Alta | - |
| TC-RES-007 | Mobile 375x667 iPhone | UI | Crítica | - |
| TC-RES-008 | Mobile 360x640 Android | UI | Crítica | - |
| TC-RES-009 | Sidebar colapsa en mobile | UI | Crítica | - |
| TC-RES-010 | Navbar en mobile | UI | Alta | - |
| TC-RES-011 | Formularios en mobile | UI | Crítica | - |
| TC-RES-012 | Tablas en mobile | UI | Alta | - |
| TC-RES-013 | Cards apiladas en mobile | UI | Alta | - |
| TC-RES-014 | Botones táctiles en mobile | UI | Crítica | - |
| TC-RES-015 | Sidebar oculta en pantallas pequeñas | UI | Alta | - |
| TC-RES-016 | Sidebar overlay en mobile | UI | Alta | - |
| TC-RES-017 | Info tenant oculta en mobile | UI | Media | - |
| TC-RES-018 | Dropdowns en navbar mobile | UI | Alta | - |
| TC-RES-019 | Tablas scroll horizontal mobile | UI | Alta | - |
| TC-RES-020 | Tablas a cards en mobile | UI | Media | - |
| TC-RES-021 | Campos formulario mobile | UI | Crítica | - |
| TC-RES-022 | Botones formulario mobile | UI | Alta | - |
| TC-RES-023 | Cards se adaptan al ancho | UI | Alta | - |
| TC-RES-024 | Contenedores se ajustan | UI | Alta | - |
| TC-RES-025 | Zoom 125% | UI | Media | - |
| TC-RES-026 | Zoom 150% | UI | Media | - |
| TC-RES-027 | Orientación landscape tablet | UI | Media | - |
| TC-RES-028 | Orientación portrait mobile | UI | Media | - |
| TC-RES-029 | Breakpoint 768px | UI | Alta | - |
| TC-RES-030 | Breakpoint 576px | UI | Alta | - |

**Total de casos:** 30  
**Críticos:** 6  
**Altos:** 18  
**Medios:** 6

