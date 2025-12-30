# Script para instalar y configurar n8n
# Ejecutar: .\scripts\instalar-n8n.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Instalación y Configuración de n8n" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar Node.js
Write-Host "Verificando Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js instalado: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Node.js no está instalado" -ForegroundColor Red
    Write-Host "Por favor, instala Node.js desde https://nodejs.org/" -ForegroundColor Yellow
    exit 1
}

# Verificar npm
Write-Host "Verificando npm..." -ForegroundColor Yellow
try {
    $npmVersion = npm --version
    Write-Host "✅ npm instalado: $npmVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ npm no está disponible" -ForegroundColor Red
    exit 1
}

# Verificar si n8n ya está instalado
Write-Host ""
Write-Host "Verificando si n8n está instalado..." -ForegroundColor Yellow
try {
    $n8nVersion = n8n --version 2>&1
    Write-Host "✅ n8n ya está instalado: $n8nVersion" -ForegroundColor Green
    $n8nInstalado = $true
} catch {
    Write-Host "⚠️  n8n no está instalado" -ForegroundColor Yellow
    $n8nInstalado = $false
}

# Instalar n8n si no está instalado
if (-not $n8nInstalado) {
    Write-Host ""
    Write-Host "Instalando n8n globalmente..." -ForegroundColor Yellow
    Write-Host "Esto puede tomar varios minutos..." -ForegroundColor Yellow
    
    npm install n8n -g
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ n8n instalado correctamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error al instalar n8n" -ForegroundColor Red
        exit 1
    }
}

# Verificar instalación
Write-Host ""
Write-Host "Verificando instalación..." -ForegroundColor Yellow
try {
    $n8nVersion = n8n --version 2>&1
    Write-Host "✅ n8n instalado correctamente: $n8nVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Error al verificar n8n" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Instalación Completada" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Yellow
Write-Host "1. Iniciar n8n: n8n start" -ForegroundColor White
Write-Host "2. Abrir navegador en: http://localhost:5678" -ForegroundColor White
Write-Host "3. Crear cuenta inicial" -ForegroundColor White
Write-Host "4. Importar workflows desde: workflows/n8n/" -ForegroundColor White
Write-Host ""
Write-Host "¿Deseas iniciar n8n ahora? (S/N)" -ForegroundColor Yellow
$respuesta = Read-Host

if ($respuesta -eq "S" -or $respuesta -eq "s") {
    Write-Host ""
    Write-Host "Iniciando n8n..." -ForegroundColor Yellow
    Write-Host "Presiona Ctrl+C para detener n8n" -ForegroundColor Yellow
    Write-Host ""
    n8n start
}

