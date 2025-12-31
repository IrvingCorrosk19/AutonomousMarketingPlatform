# Script para importar la base de datos a Render
# Uso: .\scripts\importar-db-render.ps1 [archivo.sql]

param(
    [string]$backupFile = "",
    [switch]$Force
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Importando Base de Datos a Render" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuración de la base de datos de Render
$renderHost = "dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com"
$renderPort = "5432"
$renderDatabase = "autonomousmarketingplatform"
$renderUser = "admin"
$renderPassword = "0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1"

Write-Host "Configuracion de la base de datos de Render:" -ForegroundColor Yellow
Write-Host "  Host: $renderHost" -ForegroundColor White
Write-Host "  Port: $renderPort" -ForegroundColor White
Write-Host "  Database: $renderDatabase" -ForegroundColor White
Write-Host "  User: $renderUser" -ForegroundColor White
Write-Host ""

# Buscar archivo de backup si no se especificó
if ([string]::IsNullOrEmpty($backupFile)) {
    Write-Host "Buscando archivo de backup mas reciente..." -ForegroundColor Yellow
    $backupFiles = Get-ChildItem -Path (Join-Path $PSScriptRoot "..") -Filter "db_backup_*.sql" | Sort-Object LastWriteTime -Descending
    
    if ($backupFiles.Count -eq 0) {
        Write-Host "[ERROR] No se encontro ningun archivo de backup." -ForegroundColor Red
        Write-Host ""
        Write-Host "Ejecuta primero: .\scripts\exportar-db-local.ps1" -ForegroundColor Yellow
        exit 1
    }
    
    $backupFile = $backupFiles[0].FullName
    Write-Host "[OK] Archivo encontrado: $backupFile" -ForegroundColor Green
} else {
    if (-not (Test-Path $backupFile)) {
        Write-Host "[ERROR] El archivo especificado no existe: $backupFile" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Archivo a importar: $backupFile" -ForegroundColor Cyan
Write-Host "Tamanio: $((Get-Item $backupFile).Length / 1MB) MB" -ForegroundColor Cyan
Write-Host ""

# Verificar si psql está disponible
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue
if (-not $psqlPath) {
    # Intentar encontrar psql en la ruta común de PostgreSQL
    $commonPaths = @(
        "C:\Program Files\PostgreSQL\18\bin\psql.exe",
        "C:\Program Files\PostgreSQL\17\bin\psql.exe",
        "C:\Program Files\PostgreSQL\16\bin\psql.exe",
        "C:\Program Files\PostgreSQL\15\bin\psql.exe",
        "C:\Program Files\PostgreSQL\14\bin\psql.exe"
    )
    
    $psqlPath = $null
    foreach ($path in $commonPaths) {
        if (Test-Path $path) {
            $psqlPath = $path
            break
        }
    }
    
    if (-not $psqlPath) {
        Write-Host "[ERROR] psql no encontrado." -ForegroundColor Red
        Write-Host ""
        Write-Host "Por favor, asegurate de que PostgreSQL este instalado y psql este en el PATH." -ForegroundColor Yellow
        Write-Host "O especifica la ruta completa a psql.exe" -ForegroundColor Yellow
        exit 1
    }
} else {
    $psqlPath = $psqlPath.Source
}

Write-Host "[OK] psql encontrado en: $psqlPath" -ForegroundColor Green
Write-Host ""

# Verificar conexión a Render
Write-Host "Verificando conexion a Render..." -ForegroundColor Yellow
$env:PGPASSWORD = $renderPassword
try {
    $testConnection = & $psqlPath -h $renderHost -p $renderPort -U $renderUser -d $renderDatabase -c "SELECT version();" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Conexion exitosa a Render" -ForegroundColor Green
    } else {
        Write-Host "[ERROR] No se pudo conectar a Render" -ForegroundColor Red
        Write-Host "Salida: $testConnection" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "[ERROR] Error al conectar: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Confirmar antes de importar
Write-Host "[ADVERTENCIA] Esto eliminara todos los datos existentes en Render y los reemplazara con los datos locales." -ForegroundColor Yellow
Write-Host ""

if (-not $Force) {
    $confirmar = Read-Host "¿Deseas continuar? (escribe 'SI' para confirmar)"
    if ($confirmar -ne "SI") {
        Write-Host "Operacion cancelada." -ForegroundColor Yellow
        exit 0
    }
} else {
    Write-Host "[INFO] Modo Force activado, continuando sin confirmacion..." -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Importando base de datos..." -ForegroundColor Yellow
Write-Host "Esto puede tardar varios minutos dependiendo del tamanio de la base de datos..." -ForegroundColor Yellow
Write-Host ""

try {
    $env:PGPASSWORD = $renderPassword
    
    # Leer el archivo SQL y ejecutarlo
    # Nota: pg_dump con --create genera comandos CREATE DATABASE que pueden fallar
    # Vamos a usar psql directamente para importar
    
    Write-Host "Ejecutando importacion..." -ForegroundColor Yellow
    
    # Usar psql para importar el archivo
    $importResult = & $psqlPath `
        -h $renderHost `
        -p $renderPort `
        -U $renderUser `
        -d $renderDatabase `
        -f $backupFile `
        2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "[OK] Importacion completada exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "La base de datos ha sido copiada a Render." -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Siguiente paso: Verifica que la aplicacion en Render este funcionando correctamente." -ForegroundColor Yellow
    } else {
        Write-Host ""
        Write-Host "[ADVERTENCIA] La importacion puede haber tenido algunos errores." -ForegroundColor Yellow
        Write-Host "Codigo de salida: $LASTEXITCODE" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Salida:" -ForegroundColor Yellow
        Write-Host $importResult -ForegroundColor White
        Write-Host ""
        Write-Host "Nota: Algunos errores pueden ser normales (ej: 'database already exists')." -ForegroundColor Cyan
        Write-Host "Verifica manualmente que los datos se hayan importado correctamente." -ForegroundColor Cyan
    }
} catch {
    Write-Host "[ERROR] Error al importar: $_" -ForegroundColor Red
    exit 1
} finally {
    $env:PGPASSWORD = $null
}

