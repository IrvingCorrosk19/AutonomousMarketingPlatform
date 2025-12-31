# Script para instalar y configurar ngrok automáticamente
# Uso: .\scripts\instalar-y-iniciar-ngrok.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Instalacion y Configuracion de ngrok" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si ngrok ya esta instalado
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue

if ($ngrokPath) {
    Write-Host "[OK] ngrok ya esta instalado en: $($ngrokPath.Source)" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "[INFO] ngrok no esta instalado. Descargando..." -ForegroundColor Yellow
    Write-Host ""
    
    # Crear directorio para ngrok
    $ngrokDir = "$env:USERPROFILE\ngrok"
    if (-not (Test-Path $ngrokDir)) {
        New-Item -ItemType Directory -Path $ngrokDir -Force | Out-Null
    }
    
    $ngrokZip = "$ngrokDir\ngrok.zip"
    $ngrokExe = "$ngrokDir\ngrok.exe"
    
    # Descargar ngrok para Windows
    Write-Host "Descargando ngrok desde https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip ..." -ForegroundColor Yellow
    try {
        Invoke-WebRequest -Uri "https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip" -OutFile $ngrokZip -UseBasicParsing
        Write-Host "[OK] Descarga completada" -ForegroundColor Green
    } catch {
        Write-Host "[ERROR] Error al descargar ngrok: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Por favor, descarga ngrok manualmente desde:" -ForegroundColor Yellow
        Write-Host "  https://ngrok.com/download" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "O ejecuta: winget install ngrok" -ForegroundColor Yellow
        exit 1
    }
    
    # Extraer ngrok
    Write-Host "Extrayendo ngrok..." -ForegroundColor Yellow
    try {
        Expand-Archive -Path $ngrokZip -DestinationPath $ngrokDir -Force
        Remove-Item $ngrokZip -Force
        Write-Host "[OK] Extraccion completada" -ForegroundColor Green
    } catch {
        Write-Host "[ERROR] Error al extraer ngrok: $_" -ForegroundColor Red
        exit 1
    }
    
    # Agregar al PATH del usuario (solo para esta sesion)
    $env:Path += ";$ngrokDir"
    
    # Verificar que ngrok funciona
    if (Test-Path $ngrokExe) {
        Write-Host "[OK] ngrok instalado correctamente en: $ngrokExe" -ForegroundColor Green
        Write-Host ""
        Write-Host "[IMPORTANTE] Para que ngrok este disponible permanentemente, agrega esta ruta a tu PATH:" -ForegroundColor Yellow
        Write-Host "  $ngrokDir" -ForegroundColor Cyan
        Write-Host ""
    } else {
        Write-Host "[ERROR] Error: ngrok.exe no se encontro despues de la extraccion" -ForegroundColor Red
        exit 1
    }
}

# Verificar authtoken
Write-Host "Verificando configuracion de ngrok..." -ForegroundColor Yellow
$ngrokConfigPath = "$env:USERPROFILE\.ngrok2\ngrok.yml"
$hasAuthtoken = $false

if (Test-Path $ngrokConfigPath) {
    $configContent = Get-Content $ngrokConfigPath -Raw
    if ($configContent -match "authtoken:") {
        $hasAuthtoken = $true
        Write-Host "[OK] Authtoken configurado" -ForegroundColor Green
    }
}

if (-not $hasAuthtoken) {
    Write-Host ""
    Write-Host "[ADVERTENCIA] ngrok requiere un authtoken para funcionar." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Pasos para obtener tu authtoken:" -ForegroundColor Cyan
    Write-Host "1. Ve a: https://dashboard.ngrok.com/signup" -ForegroundColor White
    Write-Host "2. Crea una cuenta gratuita (o inicia sesion)" -ForegroundColor White
    Write-Host "3. Ve a: https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor White
    Write-Host "4. Copia tu authtoken" -ForegroundColor White
    Write-Host ""
    $authtoken = Read-Host "Pega tu authtoken aqui"
    
    if ($authtoken) {
        Write-Host ""
        Write-Host "Configurando authtoken..." -ForegroundColor Yellow
        try {
            if (-not $ngrokPath) {
                & "$ngrokDir\ngrok.exe" config add-authtoken $authtoken
            } else {
                ngrok config add-authtoken $authtoken
            }
            Write-Host "[OK] Authtoken configurado correctamente" -ForegroundColor Green
        } catch {
            Write-Host "[ERROR] Error al configurar authtoken: $_" -ForegroundColor Red
            Write-Host ""
            Write-Host "Configura manualmente ejecutando:" -ForegroundColor Yellow
            Write-Host "  ngrok config add-authtoken TU_AUTHTOKEN" -ForegroundColor Cyan
            exit 1
        }
    } else {
        Write-Host "[ERROR] No se proporciono authtoken. ngrok no funcionara sin el." -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Iniciando ngrok..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si la aplicacion esta corriendo
Write-Host "Verificando si la aplicacion esta corriendo en localhost:56610..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:56610" -Method GET -TimeoutSec 2 -UseBasicParsing -ErrorAction Stop
    Write-Host "[OK] Aplicacion esta corriendo" -ForegroundColor Green
} catch {
    Write-Host "[ADVERTENCIA] No se pudo conectar a localhost:56610" -ForegroundColor Yellow
    Write-Host "   Asegurate de que la aplicacion este corriendo antes de continuar" -ForegroundColor Yellow
    Write-Host ""
    $continuar = Read-Host "Deseas continuar de todas formas? (s/n)"
    if ($continuar -ne "s" -and $continuar -ne "S") {
        exit 1
    }
}

Write-Host ""
Write-Host "[INICIANDO] Iniciando ngrok en puerto 56610..." -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INSTRUCCIONES IMPORTANTES" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. ngrok mostrara una URL como:" -ForegroundColor White
Write-Host "   Forwarding: https://abc123.ngrok-free.app -> http://localhost:56610" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Copia la URL que aparece despues de 'Forwarding:'" -ForegroundColor White
Write-Host ""
Write-Host "3. En n8n, ve a Settings -> Environment Variables" -ForegroundColor White
Write-Host ""
Write-Host "4. Agrega una variable:" -ForegroundColor White
Write-Host "   Name:  BACKEND_URL" -ForegroundColor Cyan
Write-Host "   Value: https://abc123.ngrok-free.app" -ForegroundColor Cyan
Write-Host "   (usa la URL que te dio ngrok)" -ForegroundColor White
Write-Host ""
Write-Host "5. Guarda y prueba el workflow en n8n" -ForegroundColor White
Write-Host ""
Write-Host "6. Tambien puedes ver la URL en: http://localhost:4040" -ForegroundColor Cyan
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Presiona Ctrl+C para detener ngrok" -ForegroundColor Yellow
Write-Host ""

# Iniciar ngrok
if (-not $ngrokPath) {
    Start-Process -FilePath "$ngrokDir\ngrok.exe" -ArgumentList "http", "56610" -NoNewWindow
    Write-Host "[OK] ngrok iniciado. Abre http://localhost:4040 en tu navegador para ver la URL publica." -ForegroundColor Green
} else {
    Start-Process -FilePath "ngrok" -ArgumentList "http", "56610" -NoNewWindow
    Write-Host "[OK] ngrok iniciado. Abre http://localhost:4040 en tu navegador para ver la URL publica." -ForegroundColor Green
}

Write-Host ""
Write-Host "ngrok esta corriendo en segundo plano." -ForegroundColor Green
Write-Host "Para detenerlo, cierra esta ventana o ejecuta: taskkill /F /IM ngrok.exe" -ForegroundColor Yellow
