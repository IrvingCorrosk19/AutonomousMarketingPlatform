# Script completo para migrar la base de datos local a Render
# Uso: .\scripts\migrar-db-completo.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Migracion Completa: Local -> Render" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Exportar
Write-Host "PASO 1: Exportando base de datos local..." -ForegroundColor Yellow
Write-Host ""
& "$PSScriptRoot\exportar-db-local.ps1"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] La exportacion fallo. Abortando." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paso 2: Importar
Write-Host "PASO 2: Importando a Render..." -ForegroundColor Yellow
Write-Host ""
& "$PSScriptRoot\importar-db-render.ps1"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[ERROR] La importacion fallo." -ForegroundColor Red
    Write-Host "Revisa los errores anteriores." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "[OK] Migracion completada!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Siguiente paso: Verifica que la aplicacion en Render este funcionando." -ForegroundColor Yellow

