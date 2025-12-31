# Problema: n8n Remoto no puede acceder a localhost

## 🔴 Problema Identificado

### Situación Actual
- **n8n está en:** `n8n.bashpty.com` (servidor remoto)
- **Tu aplicación está en:** `localhost:56610` (tu máquina local)
- **Error:** `ECONNREFUSED 127.0.0.1:56610`

### ¿Por qué falla?
Cuando n8n (que está en un servidor remoto) intenta conectarse a `localhost` o `127.0.0.1`, está intentando conectarse a **su propio servidor**, no a tu máquina local.

```
n8n (n8n.bashpty.com) → intenta → localhost:56610
                          ↓
                    ❌ Se conecta a SÍ MISMO
                    ❌ No encuentra tu aplicación
```

---

## ✅ Soluciones

### Opción 1: Configurar BACKEND_URL en n8n (RECOMENDADO)

**Pasos:**
1. En n8n, ve a **Settings** → **Environment Variables**
2. Agrega una variable:
   - **Name:** `BACKEND_URL`
   - **Value:** La URL pública de tu aplicación
     - Si usas ngrok: `https://tu-subdominio.ngrok.io`
     - Si tienes dominio público: `https://tu-dominio.com`
     - Si tienes IP pública: `http://tu-ip-publica:56610`

3. El workflow usará automáticamente esta URL

### Opción 2: Usar ngrok (Para desarrollo)

**Instalación:**
```bash
# Descargar ngrok desde https://ngrok.com
# O instalar con chocolatey (Windows)
choco install ngrok
```

**Uso:**
```bash
# Exponer tu aplicación local
ngrok http 56610
```

**Resultado:**
```
Forwarding: https://abc123.ngrok.io -> http://localhost:56610
```

**Configurar en n8n:**
- Variable: `BACKEND_URL = https://abc123.ngrok.io`

### Opción 3: Usar IP Pública (Solo si estás en la misma red)

Si n8n y tu aplicación están en la misma red:
- Obtén tu IP local: `ipconfig` (Windows) o `ifconfig` (Linux/Mac)
- Configura: `BACKEND_URL = http://192.168.x.x:56610`

⚠️ **Nota:** Esto solo funciona si están en la misma red local.

---

## 🔧 Problema Adicional: tenantId undefined

### Causa
El `tenantId` está llegando como `00000000-0000-0000-0000-000000000000` (Guid.Empty), probablemente porque:
- El usuario es SuperAdmin (su `TenantId` es `Guid.Empty`)
- O hay un problema en el backend al determinar el tenant

### Solución Temporal
El workflow ahora valida que `tenantId` no sea `Guid.Empty` antes de usarlo.

### Solución Definitiva
Revisar en el backend por qué se está enviando `Guid.Empty` como `tenantId`. Si es un SuperAdmin, el backend debería enviar el `TenantId` de la campaña o del contexto, no `Guid.Empty`.

---

## 📋 Checklist de Verificación

- [ ] ¿Configuraste `BACKEND_URL` en n8n?
- [ ] ¿La URL es accesible desde internet? (prueba en navegador)
- [ ] ¿El `tenantId` no es `Guid.Empty`?
- [ ] ¿El `userId` está presente?
- [ ] ¿El endpoint `/api/consents/check` está funcionando?

---

## 🧪 Prueba Rápida

1. **Desde tu máquina local, prueba:**
   ```bash
   curl http://localhost:56610/api/consents/check?tenantId=test&userId=test
   ```

2. **Desde n8n (o cualquier servidor remoto), prueba:**
   ```bash
   curl https://tu-url-publica/api/consents/check?tenantId=test&userId=test
   ```

Si el segundo falla, la URL no es accesible desde internet.

---

## 🎯 Resumen

**El problema NO es el código del workflow.**

**El problema ES:**
1. n8n está en servidor remoto y no puede acceder a `localhost`
2. Necesitas exponer tu aplicación local a internet (ngrok, túnel, o URL pública)
3. Configurar `BACKEND_URL` en n8n con la URL pública

