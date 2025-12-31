# Guía de Despliegue en Render

## 📋 Requisitos Previos

1. Cuenta en Render (https://render.com)
2. Repositorio Git (GitHub, GitLab, o Bitbucket)
3. Base de datos PostgreSQL (Render puede crear una automáticamente)

## 🚀 Pasos para Desplegar

### 1. Preparar el Repositorio

Asegúrate de que tu código esté en un repositorio Git y que `render.yaml` esté en la raíz del proyecto.

### 2. Crear Servicio en Render

1. Ve a https://dashboard.render.com
2. Click en **"New +"** → **"Blueprint"**
3. Conecta tu repositorio Git
4. Render detectará automáticamente el archivo `render.yaml`

### 3. Configurar Variables de Entorno

En el dashboard de Render, configura las siguientes variables de entorno:

#### Obligatorias:

- **`ConnectionStrings__DefaultConnection`**
  - Valor: La cadena de conexión a PostgreSQL que Render genera automáticamente
  - Formato: `Host=xxx.xxx.xxx.xxx;Port=5432;Database=xxx;Username=xxx;Password=xxx;SSL Mode=Require;`
  - Render puede generar esto automáticamente si conectas la base de datos

- **`Encryption__Key`**
  - Valor: Una clave de 32 caracteres para encriptación
  - Genera una clave segura: `openssl rand -base64 32`
  - Ejemplo: `K8j3mN9pQ2rT5vX7zA1bC4dE6fG8hI0j`

#### Opcionales (si usas OpenAI):

- **`AI__OpenAI__ApiKey`**
  - Valor: Tu API key de OpenAI
  - Obtén en: https://platform.openai.com/api-keys

### 4. Configurar Base de Datos PostgreSQL

1. En Render, crea una nueva base de datos PostgreSQL
2. Render generará automáticamente la cadena de conexión
3. Copia la cadena de conexión y úsala en `ConnectionStrings__DefaultConnection`

### 5. Ejecutar Migraciones

Después del primer despliegue, necesitas ejecutar las migraciones de Entity Framework:

**Opción A: Desde el Dashboard de Render**
1. Ve a tu servicio web
2. Click en **"Shell"**
3. Ejecuta:
   ```bash
   cd publish
   dotnet ef database update --project ../src/AutonomousMarketingPlatform.Infrastructure --startup-project ../src/AutonomousMarketingPlatform.Web
   ```

**Opción B: Agregar comando de build**
Modifica `render.yaml` para ejecutar migraciones automáticamente:
```yaml
buildCommand: dotnet publish -c Release -o ./publish && dotnet ef database update --project ./src/AutonomousMarketingPlatform.Infrastructure --startup-project ./src/AutonomousMarketingPlatform.Web
```

### 6. Configurar n8n

Una vez que tu aplicación esté desplegada en Render, obtendrás una URL pública como:
- `https://autonomous-marketing-platform.onrender.com`

**Configura en n8n:**
1. Ve a n8n → Settings → Environment Variables
2. Agrega:
   - **Name:** `BACKEND_URL`
   - **Value:** `https://autonomous-marketing-platform.onrender.com` (tu URL de Render)

### 7. Verificar Despliegue

1. Visita la URL de tu aplicación en Render
2. Verifica que la aplicación carga correctamente
3. Prueba crear una campaña
4. Prueba el flujo de marketing request desde n8n

## 🔧 Configuración Adicional

### Dominio Personalizado

1. En Render, ve a tu servicio web
2. Click en **"Settings"** → **"Custom Domains"**
3. Agrega tu dominio personalizado
4. Sigue las instrucciones para configurar DNS

### SSL/HTTPS

Render proporciona SSL automáticamente para todos los servicios. No necesitas configuración adicional.

### Variables de Entorno Importantes

| Variable | Descripción | Requerida |
|----------|-------------|-----------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a PostgreSQL | ✅ Sí |
| `Encryption__Key` | Clave de encriptación (32 caracteres) | ✅ Sí |
| `AI__OpenAI__ApiKey` | API key de OpenAI | ❌ No (si usas mock) |
| `ASPNETCORE_ENVIRONMENT` | Entorno (Production) | ✅ Sí (automático) |
| `PORT` | Puerto (Render lo asigna automáticamente) | ✅ Sí (automático) |

## 📝 Notas Importantes

1. **Primera vez:** El despliegue puede tardar 5-10 minutos
2. **Base de datos:** Asegúrate de que la base de datos esté creada antes de desplegar
3. **Migraciones:** Ejecuta las migraciones después del primer despliegue
4. **n8n:** Actualiza `BACKEND_URL` en n8n con la URL de Render
5. **Logs:** Revisa los logs en Render si hay problemas

## 🐛 Troubleshooting

### Error: "Connection string not found"
- Verifica que `ConnectionStrings__DefaultConnection` esté configurada en Render
- Asegúrate de que la base de datos esté conectada al servicio web

### Error: "Database migration failed"
- Ejecuta las migraciones manualmente desde el Shell de Render
- Verifica que la base de datos tenga los permisos correctos

### Error: "Port already in use"
- Render asigna el puerto automáticamente, no lo configures manualmente
- Usa `$PORT` en la configuración (ya está configurado en `render.yaml`)

### La aplicación no responde
- Verifica los logs en Render
- Asegúrate de que el build fue exitoso
- Verifica que el comando de inicio sea correcto

## 🔗 Enlaces Útiles

- Render Dashboard: https://dashboard.render.com
- Documentación de Render: https://render.com/docs
- Guía de .NET en Render: https://render.com/docs/deploy-dotnet

