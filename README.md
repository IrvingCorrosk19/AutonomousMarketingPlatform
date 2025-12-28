# Autonomous Marketing Platform

Plataforma de marketing autónomo con IA - Sistema SaaS Multi-Tenant

## Tecnologías

- **Backend:** ASP.NET Core (.NET 8)
- **Base de datos:** PostgreSQL
- **ORM:** Entity Framework Core
- **Frontend:** Razor Pages / MVC Views
- **UI Framework:** AdminLTE (customizado)

## Arquitectura

El proyecto sigue Clean Architecture con las siguientes capas:

- **Web** - Presentación (Controllers, Views, Razor Pages)
- **Application** - Lógica de negocio (Use Cases, DTOs, Interfaces)
- **Domain** - Entidades y reglas de negocio (Entities, Value Objects, Domain Services)
- **Infrastructure** - Implementaciones técnicas (DbContext, Repositorios, Integraciones externas)

## Características Principales

- ✅ Sistema Multi-Tenant (aislamiento total de datos por empresa)
- ✅ Marketing autónomo 24/7 con IA
- ✅ Generación automática de contenido publicitario
- ✅ Publicación automática en redes sociales
- ✅ Aprendizaje y memoria de preferencias

## Estructura del Proyecto

```
AutonomousMarketingPlatform/
├── src/
│   ├── AutonomousMarketingPlatform.Web/          # Capa de presentación
│   ├── AutonomousMarketingPlatform.Application/  # Capa de aplicación
│   ├── AutonomousMarketingPlatform.Domain/       # Capa de dominio
│   └── AutonomousMarketingPlatform.Infrastructure/ # Capa de infraestructura
├── tests/
└── docs/
```

## Requisitos

- .NET 8 SDK
- PostgreSQL 14+
- Visual Studio 2022 o VS Code

