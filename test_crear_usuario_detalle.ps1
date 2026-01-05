# Prueba detallada de creación de usuario
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$baseUrl = "https://localhost:56609"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Host "=== PRUEBA DETALLADA: Crear Usuario ===" -ForegroundColor Cyan
Write-Host ""

# 1. Login
Write-Host "1. Haciendo login..." -ForegroundColor Yellow
$loginBody = @{
    Email = "admin@test.com"
    Password = "Admin123!"
    RememberMe = "false"
}
$loginResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" `
    -Method POST `
    -WebSession $session `
    -Body $loginBody `
    -UseBasicParsing `
    -ErrorAction Stop
Write-Host "   Login OK" -ForegroundColor Green
Write-Host ""

# 2. Obtener formulario
Write-Host "2. Obteniendo formulario..." -ForegroundColor Yellow
$getResponse = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
    -WebSession $session `
    -UseBasicParsing `
    -ErrorAction Stop

# Extraer TenantId
$tenantId = $null
if ($getResponse.Content -match 'name="TenantId"[^>]*value="([^"]+)"') {
    $tenantId = $matches[1]
}
Write-Host "   TenantId encontrado: $tenantId" -ForegroundColor Cyan
Write-Host ""

# 3. Crear usuario
Write-Host "3. Creando usuario..." -ForegroundColor Yellow
$email = "testuser_$(Get-Date -Format 'yyyyMMddHHmmss')@example.com"
$userBody = @{
    Email = $email
    Password = "Test123!@#"
    ConfirmPassword = "Test123!@#"
    FullName = "Test User E2E"
    Role = "Marketer"
    IsActive = "true"
}

if ($tenantId) {
    $userBody["TenantId"] = $tenantId
}

Write-Host "   Datos a enviar:" -ForegroundColor Cyan
foreach ($key in $userBody.Keys) {
    $value = if ($key -match "Password") { "***" } else { $userBody[$key] }
    Write-Host "     $key = $value"
}
Write-Host ""

try {
    $createResponse = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
        -Method POST `
        -WebSession $session `
        -Body $userBody `
        -UseBasicParsing `
        -ErrorAction Stop
    
    Write-Host "   Status: $($createResponse.StatusCode)" -ForegroundColor Green
    Write-Host "   Content Length: $($createResponse.Content.Length) bytes"
    
    # Verificar contenido
    $hasSuccess = $createResponse.Content -match "SuccessMessage|exitosamente|Usuario creado"
    $hasErrors = $createResponse.Content -match "alert-danger|validation-summary|ModelState"
    $isRedirect = $createResponse.StatusCode -eq 302 -or $createResponse.StatusCode -eq 301
    
    Write-Host "   Tiene mensaje de éxito: $hasSuccess"
    Write-Host "   Tiene errores: $hasErrors"
    Write-Host "   Es redirect: $isRedirect"
    
    if ($createResponse.Headers.Location) {
        Write-Host "   Location: $($createResponse.Headers.Location)" -ForegroundColor Cyan
    }
    
    # Buscar mensajes específicos
    if ($createResponse.Content -match 'alert-success|SuccessMessage') {
        Write-Host "   [OK] Usuario creado exitosamente" -ForegroundColor Green
    }
    elseif ($createResponse.Content -match 'alert-danger') {
        Write-Host "   [ERROR] Hay errores en la respuesta" -ForegroundColor Red
        # Extraer primeros 500 caracteres del contenido para ver errores
        $errorSnippet = $createResponse.Content.Substring(0, [Math]::Min(500, $createResponse.Content.Length))
        Write-Host "   Snippet: $errorSnippet" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "   ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "   Status Code: $statusCode"
        
        try {
            $errorStream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorStream)
            $errorContent = $reader.ReadToEnd()
            Write-Host "   Error Content Length: $($errorContent.Length) bytes"
            
            if ($errorContent -match 'alert-danger|validation-summary') {
                Write-Host "   Contiene errores de validación" -ForegroundColor Yellow
            }
        }
        catch {
            Write-Host "   No se pudo leer el contenido del error"
        }
    }
}

Write-Host ""
Write-Host "=== PRUEBA COMPLETADA ===" -ForegroundColor Cyan

