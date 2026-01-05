# Datos que se Envían en el Formulario de Crear Usuario

## Formulario HTML

```html
<form asp-action="Create" asp-controller="Users" method="post" id="createUserForm">
```

## Datos que se Envían en el POST

Cuando el usuario hace clic en "Crear Usuario", el navegador envía un POST con los siguientes datos:

### 1. Email
- **Campo HTML**: `<input asp-for="Email" name="Email" ... />`
- **Nombre en POST**: `Email`
- **Tipo**: `string`
- **Valor ejemplo**: `test@example.com`
- **Validación**: Required, EmailAddress

### 2. Password
- **Campo HTML**: `<input asp-for="Password" name="Password" type="password" ... />`
- **Nombre en POST**: `Password`
- **Tipo**: `string`
- **Valor ejemplo**: `Test123!@#`
- **Validación**: Required, MinLength(8), Regex (mayúsculas, minúsculas, números, especiales)

### 3. ConfirmPassword
- **Campo HTML**: `<input asp-for="ConfirmPassword" name="ConfirmPassword" type="password" ... />`
- **Nombre en POST**: `ConfirmPassword`
- **Tipo**: `string`
- **Valor ejemplo**: `Test123!@#`
- **Validación**: Required, Compare("Password")

### 4. FullName
- **Campo HTML**: `<input asp-for="FullName" name="FullName" ... />`
- **Nombre en POST**: `FullName`
- **Tipo**: `string`
- **Valor ejemplo**: `Test User`
- **Validación**: Required, MaxLength(200)

### 5. TenantId
- **Campo HTML**: 
  - Si es SuperAdmin: `<select asp-for="TenantId" name="TenantId" ... />`
  - Si NO es SuperAdmin: `<input type="hidden" asp-for="TenantId" name="TenantId" value="..." />`
- **Nombre en POST**: `TenantId`
- **Tipo**: `Guid` (se envía como string, ASP.NET Core lo convierte)
- **Valor ejemplo**: `12345678-1234-1234-1234-123456789abc`
- **Validación**: NO Required (se asigna en el controlador)

### 6. Role
- **Campo HTML**: `<select asp-for="Role" name="Role" ... />`
- **Nombre en POST**: `Role`
- **Tipo**: `string`
- **Valores posibles**: `"Marketer"`, `"Admin"`, `"Owner"`, `"Viewer"`
- **Valor por defecto**: `"Marketer"`
- **Validación**: Required

### 7. IsActive
- **Campo HTML**: `<input asp-for="IsActive" name="IsActive" type="checkbox" ... />`
- **Nombre en POST**: `IsActive`
- **Tipo**: `bool`
- **Valor si está marcado**: `"true"` (string)
- **Valor si NO está marcado**: No se envía (ASP.NET Core asigna `false` por defecto)
- **Valor por defecto en DTO**: `true`
- **Validación**: Ninguna

## Ejemplo de POST Request

### Headers
```
POST /Users/Create HTTP/1.1
Host: localhost:56609
Content-Type: application/x-www-form-urlencoded
Content-Length: 234
Cookie: .AspNetCore.Identity.Application=...
```

### Body (URL Encoded)
```
Email=test@example.com&Password=Test123!@#&ConfirmPassword=Test123!@#&FullName=Test+User&TenantId=12345678-1234-1234-1234-123456789abc&Role=Marketer&IsActive=true
```

### Body (Formato Legible)
```
Email=test@example.com
Password=Test123!@#
ConfirmPassword=Test123!@#
FullName=Test User
TenantId=12345678-1234-1234-1234-123456789abc
Role=Marketer
IsActive=true
```

## Mapeo al DTO

El `[FromForm]` attribute en el controlador hace que ASP.NET Core mapee automáticamente:

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromForm] CreateUserDto model)
{
    // model.Email = "test@example.com"
    // model.Password = "Test123!@#"
    // model.ConfirmPassword = "Test123!@#"
    // model.FullName = "Test User"
    // model.TenantId = Guid.Parse("12345678-1234-1234-1234-123456789abc")
    // model.Role = "Marketer"
    // model.IsActive = true
}
```

## Casos Especiales

### 1. Checkbox IsActive
- **Si está marcado**: Se envía `IsActive=true` → `model.IsActive = true`
- **Si NO está marcado**: No se envía → `model.IsActive = false` (valor por defecto del DTO es `true`, pero si no se envía, el model binder asigna `false`)

**NOTA**: Hay un problema aquí. Si el checkbox no está marcado, no se envía nada, y el model binder podría asignar `false` en lugar del valor por defecto `true`. Esto podría necesitar un ajuste.

### 2. TenantId Hidden
- **Si NO es SuperAdmin**: Se envía como hidden input con el valor del usuario actual
- **Si es SuperAdmin**: Se envía desde el select

### 3. Role Select
- Si no se selecciona nada (valor vacío), se envía `Role=` (string vacío)
- El DTO tiene valor por defecto `"Marketer"`, pero si se envía vacío, el model binder asignará `""`

## Verificación en el Controlador

El controlador ya tiene logging que muestra todos los datos recibidos (líneas 148-186):

```csharp
_logger.LogInformation("[UsersController.Create] Form[{Key}] = {Value}", key, value);
_logger.LogInformation("[UsersController.Create] Model recibido - Email: {Email}, TenantId: {TenantId}, FullName: {FullName}, Role: {Role}", 
    model?.Email ?? "NULL", 
    model?.TenantId ?? Guid.Empty, 
    model?.FullName ?? "NULL",
    model?.Role ?? "NULL");
```

Esto permite verificar exactamente qué datos llegan al controlador.

