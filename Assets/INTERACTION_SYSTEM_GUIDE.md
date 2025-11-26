# Sistema de Interacción - Guía de Implementación

**Proyecto:** TheHuntProject | **Unity:** 6000.3  
**Arquitectura:** Interfaces + Eventos (igual que HealthController)

---

## 🎯 Objetivo

Sistema genérico de interacción donde el Player puede interactuar con objetos en el mundo (pickups, NPCs, puertas, etc) presionando la tecla E.

---

## 🏗️ Arquitectura

### Componentes Principales

```
Player
├── PlayerInteractionController  (IInteractor)
│   ├── Detecta IInteractables cercanos
│   ├── Maneja input (tecla E)
│   └── Dispara eventos

Objeto Interactuable
├── PickupInteractable (IInteractable)
│   ├── Implementa lógica de pickup
│   ├── Se añade a inventario
│   └── Se destruye/desactiva
```

---

## 📦 Interfaces

### IInteractable

```csharp
public interface IInteractable
{
    string InteractionPrompt { get; }      // "Press E to pick up Sword"
    bool CanInteract(GameObject interactor); // Validación
    void Interact(GameObject interactor);   // Ejecutar interacción
    bool IsInteractable { get; }           // Estado
}
```

### IInteractor

```csharp
public interface IInteractor
{
    IInteractable CurrentInteractable { get; }
    bool CanInteract { get; }
    
    void SetInteractable(IInteractable interactable);
    void ClearInteractable();
    void TryInteract();
    
    event Action<IInteractable> OnInteractableDetected;
    event Action OnInteractableCleared;
    event Action<IInteractable> OnInteracted;
}
```

---

## 🎮 Flujo de Interacción

### Detección

```
Update()
    ↓
Physics2D.OverlapCircle(radius, interactionLayer)
    ↓
Encuentra IInteractables
    ↓
Selecciona el más cercano
    ↓
SetInteractable() → OnInteractableDetected(interactable)
```

### Interacción

```
Player presiona E
    ↓
OnInteractPerformed(InputAction.CallbackContext)
    ↓
TryInteract()
    ├─ Guard: CanInteract? → false: return
    ├─ currentInteractable.Interact(player)
    └─ OnInteracted(interactable)
    ↓
PickupInteractable.OnInteract()
    ├─ AddToInventory()
    ├─ PlayFeedback()
    └─ Destroy(gameObject)
```

---

## 🚀 Setup Paso a Paso

### 1. Configurar Input System

**Ya está configurado:** Acción "Interact" en `InputSystem_Actions`

### 2. Configurar Player

1. Añadir `PlayerInteractionController` al GameObject Player
2. Configurar:
   - Detection Radius: `2.0`
   - Interaction Layer: crear layer "Interactable"
   - Interact Action: arrastra la acción desde Input Actions

```
Player (GameObject)
└── PlayerInteractionController
    ├── Detection Radius: 2
    ├── Interaction Layer: Interactable
    └── Interact Action: Player/Interact
```

### 3. Crear Objeto Interactuable

1. Crear GameObject (ej: "Sword Pickup")
2. Añadir `PickupInteractable`
3. Configurar layer "Interactable"
4. Configurar Collider2D (trigger)
5. Configurar settings:

```
Sword Pickup (GameObject)
├── Layer: Interactable
├── Collider2D (isTrigger: true)
└── PickupInteractable
    ├── Item Name: "Sword"
    ├── Destroy On Pickup: true
    ├── Pickup VFX: (opcional)
    └── Pickup Sound: (opcional)
```

---

## 🎨 Ejemplo de Uso: UI Prompt

```csharp
using UnityEngine;
using UnityEngine.UI;
using TheHunt.Interaction;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Text promptText;
    [SerializeField] private GameObject promptPanel;
    
    private IInteractor interactor;
    
    void Start()
    {
        interactor = player.GetComponent<IInteractor>();
        
        if (interactor != null)
        {
            interactor.OnInteractableDetected += ShowPrompt;
            interactor.OnInteractableCleared += HidePrompt;
        }
        
        HidePrompt();
    }
    
    void ShowPrompt(IInteractable interactable)
    {
        promptText.text = interactable.InteractionPrompt;
        promptPanel.SetActive(true);
    }
    
    void HidePrompt()
    {
        promptPanel.SetActive(false);
    }
    
    void OnDestroy()
    {
        if (interactor != null)
        {
            interactor.OnInteractableDetected -= ShowPrompt;
            interactor.OnInteractableCleared -= HidePrompt;
        }
    }
}
```

---

## 🛠️ Ejemplo: Objeto Personalizado

```csharp
using UnityEngine;
using TheHunt.Interaction;

public class ChestInteractable : InteractableObject
{
    [SerializeField] private GameObject[] lootItems;
    [SerializeField] private Animator animator;
    
    private bool isOpened = false;
    
    void Awake()
    {
        interactionPrompt = "Press E to open chest";
    }
    
    public override bool CanInteract(GameObject interactor)
    {
        return base.CanInteract(interactor) && !isOpened;
    }
    
    protected override void OnInteract(GameObject interactor)
    {
        isOpened = true;
        animator.SetTrigger("open");
        
        foreach (GameObject item in lootItems)
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
        
        SetInteractable(false);
    }
}
```

---

## 🔧 Ejemplo: NPC Dialogue

```csharp
using UnityEngine;
using TheHunt.Interaction;

public class NPCInteractable : InteractableObject
{
    [SerializeField] private string[] dialogueLines;
    
    void Awake()
    {
        interactionPrompt = "Press E to talk";
    }
    
    protected override void OnInteract(GameObject interactor)
    {
        Debug.Log($"NPC says: {dialogueLines[0]}");
    }
}
```

---

## 📊 Comparación con HealthController

| Aspecto | HealthController | InteractionController |
|---------|------------------|----------------------|
| Interface principal | `IHealth`, `IDamageable`, `IHealable` | `IInteractable`, `IInteractor` |
| Eventos | `OnHealthChanged`, `OnDamaged`, `OnHealed`, `OnDeath` | `OnInteractableDetected`, `OnInteractableCleared`, `OnInteracted` |
| Componente base | `HealthController` | `PlayerInteractionController` |
| Implementaciones | Damage, Heal, Regeneration | Pickup, Chest, NPC, Door |
| Patrón | Event-driven | Event-driven |
| Desacoplamiento | ✅ Completo | ✅ Completo |

---

## ✅ Ventajas de Esta Arquitectura

1. **Extensible:** Crear nuevos interactuables heredando `InteractableObject`
2. **Desacoplado:** UI/Audio/VFX escuchan eventos
3. **Testeable:** Interfaces facilitan unit testing
4. **Reutilizable:** Mismo sistema para Player/NPC
5. **Configurable:** Settings en Inspector
6. **Performance:** Detection optimizada con `OverlapCircleNonAlloc`

---

## 🎯 Próximos Pasos

1. ✅ Añadir `PlayerInteractionController` al Player
2. ✅ Crear layer "Interactable"
3. ✅ Testear con `PickupInteractable`
4. 📝 Crear UI prompt
5. 📝 Integrar con sistema de inventario
6. 📝 Crear más tipos de interactuables

---

## 🔗 Integración con Inventario

Cuando tengas el sistema de inventario, modifica `PickupInteractable.AddToInventory()`:

```csharp
bool AddToInventory(GameObject interactor)
{
    IInventory inventory = interactor.GetComponent<IInventory>();
    
    if (inventory != null && itemPrefab != null)
    {
        bool success = inventory.AddItem(itemPrefab);
        
        if (success)
        {
            Debug.Log($"<color=green>[PICKUP] Added {itemName} to inventory</color>");
            return true;
        }
    }
    
    Debug.LogWarning($"<color=yellow>[PICKUP] Failed to add {itemName} to inventory</color>");
    return false;
}
```

---

**Arquitectura coherente con HealthController ✅**  
**Interfaces + Eventos ✅**  
**Extensible y desacoplado ✅**
