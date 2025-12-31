# Script para exportar la base de datos local
# Uso: .\scripts\exportar-db-local.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Exportando Base de Datos Local" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuración de la base de datos local
$localHost = "localhost"
$localPort = "5432"
$localDatabase = "AutonomousMarketingPlatform"
$localUser = "postgres"
$localPassword = "Panama2020$"

# Archivo de salida
$outputFile = "db_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql"
$outputDir = Join-Path $PSScriptRoot ".."
$outputPath = Join-Path $outputDir $outputFile

Write-Host "Configuracion de la base de datos local:" -ForegroundColor Yellow
Write-Host "  Host: $localHost" -ForegroundColor White
Write-Host "  Port: $localPort" -ForegroundColor White
Write-Host "  Database: $localDatabase" -ForegroundColor White
Write-Host "  User: $localUser" -ForegroundColor White
Write-Host ""

# Verificar si pg_dump está disponible
$pgDumpPath = Get-Command pg_dump -ErrorAction SilentlyContinue
if (-not $pgDumpPath) {
    # Intentar encontrar pg_dump en la ruta común de PostgreSQL
    $commonPaths = @(
        "C:\Program Files\PostgreSQL\18\bin\pg_dump.exe",
        "C:\Program Files\PostgreSQL\17\bin\pg_dump.exe",
        "C:\Program Files\PostgreSQL\16\bin\pg_dump.exe",
        "C:\Program Files\PostgreSQL\15\bin\pg_dump.exe",
        "C:\Program Files\PostgreSQL\14\bin\pg_dump.exe"
    )
    
    $pgDumpPath = $null
    foreach ($path in $commonPaths) {
        if (Test-Path $path) {
            $pgDumpPath = $path
            break
        }
    }
    
    if (-not $pgDumpPath) {
        Write-Host "[ERROR] pg_dump no encontrado." -ForegroundColor Red
        Write-Host ""
        Write-Host "Por favor, asegurate de que PostgreSQL este instalado y pg_dump este en el PATH." -ForegroundColor Yellow
        Write-Host "O especifica la ruta completa a pg_dump.exe" -ForegroundColor Yellow
        exit 1
    }
} else {
    $pgDumpPath = $pgDumpPath.Source
}

Write-Host "[OK] pg_dump encontrado en: $pgDumpPath" -ForegroundColor Green
Write-Host ""

# Verificar conexión a la base de datos
Write-Host "Verificando conexion a la base de datos local..." -ForegroundColor Yellow
$env:PGPASSWORD = $localPassword
try {
    $testConnection = & psql -h $localHost -p $localPort -U $localUser -d $localDatabase -c "SELECT 1;" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Conexion exitosa" -ForegroundColor Green
    } else {
        Write-Host "[ADVERTENCIA] No se pudo verificar la conexion, pero continuaremos..." -ForegroundColor Yellow
    }
} catch {
    Write-Host "[ADVERTENCIA] No se pudo verificar la conexion, pero continuaremos..." -ForegroundColor Yellow
}
Write-Host ""

# Exportar la base de datos
Write-Host "Exportando base de datos a: $outputPath" -ForegroundColor Yellow
Write-Host "Esto puede tardar varios minutos..." -ForegroundColor Yellow
Write-Host ""

try {
    $env:PGPASSWORD = $localPassword
    
    # Usar pg_dump con opciones para crear un dump completo (sin --create para importar a DB existente)
    & $pgDumpPath `
        -h $localHost `
        -p $localPort `
        -U $localUser `
        -d $localDatabase `
        --clean `
        --if-exists `
        --no-owner `
        --no-privileges `
        --verbose `
        -f $outputPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "[OK] Exportacion completada exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Archivo generado: $outputPath" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Tamanio del archivo: $((Get-Item $outputPath).Length / 1MB) MB" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Siguiente paso: Ejecuta .\scripts\importar-db-render.ps1" -ForegroundColor Yellow
    } else {
        Write-Host "[ERROR] Error al exportar la base de datos. Codigo de salida: $LASTEXITCODE" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "[ERROR] Error al exportar: $_" -ForegroundColor Red
    exit 1
} finally {
    $env:PGPASSWORD = $null
}

