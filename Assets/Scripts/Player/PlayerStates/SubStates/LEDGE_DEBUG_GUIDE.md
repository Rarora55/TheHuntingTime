# 🔍 Guía de Debug: Ledge Edge vs Corner con Plataformas One-Way

## 🐛 Problema Reportado

Las plataformas one-way activan el **ledge Corner** en lugar del **ledge Edge**.

---

## 🎯 Solución Implementada

Mejorada la detección de tipo de ledge en `PlayerCollisionController.DetectLedgeType()` para diferenciar:

### Lógica Actualizada:

```csharp
1. Hacer raycast desde wallCheck hacia adelante
2. Hacer raycast desde ledgeCheck hacia adelante
3. Si AMBOS detectan hit:
   a. Calcular diferencia de altura entre los dos puntos
   b. Si diferencia < 0.2f → **EDGE** (plataforma horizontal)
   c. Si diferencia >= 0.2f → **CORNER** (esquina real)
4. Si solo ledgeCheck detecta → **EDGE**
5. Si solo wallCheck detecta → **CORNER**
```

---

## 🔧 Por Qué Ocurre el Problema

### Plataforma One-Way con BoxCollider:

```
┌─────────────────────────┐
│    BoxCollider2D        │  ← Tiene altura (size.y)
│    con offset Y         │
└─────────────────────────┘

Raycasts del Player:
   wallCheck →  ━━━━━━━┐  ← Detecta el lado del collider
                       │
   ledgeCheck → ━━━━━━━┘  ← Detecta también el lado
```

**Problema**: Ambos raycasts detectan el collider → Sistema antiguo pensaba que era **Corner**.

**Solución**: Verificar la altura de los impactos:
- Si están a la **misma altura** (< 0.2f diferencia) → Es una plataforma horizontal → **EDGE**
- Si están a **diferente altura** → Es una esquina real → **CORNER**

---

## 🧪 Cómo Debuggear

### Paso 1: Activar Logs

Los logs ya están activos en `DetectLedgeType()`. Verás en Console:

```
[LEDGE TYPE] WallCheck: true (at Y.YY) | LedgeCheck: true (at Y.YY)
[LEDGE TYPE] Ambos hits - WallY: X.XX | LedgeY: Y.YY | Diff: Z.ZZ
[LEDGE TYPE] ✅ Edge detectado (misma altura - plataforma one-way)
```

---

### Paso 2: Ver Raycasts en Scene View

Con el Player seleccionado en Hierarchy, en **Scene View** verás:

```
wallCheck →  ━━━━━━━  (Rayo rojo/verde)
ledgeCheck → ━━━━━━━  (Rayo cyan/magenta)
```

**Colores**:
- **Verde/Cyan**: Hit detectado ✅
- **Rojo/Magenta**: Sin hit ❌

---

### Paso 3: Interpretar los Logs

#### ✅ Caso 1: Edge (Plataforma One-Way)
```
[LEDGE TYPE] WallCheck: true (at -0.19) | LedgeCheck: true (at -0.18)
[LEDGE TYPE] Ambos hits - WallY: -0.19 | LedgeY: -0.18 | Diff: 0.01
[LEDGE TYPE] ✅ Edge detectado (misma altura - plataforma one-way)
```
→ Diferencia de altura **< 0.2** → **EDGE** ✅

---

#### ✅ Caso 2: Corner (Esquina Real)
```
[LEDGE TYPE] WallCheck: true (at 2.50) | LedgeCheck: true (at 3.20)
[LEDGE TYPE] Ambos hits - WallY: 2.50 | LedgeY: 3.20 | Diff: 0.70
[LEDGE TYPE] ✅ Corner detectado (diferente altura)
```
→ Diferencia de altura **>= 0.2** → **CORNER** ✅

---

#### ✅ Caso 3: Edge (Solo Ledge)
```
[LEDGE TYPE] WallCheck: false | LedgeCheck: true
[LEDGE TYPE] ✅ Edge detectado (sin pared, con ledge)
```
→ Solo ledge detecta → **EDGE** ✅

---

#### ✅ Caso 4: Corner (Solo Wall)
```
[LEDGE TYPE] WallCheck: true | LedgeCheck: false
[LEDGE TYPE] ✅ Corner detectado (con pared, sin ledge)
```
→ Solo wall detecta → **CORNER** ✅

---

## 🎨 Visualización

### Edge (Plataforma Horizontal):

```
Player →   👤
           ↓
       wallCheck ━━━┐  } Misma altura (< 0.2f diff)
      ledgeCheck ━━━┘  }
           ↓
    ─────────────────  ← Plataforma one-way
    (BoxCollider2D)
```

**Detección**: Ambos raycasts hit a misma altura → **EDGE**

---

### Corner (Esquina Real):

```
Player →   👤
           ↓
       wallCheck ━━━┐  } Diferente altura (>= 0.2f diff)
                    │  }
                    │  } 0.7f
      ledgeCheck ━━━┘  }
           ↓
         ┌─────
         │     ← Esquina real (pared + techo)
    ─────┘
```

**Detección**: Ambos raycasts hit a diferente altura → **CORNER**

---

## ⚙️ Parámetros de Ajuste

Si necesitas ajustar la sensibilidad:

### En `PlayerCollisionController.DetectLedgeType()`:

```csharp
float heightDifference = Mathf.Abs(wallHit.point.y - ledgeHit.point.y);

if (heightDifference < 0.2f)  // ← AJUSTA ESTE VALOR
{
    return LedgeType.Edge;  // Plataforma horizontal
}
else
{
    return LedgeType.Corner;  // Esquina real
}
```

**Valores recomendados**:
- **0.1f**: Más estricto (solo plataformas muy horizontales → Edge)
- **0.2f**: Balanceado (valor actual) ✅
- **0.3f**: Más permisivo (más casos → Edge)

---

## 🔍 Checks Manuales

### 1. Verificar Posición de Checks

Selecciona el **Player** en Hierarchy y verifica en **Scene View**:

```
Player
├── wallCheck      (Transform hijo)
│   └── Position.y: ~2.5 (altura media del player)
├── ledgeCheck     (Transform hijo)
│   └── Position.y: ~3.2 (altura de la cabeza)
└── groundCheck    (Transform hijo)
    └── Position.y: ~1.8 (pies)
```

**Importante**: 
- `ledgeCheck` debe estar **más arriba** que `wallCheck`
- Típicamente: `ledgeCheck.y > wallCheck.y + 0.5f`

---

### 2. Verificar PlayerData Distances

Selecciona `PlayerData` ScriptableObject:

```
├── WallCheckDistance: 0.2 - 0.4 (alcance del raycast)
└── LedgeCheckDistance: 0.2 - 0.4 (alcance del raycast)
```

**Recomendado**: Ambas distancias iguales (~0.3f)

---

### 3. Verificar Platform Effector 2D

Selecciona la plataforma one-way:

```
Platform Effector 2D
├── ✅ Use One Way: true
├── Surface Arc: 180
├── Rotation Offset: 0
└── ✅ Use One Way Grouping: true
```

---

## 🎯 Testing

### Test 1: Plataforma One-Way Horizontal

1. **Setup**:
   ```
   ─────────────  ← Plataforma one-way
   ```

2. **Acércate desde el lado**:
   ```
   👤 →  ─────────────
   ```

3. **Verifica Console**:
   ```
   [LEDGE TYPE] Ambos hits - Diff: 0.XX
   [LEDGE TYPE] ✅ Edge detectado
   ```

4. **Verifica Animator**:
   - Parámetro `edgeLedge` = **true** ✅
   - Parámetro `ledge` = **false**

---

### Test 2: Esquina Real (Corner)

1. **Setup**:
   ```
   ┌─────
   │      ← Esquina de plataforma
   ─┘
   ```

2. **Acércate desde el lado**:
   ```
   👤 →  ┌─────
         │
   ```

3. **Verifica Console**:
   ```
   [LEDGE TYPE] Ambos hits - Diff: 0.XX (>= 0.2)
   [LEDGE TYPE] ✅ Corner detectado
   ```

4. **Verifica Animator**:
   - Parámetro `ledge` = **true** ✅
   - Parámetro `edgeLedge` = **false**

---

### Test 3: Barra Horizontal (Edge Sin Wall)

1. **Setup**:
   ```
      ━━━━━━  ← Barra en el aire
   ```

2. **Acércate por debajo**:
   ```
      ━━━━━━
      ↑
      👤
   ```

3. **Verifica Console**:
   ```
   [LEDGE TYPE] WallCheck: false | LedgeCheck: true
   [LEDGE TYPE] ✅ Edge detectado
   ```

4. **Verifica Animator**:
   - Parámetro `edgeLedge` = **true** ✅

---

## 🛠️ Troubleshooting

### ❌ Problema: Siempre detecta Corner

**Posibles causas**:

1. **BoxCollider muy alto**:
   - La plataforma tiene `size.y` muy grande
   - Los raycasts detectan diferentes alturas
   - **Solución**: Reduce `size.y` del BoxCollider

2. **Checks mal posicionados**:
   - `wallCheck` y `ledgeCheck` muy separados en Y
   - **Solución**: Acerca `wallCheck.y` a `ledgeCheck.y`

3. **Threshold muy estricto**:
   - `heightDifference < 0.2f` muy bajo
   - **Solución**: Aumenta a `0.3f`

---

### ❌ Problema: Siempre detecta Edge

**Posibles causas**:

1. **Threshold muy permisivo**:
   - `heightDifference < 0.2f` muy alto
   - **Solución**: Reduce a `0.1f`

2. **Checks muy juntos**:
   - `wallCheck.y` casi igual a `ledgeCheck.y`
   - **Solución**: Separa más en Y

---

### ❌ Problema: No detecta nada

**Posibles causas**:

1. **Layer incorrecto**:
   - Plataforma no está en layer `Ground`
   - **Solución**: Cambia layer de la plataforma a `Ground`

2. **Distances muy cortas**:
   - `WallCheckDistance` o `LedgeCheckDistance` < 0.2
   - **Solución**: Aumenta a ~0.3-0.4

3. **Checks fuera del collider**:
   - Player muy lejos de la plataforma
   - **Solución**: Acércate más

---

## 📊 Resumen de Cambios

### Antes:
```csharp
if (!touchingWall && touchingLedge) → Edge
if (touchingWall && !touchingLedge) → Corner
else → None
```

**Problema**: No manejaba el caso `touchingWall && touchingLedge` correctamente.

---

### Ahora:
```csharp
if (wallHit && ledgeHit):
  if (heightDiff < 0.2f) → Edge (plataforma horizontal)
  else → Corner (esquina real)
if (!wallHit && ledgeHit) → Edge
if (wallHit && !ledgeHit) → Corner
```

**Mejora**: Distingue plataformas horizontales de esquinas reales usando la diferencia de altura.

---

## ✅ Checklist de Verificación

Antes de reportar que no funciona:

- [ ] Los logs aparecen en Console (Play Mode)
- [ ] `wallCheck` y `ledgeCheck` están en el Player
- [ ] La plataforma está en layer `Ground`
- [ ] `WallCheckDistance` y `LedgeCheckDistance` ≥ 0.3
- [ ] Animator tiene parámetro bool `edgeLedge` creado
- [ ] Animación `edgeLedgeHang.anim` existe
- [ ] Transiciones desde AnyState → Edge Ledge State configuradas

---

## 🎉 Resultado Esperado

### Con Plataforma One-Way:
```
[LEDGE TYPE] ✅ Edge detectado (misma altura - plataforma one-way)
→ Animator: edgeLedge = true
→ Animación: edgeLedgeHang.anim
```

### Con Esquina Real:
```
[LEDGE TYPE] ✅ Corner detectado (diferente altura)
→ Animator: ledge = true
→ Animación: ledgeHang.anim
```

---

Si el problema persiste después de seguir esta guía, comparte los logs que aparecen en Console para un diagnóstico más detallado. 🔍
