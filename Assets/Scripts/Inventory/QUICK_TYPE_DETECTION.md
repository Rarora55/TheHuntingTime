# 🎯 Detección de Tipo: Explicación Rápida

## ❓ Pregunta: ¿Cómo el inventario sabe que la planta verde es consumible?

### Respuesta Corta

**El sistema usa el operador `is` de C# para detectar interfaces en runtime.**

```csharp
if (itemData is IUsable)  // ← Chequea si implementa IUsable
{
    // ✅ Es consumible, puede ser usado
}
```

---

## 🔍 Los 3 Pasos Clave

### 1️⃣ CREACIÓN: El Item YA ES Consumible

```csharp
[CreateAssetMenu(menuName = "Inventory/Consumable Item")]
public class ConsumableItemData : ItemData, IUsable  // ← Implementa IUsable
{
    public float healAmount = 10f;
    
    public bool Use(GameObject user)
    {
        health.Heal(healAmount);
        return true;
    }
}
```

**Cuando creas `GreenHerbItem.asset`, ya es `ConsumableItemData`**

---

### 2️⃣ ALMACENAMIENTO: Se Guarda el Tipo Real

```csharp
// En InventorySystem.cs
private ItemInstance[] items;

// Añadir item
items[0] = new ItemInstance {
    itemData = GreenHerbItem  // ← GreenHerbItem es ConsumableItemData
};
```

**Aunque la variable es tipo `ItemData`, el objeto REAL sigue siendo `ConsumableItemData`**

```
┌─────────────────────────┐
│ items[0].itemData       │ ← Tipo declarado: ItemData
│ (referencia)            │
└───────────┬─────────────┘
            │
            ↓
┌───────────────────────────────────┐
│ GreenHerbItem                     │
│ Tipo REAL: ConsumableItemData     │ ← El objeto sabe su tipo real
│ • Implementa IUsable              │
│ • healAmount = 10                 │
└───────────────────────────────────┘
```

---

### 3️⃣ DETECCIÓN: Se Chequea el Tipo Real

```csharp
// En InventoryUIController.cs - OpenContextMenu()

ItemData item = items[0].itemData;  // Tipo declarado: ItemData

// C# chequea el tipo REAL del objeto en runtime
if (item is IUsable usable)  // ← Pattern matching
{
    // ✅ GreenHerbItem ES ConsumableItemData
    // ✅ ConsumableItemData IMPLEMENTA IUsable
    // → La condición es TRUE
    
    if (usable.CanUse(gameObject))
    {
        availableActions.Add(ItemContextAction.Use);  // ← Se añade "Use"
    }
}
```

**El menú muestra "Use" porque detectó la interfaz `IUsable`**

---

## 🧪 Ejemplo Completo: Green Herb vs Pistol

### Green Herb (Consumible)

```csharp
// TIPO REAL
class ConsumableItemData : ItemData, IUsable { }

// DETECCIÓN
if (itemData is IUsable)      → ✅ TRUE
if (itemData is WeaponItemData) → ❌ FALSE

// MENÚ RESULTANTE
┌────────────┐
│ ► Use      │ ← Detectó IUsable
│   Examine  │
│   Drop     │
└────────────┘
```

### Pistol (Arma)

```csharp
// TIPO REAL
class WeaponItemData : ItemData, IEquippable { }

// DETECCIÓN
if (itemData is IUsable)        → ❌ FALSE
if (itemData is WeaponItemData) → ✅ TRUE

// MENÚ RESULTANTE
┌─────────────────┐
│ ► Equip Primary │ ← Detectó WeaponItemData
│   Equip Second  │
│   Examine       │
│   Drop          │
└─────────────────┘
```

---

## 🎨 Diagrama Visual Completo

```
CREACIÓN                ALMACENAMIENTO           DETECCIÓN              USO
(Editor)               (Runtime)                 (Runtime)              (Runtime)

Designer crea          InventorySystem           UIController          InventorySystem
GreenHerbItem.asset    almacena                  detecta               ejecuta
                                                 
┌──────────────┐       ┌──────────────┐         ┌──────────────┐      ┌──────────────┐
│ Consumable   │       │ items[0] =   │         │ if (item is  │      │ if (item is  │
│ ItemData     │       │ {            │         │    IUsable)  │      │    IUsable)  │
│              │       │   itemData: ─┼────────→│              │      │ {            │
│ + IUsable ✅ │       │   GreenHerb  │         │ → TRUE ✅    │      │   usable.    │
└──────────────┘       │ }            │         │              │      │   Use(player)│
                       └──────────────┘         │ Add "Use" to │      │ }            │
                                                │ menu         │      └──────────────┘
                                                └──────────────┘              │
                                                                              ↓
                                                                      ┌──────────────┐
                                                                      │ Consumable.  │
                                                                      │ Use()        │
                                                                      │              │
                                                                      │ health.Heal  │
                                                                      │ (10)         │
                                                                      └──────────────┘
```

---

## 💻 Código Real del Sistema

### Donde Ocurre la Magia

```csharp
// InventoryUIController.cs - Línea ~120
private void OpenContextMenu()
{
    ItemInstance currentItem = inventorySystem.CurrentItem;
    availableActions.Clear();
    
    // ═══════════════════════════════════════════════════════════
    // AQUÍ ES DONDE SE DETECTA EL TIPO
    // ═══════════════════════════════════════════════════════════
    
    if (currentItem.itemData is IUsable usable)  // ← DETECCIÓN
    {
        if (usable.CanUse(gameObject))  // ← VALIDACIÓN
        {
            availableActions.Add(ItemContextAction.Use);  // ← AÑADE OPCIÓN
        }
    }
    
    // Resto del código...
}
```

### Cómo Funciona el Operador `is`

```csharp
ItemData item = GreenHerbItem;

// El compilador traduce esto:
if (item is IUsable usable)

// A esto (simplificado):
if (item != null && item.GetType().GetInterfaces().Contains(typeof(IUsable)))
{
    IUsable usable = (IUsable)item;
    // ...
}

// Resultado:
// - Chequea si el tipo REAL implementa la interfaz
// - Si es true, hace el cast automáticamente
// - La variable 'usable' ya es tipo IUsable
```

---

## 🔑 Conceptos Clave de C#

### Polimorfismo

```csharp
// Una variable puede tener un tipo declarado diferente al tipo real

ItemData item;  // Tipo declarado: ItemData (base)

item = new ConsumableItemData();  // Tipo real: ConsumableItemData
item = new WeaponItemData();      // Tipo real: WeaponItemData
item = new KeyItemData();         // Tipo real: KeyItemData

// C# RECUERDA el tipo real en runtime
```

### Type Checking con `is`

```csharp
object obj = "Hello";

if (obj is string)     → ✅ true (es string)
if (obj is int)        → ❌ false (no es int)

// Con pattern matching (C# 7.0+)
if (obj is string text)
{
    // 'text' es automáticamente tipo string
    int length = text.Length;  // ← No necesitas cast
}
```

### Interfaces

```csharp
// Contrato que promete implementar métodos
interface IUsable
{
    bool Use(GameObject user);
}

// Clase que cumple el contrato
class ConsumableItemData : ItemData, IUsable
{
    public bool Use(GameObject user)  // ← Implementación
    {
        // Código...
    }
}

// Uso polimórfico
IUsable usable = new ConsumableItemData();
usable.Use(player);  // ← Llama a ConsumableItemData.Use()
```

---

## 🎓 Por Qué Esto Es Poderoso

### ✅ Extensible

Añadir nuevos tipos de items es fácil:

```csharp
// Nuevo tipo: Comida
public class FoodItemData : ItemData, IUsable
{
    public float hungerRestore = 50f;
    
    public bool Use(GameObject user)
    {
        user.GetComponent<HungerSystem>()?.Restore(hungerRestore);
        return true;
    }
}

// ✅ El menú contextual YA lo detectará automáticamente
// ✅ NO necesitas modificar InventoryUIController
// ✅ NO necesitas modificar InventorySystem
```

### ✅ Sin Switch Statements

**Antes (malo):**
```csharp
switch (item.itemType)
{
    case ItemType.Consumable:
        // Lógica de consumible
        break;
    case ItemType.Weapon:
        // Lógica de arma
        break;
    // Cada nuevo tipo requiere modificar este switch
}
```

**Ahora (bueno):**
```csharp
if (item is IUsable usable)
    usable.Use(player);  // ← Polimorfismo se encarga
    
if (item is IEquippable equippable)
    equippable.Equip(player);

// Nuevos tipos NO requieren modificar este código
```

### ✅ Type Safety

```csharp
// El compilador garantiza que si pasas el 'is', el objeto tiene los métodos

if (item is IUsable usable)
{
    usable.Use(player);  // ✅ Compilador sabe que Use() existe
}

// Si no pasas el 'is':
usable.Use(player);  // ❌ Error de compilación (usable no existe)
```

---

## 📊 Tabla de Detección

| Item Type | ItemData | IUsable | IEquippable | WeaponItemData | Menú Generado |
|-----------|----------|---------|-------------|----------------|---------------|
| **Green Herb** | ✅ | ✅ | ❌ | ❌ | Use, Examine, Drop |
| **Pistol** | ✅ | ❌ | ✅ | ✅ | Equip×2, Examine, Drop |
| **Key** | ✅ | ❌ | ❌ | ❌ | Examine, Drop |
| **Ammo** | ✅ | ❌ | ❌ | ❌ | (No slot, va a contador) |

---

## 🚀 Resumen Ultra-Rápido

1. **Creas** `GreenHerbItem` como `ConsumableItemData` (implementa `IUsable`)
2. **Se almacena** la referencia como `ItemData`, pero el objeto REAL es `ConsumableItemData`
3. **Se detecta** con `if (item is IUsable)` → devuelve `true`
4. **Se añade** "Use" al menú contextual
5. **Se ejecuta** con `usable.Use()` → polimorfismo llama a `ConsumableItemData.Use()`

**La planta "sabe" que es consumible porque ES un `ConsumableItemData` que implementa `IUsable`.** 🌿✨

---

## 📚 Más Info

- Lee `/Assets/Scripts/Inventory/ARCHITECTURE.md` para arquitectura completa
- Lee `/Assets/Scripts/Inventory/ITEM_DETECTION_FLOW.md` para flujo detallado
- Lee documentación de C# sobre polimorfismo e interfaces
