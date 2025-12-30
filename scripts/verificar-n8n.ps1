# Script para verificar que n8n esté corriendo y accesible
# Ejecutar: .\scripts\verificar-n8n.ps1

$n8nUrl = "http://localhost:5678"
$backendUrl = "http://localhost:5000"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verificación de n8n y Backend" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar n8n
Write-Host "Verificando n8n en $n8nUrl..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $n8nUrl -Method GET -TimeoutSec 5 -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ n8n está corriendo y accesible" -ForegroundColor Green
    } else {
        Write-Host "⚠️  n8n responde con código: $($response.StatusCode)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ n8n NO está accesible en $n8nUrl" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Solución: Ejecuta 'n8n start' o '.\scripts\iniciar-n8n.ps1'" -ForegroundColor Yellow
}

Write-Host ""

# Verificar Backend
Write-Host "Verificando Backend en $backendUrl..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $backendUrl -Method GET -TimeoutSec 5 -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Backend está corriendo y accesible" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Backend responde con código: $($response.StatusCode)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ Backend NO está accesible en $backendUrl" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Solución: Inicia el proyecto ASP.NET Core" -ForegroundColor Yellow
}

Write-Host ""

# Verificar endpoints del backend
Write-Host "Verificando endpoints del backend..." -ForegroundColor Yellow
$endpoints = @(
    "/api/consents/check",
    "/api/memory/context",
    "/api/marketing-packs",
    "/api/publishing-jobs",
    "/api/metrics/campaign",
    "/api/metrics/publishing-job",
    "/api/memory/save"
)

$endpointsOk = 0
$endpointsFail = 0

foreach ($endpoint in $endpoints) {
    try {
        $url = "$backendUrl$endpoint"
        $response = Invoke-WebRequest -Uri $url -Method GET -TimeoutSec 3 -UseBasicParsing -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 400 -or $response.StatusCode -eq 405) {
            # 200 = OK, 400 = Bad Request (pero endpoint existe), 405 = Method Not Allowed (pero endpoint existe)
            Write-Host "  ✅ $endpoint" -ForegroundColor Green
            $endpointsOk++
        } else {
            Write-Host "  ⚠️  $endpoint (Status: $($response.StatusCode))" -ForegroundColor Yellow
            $endpointsFail++
        }
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 400 -or $statusCode -eq 405) {
            # Endpoint existe pero requiere parámetros o método diferente
            Write-Host "  ✅ $endpoint (existe, requiere parámetros)" -ForegroundColor Green
            $endpointsOk++
        } else {
            Write-Host "  ❌ $endpoint (no accesible)" -ForegroundColor Red
            $endpointsFail++
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Resumen" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Endpoints verificados: $($endpoints.Count)" -ForegroundColor White
Write-Host "Endpoints OK: $endpointsOk" -ForegroundColor Green
Write-Host "Endpoints con problemas: $endpointsFail" -ForegroundColor $(if ($endpointsFail -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

