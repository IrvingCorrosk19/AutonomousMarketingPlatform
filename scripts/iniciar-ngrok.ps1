# Script para iniciar ngrok y exponer la aplicación local
# Uso: .\scripts\iniciar-ngrok.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Iniciando ngrok para exponer aplicación" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si ngrok está instalado
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue

if (-not $ngrokPath) {
    Write-Host "❌ ngrok no está instalado." -ForegroundColor Red
    Write-Host ""
    Write-Host "Opciones para instalar:" -ForegroundColor Yellow
    Write-Host "1. Con Chocolatey: choco install ngrok" -ForegroundColor Yellow
    Write-Host "2. Descargar desde: https://ngrok.com/download" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Después de instalar, configura tu authtoken:" -ForegroundColor Yellow
    Write-Host "  ngrok config add-authtoken TU_AUTHTOKEN" -ForegroundColor Yellow
    Write-Host "  (Obtén tu authtoken en: https://dashboard.ngrok.com/get-started/your-authtoken)" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ ngrok encontrado" -ForegroundColor Green
Write-Host ""

# Verificar si la aplicación está corriendo
Write-Host "Verificando si la aplicación está corriendo en localhost:56610..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:56610" -Method GET -TimeoutSec 2 -UseBasicParsing -ErrorAction Stop
    Write-Host "✅ Aplicación está corriendo" -ForegroundColor Green
} catch {
    Write-Host "⚠️  No se pudo conectar a localhost:56610" -ForegroundColor Yellow
    Write-Host "   Asegúrate de que la aplicación esté corriendo antes de iniciar ngrok" -ForegroundColor Yellow
    Write-Host ""
    $continuar = Read-Host "¿Deseas continuar de todas formas? (s/n)"
    if ($continuar -ne "s" -and $continuar -ne "S") {
        exit 1
    }
}

Write-Host ""
Write-Host "Iniciando ngrok en puerto 56610..." -ForegroundColor Yellow
Write-Host "⚠️  IMPORTANTE: Copia la URL 'Forwarding' que aparece y configúrala en n8n como BACKEND_URL" -ForegroundColor Yellow
Write-Host ""
Write-Host "Ejemplo de URL que verás:" -ForegroundColor Cyan
Write-Host "  Forwarding: https://abc123.ngrok-free.app -> http://localhost:56610" -ForegroundColor Cyan
Write-Host ""
Write-Host "En n8n, configura:" -ForegroundColor Cyan
Write-Host "  BACKEND_URL = https://abc123.ngrok-free.app" -ForegroundColor Cyan
Write-Host ""
Write-Host "Presiona Ctrl+C para detener ngrok" -ForegroundColor Yellow
Write-Host ""

# Iniciar ngrok
Start-Process -FilePath "ngrok" -ArgumentList "http", "56610" -NoNewWindow

Write-Host "ngrok iniciado. Abre http://localhost:4040 en tu navegador para ver la URL pública." -ForegroundColor Green

