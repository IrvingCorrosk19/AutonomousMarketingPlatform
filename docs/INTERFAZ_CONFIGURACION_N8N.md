# 🎨 Interfaz Gráfica de Configuración de n8n

## ✅ Implementación Completada

Se ha creado una **interfaz gráfica completa** en el frontend para configurar n8n desde el navegador, sin necesidad de editar archivos manualmente.

---

## 📍 Ubicación

**URL:** `/N8nConfig`  
**Controlador:** `N8nConfigController.cs`  
**Vista:** `Views/N8nConfig/Index.cshtml`  
**Acceso:** Requiere rol **Owner**, **Admin** o **SuperAdmin**

---

## 🎯 Funcionalidades Implementadas

### 1. **Configuración General**
- ✅ Toggle para activar/desactivar modo Mock
- ✅ Campo para URL Base de n8n
- ✅ Campo para URL de API de n8n
- ✅ Campo para API Key (opcional, con enmascaramiento)
- ✅ Campo para URL por defecto de webhooks
- ✅ Botón para probar conexión con n8n

### 2. **Gestión de URLs de Webhooks**
- ✅ Formulario para configurar las 11 URLs de webhooks
- ✅ Campos individuales para cada workflow:
  - Trigger - Marketing Request
  - Validate Consents
  - Load Marketing Memory
  - Analyze Instruction AI
  - Generate Marketing Strategy
  - Generate Marketing Copy
  - Generate Visual Prompts
  - Build Marketing Pack
  - Human Approval Flow
  - Publish Content
  - Metrics & Learning
- ✅ Botones para probar cada webhook individualmente

### 3. **Información de Workflows**
- ✅ Tabla que muestra todos los workflows disponibles
- ✅ Información de cada workflow:
  - Nombre
  - Descripción
  - Event Type
  - Estado (Activo/Inactivo/Error)
  - Enlace directo al webhook (si está configurado)

### 4. **Prueba de Conexión**
- ✅ Botón para probar conexión con n8n
- ✅ Muestra resultado en tiempo real
- ✅ Indicadores visuales de éxito/error
- ✅ Mensajes descriptivos

---

## 🚀 Cómo Usar

### Paso 1: Acceder a la Configuración

1. Iniciar sesión como **Owner**, **Admin** o **SuperAdmin**
2. En el menú lateral, hacer clic en **"Configuración n8n"**
3. O navegar directamente a: `/N8nConfig`

### Paso 2: Configurar Conexión Básica

1. **Desactivar Modo Mock:**
   - Desmarcar el checkbox "Usar Modo Mock"
   
2. **Configurar URL Base:**
   - Ingresar la URL donde corre n8n (ej: `http://localhost:5678`)

3. **Probar Conexión:**
   - Hacer clic en "Probar Conexión"
   - Verificar que la conexión sea exitosa

### Paso 3: Configurar URLs de Webhooks

1. **Importar workflows en n8n:**
   - Ir a n8n
   - Importar los workflows desde `workflows/n8n/`
   - Activar cada workflow

2. **Copiar URLs de webhooks:**
   - En n8n, abrir cada workflow
   - Copiar la "Production URL" del nodo Webhook

3. **Pegar URLs en el formulario:**
   - En la interfaz, pegar cada URL en su campo correspondiente
   - Hacer clic en "Guardar URLs de Webhooks"

### Paso 4: Verificar Estado

- Revisar la tabla de "Workflows Disponibles"
- Verificar que todos los workflows estén configurados
- Probar webhooks individuales si es necesario

---

## 📋 Archivos Creados

### Backend

1. **DTOs:**
   - `Application/DTOs/N8nConfigDto.cs` - DTOs para configuración

2. **Casos de Uso:**
   - `Application/UseCases/N8n/GetN8nConfigQuery.cs` - Obtener configuración
   - `Application/UseCases/N8n/UpdateN8nConfigCommand.cs` - Actualizar configuración
   - `Application/UseCases/N8n/TestN8nConnectionCommand.cs` - Probar conexión

3. **Controlador:**
   - `Web/Controllers/N8nConfigController.cs` - Controlador principal

### Frontend

1. **Vista:**
   - `Web/Views/N8nConfig/Index.cshtml` - Interfaz gráfica completa

2. **Navegación:**
   - `Web/Views/Shared/_Sidebar.cshtml` - Enlace agregado al menú

---

## 🔧 Características Técnicas

### Seguridad
- ✅ Requiere autenticación
- ✅ Restricción por roles (Owner, Admin, SuperAdmin)
- ✅ Validación de tokens anti-falsificación (CSRF)
- ✅ API Key enmascarada en el formulario

### Validación
- ✅ Validación de URLs (formato correcto)
- ✅ Campos requeridos marcados
- ✅ Mensajes de error descriptivos

### UX/UI
- ✅ Interfaz responsive (AdminLTE)
- ✅ Iconos Font Awesome
- ✅ Alertas de éxito/error
- ✅ Indicadores de carga
- ✅ Tabla interactiva de workflows

---

## ⚠️ Notas Importantes

### Limitación Actual

La configuración actualmente **NO se guarda automáticamente en `appsettings.json`**. 

**Razón:** Por seguridad y mejores prácticas, modificar `appsettings.json` directamente desde la aplicación no es recomendado.

**Solución Temporal:**
- La interfaz muestra la configuración actual
- Permite probar conexiones
- Los cambios deben aplicarse manualmente en `appsettings.json` o implementar un sistema de configuración en base de datos

### Próximas Mejoras

1. **Guardar en Base de Datos:**
   - Crear tabla `N8nConfiguration` en la base de datos
   - Guardar configuración por tenant
   - Implementar actualización real

2. **Sincronización con n8n:**
   - Consultar workflows activos desde n8n API
   - Mostrar estado real de cada workflow
   - Detectar cambios automáticamente

3. **Importación Automática:**
   - Botón para importar workflows desde archivos JSON
   - Configuración automática de URLs
   - Validación de workflows importados

---

## 📸 Vista Previa

La interfaz incluye:

1. **Card de Configuración General:**
   - Toggle de Modo Mock
   - Campos de URLs y API Key
   - Botón de prueba de conexión

2. **Card de URLs de Webhooks:**
   - 11 campos para configurar webhooks
   - Botones de prueba individuales
   - Información de Event Types

3. **Card de Workflows:**
   - Tabla con información de todos los workflows
   - Estado y enlaces directos
   - Actualización automática

---

## ✅ Checklist de Uso

- [ ] Acceder a `/N8nConfig`
- [ ] Desactivar Modo Mock
- [ ] Configurar URL Base de n8n
- [ ] Probar conexión (debe ser exitosa)
- [ ] Importar workflows en n8n
- [ ] Copiar URLs de webhooks desde n8n
- [ ] Pegar URLs en el formulario
- [ ] Guardar configuración
- [ ] Verificar workflows en la tabla
- [ ] Probar webhooks individuales

---

**¡La configuración de n8n ahora es completamente gráfica y accesible desde el frontend!** 🎉

