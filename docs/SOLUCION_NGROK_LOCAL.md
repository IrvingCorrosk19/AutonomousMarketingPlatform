# Solución: Exponer Aplicación Local con ngrok

## 🔴 Problema

- **Tu aplicación:** `localhost:56610` (máquina local)
- **n8n:** `n8n.bashpty.com` (servidor remoto)
- **Error:** n8n no puede acceder a `localhost` de tu máquina

## ✅ Solución: ngrok

### Paso 1: Instalar ngrok

**Windows (con Chocolatey):**
```powershell
choco install ngrok
```

**O descargar manualmente:**
1. Ve a https://ngrok.com/download
2. Descarga ngrok para Windows
3. Extrae el archivo `ngrok.exe` a una carpeta (ej: `C:\ngrok\`)

### Paso 2: Crear cuenta en ngrok (gratis)

1. Ve a https://dashboard.ngrok.com/signup
2. Crea una cuenta gratuita
3. Obtén tu authtoken desde https://dashboard.ngrok.com/get-started/your-authtoken

### Paso 3: Configurar ngrok

```powershell
# Configurar authtoken (solo la primera vez)
ngrok config add-authtoken TU_AUTHTOKEN_AQUI

# Exponer tu aplicación
ngrok http 56610
```

### Paso 4: Obtener la URL pública

ngrok mostrará algo como:
```
Forwarding: https://abc123.ngrok-free.app -> http://localhost:56610
```

**Copia la URL:** `https://abc123.ngrok-free.app`

### Paso 5: Configurar en n8n

1. En n8n, ve a **Settings** → **Environment Variables**
2. Agrega:
   - **Name:** `BACKEND_URL`
   - **Value:** `https://abc123.ngrok-free.app` (la URL que te dio ngrok)

### Paso 6: Probar

1. Asegúrate de que tu aplicación esté corriendo en `localhost:56610`
2. Asegúrate de que ngrok esté corriendo
3. Prueba el workflow en n8n

---

## ⚠️ Notas Importantes

### ngrok Free tiene limitaciones:
- La URL cambia cada vez que reinicias ngrok (a menos que uses cuenta paga)
- Hay límite de conexiones simultáneas
- Para producción, considera usar ngrok Pro o un túnel permanente

### Alternativa: ngrok con dominio fijo (pago)

Si necesitas una URL fija:
1. Compra ngrok Pro
2. Configura un dominio fijo
3. Usa esa URL en `BACKEND_URL`

---

## 🧪 Verificación

1. **Desde tu máquina:**
   ```powershell
   curl http://localhost:56610/api/consents/check?tenantId=test&userId=test
   ```

2. **Desde internet (usando la URL de ngrok):**
   ```powershell
   curl https://abc123.ngrok-free.app/api/consents/check?tenantId=test&userId=test
   ```

Si ambos funcionan, está listo.

---

## 📋 Script Automático (Opcional)

Puedo crear un script PowerShell que:
1. Inicia tu aplicación
2. Inicia ngrok
3. Muestra la URL para copiar

¿Quieres que lo cree?

