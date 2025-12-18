# 🔑🚪 Escenario Completo: Llave → Inventario → Puerta

Este documento explica **paso a paso** cómo funciona el escenario de recoger una llave del suelo, añadirla al inventario, y usarla para abrir una puerta.

---

## 📋 Índice

1. [Resumen del Flujo Completo](#resumen-del-flujo-completo)
2. [Parte 1: Llave en el Suelo](#parte-1-llave-en-el-suelo)
3. [Parte 2: Recoger la Llave](#parte-2-recoger-la-llave)
4. [Parte 3: Llave en el Inventario](#parte-3-llave-en-el-inventario)
5. [Parte 4: Llegar a la Puerta](#parte-4-llegar-a-la-puerta)
6. [Parte 5: Verificar y Usar la Llave](#parte-5-verificar-y-usar-la-llave)
7. [Parte 6: Abrir la Puerta](#parte-6-abrir-la-puerta)
8. [Implementación Completa](#implementación-completa)
9. [Setup en Unity](#setup-en-unity)

---

## 🎯 Resumen del Flujo Completo

```
┌─────────────────┐
│ 1. LLAVE EN     │  GameObject con:
│    EL SUELO     │  - Sprite de llave 🔑
└────────┬────────┘  - PickupInteractable
         │           - Collider2D (trigger)
         │           - KeyItemData asset
         ↓
┌─────────────────┐
│ 2. JUGADOR      │  Jugador se acerca
│    SE ACERCA    │  PlayerInteractionController detecta
└────────┬────────┘  "Press E to pick up Rusty Key"
         │
         ↓
┌─────────────────┐
│ 3. PRESIONA E   │  PlayerInputHandler → OnInteract()
│    PARA RECOGER │  PlayerInteractionController.TryInteract()
└────────┬────────┘  PickupInteractable.Interact()
         │
         ↓
┌─────────────────┐
│ 4. AÑADIR AL    │  InventorySystem.TryAddItem(keyData)
│    INVENTARIO   │  items[slot] = new ItemInstance(keyData)
└────────┬────────┘  OnItemAdded evento
         │           InventorySlotUI.UpdateSlot()
         ↓
┌─────────────────┐
│ 5. LLAVE        │  Slot muestra:
│    EN SLOT      │  - Icono de llave 🔑
└────────┬────────┘  - "Rusty Key"
         │           GameObject llave destruido
         ↓
┌─────────────────┐
│ 6. JUGADOR      │  Jugador camina por el mundo
│    EXPLORA      │  Llave guardada en inventario
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│ 7. LLEGA A      │  GameObject con:
│    LA PUERTA    │  - Sprite de puerta 🚪
└────────┬────────┘  - LockedDoorInteractable
         │           - requiredKeyID: "rusty_key"
         │
         ↓
┌─────────────────┐
│ 8. SE ACERCA    │  PlayerInteractionController detecta
│    A LA PUERTA  │  "Locked. Requires: Rusty Key"
└────────┬────────┘  (sin llave) ó
         │           "Press E to unlock" (con llave)
         ↓
┌─────────────────┐
│ 9. PRESIONA E   │  LockedDoorInteractable.Interact()
│    EN PUERTA    │  CanInteract() → busca llave en inventario
└────────┬────────┘  inventorySystem.HasKeyItem("rusty_key")
         │
         ↓
┌─────────────────┐
│ 10. VERIFICAR   │  if (HasKeyItem):
│     LLAVE       │    ✅ Tiene llave → continuar
└────────┬────────┘  else:
         │             ❌ No tiene → mensaje error
         ↓
┌─────────────────┐
│ 11. USAR LLAVE  │  ConsumeKeyItem("rusty_key")
│     (OPCIONAL)  │  Elimina llave del inventario
└────────┬────────┘  OnItemRemoved evento
         │
         ↓
┌─────────────────┐
│ 12. ABRIR       │  isLocked = false
│     PUERTA      │  Animación de apertura
└────────┬────────┘  Sonido de desbloqueo
         │           Puerta abierta ✅
         ↓
┌─────────────────┐
│ 13. JUGADOR     │  Collider desactivado
│     PASA        │  o trigger abierto
└─────────────────┘
```

---

## 🗝️ Parte 1: Llave en el Suelo

### GameObject en la Escena

```
Hierarchy:
  Scene
    ├─ Player
    ├─ Environment
    │    └─ RustyKey ← Este GameObject
    └─ Doors
```

### Componentes del GameObject `RustyKey`

```
RustyKey (GameObject)
  Components:
    1. Transform
       - Position: (10, 0, 0)
       - Rotation: (0, 0, 0)
       - Scale: (1, 1, 1)
    
    2. Sprite Renderer
       - Sprite: key_sprite 🔑
       - Sorting Layer: Items
       - Order in Layer: 5
    
    3. Collider2D (CircleCollider2D or BoxCollider2D)
       - Is Trigger: ✅ true
       - Radius/Size: (0.5, 0.5)
    
    4. PickupInteractable (Script)
       - Item Data: RustyKeyData (KeyItemData asset)
       - Item Name: "Rusty Key"
       - Destroy On Pickup: ✅ true
       - Pickup VFX: (opcional) sparkle_effect
       - Pickup Sound: (opcional) pickup_sound
    
    Layer: Interactable
```

### KeyItemData Asset

```
Asset: /Assets/Assets/Data/Items/RustyKeyData.asset

Configuración:
  Item Name: "Rusty Key"
  Item Description: "A rusty key found in the basement"
  Item Icon: key_icon_sprite 🔑
  Item Type: KeyItem
  Is Stackable: false
  Max Stack Size: 1
  
  Key Item Settings:
    Unlocks: ["rusty_door", "basement_door"]
    Is Quest Item: false
    Can Be Discarded: true
```

---

## 👟 Parte 2: Recoger la Llave

### Step 1: Jugador se Acerca

```
Player GameObject
  Position: (8, 0, 0)
  ↓
PlayerInteractionController.Update()
  ↓
DetectNearbyInteractables()
  ↓
Physics2D.OverlapCircle(playerPos, detectionRadius=2f, interactionLayer)
  ↓
Detecta: RustyKey a distancia 2.0
  ↓
closestInteractable = RustyKey.GetComponent<IInteractable>()
  ↓
SetInteractable(closestInteractable)
  ↓
OnInteractableDetected?.Invoke(rustyKeyInteractable)
  ↓
Console: "[INTERACTION] Detected: Press E to pick up Rusty Key"
```

### Step 2: UI Muestra Prompt

```
InteractionPromptUI (si existe)
  ↓
OnInteractableDetected(interactable)
  ↓
promptText.text = "Press E to pick up Rusty Key"
  ↓
promptPanel.SetActive(true)
  ↓
Jugador ve: [E] Press E to pick up Rusty Key
```

### Step 3: Jugador Presiona E

```
Input: Keyboard E
  ↓
PlayerInputHandler.OnInteract()
  ↓
interactionController.TryInteract()
  ↓
if (!CanInteract) → return ❌
if (currentInteractable == null) → return ❌
  ↓
CanInteract == true ✅
  ↓
currentInteractable.Interact(playerGameObject)
  ↓
PickupInteractable.Interact(player)
  ↓
OnInteract(player)
```

### Step 4: PickupInteractable.OnInteract()

```csharp
protected override void OnInteract(GameObject interactor)
{
    // 1. Intentar añadir al inventario
    bool addedToInventory = AddToInventory(interactor);
    
    if (addedToInventory)
    {
        // 2. Feedback visual/audio
        PlayFeedback();
        
        // 3. Destruir el GameObject
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
```

### Step 5: AddToInventory()

```csharp
bool AddToInventory(GameObject interactor)
{
    // Obtener InventorySystem del jugador
    InventorySystem inventory = interactor.GetComponent<InventorySystem>();
    
    if (inventory == null)
    {
        Debug.LogError("[PICKUP] Player has no InventorySystem!");
        return false;
    }
    
    // Intentar añadir el item
    bool added = inventory.TryAddItem(itemData);
    
    if (added)
    {
        Debug.Log($"<color=green>[PICKUP] {interactor.name} picked up {itemName}</color>");
    }
    else
    {
        Debug.Log($"<color=yellow>[PICKUP] Inventory is full!</color>");
    }
    
    return added;
}
```

---

## 🎒 Parte 3: Llave en el Inventario

### Step 1: InventorySystem.TryAddItem()

```csharp
public bool TryAddItem(ItemData itemData)
{
    // itemData = RustyKeyData (KeyItemData)
    
    // 1. Validar
    if (itemData == null)
    {
        Debug.LogWarning("[INVENTORY] Cannot add null item");
        return false;
    }
    
    // 2. ¿Es munición? (No, es KeyItem)
    if (itemData is AmmoItemData) → Skip
    
    // 3. ¿Es stackable? (No, KeyItems no son stackable)
    if (itemData.IsStackable) → Skip
    
    // 4. Buscar slot vacío
    int emptySlot = FindEmptySlot();
    
    if (emptySlot == -1)
    {
        OnInventoryFull?.Invoke();
        Debug.Log("<color=yellow>[INVENTORY] Inventory is full!</color>");
        return false;
    }
    
    // 5. Crear nueva instancia
    items[emptySlot] = new ItemInstance(itemData, 1);
    
    // 6. Disparar evento
    OnItemAdded?.Invoke(emptySlot, items[emptySlot]);
    
    Debug.Log($"<color=green>[INVENTORY] Added {itemData.ItemName} to slot {emptySlot}</color>");
    
    return true;
}
```

### Step 2: Evento OnItemAdded

```
OnItemAdded(slotIndex: 2, item: ItemInstance(RustyKeyData, quantity: 1))
  ↓
  ┌─────────────────────────────────┐
  │ InventoryPanelUI escucha evento │
  └────────────┬────────────────────┘
               ↓
  OnItemAdded(2, item)
    ↓
  slotUIList[2].UpdateSlot(item)
```

### Step 3: InventorySlotUI.UpdateSlot()

```csharp
public void UpdateSlot(ItemInstance item)
{
    if (item == null)
    {
        ClearSlot();
        return;
    }
    
    // item.itemData = RustyKeyData
    // item.quantity = 1
    
    // Actualizar icono
    if (iconImage != null)
    {
        iconImage.sprite = item.itemData.ItemIcon;  // 🔑
        iconImage.color = fullIconColor;
        iconImage.enabled = true;
    }
    
    // Actualizar cantidad (KeyItems no muestran cantidad)
    if (quantityText != null)
    {
        if (item.quantity > 1)
        {
            quantityText.text = $"x{item.quantity}";
            quantityText.enabled = true;
        }
        else
        {
            quantityText.enabled = false;  ← Oculto
        }
    }
}
```

### Step 4: Resultado Visual

```
Inventory UI:

Slot 0:         Slot 1:         Slot 2:         Slot 3:
┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐
│         │    │         │    │   🔑    │    │         │
│         │    │         │    │         │    │         │
└─────────┘    └─────────┘    └─────────┘    └─────────┘
  (vacío)        (vacío)     Rusty Key       (vacío)
```

### Step 5: GameObject Llave Destruido

```
Hierarchy ANTES:
  Scene
    ├─ Player
    ├─ Environment
    │    └─ RustyKey ← Existe
    └─ Doors

Hierarchy DESPUÉS:
  Scene
    ├─ Player
    ├─ Environment  ← RustyKey destruido ✅
    └─ Doors
```

---

## 🚪 Parte 4: Llegar a la Puerta

### GameObject en la Escena

```
Hierarchy:
  Scene
    └─ Doors
         └─ RustyDoor ← Este GameObject
```

### Componentes del GameObject `RustyDoor`

```
RustyDoor (GameObject)
  Components:
    1. Transform
       - Position: (20, 0, 0)
    
    2. Sprite Renderer
       - Sprite: door_locked_sprite 🚪
       - Sorting Layer: Environment
    
    3. Collider2D (BoxCollider2D)
       - Is Trigger: ✅ true
       - Size: (1, 2)
    
    4. LockedDoorInteractable (Script)
       - Required Key ID: "rusty_key"
       - Door Name: "Basement Door"
       - Consume Key On Unlock: true (opcional)
       - Door Animator: (opcional) animator
       - Unlock Sound: unlock_sound
    
    5. Animator (opcional)
       - Controller: DoorAnimatorController
       - Animations: door_locked, door_opening, door_open
    
    Layer: Interactable
```

### Step 1: Jugador se Acerca

```
Player Position: (18, 0, 0)
  ↓
PlayerInteractionController.Update()
  ↓
DetectNearbyInteractables()
  ↓
Physics2D.OverlapCircle(playerPos, radius: 2f)
  ↓
Detecta: RustyDoor a distancia 1.5
  ↓
closestInteractable = RustyDoor.GetComponent<IInteractable>()
  ↓
SetInteractable(rustyDoorInteractable)
  ↓
OnInteractableDetected?.Invoke(rustyDoorInteractable)
```

---

## 🔍 Parte 5: Verificar y Usar la Llave

### Step 1: LockedDoorInteractable.CanInteract()

```csharp
public bool CanInteract(GameObject interactor)
{
    // Si ya está desbloqueada, siempre puede interactuar
    if (!isLocked)
    {
        return true;
    }
    
    // Si está bloqueada, verificar si tiene la llave
    InventorySystem inventory = interactor.GetComponent<InventorySystem>();
    
    if (inventory == null)
    {
        return false;
    }
    
    // Buscar la llave en el inventario
    bool hasKey = inventory.HasKeyItem(requiredKeyID);
    
    return hasKey;
}
```

### Step 2: InventorySystem.HasKeyItem()

```csharp
public bool HasKeyItem(string keyID)
{
    for (int i = 0; i < MAX_SLOTS; i++)
    {
        if (items[i] != null)
        {
            // ¿Es KeyItemData?
            if (items[i].itemData is KeyItemData keyData)
            {
                // ¿Desbloquea esta puerta?
                if (keyData.Unlocks != null)
                {
                    foreach (string unlockID in keyData.Unlocks)
                    {
                        if (unlockID == keyID)
                        {
                            return true;  // ✅ Tiene la llave!
                        }
                    }
                }
            }
        }
    }
    
    return false;  // ❌ No tiene la llave
}
```

### Step 3: InteractionPrompt Dinámico

```csharp
public string InteractionPrompt
{
    get
    {
        if (!isLocked)
        {
            return "Press E to open";
        }
        
        // Verificar si el jugador tiene la llave
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null && CanInteract(player))
        {
            return "Press E to unlock";  ← Con llave ✅
        }
        else
        {
            return $"Locked. Requires: {doorName}";  ← Sin llave ❌
        }
    }
}
```

### Ejemplos de Prompts

**Sin llave en inventario:**
```
UI muestra: "Locked. Requires: Rusty Key" ❌
CanInteract() == false
Presionar E → No hace nada
```

**Con llave en inventario:**
```
UI muestra: "Press E to unlock" ✅
CanInteract() == true
Presionar E → Desbloquea puerta
```

---

## 🔓 Parte 6: Abrir la Puerta

### Step 1: Jugador Presiona E

```
Input: Keyboard E
  ↓
PlayerInputHandler.OnInteract()
  ↓
interactionController.TryInteract()
  ↓
if (!CanInteract) → return ❌
  ↓
CanInteract == true ✅
  ↓
currentInteractable.Interact(playerGameObject)
  ↓
LockedDoorInteractable.Interact(player)
```

### Step 2: LockedDoorInteractable.OnInteract()

```csharp
protected override void OnInteract(GameObject interactor)
{
    if (!isLocked)
    {
        // Puerta ya desbloqueada, solo abrir
        OpenDoor();
        return;
    }
    
    // Verificar llave
    InventorySystem inventory = interactor.GetComponent<InventorySystem>();
    
    if (inventory == null || !inventory.HasKeyItem(requiredKeyID))
    {
        PlayLockedSound();
        Debug.Log("<color=yellow>[DOOR] Door is locked!</color>");
        return;
    }
    
    // Desbloquear
    UnlockDoor();
    
    // Consumir llave (opcional)
    if (consumeKeyOnUnlock)
    {
        inventory.ConsumeKeyItem(requiredKeyID);
    }
    
    // Abrir puerta
    OpenDoor();
}
```

### Step 3: UnlockDoor()

```csharp
void UnlockDoor()
{
    isLocked = false;
    
    // Cambiar sprite
    if (spriteRenderer != null && unlockedSprite != null)
    {
        spriteRenderer.sprite = unlockedSprite;
    }
    
    // Sonido de desbloqueo
    if (unlockSound != null)
    {
        AudioSource.PlayClipAtPoint(unlockSound, transform.position);
    }
    
    // Partículas
    if (unlockVFX != null)
    {
        Instantiate(unlockVFX, transform.position, Quaternion.identity);
    }
    
    Debug.Log($"<color=green>[DOOR] {doorName} unlocked!</color>");
}
```

### Step 4: OpenDoor()

```csharp
void OpenDoor()
{
    // Animación
    if (doorAnimator != null)
    {
        doorAnimator.SetTrigger("Open");
    }
    
    // Desactivar collider (jugador puede pasar)
    if (doorCollider != null)
    {
        doorCollider.enabled = false;
    }
    
    // Sonido de apertura
    if (openSound != null)
    {
        AudioSource.PlayClipAtPoint(openSound, transform.position);
    }
    
    Debug.Log($"<color=cyan>[DOOR] {doorName} opened!</color>");
    
    // Desactivar interacción
    SetInteractable(false);
}
```

### Step 5: ConsumeKeyItem() (Opcional)

```csharp
public bool ConsumeKeyItem(string keyID)
{
    for (int i = 0; i < MAX_SLOTS; i++)
    {
        if (items[i] != null && items[i].itemData is KeyItemData keyData)
        {
            if (keyData.Unlocks != null)
            {
                foreach (string unlockID in keyData.Unlocks)
                {
                    if (unlockID == keyID)
                    {
                        // Eliminar del inventario
                        ItemInstance removedItem = items[i];
                        items[i] = null;
                        
                        OnItemRemoved?.Invoke(i, removedItem);
                        
                        Debug.Log($"<color=cyan>[INVENTORY] Consumed {keyData.ItemName}</color>");
                        
                        return true;
                    }
                }
            }
        }
    }
    
    return false;
}
```

---

## 🎬 Flujo Completo en Código

### Diagrama de Secuencia

```
Player                 Input           Interaction        Pickup           Inventory
  │                      │                  │                 │                 │
  │  Se acerca a llave   │                  │                 │                 │
  │─────────────────────>│                  │                 │                 │
  │                      │  Update()        │                 │                 │
  │                      │─────────────────>│                 │                 │
  │                      │  DetectNearby()  │                 │                 │
  │                      │  SetInteractable()│                │                 │
  │                      │<─────────────────│                 │                 │
  │  UI: "Press E"       │                  │                 │                 │
  │<─────────────────────│                  │                 │                 │
  │                      │                  │                 │                 │
  │  Presiona E          │                  │                 │                 │
  │─────────────────────>│                  │                 │                 │
  │                      │  OnInteract()    │                 │                 │
  │                      │─────────────────>│                 │                 │
  │                      │  TryInteract()   │                 │                 │
  │                      │  Interact(player)│                 │                 │
  │                      │─────────────────────────────────>│                 │
  │                      │                  │  OnInteract()   │                 │
  │                      │                  │  AddToInventory()│                │
  │                      │                  │─────────────────────────────────>│
  │                      │                  │  TryAddItem(keyData)              │
  │                      │                  │  items[2] = new ItemInstance()    │
  │                      │                  │  OnItemAdded.Invoke(2, item)      │
  │                      │                  │<─────────────────────────────────│
  │                      │                  │  PlayFeedback()  │                 │
  │                      │                  │  Destroy(this)   │                 │
  │                      │                  │     ❌            │                 │
  │                      │                  │                 │                 │
  │  Llave en slot 2     │                  │                 │                 │
  │<─────────────────────────────────────────────────────────────────────────────│
  │  🔑 Rusty Key        │                  │                 │                 │
  │                      │                  │                 │                 │
  │  Camina a puerta     │                  │                 │                 │
  │─────────────────────>│                  │                 │                 │
  │                      │  Update()        │                 │                 │
  │                      │─────────────────>│                 │                 │
  │                      │  DetectNearby()  │                 │                 │
  │                      │  SetInteractable()│                │                 │
  │                      │  (RustyDoor)     │                 │                 │
  │                      │<─────────────────│                 │                 │
  │  UI: "Press E to     │                  │                 │                 │
  │       unlock"        │                  │                 │                 │
  │<─────────────────────│                  │                 │                 │
  │                      │                  │                 │                 │
  │  Presiona E          │                  │                 │                 │
  │─────────────────────>│                  │                 │                 │
  │                      │  OnInteract()    │                 │                 │
  │                      │─────────────────>│                 │                 │
  │                      │  TryInteract()   │                 │                 │
  │                      │  Interact(player)│                 │                 │
  │                      │────────────────────────────────────────────┐         │
  │                      │                  │  LockedDoor.OnInteract()│         │
  │                      │                  │  HasKeyItem("rusty_key")?         │
  │                      │                  │<────────────────────────┘         │
  │                      │                  │─────────────────────────────────>│
  │                      │                  │  HasKeyItem("rusty_key")          │
  │                      │                  │  → return true ✅                 │
  │                      │                  │<─────────────────────────────────│
  │                      │                  │  UnlockDoor()   │                 │
  │                      │                  │  OpenDoor()     │                 │
  │                      │                  │  🚪 Abierta ✅   │                 │
  │                      │                  │                 │                 │
  │  Puerta abierta      │                  │                 │                 │
  │<─────────────────────────────────────────                 │                 │
  │                      │                  │                 │                 │
  │  Jugador pasa        │                  │                 │                 │
  │─────────────────────>│                  │                 │                 │
```

---

## ✅ Implementación Completa

### Scripts Necesarios (Ya Existen)

1. **PickupInteractable.cs** ✅
   - Ubicación: `/Assets/Scripts/Interaction/PickupInteractable.cs`
   - Funciona: Sí, pero necesita actualización para conectar con inventario

2. **KeyItemData.cs** ✅
   - Ubicación: `/Assets/Scripts/Inventory/Data/KeyItemData.cs`
   - Funciona: Sí, ya tiene campo `Unlocks`

3. **InventorySystem.cs** ✅
   - Ubicación: `/Assets/Scripts/Inventory/Core/InventorySystem.cs`
   - Funciona: Sí, pero necesita métodos `HasKeyItem` y `ConsumeKeyItem`

4. **PlayerInteractionController.cs** ✅
   - Ubicación: `/Assets/Scripts/Interaction/PlayerInteractionController.cs`
   - Funciona: Sí, sistema completo

### Scripts Nuevos (Necesitan Crearse)

1. **LockedDoorInteractable.cs** ❌
   - Ubicación: `/Assets/Scripts/Interaction/LockedDoorInteractable.cs`
   - Responsabilidad: Puerta que requiere llave

---

## 🎨 Setup en Unity

### 1. Crear KeyItemData Asset

```
1. Project → Assets/Assets/Data/Items
2. Click derecho → Create → Inventory → Key Item
3. Nombre: "RustyKeyData"
4. Inspector:
   - Item Name: "Rusty Key"
   - Item Description: "An old rusty key. What does it unlock?"
   - Item Icon: (asignar sprite de llave)
   - Item Type: KeyItem
   - Is Stackable: false
   - Unlocks: Array Size = 1
     - Element 0: "rusty_key"
   - Is Quest Item: false
   - Can Be Discarded: true
```

### 2. Crear GameObject Llave

```
1. Hierarchy → Create Empty → Nombre: "RustyKey"
2. Add Component → Sprite Renderer
   - Sprite: key_sprite
   - Sorting Layer: Items
3. Add Component → Circle Collider 2D
   - Is Trigger: ✅ true
   - Radius: 0.5
4. Add Component → PickupInteractable
   - Item Data: RustyKeyData (asset)
   - Item Name: "Rusty Key"
   - Destroy On Pickup: ✅ true
5. Layer: Interactable
```

### 3. Crear GameObject Puerta

```
1. Hierarchy → Create Empty → Nombre: "RustyDoor"
2. Add Component → Sprite Renderer
   - Sprite: door_locked_sprite
   - Sorting Layer: Environment
3. Add Component → Box Collider 2D
   - Is Trigger: ✅ true
   - Size: (1, 2)
4. Add Component → LockedDoorInteractable
   - Required Key ID: "rusty_key"
   - Door Name: "Basement Door"
   - Consume Key On Unlock: ✅ true (opcional)
5. Layer: Interactable
```

### 4. Configurar Player

```
Player GameObject debe tener:
  - InventorySystem (script) ✅
  - PlayerInteractionController (script) ✅
  - Tag: "Player" ✅
  - Layer: Player ✅
```

### 5. Configurar Interaction Layer

```
1. Edit → Project Settings → Physics 2D
2. Layer Collision Matrix:
   - Player ✅ interactúa con Interactable ✅
3. PlayerInteractionController:
   - Detection Radius: 2
   - Interaction Layer: Interactable
```

---

## 🧪 Prueba el Flujo

### Checklist de Prueba

1. **Llave en suelo:**
   - [ ] Llave visible en escena
   - [ ] Al acercarse muestra "Press E to pick up Rusty Key"
   - [ ] Círculo de detección visible (Gizmos)

2. **Recoger llave:**
   - [ ] Presionar E recoge la llave
   - [ ] Llave desaparece del mundo
   - [ ] Llave aparece en inventario (slot con icono 🔑)
   - [ ] Console muestra "[INVENTORY] Added Rusty Key to slot X"

3. **Puerta bloqueada (sin llave):**
   - [ ] Al acercarse muestra "Locked. Requires: Basement Door"
   - [ ] Presionar E no hace nada
   - [ ] Console muestra "[DOOR] Door is locked!"

4. **Puerta bloqueada (con llave):**
   - [ ] Al acercarse muestra "Press E to unlock"
   - [ ] Presionar E desbloquea la puerta
   - [ ] Sonido de desbloqueo (si asignado)
   - [ ] Llave desaparece del inventario (si consume_key=true)
   - [ ] Console muestra "[DOOR] Basement Door unlocked!"

5. **Puerta abierta:**
   - [ ] Puerta se abre (animación o cambio de sprite)
   - [ ] Jugador puede pasar (collider desactivado)
   - [ ] Prompt desaparece o cambia a "Open"

---

## 🔗 Relación Entre Scripts

```
KeyItemData.asset
  ↓ (assigned to)
PickupInteractable
  ↓ (adds to)
InventorySystem
  ↓ (checked by)
LockedDoorInteractable
  ↓ (unlocks)
Door GameObject
```

---

¡Listo! Este documento explica TODO el flujo del escenario llave → inventario → puerta. 🔑🚪✨
