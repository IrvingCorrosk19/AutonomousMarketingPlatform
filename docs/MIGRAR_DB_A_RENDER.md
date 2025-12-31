# Guía: Migrar Base de Datos Local a Render

## 📋 Requisitos Previos

1. PostgreSQL instalado localmente
2. Acceso a la base de datos local
3. Credenciales de la base de datos de Render

## 🚀 Pasos para Migrar

### Paso 1: Exportar Base de Datos Local

Ejecuta el script de exportación:

```powershell
.\scripts\exportar-db-local.ps1
```

Este script:
- Se conecta a tu base de datos local
- Crea un archivo SQL con todos los datos
- Guarda el archivo como `db_backup_YYYYMMDD_HHMMSS.sql`

**Nota:** El script busca `pg_dump` automáticamente. Si no lo encuentra, asegúrate de que PostgreSQL esté instalado y en el PATH.

### Paso 2: Importar a Render

Ejecuta el script de importación:

```powershell
.\scripts\importar-db-render.ps1
```

O si quieres especificar un archivo específico:

```powershell
.\scripts\importar-db-render.ps1 -backupFile ".\db_backup_20250101_120000.sql"
```

Este script:
- Se conecta a la base de datos de Render
- Importa todos los datos del archivo SQL
- Reemplaza los datos existentes en Render

**⚠️ ADVERTENCIA:** Este proceso eliminará todos los datos existentes en Render y los reemplazará con los datos locales.

## 🔧 Configuración Manual (Alternativa)

Si prefieres hacerlo manualmente:

### Exportar (Manual)

```powershell
$env:PGPASSWORD = "Panama2020$"
pg_dump -h localhost -p 5432 -U postgres -d AutonomousMarketingPlatform --clean --if-exists --create --no-owner --no-privileges -f backup.sql
```

### Importar (Manual)

```powershell
$env:PGPASSWORD = "0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1"
psql -h dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com -p 5432 -U admin -d autonomousmarketingplatform -f backup.sql
```

## 📝 Credenciales de Render

**Host:** `dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com`  
**Port:** `5432`  
**Database:** `autonomousmarketingplatform`  
**Username:** `admin`  
**Password:** `0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1`

**Internal Database URL:**
```
postgresql://admin:0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1@dpg-d5a8afv5r7bs739m2vlg-a/autonomousmarketingplatform
```

**External Database URL:**
```
postgresql://admin:0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1@dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com/autonomousmarketingplatform
```

## ✅ Verificación

Después de importar, verifica que los datos se hayan copiado correctamente:

```powershell
$env:PGPASSWORD = "0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1"
psql -h dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com -p 5432 -U admin -d autonomousmarketingplatform -c "SELECT COUNT(*) FROM \"AspNetUsers\";"
```

## 🐛 Troubleshooting

### Error: "pg_dump no encontrado"
- Asegúrate de que PostgreSQL esté instalado
- Agrega PostgreSQL al PATH o especifica la ruta completa

### Error: "Connection refused"
- Verifica que la base de datos local esté corriendo
- Verifica las credenciales

### Error: "Database does not exist" en Render
- La base de datos debe existir en Render antes de importar
- Render la crea automáticamente si usas el Blueprint

### Error: "Permission denied"
- Verifica que tengas permisos en ambas bases de datos
- En Render, el usuario `admin` tiene todos los permisos

## 📌 Notas Importantes

1. **Tamaño del archivo:** Si tu base de datos es muy grande (>100MB), el proceso puede tardar varios minutos
2. **Conexión estable:** Asegúrate de tener una conexión a internet estable durante la importación
3. **Backup:** Siempre haz un backup antes de importar datos importantes
4. **Migraciones:** Después de importar, puede que necesites ejecutar migraciones de Entity Framework si hay diferencias de esquema

## 🔄 Sincronización Continua

Si necesitas sincronizar datos regularmente, puedes:

1. Crear un script que ejecute ambos procesos
2. Usar herramientas como `pg_dump` y `psql` en un cron job
3. Considerar usar replicación de PostgreSQL (avanzado)

