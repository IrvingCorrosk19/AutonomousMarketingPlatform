# Migraciones de Base de Datos

## Crear una Nueva Migración

```bash
cd src/AutonomousMarketingPlatform.Infrastructure
dotnet ef migrations add NombreDeLaMigracion --startup-project ../AutonomousMarketingPlatform.Web
```

## Aplicar Migraciones

```bash
dotnet ef database update --startup-project ../AutonomousMarketingPlatform.Web
```

## Generar Script SQL

```bash
dotnet ef migrations script --startup-project ../AutonomousMarketingPlatform.Web
```

## Revertir Migración

```bash
dotnet ef database update NombreMigracionAnterior --startup-project ../AutonomousMarketingPlatform.Web
```

