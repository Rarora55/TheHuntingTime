# 📖 InventoryUIController.cs - Explicación Línea por Línea

**Ubicación:** `/Assets/Scripts/Inventory/UI/InventoryUIController.cs`  
**Responsabilidad:** Controlador central de la UI del inventario, gestiona estados, navegación y menú contextual.

---

## 📦 Sección 1: Imports y Namespace (Líneas 1-6)

```csharp
1: using System;
2: using System.Collections.Generic;
3: using UnityEngine;
```

**Línea 1:** `using System;`  
Importa el namespace base de .NET que contiene tipos fundamentales como `Action<T>` (usado para eventos).

**Línea 2:** `using System.Collections.Generic;`  
Importa colecciones genéricas. Necesario para `List<ItemContextAction>`.

**Línea 3:** `using UnityEngine;`  
Importa el namespace de Unity con clases como `MonoBehaviour`, `GameObject`, `Debug`, etc.

```csharp
5: namespace TheHunt.Inventory
6: {
```

**Línea 5:** Define el namespace del proyecto para organizar el código y evitar colisiones de nombres.

---

## 🏗️ Sección 2: Declaración de Clase (Línea 7)

```csharp
7:     public class InventoryUIController : MonoBehaviour
```

**Clase pública** que hereda de `MonoBehaviour` (puede ser componente de Unity).

**Responsabilidades:**
- Gestiona estados del inventario (Closed, Open, ContextMenu)
- Coordina navegación entre slots
- Gestiona apertura/cierre del menú contextual
- Ejecuta acciones contextuales (Use, Examine, Drop, etc.)
- Controla pausa del juego (`Time.timeScale`)

---

## 🔧 Sección 3: Referencias (Líneas 9-10)

```csharp
9:         [Header("References")]
10:         [SerializeField] private InventorySystem inventorySystem;
```

**Línea 9:** `[Header("References")]`  
Decorador que crea una sección visual en el Inspector de Unity.

**Línea 10:** `private InventorySystem inventorySystem;`  
Referencia al sistema de inventario (backend). `[SerializeField]` permite asignarla desde el Inspector.

**Uso:**
- Llamar a métodos del sistema: `inventorySystem.SelectNext()`
- Obtener datos: `inventorySystem.CurrentItem`
- Ejecutar acciones: `inventorySystem.UseCurrentItem()`

---

## 📊 Sección 4: Estado (Líneas 12-15)

```csharp
12:         [Header("State")]
13:         private InventoryState currentState = InventoryState.Closed;
14:         private int contextMenuIndex = 0;
15:         private List<ItemContextAction> availableActions = new List<ItemContextAction>();
```

**Línea 13:** `currentState`  
Estado actual del inventario. Valores posibles:
- `Closed` → Inventario cerrado (juego en marcha)
- `Open` → Inventario abierto (navegando entre slots)
- `ContextMenu` → Menú de acciones abierto (navegando opciones)

**Línea 14:** `contextMenuIndex`  
Índice de la opción seleccionada en el menú contextual (0, 1, 2...).

**Línea 15:** `availableActions`  
Lista dinámica de acciones disponibles para el item actual:
- `Use` → Solo si item implementa `IUsable` y `CanUse()` es `true`
- `Examine` → Solo si `CanBeExamined` es `true`
- `Drop` → Siempre disponible
- `EquipPrimary/Secondary` → Solo para armas

---

## 🔍 Sección 5: Propiedades Públicas (Líneas 17-21)

```csharp
17:         public InventoryState CurrentState => currentState;
18:         public bool IsOpen => currentState != InventoryState.Closed;
19:         public bool IsInContextMenu => currentState == InventoryState.ContextMenu;
20:         public int ContextMenuIndex => contextMenuIndex;
21:         public List<ItemContextAction> AvailableActions => availableActions;
```

**Propiedades de solo lectura** (Expression-bodied properties).

**Línea 17:** Expone el estado actual.  
**Línea 18:** Atajos booleanos para verificar si el inventario está abierto.  
**Línea 19:** Verifica si estamos en el menú contextual.  
**Línea 20:** Índice de la opción seleccionada en el menú.  
**Línea 21:** Lista de acciones disponibles (para que `ContextMenuUI` las muestre).

---

## 📡 Sección 6: Eventos (Líneas 23-26)

```csharp
23:         public event Action<InventoryState> OnStateChanged;
24:         public event Action<List<ItemContextAction>> OnContextMenuOpened;
25:         public event Action OnContextMenuClosed;
26:         public event Action<int> OnContextMenuSelectionChanged;
```

**Eventos públicos** que otros scripts pueden escuchar.

**Línea 23:** `OnStateChanged`  
Se dispara cuando cambia el estado (Closed → Open → ContextMenu).  
**Subscribers:** `InventoryPanelUI` (muestra/oculta panel).

**Línea 24:** `OnContextMenuOpened`  
Se dispara al abrir el menú contextual, pasando la lista de acciones.  
**Subscribers:** `ContextMenuUI` (crea opciones en la UI).

**Línea 25:** `OnContextMenuClosed`  
Se dispara al cerrar el menú contextual.  
**Subscribers:** `ContextMenuUI` (limpia opciones).

**Línea 26:** `OnContextMenuSelectionChanged`  
Se dispara al cambiar la opción seleccionada (arriba/abajo en el menú).  
**Subscribers:** `ContextMenuUI` (actualiza highlight).

---

## 🏁 Sección 7: Awake (Líneas 28-32)

```csharp
28:         private void Awake()
29:         {
30:             if (inventorySystem == null)
31:                 inventorySystem = GetComponent<InventorySystem>();
32:         }
```

**Método de Unity** llamado al inicializar el script (antes de `Start`).

**Líneas 30-31:** Auto-referencia  
Si `inventorySystem` no está asignada en el Inspector, busca el componente en el mismo GameObject.

---

## 🔄 Sección 8: Toggle Inventario (Líneas 34-44)

```csharp
34:         public void ToggleInventory()
35:         {
36:             if (currentState == InventoryState.Closed)
37:             {\n38:                 OpenInventory();
39:             }
40:             else if (currentState == InventoryState.Open)
41:             {
42:                 CloseInventory();
43:             }
44:         }
```

**Método público** llamado por `PlayerInputHandler` al presionar Tab.

**Lógica:**
- Si está cerrado → Abre
- Si está abierto → Cierra
- Si está en menú contextual → No hace nada (primero cierra el menú con Escape)

---

## ✅ Sección 9: Abrir Inventario (Líneas 46-53)

```csharp
46:         public void OpenInventory()
47:         {
48:             if (currentState != InventoryState.Closed)
49:                 return;
50: 
51:             SetState(InventoryState.Open);
52:             Debug.Log("<color=cyan>[INVENTORY UI] Inventory opened</color>");
53:         }
```

**Método público** que abre el inventario.

**Línea 48-49:** Guarda de seguridad  
Solo abre si está cerrado (previene llamadas duplicadas).

**Línea 51:** Cambia el estado a `Open`  
Dispara evento `OnStateChanged` → `InventoryPanelUI` muestra el panel.

---

## ❌ Sección 10: Cerrar Inventario (Líneas 55-67)

```csharp
55:         public void CloseInventory()
56:         {
57:             if (currentState == InventoryState.Closed)
58:                 return;
59: 
60:             if (currentState == InventoryState.ContextMenu)
61:             {
62:                 CloseContextMenu();
63:             }
64: 
65:             SetState(InventoryState.Closed);
66:             Debug.Log("<color=cyan>[INVENTORY UI] Inventory closed</color>");
67:         }
```

**Método público** que cierra el inventario.

**Líneas 57-58:** Guarda de seguridad  
Solo cierra si está abierto.

**Líneas 60-63:** Limpieza  
Si el menú contextual está abierto, primero lo cierra.

**Línea 65:** Cambia el estado a `Closed`  
Dispara evento → `InventoryPanelUI` oculta el panel → `Time.timeScale = 1f` (reactiva el juego).

---

## ⬅️➡️ Sección 11: Navegación de Slots (Líneas 69-82)

```csharp
69:         public void NavigateInventory(float direction)
70:         {
71:             if (currentState != InventoryState.Open)
72:                 return;
73: 
74:             if (direction > 0)
75:             {
76:                 inventorySystem.SelectNext();
77:             }
78:             else if (direction < 0)
79:             {
80:                 inventorySystem.SelectPrevious();
81:             }
82:         }
```

**Método público** llamado por `PlayerInputHandler` con input de flechas.

**Parámetro:** `direction`  
- `> 0` → Right Arrow → Siguiente slot
- `< 0` → Left Arrow → Slot anterior

**Línea 71-72:** Solo navega si está en estado `Open`  
Si estamos en `ContextMenu`, este método no hace nada (usa `NavigateContextMenu` en su lugar).

**Lógica:**
1. Input detectado → `PlayerInputHandler.OnNavigate()`
2. Llama a `NavigateInventory(1f)` o `NavigateInventory(-1f)`
3. Delega al sistema → `inventorySystem.SelectNext()` o `SelectPrevious()`
4. Sistema dispara evento → `InventoryPanelUI` actualiza carrusel

---

## 🔘 Sección 12: Interacción (Líneas 84-94)

```csharp
84:         public void InteractWithCurrentItem()
85:         {
86:             if (currentState == InventoryState.Open)
87:             {
88:                 OpenContextMenu();
89:             }
90:             else if (currentState == InventoryState.ContextMenu)
91:             {
92:                 ExecuteContextAction();
93:             }
94:         }
```

**Método público** llamado al presionar E.

**Comportamiento contextual:**
- Si estado = `Open` → Abre menú contextual
- Si estado = `ContextMenu` → Ejecuta la acción seleccionada

**Flujo:**
```
Usuario presiona E
  ↓
Estado = Open → OpenContextMenu() → Muestra opciones (Use, Examine, Drop)
  ↓
Usuario navega y presiona E
  ↓
Estado = ContextMenu → ExecuteContextAction() → Ejecuta "Use" / "Examine" / etc.
```

---

## 🚫 Sección 13: Cancelación (Líneas 96-106)

```csharp
96:         public void CancelCurrentAction()
97:         {
98:             if (currentState == InventoryState.ContextMenu)
99:             {
100:                 CloseContextMenu();
101:             }
102:             else if (currentState == InventoryState.Open)
103:             {
104:                 CloseInventory();
105:             }
106:         }
```

**Método público** llamado al presionar Escape.

**Comportamiento contextual:**
- Si estado = `ContextMenu` → Cierra menú contextual (vuelve a `Open`)
- Si estado = `Open` → Cierra inventario (vuelve a `Closed`)

**Flujo de cancelación:**
```
Estado: ContextMenu
Presiona Escape → CloseContextMenu() → Estado: Open

Estado: Open
Presiona Escape → CloseInventory() → Estado: Closed
```

---

## 📋 Sección 14: Abrir Menú Contextual (Líneas 108-148)

```csharp
108:         private void OpenContextMenu()
109:         {
110:             ItemInstance currentItem = inventorySystem.CurrentItem;
111: 
112:             if (currentItem == null)
113:             {
114:                 Debug.Log("<color=yellow>[INVENTORY UI] No item selected</color>");
115:                 return;
116:             }
```

**Línea 110:** Obtiene el item actualmente seleccionado.

**Líneas 112-116:** Guarda de seguridad  
Si el slot está vacío, no abre el menú.

```csharp
118:             availableActions.Clear();
119:             contextMenuIndex = 0;
```

**Línea 118:** Limpia la lista de acciones anteriores.  
**Línea 119:** Resetea la selección a la primera opción.

```csharp
121:             if (currentItem.itemData is IUsable usable)
122:             {
123:                 if (usable.CanUse(gameObject))
124:                 {
125:                     availableActions.Add(ItemContextAction.Use);
126:                 }
127:             }
```

**Líneas 121-127:** Verifica si el item es usable  
- `is IUsable` → Pattern matching (C# 7.0)
- Si implementa `IUsable` Y `CanUse()` retorna `true` → Añade "Use"

**Ejemplo:**
- Health Potion con HP lleno → `CanUse()` retorna `false` → "Use" NO aparece
- Health Potion con HP < Max → `CanUse()` retorna `true` → "Use" SÍ aparece

```csharp
129:             if (currentItem.itemData.CanBeExamined)
130:             {
131:                 availableActions.Add(ItemContextAction.Examine);
132:             }
```

**Líneas 129-132:** Añade "Examine"  
Si `CanBeExamined = true` en el `ItemData`.

```csharp
134:             if (currentItem.itemData is WeaponItemData)
135:             {
136:                 availableActions.Add(ItemContextAction.EquipPrimary);
137:                 availableActions.Add(ItemContextAction.EquipSecondary);
138:             }
```

**Líneas 134-138:** Añade "Equip Primary" y "Equip Secondary"  
Solo si el item es de tipo `WeaponItemData`.

```csharp
140:             availableActions.Add(ItemContextAction.Drop);
```

**Línea 140:** "Drop" siempre disponible  
Todos los items pueden ser dropeados.

```csharp
142:             if (availableActions.Count > 0)
143:             {
144:                 SetState(InventoryState.ContextMenu);
145:                 OnContextMenuOpened?.Invoke(availableActions);
146:                 Debug.Log($"<color=cyan>[INVENTORY UI] Context menu opened with {availableActions.Count} options</color>");
147:             }
148:         }
```

**Líneas 142-147:** Abre el menú si hay acciones  
1. Cambia estado a `ContextMenu`
2. Dispara evento `OnContextMenuOpened` → `ContextMenuUI` crea opciones
3. Logea cuántas opciones hay

---

## 🚪 Sección 15: Cerrar Menú Contextual (Líneas 150-157)

```csharp
150:         private void CloseContextMenu()
151:         {
152:             availableActions.Clear();
153:             contextMenuIndex = 0;
154:             SetState(InventoryState.Open);
155:             OnContextMenuClosed?.Invoke();
156:             Debug.Log("<color=cyan>[INVENTORY UI] Context menu closed</color>");
157:         }
```

**Método privado** que cierra el menú contextual.

**Línea 152:** Limpia la lista de acciones.  
**Línea 153:** Resetea el índice de selección.  
**Línea 154:** Vuelve al estado `Open`.  
**Línea 155:** Dispara evento → `ContextMenuUI` oculta el menú y limpia opciones.

---

## ⬆️⬇️ Sección 16: Navegación del Menú (Líneas 159-182)

```csharp
159:         public void NavigateContextMenu(float direction)
160:         {
161:             if (currentState != InventoryState.ContextMenu || availableActions.Count == 0)
162:                 return;
163: 
164:             int oldIndex = contextMenuIndex;
165: 
166:             if (direction > 0)
167:             {
168:                 contextMenuIndex = (contextMenuIndex + 1) % availableActions.Count;
169:             }
170:             else if (direction < 0)
171:             {
172:                 contextMenuIndex--;
173:                 if (contextMenuIndex < 0)
174:                     contextMenuIndex = availableActions.Count - 1;
175:             }
176: 
177:             if (oldIndex != contextMenuIndex)
178:             {
179:                 OnContextMenuSelectionChanged?.Invoke(contextMenuIndex);
180:                 Debug.Log($"<color=cyan>[INVENTORY UI] Context menu selection: {availableActions[contextMenuIndex]}</color>");
181:             }
182:         }
```

**Método público** llamado por input de flechas cuando estás en el menú contextual.

**Línea 161-162:** Guarda de seguridad  
Solo navega si estamos en `ContextMenu` y hay opciones.

**Líneas 166-175:** Navegación circular  
- **Down Arrow** (`direction > 0`): Siguiente opción con wrap-around (módulo)
- **Up Arrow** (`direction < 0`): Opción anterior con wrap-around

**Ejemplo con 3 opciones:**
```
Índice 0 (Use) → Down → Índice 1 (Examine)
Índice 2 (Drop) → Down → Índice 0 (Use) [circular]
Índice 0 (Use) → Up → Índice 2 (Drop) [circular]
```

**Líneas 177-181:** Notifica el cambio  
Solo si el índice cambió realmente → Dispara evento → `ContextMenuUI` actualiza highlight.

---

## ⚡ Sección 17: Ejecutar Acción (Líneas 184-229)

```csharp
184:         private void ExecuteContextAction()
185:         {
186:             if (availableActions.Count == 0 || contextMenuIndex < 0 || contextMenuIndex >= availableActions.Count)
187:                 return;
188: 
189:             ItemContextAction action = availableActions[contextMenuIndex];
190:             ItemInstance currentItem = inventorySystem.CurrentItem;
191: 
192:             if (currentItem == null)
193:                 return;
194: 
195:             Debug.Log($"<color=green>[INVENTORY UI] Executing action: {action} on {currentItem.itemData.ItemName}</color>");
```

**Líneas 186-187:** Validación  
Verifica que el índice sea válido.

**Línea 189:** Obtiene la acción seleccionada.  
**Línea 190:** Obtiene el item actual.  
**Línea 195:** Logea la acción que se va a ejecutar.

```csharp
197:             switch (action)
198:             {
199:                 case ItemContextAction.Use:
200:                     inventorySystem.UseCurrentItem();
201:                     CloseContextMenu();
202:                     break;
```

**Caso Use:**  
Llama a `UseCurrentItem()` del sistema → Item ejecuta su efecto → Cierra menú.

```csharp
204:                 case ItemContextAction.Examine:
205:                     inventorySystem.ExamineCurrentItem();
206:                     break;
```

**Caso Examine:**  
Llama a `ExamineCurrentItem()` → Muestra descripción → Menú permanece abierto.

```csharp
208:                 case ItemContextAction.Drop:
209:                     inventorySystem.DropCurrentItem();
210:                     CloseContextMenu();
211:                     break;
```

**Caso Drop:**  
Dropea el item → Cierra menú.

```csharp
213:                 case ItemContextAction.EquipPrimary:
214:                     if (currentItem.itemData is WeaponItemData weapon)
215:                     {
216:                         inventorySystem.EquipWeapon(weapon, EquipSlot.Primary);
217:                     }
218:                     CloseContextMenu();
219:                     break;
220: 
221:                 case ItemContextAction.EquipSecondary:
222:                     if (currentItem.itemData is WeaponItemData weaponSecondary)
223:                     {
224:                         inventorySystem.EquipWeapon(weaponSecondary, EquipSlot.Secondary);
225:                     }
226:                     CloseContextMenu();
227:                     break;
228:             }
229:         }
```

**Casos EquipPrimary/Secondary:**  
Verifica que sea arma → Equipa en el slot correspondiente → Cierra menú.

---

## 🔄 Sección 18: Cambio de Estado (Líneas 231-248)

```csharp
231:         private void SetState(InventoryState newState)
232:         {
233:             if (currentState == newState)
234:                 return;
235: 
236:             InventoryState oldState = currentState;
237:             currentState = newState;
238:             OnStateChanged?.Invoke(newState);
```

**Método privado** centralizado para cambiar el estado.

**Líneas 233-234:** Evita cambios redundantes.  
**Línea 237:** Actualiza el estado.  
**Línea 238:** Dispara evento → `InventoryPanelUI` muestra/oculta panel.

```csharp
240:             if (newState == InventoryState.Closed)
241:             {
242:                 Time.timeScale = 1f;
243:             }
244:             else if (oldState == InventoryState.Closed)
245:             {
246:                 Time.timeScale = 0f;
247:             }
248:         }
```

**Gestión de pausa del juego:**

**Líneas 240-243:** Al cerrar inventario  
Reactiva el juego (`Time.timeScale = 1f`).

**Líneas 244-247:** Al abrir inventario  
Pausa el juego (`Time.timeScale = 0f`).

**Nota:** `Time.unscaledDeltaTime` se usa en animaciones para que funcionen aunque el juego esté pausado.

---

## 📝 Sección 19: Nombres de Acciones (Líneas 250-261)

```csharp
250:         public string GetContextActionDisplayName(ItemContextAction action)
251:         {
252:             switch (action)
253:             {
254:                 case ItemContextAction.Use: return "Use";
255:                 case ItemContextAction.Examine: return "Examine";
256:                 case ItemContextAction.Drop: return "Drop";
257:                 case ItemContextAction.EquipPrimary: return "Equip Primary";
258:                 case ItemContextAction.EquipSecondary: return "Equip Secondary";
259:                 default: return action.ToString();
260:             }\n261:         }
```

**Método público** que convierte enum a texto legible para la UI.

**Uso:** `ContextMenuUI` llama a este método para obtener el texto de cada opción.

**Ejemplo:**
```csharp
ItemContextAction.Use → "Use"
ItemContextAction.EquipPrimary → "Equip Primary"
```

---

## 🎯 Flujo Completo de Uso

### 1. Abrir Inventario

```
Usuario presiona Tab
  ↓
PlayerInputHandler.OnToggleInventory()
  ↓
InventoryUIController.ToggleInventory()
  ↓
OpenInventory()
  ↓
SetState(InventoryState.Open)
  ↓
OnStateChanged → InventoryPanelUI.ShowInventory()
  ↓
Time.timeScale = 0f (pausa el juego)
```

### 2. Navegar entre Slots

```
Usuario presiona Right Arrow
  ↓
PlayerInputHandler.OnNavigate(1f)
  ↓
InventoryUIController.NavigateInventory(1f)
  ↓
inventorySystem.SelectNext()
  ↓
OnSelectionChanged → InventoryPanelUI actualiza carrusel
```

### 3. Abrir Menú Contextual

```
Usuario presiona E
  ↓
InteractWithCurrentItem() [estado = Open]
  ↓
OpenContextMenu()
  ↓
Construye lista de acciones:
  - Verifica IUsable → Añade "Use"
  - Verifica CanBeExamined → Añade "Examine"
  - Verifica WeaponItemData → Añade "Equip"
  - Siempre añade "Drop"
  ↓
SetState(InventoryState.ContextMenu)
  ↓
OnContextMenuOpened → ContextMenuUI crea opciones
```

### 4. Ejecutar Acción

```
Usuario navega con Up/Down
  ↓
NavigateContextMenu()
  ↓
OnContextMenuSelectionChanged → ContextMenuUI actualiza highlight
  ↓
Usuario presiona E
  ↓
InteractWithCurrentItem() [estado = ContextMenu]
  ↓
ExecuteContextAction()
  ↓
Switch según acción:
  Use → UseCurrentItem() → Cierra menú
  Examine → ExamineCurrentItem() → Menú abierto
  Drop → DropCurrentItem() → Cierra menú
```

---

## 📊 Diagrama de Estados

```
┌─────────────────────────────────────────────┐
│                                             │
│   CLOSED                                    │
│   Time.timeScale = 1f                       │
│   Panel oculto                              │
│                                             │
│   Input: Tab                                │
└──────────────┬──────────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────────┐
│                                             │
│   OPEN                                      │
│   Time.timeScale = 0f                       │
│   Panel visible                             │
│   Navegación: Arrow Left/Right              │
│                                             │
│   Input: E (en item)                        │
└──────────────┬──────────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────────┐
│                                             │
│   CONTEXT MENU                              │
│   Time.timeScale = 0f                       │
│   Menú de acciones visible                  │
│   Navegación: Arrow Up/Down                 │
│                                             │
│   Input: E (ejecuta acción)                 │
│   Input: Esc (vuelve a OPEN)                │
└─────────────────────────────────────────────┘
```

---

## ✅ Responsabilidades Clave

1. **Gestión de Estados:** Closed ↔ Open ↔ ContextMenu
2. **Navegación:** Delega al `InventorySystem` (SelectNext/Previous)
3. **Menú Contextual:** Construye lista dinámica de acciones según item
4. **Ejecución:** Switch-case que delega acciones al sistema
5. **Pausa:** Controla `Time.timeScale` según estado
6. **Eventos:** Notifica a la UI (`InventoryPanelUI`, `ContextMenuUI`)

---

## 🔗 Interacción con Otros Scripts

**Recibe input de:**
- `PlayerInputHandler` → Toggle, Navigate, Interact, Cancel

**Controla:**
- `InventorySystem` → SelectNext, UseItem, DropItem, etc.

**Notifica a:**
- `InventoryPanelUI` → OnStateChanged
- `ContextMenuUI` → OnContextMenuOpened, OnContextMenuClosed, OnContextMenuSelectionChanged

---

¡Este script es el **cerebro** de la UI del inventario! 🧠✨
