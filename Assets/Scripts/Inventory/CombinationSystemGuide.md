# 🧪 Sistema de Combinación de Items - Guía Completa

## Índice
1. [Visión General](#visión-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Crear Recetas de Combinación](#crear-recetas-de-combinación)
4. [Configurar Items Combinables](#configurar-items-combinables)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Integración con UI](#integración-con-ui)
7. [Testing](#testing)

> 📚 **Para detalles técnicos de arquitectura:** Ver `/Assets/Scripts/Inventory/Info/COMBINATION_ARCHITECTURE.md`

---

## Visión General

Sistema de combinación inspirado en **Resident Evil** y **Silent Hill** que permite:

✅ **Combinar 2 items** para crear uno nuevo  
✅ **Consumir items** del inventario automáticamente  
✅ **Recetas bidireccionales** (A+B = B+A)  
✅ **Validación automática** de cantidades  
✅ **Feedback visual y sonoro**  
✅ **Integración con menú contextual**  

### Flujo del Jugador

```
1. Selecciona un item combinable
2. Abre menú contextual (E)
3. Selecciona "Combine"
4. Selecciona el segundo item
5. ✅ Items se combinan automáticamente
6. 🎁 Nuevo item aparece en inventario
```

---

## Arquitectura del Sistema

### Componentes Principales

```
┌─────────────────────────────────────────┐
│         ICombinable (Interface)         │
│  - CanCombineWith()                     │
│  - GetPossibleCombinations()            │
│  - GetCombinationHint()                 │
└────────────────┬────────────────────────┘
                 │
                 │ implementa
                 ▼
┌─────────────────────────────────────────┐
│         ItemData (Abstract)             │
│  + CanBeCombined: bool                  │
│  + CombinationHint: string              │
└────────────────┬────────────────────────┘
                 │
                 │ usa
                 ▼
┌─────────────────────────────────────────┐
│    CombinationRecipe (ScriptableObject)│
│  - ItemA: ItemData                      │
│  - ItemB: ItemData                      │
│  - ResultItem: ItemData                 │
│  - ConsumeAmountA: int                  │
│  - ConsumeAmountB: int                  │
│  - ResultQuantity: int                  │
│  - Bidirectional: bool                  │
└────────────────┬────────────────────────┘
                 │
                 │ gestiona
                 ▼
┌─────────────────────────────────────────┐
│    CombinationManager (MonoBehaviour)   │
│  + TryCombine()                         │
│  + FindRecipe()                         │
│  + GetAvailableCombinations()           │
│  + Events: OnCombinationSuccess/Failed  │
└────────────────┬────────────────────────┘
                 │
                 │ integra
                 ▼
┌─────────────────────────────────────────┐
│    InventoryUIController                │
│  + StartCombineMode()                   │
│  + TryCombineWithSelected()             │
│  + Event: OnCombineModeChanged          │
└─────────────────────────────────────────┘
```

---

## Crear Recetas de Combinación

### Paso 1: Crear el ScriptableObject

1. **En Project:** `Create > Inventory > Combination Recipe`
2. **Renombrar:** `GunpowderMix_Recipe` (ejemplo)

### Paso 2: Configurar la Receta

#### **Recipe Info**
```
Recipe Name: "Gunpowder Mix"
Recipe Description: "Combine two types of gunpowder to create high-grade powder."
```

#### **Required Items**
```
Item A: [Arrastra GunpowderA.asset]
Item B: [Arrastra GunpowderB.asset]
Bidirectional: ✓  (permite A+B o B+A)
```

#### **Result**
```
Result Item: [Arrastra HighGradeGunpowder.asset]
Result Quantity: 1
```

#### **Consumption**
```
Consume Amount A: 1  (cuántas unidades de A se consumen)
Consume Amount B: 1  (cuántas unidades de B se consumen)
```

#### **Feedback**
```
Success Message: "Mixed gunpowder successfully!"
Fail Message: "These powders cannot be mixed."
Combination Sound: [Arrastra clip de audio opcional]
```

### Ejemplo Visual

```
┌─────────────────────────────────────┐
│   GunpowderMix_Recipe               │
├─────────────────────────────────────┤
│ Recipe Info:                        │
│   Name: "Gunpowder Mix"             │
│                                     │
│ Required Items:                     │
│   Item A: GunpowderA                │
│   Item B: GunpowderB                │
│   Bidirectional: ✓                  │
│                                     │
│ Result:                             │
│   Result Item: HighGradeGunpowder   │
│   Result Quantity: 1                │
│                                     │
│ Consumption:                        │
│   Consume Amount A: 1               │
│   Consume Amount B: 1               │
└─────────────────────────────────────┘
```

---

## Configurar Items Combinables

### En el ScriptableObject del Item

Para que un item sea combinable:

```
┌─────────────────────────────────────┐
│   GunpowderA (ItemData)             │
├─────────────────────────────────────┤
│ Basic Info:                         │
│   Item Name: "Gunpowder A"          │
│   ...                               │
│                                     │
│ Combination:                        │
│   Can Be Combined: ✓                │ ← ACTIVAR ESTO
│   Combination Hint: "Can be mixed   │
│                      with other     │
│                      gunpowder."    │
└─────────────────────────────────────┘
```

**Importante:**
- ✅ Ambos items (A y B) deben tener `Can Be Combined: ✓`
- ✅ El `Combination Hint` aparece cuando examinas el item

---

## Ejemplos Prácticos

### Ejemplo 1: Gunpowder Mix (Resident Evil Style)

#### Items Necesarios

**GunpowderA.asset**
```
Item Name: "Gunpowder A"
Item Type: Consumable
Stackable: ✓
Can Be Combined: ✓
Combination Hint: "Used to create handgun bullets."
```

**GunpowderB.asset**
```
Item Name: "Gunpowder B"
Item Type: Consumable
Stackable: ✓
Can Be Combined: ✓
Combination Hint: "Used to create shotgun shells."
```

**HighGradeGunpowder.asset**
```
Item Name: "High Grade Gunpowder"
Item Type: Consumable
Stackable: ✓
Description: "High quality gunpowder for crafting ammunition."
```

#### Receta

**GunpowderMix_Recipe.asset**
```
Recipe Name: "Gunpowder Mix"
Item A: GunpowderA
Item B: GunpowderB
Result Item: HighGradeGunpowder
Result Quantity: 1
Consume Amount A: 1
Consume Amount B: 1
Bidirectional: ✓
Success Message: "Created High Grade Gunpowder!"
```

---

### Ejemplo 2: First Aid Spray (RE Style)

#### Items

**Herb.asset**
```
Item Name: "Green Herb"
Stackable: ✓
Can Be Combined: ✓
```

**ChemicalFluid.asset**
```
Item Name: "Chemical Fluid"
Stackable: ✓
Can Be Combined: ✓
```

**FirstAidSpray.asset**
```
Item Name: "First Aid Spray"
Description: "Fully restores health."
```

#### Receta

**FirstAidSpray_Recipe.asset**
```
Item A: Herb
Item B: ChemicalFluid
Result Item: FirstAidSpray
Consume Amount A: 3  ← Necesita 3 hierbas
Consume Amount B: 1
Result Quantity: 1
```

---

### Ejemplo 3: Enhanced Weapon

#### Items

**Pistol.asset**
```
Item Name: "9mm Pistol"
Item Type: Weapon
Can Be Combined: ✓
```

**SilencerAttachment.asset**
```
Item Name: "Silencer"
Item Type: KeyItem
Can Be Combined: ✓
```

**SilencedPistol.asset**
```
Item Name: "Silenced Pistol"
Item Type: Weapon
Damage: 25  ← Mayor que pistola normal
```

#### Receta

**SilencedPistol_Recipe.asset**
```
Item A: Pistol
Item B: SilencerAttachment
Result Item: SilencedPistol
Consume Amount A: 1
Consume Amount B: 1
Result Quantity: 1
Bidirectional: ✓
Success Message: "Attached silencer to pistol!"
```

---

## Integración con UI

### Setup en el Player

El Player GameObject debe tener:

```
Player
├── InventorySystem
├── WeaponInventoryManager
├── AmmoInventoryManager
└── CombinationManager  ← AÑADIR ESTO
```

1. **Add Component** al Player: `Combination Manager`

2. **En CombinationManager Inspector:**
```
All Recipes:
  Size: 3
  Element 0: GunpowderMix_Recipe
  Element 1: FirstAidSpray_Recipe
  Element 2: SilencedPistol_Recipe

Settings:
  Show Debug Logs: ✓
  Allow Multiple Combinations: ☐
```

3. **En InventoryUIController:**
```
References:
  Inventory System: [Auto]
  Weapon Manager: [Auto]
  Combination Manager: [Auto]  ← Se asigna automáticamente
```

### Flujo de UI

#### **Estado Normal**
```
┌─────────────┐
│ INVENTORY   │
├─────────────┤
│ [Item 1]    │ ← Seleccionado
│  Item 2     │
│  Item 3     │
└─────────────┘

Presiona E → Abre Context Menu
```

#### **Context Menu (Item Combinable)**
```
┌─────────────┐
│  Actions    │
├─────────────┤
│ Use         │
│ Examine     │
│ Combine     │ ← NUEVA OPCIÓN
│ Drop        │
└─────────────┘

Selecciona "Combine" → Entra en Combine Mode
```

#### **Combine Mode**
```
┌──────────────────────────┐
│ COMBINE MODE             │
│ Select item to combine   │
├──────────────────────────┤
│ [Gunpowder A] ← Origen   │
│  Gunpowder B  ← Cursor   │
│  Herb         │
└──────────────────────────┘

Presiona E → Combina items
Presiona ESC → Cancela
```

#### **Resultado**
```
┌──────────────────────────┐
│ INVENTORY                │
├──────────────────────────┤
│ [High Grade Gunpowder]   │ ← NUEVO!
│  Herb                    │
│  Empty                   │
└──────────────────────────┘

✅ "Created High Grade Gunpowder!"
```

---

## Testing

### Método 1: Context Menu en Inspector

Con el Player seleccionado:

1. **Right Click** en `CombinationManager` (Inspector)
2. **"List All Recipes"** → Muestra todas las recetas
3. **"List Available Combinations"** → Muestra recetas disponibles

### Método 2: Logs de Debug

Activa `Show Debug Logs` en `CombinationManager`:

```
[COMBINATION] Added recipe: Gunpowder Mix
[COMBINATION] Successfully combined Gunpowder A + Gunpowder B → High Grade Gunpowder
```

### Método 3: Testing en Play Mode

```csharp
// En InventoryDebugger o script de testing
void TestCombination()
{
    // Añadir items al inventario
    inventory.TryAddItem(gunpowderA);
    inventory.TryAddItem(gunpowderB);
    
    // Intentar combinar
    bool success = combinationManager.TryCombine(0, 1);
    
    if (success)
    {
        Debug.Log("✅ Combination successful!");
    }
}
```

### Checklist de Testing

- [ ] Items tienen `Can Be Combined: ✓`
- [ ] Receta está en la lista del `CombinationManager`
- [ ] Receta es válida (ItemA, ItemB, Result asignados)
- [ ] Inventario tiene suficientes cantidades
- [ ] "Combine" aparece en el context menu
- [ ] Combine mode se activa correctamente
- [ ] Items se consumen después de combinar
- [ ] Resultado aparece en inventario
- [ ] Si inventario lleno, no se pierden items

---

## Validaciones Automáticas

El sistema valida automáticamente:

### ✅ Validaciones en CombinationRecipe

```csharp
IsValidRecipe() verifica:
- ItemA != null
- ItemB != null  
- ResultItem != null
- ItemA != ItemB (no puede ser el mismo item)
- ConsumeAmountA > 0
- ConsumeAmountB > 0
- ResultQuantity > 0
```

### ✅ Validaciones en CombinationManager

```csharp
TryCombine() verifica:
- Items no son null
- Items no son iguales
- Existe una receta válida
- Hay suficiente cantidad en inventario
- Hay espacio para el resultado
```

### ✅ Feedback de Error

```
❌ "Cannot combine null items!"
❌ "Cannot combine an item with itself!"
❌ "No recipe found for ItemA + ItemB"
❌ "Not enough items to combine!"
❌ "Inventory is full!"
```

---

## Eventos del Sistema

### CombinationManager Events

```csharp
// Combinación exitosa
OnCombinationSuccess?.Invoke(itemA, itemB, resultItem);

// Combinación fallida
OnCombinationFailed?.Invoke(itemA, itemB);

// Recetas disponibles cambiaron
OnAvailableCombinationsChanged?.Invoke(availableRecipes);
```

### InventoryUIController Events

```csharp
// Modo combine activado/desactivado
OnCombineModeChanged?.Invoke(isActive, sourceItem);
```

### Suscribirse a Eventos

```csharp
void Start()
{
    combinationManager.OnCombinationSuccess += HandleSuccess;
    combinationManager.OnCombinationFailed += HandleFailed;
}

void HandleSuccess(ItemData a, ItemData b, ItemData result)
{
    Debug.Log($"Created {result.ItemName}!");
    // Mostrar UI, reproducir sonido, etc.
}

void HandleFailed(ItemData a, ItemData b)
{
    Debug.Log("Combination failed!");
    // Mostrar mensaje de error
}
```

---

## API Pública

### CombinationManager

```csharp
// Intentar combinar por slots
bool TryCombine(int slotA, int slotB)

// Intentar combinar por items
bool TryCombineItems(ItemData itemA, ItemData itemB)

// Buscar receta
CombinationRecipe FindRecipe(ItemData itemA, ItemData itemB)

// Obtener combinaciones disponibles
List<CombinationRecipe> GetAvailableCombinations()

// Obtener items combinables con X
List<ItemData> GetCombinableItemsFor(ItemData sourceItem)

// Verificar si pueden combinarse
bool CanCombineWith(ItemData itemA, ItemData itemB)

// Gestionar recetas en runtime
void AddRecipe(CombinationRecipe recipe)
void RemoveRecipe(CombinationRecipe recipe)
```

### ItemData (ICombinable)

```csharp
// Verificar si puede combinarse con otro item
bool CanCombineWith(ItemData otherItem)

// Obtener lista de items combinables
List<ItemData> GetPossibleCombinations()

// Obtener hint de combinación
string GetCombinationHint(ItemData otherItem)
```

### InventoryUIController

```csharp
// Iniciar modo combine
void StartCombineMode()

// Cancelar modo combine
void CancelCombineMode()

// Intentar combinar con seleccionado
void TryCombineWithSelected()

// Manejar input de combine
void HandleCombineInput()
```

---

## Tips y Best Practices

### 🎯 Diseño de Recetas

1. **Lógicas y temáticas:**
   - Gunpowder A + B = High Grade
   - Herb + Chemical = Medicine
   - Weapon + Upgrade = Enhanced Weapon

2. **Balanceo:**
   - Consumibles: 1+1 = 1 más poderoso
   - Munición: 2+2 = 3 mejorada
   - Armas: Item + Accesorio = Versión mejorada

3. **Feedback claro:**
   - Mensajes descriptivos
   - Sonidos únicos por tipo
   - Hints informativos

### 🚀 Organización

```
/Assets/Data/Items/
  /Consumables
    Herb.asset
    Chemical.asset
    FirstAidSpray.asset
  /Weapons
    Pistol.asset
    SilencedPistol.asset
  /Attachments
    Silencer.asset

/Assets/Data/Recipes/
  FirstAidSpray_Recipe.asset
  SilencedPistol_Recipe.asset
  GunpowderMix_Recipe.asset
```

### ⚡ Performance

- Recetas se validan en `Awake()` (una vez)
- Búsqueda de recetas es O(n) - optimizable con Dictionary si >50 recetas
- Eventos evitan polling

---

## Troubleshooting

| Problema | Solución |
|----------|----------|
| "Combine" no aparece | Verifica `Can Be Combined: ✓` en ambos items |
| No encuentra receta | Añade receta al array de `CombinationManager` |
| Items no se consumen | Verifica `Consume Amount > 0` |
| Resultado no aparece | Verifica que hay espacio en inventario |
| Receta inválida | Check console: `List All Recipes` en context menu |

---

## Próximos Pasos

Ahora que tienes el sistema:

1. ✅ Crea tus items combinables
2. ✅ Define recetas de combinación
3. ✅ Añade `CombinationManager` al Player
4. ✅ Asigna recetas al manager
5. ✅ ¡Prueba combinando items!

El sistema está listo y completamente integrado con el inventario existente. 🎉
