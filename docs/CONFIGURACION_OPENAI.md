# Configuración de OpenAI

## API Key Configurada

La API key de OpenAI ha sido configurada en `appsettings.Development.json`.

**IMPORTANTE:**
- El archivo `appsettings.Development.json` está en `.gitignore` y NO se subirá al repositorio
- La API key está protegida y solo existe localmente
- Para producción, usar variables de entorno o Azure Key Vault

## Configuración Actual

```json
{
  "AI": {
    "UseMock": false,
    "OpenAI": {
      "ApiKey": "sk-proj-...",
      "Model": "gpt-4"
    }
  }
}
```

## Uso

El sistema ahora usará la API real de OpenAI en lugar del modo mock.

Para cambiar a modo mock (sin usar API real):
- Cambiar `"UseMock": true` en `appsettings.Development.json`

## Variables de Entorno (Recomendado para Producción)

En producción, usar variables de entorno:

```bash
AI__OpenAI__ApiKey=sk-proj-...
AI__UseMock=false
AI__OpenAI__Model=gpt-4
```

## Seguridad

- ✅ API key NO está en el repositorio
- ✅ appsettings.Development.json está en .gitignore
- ✅ En producción, usar secretos seguros (Azure Key Vault, AWS Secrets Manager, etc.)

