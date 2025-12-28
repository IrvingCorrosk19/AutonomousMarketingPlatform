# Prompts de Arquitectura - Autonomous Marketing Platform

**Fecha:** 27 de enero de 2025  
**Proyecto:** Autonomous Marketing Platform  
**Tecnología:** ASP.NET Core (.NET 8), PostgreSQL, Clean Architecture

---

## 🔹 PROMPT 1 – CONTEXTO GENERAL DEL SISTEMA (OBLIGATORIO)

Actúa como un arquitecto de software senior experto en ASP.NET Core (.NET 8), Clean Architecture, PostgreSQL, sistemas SaaS multi-empresa (multi-tenant) y plataformas de marketing autónomo con IA.

Quiero construir una aplicación WEB llamada provisionalmente **"Autonomous Marketing Platform"**.

### Tecnologías obligatorias:
- Backend: ASP.NET Core (.NET 8)
- Base de datos: PostgreSQL
- ORM: Entity Framework Core
- Frontend: Razor Pages / MVC Views
- CMS UI base: AdminLTE (customizado, NO diseño default)

### Objetivo del sistema:
Crear un sistema serio de marketing y publicidad autónoma donde el usuario:
- Carga imágenes o videos de referencia desde la web
- Autoriza el uso de IA
- El sistema genera automáticamente:
  - Estrategia de marketing
  - Contenido publicitario
  - Imágenes
  - Reels / videos cortos
  - Copy publicitario
  - Publicación automática en redes
- El sistema aprende y recuerda preferencias y conversaciones

### Requisitos clave:
- Sistema SaaS multi-empresa (multi-tenant desde el inicio)
- Aislamiento total de datos por empresa
- Consentimiento explícito del usuario
- Marketing autónomo 24/7
- Arquitectura limpia y mantenible
- El frontend NO contiene lógica pesada
- La IA y automatizaciones viven en backend / servicios externos

**Guíame paso a paso y genera código profesional, escalable y bien documentado.**

---

## 🔹 PROMPT 2 – ARQUITECTURA .NET CORE + MULTI-TENANT

Diseña la arquitectura del sistema usando Clean Architecture en ASP.NET Core.

### Incluye claramente las capas:
- API / Web (Controllers, Views)
- Application (Use Cases)
- Domain (Entities, Value Objects, Interfaces)
- Infrastructure (DbContext, Repositorios, Integraciones)

### Explica con detalle:
- Estructura de carpetas del proyecto
- Responsabilidad de cada capa
- Cómo se implementa multi-tenant (tenant_id obligatorio)
- Cómo se evita mezclar datos entre empresas
- Cómo se maneja autenticación por empresa
- Cómo se prepara el sistema para crecer como SaaS

**NO generes UI todavía.**

---

## 🔹 PROMPT 3 – MODELO DE DATOS (POSTGRESQL + MULTI-EMPRESA)

Diseña el modelo de base de datos en PostgreSQL para este sistema.

### Debe incluir al menos:
- Tenants (empresas)
- Usuarios
- Consentimientos
- Campañas
- Contenido cargado
- Preferencias del usuario
- Memoria de marketing
- Estados de automatización

### Reglas:
- TODAS las tablas deben estar asociadas a tenant_id
- Explica claves primarias y foráneas
- Explica por qué cada tabla existe
- Pensar en crecimiento y analítica futura

---

## 🔹 PROMPT 4 – ADMINLTE COMO CMS (PERO CUSTOMIZADO)

Usa AdminLTE como base del CMS, pero NO como diseño final.

### Objetivo:
Transformar AdminLTE en un CMS moderno, sobrio y profesional.

### Indica:
- Qué partes de AdminLTE usar
- Qué partes ocultar o eliminar
- Cómo organizar:
  - Sidebar
  - Navbar
  - Dashboard
  - Módulos

### El CMS debe sentirse:
- Corporativo
- Moderno
- No genérico
- No "plantilla gratis"

**Explica la estructura de vistas Razor.**

---

## 🔹 PROMPT 5 – DISEÑO Y CSS (ROMPER EL LOOK ADMINLTE)

Actúa como diseñador UX/UI senior especializado en SaaS B2B.

Propón un diseño visual para customizar AdminLTE usando CSS propio.

### Objetivo del diseño:
- Profesional
- Sobrio
- Minimalista
- Enfocado en confianza
- Enfocado en control y automatización
- Estilo "producto serio", no marketing barato

### Incluye:
- Paleta de colores recomendada
- Tipografías
- Espaciados
- Cards
- Estados visuales
- Uso de sombras y bordes
- Cómo evitar que se vea como AdminLTE default

**Genera un archivo CSS base que pueda sobreescribir AdminLTE.**

---

## Notas del Proyecto

Este documento contiene los prompts base para la construcción de la plataforma de marketing autónomo. Cada prompt debe ser ejecutado en orden para construir el sistema de manera estructurada y escalable.

