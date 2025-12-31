# Script para probar el flujo completo: Login -> Crear Campaña -> Solicitar Contenido (n8n)
# Simula un usuario navegando por el frontend

$baseUrl = "http://localhost:56610"
$loginUrl = "$baseUrl/Account/Login"
$campaignsUrl = "$baseUrl/Campaigns"
$createCampaignUrl = "$baseUrl/Campaigns/Create"
$marketingRequestUrl = "$baseUrl/MarketingRequest/Create"

# Credenciales de prueba
$email = "admin@test.com"
$password = "Admin123!"

Write-Host "=== PRUEBA DE FLUJO COMPLETO ===" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Login
Write-Host "1. Iniciando sesion..." -ForegroundColor Yellow
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

# Obtener pagina de login para obtener el token anti-forgery
try {
    $loginPage = Invoke-WebRequest -Uri $loginUrl -SessionVariable session -UseBasicParsing
    $csrfToken = ""
    
    # Extraer el token anti-forgery usando Select-String
    $tokenMatch = $loginPage.Content | Select-String -Pattern '__RequestVerificationToken.*?value="([^"]+)"'
    if ($tokenMatch) {
        $csrfToken = $tokenMatch.Matches[0].Groups[1].Value
        Write-Host "   Token CSRF obtenido" -ForegroundColor Green
    }
    
    # Preparar datos de login
    $loginData = @{
        Email = $email
        Password = $password
        __RequestVerificationToken = $csrfToken
    }
    
    # Hacer login
    $loginResponse = Invoke-WebRequest -Uri $loginUrl -Method POST -Body $loginData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($loginResponse.StatusCode -eq 302 -or $loginResponse.StatusCode -eq 200) {
        Write-Host "   Login exitoso" -ForegroundColor Green
    } else {
        Write-Host "   Error en login: Status $($loginResponse.StatusCode)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "   Error al hacer login: $_" -ForegroundColor Red
    exit 1
}

# Paso 2: Ir a crear campana
Write-Host ""
Write-Host "2. Navegando a crear campana..." -ForegroundColor Yellow
try {
    $createPage = Invoke-WebRequest -Uri $createCampaignUrl -WebSession $session -UseBasicParsing
    
    # Extraer token CSRF de la pagina de crear campana
    $csrfToken = ""
    $tokenMatch = $createPage.Content | Select-String -Pattern '__RequestVerificationToken.*?value="([^"]+)"'
    if ($tokenMatch) {
        $csrfToken = $tokenMatch.Matches[0].Groups[1].Value
    }
    
    Write-Host "   Pagina de crear campana cargada" -ForegroundColor Green
} catch {
    Write-Host "   Error al cargar pagina de crear campana: $_" -ForegroundColor Red
    exit 1
}

# Paso 3: Crear campana
Write-Host ""
Write-Host "3. Creando campana..." -ForegroundColor Yellow
$campaignName = "Campana de Prueba - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$campaignData = @{
    Name = $campaignName
    Description = "Campana creada desde script de prueba para verificar el flujo completo"
    Status = "Draft"
    StartDate = (Get-Date).ToString("yyyy-MM-dd")
    EndDate = (Get-Date).AddDays(30).ToString("yyyy-MM-dd")
    Budget = "1000"
    __RequestVerificationToken = $csrfToken
}

$campaignId = $null
try {
    $createResponse = Invoke-WebRequest -Uri $createCampaignUrl -Method POST -Body $campaignData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($createResponse.StatusCode -eq 302) {
        # Extraer el ID de la campana de la URL de redireccion
        $location = $createResponse.Headers.Location
        $idMatch = $location | Select-String -Pattern '/Campaigns/Details/([a-f0-9-]+)'
        if ($idMatch) {
            $campaignId = $idMatch.Matches[0].Groups[1].Value
            Write-Host "   Campana creada exitosamente" -ForegroundColor Green
            Write-Host "   Campaign ID: $campaignId" -ForegroundColor Cyan
        } else {
            Write-Host "   Campana creada (redirigido)" -ForegroundColor Green
        }
    } else {
        Write-Host "   Error al crear campana: Status $($createResponse.StatusCode)" -ForegroundColor Red
        $contentPreview = $createResponse.Content.Substring(0, [Math]::Min(500, $createResponse.Content.Length))
        Write-Host "   Response: $contentPreview" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "   Error al crear campana: $_" -ForegroundColor Red
    exit 1
}

# Paso 4: Solicitar contenido (dispara n8n)
Write-Host ""
Write-Host "4. Solicitando contenido de marketing (dispara n8n)..." -ForegroundColor Yellow

try {
    # Ir a la pagina de solicitar contenido
    $requestPageUrl = "$marketingRequestUrl"
    if ($campaignId) {
        $requestPageUrl += "?campaignId=$campaignId"
    }
    
    $requestPage = Invoke-WebRequest -Uri $requestPageUrl -WebSession $session -UseBasicParsing
    
    # Extraer token CSRF
    $csrfToken = ""
    $tokenMatch = $requestPage.Content | Select-String -Pattern '__RequestVerificationToken.*?value="([^"]+)"'
    if ($tokenMatch) {
        $csrfToken = $tokenMatch.Matches[0].Groups[1].Value
    }
    
    # Preparar datos de solicitud
    $requestData = @{
        Instruction = "Crear contenido de marketing para Instagram promocionando la nueva campana. Enfocarse en publico joven, usar tono casual y moderno."
        ChannelsJson = '["instagram"]'
        RequiresApproval = "false"
        CampaignId = $campaignId
        AssetsText = ""
        __RequestVerificationToken = $csrfToken
    }
    
    $requestResponse = Invoke-WebRequest -Uri $marketingRequestUrl -Method POST -Body $requestData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($requestResponse.StatusCode -eq 302) {
        Write-Host "   Solicitud de contenido enviada a n8n exitosamente" -ForegroundColor Green
        
        # Intentar extraer el RequestId de la URL de redireccion
        $location = $requestResponse.Headers.Location
        $requestIdMatch = $location | Select-String -Pattern 'requestId=([^&]+)'
        if ($requestIdMatch) {
            $requestId = $requestIdMatch.Matches[0].Groups[1].Value
            Write-Host "   Request ID (n8n): $requestId" -ForegroundColor Cyan
        }
    } else {
        Write-Host "   Error al enviar solicitud: Status $($requestResponse.StatusCode)" -ForegroundColor Red
        $contentPreview = $requestResponse.Content.Substring(0, [Math]::Min(500, $requestResponse.Content.Length))
        Write-Host "   Response: $contentPreview" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   Error al solicitar contenido: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "   Response: $responseBody" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== PRUEBA COMPLETADA ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Resumen:" -ForegroundColor Yellow
Write-Host "  - Campana creada: $campaignName" -ForegroundColor White
if ($campaignId) {
    Write-Host "  - Campaign ID: $campaignId" -ForegroundColor White
}
Write-Host "  - Solicitud enviada a n8n: OK" -ForegroundColor White
Write-Host ""
Write-Host "Verifica en los logs de la aplicacion y en n8n que la solicitud fue recibida." -ForegroundColor Green
