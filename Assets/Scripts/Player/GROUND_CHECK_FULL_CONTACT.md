# ✅ Sistema de Detección de Suelo Completo (Full Ground Contact)

## 🎯 Objetivo

Asegurar que el personaje solo se considere "grounded" cuando está **COMPLETAMENTE** sobre el suelo, no parcialmente colgando del borde como en la imagen reportada.

---

## 🐛 Problema Anterior

### Escenario del Bug:

```
     👤  ← Player parcialmente fuera de la plataforma
    /|\ 
    / \
   ────────  ← Plataforma
```

**Síntoma**: El player aparece "grounded" aunque está colgando parcialmente del borde.

**Causa**: Solo se usaba **1 raycast central**, que detectaba suelo aunque el player no estuviera completamente apoyado.

---

## ✅ Solución Implementada

### Sistema de 3 Raycasts:

```
     👤  ← Player
    /|\ 
    / \
    ↓ ↓ ↓  ← 3 raycasts (Left, Center, Right)
   ────────  ← Plataforma
```

**Lógica**:
```csharp
isFullyGrounded = centerGrounded && leftGrounded && rightGrounded;
```

El player solo está "grounded" si **LOS 3 raycasts** detectan suelo.

---

## 📐 Configuración Actual

### Player BoxCollider:
- **size.x**: 1.02 (ancho del collider)
- **size.y**: 2.15 (altura del collider)

### GroundCheck (Transform hijo):
- **localPosition.y**: -1.08 (en los pies del player)

### Horizontal Offset (Distancia Lateral):
- **horizontalOffset**: 0.45f

**Cálculo**:
```
Total Coverage = horizontalOffset × 2 = 0.45 × 2 = 0.9 units
Player Width = 1.02 units
Coverage Percentage = (0.9 / 1.02) × 100 = ~88%
```

✅ **Cubre casi el 90% del ancho del player** → Excelente cobertura

---

## 🎨 Visualización en Scene View

### En Play Mode con Player seleccionado:

```
          👤 Player
         /|\ 
         / \
        L C R    ← 3 raycasts
        | | |
        ↓ ↓ ↓
    ════════════  ← Plataforma
```

**Colores de los raycasts**:
- **Verde**: Detecta suelo ✅
- **Rojo**: No detecta suelo ❌

---

### Caso 1: Totalmente en el Suelo

```
          👤
         /|\ 
         / \
        L C R
        🟢🟢🟢  ← Todos verdes
    ════════════
```

**Resultado**: `isFullyGrounded = true` ✅

---

### Caso 2: Parcialmente en el Borde (El Bug Reportado)

```
          👤
         /|\ 
         / \
        L C R
        🔴🟢🟢  ← Left rojo, otros verdes
    ────────
```

**Resultado**: `isFullyGrounded = false` ✅ (Entra en AirState → FallState)

**Log en Console**:
```
[GROUND] Parcialmente en suelo - Center: true | Left: false | Right: true
```

---

### Caso 3: Completamente en el Aire

```
          👤
         /|\ 
         / \
        L C R
        🔴🔴🔴  ← Todos rojos
        
    ════════════
```

**Resultado**: `isFullyGrounded = false` ✅ (Sale rápido del método)

---

## 🔧 Cómo Funciona el Código

### CheckIsGrounded() - Flujo Completo:

```csharp
public bool CheckIsGrounded()
{
    // 1️⃣ RAYCAST CENTRAL (Early Exit)
    bool centerGrounded = Physics2D.Raycast(
        groundCheck.position, 
        Vector2.down, 
        playerData.GroundCheckRadius, 
        playerData.WhatIsGround);
    
    if (!centerGrounded)
    {
        // Si el centro no toca → Definitivamente en el aire
        wasGrounded = false;
        return false;  // EXIT RÁPIDO ⚡
    }
    
    // 2️⃣ RAYCASTS LATERALES (Solo si center = true)
    float horizontalOffset = 0.45f;
    
    Vector2 leftCheckPos = groundCheck.position;
    leftCheckPos.x -= horizontalOffset;
    bool leftGrounded = Physics2D.Raycast(...);
    
    Vector2 rightCheckPos = groundCheck.position;
    rightCheckPos.x += horizontalOffset;
    bool rightGrounded = Physics2D.Raycast(...);
    
    // 3️⃣ VERIFICACIÓN COMPLETA
    bool isFullyGrounded = centerGrounded && leftGrounded && rightGrounded;
    
    // 4️⃣ DEBUG LOG (Solo si parcialmente en suelo)
    if (!isFullyGrounded && (centerGrounded || leftGrounded || rightGrounded))
    {
        Debug.Log($"[GROUND] Parcialmente en suelo - Center: {centerGrounded} | Left: {leftGrounded} | Right: {rightGrounded}");
    }
    
    // 5️⃣ DEBUG VISUAL
    Debug.DrawRay(groundCheck.position, Vector2.down * radius, centerColor);
    Debug.DrawRay(leftCheckPos, Vector2.down * radius, leftColor);
    Debug.DrawRay(rightCheckPos, Vector2.down * radius, rightColor);
    
    // 6️⃣ EVENTO DE CAMBIO (Solo si cambió el estado)
    if (wasGrounded != isFullyGrounded)
    {
        events?.InvokeGroundedChanged(new PlayerCollisionData
        {
            WasGrounded = wasGrounded,
            IsGrounded = isFullyGrounded
        });
        wasGrounded = isFullyGrounded;
    }
    
    return isFullyGrounded;
}
```

---

## 🧪 Testing

### Test 1: Borde de Plataforma (El Bug Reportado)

1. **Setup**: Coloca al player en el borde de una plataforma
   ```
        👤
       ────────
   ```

2. **Mueve al player hacia el borde**:
   ```
         👤
       ────────
   ```

3. **Verifica en Console**:
   ```
   [GROUND] Parcialmente en suelo - Center: true | Left: false | Right: true
   ```

4. **Verifica en Scene View**:
   - Raycast Left: **Rojo** ❌
   - Raycast Center: **Verde** ✅
   - Raycast Right: **Verde** ✅

5. **Verifica el comportamiento**:
   - Player entra en **AirState** ✅
   - Player entra en **FallState** ✅
   - Player **NO** activa ledge grab inmediatamente

---

### Test 2: Plataforma Completa

1. **Setup**: Player completamente sobre plataforma
   ```
        👤
    ════════════
   ```

2. **Verifica en Scene View**:
   - Todos los raycasts: **Verde** ✅ ✅ ✅

3. **Verifica el estado**:
   - `isGrounded` = **true**
   - Player en **GroundedState**

---

### Test 3: Salto

1. **Presiona Jump**

2. **Verifica en aire**:
   - Todos los raycasts: **Rojo** ❌ ❌ ❌

3. **Verifica el estado**:
   - `isGrounded` = **false**
   - Player en **JumpState** → **InAirState**

---

## ⚙️ Parámetros Ajustables

### Horizontal Offset (Distancia Lateral)

**Ubicación**: `PlayerCollisionController.CheckIsGrounded()`

```csharp
float horizontalOffset = 0.3f;  // ← AJUSTA AQUÍ
```

**Cómo calcular el valor correcto**:
1. Mide el **ancho de los pies** de tu sprite (no el collider completo)
2. Divide entre 2 para obtener el offset desde el centro
3. Usa ese valor

**Valores recomendados según tipo de sprite**:

| Valor | Uso |
|-------|-----|
| **0.2f** | Sprites con pies muy pequeños/juntos |
| **0.3f** | **Sprites normales (actual)** ✅ - Cubre el ancho de los pies |
| **0.4f** | Sprites con stance muy amplio |
| **0.5f** | Sprites muy anchos o postura de combate |

**⚠️ IMPORTANTE**: Este valor debe representar el **ancho de los pies del personaje**, NO el ancho completo del collider. Si es demasiado grande, el personaje puede "flotar" con un solo pie en el borde.

---

### 2. Ground Check Radius (Alcance Vertical)

**Ubicación**: `PlayerData.asset` ScriptableObject

```
GroundCheckRadius: 0.3  // ← AJUSTA EN INSPECTOR
```

**Valores recomendados**:

| Valor | Uso |
|-------|-----|
| **0.2f** | Detección muy precisa (puede tener problemas en terrenos irregulares) |
| **0.3f** | **Balanceado (actual)** ✅ |
| **0.4f** | Más tolerante (puede detectar suelo antes de tiempo) |

**Recomendación**: Mantén **0.3f**.

---

## 🔍 Debugging

### Ver Logs en Console

Con el player en el borde de una plataforma:

```
[GROUND] Parcialmente en suelo - Center: true | Left: false | Right: true
```

**Interpretación**:
- **Center: true** → El centro del player toca suelo
- **Left: false** → El lado izquierdo está en el aire
- **Right: true** → El lado derecho toca suelo
- **Resultado**: `isFullyGrounded = false` → **AirState**

---

### Ver Raycasts en Scene View

1. Selecciona el **Player** en Hierarchy
2. Entra en **Play Mode**
3. Mira la **Scene View**
4. Los raycasts se dibujan continuamente:
   - Verde = Hit ✅
   - Rojo = No Hit ❌

---

### Caso Especial: Ledge Grab

El sistema de ledge grab ahora verifica:

```csharp
bool currentlyGrounded = CheckIsGrounded();
if (currentlyGrounded)
{
    Debug.Log("[AUTO LEDGE] Cancelado: Player está grounded");
    return false;  // No activar ledge grab
}
```

**Importante**: Si el player está **completamente** sobre el suelo, **NO** puede hacer ledge grab.

---

## 🎯 Flujo de Estados

### Antes (Bug):

```
Player en borde →  Detecta suelo (falso positivo)
                 →  Mantiene GroundedState
                 →  ❌ No entra en AirState
                 →  ❌ Activa ledge grab incorrectamente
```

---

### Ahora (Corregido):

```
Player en borde →  Detecta solo 1-2 raycasts
                 →  isFullyGrounded = false
                 →  ✅ Entra en AirState
                 →  ✅ Entra en FallState
                 →  ✅ Puede activar ledge grab correctamente
```

---

## 🛠️ Troubleshooting

### ❌ Problema: Player "flota" con un solo pie en el borde (Bug reportado)

**Síntoma**:
```
     👤  ← Solo un pie en plataforma
    /|\ 
    / \
    🟢   ← Detecta suelo (incorrecto)
   ─────
```

**Causa**: `horizontalOffset` demasiado grande. Los raycasts laterales están muy lejos del centro y detectan suelo que está fuera del área de contacto real de los pies.

**Solución**:
```csharp
float horizontalOffset = 0.3f;  // Reduce para cubrir solo el ancho de los pies
```

**Cómo encontrar el valor correcto**:
1. **Pausa el juego** cuando el player esté en el borde
2. **Observa la Scene View** - mira dónde están los raycasts
3. **Ajusta `horizontalOffset`** hasta que los raycasts cubran solo el ancho de los pies
4. **Prueba de nuevo**

**Valores según sprite**:
- **Sprite con pies pequeños**: 0.2f - 0.3f
- **Sprite normal**: 0.3f - 0.4f  
- **Sprite con stance amplio**: 0.4f - 0.5f

---

### ❌ Problema: Player se cae demasiado fácil

**Causa**: `horizontalOffset` muy grande (> 0.5f)

**Solución**:
```csharp
float horizontalOffset = 0.40f;  // Reduce el offset
```

---

### ❌ Problema: Player puede colgar mucho del borde

**Causa**: `horizontalOffset` muy pequeño (< 0.3f)

**Solución**:
```csharp
float horizontalOffset = 0.50f;  // Aumenta el offset
```

---

### ❌ Problema: No veo los raycasts en Scene View

**Solución**:
1. Selecciona el **Player** en Hierarchy
2. Activa **Gizmos** en Scene View (botón arriba)
3. Entra en **Play Mode**

---

### ❌ Problema: Player detecta suelo en el aire

**Causa**: `GroundCheckRadius` muy grande (> 0.4f)

**Solución**:
1. Abre `PlayerData.asset`
2. Reduce `GroundCheckRadius` a **0.3f**

---

## 📊 Resumen de Cambios

### Antes:
```csharp
bool isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, ...);
return isGrounded;
```

**Problema**: Solo 1 raycast → Falsos positivos en bordes.

---

### Ahora:
```csharp
bool centerGrounded = Raycast(center);
if (!centerGrounded) return false;  // Early exit

bool leftGrounded = Raycast(center - 0.45f);
bool rightGrounded = Raycast(center + 0.45f);

bool isFullyGrounded = centerGrounded && leftGrounded && rightGrounded;
return isFullyGrounded;
```

**Mejora**: 3 raycasts → Detección precisa de contacto completo.

---

## ✅ Checklist de Verificación

Antes de reportar que no funciona:

- [ ] `horizontalOffset` configurado a **0.45f** en el código
- [ ] `GroundCheckRadius` configurado a **0.3f** en `PlayerData.asset`
- [ ] GroundCheck está en la posición correcta (y = -1.08 en los pies)
- [ ] Layer `Ground` está asignado a las plataformas
- [ ] `WhatIsGround` en `PlayerData` incluye layer `Ground`
- [ ] Los raycasts se ven en Scene View (verde/rojo)
- [ ] Los logs aparecen en Console cuando estás en el borde

---

## 🎉 Resultado Esperado

Con la imagen reportada:

```
     👤  ← Player en el borde
    /|\ 
    / \
   🔴🟢🟢  ← Left no detecta, Center y Right sí
   ────────
```

**Comportamiento**:
1. `CheckIsGrounded()` → **false** ✅
2. Player sale de `GroundedState`
3. Player entra en `AirState`
4. `InAirState` detecta velocity.y < 0 → Entra en `FallState`
5. Player ahora está en **FallState** (cayendo)
6. Puede activar ledge grab si lo desea

**Prevención del bug**: El player NO puede activar ledge grab mientras esté "grounded", así que solo se activa cuando realmente está cayendo.

---

Si el problema persiste, comparte los logs de Console y capturas de Scene View para diagnosticar más. 🔍
