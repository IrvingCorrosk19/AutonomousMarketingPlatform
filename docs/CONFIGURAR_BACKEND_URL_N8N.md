# 🔧 Configurar BACKEND_URL en n8n Self-Hosted

## Tu Configuración Actual

- **Aplicación ASP.NET Core:** `https://autonomousmarketingplatform.onrender.com`
- **n8n:** `https://n8n.bashpty.com` (self-hosted)

## ✅ Solución: Configurar Variable de Entorno en n8n

### Opción 1: Variables de Entorno del Sistema (Recomendado)

Si n8n está corriendo en Docker o como servicio del sistema:

#### Docker Compose

Edita tu `docker-compose.yml` o archivo de configuración de Docker:

```yaml
services:
  n8n:
    image: n8nio/n8n
    environment:
      - BACKEND_URL=https://autonomousmarketingplatform.onrender.com
      # ... otras variables
```

Luego reinicia:
```bash
docker-compose down
docker-compose up -d
```

#### Servicio del Sistema (systemd)

Si n8n corre como servicio:

1. Edita el archivo de servicio (ej: `/etc/systemd/system/n8n.service`):
```ini
[Service]
Environment="BACKEND_URL=https://autonomousmarketingplatform.onrender.com"
```

2. Recarga y reinicia:
```bash
sudo systemctl daemon-reload
sudo systemctl restart n8n
```

#### Variables de Entorno del Host

Si n8n lee variables del sistema:

```bash
export BACKEND_URL=https://autonomousmarketingplatform.onrender.com
```

Agrega esto a tu `~/.bashrc` o `~/.profile` para que persista.

### Opción 2: Variables de Entorno en n8n (UI)

1. Inicia sesión en `https://n8n.bashpty.com`
2. Ve a **Settings** → **Environment Variables** (o **Variables**)
3. Agrega:
   - **Name:** `BACKEND_URL`
   - **Value:** `https://autonomousmarketingplatform.onrender.com`
4. Guarda y reinicia n8n

### Opción 3: Archivo `.env` (Si n8n lo soporta)

Si n8n está configurado para leer un archivo `.env`:

1. Crea o edita el archivo `.env` en el directorio de n8n:
```env
BACKEND_URL=https://autonomousmarketingplatform.onrender.com
```

2. Reinicia n8n

## 🔍 Verificar que Funciona

1. En n8n, abre tu workflow `01-trigger-marketing-request.json`
2. Ve al nodo **"HTTP Request - Check Consents"**
3. La URL debería resolverse a:
   ```
   https://autonomousmarketingplatform.onrender.com/api/ConsentsApi/check
   ```
4. Ejecuta una prueba del workflow
5. Verifica que no haya errores de conexión

## ⚠️ Nota Importante

El workflow ya tiene un fallback:
- Si `BACKEND_URL` está configurada → usa esa URL
- Si no está configurada o es `http://localhost:5000` → usa `https://autonomousmarketingplatform.onrender.com`

Pero es mejor configurar `BACKEND_URL` explícitamente para evitar confusiones.

## 🆘 Solución de Problemas

### Error: "ECONNREFUSED" o "Connection refused"

- Verifica que `BACKEND_URL` esté configurada correctamente
- Verifica que la URL de Render esté accesible desde el servidor de n8n
- Prueba hacer un `curl` desde el servidor de n8n:
  ```bash
  curl https://autonomousmarketingplatform.onrender.com/api/ConsentsApi/check?tenantId=test&userId=test
  ```

### Error: "400 Bad Request" con valores vacíos

- Ya está resuelto con el cambio en el nodo HTTP Request
- Verifica que los parámetros usen: `{{ $node["Set Validated Data"].json.tenantId }}`

