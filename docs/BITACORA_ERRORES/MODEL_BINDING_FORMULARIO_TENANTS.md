# Bitácora de Errores - Model Binding Vacío en Formulario Tenants

**Fecha:** 5 de Enero, 2025  
**Módulo:** Tenants - Crear Tenant  
**Severidad:** Alta  
**Estado:** ✅ Resuelto

---

## Resumen del Problema

El formulario de creación de Tenants no estaba enviando datos al controlador. El modelo `CreateTenantDto` llegaba completamente vacío (todas las propiedades en `null` o valores por defecto), a pesar de que:

- El HTML generado era correcto
- Los inputs tenían los atributos `name` correctos
- El formulario tenía `method="post"` y `action` correctos
- El Content-Type era `application/x-www-form-urlencoded`
- El controlador tenía `[FromForm]` explícito

**Síntoma Principal:**
```
[TenantsController.Create] Form.Count: 0
[TenantsController.Create] Model.Name: NULL
[TenantsController.Create] Model.Subdomain: NULL
[TenantsController.Create] Model.ContactEmail: NULL
```

---

## Investigación y Diagnóstico

### 1. Verificación del HTML Generado

El HTML generado por los Tag Helpers era **completamente correcto**:

```html
<form method="post" action="/Tenants/Create">
    <input name="Name" id="Name" type="text" value="" ... />
    <input name="Subdomain" id="Subdomain" type="text" value="" ... />
    <input name="ContactEmail" id="ContactEmail" type="email" value="" ... />
    <button type="submit">Crear Tenant</button>
</form>
```

✅ Los atributos `name` coincidían exactamente con las propiedades del DTO  
✅ El formulario tenía `method="post"`  
✅ El `action` apuntaba al controlador correcto

### 2. Verificación del Controlador

El controlador estaba configurado correctamente:

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromForm] CreateTenantDto model)
{
    // Logging extensivo mostraba:
    // - ContentType: application/x-www-form-urlencoded ✅
    // - HasFormContentType: True ✅
    // - Form.Count: 0 ❌ (PROBLEMA)
}
```

✅ El atributo `[FromForm]` estaba presente  
✅ El Content-Type era correcto  
❌ Pero `Form.Count: 0` indicaba que no había datos

### 3. Verificación del DTO

El DTO estaba correctamente definido:

```csharp
public class CreateTenantDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Subdomain { get; set; }
    
    public string? ContactEmail { get; set; }
}
```

✅ Las propiedades coincidían con los nombres de los inputs  
✅ No había problemas de mapeo

### 4. Descubrimiento del Problema

Al revisar el código JavaScript global (`site.js`), se encontró el problema:

**Archivo:** `src/AutonomousMarketingPlatform.Web/wwwroot/js/site.js`

**Función problemática:**
```javascript
function showFormLoading($form, message) {
    // ...
    
    // ❌ ESTA LÍNEA CAUSABA EL PROBLEMA
    $form.find('input, select, textarea, button').prop('disabled', true);
    
    // ...
}
```

**Event listener que activaba la función:**
```javascript
$('form').on('submit', function(e) {
    var $form = $(this);
    
    if ($form[0].checkValidity()) {
        showFormLoading($form, 'Guardando...'); // ← Se ejecutaba ANTES del envío
    }
});
```

---

## Causa Raíz

### El Problema

Cuando un campo de formulario HTML tiene el atributo `disabled`, **NO se incluye en el envío del formulario**. Esto es un comportamiento estándar de HTML.

**Secuencia de eventos que causaba el problema:**

1. Usuario llena el formulario con datos
2. Usuario hace clic en "Crear Tenant" (submit)
3. Se dispara el event listener `$('form').on('submit')`
4. Se ejecuta `showFormLoading()` que **deshabilita TODOS los campos**:
   ```javascript
   $form.find('input, select, textarea, button').prop('disabled', true);
   ```
5. El formulario se envía, pero como todos los campos están `disabled`, **ningún dato se incluye en el POST**
6. El servidor recibe `Form.Count: 0` y el modelo llega vacío

### Por Qué No Era Obvio

- El HTML generado era correcto
- El formulario se enviaba (no había errores de JavaScript)
- El Content-Type era correcto
- El problema solo se manifestaba en el servidor (Form.Count: 0)
- No había errores visibles en la consola del navegador

---

## Solución Aplicada

### Cambio 1: Modificar `showFormLoading()`

**Antes:**
```javascript
function showFormLoading($form, message) {
    // ...
    
    // ❌ Deshabilitaba TODOS los campos
    $form.find('input, select, textarea, button').prop('disabled', true);
    
    // ...
}
```

**Después:**
```javascript
function showFormLoading($form, message) {
    // ...
    
    var $submitBtn = $form.find('button[type="submit"], input[type="submit"]');
    if ($submitBtn.length > 0) {
        showButtonLoading($submitBtn, '<i class="fas fa-spinner fa-spin mr-1"></i>' + message);
    }
    
    // ✅ NO deshabilitar los campos del formulario porque no se enviarían
    // ✅ Solo deshabilitar el botón de submit para evitar doble envío
    // ✅ Los campos se mantienen habilitados para que se envíen correctamente
    
    // Agregar overlay visual solamente
    if ($form.find('.form-loading-overlay').length === 0) {
        $form.css('position', 'relative');
        $form.append(`
            <div class="form-loading-overlay">
                <div class="form-loading-spinner">
                    <div class="spinner-border text-primary" role="status">
                        <span class="sr-only">Cargando...</span>
                    </div>
                </div>
            </div>
        `);
    }
}
```

### Cambio 2: Actualizar `hideFormLoading()`

**Antes:**
```javascript
function hideFormLoading($form) {
    // ...
    
    // Habilitar todos los campos del formulario
    $form.find('input, select, textarea, button').prop('disabled', false);
    
    // ...
}
```

**Después:**
```javascript
function hideFormLoading($form) {
    // ...
    
    var $submitBtn = $form.find('button[type="submit"], input[type="submit"]');
    if ($submitBtn.length > 0) {
        hideButtonLoading($submitBtn);
    }
    
    // ✅ Los campos ya estaban habilitados, solo habilitar el botón si estaba deshabilitado
    // Remover overlay del formulario
    $form.find('.form-loading-overlay').remove();
}
```

### Cambio 3: Agregar comentario en el event listener

```javascript
$('form').on('submit', function(e) {
    var $form = $(this);
    
    // Solo aplicar loading si el formulario es válido
    // ✅ NO usar preventDefault() - dejar que el formulario se envíe normalmente
    if ($form[0].checkValidity()) {
        // Aplicar loading visual pero NO bloquear el envío
        showFormLoading($form, 'Guardando...');
        // El formulario se enviará normalmente con todos los datos
    }
});
```

---

## Archivos Modificados

1. **`src/AutonomousMarketingPlatform.Web/wwwroot/js/site.js`**
   - Función `showFormLoading()`: Removida la línea que deshabilitaba campos
   - Función `hideFormLoading()`: Simplificada (ya no necesita habilitar campos)
   - Event listener de formularios: Agregado comentario explicativo

---

## Verificación de la Solución

### Antes de la Corrección

```
[REQUEST] POST /Tenants/Create
[TenantsController.Create] ContentType: application/x-www-form-urlencoded
[TenantsController.Create] HasFormContentType: True
[TenantsController.Create] Form.Count: 0 ❌
[TenantsController.Create] Model.Name: NULL ❌
[TenantsController.Create] Model.Subdomain: NULL ❌
[TenantsController.Create] Model.ContactEmail: NULL ❌
```

### Después de la Corrección (Esperado)

```
[REQUEST] POST /Tenants/Create
[TenantsController.Create] ContentType: application/x-www-form-urlencoded
[TenantsController.Create] HasFormContentType: True
[TenantsController.Create] Form.Count: 3 ✅
[TenantsController.Create] Form[Name] = "Empresa ABC" ✅
[TenantsController.Create] Form[Subdomain] = "empresa-abc" ✅
[TenantsController.Create] Form[ContactEmail] = "contacto@empresa.com" ✅
[TenantsController.Create] Model.Name: "Empresa ABC" ✅
[TenantsController.Create] Model.Subdomain: "empresa-abc" ✅
[TenantsController.Create] Model.ContactEmail: "contacto@empresa.com" ✅
```

---

## Lecciones Aprendidas

### 1. Campos `disabled` No Se Envían

**Regla importante:** En HTML, los campos con atributo `disabled` **NO se incluyen** en el envío del formulario. Esto es un comportamiento estándar y esperado.

**Solución:** Si necesitas prevenir la edición durante el envío, usa `readonly` en lugar de `disabled`, o simplemente no deshabilites los campos.

### 2. JavaScript Puede Interferir con Formularios

Los event listeners de JavaScript que se ejecutan en el evento `submit` pueden interferir con el envío normal del formulario si:

- Usan `preventDefault()` sin enviar los datos manualmente
- Deshabilitan campos antes del envío
- Modifican el DOM de manera que afecte la serialización del formulario

**Solución:** Si necesitas mostrar loading, hazlo de forma visual (overlay, spinner) sin deshabilitar los campos.

### 3. Debugging de Model Binding

Cuando el model binding falla:

1. ✅ Verificar que el HTML tenga los atributos `name` correctos
2. ✅ Verificar que el formulario tenga `method="post"`
3. ✅ Verificar que el Content-Type sea correcto
4. ✅ **Verificar que los campos NO estén `disabled`**
5. ✅ Verificar que no haya JavaScript bloqueando el envío
6. ✅ Revisar los logs del servidor para `Form.Count`

### 4. Separación de Responsabilidades

El código de loading visual no debería interferir con la funcionalidad del formulario. Separar:

- **Loading visual:** Overlay, spinner, deshabilitar botón
- **Funcionalidad del formulario:** Mantener campos habilitados para envío

---

## Prevención Futura

### Checklist para Nuevos Formularios

- [ ] Verificar que los Tag Helpers generen `name` correctos
- [ ] Verificar que el formulario tenga `method="post"`
- [ ] Verificar que el controlador tenga `[FromForm]` si es necesario
- [ ] **Verificar que ningún JavaScript deshabilite campos antes del submit**
- [ ] Probar el envío y verificar `Form.Count > 0` en logs
- [ ] Verificar que el modelo llegue poblado al controlador

### Código de Ejemplo Seguro

```javascript
// ✅ CORRECTO: Solo deshabilitar el botón, no los campos
$('form').on('submit', function(e) {
    var $form = $(this);
    var $submitBtn = $form.find('button[type="submit"]');
    
    // Deshabilitar solo el botón para evitar doble envío
    $submitBtn.prop('disabled', true);
    
    // Mostrar loading visual
    showLoadingOverlay($form);
    
    // NO usar preventDefault() - dejar que el formulario se envíe
    // NO deshabilitar los campos del formulario
});

// ❌ INCORRECTO: Deshabilitar todos los campos
$('form').on('submit', function(e) {
    var $form = $(this);
    
    // ❌ Esto previene que los datos se envíen
    $form.find('input, select, textarea').prop('disabled', true);
});
```

---

## Referencias

- [HTML5 Spec - Disabled Form Controls](https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#attr-fe-disabled)
- [MDN - disabled attribute](https://developer.mozilla.org/en-US/docs/Web/HTML/Attributes/disabled)
- [ASP.NET Core Model Binding](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding)

---

## Notas Adicionales

- Este error afectaba a **todos los formularios** de la aplicación, no solo al de Tenants
- La solución aplicada corrige el problema globalmente
- No se requirieron cambios en las vistas, solo en el JavaScript global
- El problema no era visible en la consola del navegador, solo se manifestaba en el servidor

---

**Resuelto por:** Auto (AI Assistant)  
**Revisado por:** Usuario  
**Fecha de Resolución:** 5 de Enero, 2025

