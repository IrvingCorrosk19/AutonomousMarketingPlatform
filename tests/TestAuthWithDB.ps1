# Script simplificado para verificar BD y ejecutar pruebas de autenticación
$psqlPath = "C:\Program Files\PostgreSQL\18\bin\psql.exe"
$connString = "postgresql://postgres:Panama2020$@localhost:5432/AutonomousMarketingPlatform"

Write-Host "=== VERIFICACIÓN DE BASE DE DATOS ===" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $psqlPath)) {
    Write-Host "ERROR: PostgreSQL no encontrado" -ForegroundColor Red
    exit 1
}

# Consultar usuarios
Write-Host "Usuarios en el sistema:" -ForegroundColor Yellow
& $psqlPath -d $connString -c "SELECT email, \"FullName\", \"IsActive\", \"FailedLoginAttempts\", \"LastLoginAt\" FROM \"AspNetUsers\" ORDER BY email;" 2>&1

Write-Host ""
Write-Host "Tenants en el sistema:" -ForegroundColor Yellow
& $psqlPath -d $connString -c "SELECT id, \"Name\", \"Subdomain\" FROM \"Tenants\" ORDER BY \"Name\";" 2>&1

Write-Host ""
Write-Host "=== LISTO PARA PRUEBAS ===" -ForegroundColor Green

