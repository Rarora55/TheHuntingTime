# Arquitectura del Sistema de Inventario - Visión General

## Estructura de Componentes

```
Player (GameObject)
│
├─ InventorySystem
│  ├─ Responsabilidad: Gestión de items generales en slots
│  ├─ API: TryAddItem, RemoveItem, UseCurrentItem, SelectSlot
│  └─ Eventos: OnItemAdded, OnItemRemoved, OnItemUsed, OnSelectionChanged
│
├─ AmmoInventoryManager
│  ├─ Responsabilidad: Gestión de munición
│  ├─ API: AddAmmo, RemoveAmmo, GetAmmoCount, HasAmmo
│  └─ Eventos: OnAmmoChanged, OnAmmoAdded, OnAmmoRemoved, OnAmmoEmpty
│
└─ WeaponInventoryManager
   ├─ Responsabilidad: Gestión de equipamiento de armas
   ├─ API: EquipWeapon, UnequipWeapon, SwapWeapons, GetEquippedWeapon
   └─ Eventos: OnWeaponEquipped, OnWeaponUnequipped, OnWeaponsSwapped

NOTA: KeyInventoryManager está OBSOLETO. 
      Usar InventorySystem.HasItem() y RemoveItem() directamente.
```

## Flujo de Datos

### 1. Añadir Item al Inventario

```
Pickup/Interactable
      ↓
InventorySystem.TryAddItem(itemData)
      ↓
  ¿Es AmmoItemData? ──YES──→ AmmoInventoryManager.AddAmmo()
      ↓ NO                          ↓
  ¿Es stackable?                OnAmmoAdded Event
      ↓                              
  Buscar slot vacío
      ↓
  Crear ItemInstance
      ↓
  OnItemAdded Event
```

### 2. Equipar Arma

```
InventoryUIController
      ↓
WeaponInventoryManager.EquipWeapon(weapon, slot)
      ↓
Verificar si está en inventario (usando InventorySystem.Items)
      ↓
¿Hay arma equipada? ──YES──→ UnequipWeapon()
      ↓ NO
Equipar nueva arma
      ↓
weapon.Equip(gameObject)
      ↓
OnWeaponEquipped Event
```

### 3. Consumir Munición

```
Weapon System
      ↓
AmmoInventoryManager.HasAmmo(type, required)
      ↓
¿Hay suficiente? ──YES──→ AmmoInventoryManager.RemoveAmmo(type, amount)
      ↓ NO                          ↓
Weapon Out of Ammo              OnAmmoChanged Event
                                    ↓
                                UI Update
```

### 4. Usar Llave

```
LockedDoorInteractable
      ↓
KeyInventoryManager.HasKeyItem(keyID)
      ↓
¿Tiene llave? ──YES──→ KeyInventoryManager.ConsumeKeyItem(keyID)
      ↓ NO                          ↓
Play Locked Sound               OnKeyConsumed Event
                                    ↓
                                Door Unlocks
```

## Separación de Responsabilidades

| Sistema | Responsabilidad | Tamaño (líneas) |
|---------|----------------|-----------------|
| **InventorySystem** | Items generales, slots, selección | ~200 |
| **AmmoInventoryManager** | Munición por tipo, cantidades | ~100 |
| **WeaponInventoryManager** | Equipar/desequipar armas | ~120 |
| ~~**KeyInventoryManager**~~ | ~~Llaves y objetos clave~~ | ~~OBSOLETO~~ |

**Antes**: Un solo `InventorySystem` con ~500 líneas mezclando todo

**Ahora**: 3 managers especializados, cada uno con responsabilidad única
(KeyInventoryManager eliminado - funcionalidad integrada en InventorySystem)

## Ventajas del Diseño Actual

### 1. Single Responsibility Principle ✅
Cada manager tiene una sola razón para cambiar. Si necesitas modificar cómo funcionan las armas, solo tocas `WeaponInventoryManager`.

### 2. Composición sobre Herencia ✅
Los managers se componen en el GameObject del jugador sin heredar de una clase base monolítica.

### 3. Eventos Específicos por Dominio ✅
```csharp
// Solo te suscribes a lo que necesitas
ammoDisplay.Subscribe(ammoManager.OnAmmoChanged);
weaponUI.Subscribe(weaponManager.OnWeaponEquipped);
```

### 4. Testing Independiente ✅
Puedes testear munición sin inicializar todo el sistema de inventario.

### 5. Extensibilidad ✅
Añadir nuevas características es más fácil:
- ¿Sistema de crafting? → Nuevo `CraftingInventoryManager`
- ¿Items consumibles especiales? → Nuevo `ConsumableInventoryManager`
- ¿Sistema de Quest Items? → Nuevo `QuestItemManager`

## Dependencias entre Managers

```
       InventorySystem (Core)
              ↑
              │ Usa Items array
              │
    ┌─────────┼─────────┬─────────┐
    │         │         │         │
AmmoMgr   WeaponMgr  KeyMgr   (Futuros...)
    │         │         │
    │         │         └──→ Lee Items[] para verificar llaves
    │         └──────────────→ Lee Items[] para verificar armas en inventario
    └────────────────────────→ Recibe notificación cuando se añade AmmoItemData
```

**Nota**: Los managers NO se comunican entre sí directamente. Todos dependen de `InventorySystem.Items[]` como fuente de verdad para items en slots.

## Integración con UI

```
InventoryUIController
      │
      ├──→ InventorySystem (navegación, selección)
      ├──→ WeaponInventoryManager (equipar desde menú contextual)
      └──→ (Potencialmente otros managers en el futuro)

InventoryDebugger
      │
      ├──→ InventorySystem (debug de slots)
      ├──→ AmmoInventoryManager (debug de munición)
      └──→ WeaponInventoryManager (debug de armas equipadas)
```

## Buenas Prácticas

### ✅ DO
- Usa el manager apropiado para cada operación
- Suscríbete solo a los eventos que necesitas
- Verifica que el manager exista antes de usarlo (null checks)
- Usa `GetComponent<>()` en `Awake()` para obtener referencias

### ❌ DON'T
- No accedas directamente a campos privados de managers desde fuera
- No dupliques lógica que ya existe en un manager
- No intentes sincronizar estado manualmente entre managers
- No uses `InventorySystem` para operaciones específicas de armas/munición/llaves

## Ejemplo de Uso Correcto

```csharp
public class PlayerCombat : MonoBehaviour
{
    private AmmoInventoryManager ammoManager;
    private WeaponInventoryManager weaponManager;

    void Awake()
    {
        ammoManager = GetComponent<AmmoInventoryManager>();
        weaponManager = GetComponent<WeaponInventoryManager>();
    }

    void Start()
    {
        weaponManager.OnWeaponEquipped += OnWeaponEquipped;
        ammoManager.OnAmmoChanged += OnAmmoChanged;
    }

    void Shoot()
    {
        WeaponItemData currentWeapon = weaponManager.PrimaryWeapon;
        if (currentWeapon == null) return;

        AmmoType ammoType = currentWeapon.AmmoType;
        int required = currentWeapon.AmmoPerShot;

        if (!ammoManager.HasAmmo(ammoType, required))
        {
            Debug.Log("Out of ammo!");
            return;
        }

        ammoManager.RemoveAmmo(ammoType, required);
        // Shoot logic...
    }

    void OnWeaponEquipped(EquipSlot slot, WeaponItemData weapon)
    {
        Debug.Log($"Equipped {weapon.ItemName} in {slot}");
    }

    void OnAmmoChanged(AmmoType type, int count)
    {
        Debug.Log($"{type}: {count}");
    }
}
```

## Migración desde Versión Anterior

Si tienes código que usa la versión antigua de `InventorySystem`, consulta:
- `/Assets/Scripts/Inventory/Info/REFACTORING_SEPARATION_OF_CONCERNS.md` (Llaves)
- `/Assets/Scripts/Inventory/Info/REFACTORING_AMMO_WEAPONS.md` (Munición y Armas)
