# 💀 Sistema de Muerte - Arquitectura SO (Sin Managers)

## 🎯 Filosofía de Diseño

**SIN Singletons** - **SIN Managers** - **100% ScriptableObjects Events**

Esta implementación sigue el patrón de arquitectura basada en ScriptableObjects presentado por Ryan Hipple en Unite Austin 2017.

---

## 🏗️ Arquitectura Completa

```
┌──────────────────────────────────────────────────┐
│           SCRIPTABLE OBJECTS (Assets)            │
├──────────────────────────────────────────────────┤
│                                                   │
│  📁 Events (SO)                                   │
│  ├─ PlayerDeathEvent.asset                       │
│  ├─ ShowDeathScreenEvent.asset                   │
│  └─ PlayerRespawnEvent.asset                     │
│                                                   │
│  📁 Data (SO)                                     │
│  └─ DeathData.asset                              │
│                                                   │
└──────────────────────────────────────────────────┘
                    ↓ drag & drop
┌──────────────────────────────────────────────────┐
│            MONOBEHAVIOUR LISTENERS               │
├──────────────────────────────────────────────────┤
│                                                   │
│  🎮 Player GameObject                            │
│  ├─ PlayerDeathHandler (escucha OnDeath)         │
│  └─ PlayerRespawnHandler (escucha RespawnEvent)  │
│                                                   │
│  📺 DeathCanvas GameObject                        │
│  └─ DeathUIController (escucha ShowDeathScreen)  │
│                                                   │
└──────────────────────────────────────────────────┘
```

---

## ✨ Ventajas de esta Arquitectura

| Aspecto | Singleton Manager | SO Architecture |
|---------|------------------|-----------------|
| **Acoplamiento** | Alto - Todos llaman al Manager | Bajo - Eventos desacoplados |
| **Testeable** | Difícil - Necesitas escena completa | Fácil - Puedes testear SOs independientes |
| **Reutilizable** | No - Atado a la escena | Sí - SOs funcionan en cualquier escena |
| **Escalable** | No - Más features = Manager gigante | Sí - Agrega listeners sin tocar código |
| **Inspector** | Referencias complejas | Drag & drop de assets |
| **Debugging** | Difícil - Singleton oculta flujo | Fácil - Ves eventos en runtime |

---

## 📦 Componentes del Sistema

### 1. **ScriptableObject Events**

#### **PlayerDeathEvent.cs**
```csharp
// Ubicación: /Assets/Scripts/Events/
// Dispara cuando el jugador muere
Raise(DeathType deathType, Vector3 deathPosition)
```

**Uso:**
- `PlayerDeathHandler` → Raise cuando `HealthController.OnDeath`
- Listeners pueden reaccionar (efectos, sonidos, estadísticas)

---

#### **ShowDeathScreenEvent.cs**
```csharp
// Ubicación: /Assets/Scripts/Events/
// Comando para mostrar la pantalla de muerte
Raise(DeathType deathType)
```

**Uso:**
- `PlayerDeathHandler` → Raise después de animación de muerte
- `DeathUIController` → Listen y muestra UI

---

#### **PlayerRespawnEvent.cs**
```csharp
// Ubicación: /Assets/Scripts/Events/
// Dispara cuando el jugador debe respawnear
Raise()
```

**Uso:**
- `DeathUIController` → Raise cuando player presiona "Respawn"
- `PlayerRespawnHandler` → Listen y ejecuta lógica de respawn

---

### 2. **ScriptableObject Data**

#### **DeathData.cs**
```csharp
// Ubicación: /Assets/Scripts/Data/
// Almacena configuración y estado de muerte
```

**Propiedades:**
- Normal Death Duration: `2f`
- Fall Death Duration: `1f`
- Fall Death Threshold: `20f`
- Death Messages (customizables)
- Runtime State (última posición segura, tipo de muerte, etc.)

---

### 3. **MonoBehaviour Listeners**

#### **PlayerDeathHandler.cs**
**Ubicación:** `/Assets/Scripts/Player/`  
**Responsabilidad:** Escucha `HealthController.OnDeath` y dispara eventos

```csharp
// Escucha
healthController.OnDeath += HandleDeath

// Dispara
onPlayerDeathEvent.Raise(currentDeathType, deathPosition)
showDeathScreenEvent.Raise(deathData.CurrentDeathType) // Después de animación
```

---

#### **PlayerRespawnHandler.cs**
**Ubicación:** `/Assets/Scripts/Player/`  
**Responsabilidad:** Maneja lógica de respawn

```csharp
// Escucha
onPlayerRespawnEvent.AddListener(HandleRespawn)

// Ejecuta
- Restaura posición
- Resetea salud
- Cambia a IdleState
```

---

#### **DeathUIController.cs**
**Ubicación:** `/Assets/Scripts/UI/`  
**Responsabilidad:** Muestra/oculta UI de muerte

```csharp
// Escucha
showDeathScreenEvent.AddListener(ShowDeathScreen)

// Ejecuta
- Muestra panel
- Actualiza textos
- Pausa juego (Time.timeScale = 0)
```

---

## 🎯 Setup Paso a Paso

### PASO 1: Crear ScriptableObjects (Assets)

1. **Crear DeathData**
```
Assets/SO → Click derecho → Create → TheHunt/Data/Death Data
Nombre: DeathData
```

2. **Crear PlayerDeathEvent**
```
Assets/SO → Click derecho → Create → TheHunt/Events/Player Death Event
Nombre: PlayerDeathEvent
```

3. **Crear ShowDeathScreenEvent**
```
Assets/SO → Click derecho → Create → TheHunt/Events/Show Death Screen Event
Nombre: ShowDeathScreenEvent
```

4. **Crear PlayerRespawnEvent**
```
Assets/SO → Click derecho → Create → TheHunt/Events/Player Respawn Event
Nombre: PlayerRespawnEvent
```

---

### PASO 2: Configurar DeathData

Selecciona `DeathData.asset` en el Inspector:

```
Death Settings:
- Normal Death Duration: 2
- Fall Death Duration: 1
- Fall Death Threshold: 20

Death Messages:
- Normal Death Title: "HAS MUERTO"
- Fall Death Title: "CAÍDA MORTAL"
- Normal Death Message: "Presiona para continuar"
- Fall Death Message: "Cuidado con las alturas"
```

---

### PASO 3: Configurar Player

Selecciona **Player** GameObject:

1. **Add Component** → `PlayerDeathHandler`
   - Death Data: Arrastra `DeathData.asset`
   - On Player Death Event: Arrastra `PlayerDeathEvent.asset`
   - Show Death Screen Event: Arrastra `ShowDeathScreenEvent.asset`
   - Player: Auto-asignado
   - Health Controller: Auto-asignado

2. **Add Component** → `PlayerRespawnHandler`
   - Death Data: Arrastra `DeathData.asset`
   - On Player Respawn Event: Arrastra `PlayerRespawnEvent.asset`
   - Player: Auto-asignado
   - Health Controller: Auto-asignado

---

### PASO 4: Configurar DeathCanvas (UI)

Crea la estructura:

```
DeathCanvas
├── DeathPanel (Image - fondo negro alpha 180)
    ├── DeathTitle (TextMeshProUGUI)
    ├── DeathMessage (TextMeshProUGUI)
    └── ButtonsContainer (Vertical Layout Group)
        ├── RespawnButton
        ├── RestartButton
        └── QuitButton
```

Selecciona **DeathCanvas**:

**Add Component** → `DeathUIController`
- Death Data: Arrastra `DeathData.asset`
- Show Death Screen Event: Arrastra `ShowDeathScreenEvent.asset`
- Player Respawn Event: Arrastra `PlayerRespawnEvent.asset`
- Death Panel: Arrastra `DeathPanel`
- Death Title Text: Arrastra `DeathTitle`
- Death Message Text: Arrastra `DeathMessage`
- Respawn/Restart/Quit Buttons: Arrastra los botones
- Fade In Duration: `0.5`

---

### PASO 5: Conectar DeathState con DeathData

En `Player.cs` Awake, después de crear `DeathState`:

```csharp
// En tu código, necesitas inyectar el DeathData al estado
// Puedes hacerlo agregando un campo en Player:

[Header("Death Settings")]
[SerializeField] private DeathData deathData;

// Y en Awake, después de crear DeathState:
if (deathData != null)
{
    DeathState.SetDeathData(deathData);
}
```

---

## 🔄 Flujo Completo de Muerte

### Muerte Normal

```
1. Player recibe daño
2. HealthController.TakeDamage()
3. Salud = 0
4. HealthController.OnDeath event
   ↓
5. PlayerDeathHandler.HandleDeath()
   ├─ Guarda posición en DeathData
   ├─ Desactiva input
   ├─ Dispara PlayerDeathEvent ✉️
   └─ Cambia a PlayerDeathState
   ↓
6. PlayerDeathState (animación 2s)
   ↓
7. PlayerDeathState timer = 0
8. PlayerDeathHandler.OnDeathAnimationComplete()
   └─ Dispara ShowDeathScreenEvent ✉️
   ↓
9. DeathUIController.ShowDeathScreen()
   ├─ Muestra UI "HAS MUERTO"
   ├─ Pausa juego (timeScale = 0)
   └─ Espera input del jugador
   ↓
10. Player presiona "Respawn"
11. DeathUIController.OnRespawnClicked()
    ├─ Oculta UI
    ├─ Resume tiempo
    └─ Dispara PlayerRespawnEvent ✉️
    ↓
12. PlayerRespawnHandler.HandleRespawn()
    ├─ Restaura posición desde DeathData
    ├─ Resetea salud
    ├─ Activa input
    ├─ Cambia a IdleState
    └─ Limpia DeathData
```

### Muerte por Caída

```
1. PlayerAirState detecta caída alta
2. Llama PlayerDeathHandler.CheckForFallDeath(fallHeight)
3. Si fallHeight >= 20m:
   ├─ SetDeathType(DeathType.Fall)
   └─ HealthController.TakeDamage(toda la vida)
   ↓
4. Sigue flujo normal pero con:
   ├─ Animación más rápida (1s)
   └─ Mensaje "CAÍDA MORTAL"
```

---

## 🧪 Testing

### Test 1: Eventos en Runtime
1. Play Mode
2. Window → Analysis → Event Debugger (si tienes)
3. O simplemente observa los Debug.Log con colores

### Test 2: Muerte Normal
```csharp
// En Inspector, reduce Health a 0
// Deberías ver en consola:
[HEALTH] Player has died!
[DEATH EVENT] Raised - Type: Normal
[DEATH STATE] Player has died. Fall death: False, Duration: 2s
[SHOW DEATH SCREEN] Type: Normal
```

### Test 3: Modificar Mensajes
1. Selecciona `DeathData.asset`
2. Cambia "HAS MUERTO" por "GAME OVER"
3. Play y mata al player
4. ✅ Debe mostrar "GAME OVER"

### Test 4: Multiple Listeners
Puedes agregar más listeners sin modificar código:

```csharp
public class DeathSoundController : MonoBehaviour
{
    [SerializeField] private PlayerDeathEvent onPlayerDeathEvent;
    [SerializeField] private AudioClip deathSound;
    
    void Start()
    {
        onPlayerDeathEvent.AddListener(HandleDeath);
    }
    
    void HandleDeath(DeathType type, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(deathSound, position);
    }
}
```

---

## 🆚 Comparación con Manager Singleton

### ❌ Código Anterior (Singleton)
```csharp
// Desde cualquier script:
PlayerDeathManager.Instance.CheckForFallDeath(25f);
PlayerDeathManager.Instance.Respawn();

// Problemas:
// - Acoplamiento fuerte
// - Singleton global
// - Difícil de testear
// - No reutilizable
```

### ✅ Código Nuevo (SO Events)
```csharp
// Desde cualquier script:
[SerializeField] private PlayerDeathEvent deathEvent;
deathEvent.Raise(DeathType.Fall, transform.position);

[SerializeField] private PlayerRespawnEvent respawnEvent;
respawnEvent.Raise();

// Ventajas:
// - Desacoplado totalmente
// - Sin globals
// - Testeable
// - Reutilizable entre escenas
```

---

## 📝 Extensibilidad

### Agregar Listener de Muerte (Sin modificar código)

**Ejemplo: Sistema de Estadísticas**

```csharp
public class DeathStatistics : MonoBehaviour
{
    [SerializeField] private PlayerDeathEvent onPlayerDeathEvent;
    private int totalDeaths;
    private int fallDeaths;
    
    void Start()
    {
        onPlayerDeathEvent.AddListener(OnPlayerDied);
    }
    
    void OnPlayerDied(DeathType type, Vector3 position)
    {
        totalDeaths++;
        if (type == DeathType.Fall)
            fallDeaths++;
            
        Debug.Log($"Deaths: {totalDeaths}, Fall: {fallDeaths}");
    }
}
```

Solo necesitas:
1. Crear el script
2. Arrastra `PlayerDeathEvent.asset`
3. ¡Ya funciona! Sin tocar PlayerDeathHandler

---

## 🎨 Personalización

### Cambiar Duraciones
`DeathData.asset` → Inspector:
- Normal Death Duration: `2` → Cambia a lo que quieras
- Fall Death Duration: `1` → Cambia a lo que quieras

### Cambiar Mensajes
`DeathData.asset` → Inspector:
- Títulos y mensajes editables sin código

### Agregar Nuevo Tipo de Muerte
1. En `PlayerDeathEvent.cs`:
```csharp
public enum DeathType
{
    Normal,
    Fall,
    Instant,
    Fire,      // Nuevo
    Poison     // Nuevo
}
```

2. Agrega duraciones y mensajes en `DeathData`
3. ¡Listo! No necesitas modificar handlers

---

## 📋 Resumen de Archivos

### Scripts Creados
| Archivo | Ubicación | Tipo |
|---------|-----------|------|
| `PlayerDeathEvent.cs` | `/Assets/Scripts/Events/` | SO Event |
| `ShowDeathScreenEvent.cs` | `/Assets/Scripts/Events/` | SO Event |
| `PlayerRespawnEvent.cs` | `/Assets/Scripts/Events/` | SO Event |
| `DeathData.cs` | `/Assets/Scripts/Data/` | SO Data |
| `PlayerDeathHandler.cs` | `/Assets/Scripts/Player/` | MonoBehaviour |
| `PlayerRespawnHandler.cs` | `/Assets/Scripts/Player/` | MonoBehaviour |
| `DeathUIController.cs` | `/Assets/Scripts/UI/` | MonoBehaviour |
| `PlayerDeathState.cs` | `/Assets/Scripts/Player/PlayerStates/SubStates/` | State |

### Assets a Crear
- `DeathData.asset` → `/Assets/SO/`
- `PlayerDeathEvent.asset` → `/Assets/SO/Events/`
- `ShowDeathScreenEvent.asset` → `/Assets/SO/Events/`
- `PlayerRespawnEvent.asset` → `/Assets/SO/Events/`

---

## ✅ Checklist de Setup

- [ ] Crear 4 ScriptableObject assets (DeathData + 3 Events)
- [ ] Configurar DeathData con duraciones y mensajes
- [ ] Agregar PlayerDeathHandler al Player
- [ ] Agregar PlayerRespawnHandler al Player
- [ ] Asignar referencias SO en ambos handlers
- [ ] Crear Canvas UI con estructura de muerte
- [ ] Agregar DeathUIController al Canvas
- [ ] Asignar referencias SO y UI en DeathUIController
- [ ] Inyectar DeathData en DeathState (Player.cs)
- [ ] Configurar Animator con parámetro "death"
- [ ] Testear muerte normal
- [ ] Testear muerte por caída

---

## 🎯 Siguiente Paso

1. Crea los 4 ScriptableObject assets
2. Sigue el setup paso a paso
3. Testea el flujo completo
4. Disfruta de una arquitectura limpia y escalable

**¡Sin Singletons! ¡Sin Managers! ¡100% SO Architecture!** 🎮✨
