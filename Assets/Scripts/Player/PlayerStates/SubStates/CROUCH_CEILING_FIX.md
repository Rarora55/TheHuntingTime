# 🔧 Fix: Crouch System - Bloqueo por Techo

## 🐛 Problema Original

El jugador podía **levantarse en medio de un pasadizo bajo** simplemente soltando el input de crouch, incluso cuando había un techo encima que debería impedirlo.

### Comportamiento Incorrecto:
```
1. Jugador entra agachado bajo un muro bajo
2. Jugador suelta el input de crouch (deja de presionar ⬇️)
3. Personaje SE LEVANTA atravesando el techo ❌
```

---

## ✅ Solución Implementada

El jugador ahora **permanece obligatoriamente agachado** cuando hay un techo, sin importar si suelta el input de crouch.

### Comportamiento Correcto:
```
1. Jugador entra agachado bajo un muro bajo
2. Jugador suelta el input de crouch (deja de presionar ⬇️)
3. Sistema detecta techo: isTouchingCeiling == true
4. Personaje PERMANECE AGACHADO ✅
5. Log: "No se puede levantar: hay techo encima"
```

---

## 🔧 Cambios Realizados

### 1. **PlayerGroundState.cs** - Detección continua de techo

**Antes:**
```csharp
public override void DoChecks()
{
    base.DoChecks();
    bool wasGrounded = isGrounded;
    isGrounded = player.CheckIsGrounded();
    isTouchingWall = player.CheckIfTouchingWall();
    // ❌ No se actualizaba isTouchingCeiling
}
```

**Ahora:**
```csharp
public override void DoChecks()
{
    base.DoChecks();
    bool wasGrounded = isGrounded;
    isGrounded = player.CheckIsGrounded();
    isTouchingWall = player.CheckIfTouchingWall();
    isTouchingCeiling = player.CheckForCeiling(); // ✅ Ahora se actualiza cada frame
}
```

---

### 2. **PlayerCrouchIdleState.cs** - Bloqueo al intentar levantarse

**Antes:**
```csharp
if (xInput != 0)
    stateMachine.ChangeState(player.CrouchMoveState);
else if (yInput != -1 && !isTouchingCeiling) // ❌ Ambas condiciones en una línea
    stateMachine.ChangeState(player.IdleState);
```

**Problema**: Si `yInput != -1` Y `!isTouchingCeiling`, ambas condiciones deben cumplirse para cambiar. Pero si sueltas el input, `yInput` es `0` (no `-1`), entonces la primera parte es `true`, y si `isTouchingCeiling` es `false` (por no actualizarse), cambiaba a Idle.

**Ahora:**
```csharp
if (xInput != 0)
{
    stateMachine.ChangeState(player.CrouchMoveState);
}
else if (yInput != -1) // Jugador no está presionando abajo
{
    if (!isTouchingCeiling) // ✅ Verificación explícita de techo
    {
        stateMachine.ChangeState(player.IdleState);
    }
    else
    {
        Debug.Log("<color=yellow>[CROUCH IDLE] No se puede levantar: hay techo encima</color>");
    }
}
```

---

### 3. **PlayerCrouchMoveState.cs** - Mismo bloqueo en movimiento

**Antes:**
```csharp
if (xInput == 0)
    stateMachine.ChangeState(player.CrouchIdleState);
else if (yInput != -1 && !isTouchingCeiling) // ❌ Mismo problema
    stateMachine.ChangeState(player.MoveState);
```

**Ahora:**
```csharp
if (xInput == 0)
{
    stateMachine.ChangeState(player.CrouchIdleState);
}
else if (yInput != -1) // Jugador no está presionando abajo
{
    if (!isTouchingCeiling) // ✅ Verificación explícita de techo
    {
        stateMachine.ChangeState(player.MoveState);
    }
    else
    {
        Debug.Log("<color=yellow>[CROUCH MOVE] No se puede levantar: hay techo encima</color>");
    }
}
```

---

## 🎯 Lógica de Transición

### Condiciones para SALIR de Crouch:

| Input | Ceiling | Resultado |
|-------|---------|-----------|
| yInput == -1 (⬇️ presionado) | ❌ No importa | **Permanece en Crouch** |
| yInput != -1 (⬇️ soltado) | ✅ Hay techo | **Permanece en Crouch** ⚠️ **NUEVO** |
| yInput != -1 (⬇️ soltado) | ❌ Sin techo | **Sale a Idle/Move** ✅ |

---

## 🎮 Flujo de Estados

```
┌─────────────────────────────────────────────────────┐
│         CrouchIdle / CrouchMove                     │
│  (Jugador agachado bajo un muro bajo)               │
└─────────────────────────────────────────────────────┘
                      │
                      │ Cada frame:
                      │ DoChecks() actualiza isTouchingCeiling
                      │
                      ▼
         ┌────────────────────────────┐
         │   yInput != -1?            │ ← Jugador suelta crouch
         │   (No presiona ⬇️)          │
         └────────────────────────────┘
                      │
            ┌─────────┴─────────┐
            │                   │
      isTouchingCeiling?   isTouchingCeiling?
        == true             == false
            │                   │
            ▼                   ▼
    ┌──────────────┐    ┌──────────────┐
    │ PERMANECE    │    │ CAMBIA A     │
    │ en Crouch    │    │ Idle / Move  │
    └──────────────┘    └──────────────┘
         ⚠️                    ✅
```

---

## 🧪 Testing

### Escenarios a Probar:

#### ✅ Caso 1: Pasadizo Bajo
```
Entrada:
  ──────────────  ← Techo
       🧍
  ──────────────  ← Suelo

1. Jugador presiona ⬇️ → Entra en crouch
2. Jugador suelta ⬇️ → Permanece agachado
3. Log: "No se puede levantar: hay techo encima"
```

#### ✅ Caso 2: Espacio Abierto
```
  (sin techo)
       🧍
  ──────────────  ← Suelo

1. Jugador presiona ⬇️ → Entra en crouch
2. Jugador suelta ⬇️ → Se levanta a Idle
```

#### ✅ Caso 3: Moviéndose Bajo Techo
```
  ──────────────  ← Techo
    🧍 →
  ──────────────  ← Suelo

1. Jugador agachado + moviéndose (CrouchMove)
2. Jugador suelta ⬇️ → Permanece agachado moviéndose
3. Jugador sale del techo → Ahora puede levantarse
```

---

## 📊 Diferencias Técnicas

### Antes vs Ahora

| Aspecto | Antes ❌ | Ahora ✅ |
|---------|----------|----------|
| **Actualización ceiling** | No se actualizaba en DoChecks() | Se actualiza cada frame |
| **Lógica de salida** | Condición única `&&` | Verificación explícita anidada |
| **Detección techo** | Inconsistente | Siempre actualizada |
| **Feedback jugador** | Ninguno | Log de debug cuando bloqueado |
| **Comportamiento** | Atravesaba techo | Permanece agachado |

---

## 🐛 Debug

### Logs Disponibles:

**Cuando intenta levantarse con techo:**
```
[CROUCH IDLE] No se puede levantar: hay techo encima
[CROUCH MOVE] No se puede levantar: hay techo encima
```

### Cómo Verificar:

1. Entra en modo Play
2. Ve a Console (⌘/Ctrl + Shift + C)
3. Agáchate bajo un techo
4. Suelta el input de crouch
5. Verifica que aparece el log amarillo

---

## 💡 Notas Importantes

1. **La detección de techo usa `ceilingCheck` transform** definido en el Player
2. **El radio de detección está en `PlayerData.GroundCheckRadius`**
3. **El layer de detección es `PlayerData.WhatIsGround`**
4. **El jugador puede salir de crouch presionando salto** (transición a JumpState tiene prioridad)

---

## 📁 Archivos Modificados

1. `/Assets/Scripts/Player/PlayerStates/SuperStates/PlayerGroundState.cs`
   - Añadido `isTouchingCeiling = player.CheckForCeiling()` en `DoChecks()`

2. `/Assets/Scripts/Player/PlayerStates/SubStates/PlayerCrouchIdleState.cs`
   - Refactorizada condición de salida a verificación explícita
   - Añadido log de debug

3. `/Assets/Scripts/Player/PlayerStates/SubStates/PlayerCrouchMoveState.cs`
   - Refactorizada condición de salida a verificación explícita
   - Añadido log de debug

---

## ✅ Resultado Final

El jugador ahora **no puede atravesar techos** al soltar el input de crouch. El sistema **detecta automáticamente** cuando hay un obstáculo arriba y **bloquea la transición** a estados de pie hasta que haya espacio libre.

¡Bug corregido! 🎉
