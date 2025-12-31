# 🔧 Validación de Arrays en n8n - Solución

## ✅ **ESTADO ACTUAL**

El webhook **YA FUNCIONA CORRECTAMENTE**:
- ✅ URL correcta: `https://n8n.bashpty.com/webhook/marketing-request`
- ✅ POST llega correctamente
- ✅ Respuesta 200 OK
- ✅ Payload correcto con todos los campos

## ❌ **ERROR EN N8N WORKFLOW**

**Error:**
```
Wrong type: '' is a string but was expecting an array [condition 3, item 0]
```

**Causa:**
El nodo `Validate Required Fields` está usando `is not empty` para validar `channels`, pero:
- `channels` es un **ARRAY** `["instagram", "facebook"]`
- `is not empty` no funciona bien con arrays en n8n
- Cuando n8n evalúa un array vacío o mal formado, lo convierte en `""` (string vacío)
- Esto causa el error de tipo

---

## ✅ **SOLUCIÓN CORRECTA**

### **Opción 1: Recomendada (Robusta)**

En el nodo `Validate Required Fields`, configura:

| Campo | Expresión | Operador |
|-------|-----------|----------|
| `tenantId` | `{{ $json.body.tenantId }}` | `is not empty` |
| `userId` | `{{ $json.body.userId }}` | `is not empty` |
| `instruction` | `{{ $json.body.instruction }}` | `is not empty` |
| `channels` | `{{ Array.isArray($json.body.channels) && $json.body.channels.length > 0 }}` | `is true` |
| `requiresApproval` | `{{ $json.body.requiresApproval !== undefined }}` | `is true` |

**Ventajas:**
- ✅ Valida que existe
- ✅ Valida que es array
- ✅ Valida que tiene al menos 1 elemento
- ✅ Robusto y profesional

### **Opción 2: Rápida (Menos elegante)**

| Campo | Expresión | Operador |
|-------|-----------|----------|
| `channels` | `{{ $json.body.channels[0] }}` | `is not empty` |

**Ventajas:**
- ✅ Simple y rápido
- ✅ Funciona si hay elementos

**Desventajas:**
- ⚠️ No valida que sea array (solo que tenga algo)
- ⚠️ Puede fallar si llega un string en lugar de array

---

## 🔥 **MEJOR PRÁCTICA: Normalizar antes de validar**

Agregar un nodo `Set` **ANTES** del nodo `Validate Required Fields`:

### **Nodo: Normalize Data**

**Mode:** Manual

**Fields to Set:**
```json
{
  "tenantId": "{{ $json.body.tenantId }}",
  "userId": "{{ $json.body.userId }}",
  "instruction": "{{ $json.body.instruction }}",
  "channels": "{{ $json.body.channels || [] }}",
  "requiresApproval": "{{ $json.body.requiresApproval !== undefined ? $json.body.requiresApproval : false }}",
  "campaignId": "{{ $json.body.campaignId || null }}",
  "assets": "{{ $json.body.assets || [] }}"
}
```

**Ventajas:**
- ✅ Nunca llega `null`
- ✅ Nunca llega `""`
- ✅ Siempre es array (para `channels` y `assets`)
- ✅ Valores por defecto seguros

**Luego, en `Validate Required Fields`:**
- `channels`: `{{ $json.channels.length > 0 }}` → `is true`
- `assets`: `{{ Array.isArray($json.assets) }}` → `is true` (opcional, puede estar vacío)

---

## 📋 **CONFIGURACIÓN COMPLETA RECOMENDADA**

### **Flujo del Workflow:**

1. **Webhook Node** (Trigger)
   - Path: `marketing-request`
   - Method: `POST`
   - Response Mode: `On Received`

2. **Set Node: Normalize Data** (NUEVO - Recomendado)
   - Normaliza todos los campos
   - Asegura tipos correctos

3. **Validate Required Fields Node**
   - Valida con expresiones robustas
   - Usa `is true` para arrays

4. **Resto del workflow...**

---

## 🧪 **PAYLOAD DE PRUEBA**

Tu backend envía:
```json
{
  "tenantId": "00000000-0000-0000-0000-000000000000",
  "userId": "532b8976-25e8-4f84-953e-289cec40aebf",
  "instruction": "Crear contenido de marketing...",
  "channels": ["instagram"],
  "requiresApproval": false,
  "campaignId": null,
  "assets": []
}
```

**✅ Este payload es CORRECTO**

El problema NO es el backend, es la validación en n8n.

---

## ✅ **CONCLUSIÓN**

1. ✅ **Backend funcionando perfectamente**
2. ✅ **Webhook recibiendo datos correctamente**
3. ❌ **Error en validación de n8n (arrays)**
4. ✅ **Solución: Usar `Array.isArray()` y `.length > 0` con `is true`**

**Después de aplicar la solución, el workflow funcionará 100%.**

