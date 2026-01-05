# Pruebas Frontend - Flujo Completo de Usuario

## URL Base
- **Login**: `https://localhost:56609/Account/Login`
- **Crear Usuario**: `https://localhost:56609/Users/Create`

## Credenciales de Prueba
- **Email**: `admin@test.com`
- **Password**: `Admin123!`

---

## Prueba 1: Login

### Pasos:
1. Abrir navegador en `https://localhost:56609/Account/Login`
2. Verificar que el formulario carga correctamente
3. Ingresar credenciales:
   - Email: `admin@test.com`
   - Password: `Admin123!`
4. Hacer clic en "Iniciar sesión"

### Resultado Esperado:
- ✅ Login exitoso
- ✅ Redirección a `/Home/Index` o dashboard
- ✅ No hay errores en consola del navegador

---

## Prueba 2: Acceder a Crear Usuario

### Pasos:
1. Después del login, navegar a `https://localhost:56609/Users/Create`
2. Verificar que el formulario carga correctamente

### Verificaciones:
- ✅ El formulario tiene todos los campos:
  - Email
  - Password
  - ConfirmPassword
  - FullName
  - Role (select con opciones)
  - IsActive (checkbox)
- ✅ Si NO eres SuperAdmin, debe haber un campo `TenantId` oculto (hidden input)
- ✅ El select de `Role` debe tener "Marketer" seleccionado por defecto
- ✅ El checkbox `IsActive` debe estar marcado por defecto
- ✅ No hay errores en consola del navegador

---

## Prueba 3: Crear Usuario - Caso Exitoso

### Pasos:
1. Llenar el formulario con:
   - **Email**: `testuser@example.com`
   - **Password**: `Test123!@#`
   - **ConfirmPassword**: `Test123!@#`
   - **FullName**: `Test User`
   - **Role**: `Marketer` (o cualquier otro)
   - **IsActive**: ✅ Marcado
2. Hacer clic en "Crear Usuario"
3. Abrir DevTools (F12) → Network tab para ver la petición POST

### Verificaciones en Network:
- ✅ La petición es `POST /Users/Create`
- ✅ Content-Type: `application/x-www-form-urlencoded`
- ✅ El body contiene todos los campos:
  ```
  Email=testuser@example.com
  Password=Test123!@#
  ConfirmPassword=Test123!@#
  FullName=Test User
  Role=Marketer
  IsActive=true
  TenantId=<guid-del-usuario-actual>
  ```
- ✅ Status Code: `302 Redirect` (éxito) o `200 OK` con mensaje de éxito
- ✅ Si es redirect, debe ir a `/Users/Index`

### Resultado Esperado:
- ✅ Usuario creado exitosamente
- ✅ Redirección a `/Users/Index`
- ✅ Mensaje de éxito: "Usuario creado exitosamente."
- ✅ El nuevo usuario aparece en la lista

---

## Prueba 4: Validaciones - Campos Vacíos

### Pasos:
1. Ir a `/Users/Create`
2. Intentar enviar el formulario sin llenar ningún campo
3. Hacer clic en "Crear Usuario"

### Resultado Esperado:
- ✅ El formulario NO se envía (validación HTML5)
- ✅ O si se envía, muestra errores de validación:
  - "El email es requerido"
  - "La contraseña es requerida"
  - "La confirmación de contraseña es requerida"
  - "El nombre completo es requerido"
  - "El rol es requerido"

---

## Prueba 5: Validaciones - Contraseña Inválida

### Pasos:
1. Llenar el formulario con:
   - Email: `test2@example.com`
   - Password: `123` (muy corta)
   - ConfirmPassword: `123`
   - FullName: `Test User 2`
   - Role: `Marketer`
2. Hacer clic en "Crear Usuario"

### Resultado Esperado:
- ✅ Error de validación: "La contraseña debe tener al menos 8 caracteres"
- ✅ El formulario NO se envía o muestra el error

---

## Prueba 6: Validaciones - Contraseñas No Coinciden

### Pasos:
1. Llenar el formulario con:
   - Email: `test3@example.com`
   - Password: `Test123!@#`
   - ConfirmPassword: `Test456!@#` (diferente)
   - FullName: `Test User 3`
   - Role: `Marketer`
2. Hacer clic en "Crear Usuario"

### Resultado Esperado:
- ✅ Error de validación: "Las contraseñas no coinciden"
- ✅ El formulario NO se envía o muestra el error

---

## Prueba 7: Verificar que NO hay JavaScript Interfiriendo

### Pasos:
1. Abrir DevTools (F12) → Console
2. Ir a `/Users/Create`
3. Llenar el formulario y hacer clic en "Crear Usuario"
4. Verificar en Console que NO hay:
   - `e.preventDefault()` ejecutándose
   - Peticiones `fetch()` a `/Users/Create`
   - Errores de JavaScript relacionados con el formulario

### Verificaciones en Network:
- ✅ Solo hay UNA petición POST a `/Users/Create`
- ✅ NO hay peticiones AJAX/fetch adicionales
- ✅ La petición es un POST HTML estándar

---

## Checklist Final

- [ ] Login funciona correctamente
- [ ] Formulario de creación carga correctamente
- [ ] TenantId se asigna automáticamente (hidden field)
- [ ] Crear usuario funciona con datos válidos
- [ ] Validaciones funcionan (campos vacíos, contraseña inválida, contraseñas no coinciden)
- [ ] NO hay JavaScript interfiriendo con el POST
- [ ] El formulario hace POST HTML estándar
- [ ] Redirección funciona después de crear usuario
- [ ] Mensajes de éxito/error se muestran correctamente

---

## Notas Técnicas

### Lo que se corrigió:
1. ✅ Eliminado todo JavaScript que interfería con el POST
2. ✅ Solo Data Annotations para validación
3. ✅ Postback HTML estándar (sin `e.preventDefault()` ni `fetch()`)
4. ✅ `TenantId` se asigna automáticamente desde el usuario actual en el controlador
5. ✅ Checkbox `IsActive` corregido (sin inputs duplicados)
6. ✅ Select de `Role` con valor por defecto "Marketer"

### Campos del Formulario:
- `Email` - Required, EmailAddress
- `Password` - Required, MinLength(8), Regex para complejidad
- `ConfirmPassword` - Required, Compare con Password
- `FullName` - Required, MaxLength(200)
- `TenantId` - NO Required (se asigna en controlador)
- `Role` - Required, default "Marketer"
- `IsActive` - Opcional, default true

