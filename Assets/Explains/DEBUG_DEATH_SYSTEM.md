# 🐛 DEBUG: Sistema de Muerte - Guía de Resolución

## 🚨 **Problemas Reportados**

1. ❌ **No entra en animación de muerte** - El player muere pero no muestra animación
2. ❌ **Loop infinito de muerte** - Al respawnear, vuelve a morir constantemente

---

## ✅ **Fixes Aplicados**

### **1. Inyección de DeathData en DeathState**

**Problema:** `PlayerDeathState` no tenía acceso a `DeathData`

**Solución:** En `Player.cs` Start(), agregué:
```csharp
PlayerDeathHandler deathHandler = GetComponent<PlayerDeathHandler>();
if (deathHandler != null && deathHandler.GetDeathData() != null)
{
    DeathState.SetDeathData(deathHandler.GetDeathData());
}
```

---

### **2. Reset del Timer en Exit()**

**Problema:** El timer quedaba en `float.MaxValue` y nunca se reseteaba

**Solución:** En `PlayerDeathState.Exit()`:
```csharp
public override void Exit()
{
    base.Exit();
    isDeathByFall = false;
    deathTimer = 0f;  // ✅ NUEVO: Resetea el timer
}
```

---

### **3. Protección contra Timer Negativo**

**Problema:** El timer podía seguir decrementando infinitamente

**Solución:** En `PlayerDeathState.LogicUpdate()`:
```csharp
if (deathTimer > 0f && deathTimer != float.MaxValue)
{
    deathTimer -= Time.deltaTime;
    // Solo ejecuta si está en rango válido
}
```

---

## 🔍 **Debugging: Paso a Paso**

### **PASO 1: Verificar Parámetro del Animator**

El problema de "no entra en animación" probablemente es que **falta el parámetro "death" en el Animator**.

#### **1.1 Verificar que existe el parámetro**

1. Selecciona **Player 1.2** en la jerarquía
2. En el Inspector, encuentra el componente **Animator**
3. Haz clic en el **Controller** (debe ser `/Assets/Animations/Animators Controller/Mono/Player.controller`)
4. Se abrirá la ventana del **Animator**
5. En la pestaña **Parameters** (izquierda), verifica si existe un parámetro llamado **`death`** de tipo **Bool**

#### **1.2 Si NO existe, créalo**

1. En la ventana del Animator, pestaña **Parameters**
2. Click en **"+"** → **Bool**
3. Nómbralo exactamente: **`death`**

#### **1.3 Crear Estado de Muerte (si no existe)**

1. En la ventana del Animator, click derecho en el canvas → **Create State → Empty**
2. Nómbralo **`Death`**
3. Asigna la animación de muerte (si tienes una)
4. **IMPORTANTE:** NO crees transiciones desde otros estados a Death (el código lo maneja)

---

### **PASO 2: Agregar DeathSystemDebugger**

Para ver qué está pasando en runtime:

1. Selecciona **Player 1.2**
2. **Add Component** → busca `DeathSystemDebugger`
3. Arrastra referencias:
   - **Player:** Auto-asignado
   - **Death Data:** Arrastra `DeathData.asset`
   - **Animator:** Auto-asignado
4. Marca **Show Debug Info** como `true`

---

### **PASO 3: Testear con Debug Info**

1. **Play Mode**
2. En la esquina superior izquierda verás un panel con:
   ```
   Current State: PlayerIdleState
   Is Dead: False
   Death Type: Normal
   Last Safe Position: (x, y, z)
   
   Animator Parameters:
     idle: True
     move: False
     inAir: False
     death: False   ← IMPORTANTE: Debe existir
   ```

3. **Mata al player** (reduce vida a 0)

4. **Observa el panel:**
   ```
   Current State: PlayerDeathState   ← Debe cambiar a esto
   Is Dead: True
   Death Type: Normal
   
   Animator Parameters:
     death: True   ← Debe cambiar a True
   ```

5. **Después de 2 segundos** (Normal Death Duration), debe aparecer la UI de muerte

6. **Presiona "Respawn"**

7. **Observa el panel:**
   ```
   Current State: PlayerIdleState   ← Debe volver a Idle
   Is Dead: False   ← Debe volver a False
   ```

---

## 🎯 **Posibles Problemas y Soluciones**

### **Problema 1: "death" parameter no existe**

**Síntoma:**
- No entra en animación de muerte
- El debugger muestra que falta el parámetro "death"

**Solución:**
- Crear parámetro "death" (Bool) en el Animator Controller

---

### **Problema 2: Loop infinito después de respawn**

**Síntoma:**
- Al respawnear, `IsDead` sigue en `true`
- Vuelve a mostrar UI de muerte
- El debugger muestra `Is Dead: True` constantemente

**Posibles causas:**

#### **Causa A: DeathData no se limpia**

Verifica en `PlayerRespawnHandler.HandleRespawn()`:
```csharp
deathData.ClearDeathState();  // ← DEBE estar esta línea
```

#### **Causa B: HealthController no resetea**

Verifica en `PlayerRespawnHandler.HandleRespawn()`:
```csharp
healthController.ResetHealth();  // ← DEBE estar esta línea
```

#### **Causa C: No cambia a IdleState**

Verifica en `PlayerRespawnHandler.HandleRespawn()`:
```csharp
player.StateMachine.ChangeState(player.IdleState);  // ← DEBE cambiar estado
```

---

### **Problema 3: UI de muerte aparece inmediatamente (sin animación)**

**Síntoma:**
- Mueres y la UI aparece al instante
- No hay delay de 2 segundos

**Posibles causas:**

#### **Causa A: Time.timeScale = 0**

El timer usa `Time.deltaTime`, que se afecta por `Time.timeScale`.

**Solución temporal:** En `PlayerDeathState.LogicUpdate()`:
```csharp
deathTimer -= Time.unscaledDeltaTime;  // Usar unscaledDeltaTime
```

#### **Causa B: DeathData no está asignado**

El timer usa duraciones por defecto (2s normal, 1s caída) si `deathData == null`.

Verifica que en **PlayerDeathHandler** tienes asignado `DeathData.asset`.

---

### **Problema 4: No responde al botón "Respawn"**

**Síntoma:**
- UI de muerte aparece
- Presionas "Respawn" pero nada pasa

**Posibles causas:**

#### **Causa A: PlayerRespawnEvent no está asignado**

Verifica en **DeathUIController** que tienes asignado `PlayerRespawnEvent.asset`.

#### **Causa B: Time.timeScale = 0 bloquea el evento**

Verifica en `DeathUIController.OnRespawnClicked()`:
```csharp
void OnRespawnClicked()
{
    HideDeathScreen();  // ← Debe llamar esto primero (resume Time.timeScale)
    
    if (playerRespawnEvent != null)
    {
        playerRespawnEvent.Raise();
    }
}
```

---

## 📋 **Checklist de Verificación**

### **Setup Básico**
- [ ] DeathData.asset existe en `/Assets/SO/`
- [ ] PlayerDeathEvent.asset existe en `/Assets/SO/`
- [ ] ShowDeathScreenEvent.asset existe en `/Assets/SO/`
- [ ] PlayerRespawnEvent.asset existe en `/Assets/SO/`

### **Player GameObject**
- [ ] Tiene componente `PlayerDeathHandler`
  - [ ] Death Data asignado
  - [ ] On Player Death Event asignado
  - [ ] Show Death Screen Event asignado
- [ ] Tiene componente `PlayerRespawnHandler`
  - [ ] Death Data asignado
  - [ ] On Player Respawn Event asignado
- [ ] Tiene componente `HealthController`

### **DeathCanvas (UI)**
- [ ] Tiene componente `DeathUIController`
  - [ ] Death Data asignado
  - [ ] Show Death Screen Event asignado
  - [ ] Player Respawn Event asignado
  - [ ] Todas las referencias UI asignadas

### **Animator Controller**
- [ ] Parámetro "death" (Bool) existe
- [ ] Estado "Death" existe (opcional, pero recomendado)

---

## 🧪 **Test Manual**

Ejecuta estos tests en orden:

### **Test 1: Muerte Normal**
1. Play Mode
2. Reduce vida del player a 0
3. **Esperado:**
   - Entra en animación de muerte
   - Después de 2 segundos, aparece UI "HAS MUERTO"
   - Presionas "Respawn"
   - Player aparece en última posición segura
   - Vuelve a IdleState
   - Vida completa

### **Test 2: Muerte por Caída**
(Requiere implementar detección de caída en PlayerAirState)

1. Play Mode
2. Salta desde gran altura (>20m)
3. **Esperado:**
   - Entra en animación de muerte (más rápida)
   - Después de 1 segundo, aparece UI "CAÍDA MORTAL"
   - Mismo comportamiento de respawn

### **Test 3: Múltiples Muertes**
1. Play Mode
2. Muere 3 veces seguidas
3. **Esperado:**
   - Cada muerte funciona correctamente
   - No hay loops infinitos
   - Respawn siempre funciona

---

## 🔧 **Debugging Avanzado**

### **Logs a Verificar**

Cuando mueres, deberías ver en consola (en este orden):

```
1. [HEALTH] Player has died!
2. [DEATH EVENT] Raised - Type: Normal
3. [DEATH STATE] Player has died. Fall death: False, Duration: 2s
4. (después de 2s)
5. [SHOW DEATH SCREEN] Type: Normal
6. (presionas Respawn)
7. [RESPAWN EVENT] Raised
8. [RESPAWN HANDLER] Player respawned
```

Si falta algún log, indica dónde está el problema.

---

### **Si el Debug Panel no aparece**

Verifica en `DeathSystemDebugger.cs` que `showDebugInfo = true`.

Si sigue sin aparecer, agrega logs manuales:

```csharp
void Update()
{
    Debug.Log($"Current State: {player?.StateMachine?.CurrentState?.GetType().Name}");
    Debug.Log($"Is Dead: {deathData?.IsDead}");
}
```

---

## 🎯 **Siguiente Paso**

1. **Agrega `DeathSystemDebugger`** al Player
2. **Verifica parámetro "death"** en Animator
3. **Testea** y observa el debug panel
4. **Reporta** qué ves en el panel cuando mueres

Basándome en los resultados del debug, puedo ayudarte a identificar exactamente dónde está el problema.

---

## 📝 **Resumen de Cambios Aplicados**

| Archivo | Cambio | Razón |
|---------|--------|-------|
| `Player.cs` | Inyecta DeathData en DeathState | DeathState necesita acceso a duraciones |
| `PlayerDeathHandler.cs` | Agregado `GetDeathData()` | Player.cs necesita obtener referencia |
| `PlayerDeathState.cs` | Reset timer en Exit() | Evita loop infinito |
| `PlayerDeathState.cs` | Protección en LogicUpdate() | Evita decrementar timer infinitamente |
| `DeathSystemDebugger.cs` | Nuevo script de debugging | Ver estado en runtime |

---

**¡Usa el debugger y reporta qué ves!** 🎮🐛
