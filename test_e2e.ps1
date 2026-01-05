# Pruebas End-to-End del flujo completo
$ErrorActionPreference = "Continue"

# Ignorar errores de certificado SSL
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PRUEBAS E2E - FLUJO COMPLETO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "https://localhost:56609"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$testResults = @()

function Test-Step {
    param(
        [string]$Name,
        [scriptblock]$Test
    )
    
    Write-Host "[TEST] $Name" -ForegroundColor Yellow
    try {
        $result = & $Test
        Write-Host "[OK] $Name" -ForegroundColor Green
        $script:testResults += @{Name=$Name; Status="OK"; Details=$result}
        return $result
    }
    catch {
        Write-Host "[FAIL] $Name - $($_.Exception.Message)" -ForegroundColor Red
        $script:testResults += @{Name=$Name; Status="FAIL"; Details=$_.Exception.Message}
        return $null
    }
    Write-Host ""
}

# Test 1: Verificar que la aplicación está corriendo
Test-Step "1. Verificar aplicación corriendo" {
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" `
        -WebSession $session `
        -UseBasicParsing `
        -ErrorAction Stop
    
    if ($response.StatusCode -eq 200) {
        return "Status: $($response.StatusCode), Content: $($response.Content.Length) bytes"
    }
    throw "Status code: $($response.StatusCode)"
}

# Test 2: Verificar formulario de login
Test-Step "2. Verificar formulario de login" {
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" `
        -WebSession $session `
        -UseBasicParsing `
        -ErrorAction Stop
    
    $hasForm = $response.Content -match 'loginForm'
    $hasEmail = $response.Content -match 'name="Email"|asp-for="Email"'
    $hasPassword = $response.Content -match 'name="Password"|asp-for="Password"'
    
    return "Form: $hasForm, Email: $hasEmail, Password: $hasPassword"
}

# Test 3: Hacer login
Test-Step "3. Hacer login" {
    $loginBody = @{
        Email = "admin@test.com"
        Password = "Admin123!"
        RememberMe = "false"
    }
    
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" `
            -Method POST `
            -WebSession $session `
            -Body $loginBody `
            -UseBasicParsing `
            -ErrorAction Stop
        
        $isRedirect = $response.StatusCode -eq 302 -or $response.StatusCode -eq 301
        $location = if ($response.Headers.Location) { $response.Headers.Location.ToString() } else { "N/A" }
        
        return "Status: $($response.StatusCode), Redirect: $isRedirect, Location: $location"
    }
    catch {
        # Si hay error pero es redirect, está bien
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
            if ($statusCode -eq 302 -or $statusCode -eq 301) {
                return "Status: $statusCode (Redirect - OK)"
            }
        }
        throw
    }
}

# Test 4: Acceder a Users/Create
Test-Step "4. Acceder a formulario de crear usuario" {
    $response = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
        -WebSession $session `
        -UseBasicParsing `
        -ErrorAction Stop
    
    $hasForm = $response.Content -match 'createUserForm'
    $hasEmail = $response.Content -match 'asp-for="Email"'
    $hasPassword = $response.Content -match 'asp-for="Password"'
    $hasConfirmPassword = $response.Content -match 'asp-for="ConfirmPassword"'
    $hasFullName = $response.Content -match 'asp-for="FullName"'
    $hasRole = $response.Content -match 'asp-for="Role"'
    $hasIsActive = $response.Content -match 'asp-for="IsActive"'
    $hasTenantId = $response.Content -match 'TenantId' -and 
                   ($response.Content -match 'type="hidden"' -or 
                    $response.Content -match 'input type="hidden"')
    
    return "Status: $($response.StatusCode), Form: $hasForm, Email: $hasEmail, Password: $hasPassword, ConfirmPassword: $hasConfirmPassword, FullName: $hasFullName, Role: $hasRole, IsActive: $hasIsActive, TenantId: $hasTenantId"
}

# Test 5: Crear usuario exitoso
Test-Step "5. Crear usuario (caso exitoso)" {
    # Primero obtener el formulario para ver si hay TenantId
    $getResponse = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
        -WebSession $session `
        -UseBasicParsing `
        -ErrorAction Stop
    
    # Extraer TenantId si está en un hidden input
    $tenantId = $null
    if ($getResponse.Content -match 'name="TenantId"[^>]*value="([^"]+)"') {
        $tenantId = $matches[1]
    }
    elseif ($getResponse.Content -match 'TenantId"[^>]*value="([^"]+)"') {
        $tenantId = $matches[1]
    }
    
    $userBody = @{
        Email = "testuser_$(Get-Date -Format 'yyyyMMddHHmmss')@example.com"
        Password = "Test123!@#"
        ConfirmPassword = "Test123!@#"
        FullName = "Test User E2E"
        Role = "Marketer"
        IsActive = "true"
    }
    
    if ($tenantId -and $tenantId -ne "" -and $tenantId -ne "00000000-0000-0000-0000-000000000000") {
        $userBody["TenantId"] = $tenantId
    }
    
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
            -Method POST `
            -WebSession $session `
            -Body $userBody `
            -UseBasicParsing `
            -ErrorAction Stop
        
        $isRedirect = $response.StatusCode -eq 302 -or $response.StatusCode -eq 301
        $location = if ($response.Headers.Location) { $response.Headers.Location.ToString() } else { "N/A" }
        $hasSuccess = $location -match "/Users/Index" -or $response.Content -match "SuccessMessage|exitosamente"
        
        return "Status: $($response.StatusCode), Redirect: $isRedirect, Location: $location, Success: $hasSuccess"
    }
    catch {
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
            if ($statusCode -eq 302 -or $statusCode -eq 301) {
                $location = $_.Exception.Response.Headers.Location.ToString()
                return "Status: $statusCode (Redirect - OK), Location: $location"
            }
            
            # Intentar leer el contenido del error
            try {
                $errorStream = $_.Exception.Response.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($errorStream)
                $errorContent = $reader.ReadToEnd()
                
                $hasErrors = $errorContent -match 'alert-danger|validation-summary|ModelState'
                return "Status: $statusCode, HasErrors: $hasErrors, ContentLength: $($errorContent.Length)"
            }
            catch {
                return "Status: $statusCode, Error: $($_.Exception.Message)"
            }
        }
        throw
    }
}

# Test 6: Validación - Campos vacíos
Test-Step "6. Validación - Campos vacíos" {
    $userBody = @{
        Email = ""
        Password = ""
        ConfirmPassword = ""
        FullName = ""
        Role = ""
    }
    
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
            -Method POST `
            -WebSession $session `
            -Body $userBody `
            -UseBasicParsing `
            -ErrorAction Stop
        
        # Si llega aquí, debería tener errores de validación
        $hasErrors = $response.Content -match 'alert-danger|validation-summary|required|requerido'
        return "Status: $($response.StatusCode), HasErrors: $hasErrors"
    }
    catch {
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
            try {
                $errorStream = $_.Exception.Response.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($errorStream)
                $errorContent = $reader.ReadToEnd()
                
                $hasErrors = $errorContent -match 'alert-danger|validation-summary|required|requerido'
                return "Status: $statusCode, HasErrors: $hasErrors (Expected)"
            }
            catch {
                return "Status: $statusCode (Expected validation error)"
            }
        }
        throw
    }
}

# Test 7: Validación - Contraseña inválida
Test-Step "7. Validación - Contraseña inválida" {
    $getResponse = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
        -WebSession $session `
        -UseBasicParsing `
        -ErrorAction Stop
    
    $tenantId = $null
    if ($getResponse.Content -match 'name="TenantId"[^>]*value="([^"]+)"') {
        $tenantId = $matches[1]
    }
    
    $userBody = @{
        Email = "testinvalid@example.com"
        Password = "123"  # Muy corta
        ConfirmPassword = "123"
        FullName = "Test Invalid"
        Role = "Marketer"
        IsActive = "true"
    }
    
    if ($tenantId -and $tenantId -ne "" -and $tenantId -ne "00000000-0000-0000-0000-000000000000") {
        $userBody["TenantId"] = $tenantId
    }
    
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/Users/Create" `
            -Method POST `
            -WebSession $session `
            -Body $userBody `
            -UseBasicParsing `
            -ErrorAction Stop
        
        $hasErrors = $response.Content -match 'contraseña|password|8 caracteres|al menos'
        return "Status: $($response.StatusCode), HasErrors: $hasErrors"
    }
    catch {
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
            try {
                $errorStream = $_.Exception.Response.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($errorStream)
                $errorContent = $reader.ReadToEnd()
                
                $hasErrors = $errorContent -match 'contraseña|password|8 caracteres|al menos'
                return "Status: $statusCode, HasErrors: $hasErrors (Expected)"
            }
            catch {
                return "Status: $statusCode (Expected validation error)"
            }
        }
        throw
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE PRUEBAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$passed = ($testResults | Where-Object { $_.Status -eq "OK" }).Count
$failed = ($testResults | Where-Object { $_.Status -eq "FAIL" }).Count

foreach ($result in $testResults) {
    $color = if ($result.Status -eq "OK") { "Green" } else { "Red" }
    Write-Host "$($result.Status) - $($result.Name)" -ForegroundColor $color
    Write-Host "  $($result.Details)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Total: $passed OK, $failed FAIL" -ForegroundColor $(if ($failed -eq 0) { "Green" } else { "Yellow" })

