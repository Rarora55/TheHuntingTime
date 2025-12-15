# 🎒 InventorySystem - Línea por Línea (271-311)

**Archivo:** `/Assets/Scripts/Inventory/Core/InventorySystem.cs`

---

## 🔫 Líneas 271-275: Método AddAmmo (Continuación)

```csharp
272:             ammoInventory[type] += amount;
```
- Accede al Dictionary usando `type` como clave
- `+=`: Incrementa el valor actual
- Ejemplo:
  ```csharp
  // ammoInventory[Pistol_9mm] = 30
  AddAmmo(AmmoType.Pistol_9mm, 12);
  // ammoInventory[Pistol_9mm] = 42
  ```

```csharp
273:             OnAmmoChanged?.Invoke(type, ammoInventory[type]);
```
- Dispara evento con:
  - `type`: Tipo de munición modificada
  - `ammoInventory[type]`: Cantidad **total** actual (no la añadida)
- UI puede actualizar contador: "9mm: 42"

```csharp
274:             Debug.Log($"<color=green>[AMMO] Added {amount} {type}. Total: {ammoInventory[type]}</color>");
```
- Log verde mostrando:
  - Cantidad añadida
  - Tipo de munición
  - Total actualizado
- Ejemplo: "[AMMO] Added 12 Pistol_9mm. Total: 42"

```csharp
275:         }
```

---

## ➖ Líneas 277-285: Método RemoveAmmo

```csharp
277:         public bool RemoveAmmo(AmmoType type, int amount)
```
- Remueve munición
- `bool`: Retorna si fue exitoso
- Usado típicamente al disparar

```csharp
278:         {
```

```csharp
279:             if (!HasAmmo(type, amount))
```
- Valida si hay suficiente munición
- Llama método `HasAmmo()` (línea 292)
- Si retorna `false`, no hay munición suficiente

```csharp
280:                 return false;
```
- No puede remover → retorna `false`
- El sistema de disparo puede mostrar "No ammo!"

```csharp
282:             ammoInventory[type] -= amount;
```
- Reduce la cantidad en el Dictionary
- `-=`: Decrementa
- Ejemplo:
  ```csharp
  // ammoInventory[Pistol_9mm] = 42
  RemoveAmmo(AmmoType.Pistol_9mm, 1);
  // ammoInventory[Pistol_9mm] = 41
  ```

```csharp
283:             OnAmmoChanged?.Invoke(type, ammoInventory[type]);
```
- Dispara evento con cantidad actualizada
- UI actualiza contador

```csharp
284:             return true;
```
- Remoción exitosa
- El sistema de disparo puede ejecutar el disparo

```csharp
285:         }
```

---

## 🔢 Líneas 287-290: Método GetAmmoCount

```csharp
287:         public int GetAmmoCount(AmmoType type)
```
- Getter simple para cantidad de munición
- Retorna `int` (la cantidad)

```csharp
288:         {
```

```csharp
289:             return type == AmmoType.None ? 0 : ammoInventory[type];
```
- **Operador ternario**
- Si `type == AmmoType.None`:
  - Retorna `0` (armas sin munición)
- Si no:
  - Retorna `ammoInventory[type]` (cantidad del Dictionary)
- Usado para: Mostrar en UI, validar antes de disparar

```csharp
290:         }
```

---

## ✅ Líneas 292-298: Método HasAmmo

```csharp
292:         public bool HasAmmo(AmmoType type, int required)
```
- Verifica si hay suficiente munición
- `type`: Tipo de munición
- `required`: Cantidad requerida
- Retorna `bool`

```csharp
293:         {
```

```csharp
294:             if (type == AmmoType.None)
```
- ¿El arma no requiere munición?
- Ejemplo: Espada, cuchillo

```csharp
295:                 return true;
```
- Siempre hay "munición" para armas cuerpo a cuerpo

```csharp
297:             return ammoInventory[type] >= required;
```
- Compara cantidad actual vs requerida
- `>=`: Mayor o igual
- Ejemplos:
  ```csharp
  // ammoInventory[Pistol_9mm] = 5
  HasAmmo(Pistol_9mm, 1)  → true  (5 >= 1)
  HasAmmo(Pistol_9mm, 10) → false (5 >= 10)
  ```

```csharp
298:         }
```

---

## 🔍 Líneas 300-308: Método FindEmptySlot (Privado)

```csharp
300:         private int FindEmptySlot()
```
- `private`: Solo usado internamente
- `int`: Retorna índice del slot vacío
- Usado por: `TryAddItem()`

```csharp
301:         {
```

```csharp
302:             for (int i = 0; i < MAX_SLOTS; i++)
```
- Loop por todos los slots (0-5)

```csharp
303:             {
```

```csharp
304:                 if (items[i] == null)
```
- ¿Este slot está vacío?

```csharp
305:                     return i;
```
- **Sale inmediatamente** retornando el índice
- Retorna el **primer** slot vacío encontrado

```csharp
306:             }
```

```csharp
307:             return -1;
```
- Si el loop termina sin encontrar slot vacío
- Retorna `-1` (valor especial indicando "no encontrado")
- Usado en `TryAddItem()` para detectar inventario lleno

```csharp
308:         }
```

---

## 🏁 Líneas 309-311: Cierre

```csharp
309:     }
```
- Cierre del bloque de la clase `InventorySystem`

```csharp
310: }
```
- Cierre del namespace `TheHunt.Inventory`

```csharp
311:
```
- Línea vacía final (buena práctica)

---

## 📊 Resumen del Script Completo

### Estructura General:

```
InventorySystem (MonoBehaviour)
├── Constantes (3)
│   ├── MAX_SLOTS = 6
│   ├── MAX_STACK_SIZE = 6
│   └── EQUIPMENT_SLOTS = 2
│
├── Variables Privadas (5)
│   ├── items[6]
│   ├── selectedIndex
│   ├── primaryWeapon
│   ├── secondaryWeapon
│   └── ammoInventory{}
│
├── Propiedades (7)
│   ├── CurrentItem
│   ├── IsFull / HasSpace
│   ├── SelectedSlot
│   ├── PrimaryWeapon
│   ├── SecondaryWeapon
│   └── Items
│
├── Eventos (8)
│   ├── OnItemAdded
│   ├── OnItemRemoved
│   ├── OnItemUsed
│   ├── OnSelectionChanged
│   ├── OnInventoryFull
│   ├── OnWeaponEquipped
│   ├── OnWeaponUnequipped
│   └── OnAmmoChanged
│
└── Métodos (15)
    ├── Items (6)
    │   ├── TryAddItem()
    │   ├── RemoveItem()
    │   ├── UseCurrentItem()
    │   ├── DropCurrentItem()
    │   ├── ExamineCurrentItem()
    │   └── SelectNext/Previous/Slot()
    │
    ├── Armas (4)
    │   ├── EquipWeapon()
    │   ├── UnequipWeapon()
    │   ├── SwapWeapons()
    │   └── GetEquippedWeapon()
    │
    ├── Munición (4)
    │   ├── AddAmmo()
    │   ├── RemoveAmmo()
    │   ├── GetAmmoCount()
    │   └── HasAmmo()
    │
    └── Privados (1)
        └── FindEmptySlot()
```

---

## 🎓 Conceptos Clave Usados en el Script

### 1. Pattern Matching (C# 7.0+)

```csharp
if (itemData is AmmoItemData ammoData)
{
    // ammoData ya está casteado
}
```

### 2. Null-Conditional Operator

```csharp
OnItemAdded?.Invoke(slot, item);
// Solo invoca si OnItemAdded != null
```

### 3. Expression-Bodied Members

```csharp
public bool IsFull => FindEmptySlot() == -1;
// Propiedad de una línea
```

### 4. Operador Ternario

```csharp
weapon = slot == Primary ? primaryWeapon : secondaryWeapon;
// if-else compacto
```

### 5. Operador Módulo (%)

```csharp
selectedIndex = (selectedIndex + 1) % MAX_SLOTS;
// Wrap-around: 5 → 0
```

---

## 💡 Flujo Típico Completo

```
1. Usuario recoge item del mundo
   ↓
2. PickupItem.Interact()
   → inventory.TryAddItem(greenHerbItem)
   ↓
3. InventorySystem.TryAddItem()
   → Valida null
   → ¿Es AmmoItemData? NO
   → ¿Es stackable? SÍ
   → ¿Existe en slot con espacio? SÍ (slot 0, qty=2)
   → items[0].quantity++ → 3
   → OnItemAdded?.Invoke(0, items[0])
   ↓
4. InventoryDebugger escucha OnItemAdded
   → Actualiza UI: "Green Herb x3"
   ↓
5. Usuario presiona E (Use)
   → InventoryUIController.ExecuteContextAction(Use)
   → inventory.UseCurrentItem()
   ↓
6. InventorySystem.UseCurrentItem()
   → CurrentItem != null
   → is IUsable? SÍ
   → CanUse(player)? SÍ (vida no llena)
   → itemData.Use(player)
   ↓
7. ConsumableItemData.Use()
   → health.Heal(10)
   ↓
8. InventorySystem continúa
   → OnItemUsed?.Invoke(CurrentItem)
   → RemoveOnUse? SÍ
   → RemoveItem(0, 1)
   ↓
9. InventorySystem.RemoveItem()
   → items[0].quantity-- → 2
   → OnItemRemoved?.Invoke(0, items[0])
   ↓
10. UI actualiza: "Green Herb x2"
```

---

## ✅ Conclusión

**Total de líneas analizadas:** 311

**El script hace:**
- ✅ Gestiona 6 slots de inventario
- ✅ Stacking automático de items
- ✅ Sistema de munición separado
- ✅ Equipamiento de 2 armas
- ✅ Eventos para desacoplamiento
- ✅ Validaciones robustas

**Patrones de diseño aplicados:**
- Observer Pattern (eventos)
- Strategy Pattern (ItemData.Use())
- Separation of Concerns

**¡Script completo explicado línea por línea!** 🎮✨
