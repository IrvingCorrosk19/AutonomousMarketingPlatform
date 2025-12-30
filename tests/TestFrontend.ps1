# Script de Pruebas del Frontend - Autonomous Marketing Platform
# Ejecutar después de iniciar la aplicación

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

Write-Host "=== PRUEBAS DEL FRONTEND ===" -ForegroundColor Cyan
Write-Host "URL Base: $baseUrl"
Write-Host ""

# Test 1: Pagina de Login carga correctamente
Write-Host "Test 1: Pagina de Login (GET) ... " -NoNewline
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method GET -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        $content = $response.Content
        if ($content -like "*Login*" -and $content -like "*Correo electrónico*" -and $content -like "*Contraseña*") {
            Test-Passed "Pagina de Login"
        } else {
            Test-Failed "Pagina de Login" "Faltan elementos del formulario"
        }
    } else {
        Test-Failed "Pagina de Login" "Status code: $($response.StatusCode)"
    }
} catch {
    Test-Failed "Pagina de Login" $_.Exception.Message
}

# Test 2: Login con credenciales validas
Write-Host "Test 2: Login con credenciales validas ... " -NoNewline
try {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    
    # Primero obtener el token anti-forgery
    $loginPage = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method GET -WebSession $session -UseBasicParsing
    $csrfToken = ""
    if ($loginPage.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $csrfToken = $matches[1]
    }
    
    $body = @{
        Email = "admin@test.com"
        Password = "Admin123!"
        RememberMe = $false
    }
    
    if ($csrfToken) {
        $body["__RequestVerificationToken"] = $csrfToken
    }
    
    $response = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method POST -Body $body -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($response.StatusCode -eq 302 -or $response.StatusCode -eq 200) {
        Test-Passed "Login con credenciales validas"
        
        # Test 3: Dashboard accesible después de login
        Write-Host "Test 3: Dashboard accesible ... " -NoNewline
        try {
            $dashboardResponse = Invoke-WebRequest -Uri "$baseUrl/Home" -Method GET -WebSession $session -UseBasicParsing
            if ($dashboardResponse.StatusCode -eq 200 -and ($dashboardResponse.Content -like "*Dashboard*" -or $dashboardResponse.Content -like "*Campaña*")) {
                Test-Passed "Dashboard accesible"
            } else {
                Test-Failed "Dashboard accesible" "No se pudo cargar el dashboard"
            }
        } catch {
            Test-Failed "Dashboard accesible" $_.Exception.Message
        }
        
        # Test 4: Lista de Campañas
        Write-Host "Test 4: Lista de Campañas ... " -NoNewline
        try {
            $campaignsResponse = Invoke-WebRequest -Uri "$baseUrl/Campaigns" -Method GET -WebSession $session -UseBasicParsing
            if ($campaignsResponse.StatusCode -eq 200) {
                Test-Passed "Lista de Campañas"
            } else {
                Test-Failed "Lista de Campañas" "Status code: $($campaignsResponse.StatusCode)"
            }
        } catch {
            Test-Failed "Lista de Campañas" $_.Exception.Message
        }
        
        # Test 5: Formulario de Crear Campaña
        Write-Host "Test 5: Formulario Crear Campaña ... " -NoNewline
        try {
            $createResponse = Invoke-WebRequest -Uri "$baseUrl/Campaigns/Create" -Method GET -WebSession $session -UseBasicParsing
            if ($createResponse.StatusCode -eq 200 -and ($createResponse.Content -like "*Crear*" -or $createResponse.Content -like "*Nombre*")) {
                Test-Passed "Formulario Crear Campaña"
            } else {
                Test-Failed "Formulario Crear Campaña" "No se pudo cargar el formulario"
            }
        } catch {
            Test-Failed "Formulario Crear Campaña" $_.Exception.Message
        }
        
        # Test 6: Contenido
        Write-Host "Test 6: Pagina de Contenido ... " -NoNewline
        try {
            $contentResponse = Invoke-WebRequest -Uri "$baseUrl/Content" -Method GET -WebSession $session -UseBasicParsing
            if ($contentResponse.StatusCode -eq 200) {
                Test-Passed "Pagina de Contenido"
            } else {
                Test-Failed "Pagina de Contenido" "Status code: $($contentResponse.StatusCode)"
            }
        } catch {
            Test-Failed "Pagina de Contenido" $_.Exception.Message
        }
        
        # Test 7: Publicaciones
        Write-Host "Test 7: Lista de Publicaciones ... " -NoNewline
        try {
            $publishingResponse = Invoke-WebRequest -Uri "$baseUrl/Publishing" -Method GET -WebSession $session -UseBasicParsing
            if ($publishingResponse.StatusCode -eq 200) {
                Test-Passed "Lista de Publicaciones"
            } else {
                Test-Failed "Lista de Publicaciones" "Status code: $($publishingResponse.StatusCode)"
            }
        } catch {
            Test-Failed "Lista de Publicaciones" $_.Exception.Message
        }
        
        # Test 8: Métricas
        Write-Host "Test 8: Pagina de Métricas ... " -NoNewline
        try {
            $metricsResponse = Invoke-WebRequest -Uri "$baseUrl/Metrics" -Method GET -WebSession $session -UseBasicParsing
            if ($metricsResponse.StatusCode -eq 200) {
                Test-Passed "Pagina de Métricas"
            } else {
                Test-Failed "Pagina de Métricas" "Status code: $($metricsResponse.StatusCode)"
            }
        } catch {
            Test-Failed "Pagina de Métricas" $_.Exception.Message
        }
        
    } else {
        Test-Failed "Login con credenciales validas" "No se redirigio correctamente"
    }
} catch {
    Test-Failed "Login con credenciales validas" $_.Exception.Message
}

# Resumen
Write-Host ""
Write-Host "=== RESUMEN DE PRUEBAS FRONTEND ===" -ForegroundColor Cyan
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

Write-Host ""
Write-Host "Para pruebas visuales, abrir en navegador: $baseUrl" -ForegroundColor Cyan


