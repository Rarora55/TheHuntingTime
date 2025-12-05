# 🏗️ Arquitectura del Sistema de Inventario

## 📚 Índice

1. [Visión General](#visión-general)
2. [Capas de la Arquitectura](#capas-de-la-arquitectura)
3. [Componentes Principales](#componentes-principales)
4. [Flujo de Datos](#flujo-de-datos)
5. [Patrones de Diseño](#patrones-de-diseño)
6. [Sistemas Relacionados](#sistemas-relacionados)
7. [Ejemplos Prácticos](#ejemplos-prácticos)

---

## 🎯 Visión General

El sistema de inventario sigue una **arquitectura en capas** que separa responsabilidades y facilita el mantenimiento y extensibilidad.

### Principios Clave

- **Separación de Responsabilidades:** Cada componente tiene un propósito específico
- **Data-Driven:** Los items se definen como ScriptableObjects
- **Event-Driven:** Comunicación mediante eventos para bajo acoplamiento
- **Extensible:** Fácil añadir nuevos tipos de items y funcionalidades

### Stack Tecnológico

```
Unity Input System
      ↓
C# Events & Delegates
      ↓
ScriptableObjects (Data)
      ↓
MonoBehaviour (Logic)
```

---

## 🏛️ Capas de la Arquitectura

```
┌─────────────────────────────────────────────────────────┐
│                    CAPA DE INPUT                        │
│  • PlayerInputHandler                                   │
│  • Unity Input System                                   │
└────────────────────────┬────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│                   CAPA DE PRESENTACIÓN                  │
│  • InventoryUIController (States & Context Menu)        │
│  • InventoryDebugger (Debug UI)                         │
└────────────────────────┬────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│                   CAPA DE LÓGICA                        │
│  • InventorySystem (Core Logic)                         │
│  • ItemInstance (Runtime Data)                          │
└────────────────────────┬────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│                   CAPA DE DATOS                         │
│  • ItemData (ScriptableObjects)                         │
│  • ConsumableItemData, WeaponItemData, etc.             │
└────────────────────────┬────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│                 CAPA DE CONTRATOS                       │
│  • IUsable, IEquippable, IExaminable                    │
│  • Enums (ItemType, AmmoType, etc.)                     │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 Componentes Principales

### 1. Capa de Input

#### **PlayerInputHandler.cs**

**Responsabilidad:** Traducir inputs del jugador a acciones del inventario

**Ubicación:** `/Assets/Scripts/NewInput/`

**Funciones:**
- Lee el Unity Input System
- Delega comandos a `InventoryUIController`
- Maneja contexto (inventario abierto vs cerrado)

**Métodos Clave:**
```csharp
OnInventoryToggleInput()      // Tab: Abre/cierra
OnInventoryNavigateInput()    // ← → ↑ ↓: Navega
OnInventoryInteractInput()    // E: Confirma/Abre menú
OnInventoryCancelInput()      // Esc: Cancela
```

**Flujo:**
```
Usuario presiona Tab
    ↓
Unity Input System detecta
    ↓
OnInventoryToggleInput() ejecuta
    ↓
inventoryUIController.ToggleInventory()
```

---

### 2. Capa de Presentación

#### **InventoryUIController.cs** ⭐ (Nuevo)

**Responsabilidad:** Gestionar estados del inventario y el menú contextual

**Ubicación:** `/Assets/Scripts/Inventory/UI/`

**Funciones:**
- **Máquina de estados:** Closed → Open → ContextMenu
- **Construcción dinámica** del menú contextual
- **Validación** de acciones disponibles
- **Pausa/Resume** del juego
- **Navegación contextual** (items vs menú)

**Estados:**

```csharp
public enum InventoryState
{
    Closed,        // Juego normal, inventario cerrado
    Open,          // Inventario abierto, navegando items
    ContextMenu    // Menú contextual abierto, seleccionando acción
}
```

**Propiedades Públicas:**
```csharp
InventoryState CurrentState { get; }                 // Estado actual
bool IsOpen { get; }                                 // ¿Está abierto?
bool IsInContextMenu { get; }                        // ¿Menú activo?
List<ItemContextAction> AvailableActions { get; }    // Acciones disponibles
int ContextMenuIndex { get; }                        // Opción seleccionada
```

**Eventos:**
```csharp
event Action<InventoryState> OnStateChanged;
event Action<List<ItemContextAction>> OnContextMenuOpened;
event Action OnContextMenuClosed;
event Action<int> OnContextMenuSelectionChanged;
```

**Métodos Principales:**

```csharp
// Control de estado
ToggleInventory()          // Abre/cierra inventario
OpenInventory()            // Abre y pausa
CloseInventory()           // Cierra y resume

// Navegación
NavigateInventory(float)   // Navega items (← →)
NavigateContextMenu(float) // Navega menú (↑ ↓)

// Interacción
InteractWithCurrentItem()  // E: Abre menú o ejecuta acción
CancelCurrentAction()      // Esc: Cierra menú/inventario

// Menú contextual
OpenContextMenu()          // Construye y abre menú
CloseContextMenu()         // Cierra menú
ExecuteContextAction()     // Ejecuta acción seleccionada
```

**Algoritmo de Construcción del Menú:**

```csharp
1. Obtener item actual → currentItem
2. Limpiar lista → availableActions.Clear()
3. Validar "Use":
   if (item is IUsable && CanUse()) → ADD "Use"
4. Validar "Examine":
   if (item.CanBeExamined) → ADD "Examine"
5. Validar "Equip":
   if (item is WeaponItemData)
      → ADD "EquipPrimary"
      → ADD "EquipSecondary"
6. Siempre → ADD "Drop"
7. Cambiar estado → ContextMenu
8. Emitir evento → OnContextMenuOpened
```

**Ejemplo de Menú Generado:**

```
Health Potion (salud 80/100):
┌──────────────┐
│ ► Use        │  ← CanUse() = true (salud < max)
│   Examine    │
│   Drop       │
└──────────────┘

Pistol:
┌──────────────────┐
│ ► Equip Primary  │
│   Equip Secondary│
│   Examine        │
│   Drop           │
└──────────────────┘

Health Potion (salud 100/100):
┌──────────────┐
│ ► Examine    │  ← "Use" no aparece (salud llena)
│   Drop       │
└──────────────┘
```

---

#### **InventoryDebugger.cs**

**Responsabilidad:** Herramientas de debugging y testing

**Ubicación:** `/Assets/Scripts/Inventory/`

**Funciones:**
- Visualización en tiempo real del estado
- Quick add de items (F1-F3)
- Console logging de eventos
- OnGUI debug panel

**Eventos Suscritos:**

```csharp
// De InventorySystem:
OnItemAdded, OnItemRemoved, OnItemUsed
OnSelectionChanged, OnInventoryFull
OnWeaponEquipped, OnWeaponUnequipped
OnAmmoChanged

// De InventoryUIController:
OnStateChanged, OnContextMenuOpened, OnContextMenuClosed
```

---

### 3. Capa de Lógica

#### **InventorySystem.cs**

**Responsabilidad:** Lógica core del inventario (datos, reglas, eventos)

**Ubicación:** `/Assets/Scripts/Inventory/Core/`

**Funciones:**
- Gestión de **6 slots** de inventario
- Stacking de items (hasta MaxStackSize)
- Sistema de **equipamiento** de armas (Primary/Secondary)
- Gestión de **munición** por tipo
- Validación de reglas de negocio
- Emisión de eventos

**Estructura de Datos:**

```csharp
private ItemInstance[] items = new ItemInstance[MAX_SLOTS];
private int selectedSlot = 0;
private Dictionary<AmmoType, int> ammoInventory;
private WeaponItemData primaryWeapon;
private WeaponItemData secondaryWeapon;
```

**API Pública:**

```csharp
// Propiedades
int SelectedSlot { get; }
ItemInstance[] Items { get; }
ItemInstance CurrentItem { get; }
bool IsFull { get; }
WeaponItemData PrimaryWeapon { get; }
WeaponItemData SecondaryWeapon { get; }

// Añadir/Remover
bool TryAddItem(ItemData itemData, int quantity = 1)
bool RemoveItemAt(int slot, int quantity = 1)

// Navegación
void SelectNext()
void SelectPrevious()

// Acciones
bool UseCurrentItem()
void DropCurrentItem()

// Equipamiento
void EquipWeapon(WeaponItemData weapon, EquipSlot slot)
void UnequipWeapon(EquipSlot slot)
void SwapWeapons()

// Munición
int GetAmmoCount(AmmoType type)
bool AddAmmo(AmmoType type, int amount)
bool ConsumeAmmo(AmmoType type, int amount)

// Utilidades
int GetItemCount(ItemData itemData)
int FindItemSlot(ItemData itemData)
```

**Eventos:**

```csharp
event Action<int, ItemInstance> OnItemAdded;
event Action<int, ItemInstance> OnItemRemoved;
event Action<ItemInstance> OnItemUsed;
event Action<int, int> OnSelectionChanged;
event Action OnInventoryFull;
event Action<EquipSlot, WeaponItemData> OnWeaponEquipped;
event Action<EquipSlot> OnWeaponUnequipped;
event Action<AmmoType, int> OnAmmoChanged;
```

**Algoritmo de Añadir Item:**

```csharp
TryAddItem(ItemData itemData, int quantity):
    1. if (itemData == null) → return false
    
    2. Si es munición:
       → AddAmmo(ammoType, amount)
       → return true
    
    3. Buscar slot existente con mismo item:
       for cada slot:
           if (item == itemData && stack < maxStack):
               → añadir a stack existente
               → emitir OnItemAdded
               → return true
    
    4. Buscar slot vacío:
       for cada slot:
           if (slot vacío):
               → crear nuevo ItemInstance
               → asignar a slot
               → emitir OnItemAdded
               → return true
    
    5. Inventario lleno:
       → emitir OnInventoryFull
       → return false
```

---

#### **ItemInstance.cs**

**Responsabilidad:** Representar un item en runtime con cantidad

**Ubicación:** `/Assets/Scripts/Inventory/Core/`

**Estructura:**

```csharp
public class ItemInstance
{
    public ItemData itemData;     // Referencia al ScriptableObject
    public int quantity;           // Cantidad en el stack
    
    // Propiedades de conveniencia
    public string DisplayName => 
        quantity > 1 ? $"{itemData.ItemName} x{quantity}" 
                     : itemData.ItemName;
}
```

**Propósito:**
- `ItemData` es el **template** (ScriptableObject)
- `ItemInstance` es la **instancia en runtime** (con cantidad)

**Ejemplo:**

```csharp
// ItemData (ScriptableObject)
HealthPotionData {
    itemName = "Health Potion"
    maxStackSize = 5
    healAmount = 50
}

// ItemInstance (Runtime)
ItemInstance {
    itemData = HealthPotionData
    quantity = 3                  // Tenemos 3 pociones
}
```

---

### 4. Capa de Datos

#### **ItemData.cs** (Base)

**Responsabilidad:** Clase base para todos los items

**Ubicación:** `/Assets/Scripts/Inventory/Data/`

**Campos Comunes:**

```csharp
[Header("Basic Info")]
protected string itemName;
protected string description;
protected ItemType itemType;

[Header("Inventory Settings")]
protected int maxStackSize = 1;
protected bool canBeExamined = true;

[Header("Examination")]
[TextArea(3, 6)]
protected string examinationDescription;

[Header("Visuals")]
protected Sprite icon;
```

**Propiedades:**

```csharp
public string ItemName { get; }
public string Description { get; }
public ItemType ItemType { get; }
public int MaxStackSize { get; }
public bool CanBeExamined { get; }
public string ExaminationDescription { get; }
public Sprite Icon { get; }
```

---

#### **ConsumableItemData.cs**

**Responsabilidad:** Items que se pueden usar (pociones, comida)

**Ubicación:** `/Assets/Scripts/Inventory/Data/`

**Hereda de:** `ItemData, IUsable`

**Campos Adicionales:**

```csharp
[Header("Consumable Settings")]
[SerializeField] private float healAmount = 50f;
[SerializeField] private bool removeOnUse = true;
```

**Interfaz IUsable:**

```csharp
public bool CanUse(GameObject user)
{
    HealthController health = user.GetComponent<HealthController>();
    if (health == null) return false;
    
    // Solo se puede usar si la salud no está llena
    return health.CurrentHealth < health.MaxHealth;
}

public bool Use(GameObject user)
{
    HealthController health = user.GetComponent<HealthController>();
    if (health != null)
    {
        health.Heal(healAmount);
        return removeOnUse;  // true = remover del inventario
    }
    return false;
}
```

**Ejemplo de Uso:**

```
Player presiona E sobre Health Potion
    ↓
Menú muestra "Use" (porque CanUse() = true)
    ↓
Player selecciona "Use" y presiona E
    ↓
ExecuteContextAction(Use)
    ↓
inventorySystem.UseCurrentItem()
    ↓
item.itemData.Use(gameObject)
    ↓
health.Heal(50)
    ↓
RemoveItemAt() porque removeOnUse = true
```

---

#### **WeaponItemData.cs**

**Responsabilidad:** Items de armas equipables

**Ubicación:** `/Assets/Scripts/Inventory/Data/`

**Hereda de:** `ItemData, IEquippable`

**Campos Adicionales:**

```csharp
[Header("Weapon Stats")]
[SerializeField] private WeaponType weaponType;
[SerializeField] private AmmoType ammoType;
[SerializeField] private float damage = 25f;
[SerializeField] private float fireRate = 0.5f;
[SerializeField] private int magazineSize = 15;
[SerializeField] private float reloadTime = 2f;

[Header("Prefabs")]
[SerializeField] private GameObject weaponPrefab;
```

**Propiedades:**

```csharp
public WeaponType WeaponType { get; }
public AmmoType AmmoType { get; }
public float Damage { get; }
public float FireRate { get; }
public int MagazineSize { get; }
public float ReloadTime { get; }
public GameObject WeaponPrefab { get; }
```

**Interfaz IEquippable:**

```csharp
public void Equip(GameObject user, EquipSlot slot)
{
    // Lógica de equipamiento
    Debug.Log($"Equipped {itemName} to {slot}");
}

public void Unequip(GameObject user, EquipSlot slot)
{
    // Lógica de desequipamiento
    Debug.Log($"Unequipped from {slot}");
}
```

---

#### **AmmoItemData.cs**

**Responsabilidad:** Items de munición

**Ubicación:** `/Assets/Scripts/Inventory/Data/`

**Hereda de:** `ItemData`

**Campos Adicionales:**

```csharp
[Header("Ammo Settings")]
[SerializeField] private AmmoType ammoType;
[SerializeField] private int amountPerBox = 30;
```

**Propiedades:**

```csharp
public AmmoType AmmoType { get; }
public int AmountPerBox { get; }
```

**Comportamiento Especial:**

La munición **no ocupa slots** del inventario, va directamente al contador de munición:

```csharp
TryAddItem(AmmoItemData ammo):
    → AddAmmo(ammo.AmmoType, ammo.AmountPerBox)
    → NO crea ItemInstance en slots
```

---

#### **KeyItemData.cs**

**Responsabilidad:** Items especiales (llaves, documentos)

**Ubicación:** `/Assets/Scripts/Inventory/Data/`

**Hereda de:** `ItemData`

**Campos Adicionales:**

```csharp
[Header("Key Item Settings")]
[SerializeField] private string keyId;
[SerializeField] private bool isQuestItem;
[SerializeField] private bool canBeDropped = false;
```

**Características:**
- `MaxStackSize = 1` (único)
- `CanBeExamined = true` (siempre)
- No se puede usar ni equipar
- Solo se puede examinar y (opcionalmente) soltar

---

### 5. Capa de Contratos (Interfaces)

#### **IUsable.cs**

**Propósito:** Items que se pueden usar

```csharp
public interface IUsable
{
    bool CanUse(GameObject user);
    bool Use(GameObject user);
}
```

**Implementado por:** `ConsumableItemData`

---

#### **IEquippable.cs**

**Propósito:** Items que se pueden equipar

```csharp
public interface IEquippable
{
    void Equip(GameObject user, EquipSlot slot);
    void Unequip(GameObject user, EquipSlot slot);
}
```

**Implementado por:** `WeaponItemData`

---

#### **IExaminable.cs**

**Propósito:** Items que se pueden examinar (futuro)

```csharp
public interface IExaminable
{
    string GetExaminationText();
    void OnExamine(GameObject examiner);
}
```

**Nota:** Actualmente todos los items tienen `CanBeExamined` boolean. Esta interfaz está preparada para un sistema de examinación 3D más complejo.

---

## 🔄 Flujo de Datos

### Flujo Completo: Usar Poción

```
1. INPUT LAYER
   Usuario presiona Tab
   ↓
   PlayerInputHandler.OnInventoryToggleInput()
   
2. PRESENTATION LAYER
   ↓
   InventoryUIController.ToggleInventory()
   ↓
   SetState(Open)
   ↓
   Time.timeScale = 0
   ↓
   OnStateChanged?.Invoke(Open)
   
3. Usuario presiona →
   ↓
   PlayerInputHandler.OnInventoryNavigateInput(1.0)
   
4. PRESENTATION LAYER
   ↓
   InventoryUIController.NavigateInventory(1.0)
   
5. LOGIC LAYER
   ↓
   InventorySystem.SelectNext()
   ↓
   selectedSlot = (selectedSlot + 1) % MAX_SLOTS
   ↓
   OnSelectionChanged?.Invoke(oldSlot, newSlot)
   
6. Usuario presiona E
   ↓
   PlayerInputHandler.OnInventoryInteractInput()
   
7. PRESENTATION LAYER
   ↓
   InventoryUIController.InteractWithCurrentItem()
   ↓
   OpenContextMenu()
   
8. LOGIC LAYER
   ↓
   currentItem = inventorySystem.CurrentItem
   
9. DATA LAYER
   ↓
   ItemInstance.itemData (HealthPotionData)
   
10. PRESENTATION LAYER - Validación
    ↓
    item is IUsable? → YES
    ↓
    CanUse(gameObject)? → YES (salud < max)
    ↓
    availableActions.Add(Use)
    ↓
    availableActions.Add(Examine)
    ↓
    availableActions.Add(Drop)
    ↓
    SetState(ContextMenu)
    ↓
    OnContextMenuOpened?.Invoke([Use, Examine, Drop])
    
11. Usuario presiona E (ejecutar)
    ↓
    PlayerInputHandler.OnInventoryInteractInput()
    
12. PRESENTATION LAYER
    ↓
    InventoryUIController.ExecuteContextAction()
    ↓
    switch(Use)
    
13. LOGIC LAYER
    ↓
    InventorySystem.UseCurrentItem()
    ↓
    item.itemData.Use(gameObject)
    
14. DATA LAYER
    ↓
    ConsumableItemData.Use()
    ↓
    health.Heal(healAmount)
    ↓
    return removeOnUse (true)
    
15. LOGIC LAYER
    ↓
    RemoveItemAt(slot, 1)
    ↓
    OnItemRemoved?.Invoke(slot, item)
    
16. PRESENTATION LAYER
    ↓
    CloseContextMenu()
    ↓
    SetState(Open)
    
17. EXTERNAL SYSTEMS
    ↓
    HealthController.Heal(50)
    ↓
    currentHealth += 50
    ↓
    OnHealed?.Invoke(50)
```

---

## 🎨 Patrones de Diseño

### 1. **Strategy Pattern** (IUsable, IEquippable)

**Propósito:** Diferentes items tienen diferentes comportamientos

```csharp
// Strategy definida por interfaz
interface IUsable {
    bool Use(GameObject user);
}

// Implementaciones específicas
ConsumableItemData.Use() → Cura vida
WeaponItemData.Use() → No implementa (no usable)
```

---

### 2. **Observer Pattern** (Eventos)

**Propósito:** Comunicación desacoplada entre sistemas

```csharp
// Publisher
InventorySystem {
    event Action<int, ItemInstance> OnItemAdded;
    
    TryAddItem() {
        OnItemAdded?.Invoke(slot, item);  // Notifica
    }
}

// Subscribers
InventoryDebugger.OnItemAdded() → Log
InventoryUI.OnItemAdded() → Actualiza visual
AudioManager.OnItemAdded() → Reproduce sonido
```

---

### 3. **State Pattern** (InventoryUIController)

**Propósito:** Comportamiento diferente según estado

```csharp
InventoryState currentState;

NavigateInput(float value) {
    switch(currentState) {
        case Open:
            NavigateInventory(value);    // Navega items
            break;
        case ContextMenu:
            NavigateContextMenu(value);  // Navega opciones
            break;
    }
}
```

---

### 4. **Flyweight Pattern** (ItemData como ScriptableObjects)

**Propósito:** Compartir datos inmutables entre instancias

```csharp
// UNA SOLA instancia del ScriptableObject (Flyweight)
HealthPotionData (ScriptableObject)

// MÚLTIPLES referencias en runtime
Player tiene: ItemInstance { itemData = HealthPotionData, qty = 3 }
Ground tiene: PickupItem { itemData = HealthPotionData }
Vendor tiene: ShopItem { itemData = HealthPotionData, price = 50 }

→ Todos comparten el MISMO ScriptableObject en memoria
```

---

### 5. **Command Pattern** (ItemContextAction)

**Propósito:** Encapsular acciones como objetos

```csharp
enum ItemContextAction {
    Use, Examine, Drop, EquipPrimary, EquipSecondary
}

ExecuteContextAction() {
    ItemContextAction action = availableActions[contextMenuIndex];
    
    switch(action) {
        case Use: inventorySystem.UseCurrentItem(); break;
        case Examine: ExamineItem(); break;
        case Drop: inventorySystem.DropCurrentItem(); break;
        // etc.
    }
}
```

---

### 6. **Facade Pattern** (InventoryUIController)

**Propósito:** Simplificar interfaz compleja

```csharp
// Sistema complejo:
InventorySystem + Time.timeScale + State Machine + Validation

// Fachada simple:
InventoryUIController {
    ToggleInventory() {
        // Internamente:
        // - Cambia estado
        // - Pausa juego
        // - Emite eventos
        // - Valida acciones
    }
}

// Uso simple:
inventoryUIController.ToggleInventory();  // Una sola llamada
```

---

## 🔗 Sistemas Relacionados

### HealthController

**Integración:** Los consumibles interactúan con el sistema de salud

```csharp
ConsumableItemData.Use() {
    HealthController health = user.GetComponent<HealthController>();
    health.Heal(healAmount);
}
```

---

### WeaponSystem (Futuro)

**Integración:** Las armas equipadas se comunican con el sistema de combate

```csharp
WeaponController {
    void Start() {
        inventorySystem.OnWeaponEquipped += OnWeaponChanged;
    }
    
    void OnWeaponChanged(EquipSlot slot, WeaponItemData weapon) {
        if (slot == EquipSlot.Primary)
            InstantiateWeapon(weapon.WeaponPrefab);
    }
}
```

---

### PickupSystem

**Integración:** Recoger items del mundo

```csharp
public class PickupItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    
    public void Pickup(GameObject player)
    {
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory.TryAddItem(itemData))
            Destroy(gameObject);
    }
}
```

---

### SaveSystem (Futuro)

**Serialización:**

```csharp
[System.Serializable]
public class InventorySaveData
{
    public ItemSaveData[] items;
    public int selectedSlot;
    public AmmoSaveData[] ammo;
}

[System.Serializable]
public class ItemSaveData
{
    public string itemGuid;  // GUID del ScriptableObject
    public int quantity;
}
```

---

## 💡 Ejemplos Prácticos

### Ejemplo 1: Crear Nuevo Tipo de Item

```csharp
// 1. Crear nuevo ScriptableObject
[CreateAssetMenu(menuName = "Inventory/Food Item")]
public class FoodItemData : ItemData, IUsable
{
    [SerializeField] private float hungerRestore = 30f;
    [SerializeField] private float healthRestore = 10f;
    
    public bool CanUse(GameObject user)
    {
        HungerSystem hunger = user.GetComponent<HungerSystem>();
        return hunger != null && hunger.CurrentHunger < hunger.MaxHunger;
    }
    
    public bool Use(GameObject user)
    {
        HungerSystem hunger = user.GetComponent<HungerSystem>();
        HealthController health = user.GetComponent<HealthController>();
        
        hunger?.RestoreHunger(hungerRestore);
        health?.Heal(healthRestore);
        
        return true;  // Remover después de usar
    }
}

// 2. La UI ya lo maneja automáticamente:
// - Aparecerá "Use" en el menú contextual
// - Se ejecutará Food.Use() cuando se seleccione
```

---

### Ejemplo 2: Extender el Menú Contextual

```csharp
// Añadir nueva acción "Combine"
public enum ItemContextAction
{
    Use, Examine, Drop, EquipPrimary, EquipSecondary,
    Combine  // ← Nueva acción
}

// En InventoryUIController.OpenContextMenu():
if (currentItem.itemData is ICombineable)
{
    availableActions.Add(ItemContextAction.Combine);
}

// En ExecuteContextAction():
case ItemContextAction.Combine:
    OpenCombineMenu();
    break;
```

---

### Ejemplo 3: UI Visual (Canvas)

```csharp
public class InventoryUICanvas : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private InventoryUIController uiController;
    [SerializeField] private InventorySlotUI[] slotUIs;
    [SerializeField] private ContextMenuUI contextMenuUI;
    
    void Start()
    {
        // Suscribirse a eventos
        inventorySystem.OnItemAdded += UpdateSlotVisual;
        inventorySystem.OnItemRemoved += UpdateSlotVisual;
        inventorySystem.OnSelectionChanged += UpdateSelection;
        
        uiController.OnStateChanged += OnStateChanged;
        uiController.OnContextMenuOpened += ShowContextMenu;
        uiController.OnContextMenuClosed += HideContextMenu;
    }
    
    void OnStateChanged(InventoryState state)
    {
        gameObject.SetActive(state != InventoryState.Closed);
    }
    
    void ShowContextMenu(List<ItemContextAction> actions)
    {
        contextMenuUI.Show(actions);
    }
    
    void UpdateSlotVisual(int slot, ItemInstance item)
    {
        slotUIs[slot].SetItem(item);
    }
}
```

---

## 📋 Resumen de Responsabilidades

| Componente | Responsabilidad | Layer |
|-----------|----------------|-------|
| `PlayerInputHandler` | Traducir inputs → comandos | Input |
| `InventoryUIController` | Estados, menú contextual, pausa | Presentación |
| `InventoryDebugger` | Debugging, testing | Presentación |
| `InventorySystem` | Lógica core, datos runtime | Lógica |
| `ItemInstance` | Item en runtime con cantidad | Lógica |
| `ItemData` | Template de item (immutable) | Datos |
| `ConsumableItemData` | Comportamiento consumibles | Datos |
| `WeaponItemData` | Comportamiento armas | Datos |
| `AmmoItemData` | Comportamiento munición | Datos |
| `KeyItemData` | Comportamiento items clave | Datos |
| `IUsable` | Contrato para items usables | Contratos |
| `IEquippable` | Contrato para items equipables | Contratos |

---

## 🎯 Ventajas de Esta Arquitectura

### ✅ **Separación de Responsabilidades**
Cada componente hace UNA cosa y la hace bien

### ✅ **Bajo Acoplamiento**
Los sistemas se comunican mediante eventos, no referencias directas

### ✅ **Alta Cohesión**
Código relacionado está junto (ej: toda la lógica de inventario en InventorySystem)

### ✅ **Extensibilidad**
Fácil añadir nuevos tipos de items implementando interfaces

### ✅ **Testabilidad**
Cada componente se puede testear independientemente

### ✅ **Data-Driven**
Diseñadores pueden crear items sin tocar código

### ✅ **Mantenibilidad**
Cambios localizados no rompen otros sistemas

---

## 🚀 Próximos Pasos de Evolución

### Fase 1: UI Visual
- Canvas con sprites
- Animaciones de transición
- Tooltips

### Fase 2: Examinación 3D
- Rotar items en 3D
- Zoom
- Detalles interactivos

### Fase 3: Crafting
- Combinar items
- Recetas
- Desmontar items

### Fase 4: Persistencia
- Save/Load
- Cloud sync

### Fase 5: Multiplayer
- Inventario sincronizado
- Trading entre jugadores

---

**Esta arquitectura está diseñada para crecer con tu proyecto.** 🎮✨
