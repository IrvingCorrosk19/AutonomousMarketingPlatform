# 🚀 Inicio Rápido: Configuración de n8n

## Paso 1: Instalar Node.js

**Node.js no está instalado en tu sistema.**

### Opción A: Descargar e Instalar Node.js (Recomendado)

1. **Descargar Node.js:**
   - Ir a: https://nodejs.org/
   - Descargar la versión **LTS** (Long Term Support)
   - Ejecutar el instalador
   - Aceptar todas las opciones por defecto

2. **Verificar instalación:**
   ```powershell
   node --version
   npm --version
   ```

3. **Instalar n8n:**
   ```powershell
   npm install n8n -g
   ```

### Opción B: Usar Docker (Alternativa)

Si prefieres usar Docker:

```powershell
# Ejecutar n8n en Docker
docker run -it --rm --name n8n -p 5678:5678 -v ~/.n8n:/home/node/.n8n n8nio/n8n
```

---

## Paso 2: Iniciar n8n

Una vez instalado Node.js y n8n:

```powershell
# Opción 1: Usar el script
.\scripts\iniciar-n8n.ps1

# Opción 2: Comando directo
n8n start
```

**Acceder a n8n:**
- Abrir navegador en: `http://localhost:5678`
- Crear cuenta inicial (primera vez)

---

## Paso 3: Importar Workflows

1. En n8n, ir a **Workflows** → **Import from File**
2. Importar los 11 archivos JSON desde:
   ```
   workflows\n8n\
   ```
3. Activar cada workflow (toggle en la esquina superior derecha)

---

## Paso 4: Configurar URLs

1. Para cada workflow, copiar la URL del webhook
2. Actualizar `appsettings.json` con las URLs reales
3. Cambiar `"UseMock": false` en `appsettings.json`

---

## 📋 Checklist Rápido

- [ ] Node.js instalado
- [ ] n8n instalado (`npm install n8n -g`)
- [ ] n8n corriendo (`n8n start`)
- [ ] n8n accesible en `http://localhost:5678`
- [ ] Cuenta creada en n8n
- [ ] 11 workflows importados
- [ ] Workflows activados
- [ ] URLs copiadas y configuradas en `appsettings.json`

---

## 🆘 ¿Necesitas Ayuda?

Ver la guía completa: `docs/GUIA_CONFIGURACION_N8N_PASO_A_PASO.md`

