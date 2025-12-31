# 🔧 Configurar Render Manualmente (Si Blueprint no funciona)

## ⚠️ Problema: Render detecta Node.js en lugar de .NET

Si Render está mostrando "Language: Node" en la interfaz, sigue estos pasos para corregirlo.

---

## ✅ Solución: Configuración Manual Correcta

### 1. **Language (Lenguaje)**
   - ❌ **NO uses**: `Node`
   - ✅ **USA**: `Dotnet` o `.NET`
   - Si no ves la opción, busca en el dropdown o escribe "dotnet"

### 2. **Build Command (Comando de Build)**
   - ❌ **NO uses**: `yarn` o `npm install`
   - ✅ **USA**: 
     ```
     dotnet restore && dotnet publish src/AutonomousMarketingPlatform.Web/AutonomousMarketingPlatform.Web.csproj -c Release -o ./publish
     ```

### 3. **Start Command (Comando de Inicio)**
   - ❌ **NO uses**: `yarn start` o `npm start`
   - ✅ **USA**: 
     ```
     cd publish && dotnet AutonomousMarketingPlatform.Web.dll
     ```

### 4. **Root Directory**
   - ⚠️ **DEJAR VACÍO** (no poner `src` aquí)
   - El proyecto está en la raíz del repositorio

### 5. **Branch**
   - ✅ **USA**: `main` (o `feature/render-deployment` si aún no está en main)

### 6. **Region**
   - ✅ **USA**: `Virginia (US East)`

---

## 🎯 Configuración Completa Paso a Paso

### Paso 1: Seleccionar el Tipo de Servicio
1. En Render Dashboard, haz clic en **"New +"**
2. Selecciona **"Web Service"** (NO "Blueprint" si quieres configurar manualmente)

### Paso 2: Conectar el Repositorio
1. Selecciona **"Connect GitHub"** (o GitLab/Bitbucket)
2. Autoriza Render
3. Selecciona: `IrvingCorrosk19/AutonomousMarketingPlatform`

### Paso 3: Configurar el Servicio

#### **Basic Settings:**
- **Name**: `autonomous-marketing-platform`
- **Region**: `Virginia (US East)`
- **Branch**: `main`
- **Root Directory**: ⚠️ **DEJAR VACÍO**

#### **Build & Deploy:**
- **Environment**: 🔴 **IMPORTANTE** - Selecciona **"Dotnet"** o **".NET"**
  - Si no ves la opción, busca en el dropdown
  - NO uses "Node" aunque aparezca primero
  
- **Build Command**: 
  ```
  dotnet restore && dotnet publish src/AutonomousMarketingPlatform.Web/AutonomousMarketingPlatform.Web.csproj -c Release -o ./publish
  ```

- **Start Command**:
  ```
  cd publish && dotnet AutonomousMarketingPlatform.Web.dll
  ```

### Paso 4: Configurar Variables de Entorno

Después de crear el servicio, ve a **"Environment"** y agrega:

#### Obligatorias:
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `ASPNETCORE_URLS` = `http://0.0.0.0:$PORT`
- `ConnectionStrings__DefaultConnection` = (tu connection string de PostgreSQL)
- `Encryption__Key` = (clave de 32 caracteres - marcada como Secret)
- `AI__OpenAI__ApiKey` = (tu API key - marcada como Secret)
- `AI__OpenAI__Model` = `gpt-4`
- `AI__UseMock` = `false`
- `MultiTenant__ValidationEnabled` = `true`

#### n8n:
- `N8n__BaseUrl` = `https://n8n.bashpty.com`
- `N8n__ApiUrl` = `https://n8n.bashpty.com/api/v1`
- `N8n__DefaultWebhookUrl` = `https://n8n.bashpty.com/webhook/marketing-request`

---

## 🚀 Alternativa: Usar Blueprint (Recomendado)

Si prefieres que Render use automáticamente el `render.yaml`:

1. En Render Dashboard, haz clic en **"New +"**
2. Selecciona **"Blueprint"** (NO "Web Service")
3. Conecta tu repositorio
4. Render detectará automáticamente el `render.yaml` y usará la configuración correcta

---

## 🔍 Verificar que Funciona

Después de configurar, revisa los logs:

1. Ve a tu servicio en Render
2. Haz clic en **"Logs"**
3. Busca:
   ```
   ✅ Restoring packages...
   ✅ Building...
   ✅ Publishing...
   ✅ Build successful
   ```

Si ves errores de Node.js o yarn, significa que el lenguaje no está configurado correctamente como .NET.

---

## 🆘 Solución de Problemas

### Error: "Language: Node" aparece automáticamente
- **Solución**: Cambia manualmente a "Dotnet" o ".NET" en el dropdown
- Si no aparece la opción, escribe "dotnet" en el campo

### Error: "yarn: command not found"
- **Causa**: El lenguaje está configurado como Node.js
- **Solución**: Cambia el Environment a "Dotnet" y actualiza los comandos de build/start

### Error: "dotnet: command not found"
- **Causa**: El lenguaje no está configurado como .NET
- **Solución**: Verifica que el Environment sea "Dotnet"

---

## ✅ Checklist Final

Antes de hacer deploy, verifica:

- [ ] **Language/Environment**: `Dotnet` o `.NET` (NO Node)
- [ ] **Build Command**: Usa `dotnet restore && dotnet publish...`
- [ ] **Start Command**: Usa `cd publish && dotnet AutonomousMarketingPlatform.Web.dll`
- [ ] **Root Directory**: VACÍO (no poner `src`)
- [ ] **Branch**: `main` o `feature/render-deployment`
- [ ] **Region**: `Virginia (US East)`
- [ ] Variables de entorno configuradas

