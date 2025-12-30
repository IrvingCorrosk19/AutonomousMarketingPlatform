# Script para iniciar n8n
# Ejecutar: .\scripts\iniciar-n8n.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Iniciando n8n" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si n8n está instalado
try {
    $n8nVersion = n8n --version 2>&1
    Write-Host "✅ n8n encontrado: $n8nVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ n8n no está instalado" -ForegroundColor Red
    Write-Host "Ejecuta primero: .\scripts\instalar-n8n.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Iniciando n8n en http://localhost:5678" -ForegroundColor Yellow
Write-Host "Presiona Ctrl+C para detener n8n" -ForegroundColor Yellow
Write-Host ""
Write-Host "Una vez iniciado, abre tu navegador en:" -ForegroundColor Cyan
Write-Host "http://localhost:5678" -ForegroundColor White
Write-Host ""

# Iniciar n8n
n8n start

