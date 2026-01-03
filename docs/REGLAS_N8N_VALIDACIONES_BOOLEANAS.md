# Reglas n8n: Validaciones Booleanas

## Regla Crítica: Validación de Booleanos

### ❌ NUNCA usar `isNotEmpty` para booleanos

**Problema:**
- `isNotEmpty` interpreta `false` como "vacío"
- Causa falsos negativos en validaciones
- Genera errores cuando `false` es un valor válido

**Ejemplo INCORRECTO:**
```json
{
  "leftValue": "={{ $json.requiresApproval }}",
  "operator": {
    "type": "boolean",
    "operation": "isNotEmpty"  // ❌ INCORRECTO
  }
}
```

### ✅ SIEMPRE usar comparaciones explícitas

**Solución:**
- Usar `=== true` o `=== false` explícitamente
- Validar ambos valores si ambos son válidos
- Usar expresión booleana con `operation: "true"`

**Ejemplo CORRECTO:**
```json
{
  "leftValue": "={{ $json.requiresApproval === true || $json.requiresApproval === false }}",
  "rightValue": "",
  "operator": {
    "type": "boolean",
    "operation": "true",
    "singleValue": true
  }
}
```

**O si solo `true` es válido:**
```json
{
  "leftValue": "={{ $json.requiresApproval === true }}",
  "rightValue": "",
  "operator": {
    "type": "boolean",
    "operation": "true",
    "singleValue": true
  }
}
```

---

## Casos de Uso

### Caso 1: Campo booleano opcional (ambos valores válidos)

**Escenario:** `requiresApproval` puede ser `true` o `false`, ambos son válidos

**Validación:**
```javascript
$json.requiresApproval === true || $json.requiresApproval === false
```

### Caso 2: Campo booleano requerido (solo `true` válido)

**Escenario:** `aiConsent` debe ser `true` para continuar

**Validación:**
```javascript
$json.aiConsent === true
```

### Caso 3: Normalización de booleanos

**Escenario:** Valores pueden venir como strings, números, o booleanos

**Normalización:**
```javascript
Boolean($json.aiConsent)  // Fuerza a booleano real
```

**Luego validar:**
```javascript
$json.aiConsent === true  // Comparación explícita
```

---

## Checklist de Validación Booleana

Antes de validar un campo booleano en n8n:

- [ ] ¿El campo puede ser `false` como valor válido?
  - [ ] Sí → Usar: `$json.field === true || $json.field === false`
  - [ ] No → Usar: `$json.field === true`

- [ ] ¿El campo puede venir en formato ambiguo (string, número)?
  - [ ] Sí → Normalizar primero: `Boolean($json.field)`
  - [ ] No → Validar directamente

- [ ] ¿Estoy usando `isNotEmpty` sobre un booleano?
  - [ ] Sí → ❌ **CAMBIAR INMEDIATAMENTE**
  - [ ] No → ✅ Correcto

---

## Referencias

- **Problema detectado:** `workflows/n8n/Load Marketing Memory.json` - Validación de `requiresApproval`
- **Corrección aplicada:** 2024-12-19
- **Documentación:** `docs/CORRECCION_LOAD_MARKETING_MEMORY.md`
- **Revisión final:** `docs/REVISION_FINAL_LOAD_MARKETING_MEMORY.md`

---

**Última actualización:** 2024-12-19
**Estado:** ✅ Regla establecida y documentada

