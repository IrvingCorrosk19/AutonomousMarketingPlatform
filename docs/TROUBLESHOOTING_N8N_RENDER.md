# Solución de Problemas: Conexión n8n desde Render

## Problema

La conexión con n8n funciona desde local pero falla cuando se despliega en Render.

## Posibles Causas y Soluciones

### 1. Timeout del HttpClient

**Problema**: El timeout por defecto de 30 segundos es insuficiente para workflows largos de n8n.

**Solución**: Ya implementado - Timeout aumentado a 5 minutos en `Program.cs`.

### 2. BaseAddress del HttpClient

**Problema**: Cuando se configura `BaseAddress` en el HttpClient y luego se usa `PostAsync` con URLs absolutas, puede causar conflictos.

**Solución**: Ya implementado - `BaseAddress` removido, usando solo URLs absolutas.

### 3. Validación de Certificados SSL

**Problema**: Render puede tener problemas validando certificados SSL de n8n.bashpty.com.

**Solución**: Ya implementado - Configuración de `ServerCertificateCustomValidationCallback` que valida correctamente en producción.

### 4. Headers HTTP

**Problema**: n8n podría estar bloqueando requests sin User-Agent apropiado.

**Solución**: Ya implementado - Headers `User-Agent` y `Accept` agregados.

### 5. Variables de Entorno en Render

**Verificar en Render Dashboard**:

```bash
N8n__BaseUrl=https://n8n.bashpty.com
N8n__DefaultWebhookUrl=https://n8n.bashpty.com/webhook/marketing-request
```

### 6. Firewall/Red de Render

**Problema**: Render podría estar bloqueando conexiones salientes a n8n.bashpty.com.

**Solución**: 
- Verificar que n8n.bashpty.com sea accesible desde internet
- Verificar logs de Render para ver errores de conexión
- Contactar soporte de Render si hay bloqueos de red

### 7. Logs para Diagnóstico

El código ahora incluye logging detallado:

- URL completa del webhook
- Scheme, Host, Port de la URL
- Headers de respuesta
- Errores de timeout específicos
- Errores de red específicos

**Revisar logs en Render** para ver:
- ¿Se está construyendo la URL correctamente?
- ¿Hay errores de timeout?
- ¿Hay errores de SSL?
- ¿Hay errores de red?

### 8. Verificar Configuración de n8n

En n8n (https://n8n.bashpty.com):

1. Verificar que el workflow esté **activo**
2. Verificar que el webhook path sea correcto: `/webhook/marketing-request`
3. Verificar que no haya restricciones de IP
4. Verificar logs de n8n para ver si recibe las requests

### 9. Probar Conectividad desde Render

Agregar un endpoint de prueba temporal:

```csharp
[HttpGet("/api/test-n8n-connection")]
public async Task<IActionResult> TestN8nConnection()
{
    try
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        var response = await httpClient.GetAsync("https://n8n.bashpty.com/healthz");
        return Ok(new { 
            success = response.IsSuccessStatusCode, 
            statusCode = response.StatusCode,
            message = "Conexión exitosa a n8n" 
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new { 
            success = false, 
            error = ex.Message,
            innerException = ex.InnerException?.Message 
        });
    }
}
```

### 10. Verificar DNS

Render debe poder resolver `n8n.bashpty.com`. Si hay problemas de DNS:

- Verificar que el dominio esté correctamente configurado
- Verificar que el DNS resuelva correctamente
- Probar con IP directa (si está disponible)

## Checklist de Verificación

- [ ] Variables de entorno configuradas en Render
- [ ] Timeout del HttpClient aumentado (5 minutos)
- [ ] BaseAddress removido del HttpClient
- [ ] Headers User-Agent y Accept configurados
- [ ] Validación SSL configurada correctamente
- [ ] Workflow de n8n está activo
- [ ] Webhook path correcto en n8n
- [ ] Logs de Render revisados
- [ ] Logs de n8n revisados
- [ ] Conectividad de red verificada

## Comandos Útiles

### Verificar desde Render (si tienes acceso SSH)

```bash
curl -v https://n8n.bashpty.com/webhook/marketing-request
```

### Verificar DNS

```bash
nslookup n8n.bashpty.com
```

### Probar concurl desde Render

```bash
curl -X POST https://n8n.bashpty.com/webhook/marketing-request \
  -H "Content-Type: application/json" \
  -H "User-Agent: AutonomousMarketingPlatform/1.0" \
  -d '{"test": "data"}'
```

## Logs a Revisar

1. **Logs de Render**: Buscar errores de `ExternalAutomationService`
2. **Logs de n8n**: Ver si recibe las requests
3. **Logs de la aplicación**: Buscar mensajes que empiecen con "=== PAYLOAD ENVIADO A N8N ==="

## Contacto

Si el problema persiste después de verificar todos los puntos:
1. Revisar logs detallados de Render
2. Revisar logs de n8n
3. Verificar configuración de red/firewall en Render
4. Contactar soporte de Render si hay bloqueos de red

