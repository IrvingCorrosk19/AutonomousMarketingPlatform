# Script de pruebas funcionales para Autonomous Marketing Platform
# Ejecutar despues de iniciar la aplicacion

$baseUrl = "http://localhost:56610"
$testsPassed = 0
$testsFailed = 0
$errorList = @()

function Test-Passed {
    param([string]$testName)
    Write-Host "[PASS] $testName" -ForegroundColor Green
    $script:testsPassed++
}

function Test-Failed {
    param([string]$testName, [string]$errorMessage)
    Write-Host "[FAIL] $testName - $errorMessage" -ForegroundColor Red
    $script:testsFailed++
    $script:errorList += "$testName : $errorMessage"
}

Write-Host "=== INICIANDO PRUEBAS FUNCIONALES ===" -ForegroundColor Cyan
Write-Host "URL Base: $baseUrl"
Write-Host ""

# Test 1: Pagina de Login
Write-Host "Test 1: Pagina de Login (GET) ... " -NoNewline
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method GET -UseBasicParsing
    if ($response.StatusCode -eq 200 -and $response.Content -like "*Login*") {
        Test-Passed "Pagina de Login"
    } else {
        Test-Failed "Pagina de Login" "No se pudo cargar correctamente"
    }
} catch {
    Test-Failed "Pagina de Login" $_.Exception.Message
}

# Test 2: Login con credenciales invalidas
Write-Host "Test 2: Login con credenciales invalidas ... " -NoNewline
try {
    $body = @{
        Email = "invalid@test.com"
        Password = "WrongPassword123!"
    }
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method POST -Body $body -UseBasicParsing -ErrorAction SilentlyContinue
    if ($response.StatusCode -eq 200) {
        Test-Passed "Login con credenciales invalidas"
    } else {
        Test-Failed "Login con credenciales invalidas" "No retorno codigo 200"
    }
} catch {
    if ($_.Exception.Response.StatusCode -eq 400 -or $_.Exception.Response.StatusCode -eq 401) {
        Test-Passed "Login con credenciales invalidas"
    } else {
        Test-Failed "Login con credenciales invalidas" $_.Exception.Message
    }
}

# Test 3: Login con credenciales validas
Write-Host "Test 3: Login con credenciales validas ... " -NoNewline
try {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $body = @{
        Email = "admin@test.com"
        Password = "Admin123!"
    }
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method POST -Body $body -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($response.StatusCode -eq 302 -or $response.StatusCode -eq 200) {
        Test-Passed "Login con credenciales validas"
        
        # Test 4: Acceso al Dashboard
        Write-Host "Test 4: Acceso al Dashboard ... " -NoNewline
        try {
            $dashboardResponse = Invoke-WebRequest -Uri "$baseUrl/Home" -Method GET -WebSession $session -UseBasicParsing
            if ($dashboardResponse.StatusCode -eq 200 -and ($dashboardResponse.Content -like "*Dashboard*" -or $dashboardResponse.Content -like "*Campana*")) {
                Test-Passed "Acceso al Dashboard"
            } else {
                Test-Failed "Acceso al Dashboard" "No se pudo cargar el dashboard"
            }
        } catch {
            Test-Failed "Acceso al Dashboard" $_.Exception.Message
        }
        
        # Test 5: Lista de Campanas
        Write-Host "Test 5: Lista de Campanas ... " -NoNewline
        try {
            $campaignsResponse = Invoke-WebRequest -Uri "$baseUrl/Campaigns" -Method GET -WebSession $session -UseBasicParsing
            if ($campaignsResponse.StatusCode -eq 200 -and $campaignsResponse.Content -like "*Campana*") {
                Test-Passed "Lista de Campanas"
            } else {
                Test-Failed "Lista de Campanas" "No se pudo cargar la lista"
            }
        } catch {
            Test-Failed "Lista de Campanas" $_.Exception.Message
        }
        
        # Test 6: Formulario de Crear Campana
        Write-Host "Test 6: Formulario de Crear Campana ... " -NoNewline
        try {
            $createResponse = Invoke-WebRequest -Uri "$baseUrl/Campaigns/Create" -Method GET -WebSession $session -UseBasicParsing
            if ($createResponse.StatusCode -eq 200 -and ($createResponse.Content -like "*Crear*" -or $createResponse.Content -like "*Nombre*")) {
                Test-Passed "Formulario de Crear Campana"
            } else {
                Test-Failed "Formulario de Crear Campana" "No se pudo cargar el formulario"
            }
        } catch {
            Test-Failed "Formulario de Crear Campana" $_.Exception.Message
        }
        
    } else {
        Test-Failed "Login con credenciales validas" "No se redirigio correctamente"
    }
} catch {
    Test-Failed "Login con credenciales validas" $_.Exception.Message
}

# Resumen
Write-Host ""
Write-Host "=== RESUMEN DE PRUEBAS ===" -ForegroundColor Cyan
Write-Host "Total: $($testsPassed + $testsFailed)"
Write-Host "Pasadas: $testsPassed" -ForegroundColor Green
Write-Host "Fallidas: $testsFailed" -ForegroundColor $(if ($testsFailed -gt 0) { "Red" } else { "Green" })

if ($errorList.Count -gt 0) {
    Write-Host ""
    Write-Host "Errores encontrados:" -ForegroundColor Yellow
    foreach ($err in $errorList) {
        Write-Host "  - $err" -ForegroundColor Yellow
    }
}
