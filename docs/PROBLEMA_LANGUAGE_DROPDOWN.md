# 🔴 Problema: No aparece .NET en el dropdown de Language

## ❌ Problema

Cuando intentas crear un servicio manualmente en Render:
- Seleccionas "Web Service"
- En el campo "Language" solo aparecen opciones como: Node, Python, Ruby, Go, etc.
- **NO aparece .NET o Dotnet** en el dropdown

## ✅ Solución: Usar Blueprint (NO Web Service)

Render **NO muestra .NET** en el dropdown cuando creas un servicio manualmente. La solución es usar **Blueprint** que detecta automáticamente el `render.yaml`.

### Pasos Correctos:

1. **NO uses "Web Service"**
   - ❌ "New +" → "Web Service" (NO funciona para .NET)

2. **USA "Blueprint"**
   - ✅ "New +" → **"Blueprint"**
   - Conecta tu repositorio
   - Render detectará automáticamente el `render.yaml`
   - Usará `env: dotnet` del archivo

### ¿Por qué funciona Blueprint?

- Blueprint lee el `render.yaml` directamente
- No depende del dropdown de Language
- Usa la configuración exacta que tienes en el archivo
- `env: dotnet` en el YAML se aplica automáticamente

---

## 🔍 Verificar que el render.yaml está correcto

Tu `render.yaml` debe tener:

```yaml
services:
  - type: web
    name: autonomous-marketing-platform
    env: dotnet  # ← Esto es lo importante
    plan: starter
    region: virginia
    branch: main
    buildCommand: dotnet restore && dotnet publish src/AutonomousMarketingPlatform.Web/AutonomousMarketingPlatform.Web.csproj -c Release -o ./publish
    startCommand: cd publish && dotnet AutonomousMarketingPlatform.Web.dll
    envVars:
      # ... tus variables
```

✅ **Tu archivo ya tiene `env: dotnet`** - Está correcto

---

## 📝 Pasos Detallados para Blueprint

1. **Ve a Render Dashboard**
   - https://dashboard.render.com

2. **Crea Blueprint (NO Web Service)**
   - Haz clic en **"New +"** (botón verde arriba a la derecha)
   - Selecciona **"Blueprint"** (NO "Web Service")
   - Si ya tienes un servicio creado manualmente, elimínalo primero

3. **Conecta el Repositorio**
   - Selecciona "Connect GitHub" (o GitLab/Bitbucket)
   - Autoriza Render
   - Selecciona: `IrvingCorrosk19/AutonomousMarketingPlatform`

4. **Render detectará automáticamente**
   - Buscará el `render.yaml` en la raíz
   - Leerá `env: dotnet`
   - Configurará todo automáticamente

5. **Verifica en los logs**
   - Después del deploy, revisa los logs
   - Deberías ver: "Using .NET SDK..." (NO "Using Node.js...")

---

## 🆘 Si ya creaste el servicio manualmente

1. **Elimina el servicio actual**
   - Ve a tu servicio
   - Settings → Danger Zone → Delete Service

2. **Crea uno nuevo con Blueprint**
   - Sigue los pasos de arriba

---

## ✅ Resultado Esperado

Después de usar Blueprint, los logs deberían mostrar:

```
✅ Checking out commit... in branch main
✅ Using .NET SDK...
✅ Running build command 'dotnet restore && dotnet publish...'
✅ Build successful
✅ Running 'cd publish && dotnet AutonomousMarketingPlatform.Web.dll'
```

**NO deberías ver:**
- ❌ "Using Node.js version"
- ❌ "Running build command 'yarn'"
- ❌ "Running 'npm start'"

---

## 📌 Resumen

- **Problema**: Dropdown de Language no muestra .NET
- **Causa**: Render no muestra .NET en creación manual de Web Service
- **Solución**: Usar Blueprint en lugar de Web Service
- **Resultado**: Render usa automáticamente `env: dotnet` del `render.yaml`

