# Script para ejecutar pruebas de autenticación y verificar en BD
# Requiere PostgreSQL instalado

$connectionString = "Host=localhost;Port=5432;Database=AutonomousMarketingPlatform;Username=postgres;Password=Panama2020$"
$psqlPath = "C:\Program Files\PostgreSQL\18\bin\psql.exe"

if (-not (Test-Path $psqlPath)) {
    Write-Host "ERROR: PostgreSQL no encontrado en $psqlPath" -ForegroundColor Red
    exit 1
}

Write-Host "=== VERIFICACIÓN DE BASE DE DATOS ===" -ForegroundColor Cyan
Write-Host ""

# Función para ejecutar query SQL
function Invoke-PostgresQuery {
    param(
        [string]$Query,
        [string]$Description
    )
    
    Write-Host "$Description..." -NoNewline
    try {
        $result = & $psqlPath -c $Query $connectionString 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host " OK" -ForegroundColor Green
            return $result
        } else {
            Write-Host " ERROR" -ForegroundColor Red
            Write-Host $result -ForegroundColor Yellow
            return $null
        }
    } catch {
        Write-Host " ERROR: $_" -ForegroundColor Red
        return $null
    }
}

# Verificar usuarios existentes
Write-Host "1. Verificando usuarios existentes..." -ForegroundColor Cyan
$usersQuery = "SELECT id, email, \"FullName\", \"TenantId\", \"IsActive\", \"FailedLoginAttempts\", \"LockoutEndDate\", \"LastLoginAt\", \"LastLoginIp\" FROM \"AspNetUsers\" ORDER BY email;"
$users = Invoke-PostgresQuery -Query $usersQuery -Description "Consultando usuarios"

# Verificar tenants
Write-Host "2. Verificando tenants existentes..." -ForegroundColor Cyan
$tenantsQuery = "SELECT id, \"Name\", \"Subdomain\", \"ContactEmail\", \"IsActive\" FROM \"Tenants\" ORDER BY \"Name\";"
$tenants = Invoke-PostgresQuery -Query $tenantsQuery -Description "Consultando tenants"

# Verificar roles
Write-Host "3. Verificando roles existentes..." -ForegroundColor Cyan
$rolesQuery = "SELECT id, \"Name\" FROM \"AspNetRoles\" ORDER BY \"Name\";"
$roles = Invoke-PostgresQuery -Query $rolesQuery -Description "Consultando roles"

Write-Host ""
Write-Host "=== RESUMEN ===" -ForegroundColor Cyan
Write-Host "Usuarios encontrados:"
if ($users) {
    $users | Select-String -Pattern "email" | ForEach-Object { Write-Host "  $_" }
} else {
    Write-Host "  No se pudo consultar usuarios" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Para crear usuarios de prueba, usar el módulo de gestión de usuarios en la aplicación web."
Write-Host "URL: http://localhost:56610/Users"

