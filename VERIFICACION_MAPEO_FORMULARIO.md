# Verificación de Mapeo: Formulario → Modelo del Controlador

## DTO del Controlador
```csharp
public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string Role { get; set; } = "Marketer";
    public bool IsActive { get; set; } = true;
}
```

## Formulario HTML Generado

Cuando usas `asp-for="Email"`, ASP.NET Core genera:

```html
<input name="Email" id="Email" type="email" ... />
```

## Mapeo Campo por Campo

| Campo en Formulario | `asp-for` | HTML Generado (`name`) | Propiedad en DTO | ✅ Coincide |
|---------------------|-----------|------------------------|------------------|-------------|
| Email | `asp-for="Email"` | `name="Email"` | `Email` | ✅ |
| Password | `asp-for="Password"` | `name="Password"` | `Password` | ✅ |
| ConfirmPassword | `asp-for="ConfirmPassword"` | `name="ConfirmPassword"` | `ConfirmPassword` | ✅ |
| FullName | `asp-for="FullName"` | `name="FullName"` | `FullName` | ✅ |
| TenantId | `asp-for="TenantId"` | `name="TenantId"` | `TenantId` | ✅ |
| Role | `asp-for="Role"` | `name="Role"` | `Role` | ✅ |
| IsActive | `asp-for="IsActive"` | `name="IsActive"` | `IsActive` | ✅ |

## Cómo Funciona el Model Binding

1. **El navegador envía el POST:**
   ```
   POST /Users/Create
   Content-Type: application/x-www-form-urlencoded
   
   Email=test@example.com&Password=Test123!@#&ConfirmPassword=Test123!@#&FullName=Test User&Role=Marketer&IsActive=true&TenantId=...
   ```

2. **ASP.NET Core Model Binder:**
   - Lee `Request.Form["Email"]` → asigna a `model.Email`
   - Lee `Request.Form["Password"]` → asigna a `model.Password`
   - Lee `Request.Form["ConfirmPassword"]` → asigna a `model.ConfirmPassword`
   - Lee `Request.Form["FullName"]` → asigna a `model.FullName`
   - Lee `Request.Form["TenantId"]` → intenta convertir a `Guid` y asigna a `model.TenantId`
   - Lee `Request.Form["Role"]` → asigna a `model.Role`
   - Lee `Request.Form["IsActive"]` → si existe y es "true", asigna `true` a `model.IsActive`, sino `false`

3. **El controlador recibe:**
   ```csharp
   [HttpPost]
   public async Task<IActionResult> Create(CreateUserDto model)
   {
       // model.Email ya tiene el valor
       // model.Password ya tiene el valor
       // etc.
   }
   ```

## Verificación del HTML Generado

Para verificar exactamente qué HTML se genera, puedes:

1. **Ver el HTML renderizado en el navegador:**
   - Abrir DevTools (F12)
   - Ir a la pestaña "Elements"
   - Buscar el formulario
   - Verificar que cada input tenga el atributo `name` correcto

2. **Ejemplo de HTML generado:**
   ```html
   <form action="/Users/Create" method="post" id="createUserForm">
       <input name="Email" id="Email" type="email" class="form-control" required />
       <input name="Password" id="Password" type="password" class="form-control" required />
       <input name="ConfirmPassword" id="ConfirmPassword" type="password" class="form-control" required />
       <input name="FullName" id="FullName" class="form-control" required />
       <input name="TenantId" type="hidden" value="..." />
       <select name="Role" id="Role" class="form-control" required>
           <option value="Marketer">Marketer</option>
       </select>
       <input name="IsActive" type="checkbox" class="form-check-input" />
   </form>
   ```

## Casos Especiales

### 1. Checkbox (IsActive)
- Si está **marcado**: `Request.Form["IsActive"] = "true"` → `model.IsActive = true`
- Si está **desmarcado**: `Request.Form["IsActive"]` no existe → `model.IsActive = false` (valor por defecto)

### 2. Select (Role)
- El valor seleccionado se envía como string
- ASP.NET Core lo asigna directamente a `model.Role`

### 3. Hidden Input (TenantId)
- Se envía como string (GUID)
- ASP.NET Core lo convierte automáticamente a `Guid` y asigna a `model.TenantId`

## Conclusión

✅ **TODO ESTÁ CORRECTAMENTE MAPEADO**

- Todos los campos del formulario tienen `asp-for` que genera el `name` correcto
- Todos los nombres coinciden exactamente con las propiedades del DTO
- El model binding de ASP.NET Core mapeará automáticamente los valores

