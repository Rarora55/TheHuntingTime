# 🎒 InventorySystem - ÍNDICE COMPLETO

**Proyecto:** TheHuntProject | **Unity:** 6000.3  
**Archivo:** `/Assets/Scripts/Inventory/Core/InventorySystem.cs`

---

## 📚 Documentación Línea por Línea

1. **InventorySystem_Lineas_1-90.md** - Imports, Clase, Constantes, Variables, Propiedades, Eventos, TryAddItem (inicio)
2. **InventorySystem_Lineas_91-180.md** - RemoveItem, UseCurrentItem, Drop, Examine, Navegación
3. **InventorySystem_Lineas_181-270.md** - EquipWeapon, UnequipWeapon, SwapWeapons, AddAmmo (inicio)
4. **InventorySystem_Lineas_271-311.md** - AddAmmo, RemoveAmmo, HasAmmo, FindEmptySlot, Cierre

---

## 🗺️ Mapa del Script (311 líneas)

```
LÍNEAS 1-11: DECLARACIONES
├── 1-3:    Imports (System, Collections, Unity)
├── 5:      Namespace TheHunt.Inventory
├── 7:      Clase InventorySystem : MonoBehaviour
└── 9-11:   Constantes (MAX_SLOTS, MAX_STACK_SIZE, EQUIPMENT_SLOTS)

LÍNEAS 13-23: VARIABLES PRIVADAS
├── 13:     items[6]
├── 14:     selectedIndex
├── 15-16:  primaryWeapon, secondaryWeapon
└── 17-23:  ammoInventory{} (Dictionary)

LÍNEAS 25-31: PROPIEDADES PÚBLICAS
├── 25:     CurrentItem
├── 26-27:  IsFull, HasSpace
├── 28:     SelectedSlot
├── 29-30:  PrimaryWeapon, SecondaryWeapon
└── 31:     Items

LÍNEAS 33-40: EVENTOS
├── 33:     OnItemAdded
├── 34:     OnItemRemoved
├── 35:     OnItemUsed
├── 36:     OnSelectionChanged
├── 37:     OnInventoryFull
├── 38:     OnWeaponEquipped
├── 39:     OnWeaponUnequipped
└── 40:     OnAmmoChanged

LÍNEAS 42-85: TryAddItem() ⭐
├── 44-48:  Validación null
├── 50-54:  Caso especial: Munición
├── 56-70:  Intento de stackear
├── 72-78:  Validación de espacio
└── 80-84:  Añadir a slot vacío

LÍNEAS 87-106: RemoveItem()
├── 89-90:  Validación
├── 92-93:  Decrementar cantidad
├── 95-103: Vaciar slot o mantener
└── 105:    Evento OnItemRemoved

LÍNEAS 108-136: UseCurrentItem()
├── 110-114: Validación item seleccionado
├── 116-131: Si es IUsable
│   ├── 118-122: Validar CanUse()
│   ├── 124:     Ejecutar Use()
│   ├── 125:     Evento OnItemUsed
│   └── 127-130: Remover si RemoveOnUse
└── 132-135: Si NO es usable

LÍNEAS 138-145: DropCurrentItem()
LÍNEAS 147-159: ExamineCurrentItem()

LÍNEAS 161-167: SelectNext()
LÍNEAS 169-177: SelectPrevious()
LÍNEAS 179-187: SelectSlot()

LÍNEAS 189-228: EquipWeapon()
├── 191-192: Validación null
├── 194-208: Verificar arma en inventario
├── 210-223: Equipar en Primary o Secondary
├── 225:     Llamar weapon.Equip()
└── 226-227: Evento y log

LÍNEAS 230-246: UnequipWeapon()
LÍNEAS 248-260: SwapWeapons()
LÍNEAS 262-265: GetEquippedWeapon()

LÍNEAS 267-275: AddAmmo()
LÍNEAS 277-285: RemoveAmmo()
LÍNEAS 287-290: GetAmmoCount()
LÍNEAS 292-298: HasAmmo()

LÍNEAS 300-308: FindEmptySlot() (privado)

LÍNEAS 309-311: CIERRE
└── Clase y namespace
```

---

## 🎯 Navegación Rápida por Tema

### Para Aprender Items:
1. Lee **Líneas 42-85** (TryAddItem)
2. Lee **Líneas 87-106** (RemoveItem)
3. Lee **Líneas 108-136** (UseCurrentItem)

### Para Aprender Armas:
1. Lee **Líneas 189-228** (EquipWeapon)
2. Lee **Líneas 230-260** (UnequipWeapon, SwapWeapons)

### Para Aprender Munición:
1. Lee **Líneas 267-298** (AddAmmo, RemoveAmmo, HasAmmo)

---

## 🔑 Líneas Más Importantes

### Top 10 Líneas Clave:

1. **Línea 13** - `items[6]` - Estado principal
2. **Línea 42** - `TryAddItem()` - Método más importante
3. **Línea 50** - Pattern matching munición
4. **Línea 56** - Lógica de stacking
5. **Línea 108** - `UseCurrentItem()` - Usar items
6. **Línea 116** - Pattern matching IUsable
7. **Línea 189** - `EquipWeapon()` - Equipar armas
8. **Línea 267** - `AddAmmo()` - Añadir munición
9. **Línea 277** - `RemoveAmmo()` - Disparar
10. **Línea 300** - `FindEmptySlot()` - Búsqueda de espacio

---

## 💡 Conceptos por Línea

| Concepto | Línea | Descripción |
|----------|-------|-------------|
| **MonoBehaviour** | 7 | Herencia de Unity |
| **Const** | 9-11 | Constantes inmutables |
| **Array** | 13 | `items[6]` |
| **Dictionary** | 17 | `ammoInventory{}` |
| **Property** | 25-31 | Get-only properties |
| **Event** | 33-40 | Pattern Observer |
| **Pattern Matching** | 50, 116 | `is AmmoItemData` |
| **Null-Conditional** | 65, 81 | `?.Invoke()` |
| **Operador Módulo** | 164 | `% MAX_SLOTS` |
| **Operador Ternario** | 232, 264 | `? :` |

---

## 🚀 Cómo Usar Este Índice

### Opción 1: Lectura Secuencial
Lee los 4 archivos en orden (1→2→3→4)

### Opción 2: Por Tema
Usa "Navegación Rápida por Tema" arriba

### Opción 3: Referencia Rápida
Usa "Mapa del Script" para encontrar líneas específicas

---

## 📊 Estadísticas del Script

```
Total líneas:      311
Código ejecutable: ~200
Métodos públicos:  14
Métodos privados:  1
Propiedades:       7
Eventos:           8
Constantes:        3
Variables:         5
```

---

## 🔄 Flujo de Ejemplo Completo

### Recoger y Usar Green Herb:

```
[Usuario presiona F cerca de Green Herb]
  ↓
PickupItem.Interact()
  → inventory.TryAddItem(greenHerbItem)
  ↓
LÍNEA 42: TryAddItem() ejecutado
LÍNEA 44: ¿null? NO
LÍNEA 50: ¿Munición? NO
LÍNEA 56: ¿Stackable? SÍ
LÍNEA 58: Loop buscando stack
LÍNEA 60-62: Encontrado en slot 0 con qty=2
LÍNEA 64: quantity++ → 3
LÍNEA 65: OnItemAdded disparado
  ↓
[Usuario presiona E]
  ↓
LÍNEA 108: UseCurrentItem()
LÍNEA 110: ¿CurrentItem null? NO
LÍNEA 116: ¿IUsable? SÍ
LÍNEA 118: ¿CanUse()? SÍ
LÍNEA 124: itemData.Use(player) → Heal(10)
LÍNEA 125: OnItemUsed disparado
LÍNEA 127: ¿RemoveOnUse? SÍ
LÍNEA 129: RemoveItem(0, 1)
  ↓
LÍNEA 93: quantity-- → 2
LÍNEA 105: OnItemRemoved disparado
  ↓
[UI actualiza: "Green Herb x2"]
```

---

## 📋 Estructura Visual del Script

```
InventorySystem.cs (311 líneas)
│
├─ 📦 SETUP (líneas 1-40)
│  ├─ Imports
│  ├─ Constantes
│  ├─ Variables
│  ├─ Propiedades
│  └─ Eventos
│
├─ 🎒 ITEMS (líneas 42-187)
│  ├─ TryAddItem()    ⭐ Más importante
│  ├─ RemoveItem()
│  ├─ UseCurrentItem() ⭐ Clave
│  ├─ DropCurrentItem()
│  ├─ ExamineCurrentItem()
│  └─ Select (Next/Previous/Slot)
│
├─ ⚔️ ARMAS (líneas 189-265)
│  ├─ EquipWeapon()
│  ├─ UnequipWeapon()
│  ├─ SwapWeapons()
│  └─ GetEquippedWeapon()
│
├─ 🔫 MUNICIÓN (líneas 267-298)
│  ├─ AddAmmo()
│  ├─ RemoveAmmo()
│  ├─ GetAmmoCount()
│  └─ HasAmmo()
│
└─ 🔧 HELPERS (líneas 300-311)
   └─ FindEmptySlot()
```

---

**¡Todo el script InventorySystem explicado en detalle!** 🎮✨

---

## 📁 Archivos en esta carpeta:

- `InventorySystem_INDICE_COMPLETO.md` ← Estás aquí
- `InventorySystem_Lineas_1-90.md`
- `InventorySystem_Lineas_91-180.md`
- `InventorySystem_Lineas_181-270.md`
- `InventorySystem_Lineas_271-311.md`
