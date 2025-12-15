# 🎒 InventorySystem - Línea por Línea (1-90)

**Archivo:** `/Assets/Scripts/Inventory/Core/InventorySystem.cs`

---

## 📦 Líneas 1-5: Imports y Namespace

```csharp
1: using System;
```
- Importa el namespace `System` de .NET
- Necesario para: `Action` (delegados/eventos)

```csharp
2: using System.Collections.Generic;
```
- Importa colecciones genéricas de .NET
- Necesario para: `Dictionary<AmmoType, int>`

```csharp
3: using UnityEngine;
```
- Importa el namespace principal de Unity
- Necesario para: `MonoBehaviour`, `Debug`, `GameObject`

```csharp
4:
```
- Línea vacía (separación visual)

```csharp
5: namespace TheHunt.Inventory
```
- Define el namespace del proyecto
- Organiza el código en `TheHunt.Inventory`
- Evita conflictos de nombres con otros scripts

---

## 🏛️ Líneas 6-11: Declaración de Clase y Constantes

```csharp
6: {
```
- Apertura del bloque namespace

```csharp
7:     public class InventorySystem : MonoBehaviour
```
- `public class`: Clase accesible desde otros scripts
- `InventorySystem`: Nombre de la clase
- `: MonoBehaviour`: Hereda de MonoBehaviour (componente de Unity)
- Al heredar de MonoBehaviour puede:
  - Adjuntarse a GameObjects
  - Usar métodos Unity (Awake, Start, Update)
  - Acceder a `gameObject`, `transform`, etc.

```csharp
8:     {
```
- Apertura del bloque de la clase

```csharp
9:         public const int MAX_SLOTS = 6;
```
- `public`: Accesible desde otros scripts
- `const`: Valor constante (no puede cambiar en runtime)
- `int`: Tipo entero
- `MAX_SLOTS = 6`: El inventario tiene exactamente 6 slots (0-5)
- Se usa en: Tamaño del array `items`, validaciones, loops

```csharp
10:         public const int MAX_STACK_SIZE = 6;
```
- Define el máximo apilable por slot
- Un slot puede tener hasta 6 unidades del mismo item stackable
- Usado en: `TryAddItem()` para verificar si se puede stackear

```csharp
11:         public const int EQUIPMENT_SLOTS = 2;
```
- Define cantidad de slots de equipamiento (Primary + Secondary)
- No usado actualmente en el código (reservado para futuro)

---

## 💾 Líneas 13-23: Variables Privadas (Estado Interno)

```csharp
13:         private ItemInstance[] items = new ItemInstance[MAX_SLOTS];
```
- `private`: Solo accesible dentro de esta clase
- `ItemInstance[]`: Array de objetos `ItemInstance`
- `items`: Nombre de la variable
- `new ItemInstance[MAX_SLOTS]`: Crea array de 6 elementos
- **Estado principal del inventario**
- Inicialmente todos los elementos son `null` (vacíos)

```csharp
14:         private int selectedIndex = 0;
```
- Índice del slot actualmente seleccionado
- Inicializado en `0` (primer slot)
- Rango válido: 0-5
- Cambia con: `SelectNext()`, `SelectPrevious()`, `SelectSlot()`

```csharp
15:         private WeaponItemData primaryWeapon;
```
- Referencia al arma equipada en slot primario
- `WeaponItemData`: Tipo de ScriptableObject
- Inicialmente `null` (sin arma equipada)
- Se asigna con: `EquipWeapon(weapon, EquipSlot.Primary)`

```csharp
16:         private WeaponItemData secondaryWeapon;
```
- Referencia al arma equipada en slot secundario
- Similar a `primaryWeapon` pero para segundo slot
- Permite tener 2 armas equipadas simultáneamente

```csharp
17:         private Dictionary<AmmoType, int> ammoInventory = new Dictionary<AmmoType, int>
```
- `Dictionary<AmmoType, int>`: Mapa clave-valor
  - Clave: `AmmoType` (enum: Pistol_9mm, Shotgun_Shell, etc.)
  - Valor: `int` (cantidad de munición)
- **Munición NO ocupa slots del inventario principal**
- Se inicializa inmediatamente con valores:

```csharp
18:         {
```
- Inicio del inicializador de colección

```csharp
19:             { AmmoType.Pistol_9mm, 0 },
```
- Clave: `AmmoType.Pistol_9mm`
- Valor: `0` (sin munición al inicio)

```csharp
20:             { AmmoType.Shotgun_Shell, 0 },
```
- Munición de escopeta: 0

```csharp
21:             { AmmoType.Rifle_762, 0 },
```
- Munición de rifle: 0

```csharp
22:             { AmmoType.Special, 0 }
```
- Munición especial: 0

```csharp
23:         };
```
- Fin del inicializador y de la declaración

---

## 🔍 Líneas 25-31: Propiedades Públicas (Read-Only)

```csharp
25:         public ItemInstance CurrentItem => selectedIndex >= 0 && selectedIndex < MAX_SLOTS ? items[selectedIndex] : null;
```
- `public ItemInstance CurrentItem`: Propiedad pública
- `=>`: Expresión lambda (property expression-bodied)
- `selectedIndex >= 0 && selectedIndex < MAX_SLOTS`: Valida índice
- `? items[selectedIndex]`: Si válido, retorna el item
- `: null`: Si inválido, retorna null
- **Get-only property** (solo lectura desde fuera)

```csharp
26:         public bool IsFull => FindEmptySlot() == -1;
```
- Retorna `true` si el inventario está lleno
- `FindEmptySlot()` retorna `-1` cuando no hay slots vacíos
- Usado para: Deshabilitar botón de pickup, mostrar mensaje

```csharp
27:         public bool HasSpace => !IsFull;
```
- Retorna `true` si hay espacio disponible
- Simplemente es la negación de `IsFull`
- Más legible que usar `!IsFull` en el código

```csharp
28:         public int SelectedSlot => selectedIndex;
```
- Expone el índice del slot seleccionado
- Read-only: otros scripts pueden leer pero no modificar directamente
- Para cambiar se usan: `SelectNext()`, `SelectPrevious()`, `SelectSlot()`

```csharp
29:         public WeaponItemData PrimaryWeapon => primaryWeapon;
```
- Expone el arma primaria equipada
- Retorna `null` si no hay arma equipada

```csharp
30:         public WeaponItemData SecondaryWeapon => secondaryWeapon;
```
- Expone el arma secundaria equipada

```csharp
31:         public ItemInstance[] Items => items;
```
- **Expone el array completo de items**
- Permite a otros scripts leer todos los slots
- ⚠️ Retorna referencia al array (no copia)
- Se puede usar para: UI, crafting, save system

---

## 📣 Líneas 33-40: Eventos

```csharp
33:         public event Action<int, ItemInstance> OnItemAdded;
```
- `public event`: Evento público
- `Action<int, ItemInstance>`: Delegado con 2 parámetros
  - `int`: Índice del slot donde se añadió
  - `ItemInstance`: El item añadido
- Suscriptores: UI, audio, save system
- Se dispara en: `TryAddItem()` cuando se añade exitosamente

```csharp
34:         public event Action<int, ItemInstance> OnItemRemoved;
```
- Similar a `OnItemAdded`
- Se dispara en: `RemoveItem()`
- Parámetros: slot y item removido

```csharp
35:         public event Action<ItemInstance> OnItemUsed;
```
- `Action<ItemInstance>`: 1 parámetro
- Se dispara en: `UseCurrentItem()` después de usar
- Usado para: Efectos visuales, sonido, achievements

```csharp
36:         public event Action<int, int> OnSelectionChanged;
```
- `Action<int, int>`: 2 enteros
  - Primer `int`: Índice anterior
  - Segundo `int`: Índice nuevo
- Se dispara en: `SelectNext()`, `SelectPrevious()`, `SelectSlot()`
- Usado para: Actualizar highlight en UI

```csharp
37:         public event Action OnInventoryFull;
```
- `Action`: Sin parámetros
- Se dispara en: `TryAddItem()` cuando no hay espacio
- Usado para: Mostrar mensaje, reproducir sonido

```csharp
38:         public event Action<EquipSlot, WeaponItemData> OnWeaponEquipped;
```
- `Action<EquipSlot, WeaponItemData>`: 2 parámetros
  - `EquipSlot`: Primary o Secondary
  - `WeaponItemData`: Arma equipada
- Se dispara en: `EquipWeapon()`, `SwapWeapons()`

```csharp
39:         public event Action<EquipSlot> OnWeaponUnequipped;
```
- Se dispara en: `UnequipWeapon()`
- Parámetro: Slot que se vació

```csharp
40:         public event Action<AmmoType, int> OnAmmoChanged;
```
- Se dispara en: `AddAmmo()`, `RemoveAmmo()`
- Parámetros: Tipo de munición y cantidad total actual

---

## 🔧 Líneas 42-85: Método TryAddItem (Parte 1)

```csharp
42:         public bool TryAddItem(ItemData itemData)
```
- `public bool`: Método público que retorna verdadero/falso
- `TryAddItem`: Nombre del método (patrón "Try...")
- `ItemData itemData`: Parámetro - el ScriptableObject a añadir
- **Método más importante del sistema**

```csharp
43:         {
```
- Inicio del cuerpo del método

```csharp
44:             if (itemData == null)
```
- Validación: ¿El parámetro es null?
- Previene errores de NullReferenceException

```csharp
45:             {
```

```csharp
46:                 Debug.LogWarning("[INVENTORY] Cannot add null item");
```
- `Debug.LogWarning`: Log amarillo en consola
- Mensaje descriptivo para debugging

```csharp
47:                 return false;
```
- Sale del método inmediatamente
- Retorna `false` = item NO fue añadido

```csharp
48:             }
```

```csharp
50:             if (itemData is AmmoItemData ammoData)
```
- **Pattern matching** (C# 7.0+)
- `is AmmoItemData`: Comprueba si es de tipo munición
- `ammoData`: Si es verdadero, crea variable con el cast
- Manejo especial para munición (va a Dictionary, no a slots)

```csharp
51:             {
```

```csharp
52:                 AddAmmo(ammoData.AmmoType, ammoData.AmmoAmount);
```
- Llama método privado `AddAmmo()`
- `ammoData.AmmoType`: Ej. `Pistol_9mm`
- `ammoData.AmmoAmount`: Ej. `12`

```csharp
53:                 return true;
```
- Munición añadida exitosamente
- Sale del método (no continúa)

```csharp
54:             }
```

```csharp
56:             if (itemData.IsStackable)
```
- ¿El item permite apilarse?
- `IsStackable`: Propiedad de `ItemData`
- Ej: Consumibles suelen ser stackables, armas no

```csharp
57:             {
```

```csharp
58:                 for (int i = 0; i < MAX_SLOTS; i++)
```
- Loop por todos los 6 slots
- `i`: Índice actual (0 a 5)

```csharp
59:                 {
```

```csharp
60:                     if (items[i] != null &&
```
- ¿El slot tiene un item?

```csharp
61:                         items[i].itemData == itemData &&
```
- ¿Es el MISMO ScriptableObject?
- Compara referencias, no valores

```csharp
62:                         items[i].quantity < MAX_STACK_SIZE)
```
- ¿Hay espacio para stackear? (cantidad < 6)

```csharp
63:                     {
```
- Si las 3 condiciones son verdaderas:

```csharp
64:                         items[i].quantity++;
```
- Incrementa la cantidad en 1
- Ej: 3 → 4

```csharp
65:                         OnItemAdded?.Invoke(i, items[i]);
```
- `?.`: Null-conditional operator
- Solo invoca si hay suscriptores
- Parámetros: índice del slot e item

```csharp
66:                         Debug.Log($"<color=green>[INVENTORY] Stacked {itemData.ItemName}. Total: {items[i].quantity}</color>");
```
- `$"..."`: String interpolation
- `<color=green>`: Rich text de Unity (verde en consola)
- `{itemData.ItemName}`: Inserta nombre del item
- `{items[i].quantity}`: Inserta cantidad actual

```csharp
67:                         return true;
```
- Item stackeado exitosamente
- Sale del método

```csharp
68:                     }
69:                 }
70:             }
```

```csharp
72:             int emptySlot = FindEmptySlot();
```
- Busca primer slot vacío
- Retorna índice (0-5) o -1 si todos ocupados

```csharp
73:             if (emptySlot == -1)
```
- ¿No hay slots vacíos?

```csharp
74:             {
```

```csharp
75:                 OnInventoryFull?.Invoke();
```
- Dispara evento de inventario lleno

```csharp
76:                 Debug.Log("<color=yellow>[INVENTORY] Inventory is full!</color>");
```
- Log amarillo

```csharp
77:                 return false;
```
- No se pudo añadir

```csharp
78:             }
```

```csharp
80:             items[emptySlot] = new ItemInstance(itemData, 1);
```
- Crea nueva instancia con:
  - `itemData`: El ScriptableObject
  - `1`: Cantidad inicial
- La asigna al slot vacío

```csharp
81:             OnItemAdded?.Invoke(emptySlot, items[emptySlot]);
```
- Dispara evento con slot e item nuevo

```csharp
82:             Debug.Log($"<color=green>[INVENTORY] Added {itemData.ItemName} to slot {emptySlot}</color>");
```
- Log verde de éxito

```csharp
84:             return true;
```
- Item añadido exitosamente

```csharp
85:         }
```
- Fin del método `TryAddItem`

---

## 🗑️ Líneas 87-90: Método RemoveItem (Inicio)

```csharp
87:         public void RemoveItem(int slotIndex, int quantity = 1)
```
- `public void`: Público, no retorna valor
- `slotIndex`: Índice del slot (0-5)
- `quantity = 1`: Parámetro opcional, default 1

```csharp
88:         {
```

```csharp
89:             if (slotIndex < 0 || slotIndex >= MAX_SLOTS || items[slotIndex] == null)
```
- Validación triple:
  1. `slotIndex < 0`: Índice negativo
  2. `slotIndex >= MAX_SLOTS`: Índice fuera de rango
  3. `items[slotIndex] == null`: Slot vacío

```csharp
90:                 return;
```
- Si alguna validación falla, sale sin hacer nada

---

**Continúa en:** InventorySystem_Lineas_91-180.md
