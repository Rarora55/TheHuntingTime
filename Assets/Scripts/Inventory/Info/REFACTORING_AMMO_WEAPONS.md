# Refactorización: Separación de Munición y Armas del InventorySystem

## Motivación

El `InventorySystem` original contenía múltiples responsabilidades mezcladas:
- Gestión de items generales (slots, añadir/remover)
- Gestión de munición (diccionario de ammo, añadir/remover/consultar)
- Gestión de armas (equipar/desequipar/intercambiar en slots primario/secundario)
- Gestión de llaves (previamente refactorizado a `KeyInventoryManager`)

Esta violación del **Single Responsibility Principle** causaba:
- Código difícil de mantener (~364 líneas en un solo archivo)
- Acoplamiento alto entre sistemas diferentes
- Dificultad para extender funcionalidad específica de armas o munición
- Eventos mezclados de diferentes dominios

## Solución

Se implementó el patrón de **Separación de Responsabilidades** delegando la lógica específica a managers dedicados:

### AmmoInventoryManager
Gestiona exclusivamente la munición del jugador:
- `Dictionary<AmmoType, int>` para almacenar cantidades por tipo
- Métodos: `AddAmmo()`, `RemoveAmmo()`, `GetAmmoCount()`, `HasAmmo()`, `SetAmmo()`, `GetAllAmmo()`
- Eventos: `OnAmmoChanged`, `OnAmmoAdded`, `OnAmmoRemoved`, `OnAmmoEmpty`

### WeaponInventoryManager
Gestiona exclusivamente el equipamiento de armas:
- Referencias a `primaryWeapon` y `secondaryWeapon`
- Métodos: `EquipWeapon()`, `UnequipWeapon()`, `SwapWeapons()`, `GetEquippedWeapon()`
- Propiedades: `PrimaryWeapon`, `SecondaryWeapon`
- Eventos: `OnWeaponEquipped`, `OnWeaponUnequipped`, `OnWeaponsSwapped`

### InventorySystem (Simplificado)
Ahora solo gestiona:
- Slots de items generales (`ItemInstance[]`)
- Operaciones básicas: añadir/remover items, selección de slots
- Delegación a `AmmoInventoryManager` cuando se añaden items de munición

## Cambios Implementados

### 1. InventorySystem.cs
**Eliminado:**
- `private WeaponItemData primaryWeapon`
- `private WeaponItemData secondaryWeapon`
- `private Dictionary<AmmoType, int> ammoInventory`
- `public WeaponItemData PrimaryWeapon => primaryWeapon`
- `public WeaponItemData SecondaryWeapon => secondaryWeapon`
- `public event Action<EquipSlot, WeaponItemData> OnWeaponEquipped`
- `public event Action<EquipSlot> OnWeaponUnequipped`
- `public event Action<AmmoType, int> OnAmmoChanged`
- Métodos: `EquipWeapon()`, `UnequipWeapon()`, `SwapWeapons()`, `GetEquippedWeapon()`
- Métodos: `AddAmmo()`, `RemoveAmmo()`, `GetAmmoCount()`, `HasAmmo()`

**Añadido:**
- `private AmmoInventoryManager ammoManager`
- `private WeaponInventoryManager weaponManager`
- `void Awake()` para obtener referencias a los managers
- Delegación en `TryAddItem()` para items de munición

### 2. InventoryDebugger.cs
**Actualizado para usar managers:**
- Añadidas referencias a `AmmoInventoryManager` y `WeaponInventoryManager`
- Eventos suscritos directamente a los managers correspondientes
- `PrintInventoryState()` ahora usa `weaponManager.PrimaryWeapon` y `ammoManager.GetAmmoCount()`

### 3. InventoryUIController.cs
**Actualizado para usar WeaponInventoryManager:**
- Añadida referencia a `WeaponInventoryManager`
- `EquipPrimary` y `EquipSecondary` ahora usan `weaponManager.EquipWeapon()`

## Migración para Código Existente

Si tienes código que usa la API antigua de `InventorySystem`, actualízalo así:

### Para Munición
```csharp
// ❌ ANTES
inventorySystem.AddAmmo(AmmoType.Pistol_9mm, 10);
int count = inventorySystem.GetAmmoCount(AmmoType.Pistol_9mm);
bool hasEnough = inventorySystem.HasAmmo(AmmoType.Pistol_9mm, 5);

// ✅ DESPUÉS
AmmoInventoryManager ammoManager = player.GetComponent<AmmoInventoryManager>();
ammoManager.AddAmmo(AmmoType.Pistol_9mm, 10);
int count = ammoManager.GetAmmoCount(AmmoType.Pistol_9mm);
bool hasEnough = ammoManager.HasAmmo(AmmoType.Pistol_9mm, 5);
```

### Para Armas
```csharp
// ❌ ANTES
inventorySystem.EquipWeapon(weaponData, EquipSlot.Primary);
WeaponItemData current = inventorySystem.PrimaryWeapon;
inventorySystem.SwapWeapons();

// ✅ DESPUÉS
WeaponInventoryManager weaponManager = player.GetComponent<WeaponInventoryManager>();
weaponManager.EquipWeapon(weaponData, EquipSlot.Primary);
WeaponItemData current = weaponManager.PrimaryWeapon;
weaponManager.SwapWeapons();
```

### Para Eventos
```csharp
// ❌ ANTES
inventorySystem.OnAmmoChanged += OnAmmoChanged;
inventorySystem.OnWeaponEquipped += OnWeaponEquipped;

// ✅ DESPUÉS
ammoManager.OnAmmoChanged += OnAmmoChanged;
weaponManager.OnWeaponEquipped += OnWeaponEquipped;
```

## Setup en GameObject

Para que el sistema funcione correctamente, el GameObject del jugador debe tener:

```
Player (GameObject)
├── InventorySystem          (gestión de items generales)
├── AmmoInventoryManager     (gestión de munición)
├── WeaponInventoryManager   (gestión de armas)
└── KeyInventoryManager      (gestión de llaves)
```

Los managers se auto-referencian en `Awake()`, así que solo necesitas asegurarte de que todos los componentes estén presentes.

## Beneficios

1. **Single Responsibility Principle**: Cada clase tiene una sola razón para cambiar
2. **Código más limpio**: `InventorySystem` ahora ~120 líneas (vs 364 anteriores)
3. **Mejor extensibilidad**: Puedes añadir características específicas a munición o armas sin tocar el sistema general
4. **Eventos específicos**: Los sistemas interesados solo en armas pueden suscribirse a `WeaponInventoryManager`
5. **Testing más fácil**: Puedes probar munición y armas de forma independiente
6. **Mejor semántica**: `weaponManager.EquipWeapon()` es más descriptivo que `inventorySystem.EquipWeapon()`

## Archivos Afectados

- ✏️ `/Assets/Scripts/Inventory/Core/InventorySystem.cs` - Simplificado
- ✏️ `/Assets/Scripts/Inventory/InventoryDebugger.cs` - Usa managers
- ✏️ `/Assets/Scripts/Inventory/UI/InventoryUIController.cs` - Usa WeaponInventoryManager
- ℹ️ `/Assets/Scripts/Inventory/Core/AmmoInventoryManager.cs` - Ya existía
- ℹ️ `/Assets/Scripts/Inventory/Core/WeaponInventoryManager.cs` - Ya existía

## Estado

✅ **Refactorización completada sin errores de compilación**
✅ **Todos los archivos afectados actualizados**
✅ **API pública estable y documentada**
