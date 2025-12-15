# 🌿 Flujo Completo: Del Mundo al Inventario

## 🎯 Caso de Uso: Planta Verde Medicinal

**Escenario:** Te encuentras una planta verde en el mundo que restaura 10 HP

**Pregunta:** ¿Cómo el inventario sabe que es un consumible?

**Respuesta:** A través de un flujo de **detección de tipo** usando **interfaces** y **polimorfismo**

---

## 📋 Paso a Paso Completo

### 🎨 Paso 1: Creación del ScriptableObject

**Responsabilidad:** Designer/Developer

**Ubicación:** `Assets/Data/Items/`

#### Crear el Asset

1. Click derecho en Project
2. **Create** → **Inventory** → **Consumable Item**
3. Renombrar: `GreenHerbItem`

#### Configurar Propiedades

```yaml
GreenHerbItem (ConsumableItemData)
├─ itemName: "Green Herb"
├─ description: "A medicinal plant that restores health"
├─ itemType: Consumable
├─ maxStackSize: 3
├─ canBeExamined: true
├─ examinationDescription: "A common medicinal herb with healing properties"
├─ icon: [sprite de planta]
├─ healAmount: 10.0          ← ESPECÍFICO de Consumable
└─ removeOnUse: true          ← ESPECÍFICO de Consumable
```

**Código Subyacente:**

```csharp
// El ScriptableObject ya implementa IUsable
[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable Item")]
public class ConsumableItemData : ItemData, IUsable
{
    [Header("Consumable Settings")]
    [SerializeField] private float healAmount = 50f;
    [SerializeField] private bool removeOnUse = true;
    
    // Implementación de IUsable
    public bool CanUse(GameObject user) { ... }
    public bool Use(GameObject user) { ... }
}
```

**Resultado:**
✅ Asset `GreenHerbItem.asset` creado
✅ **Es ConsumableItemData** (hereda de ItemData)
✅ **Implementa IUsable** (interfaz)

---

### 🌍 Paso 2: Colocar en el Mundo

**Responsabilidad:** Level Designer

**Ubicación:** Escena del juego

#### Crear GameObject en la Escena

```
Hierarchy:
  Environment
    ├── Tree
    ├── Rock
    └── GreenHerb ← NUEVO
        ├── Sprite Renderer (visual)
        ├── Collider2D (interacción)
        └── PickupItem (script) ← ESTE ES LA CLAVE
```

#### Configurar PickupItem Component

**Script necesario:** (Puedes crear este script básico)

```csharp
using UnityEngine;
using TheHunt.Inventory;

namespace TheHunt.Items
{
    public class PickupItem : MonoBehaviour
    {
        [Header("Item Data")]
        [SerializeField] private ItemData itemData;  // ← Acepta CUALQUIER ItemData
        
        [Header("Settings")]
        [SerializeField] private int quantity = 1;
        [SerializeField] private bool destroyOnPickup = true;
        
        public ItemData ItemData => itemData;
        
        public void Pickup(GameObject collector)
        {
            InventorySystem inventory = collector.GetComponent<InventorySystem>();
            
            if (inventory != null && inventory.TryAddItem(itemData, quantity))
            {
                Debug.Log($"Picked up {itemData.ItemName}");
                
                if (destroyOnPickup)
                    Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full!");
            }
        }
    }
}
```

#### Asignar el Item

En el **Inspector** del GameObject `GreenHerb`:

```
PickupItem (Script)
┌────────────────────────────────────┐
│ Item Data:  [GreenHerbItem]    ← Arrastra el ScriptableObject
│ Quantity:   1
│ Destroy On Pickup: ☑
└────────────────────────────────────┘
```

**Resultado:**
✅ Planta verde visible en el mundo
✅ Tiene referencia a `GreenHerbItem` (ConsumableItemData)
✅ Puede ser recogida

---

### 🎮 Paso 3: Recoger el Item

**Responsabilidad:** Player Interaction System

**Ubicación:** Runtime

#### Detección de Interacción

**Opción A: Trigger Automático**

```csharp
public class PickupItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other.gameObject);
        }
    }
}
```

**Opción B: Interacción Manual (E)**

Ya tienes `PlayerInteractionController`, se integraría así:

```csharp
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    
    public string InteractionPrompt => $"Press E to pick up {itemData.ItemName}";
    
    public void Interact(GameObject interactor)
    {
        Pickup(interactor);
    }
}
```

#### Flujo de Recogida

```
1. Player presiona E cerca de la planta
   ↓
2. PlayerInteractionController detecta IInteractable
   ↓
3. PickupItem.Interact() ejecuta
   ↓
4. PickupItem.Pickup(player) ejecuta
   ↓
5. inventory.TryAddItem(itemData, 1)
```

**Resultado:**
✅ Item recogido del mundo
✅ Añadido al inventario
✅ GameObject destruido (opcional)

---

### 📦 Paso 4: Añadir al Inventario

**Responsabilidad:** InventorySystem

**Ubicación:** `InventorySystem.cs`

#### Algoritmo de Detección

```csharp
public bool TryAddItem(ItemData itemData, int quantity = 1)
{
    if (itemData == null)
        return false;
    
    // ──────────────────────────────────────────
    // DETECCIÓN #1: ¿Es munición?
    // ──────────────────────────────────────────
    if (itemData is AmmoItemData ammoItem)
    {
        // No ocupa slot, va a contador de munición
        AddAmmo(ammoItem.AmmoType, ammoItem.AmountPerBox);
        OnItemAdded?.Invoke(-1, null);
        return true;
    }
    
    // ──────────────────────────────────────────
    // DETECCIÓN #2: ¿Es stackeable?
    // ──────────────────────────────────────────
    // Buscar slot existente del mismo item
    for (int i = 0; i < MAX_SLOTS; i++)
    {
        if (items[i] != null && 
            items[i].itemData == itemData &&
            items[i].quantity < itemData.MaxStackSize)
        {
            // Añadir al stack existente
            items[i].quantity += quantity;
            OnItemAdded?.Invoke(i, items[i]);
            return true;
        }
    }
    
    // ──────────────────────────────────────────
    // DETECCIÓN #3: Slot nuevo
    // ──────────────────────────────────────────
    for (int i = 0; i < MAX_SLOTS; i++)
    {
        if (items[i] == null)
        {
            // Crear nueva instancia
            items[i] = new ItemInstance
            {
                itemData = itemData,  // ← Guarda la referencia
                quantity = quantity
            };
            
            OnItemAdded?.Invoke(i, items[i]);
            return true;
        }
    }
    
    // Inventario lleno
    OnInventoryFull?.Invoke();
    return false;
}
```

**Ejemplo Concreto: Green Herb**

```
Input: TryAddItem(GreenHerbItem, 1)

1. ¿Es AmmoItemData? 
   → NO (es ConsumableItemData)
   
2. ¿Hay stack existente de GreenHerbItem?
   → NO (primera vez)
   
3. ¿Hay slot vacío?
   → SÍ (slot 0 está vacío)
   
4. Crear ItemInstance:
   items[0] = new ItemInstance {
       itemData = GreenHerbItem,  ← Referencia al ScriptableObject
       quantity = 1
   }
   
5. Emitir evento:
   OnItemAdded(0, items[0])

Resultado: ✅ Planta en slot 0
```

**Datos en Memoria:**

```
InventorySystem.items[0]
┌─────────────────────────────────────┐
│ ItemInstance                        │
├─────────────────────────────────────┤
│ itemData ───┐                       │
│ quantity: 1 │                       │
└─────────────┼───────────────────────┘
              │
              ↓
┌─────────────────────────────────────┐
│ GreenHerbItem                       │
│ (ConsumableItemData)                │
├─────────────────────────────────────┤
│ itemName: "Green Herb"              │
│ healAmount: 10f                     │
│ • IS ItemData         ✅            │
│ • IS ConsumableItemData ✅          │
│ • IS IUsable          ✅            │
└─────────────────────────────────────┘
```

**Resultado:**
✅ Item almacenado en slot 0
✅ Referencia a `GreenHerbItem` (ConsumableItemData)
✅ **El tipo original se preserva** (polimorfismo)

---

### 🎯 Paso 5: Abrir Menú Contextual

**Responsabilidad:** InventoryUIController

**Ubicación:** `InventoryUIController.cs`

#### Flujo de Detección de Tipo

```csharp
private void OpenContextMenu()
{
    // ──────────────────────────────────────────
    // 1. OBTENER ITEM ACTUAL
    // ──────────────────────────────────────────
    ItemInstance currentItem = inventorySystem.CurrentItem;
    
    if (currentItem == null || currentItem.itemData == null)
        return;
    
    // ──────────────────────────────────────────
    // 2. LIMPIAR MENÚ
    // ──────────────────────────────────────────
    availableActions.Clear();
    contextMenuIndex = 0;
    
    // ──────────────────────────────────────────
    // 3. DETECCIÓN DE TIPO: ¿Es IUsable?
    // ──────────────────────────────────────────
    if (currentItem.itemData is IUsable usable)  // ← POLIMORFISMO
    {
        if (usable.CanUse(gameObject))
        {
            availableActions.Add(ItemContextAction.Use);
        }
    }
    
    // ──────────────────────────────────────────
    // 4. DETECCIÓN: ¿Se puede examinar?
    // ──────────────────────────────────────────
    if (currentItem.itemData.CanBeExamined)
    {
        availableActions.Add(ItemContextAction.Examine);
    }
    
    // ──────────────────────────────────────────
    // 5. DETECCIÓN DE TIPO: ¿Es WeaponItemData?
    // ──────────────────────────────────────────
    if (currentItem.itemData is WeaponItemData)
    {
        availableActions.Add(ItemContextAction.EquipPrimary);
        availableActions.Add(ItemContextAction.EquipSecondary);
    }
    
    // ──────────────────────────────────────────
    // 6. SIEMPRE: Drop
    // ──────────────────────────────────────────
    availableActions.Add(ItemContextAction.Drop);
    
    // ──────────────────────────────────────────
    // 7. CAMBIAR ESTADO Y EMITIR EVENTO
    // ──────────────────────────────────────────
    if (availableActions.Count > 0)
    {
        SetState(InventoryState.ContextMenu);
        OnContextMenuOpened?.Invoke(availableActions);
    }
}
```

#### Ejemplo Concreto: Green Herb

**Estado Inicial:**
```
Player salud: 80/100
Item seleccionado: items[0] → GreenHerbItem
```

**Ejecución:**

```csharp
// 1. Obtener item
ItemInstance currentItem = inventorySystem.CurrentItem;
// currentItem.itemData = GreenHerbItem (ConsumableItemData)

// 2. Limpiar
availableActions.Clear();

// 3. Detección IUsable
if (currentItem.itemData is IUsable usable)
// ¿GreenHerbItem es IUsable? → SÍ ✅
{
    if (usable.CanUse(gameObject))
    // ¿Player puede usar? → health.CurrentHealth < health.MaxHealth
    // → 80 < 100 → SÍ ✅
    {
        availableActions.Add(ItemContextAction.Use);  // ← AÑADIDO
    }
}

// 4. Detección Examine
if (currentItem.itemData.CanBeExamined)  // → true ✅
{
    availableActions.Add(ItemContextAction.Examine);  // ← AÑADIDO
}

// 5. Detección Weapon
if (currentItem.itemData is WeaponItemData)
// ¿GreenHerbItem es WeaponItemData? → NO ❌
{
    // NO se ejecuta
}

// 6. Siempre Drop
availableActions.Add(ItemContextAction.Drop);  // ← AÑADIDO

// Resultado:
// availableActions = [Use, Examine, Drop]
```

**Menú Generado:**

```
┌──────────────────┐
│ Green Herb       │
├──────────────────┤
│ ► Use            │ ← Porque es IUsable y CanUse() = true
│   Examine        │ ← Porque CanBeExamined = true
│   Drop           │ ← Siempre disponible
└──────────────────┘
```

**Resultado:**
✅ Menú contextual generado dinámicamente
✅ "Use" aparece porque **el sistema detectó que es IUsable**
✅ Validación automática (CanUse)

---

### ⚡ Paso 6: Usar el Item

**Responsabilidad:** InventoryUIController + InventorySystem

**Ubicación:** Runtime

#### Usuario Selecciona "Use"

```csharp
// En InventoryUIController
private void ExecuteContextAction()
{
    if (availableActions.Count == 0)
        return;
    
    ItemContextAction selectedAction = availableActions[contextMenuIndex];
    
    switch (selectedAction)
    {
        case ItemContextAction.Use:
            inventorySystem.UseCurrentItem();  // ← Delega a InventorySystem
            CloseContextMenu();
            break;
        // ... otros casos
    }
}
```

#### InventorySystem Ejecuta

```csharp
// En InventorySystem
public bool UseCurrentItem()
{
    ItemInstance currentItem = CurrentItem;
    
    if (currentItem == null || currentItem.itemData == null)
        return false;
    
    // ──────────────────────────────────────────
    // DETECCIÓN Y EJECUCIÓN POLIMÓRFICA
    // ──────────────────────────────────────────
    if (currentItem.itemData is IUsable usable)  // ← DETECCIÓN
    {
        bool shouldRemove = usable.Use(gameObject);  // ← EJECUCIÓN POLIMÓRFICA
        
        OnItemUsed?.Invoke(currentItem);
        
        if (shouldRemove)
        {
            RemoveItemAt(selectedSlot, 1);
        }
        
        return true;
    }
    
    return false;
}
```

#### ConsumableItemData Ejecuta Use()

```csharp
// En GreenHerbItem (ConsumableItemData)
public bool Use(GameObject user)
{
    HealthController health = user.GetComponent<HealthController>();
    
    if (health != null)
    {
        health.Heal(healAmount);  // healAmount = 10f
        Debug.Log($"Used {itemName}, healed {healAmount} HP");
        return removeOnUse;  // true → remover del inventario
    }
    
    return false;
}
```

**Flujo Completo de Ejecución:**

```
1. Usuario presiona E sobre "Use"
   ↓
2. InventoryUIController.ExecuteContextAction()
   switch(Use) → inventorySystem.UseCurrentItem()
   ↓
3. InventorySystem.UseCurrentItem()
   if (itemData is IUsable usable)  ← DETECCIÓN
   ↓
4. usable.Use(player)  ← POLIMORFISMO
   // C# llama automáticamente a ConsumableItemData.Use()
   ↓
5. ConsumableItemData.Use()
   health.Heal(10f)
   return true
   ↓
6. InventorySystem.RemoveItemAt(0, 1)
   items[0] = null
   OnItemRemoved(0, ...)
   ↓
7. HealthController.Heal(10f)
   currentHealth = 80 + 10 = 90
   OnHealed(10)

Resultado:
✅ Salud restaurada 80 → 90
✅ Item removido del inventario
✅ Eventos emitidos
```

---

## 🔍 La Magia: Polimorfismo e Interfaces

### Cómo Funciona la Detección de Tipo

#### 1. **Herencia de Clases**

```csharp
ItemData (abstracta)
    ↓
ConsumableItemData : ItemData, IUsable
```

**En memoria:**

```
GreenHerbItem es:
✅ ItemData          (clase base)
✅ ConsumableItemData (clase derivada)
✅ IUsable           (interfaz implementada)
✅ Object            (todo es Object en C#)
```

#### 2. **Operador `is` (Type Checking)**

```csharp
ItemData item = items[0].itemData;  // Tipo declarado: ItemData

// C# chequea en runtime el tipo REAL del objeto
if (item is IUsable usable)
{
    // ✅ GreenHerbItem implementa IUsable
    // → La condición es TRUE
    // → 'usable' ahora es una referencia IUsable al mismo objeto
    
    usable.Use(gameObject);  // ← Llama a ConsumableItemData.Use()
}

if (item is WeaponItemData weapon)
{
    // ❌ GreenHerbItem NO es WeaponItemData
    // → La condición es FALSE
    // → Este bloque NO se ejecuta
}
```

#### 3. **Polimorfismo (Late Binding)**

```csharp
// Todas estas variables apuntan al MISMO objeto en memoria
ItemData item = items[0].itemData;        // Vista como ItemData
ConsumableItemData consumable = (ConsumableItemData)item;  // Vista como Consumable
IUsable usable = (IUsable)item;           // Vista como IUsable

// Cuando llamas a un método:
usable.Use(gameObject);

// C# busca la implementación REAL en runtime:
// 1. ¿Qué tipo es realmente el objeto? → ConsumableItemData
// 2. ¿ConsumableItemData tiene Use()? → SÍ
// 3. → Ejecuta ConsumableItemData.Use()
```

### Diagrama de Tipos en Memoria

```
┌──────────────────────────────────────────────┐
│  OBJETO REAL EN MEMORIA                      │
│  GreenHerbItem (ScriptableObject)            │
│                                              │
│  Tipo Real: ConsumableItemData               │
├──────────────────────────────────────────────┤
│  [ItemData] (base)                           │
│  • itemName: "Green Herb"                    │
│  • itemType: Consumable                      │
│  • maxStackSize: 3                           │
│  • canBeExamined: true                       │
├──────────────────────────────────────────────┤
│  [ConsumableItemData] (derivada)             │
│  • healAmount: 10f                           │
│  • removeOnUse: true                         │
├──────────────────────────────────────────────┤
│  [IUsable] (interfaz)                        │
│  • CanUse(GameObject) → implementado         │
│  • Use(GameObject) → implementado            │
└──────────────────────────────────────────────┘
         ▲            ▲            ▲
         │            │            │
         │            │            │
┌────────┴──┐  ┌──────┴────┐  ┌───┴─────┐
│ ItemData  │  │Consumable │  │ IUsable │
│ reference │  │  reference│  │reference│
└───────────┘  └───────────┘  └─────────┘
   En código      En código    En código
```

### Por Qué Funciona

**1. Almacenamiento Polimórfico:**

```csharp
// InventorySystem almacena como ItemData (base)
private ItemInstance[] items;

items[0] = new ItemInstance {
    itemData = GreenHerbItem  // ← ConsumableItemData asignado a ItemData
};
```

**2. Recuperación Polimórfica:**

```csharp
// Recuperar como ItemData
ItemData item = items[0].itemData;

// Pero el objeto REAL sigue siendo ConsumableItemData
// C# recuerda el tipo real en runtime
```

**3. Detección Dinámica:**

```csharp
// C# chequea el tipo real, no el tipo declarado
if (item is IUsable)  // ← TRUE si el tipo REAL implementa IUsable
{
    // Este bloque se ejecuta para GreenHerbItem
}
```

---

## 📊 Comparación: Diferentes Tipos de Items

### Ejemplo 1: Green Herb (Consumible)

```
┌─────────────────────────────────────┐
│ GreenHerbItem                       │
│ (ConsumableItemData)                │
├─────────────────────────────────────┤
│ ES ItemData? → ✅ SÍ                │
│ ES IUsable? → ✅ SÍ                 │
│ ES IEquippable? → ❌ NO             │
│ ES WeaponItemData? → ❌ NO          │
├─────────────────────────────────────┤
│ MENÚ GENERADO:                      │
│ ┌─────────────┐                     │
│ │ ► Use       │ ← IUsable detectado │
│ │   Examine   │                     │
│ │   Drop      │                     │
│ └─────────────┘                     │
└─────────────────────────────────────┘
```

### Ejemplo 2: Pistol (Arma)

```
┌─────────────────────────────────────┐
│ PistolItem                          │
│ (WeaponItemData)                    │
├─────────────────────────────────────┤
│ ES ItemData? → ✅ SÍ                │
│ ES IUsable? → ❌ NO                 │
│ ES IEquippable? → ✅ SÍ             │
│ ES WeaponItemData? → ✅ SÍ          │
├─────────────────────────────────────┤
│ MENÚ GENERADO:                      │
│ ┌─────────────────┐                 │
│ │ ► Equip Primary │ ← Weapon detect │
│ │   Equip Second  │                 │
│ │   Examine       │                 │
│ │   Drop          │                 │
│ └─────────────────┘                 │
└─────────────────────────────────────┘
```

### Ejemplo 3: Key (Item Especial)

```
┌─────────────────────────────────────┐
│ RustyKeyItem                        │
│ (KeyItemData)                       │
├─────────────────────────────────────┤
│ ES ItemData? → ✅ SÍ                │
│ ES IUsable? → ❌ NO                 │
│ ES IEquippable? → ❌ NO             │
│ ES WeaponItemData? → ❌ NO          │
├─────────────────────────────────────┤
│ MENÚ GENERADO:                      │
│ ┌─────────────┐                     │
│ │ ► Examine   │ ← Solo examine/drop │
│ │   Drop      │                     │
│ └─────────────┘                     │
└─────────────────────────────────────┘
```

### Ejemplo 4: Ammo (Munición)

```
┌─────────────────────────────────────┐
│ AmmoBox9mm                          │
│ (AmmoItemData)                      │
├─────────────────────────────────────┤
│ ES ItemData? → ✅ SÍ                │
│ NO OCUPA SLOT (va a contador)       │
├─────────────────────────────────────┤
│ MENÚ:                               │
│ No genera menú (no ocupa slot)      │
│ Se añade directamente al contador   │
│ de munición                         │
└─────────────────────────────────────┘
```

---

## 🎓 Resumen: La Cadena Completa

```
MUNDO → PICKUP → INVENTARIO → DETECCIÓN → USO

1. DISEÑO
   Designer crea GreenHerbItem.asset
   Tipo: ConsumableItemData (implementa IUsable)
   ↓
2. MUNDO
   Level Designer coloca en escena
   PickupItem.itemData = GreenHerbItem
   ↓
3. RECOGIDA
   Player presiona E
   PickupItem.Pickup() → inventory.TryAddItem(GreenHerbItem)
   ↓
4. ALMACENAMIENTO
   InventorySystem guarda referencia
   items[0].itemData = GreenHerbItem
   (Almacenado como ItemData, pero SIGUE SIENDO ConsumableItemData)
   ↓
5. DETECCIÓN DE TIPO
   InventoryUIController.OpenContextMenu()
   if (itemData is IUsable) → ✅ TRUE
   availableActions.Add(Use)
   ↓
6. EJECUCIÓN POLIMÓRFICA
   Usuario selecciona "Use"
   inventorySystem.UseCurrentItem()
   if (itemData is IUsable usable)
   usable.Use(player)  ← C# llama a ConsumableItemData.Use()
   ↓
7. EFECTO
   ConsumableItemData.Use()
   health.Heal(10f)
   ✅ Salud restaurada!
```

---

## 🔑 Conceptos Clave

### ✅ **Tipo Preservado**
Aunque guardas como `ItemData`, el objeto **recuerda** que es `ConsumableItemData`

### ✅ **Detección en Runtime**
El operador `is` chequea el **tipo real** del objeto, no el tipo declarado

### ✅ **Polimorfismo**
`usable.Use()` llama automáticamente a la implementación correcta

### ✅ **Extensible**
Añadir nuevos tipos es fácil: crea nueva clase + implementa interfaz

### ✅ **Sin Switch**
No necesitas `if (type == "consumable")`, usas el sistema de tipos de C#

---

## 💡 Ventaja de Este Diseño

**SIN interfaces (malo):**
```csharp
// Tendrías que hacer esto en cada lugar:
if (item.itemType == ItemType.Consumable)
{
    ConsumableItemData consumable = (ConsumableItemData)item;
    health.Heal(consumable.healAmount);
    // Lógica repetida en múltiples lugares
}
```

**CON interfaces (bueno):**
```csharp
// En un solo lugar:
if (item is IUsable usable)
{
    usable.Use(gameObject);
    // La lógica está encapsulada en el ItemData
}
```

**Resultado:**
✅ Código más limpio
✅ Lógica centralizada
✅ Fácil de mantener
✅ Fácil de extender

---

**La planta sabe que es consumible porque ES ConsumableItemData, que implementa IUsable.** 🌿✨
