# 🎒 Inventory System - The Hunt

Sistema de inventario modular estilo Silent Hill con carrusel de 6 slots.

## 📁 Estructura del Sistema

```
/Inventory
├── /Core
│   ├── InventorySystem.cs       ⭐ Sistema principal (añadir al Player)
│   └── ItemInstance.cs          Runtime wrapper para items
│
├── /UI
│   └── InventoryUIController.cs ⭐ Control de estados y menú contextual
│
├── /Data
│   ├── ItemData.cs              Base abstracta para todos los items
│   ├── ConsumableItemData.cs    Pociones, vendas (curan vida)
│   ├── WeaponItemData.cs        Armas equipables
│   ├── AmmoItemData.cs          Munición (no ocupa slot)
│   └── KeyItemData.cs           Llaves, documentos
│
├── /Interfaces
│   ├── IUsable.cs               Items que se pueden usar
│   ├── IExaminable.cs           Items examinables
│   └── IEquippable.cs           Items equipables
│
├── /Enums
│   ├── ItemType.cs              Tipo de item
│   ├── AmmoType.cs              Tipo de munición
│   ├── EquipSlot.cs             Slot de equipamiento (Primary/Secondary)
│   ├── WeaponType.cs            Tipo de arma (Melee/Ranged)
│   ├── InventoryState.cs        Estado UI (Closed/Open/ContextMenu)
│   └── ItemContextAction.cs     Acciones del menú contextual
│
└── PickupItem.cs                Componente para objetos recogibles
```

## 🚀 Configuración Rápida

### 1. Añadir al Player

1. Selecciona el GameObject `Player`
2. Añade componente `InventorySystem`
3. Añade componente `InventoryUIController` ⭐ NUEVO
4. (Opcional) Añade `InventoryDebugger` para testing
5. Ya está configurado ✅

### 2. Crear Items (ScriptableObjects)

#### Crear Consumible (Poción de Vida)
```
1. Click derecho en Project → Create → Inventory → Consumable Item
2. Nombre: "HealthPotion"
3. Configurar:
   - Item Name: "Health Potion"
   - Description: "Restores 50 HP"
   - Item Type: Consumable
   - Stackable: ✓
   - Heal Amount: 50
   - Remove On Use: ✓
```

#### Crear Arma (Pistola)
```
1. Click derecho → Create → Inventory → Weapon Item
2. Nombre: "Pistol"
3. Configurar:
   - Item Name: "9mm Pistol"
   - Description: "Standard handgun"
   - Item Type: Weapon
   - Stackable: ✗
   - Weapon Type: Ranged
   - Damage: 15
   - Required Ammo: Pistol_9mm
   - Magazine Size: 12
```

#### Crear Munición
```
1. Click derecho → Create → Inventory → Ammo Item
2. Nombre: "PistolAmmo"
3. Configurar:
   - Item Name: "9mm Ammo"
   - Item Type: Ammo
   - Ammo Type: Pistol_9mm
   - Ammo Amount: 12
```

#### Crear Key Item
```
1. Click derecho → Create → Inventory → Key Item
2. Nombre: "MasterKey"
3. Configurar:
   - Item Name: "Master Key"
   - Item Type: KeyItem
   - Stackable: ✗
   - Can Be Examined: ✓
```

### 3. Crear Objeto Recogible en Escena

1. Crea un GameObject (ej: `HealthPotion_Pickup`)
2. Añade `BoxCollider2D` o `CircleCollider2D`
3. Marca como `Is Trigger: ✓`
4. Asigna Layer: `Interactable`
5. Añade componente `PickupItem`
6. Asigna el ItemData correspondiente
7. Configura Interaction Prompt: "Pick up"

## 🎮 Controles (Input Actions)

**4 inputs simplificados con menú contextual:**

| Input | Tecla | Función |
|-------|-------|---------|
| **Toggle** | Tab / I | Abrir/Cerrar inventario |
| **Navigate** | ← → ↑ ↓ | Navegar items y menús |
| **Interact** | E / Enter | Abrir menú / Confirmar acción |
| **Cancel** | Esc | Cerrar menú / Cerrar inventario |

### Menú Contextual Dinámico

Al presionar **E** sobre un item se abre un menú con opciones:

**Consumible:** Use, Examine, Drop  
**Arma:** Equip Primary, Equip Secondary, Examine, Drop  
**Key Item:** Examine, Drop

### Configuración en Input Actions

Añade estas acciones al `Player.inputactions`:

```
InventoryToggle:   Tab, I (Button)
InventoryNavigate: ← → ↑ ↓ (Axis)
InventoryInteract: E, Enter (Button)
InventoryCancel:   Esc (Button)
```

**Ver `INPUT_SETUP.md` para detalles completos**

## 📊 Especificaciones

- **Slots totales:** 6
- **Stack máximo:** 6 unidades por item
- **Slots de equipamiento:** 2 (Primary/Secondary)
- **Munición:** Sistema separado (no ocupa slots)

## 🔄 Flujos de Uso

### Recoger Item
```
Player presiona E cerca de objeto
  ↓
PickupItem.Interact()
  ↓
InventorySystem.TryAddItem(itemData)
  ↓
Item añadido o mensaje "Inventory Full"
```

### Usar Poción
```
Player navega a poción en carrusel
  ↓
Player presiona E (Use)
  ↓
InventorySystem.UseCurrentItem()
  ↓
HealthController.Heal(50)
  ↓
Item removido si RemoveOnUse = true
```

### Equipar Arma
```
Player navega a arma en carrusel
  ↓
Player presiona 1 (Equip Primary)
  ↓
InventorySystem.EquipWeapon(weapon, Primary)
  ↓
Arma equipada (permanece en inventario)
```

## 📡 Eventos Disponibles

```csharp
OnItemAdded(int slot, ItemInstance item)
OnItemRemoved(int slot, ItemInstance item)
OnItemUsed(ItemInstance item)
OnSelectionChanged(int oldIndex, int newIndex)
OnInventoryFull()
OnWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
OnWeaponUnequipped(EquipSlot slot)
OnAmmoChanged(AmmoType type, int count)
```

Suscríbete a estos eventos para actualizar UI:

```csharp
void Start()
{
    inventorySystem.OnItemAdded += OnItemAdded;
    inventorySystem.OnSelectionChanged += OnSelectionChanged;
}

void OnItemAdded(int slot, ItemInstance item)
{
    Debug.Log($"Item añadido: {item.DisplayName} en slot {slot}");
    // Actualizar UI aquí
}
```

## ✅ Próximos Pasos

1. ✅ Sistema base implementado
2. ⏳ Añadir input actions al Input System
3. ⏳ Crear UI del carrusel
4. ⏳ Crear UI de detalle de items
5. ⏳ Integrar con sistema de combate

## 🐛 Testing

Para probar sin UI:

```csharp
// Añadir item programáticamente
InventorySystem inv = GetComponent<InventorySystem>();
inv.TryAddItem(healthPotionData);

// Navegar
inv.SelectNext();
inv.SelectPrevious();

// Usar
inv.UseCurrentItem();

// Equipar
inv.EquipWeapon(pistolData, EquipSlot.Primary);

// Munición
inv.AddAmmo(AmmoType.Pistol_9mm, 24);
Debug.Log($"Ammo: {inv.GetAmmoCount(AmmoType.Pistol_9mm)}");
```

## 📝 Notas

- Los items de munición **NO ocupan slots** del inventario
- Los items equipados **permanecen visibles** en el carrusel
- Los items stackeables se apilan automáticamente
- El inventario lleno dispara evento `OnInventoryFull`
