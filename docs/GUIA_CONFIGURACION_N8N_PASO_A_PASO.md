# Guía Paso a Paso: Configuración de n8n con el Sistema

**Fecha:** 2025-01-01  
**Sistema:** Autonomous Marketing Platform

---

## 📋 Prerrequisitos

- Node.js instalado (versión 18 o superior) - [Descargar Node.js](https://nodejs.org/)
- O Docker instalado (alternativa)
- Sistema ASP.NET Core corriendo en `http://localhost:5000` (o el puerto que uses)

---

## 🚀 Paso 1: Instalar n8n

### Opción A: Instalación con npm (Recomendado)

```powershell
# Instalar n8n globalmente
npm install n8n -g

# Verificar instalación
n8n --version
```

### Opción B: Instalación con Docker

```powershell
# Ejecutar n8n en Docker
docker run -it --rm --name n8n -p 5678:5678 -v ~/.n8n:/home/node/.n8n n8nio/n8n
```

**Nota:** Si usas Docker, n8n estará disponible en `http://localhost:5678` automáticamente.

---

## 🚀 Paso 2: Iniciar n8n

### Con npm:

```powershell
# Iniciar n8n
n8n start

# O iniciar en modo webhook (para producción)
n8n start --tunnel
```

### Con Docker:

El contenedor ya está corriendo si usaste el comando anterior.

**Acceder a n8n:**
- Abrir navegador en: `http://localhost:5678`
- Crear cuenta inicial (primera vez)
- O iniciar sesión si ya tienes cuenta

---

## 🚀 Paso 3: Importar Workflows

Tenemos **11 workflows** listos para importar:

1. `01-trigger-marketing-request.json`
2. `02-validate-consents.json`
3. `03-load-marketing-memory.json`
4. `04-analyze-instruction-ai.json`
5. `05-generate-marketing-strategy.json`
6. `06-generate-marketing-copy.json`
7. `07-generate-visual-prompts.json`
8. `08-build-marketing-pack.json`
9. `09-human-approval-flow.json`
10. `10-publish-content.json`
11. `11-metrics-learning.json`

### Proceso de Importación:

1. En n8n, hacer clic en **"Workflows"** (menú lateral)
2. Hacer clic en **"Import from File"** (botón superior)
3. Seleccionar cada archivo JSON de la carpeta `workflows/n8n/`
4. Repetir para los 11 workflows

**Ubicación de los archivos:**
```
C:\Proyectos\BROKERPRO\AutonomousMarketingPlatform\workflows\n8n\
```

---

## 🚀 Paso 4: Activar Workflows y Obtener URLs de Webhooks

Para cada workflow importado:

1. **Abrir el workflow** haciendo clic en su nombre
2. **Hacer clic en el nodo "Webhook"** (primer nodo)
3. **Copiar la "Production URL"** que aparece en el panel derecho
   - Ejemplo: `http://localhost:5678/webhook/trigger-marketing-request`
4. **Activar el workflow** usando el toggle en la esquina superior derecha
5. **Guardar** el workflow (Ctrl+S o botón Save)

### Mapeo de Workflows a URLs:

| Workflow | Archivo JSON | URL Esperada |
|----------|--------------|--------------|
| Trigger Marketing Request | `01-trigger-marketing-request.json` | `/webhook/trigger-marketing-request` |
| Validate Consents | `02-validate-consents.json` | `/webhook/validate-consents` |
| Load Marketing Memory | `03-load-marketing-memory.json` | `/webhook/load-marketing-memory` |
| Analyze Instruction AI | `04-analyze-instruction-ai.json` | `/webhook/analyze-instruction-ai` |
| Generate Marketing Strategy | `05-generate-marketing-strategy.json` | `/webhook/generate-marketing-strategy` |
| Generate Marketing Copy | `06-generate-marketing-copy.json` | `/webhook/generate-marketing-copy` |
| Generate Visual Prompts | `07-generate-visual-prompts.json` | `/webhook/generate-visual-prompts` |
| Build Marketing Pack | `08-build-marketing-pack.json` | `/webhook/build-marketing-pack` |
| Human Approval Flow | `09-human-approval-flow.json` | `/webhook/human-approval-flow` |
| Publish Content | `10-publish-content.json` | `/webhook/publish-content` |
| Metrics & Learning | `11-metrics-learning.json` | `/webhook/metrics-learning` |

---

## 🚀 Paso 5: Configurar Variables de Entorno en n8n

1. En n8n, ir a **Settings** → **Environment Variables**
2. Agregar las siguientes variables:

| Variable | Valor | Descripción |
|----------|-------|-------------|
| `BACKEND_URL` | `http://localhost:5000` | URL del backend ASP.NET Core |
| `OPENAI_API_KEY` | `tu-api-key-aqui` | (Opcional) Si usas OpenAI |
| `OPENAI_MODEL` | `gpt-4` | (Opcional) Modelo de OpenAI |

**Nota:** Si no tienes credenciales de OpenAI, los workflows simularán las respuestas.

---

## 🚀 Paso 6: Actualizar appsettings.json

Una vez que tengas las URLs de webhooks de n8n, actualiza `appsettings.json`:

```json
{
  "N8n": {
    "UseMock": false,
    "BaseUrl": "http://localhost:5678",
    "ApiUrl": "http://localhost:5678/api/v1",
    "ApiKey": "",
    "DefaultWebhookUrl": "http://localhost:5678/webhook",
    "WebhookUrls": {
      "MarketingRequest": "http://localhost:5678/webhook/XXXXX",
      "ValidateConsents": "http://localhost:5678/webhook/XXXXX",
      "LoadMemory": "http://localhost:5678/webhook/XXXXX",
      "AnalyzeInstruction": "http://localhost:5678/webhook/XXXXX",
      "GenerateStrategy": "http://localhost:5678/webhook/XXXXX",
      "GenerateCopy": "http://localhost:5678/webhook/XXXXX",
      "GenerateVisualPrompts": "http://localhost:5678/webhook/XXXXX",
      "BuildMarketingPack": "http://localhost:5678/webhook/XXXXX",
      "HumanApproval": "http://localhost:5678/webhook/XXXXX",
      "PublishContent": "http://localhost:5678/webhook/XXXXX",
      "MetricsLearning": "http://localhost:5678/webhook/XXXXX"
    }
  }
}
```

**Reemplazar `XXXXX`** con las URLs reales que copiaste de n8n.

---

## 🚀 Paso 7: Configurar Credenciales (Opcional)

### OpenAI (si usas OpenAI):

1. En n8n, ir a **Settings** → **Credentials**
2. Hacer clic en **"Add Credential"**
3. Buscar **"OpenAI"**
4. Ingresar tu API Key
5. Guardar

### HTTP Header Auth (para llamadas al backend):

1. En n8n, ir a **Settings** → **Credentials**
2. Hacer clic en **"Add Credential"**
3. Buscar **"HTTP Header Auth"**
4. Configurar:
   - **Name:** `Backend API`
   - **Header Name:** `X-API-Key` (si implementas autenticación)
   - **Header Value:** (dejar vacío por ahora, o tu API key si la configuraste)

---

## 🚀 Paso 8: Probar la Conexión

### Probar desde el Sistema:

1. Asegúrate de que el sistema ASP.NET Core esté corriendo
2. Asegúrate de que n8n esté corriendo
3. El sistema intentará llamar a n8n cuando se dispare una automatización

### Probar Webhook Manualmente:

```powershell
# Probar webhook de trigger marketing request
curl -X POST http://localhost:5678/webhook/trigger-marketing-request `
  -H "Content-Type: application/json" `
  -d '{
    "tenantId": "550e8400-e29b-41d4-a716-446655440000",
    "userId": "660e8400-e29b-41d4-a716-446655440001",
    "instruction": "Crear campaña de verano",
    "channels": ["instagram", "facebook"],
    "requiresApproval": true
  }'
```

### Verificar en n8n:

1. Ir a **Executions** en n8n
2. Deberías ver la ejecución del workflow
3. Hacer clic para ver los detalles y datos procesados

---

## 🚀 Paso 9: Verificar Endpoints del Backend

Los workflows de n8n llaman a estos endpoints del backend. Verifica que estén accesibles:

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `http://localhost:5000/api/consents/check` | GET | Validar consentimientos |
| `http://localhost:5000/api/memory/context` | GET | Obtener contexto de memoria |
| `http://localhost:5000/api/marketing-packs` | POST | Guardar MarketingPack |
| `http://localhost:5000/api/publishing-jobs` | POST | Guardar resultado de publicación |
| `http://localhost:5000/api/metrics/campaign` | POST | Guardar métricas de campaña |
| `http://localhost:5000/api/metrics/publishing-job` | POST | Guardar métricas de publicación |
| `http://localhost:5000/api/memory/save` | POST | Guardar aprendizaje en memoria |

**Probar endpoint:**
```powershell
# Probar endpoint de consentimientos
curl -X GET "http://localhost:5000/api/consents/check?tenantId=550e8400-e29b-41d4-a716-446655440000&userId=660e8400-e29b-41d4-a716-446655440001"
```

---

## ✅ Checklist de Configuración

- [ ] n8n instalado y corriendo
- [ ] n8n accesible en `http://localhost:5678`
- [ ] Cuenta creada en n8n
- [ ] 11 workflows importados
- [ ] 11 workflows activados
- [ ] URLs de webhooks copiadas
- [ ] `appsettings.json` actualizado con URLs reales
- [ ] `UseMock: false` en `appsettings.json`
- [ ] Variables de entorno configuradas en n8n (`BACKEND_URL`)
- [ ] Credenciales configuradas (si aplica)
- [ ] Backend ASP.NET Core corriendo
- [ ] Endpoints del backend accesibles
- [ ] Prueba de conexión exitosa

---

## 🐛 Troubleshooting

### Error: "n8n no se reconoce como comando"
**Solución:** Instalar n8n con `npm install n8n -g` o usar Docker.

### Error: "Cannot connect to n8n"
**Solución:** 
- Verificar que n8n esté corriendo: `http://localhost:5678`
- Verificar que el puerto 5678 no esté en uso
- Verificar firewall

### Error: "Webhook not found"
**Solución:**
- Verificar que el workflow esté activado en n8n
- Verificar que la URL en `appsettings.json` coincida exactamente con la de n8n
- Copiar la URL completa desde n8n (incluye el path completo)

### Error: "Backend endpoint not found"
**Solución:**
- Verificar que el backend esté corriendo
- Verificar que `BACKEND_URL` en n8n sea correcta
- Verificar que los endpoints existan en el backend

### Error: "Timeout"
**Solución:**
- Aumentar timeout en los nodos HTTP Request de n8n
- Verificar que el backend responda rápidamente
- Verificar red/conectividad

---

## 📝 Próximos Pasos

Una vez configurado:

1. **Probar flujo completo:**
   - Disparar automatización desde el sistema
   - Verificar ejecución en n8n
   - Verificar que los datos se guarden en el backend

2. **Monitorear ejecuciones:**
   - Revisar logs en n8n
   - Revisar logs del backend
   - Verificar datos en base de datos

3. **Optimizar workflows:**
   - Ajustar timeouts
   - Agregar manejo de errores
   - Optimizar llamadas a APIs

---

**¿Listo para empezar?** Comencemos con el Paso 1: Instalación de n8n.

