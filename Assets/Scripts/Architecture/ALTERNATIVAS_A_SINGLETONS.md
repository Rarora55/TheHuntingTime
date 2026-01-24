# 🏗️ Alternativas a Singletons - Guía Completa

## ❌ Por Qué Evitar Singletons

### Problemas con Singletons en Juegos Multi-Escena con Guardado:

```
SINGLETON CLÁSICO:
public static RespawnManager Instance { get; }

PROBLEMAS:
├─ ❌ Estado global que persiste entre escenas
├─ ❌ Difícil de resetear al cargar partida
├─ ❌ No puedes testear escenas individuales
├─ ❌ Dependencias ocultas
└─ ❌ Orden de inicialización impredecible
```

---

## ✅ ALTERNATIVA 1: ScriptableObject Events (RECOMENDADO)

### Concepto

En vez de que todos accedan a un Singleton global, usas **eventos basados en ScriptableObjects** para comunicación desacoplada.

### Implementación

```csharp
// 1. EVENTO GENÉRICO (crear una vez, reutilizar)
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private event Action listeners;

    public void Raise()
    {
        listeners?.Invoke();
    }

    public void AddListener(Action listener)
    {
        listeners += listener;
    }

    public void RemoveListener(Action listener)
    {
        listeners -= listener;
    }
}
```

```csharp
// 2. EVENTO CON PARÁMETROS
[CreateAssetMenu(fileName = "RespawnEvent", menuName = "Events/Respawn Event")]
public class RespawnEvent : ScriptableObject
{
    private event Action<Vector3, string> listeners;

    public void Raise(Vector3 position, string respawnID)
    {
        listeners?.Invoke(position, respawnID);
    }

    public void AddListener(Action<Vector3, string> listener)
    {
        listeners += listener;
    }

    public void RemoveListener(Action<Vector3, string> listener)
    {
        listeners -= listener;
    }
}
```

```csharp
// 3. USO EN RespawnPoint (EMISOR)
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private RespawnEvent onRespawnActivated; // ← Asset
    [SerializeField] private string respawnID;

    private void ActivateRespawn()
    {
        // Emite evento (no necesita conocer RespawnManager)
        onRespawnActivated.Raise(transform.position, respawnID);
    }
}
```

```csharp
// 4. USO EN RespawnManager (RECEPTOR)
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private RespawnEvent onRespawnActivated; // ← Mismo asset

    private Vector3 currentRespawnPosition;
    private string currentRespawnID;

    private void OnEnable()
    {
        // Suscribirse al evento
        onRespawnActivated.AddListener(OnRespawnActivated);
    }

    private void OnDisable()
    {
        // Desuscribirse (importante!)
        onRespawnActivated.RemoveListener(OnRespawnActivated);
    }

    private void OnRespawnActivated(Vector3 position, string id)
    {
        currentRespawnPosition = position;
        currentRespawnID = id;
        Debug.Log($"Checkpoint saved: {id}");
    }
}
```

### Ventajas

```
✅ DESACOPLAMIENTO TOTAL
   - RespawnPoint no conoce RespawnManager
   - RespawnManager no conoce RespawnPoint
   - Fácil añadir nuevos listeners (ej: UI, audio, VFX)

✅ TESTEABLE
   - Puedes testear cada escena individualmente
   - Los eventos se resetean automáticamente entre escenas

✅ GUARDADO/CARGA
   - RespawnManager es un MonoBehaviour normal en escena
   - Fácil de guardar/cargar su estado
   - Se destruye y recrea con cada escena

✅ INSPECTOR-FRIENDLY
   - Eventos visibles y asignables en Inspector
   - Documentación visual de dependencias
```

---

## ✅ ALTERNATIVA 2: Service Locator

### Concepto

Un único punto centralizado para **registrar y buscar servicios**, pero sin singletons rígidos.

### Implementación

```csharp
// 1. SERVICE LOCATOR
using UnityEngine;
using System;
using System.Collections.Generic;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator instance;
    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Registrar servicio
    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (instance.services.ContainsKey(type))
        {
            Debug.LogWarning($"Service {type} already registered. Overwriting.");
        }
        instance.services[type] = service;
    }

    // Obtener servicio
    public static T Get<T>()
    {
        var type = typeof(T);
        if (instance.services.TryGetValue(type, out var service))
        {
            return (T)service;
        }
        Debug.LogError($"Service {type} not found!");
        return default;
    }

    // Des-registrar (importante para cambio de escenas)
    public static void Unregister<T>()
    {
        var type = typeof(T);
        instance.services.Remove(type);
    }

    // Limpiar todos los servicios (al cambiar escena)
    public static void Clear()
    {
        instance.services.Clear();
    }
}
```

```csharp
// 2. INTERFAZ PARA SERVICIO
public interface IRespawnService
{
    void SetRespawnPoint(Vector3 position, string id);
    Vector3 GetRespawnPosition();
    void RespawnPlayer(Player player);
}
```

```csharp
// 3. IMPLEMENTACIÓN
using UnityEngine;

public class RespawnService : MonoBehaviour, IRespawnService
{
    private Vector3 currentRespawnPosition;
    private string currentRespawnID;

    private void Awake()
    {
        // Auto-registrarse
        ServiceLocator.Register<IRespawnService>(this);
    }

    private void OnDestroy()
    {
        // Auto-desregistrarse
        ServiceLocator.Unregister<IRespawnService>();
    }

    public void SetRespawnPoint(Vector3 position, string id)
    {
        currentRespawnPosition = position;
        currentRespawnID = id;
    }

    public Vector3 GetRespawnPosition() => currentRespawnPosition;

    public void RespawnPlayer(Player player)
    {
        player.transform.position = currentRespawnPosition;
    }
}
```

```csharp
// 4. USO
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void ActivateRespawn()
    {
        var respawnService = ServiceLocator.Get<IRespawnService>();
        respawnService?.SetRespawnPoint(transform.position, respawnID);
    }
}
```

### Ventajas

```
✅ FLEXIBILIDAD
   - Puedes cambiar implementaciones fácilmente
   - Mock services para testing

✅ MENOS ACOPLAMIENTO
   - Código depende de interfaces, no implementaciones concretas

✅ CONTROL CENTRALIZADO
   - Ves todos los servicios registrados en un solo lugar
   - Fácil de limpiar entre cambios de escena

⚠️ DESVENTAJA
   - Aún tienes un singleton (ServiceLocator)
   - Dependencia oculta (ServiceLocator.Get<>())
```

---

## ✅ ALTERNATIVA 3: Dependency Injection + ScriptableObject

### Concepto

Combinas **ScriptableObjects para datos** + **inyección manual de dependencias**.

### Implementación

```csharp
// 1. SCRIPTABLEOBJECT PARA DATOS
using UnityEngine;

[CreateAssetMenu(fileName = "RespawnData", menuName = "Game/Respawn Data")]
public class RespawnData : ScriptableObject
{
    public Vector3 currentRespawnPosition;
    public string currentRespawnID;

    public void SetRespawn(Vector3 position, string id)
    {
        currentRespawnPosition = position;
        currentRespawnID = id;
    }

    public void Reset()
    {
        currentRespawnPosition = Vector3.zero;
        currentRespawnID = "";
    }
}
```

```csharp
// 2. MANAGER QUE USA EL SO
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private RespawnData respawnData; // ← Asset compartido

    public void RespawnPlayer(Player player)
    {
        player.transform.position = respawnData.currentRespawnPosition;
    }

    // Guardado
    public void SaveToFile(SaveData saveData)
    {
        saveData.respawnPosition = respawnData.currentRespawnPosition;
        saveData.respawnID = respawnData.currentRespawnID;
    }

    // Cargado
    public void LoadFromFile(SaveData saveData)
    {
        respawnData.SetRespawn(saveData.respawnPosition, saveData.respawnID);
    }
}
```

```csharp
// 3. RESPAWN POINT USA EL MISMO SO
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private RespawnData respawnData; // ← Mismo asset

    private void ActivateRespawn()
    {
        respawnData.SetRespawn(transform.position, respawnID);
    }
}
```

### Ventajas

```
✅ DATOS COMPARTIDOS
   - ScriptableObject actúa como "memoria compartida"
   - No hay singletons

✅ INSPECTOR-FRIENDLY
   - Asignas el asset en cada componente que lo necesita
   - Documentación visual clara

✅ GUARDADO/CARGA SIMPLE
   - El SO contiene el estado
   - Fácil de serializar/deserializar

✅ TESTEABLE
   - Creas diferentes assets para testing
   - Reseteas el SO antes de cada test

⚠️ CUIDADO
   - Los SO mantienen estado en Editor (usar Reset())
   - Pueden causar confusión si no se limpian
```

---

## 📊 Comparación

| Aspecto | Singletons | SO Events | Service Locator | SO + DI |
|---------|-----------|-----------|----------------|---------|
| **Desacoplamiento** | ❌ Alto acoplamiento | ✅ Total | 🟡 Medio | ✅ Alto |
| **Testeable** | ❌ Difícil | ✅ Fácil | 🟡 Medio | ✅ Fácil |
| **Guardado/Carga** | ❌ Complejo | ✅ Simple | 🟡 Medio | ✅ Simple |
| **Debugging** | ❌ Difícil | ✅ Fácil | 🟡 Medio | ✅ Fácil |
| **Múltiples Escenas** | ❌ Problemático | ✅ Funciona bien | 🟡 OK | ✅ Funciona bien |
| **Curva de Aprendizaje** | ✅ Simple | 🟡 Media | 🟡 Media | 🟡 Media |

---

## 🎯 Recomendación para TU Proyecto

### Para RespawnManager:

**MEJOR OPCIÓN:** ScriptableObject Events + ScriptableObject Data

```
Por qué:
├─ ✅ Desacoplado: RespawnPoints no conocen RespawnManager
├─ ✅ Guardado simple: Guardas el ScriptableObject con el SaveSystem
├─ ✅ Testeable: Cada escena puede testearse individualmente
└─ ✅ Multiple scenes: Funciona perfecto con carga de escenas aditiva
```

### Para ScreenFadeManager:

**MEJOR OPCIÓN:** ScriptableObject Events

```
Por qué:
├─ ✅ Cualquier sistema puede pedir un fade (UI, cinematicas, etc)
├─ ✅ No necesita persistir estado entre escenas
└─ ✅ Fácil de testear
```

### Para LightManager:

**MEJOR OPCIÓN:** MonoBehaviour en cada escena + ScriptableObject Data

```
Por qué:
├─ ✅ Cada escena tiene su configuración de luces
├─ ✅ No necesita persistir entre escenas
└─ ✅ Guardas solo el estado global (día/noche) en SO
```

---

## 🚀 Implementación Paso a Paso

### EJEMPLO: Migrar RespawnManager de Singleton a SO Events

```csharp
// PASO 1: Crear RespawnEvent.cs
[CreateAssetMenu(fileName = "RespawnActivatedEvent", menuName = "Events/Respawn Activated")]
public class RespawnActivatedEvent : ScriptableObject
{
    private event Action<Vector3, string> listeners;

    public void Raise(Vector3 position, string respawnID)
    {
        listeners?.Invoke(position, respawnID);
    }

    public void AddListener(Action<Vector3, string> listener)
    {
        listeners += listener;
    }

    public void RemoveListener(Action<Vector3, string> listener)
    {
        listeners -= listener;
    }
}
```

```csharp
// PASO 2: Crear RespawnRequestEvent.cs
[CreateAssetMenu(fileName = "RespawnRequestEvent", menuName = "Events/Respawn Request")]
public class RespawnRequestEvent : ScriptableObject
{
    private event Action<Player> listeners;

    public void Raise(Player player)
    {
        listeners?.Invoke(player);
    }

    public void AddListener(Action<Player> listener)
    {
        listeners += listener;
    }

    public void RemoveListener(Action<Player> listener)
    {
        listeners -= listener;
    }
}
```

```csharp
// PASO 3: Crear RespawnData.cs (ScriptableObject)
[CreateAssetMenu(fileName = "RespawnData", menuName = "Game/Respawn Data")]
public class RespawnData : ScriptableObject
{
    public Vector3 currentRespawnPosition;
    public string currentRespawnID;

    public void SetRespawn(Vector3 position, string id)
    {
        currentRespawnPosition = position;
        currentRespawnID = id;
    }

    public void Reset()
    {
        currentRespawnPosition = Vector3.zero;
        currentRespawnID = "";
    }
}
```

```csharp
// PASO 4: RespawnManager (MonoBehaviour normal, NO singleton)
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private RespawnActivatedEvent onRespawnActivated;
    [SerializeField] private RespawnRequestEvent onRespawnRequest;

    [Header("Data")]
    [SerializeField] private RespawnData respawnData;

    private void OnEnable()
    {
        onRespawnActivated.AddListener(OnRespawnActivated);
        onRespawnRequest.AddListener(OnRespawnRequest);
    }

    private void OnDisable()
    {
        onRespawnActivated.RemoveListener(OnRespawnActivated);
        onRespawnRequest.RemoveListener(OnRespawnRequest);
    }

    private void OnRespawnActivated(Vector3 position, string id)
    {
        respawnData.SetRespawn(position, id);
        Debug.Log($"✓ Checkpoint saved: {id} at {position}");
    }

    private void OnRespawnRequest(Player player)
    {
        player.transform.position = respawnData.currentRespawnPosition;
        Debug.Log($"✓ Player respawned at {respawnData.currentRespawnID}");
    }
}
```

```csharp
// PASO 5: RespawnPoint usa eventos
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private RespawnActivatedEvent onRespawnActivated;
    [SerializeField] private string respawnID;

    private void ActivateRespawn()
    {
        onRespawnActivated.Raise(transform.position, respawnID);
    }
}
```

```csharp
// PASO 6: PlayerRespawnController usa eventos
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [SerializeField] private RespawnRequestEvent onRespawnRequest;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void RespawnPlayer()
    {
        onRespawnRequest.Raise(player);
    }
}
```

---

## ✅ Beneficios en TU Caso

### Para Sistema de Guardado:

```csharp
public class SaveSystem : MonoBehaviour
{
    [SerializeField] private RespawnData respawnData;

    public void SaveGame()
    {
        SaveData data = new SaveData();
        
        // Guardar respawn (simple!)
        data.respawnPosition = respawnData.currentRespawnPosition;
        data.respawnID = respawnData.currentRespawnID;
        
        // Serializar...
    }

    public void LoadGame(SaveData data)
    {
        // Cargar respawn (simple!)
        respawnData.SetRespawn(data.respawnPosition, data.respawnID);
        
        // Los managers se suscriben automáticamente
    }
}
```

### Para Debugging:

```csharp
// Puedes entrar a cualquier escena en Play Mode
// El RespawnManager en esa escena funciona independientemente
// No hay estado corrupto de singletons
```

### Para Múltiples Escenas:

```csharp
// Escena 1: City
RespawnManager (suscrito a eventos)
RespawnPoint_CityEntrance
RespawnPoint_CityPlaza

// Escena 2: Dungeon (carga aditiva)
RespawnManager (otro, independiente)
RespawnPoint_DungeonStart
RespawnPoint_BossRoom

// Ambos comparten el mismo RespawnData SO
// El estado persiste automáticamente
```

---

## 🎯 Conclusión

**NO uses Singletons para tu juego.**

**USA en su lugar:**
- **ScriptableObject Events** para comunicación
- **ScriptableObject Data** para estado compartido
- **MonoBehaviours normales** para managers

**Resultado:**
✅ Fácil de testear  
✅ Fácil de guardar/cargar  
✅ Fácil de debugear  
✅ Funciona con múltiples escenas  
✅ Código más limpio y desacoplado  
