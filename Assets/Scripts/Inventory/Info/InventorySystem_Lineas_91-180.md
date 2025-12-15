# 🎒 InventorySystem - Línea por Línea (91-180)

**Archivo:** `/Assets/Scripts/Inventory/Core/InventorySystem.cs`

---

## 🗑️ Líneas 91-106: Método RemoveItem (Continuación)

```csharp
92:             ItemInstance item = items[slotIndex];
```
- Guarda referencia al item antes de modificarlo
- Necesario para el evento `OnItemRemoved`

```csharp
93:             item.quantity -= quantity;
```
- Reduce la cantidad del item
- Ejemplo: `quantity = 3`, `quantity -= 1` → `2`
- **Modifica directamente** porque `ItemInstance` es clase (referencia)

```csharp
95:             if (item.quantity <= 0)
```
- ¿Se acabaron todos los items?
- `<= 0`: También maneja casos de sobre-remoción

```csharp
96:             {
```

```csharp
97:                 items[slotIndex] = null;
```
- **Vacía el slot** completamente
- El slot queda disponible para nuevos items

```csharp
98:                 Debug.Log($"<color=orange>[INVENTORY] Removed {item.itemData.ItemName} completely from slot {slotIndex}</color>");
```
- Log naranja indicando remoción completa
- Usa el nombre del item antes de borrarlo

```csharp
99:             }
```

```csharp
100:             else
```
- Si aún quedan items en el stack

```csharp
101:             {
```

```csharp
102:                 Debug.Log($"<color=orange>[INVENTORY] Removed {quantity} {item.itemData.ItemName}. Remaining: {item.quantity}</color>");
```
- Log mostrando cantidad removida y restante
- Ejemplo: "Removed 2 Green Herb. Remaining: 4"

```csharp
103:             }
```

```csharp
105:             OnItemRemoved?.Invoke(slotIndex, item);
```
- Dispara evento **después** de modificar
- Pasa el item (con cantidad actualizada)
- UI puede usar esto para actualizar

```csharp
106:         }
```
- Fin del método `RemoveItem`

---

## 💊 Líneas 108-136: Método UseCurrentItem

```csharp
108:         public void UseCurrentItem()
```
- Método público para usar el item seleccionado
- No recibe parámetros (usa `selectedIndex`)

```csharp
109:         {
```

```csharp
110:             if (CurrentItem == null)
```
- `CurrentItem`: Propiedad que retorna `items[selectedIndex]`
- ¿El slot actual está vacío?

```csharp
111:             {
```

```csharp
112:                 Debug.Log("<color=yellow>[INVENTORY] No item selected</color>");
```
- Log amarillo (advertencia)

```csharp
113:                 return;
```
- Sale del método sin hacer nada

```csharp
114:             }
```

```csharp
116:             if (CurrentItem.itemData is IUsable usable)
```
- **Pattern matching** con interfaces
- `is IUsable`: ¿El item implementa la interfaz `IUsable`?
- `usable`: Variable con el cast si es verdadero
- Solo `ConsumableItemData` implementa `IUsable` actualmente

```csharp
117:             {
```

```csharp
118:                 if (!usable.CanUse(gameObject))
```
- `CanUse()`: Método de interfaz `IUsable`
- `gameObject`: El GameObject con este componente (el jugador)
- Ejemplo: `ConsumableItemData.CanUse()` verifica si vida no está llena

```csharp
119:                 {
```

```csharp
120:                     Debug.Log($"<color=yellow>[INVENTORY] Cannot use {CurrentItem.itemData.ItemName}</color>");
```
- Ejemplo: "Cannot use Green Herb" (si vida llena)

```csharp
121:                     return;
```
- Sale sin usar el item

```csharp
122:                 }
```

```csharp
124:                 CurrentItem.itemData.Use(gameObject);
```
- **Ejecuta la lógica de uso**
- Método abstracto de `ItemData`, implementado en cada tipo
- Ejemplo `ConsumableItemData.Use()`: `health.Heal(healAmount)`
- Pasa `gameObject` (el jugador) para acceder a componentes

```csharp
125:                 OnItemUsed?.Invoke(CurrentItem);
```
- Dispara evento **después** de usar
- Parámetro: El item usado
- UI puede mostrar animación, sonido, etc.

```csharp
127:                 if (usable.RemoveOnUse)
```
- `RemoveOnUse`: Propiedad de interfaz `IUsable`
- Consumibles típicamente tienen `true`
- Permite items reutilizables (ej: llave permanente)

```csharp
128:                 {
```

```csharp
129:                     RemoveItem(selectedIndex, 1);
```
- Remueve 1 unidad del slot actual
- Llama al método `RemoveItem()` (que dispara su propio evento)

```csharp
130:                 }
```

```csharp
131:             }
```

```csharp
132:             else
```
- Si el item NO implementa `IUsable`

```csharp
133:             {
```

```csharp
134:                 Debug.Log($"<color=yellow>[INVENTORY] {CurrentItem.itemData.ItemName} is not usable</color>");
```
- Ejemplo: "Pistol is not usable" (armas se equipan, no se usan)

```csharp
135:             }
```

```csharp
136:         }
```
- Fin del método `UseCurrentItem`

---

## 🗑️ Líneas 138-145: Método DropCurrentItem

```csharp
138:         public void DropCurrentItem()
```
- Método para "tirar" el item actual
- **Nota:** No instancia GameObject en el mundo, solo remueve

```csharp
139:         {
```

```csharp
140:             if (CurrentItem == null)
```
- Validación: ¿Hay item seleccionado?

```csharp
141:                 return;
```
- Sale si no hay item

```csharp
143:             Debug.Log($"<color=cyan>[INVENTORY] Dropped {CurrentItem.itemData.ItemName}</color>");
```
- Log cian
- **TODO:** Aquí se podría instanciar prefab en el mundo

```csharp
144:             RemoveItem(selectedIndex, 1);
```
- Remueve 1 unidad del slot actual
- Dispara `OnItemRemoved`

```csharp
145:         }
```

---

## 🔍 Líneas 147-159: Método ExamineCurrentItem

```csharp
147:         public void ExamineCurrentItem()
```
- Muestra texto de examen del item
- Usado en menús contextuales

```csharp
148:         {
```

```csharp
149:             if (CurrentItem == null)
```
- Validación

```csharp
150:                 return;
```

```csharp
152:             if (!CurrentItem.itemData.CanBeExamined)
```
- `CanBeExamined`: Propiedad bool de `ItemData`
- Algunos items pueden no tener texto de examen

```csharp
153:             {
```

```csharp
154:                 Debug.Log($"<color=yellow>[INVENTORY] {CurrentItem.itemData.ItemName} cannot be examined</color>");
```

```csharp
155:                 return;
```

```csharp
156:             }
```

```csharp
158:             Debug.Log($"<color=cyan>[EXAMINE] {CurrentItem.itemData.ItemName}\n{CurrentItem.itemData.ExaminationText}</color>");
```
- `\n`: Salto de línea
- `ExaminationText`: Propiedad string de `ItemData`
- Ejemplo:
  ```
  [EXAMINE] Green Herb
  A medicinal herb that restores health.
  ```

```csharp
159:         }
```

---

## ⏭️ Líneas 161-167: Método SelectNext

```csharp
161:         public void SelectNext()
```
- Navega al siguiente slot
- Con "wrap-around" (del 5 vuelve al 0)

```csharp
162:         {
```

```csharp
163:             int oldIndex = selectedIndex;
```
- Guarda índice anterior para el evento

```csharp
164:             selectedIndex = (selectedIndex + 1) % MAX_SLOTS;
```
- **Operador módulo** para wrap-around
- Ejemplos:
  - `selectedIndex = 0`: `(0 + 1) % 6 = 1`
  - `selectedIndex = 5`: `(5 + 1) % 6 = 0` ← Wrap!

```csharp
165:             OnSelectionChanged?.Invoke(oldIndex, selectedIndex);
```
- Dispara evento con índice viejo y nuevo
- UI puede actualizar highlight

```csharp
166:             Debug.Log($"<color=cyan>[INVENTORY] Selected slot {selectedIndex}</color>");
```

```csharp
167:         }
```

---

## ⏮️ Líneas 169-177: Método SelectPrevious

```csharp
169:         public void SelectPrevious()
```
- Navega al slot anterior

```csharp
170:         {
```

```csharp
171:             int oldIndex = selectedIndex;
```

```csharp
172:             selectedIndex--;
```
- Decrementa índice
- Puede volverse negativo (-1)

```csharp
173:             if (selectedIndex < 0)
```
- ¿Se salió del rango?

```csharp
174:                 selectedIndex = MAX_SLOTS - 1;
```
- Wrap al último slot (5)
- `MAX_SLOTS - 1 = 6 - 1 = 5`

```csharp
175:             OnSelectionChanged?.Invoke(oldIndex, selectedIndex);
```

```csharp
176:             Debug.Log($"<color=cyan>[INVENTORY] Selected slot {selectedIndex}</color>");
```

```csharp
177:         }
```

---

## 🎯 Líneas 179-187: Método SelectSlot

```csharp
179:         public void SelectSlot(int index)
```
- Selecciona slot directamente por índice
- Usado para: Hotkeys (1-6), click en UI

```csharp
180:         {
```

```csharp
181:             if (index < 0 || index >= MAX_SLOTS)
```
- Validación de rango [0, 5]

```csharp
182:                 return;
```
- Ignora índices inválidos

```csharp
184:             int oldIndex = selectedIndex;
```

```csharp
185:             selectedIndex = index;
```
- Asigna directamente

```csharp
186:             OnSelectionChanged?.Invoke(oldIndex, selectedIndex);
```
- **Nota:** No hay Debug.Log aquí (a diferencia de Next/Previous)

```csharp
187:         }
```

---

**Continúa en:** InventorySystem_Lineas_181-270.md
