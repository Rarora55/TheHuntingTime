# PlayerInteractionController - Explicación Detallada

**Proyecto:** TheHuntProject | **Unity:** 6000.3  
**Archivo:** `/Assets/Scripts/Interaction/PlayerInteractionController.cs`  
**Arquitectura:** Interfaces + Eventos (igual que HealthController)

---

## 📋 Índice

1. [Introducción](#1-introducción)
2. [Arquitectura General](#2-arquitectura-general)
3. [Campos Serializados](#3-campos-serializados)
4. [Campos Privados](#4-campos-privados)
5. [Properties](#5-properties)
6. [Eventos](#6-eventos)
7. [Lifecycle: OnEnable/OnDisable](#7-lifecycle-onenable-ondisable)
8. [Update Loop](#8-update-loop)
9. [DetectNearbyInteractables()](#9-detectnearbyinteractables)
10. [SetInteractable() y ClearInteractable()](#10-setinteractable-y-clearinteractable)
11. [TryInteract()](#11-tryinteract)
12. [Input Handling](#12-input-handling)
13. [Debug Gizmos](#13-debug-gizmos)
14. [Flujo Completo](#14-flujo-completo)
15. [Patrones de Diseño](#15-patrones-de-diseño)

---

## 1. Introducción

`PlayerInteractionController` es el componente que permite al jugador **detectar e interactuar** con objetos cercanos en el mundo.

### Responsabilidades

✅ **Detectar** objetos interactuables en un radio  
✅ **Seleccionar** el más cercano  
✅ **Escuchar** input del jugador (tecla E)  
✅ **Ejecutar** la interacción  
✅ **Notificar** via eventos a otros sistemas (UI, audio, etc)

### Comparación con HealthController

| Aspecto | HealthController | PlayerInteractionController |
|---------|------------------|----------------------------|
| **Qué hace** | Gestiona salud/daño/curación | Gestiona detección/interacción |
| **Interface** | `IHealth`, `IDamageable`, `IHealable` | `IInteractor` |
| **Eventos** | `OnHealthChanged`, `OnDamaged` | `OnInteractableDetected`, `OnInteracted` |
| **Patrón** | Event-driven ✅ | Event-driven ✅ |
| **Input** | No (recibe daño de otros) | Sí (New Input System) |
| **Detection** | No (pasivo) | Sí (activo en Update) |

---

## 2. Arquitectura General

```
┌─────────────────────────────────────────────────┐
│              PLAYER GAMEOBJECT                  │
│                                                 │
│  ┌──────────────────────────────────────────┐  │
│  │  PlayerInteractionController             │  │
│  │  (IInteractor)                           │  │
│  │                                          │  │
│  │  1. Detecta objetos (Update)            │  │
│  │  2. Encuentra el más cercano            │  │
│  │  3. Escucha input (tecla E)             │  │
│  │  4. Ejecuta interacción                 │  │
│  │  5. Dispara eventos                     │  │
│  └──────────────────────────────────────────┘  │
│                      ↓                          │
│              ┌───────────────┐                  │
│              │   Eventos     │                  │
│              ├───────────────┤                  │
│              │ UI → Prompt   │                  │
│              │ Audio → Sound │                  │
│              │ VFX → Effect  │                  │
│              └───────────────┘                  │
└─────────────────────────────────────────────────┘
                      ↓ Interact()
┌─────────────────────────────────────────────────┐
│         MUNDO - OBJETOS INTERACTUABLES          │
│                                                 │
│  ┌────────┐  ┌────────┐  ┌────────┐           │
│  │ Chest  │  │ Sword  │  │  NPC   │           │
│  │(IInter)│  │(IInter)│  │(IInter)│           │
│  └────────┘  └────────┘  └────────┘           │
└─────────────────────────────────────────────────┘
```

---

## 3. Campos Serializados

### detectionRadius

```csharp
[SerializeField] private float detectionRadius = 2f;
```

**Radio de detección** en unidades Unity (metros).

- Define qué tan cerca debe estar el player del objeto
- Se visualiza con Gizmos (círculo cyan)
- Valor recomendado: 1.5-3.0 para juegos 2D

**Ejemplo:**
```
detectionRadius = 2f

Player en (0, 0)
Objeto en (1.5, 0) → DENTRO del radio ✅
Objeto en (3, 0)   → FUERA del radio ❌
```

---

### interactionLayer

```csharp
[SerializeField] private LayerMask interactionLayer;
```

**Layer mask** para filtrar qué objetos detectar.

**¿Por qué?**
- **Performance:** No chequear TODO en la escena
- **Control:** Solo objetos en layer "Interactable"

**Setup:**
1. Crear layer "Interactable" en Unity
2. Asignar objetos interactuables a ese layer
3. Configurar field en Inspector

**Alternativa sin layer:**
```csharp
// ❌ Sin layer - detecta TODO
Physics2D.OverlapCircle(pos, radius);

// ✅ Con layer - solo "Interactable"
Physics2D.OverlapCircle(pos, radius, interactionLayer);
```

---

### interactAction

```csharp
[SerializeField] private InputActionReference interactAction;
```

**Referencia a Input Action** del New Input System.

**Conecta:**
- Asset: `InputSystem_Actions.inputactions`
- Action Map: `Player`
- Action: `Interact` (tecla E)

**En Inspector:**
```
Player Interaction Controller
├─ Detection Radius: 2
├─ Interaction Layer: Interactable
└─ Interact Action: Player/Interact  ← Arrastrar desde Input Actions
```

---

## 4. Campos Privados

### currentInteractable

```csharp
private IInteractable currentInteractable;
```

**Objeto interactuable actual** detectado y seleccionado.

**Estados:**
- `null` → No hay nada cerca
- `IInteractable` → Hay objeto interactuable cerca

**Usado por:**
- `CanInteract` property
- `TryInteract()` método
- Eventos

---

### detectionResults

```csharp
private Collider2D[] detectionResults = new Collider2D[10];
```

**Array reutilizable** para resultados de detección.

**¿Por qué?**

**Sin array (❌ Genera basura):**
```csharp
void Update()
{
    // Crea nuevo array cada frame → Garbage Collection
    Collider2D[] results = Physics2D.OverlapCircleAll(pos, radius);
}
```

**Con array (✅ Sin basura):**
```csharp
private Collider2D[] detectionResults = new Collider2D[10];  // Una vez

void Update()
{
    // Reutiliza array → 0 allocations ✅
    int count = Physics2D.OverlapCircleNonAlloc(pos, radius, detectionResults);
}
```

**Tamaño 10:**
- Máximo 10 objetos detectados simultáneamente
- Suficiente para la mayoría de casos
- Ajustar si necesitas más

---

## 5. Properties

### CurrentInteractable

```csharp
public IInteractable CurrentInteractable => currentInteractable;
```

**Solo lectura** - Expone objeto actual sin permitir modificación.

**Uso:**
```csharp
// UI puede leer
IInteractor player = GetComponent<IInteractor>();
if (player.CurrentInteractable != null)
{
    promptText.text = player.CurrentInteractable.InteractionPrompt;
}
```

---

### CanInteract

```csharp
public bool CanInteract => currentInteractable != null && 
                           currentInteractable.CanInteract(gameObject);
```

**Validación completa** antes de interactuar.

**Condiciones:**
1. `currentInteractable != null` → Hay algo detectado
2. `currentInteractable.CanInteract(gameObject)` → El objeto acepta interacción

**Ejemplo:**
```csharp
// Chest cerrado
CanInteract = true ✅

// Chest ya abierto
CanInteract = false ❌ (ChestInteractable.CanInteract retorna false)
```

---

## 6. Eventos

### OnInteractableDetected

```csharp
public event Action<IInteractable> OnInteractableDetected;
```

**Disparado cuando se detecta nuevo objeto.**

**Parámetro:** El `IInteractable` detectado

**Uso:**
```csharp
playerInteractor.OnInteractableDetected += ShowPrompt;

void ShowPrompt(IInteractable interactable)
{
    promptPanel.SetActive(true);
    promptText.text = interactable.InteractionPrompt;
}
```

---

### OnInteractableCleared

```csharp
public event Action OnInteractableCleared;
```

**Disparado cuando player se aleja** del objeto.

**Sin parámetros**

**Uso:**
```csharp
playerInteractor.OnInteractableCleared += HidePrompt;

void HidePrompt()
{
    promptPanel.SetActive(false);
}
```

---

### OnInteracted

```csharp
public event Action<IInteractable> OnInteracted;
```

**Disparado cuando se ejecuta interacción** (presiona E).

**Parámetro:** El `IInteractable` con el que se interactuó

**Uso:**
```csharp
playerInteractor.OnInteracted += PlaySound;

void PlaySound(IInteractable interactable)
{
    audioSource.PlayOneShot(interactSound);
}
```

---

## 7. Lifecycle: OnEnable/OnDisable

### OnEnable()

```csharp
void OnEnable()
{
    if (interactAction != null)
    {
        interactAction.action.performed += OnInteractPerformed;
    }
}
```

**Subscribe al evento de input.**

**Cuándo se llama:**
- GameObject/Component se activa
- Escena se carga con objeto activo

**¿Por qué OnEnable y no Start?**
```
Start()   → Solo 1 vez en lifetime
OnEnable() → Cada vez que se activa

Útil para:
- Pooling de objetos
- Habilitar/deshabilitar componente
```

---

### OnDisable()

```csharp
void OnDisable()
{
    if (interactAction != null)
    {
        interactAction.action.performed -= OnInteractPerformed;
    }
}
```

**Unsubscribe del evento de input.**

**MUY IMPORTANTE:** Prevenir memory leaks

**Sin OnDisable (❌):**
```csharp
// OnEnable subscribe pero nunca unsubscribe
// → PlayerInteractionController queda en memoria aunque se destruya
```

**Con OnDisable (✅):**
```csharp
// Subscribe/Unsubscribe balanceados
// → Garbage collector puede limpiar
```

---

## 8. Update Loop

```csharp
void Update()
{
    DetectNearbyInteractables();
}
```

**Cada frame** detecta objetos cercanos.

**¿Por qué Update y no FixedUpdate?**

| Update() | FixedUpdate() |
|----------|---------------|
| Variable (~60 FPS) | Fijo (50 FPS default) |
| ✅ Input/UI responsivo | ❌ Delay perceptible |
| ✅ Detección smooth | Para física |

**Performance:**
- `OverlapCircleNonAlloc` es muy eficiente
- 0 allocations
- Costo: ~0.01-0.05ms

---

## 9. DetectNearbyInteractables()

Este es el **método más importante**. Vamos línea por línea:

### Paso 0: Configuración del ContactFilter (Awake)

```csharp
void Awake()
{
    contactFilter = new ContactFilter2D
    {
        layerMask = interactionLayer,
        useLayerMask = true,
        useTriggers = true
    };
}
```

**Qué hace:**
- Configura el filtro de contacto una sola vez
- Especifica el layer mask para objetos interactuables
- Habilita detección de triggers

### Paso 1: OverlapCircle (Líneas 60-65)

```csharp
int numFound = Physics2D.OverlapCircle(
    transform.position,      // Centro = posición del player
    detectionRadius,         // Radio = 2m
    contactFilter,           // Filtro con layer configurado
    detectionResults         // Array para llenar
);
```

**Qué hace:**
- Busca colliders en círculo alrededor del player
- Usa `contactFilter` para filtrar por layer "Interactable"
- Llena `detectionResults` array
- Retorna cantidad encontrada

**Nota Unity 6:**
- ✅ API actualizada (antes era `OverlapCircleNonAlloc`)
- ✅ Usa `ContactFilter2D` para mayor flexibilidad
- ✅ Sin allocations (mismo rendimiento)

**Ejemplo:**
```
Player en (0, 0), radius = 2

Chest en (1, 0), layer Interactable   → ✅ numFound = 1
Sword en (1.5, 1), layer Interactable → ✅ numFound = 2
Rock en (1, 1), layer Default         → ❌ ignorado
Enemy en (3, 0), layer Enemy          → ❌ fuera de radio
```

---

### Paso 2: Inicializar Variables (Líneas 56-57)

```csharp
IInteractable closestInteractable = null;
float closestDistance = float.MaxValue;  // Infinito
```

**Algoritmo "find minimum":**
- Empezar con distancia infinita
- Comparar cada objeto
- Guardar el más cercano

---

### Paso 3: Loop Through Results (Líneas 59-73)

```csharp
for (int i = 0; i < numFound; i++)
{
    IInteractable interactable = detectionResults[i].GetComponent<IInteractable>();
    
    if (interactable != null && interactable.IsInteractable)
    {
        float distance = Vector2.Distance(transform.position, 
                                         detectionResults[i].transform.position);
        
        if (distance < closestDistance)
        {
            closestDistance = distance;
            closestInteractable = interactable;
        }
    }
}
```

#### Línea 61: GetComponent

```csharp
IInteractable interactable = detectionResults[i].GetComponent<IInteractable>();
```

**Busca interface** en el GameObject.

**¿Por qué puede ser null?**
- Objeto tiene collider pero NO tiene componente IInteractable
- Ej: Decoración con layer incorrecto

---

#### Línea 63: Validaciones

```csharp
if (interactable != null && interactable.IsInteractable)
```

**Doble check:**
1. `!= null` → Tiene componente
2. `IsInteractable` → Estado activo (ej: chest no ya abierto)

---

#### Línea 65: Calcular Distancia

```csharp
float distance = Vector2.Distance(transform.position, 
                                 detectionResults[i].transform.position);
```

**Distancia euclidiana:**
```csharp
// Internamente:
Vector2.Distance(a, b) = Mathf.Sqrt((b.x - a.x)² + (b.y - a.y)²)
```

---

#### Líneas 67-71: Actualizar Más Cercano

```csharp
if (distance < closestDistance)
{
    closestDistance = distance;
    closestInteractable = interactable;
}
```

**Algoritmo:**
```
Inicial: closestDistance = ∞

Objeto 1: distance = 2.5
  2.5 < ∞ → closestDistance = 2.5 ✅

Objeto 2: distance = 1.3
  1.3 < 2.5 → closestDistance = 1.3 ✅

Objeto 3: distance = 3.0
  3.0 < 1.3 → NO actualiza

Resultado: Objeto 2 (distance = 1.3)
```

---

### Paso 4: Comparar con Actual (Líneas 75-86)

```csharp
if (closestInteractable != currentInteractable)
{
    if (currentInteractable != null)
    {
        ClearInteractable();
    }
    
    if (closestInteractable != null)
    {
        SetInteractable(closestInteractable);
    }
}
```

**¿Por qué comparar?**

**Sin comparación (❌):**
```csharp
// Cada frame dispara eventos aunque nada cambió
SetInteractable(closestInteractable);  // 60 veces/seg ❌
```

**Con comparación (✅):**
```csharp
// Solo dispara eventos cuando cambia
if (closestInteractable != currentInteractable)  // Una vez ✅
```

**Escenarios:**

**Caso 1: Player se acerca a objeto**
```
Frame N:   closestInteractable = null, currentInteractable = null
           → No cambia, no hace nada

Frame N+1: closestInteractable = Chest, currentInteractable = null
           → Cambia! → SetInteractable(Chest) ✅
```

**Caso 2: Player se aleja**
```
Frame N:   closestInteractable = Chest, currentInteractable = Chest
           → No cambia, no hace nada

Frame N+1: closestInteractable = null, currentInteractable = Chest
           → Cambia! → ClearInteractable() ✅
```

**Caso 3: Cambiar entre objetos**
```
Frame N:   closestInteractable = Chest, currentInteractable = Chest
           → No cambia

Frame N+1: closestInteractable = Sword, currentInteractable = Chest
           → Cambia! 
           → ClearInteractable() (Chest)
           → SetInteractable(Sword) ✅
```

---

## 10. SetInteractable() y ClearInteractable()

### SetInteractable()

```csharp
public void SetInteractable(IInteractable interactable)
{
    currentInteractable = interactable;
    OnInteractableDetected?.Invoke(interactable);
    
    Debug.Log($"<color=cyan>[INTERACTION] Detected: {interactable.InteractionPrompt}</color>");
}
```

**Acciones:**
1. Guardar referencia
2. Disparar evento
3. Log para debug

**Resultado:**
- UI muestra "Press E to pick up Sword"
- Audio puede hacer sonido sutil
- VFX puede hacer glow

---

### ClearInteractable()

```csharp
public void ClearInteractable()
{
    currentInteractable = null;
    OnInteractableCleared?.Invoke();
    
    Debug.Log($"<color=cyan>[INTERACTION] Cleared</color>");
}
```

**Acciones:**
1. Limpiar referencia
2. Disparar evento
3. Log para debug

**Resultado:**
- UI oculta prompt
- VFX quita glow

---

## 11. TryInteract()

```csharp
public void TryInteract()
{
    if (!CanInteract)
    {
        Debug.Log($"<color=yellow>[INTERACTION] Cannot interact</color>");
        return;
    }
    
    Debug.Log($"<color=green>[INTERACTION] Interacting with: {currentInteractable.InteractionPrompt}</color>");
    
    currentInteractable.Interact(gameObject);
    OnInteracted?.Invoke(currentInteractable);
}
```

### Guard Clause (Líneas 107-111)

```csharp
if (!CanInteract)
{
    Debug.Log($"<color=yellow>[INTERACTION] Cannot interact</color>");
    return;
}
```

**Previene:**
- Interactuar sin objeto cerca
- Interactuar con objeto que rechaza (ej: chest abierto)

**Casos:**
```csharp
// Nada cerca
currentInteractable = null
→ CanInteract = false → return ❌

// Chest ya abierto
currentInteractable = chest
chest.CanInteract(player) = false
→ CanInteract = false → return ❌

// Sword pickup disponible
currentInteractable = sword
sword.CanInteract(player) = true
→ CanInteract = true → continúa ✅
```

---

### Ejecutar Interacción (Líneas 113-116)

```csharp
Debug.Log($"<color=green>[INTERACTION] Interacting with: {currentInteractable.InteractionPrompt}</color>");

currentInteractable.Interact(gameObject);
OnInteracted?.Invoke(currentInteractable);
```

**Orden importante:**
1. **Log** - Para debug
2. **Interact()** - Ejecuta lógica del objeto (pickup, abrir, etc)
3. **OnInteracted** - Notifica a otros sistemas

**Flujo:**
```
TryInteract()
    ↓
currentInteractable.Interact(player)
    ↓
PickupInteractable.OnInteract()
    ├─ AddToInventory()
    ├─ PlayFeedback()
    └─ Destroy(gameObject)
    ↓
OnInteracted?.Invoke(interactable)
    ↓
Subscribers:
    ├─ Audio → PlaySound
    ├─ Stats → Increment counter
    └─ Achievements → Check unlock
```

---

## 12. Input Handling

```csharp
void OnInteractPerformed(InputAction.CallbackContext context)
{
    TryInteract();
}
```

**Callback del New Input System.**

**Flujo:**
```
1. Player presiona tecla E
    ↓
2. Input System detecta
    ↓
3. Dispara evento "Interact.performed"
    ↓
4. OnInteractPerformed(context) se ejecuta
    ↓
5. Llama TryInteract()
```

**InputAction.CallbackContext:**
- Contiene info del input (duración, valor, etc)
- No lo usamos aquí (solo queremos saber que se presionó)

---

## 13. Debug Gizmos

```csharp
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);
}
```

**Visualización en Scene view.**

**Cuándo se dibuja:**
- Solo cuando GameObject está seleccionado
- Solo en Editor (no en build)

**Útil para:**
- Ver radio de detección
- Ajustar `detectionRadius` visualmente

```
Player seleccionado en Hierarchy
→ Scene view muestra círculo cyan
→ Ajustar radius en Inspector y ver en tiempo real
```

---

## 14. Flujo Completo

### Escenario: Player Recoge Espada

```
═══════════════════════════════════════════════════════
FRAME 1: Player lejos de espada
═══════════════════════════════════════════════════════

Update()
    ↓
DetectNearbyInteractables()
    ↓
OverlapCircle(pos, 2f)
    → numFound = 0 (nada cerca)
    ↓
closestInteractable = null
currentInteractable = null
    → No cambia, no hace nada


═══════════════════════════════════════════════════════
FRAME 30: Player se acerca (distance = 1.5m)
═══════════════════════════════════════════════════════

Update()
    ↓
DetectNearbyInteractables()
    ↓
OverlapCircle(pos, 2f)
    → numFound = 1 (Sword)
    ↓
Loop:
    sword.GetComponent<IInteractable>() ✅
    sword.IsInteractable = true ✅
    distance = 1.5m
    closestInteractable = sword
    ↓
if (sword != null)  // Cambió!
    ↓
SetInteractable(sword)
    ├─ currentInteractable = sword
    ├─ OnInteractableDetected?.Invoke(sword)
    │   └─ UI: Muestra "Press E to pick up Sword"
    └─ Log: [INTERACTION] Detected: Press E to pick up Sword


═══════════════════════════════════════════════════════
FRAMES 31-60: Player cerca, esperando
═══════════════════════════════════════════════════════

Update() x30
    ↓
DetectNearbyInteractables() x30
    ↓
closestInteractable = sword
currentInteractable = sword
    → No cambia, no hace nada (eficiente!)


═══════════════════════════════════════════════════════
FRAME 61: Player presiona E
═══════════════════════════════════════════════════════

Player presiona E
    ↓
Input System
    ↓
OnInteractPerformed(context)
    ↓
TryInteract()
    ├─ CanInteract?
    │   └─ currentInteractable != null ✅
    │   └─ sword.CanInteract(player) ✅
    │   → true ✅
    │
    ├─ Log: [INTERACTION] Interacting with: Press E to pick up Sword
    │
    ├─ sword.Interact(player)
    │   └─ PickupInteractable.OnInteract()
    │       ├─ AddToInventory(player) ✅
    │       ├─ PlayFeedback() 🎵
    │       └─ Destroy(sword) 💥
    │
    └─ OnInteracted?.Invoke(sword)
        ├─ Audio: PlayPickupSound()
        └─ Stats: itemsCollected++


═══════════════════════════════════════════════════════
FRAME 62: Después de pickup
═══════════════════════════════════════════════════════

Update()
    ↓
DetectNearbyInteractables()
    ↓
OverlapCircle(pos, 2f)
    → numFound = 0 (sword destruida)
    ↓
closestInteractable = null
currentInteractable = sword  // Todavía guardada
    → Cambió!
    ↓
ClearInteractable()
    ├─ currentInteractable = null
    ├─ OnInteractableCleared?.Invoke()
    │   └─ UI: Oculta prompt
    └─ Log: [INTERACTION] Cleared
```

---

## 15. Patrones de Diseño

### 1. Interface-Based Design

```csharp
private IInteractable currentInteractable;
```

**Polimorfismo:**
- Funciona con cualquier `IInteractable`
- Chest, Sword, NPC, Door, etc
- Sin conocer implementación

---

### 2. Event-Driven Architecture

```csharp
OnInteractableDetected?.Invoke(interactable);
```

**Desacoplamiento:**
- `PlayerInteractionController` NO conoce UI
- UI subscribe a eventos
- Fácil agregar sistemas (audio, VFX, stats)

---

### 3. Object Pooling Pattern

```csharp
private Collider2D[] detectionResults = new Collider2D[10];
```

**0 Allocations:**
- Array creado una vez
- Reutilizado cada frame
- Sin garbage collection

---

### 4. Guard Clauses

```csharp
if (!CanInteract)
    return;
```

**Early return:**
- Validaciones primero
- Evita anidamiento
- Código más legible

---

### 5. Null-Conditional Operator

```csharp
OnInteracted?.Invoke(interactable);
```

**Seguridad:**
- Sin subscribers → no hace nada
- Con subscribers → invoca
- Sin `if (OnInteracted != null)`

---

### 6. Find Minimum Algorithm

```csharp
float closestDistance = float.MaxValue;
for (...)
{
    if (distance < closestDistance)
        closestInteractable = object;
}
```

**Clásico algoritmo:**
- O(n) complejidad
- Eficiente
- Simple

---

## 16. Performance

### Optimizaciones Implementadas

✅ **Physics2D.OverlapCircle** - 0 allocations (Unity 6)  
✅ **ContactFilter2D** - Filtra objetos irrelevantes  
✅ **Early comparison** - Solo eventos si cambia  
✅ **Array pooling** - Reutiliza memoria

### Costo por Frame

```
DetectNearbyInteractables():
├─ OverlapCircle: ~0.01-0.03ms
├─ Loop (10 objetos max): ~0.005ms
├─ Distance checks: ~0.002ms
└─ Total: ~0.02-0.05ms/frame

A 60 FPS: 0.05ms es 0.3% del frame budget (16.6ms)
```

**Muy eficiente!** ✅

---

## 17. Preguntas Frecuentes

### ¿Por qué Update y no trigger events?

**Update (✅ Actual):**
- Encuentra objeto más cercano
- Funciona aunque objetos no se muevan
- Control total

**OnTriggerStay2D (❌ Alternativa):**
- Múltiples objetos → ¿cuál elegir?
- Requiere Rigidbody
- Menos control

---

### ¿Puedo detectar sin layer?

```csharp
// Sin layer - detecta TODO
ContactFilter2D filterAll = new ContactFilter2D();
filterAll.useTriggers = true;

int numFound = Physics2D.OverlapCircle(
    transform.position,
    detectionRadius,
    filterAll,
    detectionResults
);
```

**Funcionará pero:**
- Menos eficiente
- Detecta walls, enemies, decorations
- Más loops innecesarios

**Recomendación:** Siempre usa layer mask para mejor rendimiento

---

### ¿Cómo aumentar máximo de objetos?

```csharp
// De 10 a 20
private Collider2D[] detectionResults = new Collider2D[20];
```

**Cuándo hacerlo:**
- Muchos objetos densos
- Warning en Console: "Array too small"

---

### ¿Puedo usar 3D en vez de 2D?

```csharp
// Cambiar Physics2D por Physics
int numFound = Physics.OverlapSphereNonAlloc(
    transform.position,
    detectionRadius,
    detectionResults3D,  // Collider[] (no Collider2D[])
    interactionLayer
);
```

---

## 18. Resumen

`PlayerInteractionController` es:

✅ **Eficiente** - 0 allocations, optimizado  
✅ **Extensible** - Funciona con cualquier IInteractable  
✅ **Event-driven** - Desacoplado de UI/Audio/VFX  
✅ **Simple** - Lógica clara y directa  
✅ **Debuggable** - Logs y Gizmos  
✅ **Coherente** - Misma arquitectura que HealthController

---

**Siguiente:** Integrar con sistema de inventario y crear objetos interactuables personalizados.
