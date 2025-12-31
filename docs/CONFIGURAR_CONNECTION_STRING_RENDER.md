# 🔴 SOLUCIÓN: Connection string 'DefaultConnection' not found

## ❌ Error

```
System.InvalidOperationException:
Connection string 'DefaultConnection' not found.
Program.cs: line 28
```

## ✅ Solución: Configurar Variable de Entorno en Render

Aunque el `render.yaml` tiene la variable configurada, **debes configurarla manualmente en el Dashboard de Render** para asegurarte de que se aplique.

### Paso 1: Ir a Environment Variables en Render

1. Ve a tu servicio en Render Dashboard
2. En el menú lateral, haz clic en **"Environment"**
3. Busca la sección **"Environment Variables"**

### Paso 2: Agregar Connection String

**Variable:**
```
ConnectionStrings__DefaultConnection
```

⚠️ **IMPORTANTE:** Usa doble guion bajo `__`, NO dos puntos `:`

**Valor:**
```
Host=dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com;Port=5432;Database=autonomousmarketingplatform;Username=admin;Password=0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1;SSL Mode=Require;
```

### Paso 3: Verificar Otras Variables Mínimas

Asegúrate de que también estén configuradas:

**ASPNETCORE_ENVIRONMENT**
```
Production
```

**ASPNETCORE_URLS**
```
http://0.0.0.0:$PORT
```

**Encryption__Key** (marcada como Secret)
```
TuClaveDe32CaracteresExactos123
```

**AI__OpenAI__ApiKey** (marcada como Secret)
```
sk-proj-...tu-api-key...
```

**AI__OpenAI__Model**
```
gpt-4
```

**AI__UseMock**
```
false
```

**MultiTenant__ValidationEnabled**
```
true
```

**N8n__BaseUrl**
```
https://n8n.bashpty.com
```

**N8n__ApiUrl**
```
https://n8n.bashpty.com/api/v1
```

**N8n__DefaultWebhookUrl**
```
https://n8n.bashpty.com/webhook/marketing-request
```

### Paso 4: Guardar y Reiniciar

1. Haz clic en **"Save Changes"**
2. Render reiniciará automáticamente el servicio
3. Revisa los logs para verificar que arranque correctamente

---

## 🔍 Verificar que Funciona

Después de configurar, revisa los logs. Deberías ver:

```
✅ Application starting...
✅ Entity Framework Core initialized
✅ Database connection successful
```

**NO deberías ver:**
- ❌ "Connection string 'DefaultConnection' not found"
- ❌ "InvalidOperationException"

---

## 📝 Nota sobre render.yaml

El `render.yaml` ya tiene la variable configurada:

```yaml
envVars:
  - key: ConnectionStrings__DefaultConnection
    value: Host=dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com;...
```

Sin embargo, **siempre es recomendable configurarlas manualmente en el Dashboard** para asegurarse de que se apliquen correctamente, especialmente cuando usas Blueprint.

---

## 🆘 Si Sigue Fallando

1. **Verifica el formato:**
   - Debe ser: `ConnectionStrings__DefaultConnection` (doble guion bajo)
   - NO: `ConnectionStrings:DefaultConnection` (dos puntos)

2. **Verifica que la base de datos exista:**
   - Ve a tu base de datos PostgreSQL en Render
   - Verifica que el nombre de la base de datos sea: `autonomousmarketingplatform`

3. **Verifica las credenciales:**
   - Username: `admin`
   - Password: `0kAW5J0EWX3hR7GwDAhOUpv4ieV1IqN1`
   - Host: `dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com`

4. **Prueba la conexión:**
   - Puedes probar la conexión desde tu máquina local usando:
   ```powershell
   psql -h dpg-d5a8afv5r7bs739m2vlg-a.virginia-postgres.render.com -U admin -d autonomousmarketingplatform
   ```

---

## ✅ Checklist Final

- [ ] Variable `ConnectionStrings__DefaultConnection` configurada en Render Dashboard
- [ ] Valor de connection string correcto (con todos los parámetros)
- [ ] `ASPNETCORE_ENVIRONMENT` = `Production`
- [ ] `ASPNETCORE_URLS` = `http://0.0.0.0:$PORT`
- [ ] Variables de secretos configuradas (`Encryption__Key`, `AI__OpenAI__ApiKey`)
- [ ] Servicio reiniciado después de configurar
- [ ] Logs muestran que la aplicación arrancó correctamente

