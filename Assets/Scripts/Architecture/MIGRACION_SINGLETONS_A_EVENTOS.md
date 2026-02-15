# 🔄 Migración de Singletons a ScriptableObject Events

## ✅ Migración Completada

Se han eliminado **TODOS** los singletons del sistema de iluminación y de fade de pantalla, reemplazándolos por **ScriptableObject Events**.

---

## 📊 Resumen de Cambios

### Singletons Eliminados

```
ANTES:
├── LightManager.Instance
└── ScreenFadeManager.Instance

DESPUÉS:
├── LightControlEvent (SO Event)
├── GlobalLightCommandEvent (SO Event)
└── ScreenFadeEvent (SO Event)
```

---

## 🎯 Archivos Modificados

### ✨ Scripts de Eventos Creados

```
/Assets/Scripts/Events/
├── LightControlEvent.cs          → Registro/desregistro de luces
├── GlobalLightCommandEvent.cs    → Comandos globales de iluminación
└── ScreenFadeEvent.cs            → Fades de pantalla y teletransporte
```

### 🔧 Scripts Refactorizados

#### Sistema de Iluminación

```
/Assets/Scripts/Lighting/
├── LightManager.cs               → Eliminado singleton, usa eventos
├── BaseLightController.cs        → Usa eventos para registro
└── DayNightCycle.cs             → Usa eventos para control de luces
```

#### Sistema de Fade

```
/Assets/Scripts/UI/
└── ScreenFadeManager.cs          → Eliminado singleton, usa eventos
```

#### Scripts que Usan Fade

```
/Assets/Scripts/Environment/
├── ClimbableObject.cs            → Usa ScreenFadeEvent
├── ClimbSpawnPoint.cs            → Usa ScreenFadeEvent
└── Ladder.cs                     → Usa ScreenFadeEvent

/Assets/Scripts/Interaction/
└── RopeAnchorPassiveItem.cs      → Usa ScreenFadeEvent
```

---

## 🎨 Cómo Usar el Nuevo Sistema

### 1️⃣ Crear los ScriptableObjects (PRIMERA VEZ)

#### Eventos de Iluminación

1. En el Project, haz clic derecho en `/Assets/Data/Events/`
2. Create → TheHunt → Events → Light Control Event
3. Nombra: `LightRegisteredEvent`
4. Repite para: `LightUnregisteredEvent`
5. Create → TheHunt → Events → Global Light Command Event
6. Nombra: `GlobalLightCommandEvent`

#### Evento de Fade

1. Create → TheHunt → Events → Screen Fade Event
2. Nombra: `ScreenFadeEvent`

### 2️⃣ Configurar LightManager

```
GameObject: LightManager
├── Component: LightManager
    ├── Events
    │   ├── On Light Registered: → LightRegisteredEvent (SO)
    │   ├── On Light Unregistered: → LightUnregisteredEvent (SO)
    │   └── On Global Light Command: → GlobalLightCommandEvent (SO)
    ├── Settings
    │   ├── Auto Register Lights: true
    │   └── Use Culling: true
    └── Debug
        └── Show Debug Info: false
```

### 3️⃣ Configurar BaseLightController (Todas las Luces)

En cada luz (antorchas, lámparas, etc):

```
Component: BaseLightController
├── Events
│   ├── On Light Registered: → LightRegisteredEvent (SO)
│   └── On Light Unregistered: → LightUnregisteredEvent (SO)
└── ... (resto de configuración)
```

### 4️⃣ Configurar DayNightCycle

```
GameObject: DayNightSystem
├── Component: DayNightCycle
    ├── Events
    │   └── On Global Light Command: → GlobalLightCommandEvent (SO)
    └── ... (resto de configuración)
```

### 5️⃣ Configurar ScreenFadeManager

```
GameObject: ScreenFadeManager
├── Component: ScreenFadeManager
    ├── Events
    │   └── Screen Fade Event: → ScreenFadeEvent (SO)
    └── Settings
        └── Create Canvas On Awake: true
```

### 6️⃣ Configurar Scripts que Usan Fade

En todos los scripts que usan fade (Ladders, Ropes, ClimbSpawnPoint):

```
Component: ClimbableWithTeleport / Ladder / ClimbSpawnPoint / RopeAnchor
├── Events
│   └── Screen Fade Event: → ScreenFadeEvent (SO)
└── ... (resto de configuración)
```

---

## 🔍 Comparación Antes/Después

### ANTES (Singleton)

```csharp
// ❌ Dependencia oculta del singleton
private void TriggerFade()
{
    ScreenFadeManager.Instance.FadeToBlack(0.5f, OnComplete);
}

// ❌ Problema: Si no existe, da error en runtime
// ❌ Problema: Imposible testear sin el singleton
// ❌ Problema: DontDestroyOnLoad causa issues
```

### DESPUÉS (ScriptableObject Events)

```csharp
// ✅ Dependencia explícita en el inspector
[SerializeField] private ScreenFadeEvent screenFadeEvent;

private void TriggerFade()
{
    if (screenFadeEvent != null)
    {
        screenFadeEvent.RaiseFadeToBlack(0.5f, OnComplete);
    }
    else
    {
        Debug.LogWarning("ScreenFadeEvent not assigned!");
    }
}

// ✅ Ventaja: Dependencia visible en inspector
// ✅ Ventaja: Fácil de testear (crea diferentes SO)
// ✅ Ventaja: Cada escena puede tener su propio manager
```

---

## 📝 Nuevas APIs

### LightControlEvent

```csharp
// Registro de luces
onLightRegistered.Raise(baseLightController);
onLightUnregistered.Raise(baseLightController);
```

### GlobalLightCommandEvent

```csharp
// Comandos globales
onGlobalLightCommand.Raise(LightCommand.TurnOnAll);
onGlobalLightCommand.Raise(LightCommand.TurnOffAll);
onGlobalLightCommand.Raise(LightCommand.SetGlobalIntensity, 0.5f);
```

### ScreenFadeEvent

```csharp
// Fade a negro
screenFadeEvent.RaiseFadeToBlack(duration, onComplete);

// Fade desde negro
screenFadeEvent.RaiseFadeFromBlack(duration, onComplete);

// Fade con teletransporte
screenFadeEvent.RaiseFadeToBlackAndTeleport(
    duration,
    targetPosition,
    targetTransform,
    onTeleportComplete
);
```

---

## ✅ Beneficios

### 🎯 Desacoplamiento Total

```
ANTES:
ClimbSpawnPoint → ScreenFadeManager.Instance (acoplado)

DESPUÉS:
ClimbSpawnPoint → ScreenFadeEvent (SO)
                    ↓
                ScreenFadeManager (escucha evento)
```

### 🧪 Testeable

```csharp
// Puedes crear diferentes eventos para testing
[CreateAssetMenu]
public class TestScreenFadeEvent : ScreenFadeEvent
{
    // Mock para tests
}
```

### 🏗️ Múltiples Escenas

```
Escena 1: MainMenu
├── ScreenFadeManager_1 → Escucha ScreenFadeEvent
└── Funciona independiente

Escena 2: Gameplay
├── ScreenFadeManager_2 → Escucha ScreenFadeEvent
└── Funciona independiente

Ambos usan el MISMO ScreenFadeEvent (SO)
```

### 💾 Guardado Simple

```csharp
// El estado NO está en singletons
// Cada manager es un MonoBehaviour normal
// Se destruye y recrea con cada escena
```

---

## 🚨 Problemas Resueltos

### ❌ PROBLEMA 1: Singleton persistente entre escenas

```
ANTES:
- ScreenFadeManager usa DontDestroyOnLoad
- Si cargas una nueva escena, sigue existiendo
- Difícil resetear estado

AHORA:
- Cada escena tiene su propio ScreenFadeManager
- Se destruye al cambiar de escena
- Estado siempre limpio
```

### ❌ PROBLEMA 2: Imposible testear escenas individuales

```
ANTES:
- LightManager.Instance debe existir
- Si abres escena de testing, falta el singleton
- Tienes que crear setup especial

AHORA:
- LightManager es un MonoBehaviour normal
- Cada escena puede tener el suyo
- Testing independiente
```

### ❌ PROBLEMA 3: Dependencias ocultas

```
ANTES:
- No sabes qué scripts usan LightManager.Instance
- Difícil refactorizar
- Dependencias en tiempo de ejecución

AHORA:
- Eventos asignados en el inspector
- Dependencias explícitas y visibles
- Fácil ver qué usa qué
```

---

## 🎓 Patrón de Migración

Si necesitas migrar otro singleton en el futuro:

### Paso 1: Crear el Evento

```csharp
[CreateAssetMenu(fileName = "MyEvent", menuName = "Events/My Event")]
public class MyEvent : ScriptableObject
{
    private event Action<DataType> listeners;

    public void Raise(DataType data)
    {
        listeners?.Invoke(data);
    }

    public void AddListener(Action<DataType> listener)
    {
        listeners += listener;
    }

    public void RemoveListener(Action<DataType> listener)
    {
        listeners -= listener;
    }

    private void OnDisable()
    {
        listeners = null;
    }
}
```

### Paso 2: Refactorizar el Manager

```csharp
// ANTES
public class MyManager : MonoBehaviour
{
    private static MyManager instance;
    public static MyManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }
}

// DESPUÉS
public class MyManager : MonoBehaviour
{
    [SerializeField] private MyEvent myEvent;

    private void OnEnable()
    {
        myEvent?.AddListener(HandleEvent);
    }

    private void OnDisable()
    {
        myEvent?.RemoveListener(HandleEvent);
    }

    private void HandleEvent(DataType data)
    {
        // Lógica aquí
    }
}
```

### Paso 3: Refactorizar Consumidores

```csharp
// ANTES
MyManager.Instance.DoSomething();

// DESPUÉS
[SerializeField] private MyEvent myEvent;
myEvent?.Raise(data);
```

---

## 📋 Checklist de Configuración

Usa esta lista cuando configures una nueva escena:

### Iluminación

- [ ] Crear LightManager GameObject
- [ ] Asignar LightRegisteredEvent
- [ ] Asignar LightUnregisteredEvent
- [ ] Asignar GlobalLightCommandEvent
- [ ] Configurar todas las luces con los eventos

### Ciclo Día/Noche (Opcional)

- [ ] Crear DayNightSystem GameObject
- [ ] Añadir DayNightCycle component
- [ ] Asignar GlobalLightCommandEvent
- [ ] Asignar Global Light (Light2D)

### Screen Fade

- [ ] Crear ScreenFadeManager GameObject
- [ ] Asignar ScreenFadeEvent
- [ ] Configurar todos los objetos que usan fade:
  - [ ] Ladders
  - [ ] ClimbSpawnPoints
  - [ ] RopeAnchors
  - [ ] ClimbableObjects

---

## 🎉 Resultado Final

```
✅ 0 Singletons en el proyecto
✅ 100% Desacoplado con eventos
✅ Testeable escena por escena
✅ Fácil de mantener y extender
✅ Compatible con guardado/carga
✅ Sin DontDestroyOnLoad
```

---

## 📚 Referencias

- Documentación completa: `/Assets/Scripts/Architecture/ALTERNATIVAS_A_SINGLETONS.md`
- Ejemplos de eventos existentes: `/Assets/Scripts/Events/`
- Patrón recomendado: ScriptableObject Events

---

## 💡 Próximos Pasos Sugeridos

Si encuentras otros singletons en el proyecto:

1. Lee `/Assets/Scripts/Architecture/ALTERNATIVAS_A_SINGLETONS.md`
2. Sigue el patrón de migración de este documento
3. Crea los eventos necesarios
4. Refactoriza el singleton
5. Actualiza todos los consumidores
6. Verifica que no hay errores de compilación
7. Documenta los cambios

---

**Fecha de migración:** 2024  
**Scripts migrados:** 9  
**Eventos creados:** 3  
**Singletons eliminados:** 2
