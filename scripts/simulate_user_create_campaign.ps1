# Script que simula un usuario real creando una campaña desde el frontend
$baseUrl = "http://localhost:56610"
$loginUrl = "$baseUrl/Account/Login"
$createCampaignUrl = "$baseUrl/Campaigns/Create"

# Credenciales de usuario normal (NO SuperAdmin)
$email = "marketer@test.com"
$password = "Marketer123!"

Write-Host "=== SIMULANDO USUARIO CREANDO CAMPAÑA ===" -ForegroundColor Cyan
Write-Host ""

# Crear sesión para mantener cookies
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

# Paso 1: Obtener página de login
Write-Host "1. Accediendo a pagina de login..." -ForegroundColor Yellow
try {
    $loginPage = Invoke-WebRequest -Uri $loginUrl -SessionVariable session -UseBasicParsing
    Write-Host "   OK - Pagina de login cargada" -ForegroundColor Green
    
    # Extraer token CSRF
    $tokenMatch = $loginPage.Content | Select-String -Pattern '__RequestVerificationToken.*?value="([^"]+)"'
    $csrfToken = ""
    if ($tokenMatch) {
        $csrfToken = $tokenMatch.Matches[0].Groups[1].Value
        Write-Host "   OK - Token CSRF obtenido" -ForegroundColor Green
    }
} catch {
    Write-Host "   ERROR: $_" -ForegroundColor Red
    exit 1
}

# Paso 2: Iniciar sesión
Write-Host ""
Write-Host "2. Iniciando sesion como $email..." -ForegroundColor Yellow
try {
    $loginData = @{
        Email = $email
        Password = $password
        __RequestVerificationToken = $csrfToken
    }
    
    $loginResponse = Invoke-WebRequest -Uri $loginUrl -Method POST -Body $loginData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($loginResponse.StatusCode -eq 302 -or $loginResponse.StatusCode -eq 200) {
        Write-Host "   OK - Login exitoso" -ForegroundColor Green
    } else {
        Write-Host "   ERROR - Status $($loginResponse.StatusCode)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "   ERROR al hacer login: $_" -ForegroundColor Red
    exit 1
}

# Paso 3: Navegar a crear campaña
Write-Host ""
Write-Host "3. Navegando a crear campana..." -ForegroundColor Yellow
try {
    $createPage = Invoke-WebRequest -Uri $createCampaignUrl -WebSession $session -UseBasicParsing
    Write-Host "   OK - Pagina de crear campana cargada" -ForegroundColor Green
    
    # Extraer token CSRF
    $tokenMatch = $createPage.Content | Select-String -Pattern '__RequestVerificationToken.*?value="([^"]+)"'
    $csrfToken = ""
    if ($tokenMatch) {
        $csrfToken = $tokenMatch.Matches[0].Groups[1].Value
        Write-Host "   OK - Token CSRF obtenido" -ForegroundColor Green
    }
} catch {
    Write-Host "   ERROR: $_" -ForegroundColor Red
    exit 1
}

# Paso 4: Crear campaña
Write-Host ""
Write-Host "4. Creando campana..." -ForegroundColor Yellow

$campaignName = "Campana de Prueba - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$campaignData = @{
    Name = $campaignName
    Description = "Campana creada desde script que simula usuario navegando por el frontend"
    Status = "Draft"
    StartDate = (Get-Date).ToString("yyyy-MM-dd")
    EndDate = (Get-Date).AddDays(30).ToString("yyyy-MM-dd")
    Budget = "1000"
    "Objectives[goals][0]" = "Aumentar engagement"
    "Objectives[goals][1]" = "Generar leads"
    "TargetAudience[ageRange]" = "18-35"
    "TargetAudience[interests][0]" = "tecnologia"
    "TargetAudience[interests][1]" = "marketing"
    "TargetChannels[0]" = "instagram"
    "TargetChannels[1]" = "facebook"
    __RequestVerificationToken = $csrfToken
}

Write-Host "   Datos de la campana:" -ForegroundColor Cyan
Write-Host "     - Nombre: $campaignName" -ForegroundColor White
Write-Host "     - Estado: Draft" -ForegroundColor White
Write-Host "     - Presupuesto: 1000" -ForegroundColor White

try {
    $createResponse = Invoke-WebRequest -Uri $createCampaignUrl -Method POST -Body $campaignData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($createResponse.StatusCode -eq 302) {
        $location = $createResponse.Headers.Location
        $idMatch = $location | Select-String -Pattern '/Campaigns/Details/([a-f0-9-]+)'
        
        if ($idMatch) {
            $campaignId = $idMatch.Matches[0].Groups[1].Value
            Write-Host ""
            Write-Host "   *** CAMPAÑA CREADA EXITOSAMENTE ***" -ForegroundColor Green
            Write-Host "   Campaign ID: $campaignId" -ForegroundColor Cyan
            Write-Host "   Nombre: $campaignName" -ForegroundColor Cyan
            Write-Host "   URL de detalles: $baseUrl/Campaigns/Details/$campaignId" -ForegroundColor Cyan
        } else {
            Write-Host "   OK - Campana creada (redirigido a: $location)" -ForegroundColor Green
        }
    } else {
        Write-Host "   ERROR - Status $($createResponse.StatusCode)" -ForegroundColor Red
        Write-Host "   El servidor devolvio HTML en lugar de redireccionar" -ForegroundColor Yellow
        
        # Intentar extraer errores de validacion del HTML
        $htmlContent = $createResponse.Content
        
        # Buscar errores de validacion
        $validationErrors = $htmlContent | Select-String -Pattern 'validation-summary.*?>(.*?)</div>' -AllMatches
        if ($validationErrors) {
            Write-Host "   Errores de validacion encontrados:" -ForegroundColor Yellow
            foreach ($match in $validationErrors.Matches) {
                $errorText = $match.Groups[1].Value -replace '<[^>]+>', ''
                if ($errorText.Trim()) {
                    Write-Host "     - $errorText" -ForegroundColor Red
                }
            }
        }
        
        # Buscar mensajes de error en spans de validacion
        $spanErrors = $htmlContent | Select-String -Pattern 'text-danger[^>]*>([^<]+)</span>' -AllMatches
        if ($spanErrors) {
            Write-Host "   Errores en campos:" -ForegroundColor Yellow
            foreach ($match in $spanErrors.Matches) {
                $errorText = $match.Groups[1].Value.Trim()
                if ($errorText) {
                    Write-Host "     - $errorText" -ForegroundColor Red
                }
            }
        }
        
        # Guardar el HTML completo para inspeccion
        $htmlContent | Out-File -FilePath "campaign_create_error_response.html" -Encoding UTF8
        Write-Host "   HTML completo guardado en: campaign_create_error_response.html" -ForegroundColor Cyan
    }
} catch {
    Write-Host "   ERROR al crear campana: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "   Response: $responseBody" -ForegroundColor Yellow
        } catch {
            Write-Host "   No se pudo leer la respuesta" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "=== PROCESO COMPLETADO ===" -ForegroundColor Cyan
