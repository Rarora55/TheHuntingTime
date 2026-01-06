# 📋 Sistema de Ledge - Configuración y Uso

## 🎯 Tipos de Ledge

El sistema ahora soporta **dos tipos de ledge**:

### 1️⃣ **Corner Ledge** (Esquina)
- **Detección**: Pared detectada + Ledge NO detectado
- **Uso**: Para subir esquinas de plataformas
- **Parámetro Animator**: `ledge` (bool)
- **Animación sugerida**: `ledgeClimb` / `ledgeClimbCrouch`

### 2️⃣ **Edge Ledge** (Borde)
- **Detección**: Pared NO detectada + Ledge detectado
- **Uso**: Para agarrarse a bordes sin esquina (ej: barras, vigas)
- **Parámetro Animator**: `edgeLedge` (bool)
- **Animación sugerida**: Nueva animación de edge hang

---

## 🔧 Configuración del Animator

### Parámetros Requeridos

Añade estos parámetros booleanos en el Animator Controller:

1. **`ledge`** - Ya existe (Corner)
2. **`edgeLedge`** - NUEVO (Edge) ⚠️ **Debes crear este**
3. **`climbLedge`** - Ya existe (Trigger para subir)
4. **`isTouchingCeiling`** - Ya existe (Crouch al subir)

### Transiciones Recomendadas

#### Para Corner Ledge:
```
AnyState → LedgeClimb
Conditions: ledge == true

LedgeClimb → Idle
Conditions: ledge == false, isTouchingCeiling == false

LedgeClimb → CrouchIdle
Conditions: ledge == false, isTouchingCeiling == true
```

#### Para Edge Ledge (NUEVO):
```
AnyState → EdgeLedgeHang
Conditions: edgeLedge == true

EdgeLedgeHang → Idle
Conditions: edgeLedge == false, isTouchingCeiling == false

EdgeLedgeHang → CrouchIdle
Conditions: edgeLedge == false, isTouchingCeiling == true
```

---

## 🎨 Creación de Animaciones

### Edge Ledge Animation

Debes crear una nueva animación para el Edge:

1. **Nombre sugerido**: `edgeLedgeHang.anim`
2. **Ubicación**: `/Assets/Animations/Character/Mono/`
3. **Contenido**: Animación de personaje colgando de un borde (brazos extendidos hacia adelante)

**Diferencias visuales:**
- **Corner**: Manos en la esquina superior, cuerpo cerca de la pared
- **Edge**: Manos agarrando borde horizontal, cuerpo colgando libremente

---

## 🎮 Cómo Funciona

### Detección Automática

El sistema detecta automáticamente el tipo:

```csharp
LedgeType DetectLedgeType()
{
    bool touchingWall = CheckIfTouchingWall();
    bool touchingLedge = CheckTouchingLedge();
    
    if (!touchingWall && touchingLedge)  → LedgeType.Edge
    if (touchingWall && !touchingLedge)  → LedgeType.Corner
    
    return LedgeType.None;
}
```

### Posicionamiento

- **Corner**: Usa `DetermineCornerPosition()` - Calcula la esquina con raycasts X e Y
- **Edge**: Usa `DetermineEdgePosition()` - Detecta el punto del borde directamente

### Animación

El sistema activa automáticamente el parámetro correcto:

```csharp
if (currentLedgeType == LedgeType.Corner)
    → anim.SetBool("ledge", true)
    
if (currentLedgeType == LedgeType.Edge)
    → anim.SetBool("edgeLedge", true)
```

---

## ✅ Checklist de Implementación

### Paso 1: Animator
- [ ] Crear parámetro bool `edgeLedge` en Animator Controller
- [ ] Crear animación `edgeLedgeHang.anim`
- [ ] Configurar transiciones desde/hacia Edge state

### Paso 2: Testing
- [ ] Probar Corner Ledge en esquinas de plataformas
- [ ] Probar Edge Ledge en bordes horizontales
- [ ] Verificar que las animaciones se activan correctamente

### Paso 3: Ajustes (Opcional)
- [ ] Ajustar offsets en `PlayerData` si es necesario:
  - `startOffSet` - Posición inicial al agarrar
  - `stopOffSet` - Posición final al subir

---

## 🐛 Debugging

### Logs de Debug

El sistema imprime mensajes de color:

- **Cyan**: Detección de tipo y entrada
- **Green**: Corner detectado
- **Yellow**: Edge detectado / posición
- **Red**: Errores de detección

### Visual Debug

- **Raycasts visibles** en Scene view (Editor)
- **Corner**: Líneas roja (X) y azul (Y)
- **Edge**: Línea amarilla

---

## 📊 Diferencias Técnicas

| Característica | Corner | Edge |
|----------------|--------|------|
| **Detección Wall** | ✅ Sí | ❌ No |
| **Detección Ledge** | ❌ No | ✅ Sí |
| **Método posición** | `DetermineCornerPosition()` | `DetermineEdgePosition()` |
| **Parámetro anim** | `ledge` | `edgeLedge` |
| **Uso típico** | Esquinas de plataformas | Barras, vigas, bordes |

---

## 🎯 Ejemplo de Uso

### Escenario Corner:
```
   ┌─────────
   │
   │ ← Player detecta pared
   │    y NO detecta ledge
   │    → Corner Ledge
```

### Escenario Edge:
```
───────────┐
           │  (espacio)
           │
   Player → NO detecta pared
            y SÍ detecta ledge
            → Edge Ledge
```

---

## 💡 Notas Importantes

1. **Ambos tipos usan la misma mecánica de subida** (presionar arriba)
2. **Ambos pueden terminar en Crouch** si hay techo
3. **La detección es automática** - no requiere input del jugador
4. **Los offsets de `PlayerData` se aplican a ambos tipos**

---

## 🚀 Próximos Pasos

1. Crea la animación `edgeLedgeHang`
2. Configura el Animator con el nuevo parámetro
3. Prueba en diferentes escenarios
4. Ajusta offsets si es necesario

¡El sistema está listo para funcionar!
