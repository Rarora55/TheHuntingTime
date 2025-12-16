# 📖 InventoryPanelUI.cs - Explicación Línea por Línea

**Ubicación:** `/Assets/Scripts/Inventory/UI/InventoryPanelUI.cs`  
**Responsabilidad:** Gestiona el panel principal del inventario, slots y carrusel animado estilo Silent Hill.

---

## 📦 Sección 1: Imports y Namespace (Líneas 1-6)

```csharp
1: using UnityEngine;
2: using System.Collections;
3: using System.Collections.Generic;
4: 
5: namespace TheHunt.Inventory
6: {
```

**Línea 1:** `UnityEngine` → Clases base de Unity.  
**Línea 2:** `System.Collections` → Para coroutines (`IEnumerator`).  
**Línea 3:** `System.Collections.Generic` → Para `List<T>`, arrays, etc.

---

## 🏗️ Sección 2: Declaración de Clase (Línea 7-8)

```csharp
7:     public class InventoryPanelUI : MonoBehaviour
8:     {
```

**Clase pública** que gestiona la UI del panel de inventario.

**Responsabilidades:**
- Crear slots dinámicamente
- Actualizar slots cuando se añaden/eliminan items
- Gestionar carrusel (posición, animación, visibilidad)
- Mostrar/ocultar panel según estado
- Sincronizar highlight con selección

---

## 🔧 Sección 3: Referencias (Líneas 9-11)

```csharp
9:         [Header("References")]
10:         [SerializeField] private InventorySystem inventorySystem;
11:         [SerializeField] private InventoryUIController uiController;
```

**Línea 10:** `inventorySystem`  
Sistema de inventario (backend) para:
- Escuchar eventos (`OnItemAdded`, `OnItemRemoved`, `OnSelectionChanged`)
- Obtener datos (`Items`, `SelectedSlot`)

**Línea 11:** `uiController`  
Controlador de UI para:
- Escuchar cambios de estado (`OnStateChanged`)
- Mostrar/ocultar panel

---

## 📦 Sección 4: Configuración de Slots (Líneas 13-15)

```csharp
13:         [Header("Slot Settings")]
14:         [SerializeField] private Transform slotsContainer;
15:         [SerializeField] private GameObject slotPrefab;
```

**Línea 14:** `slotsContainer`  
Transform donde se instancian los slots (normalmente `SlotsContainer`).

**Línea 15:** `slotPrefab`  
Prefab de cada slot (debe tener componente `InventorySlotUI`).

---

## 🎨 Sección 5: Paneles (Línea 17-18)

```csharp
17:         [Header("Panels")]
18:         [SerializeField] private CanvasGroup canvasGroup;
```

**Línea 18:** `canvasGroup`  
Controla visibilidad del panel completo (`alpha`, `interactable`, `blocksRaycasts`).

---

## 🎠 Sección 6: Configuración de Carrusel (Líneas 20-24)

```csharp
20:         [Header("Carousel Settings")]
21:         [SerializeField] private int visibleSlots = 3;
22:         [SerializeField] private float slotSpacing = 220f;
23:         [SerializeField] private float transitionSpeed = 8f;
24:         [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
```

**Línea 21:** `visibleSlots`  
Cuántos slots visibles simultáneamente (3 = izquierdo, centro, derecho).

**Línea 22:** `slotSpacing`  
Distancia en píxeles entre slots (220px recomendado para slots de 200px).

**Línea 23:** `transitionSpeed`  
Velocidad de animación del carrusel (8 = rápido y fluido).

**Línea 24:** `transitionCurve`  
Curva de suavizado de la animación (`EaseInOut` = inicio lento, medio rápido, final lento).

---

## 📊 Sección 7: Variables Privadas (Líneas 26-29)

```csharp
26:         private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
27:         private int currentHighlightedSlot = 0;
28:         private Vector3[] targetPositions;
29:         private bool isAnimating = false;
```

**Línea 26:** `slotUIList`  
Lista de todos los slots creados (6 slots por defecto).

**Línea 27:** `currentHighlightedSlot`  
Índice del slot actualmente destacado (0-5).

**Línea 28:** `targetPositions`  
Array de posiciones objetivo para cada slot en el carrusel.

**Línea 29:** `isAnimating`  
Flag para evitar múltiples animaciones simultáneas.

---

## 🏁 Sección 8: Awake (Líneas 31-43)

```csharp
31:         private void Awake()
32:         {
33:             if (inventorySystem == null)
34:                 inventorySystem = GetComponent<InventorySystem>();
35: 
36:             if (uiController == null)
37:                 uiController = GetComponent<InventoryUIController>();
38: 
39:             if (canvasGroup == null)
40:                 canvasGroup = GetComponent<CanvasGroup>();
41: 
42:             CreateSlots();
43:         }
```

**Líneas 33-40:** Auto-referencias  
Busca componentes si no están asignados en el Inspector.

**Línea 42:** Crea los slots  
Llama a `CreateSlots()` para instanciar los 6 slots.

---

## 📡 Sección 9: Suscripción a Eventos (Líneas 45-73)

```csharp
45:         private void OnEnable()
46:         {
47:             if (inventorySystem != null)
48:             {
49:                 inventorySystem.OnItemAdded += OnItemAdded;
50:                 inventorySystem.OnItemRemoved += OnItemRemoved;
51:                 inventorySystem.OnSelectionChanged += OnSelectionChanged;
52:             }
53: 
54:             if (uiController != null)
55:             {
56:                 uiController.OnStateChanged += OnInventoryStateChanged;
57:             }
58:         }
59: 
60:         private void OnDisable()
61:         {
62:             if (inventorySystem != null)
63:             {
64:                 inventorySystem.OnItemAdded -= OnItemAdded;
65:                 inventorySystem.OnItemRemoved -= OnItemRemoved;
66:                 inventorySystem.OnSelectionChanged -= OnSelectionChanged;
67:             }
68: 
69:             if (uiController != null)
70:             {
71:                 uiController.OnStateChanged -= OnInventoryStateChanged;
72:             }
73:         }
```

**Eventos escuchados del InventorySystem:**
1. `OnItemAdded` → Actualizar slot cuando se añade item
2. `OnItemRemoved` → Actualizar slot cuando se elimina item
3. `OnSelectionChanged` → Mover carrusel y actualizar highlight

**Eventos escuchados del UIController:**
1. `OnStateChanged` → Mostrar/ocultar panel según estado

---

## 🔄 Sección 10: Start (Líneas 75-81)

```csharp
75:         private void Start()
76:         {
77:             RefreshAllSlots();
78:             InitializeCarouselPositions();
79:             UpdateCarouselPositions(currentHighlightedSlot, true);
80:             HideInventory();
81:         }
```

**Línea 77:** Sincroniza slots con datos del sistema.  
**Línea 78:** Inicializa array de posiciones objetivo.  
**Línea 79:** Posiciona slots inmediatamente (sin animación).  
**Línea 80:** Oculta el panel al inicio.

---

## 🔨 Sección 11: Crear Slots (Líneas 83-115)

```csharp
83:         private void CreateSlots()
84:         {
85:             if (slotsContainer == null || slotPrefab == null)
86:             {
87:                 Debug.LogError("[INVENTORY UI] Missing slots container or slot prefab!");
88:                 return;
89:             }
```

**Líneas 85-89:** Validación  
Verifica que existan el contenedor y el prefab.

```csharp
91:             for (int i = slotsContainer.childCount - 1; i >= 0; i--)
92:             {
93:                 Destroy(slotsContainer.GetChild(i).gameObject);
94:             }
95: 
96:             slotUIList.Clear();
```

**Líneas 91-94:** Limpia slots existentes  
Destruye cualquier hijo previo (útil si se llama múltiples veces).

**Línea 96:** Limpia la lista.

```csharp
98:             for (int i = 0; i < InventorySystem.MAX_SLOTS; i++)
99:             {
100:                 GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
101:                 InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
102: 
103:                 if (slotUI != null)
104:                 {
105:                     slotUI.Initialize(i);
106:                     slotUIList.Add(slotUI);
107:                 }
108:                 else
109:                 {
110:                     Debug.LogError($"[INVENTORY UI] Slot prefab missing InventorySlotUI component!");
111:                 }
112:             }
113: 
114:             Debug.Log($"<color=cyan>[INVENTORY UI] Created {slotUIList.Count} slots</color>");
115:         }
```

**Línea 98:** Crea 6 slots (MAX_SLOTS).

**Línea 100:** Instancia el prefab  
Como hijo del `slotsContainer`.

**Línea 101:** Obtiene el componente `InventorySlotUI`.

**Línea 105:** Inicializa el slot  
Asigna índice y limpia su contenido.

**Línea 106:** Añade a la lista.

**Resultado:**
```
SlotsContainer
  ├─ Slot 0 (InventorySlotUI)
  ├─ Slot 1 (InventorySlotUI)
  ├─ Slot 2 (InventorySlotUI)
  ├─ Slot 3 (InventorySlotUI)
  ├─ Slot 4 (InventorySlotUI)
  └─ Slot 5 (InventorySlotUI)
```

---

## 🔄 Sección 12: Refrescar Todos los Slots (Líneas 117-130)

```csharp
117:         private void RefreshAllSlots()
118:         {
119:             if (inventorySystem == null || slotUIList.Count == 0)
120:                 return;
121: 
122:             ItemInstance[] items = inventorySystem.Items;
123: 
124:             for (int i = 0; i < slotUIList.Count && i < items.Length; i++)
125:             {
126:                 slotUIList[i].UpdateSlot(items[i]);
127:             }
128: 
129:             UpdateHighlight(inventorySystem.SelectedSlot);
130:         }
```

**Método privado** que sincroniza todos los slots con el sistema.

**Línea 122:** Obtiene array de items del sistema.

**Líneas 124-127:** Actualiza cada slot  
Llama a `UpdateSlot()` en cada `InventorySlotUI`.

**Línea 129:** Actualiza highlight  
Marca el slot seleccionado.

**Cuándo se llama:**
- Al inicio (`Start`)
- Al abrir inventario (`ShowInventory`)

---

## ➕ Sección 13: Item Añadido (Líneas 132-139)

```csharp
132:         private void OnItemAdded(int slotIndex, ItemInstance item)
133:         {
134:             if (slotIndex >= 0 && slotIndex < slotUIList.Count)
135:             {
136:                 slotUIList[slotIndex].UpdateSlot(item);
137:                 Debug.Log($"<color=cyan>[INVENTORY UI] Updated slot {slotIndex}</color>");
138:             }
139:         }
```

**Método privado** llamado cuando se añade un item al inventario.

**Parámetros:**
- `slotIndex` → Índice del slot donde se añadió (0-5)
- `item` → ItemInstance añadido

**Acción:**  
Actualiza solo el slot específico (eficiente).

---

## ➖ Sección 14: Item Eliminado (Líneas 141-149)

```csharp
141:         private void OnItemRemoved(int slotIndex, ItemInstance item)
142:         {
143:             if (slotIndex >= 0 && slotIndex < slotUIList.Count)
144:             {
145:                 ItemInstance currentItem = inventorySystem.Items[slotIndex];
146:                 slotUIList[slotIndex].UpdateSlot(currentItem);
147:                 Debug.Log($"<color=cyan>[INVENTORY UI] Cleared/Updated slot {slotIndex}</color>");
148:             }
149:         }
```

**Método privado** llamado cuando se elimina un item.

**Línea 145:** Obtiene el item actual del slot  
(Puede ser `null` si quedó vacío, o el item restante si era stacked).

**Línea 146:** Actualiza el slot  
Si es `null`, el slot se limpia. Si es un item, se actualiza la cantidad.

---

## 🎯 Sección 15: Selección Cambiada (Líneas 151-155)

```csharp
151:         private void OnSelectionChanged(int previousSlot, int newSlot)
152:         {
153:             UpdateHighlight(newSlot);
154:             UpdateCarouselPositions(newSlot, false);
155:         }
```

**Método privado** llamado cuando el usuario navega entre slots.

**Parámetros:**
- `previousSlot` → Slot anterior (no se usa aquí)
- `newSlot` → Nuevo slot seleccionado (0-5)

**Acciones:**
1. **Línea 153:** Actualiza highlight (amarillo en nuevo slot)
2. **Línea 154:** Actualiza carrusel con animación (`false` = animado)

---

## 🌟 Sección 16: Actualizar Highlight (Líneas 157-168)

```csharp
157:         private void UpdateHighlight(int slotIndex)
158:         {
159:             for (int i = 0; i < slotUIList.Count; i++)
160:             {
161:                 if (i == slotIndex)
162:                     slotUIList[i].Highlight();
163:                 else
164:                     slotUIList[i].Unhighlight();
165:             }
166: 
167:             currentHighlightedSlot = slotIndex;
168:         }
```

**Método privado** que actualiza el highlight de los slots.

**Líneas 159-165:** Itera sobre todos los slots  
- Slot seleccionado → `Highlight()` (amarillo)
- Otros slots → `Unhighlight()` (blanco)

**Línea 167:** Guarda el índice actual.

---

## 🔄 Sección 17: Cambio de Estado (Líneas 170-186)

```csharp
170:         private void OnInventoryStateChanged(InventoryState newState)
171:         {
172:             switch (newState)
173:             {
174:                 case InventoryState.Open:
175:                     ShowInventory();
176:                     break;
177: 
178:                 case InventoryState.Closed:
179:                     HideInventory();
180:                     break;
181: 
182:                 case InventoryState.ContextMenu:
183:                     Debug.Log("<color=cyan>[INVENTORY UI] Context menu state</color>");
184:                     break;
185:             }
186:         }
```

**Método privado** llamado cuando cambia el estado del inventario.

**Estado Open:** Muestra el panel.  
**Estado Closed:** Oculta el panel.  
**Estado ContextMenu:** Panel permanece visible (solo logea).

---

## 👁️ Sección 18: Mostrar Inventario (Líneas 188-199)

```csharp
188:         private void ShowInventory()
189:         {
190:             if (canvasGroup != null)
191:             {
192:                 canvasGroup.alpha = 1f;
193:                 canvasGroup.interactable = true;
194:                 canvasGroup.blocksRaycasts = true;
195:                 RefreshAllSlots();
196:             }
197: 
198:             Debug.Log("<color=cyan>[INVENTORY UI] Inventory panel shown</color>");
199:         }
```

**Método privado** que muestra el panel.

**Líneas 192-194:** Activa CanvasGroup  
- `alpha = 1f` → Visible
- `interactable = true` → Puede recibir input
- `blocksRaycasts = true` → Bloquea clicks detrás

**Línea 195:** Refresca slots  
Sincroniza con datos actuales del sistema.

---

## 🙈 Sección 19: Ocultar Inventario (Líneas 201-211)

```csharp
201:         private void HideInventory()
202:         {
203:             if (canvasGroup != null)
204:             {
205:                 canvasGroup.alpha = 0f;
206:                 canvasGroup.interactable = false;
207:                 canvasGroup.blocksRaycasts = false;
208:             }
209: 
210:             Debug.Log("<color=cyan>[INVENTORY UI] Inventory panel hidden</color>");
211:         }
```

**Método privado** que oculta el panel.

**Líneas 205-207:** Desactiva CanvasGroup  
- `alpha = 0f` → Invisible
- `interactable = false` → No recibe input
- `blocksRaycasts = false` → No bloquea clicks

---

## 🎠 Sección 20: Inicializar Carrusel (Líneas 213-221)

```csharp
213:         private void InitializeCarouselPositions()
214:         {
215:             targetPositions = new Vector3[slotUIList.Count];
216: 
217:             for (int i = 0; i < slotUIList.Count; i++)
218:             {
219:                 targetPositions[i] = Vector3.zero;
220:             }
221:         }
```

**Método privado** que inicializa el array de posiciones objetivo.

**Línea 215:** Crea array de 6 elementos (uno por slot).

**Líneas 217-220:** Inicializa a `Vector3.zero`  
Será actualizado después en `UpdateCarouselPositions()`.

---

## 📍 Sección 21: Actualizar Posiciones del Carrusel (Líneas 223-256)

```csharp
223:         private void UpdateCarouselPositions(int centerSlot, bool immediate)
224:         {
225:             if (slotUIList.Count == 0)
226:                 return;
227: 
228:             int halfVisible = visibleSlots / 2;
```

**Parámetros:**
- `centerSlot` → Índice del slot que debe estar centrado (0-5)
- `immediate` → Si es `true`, posiciona instantáneamente; si es `false`, anima

**Línea 228:** Calcula cuántos slots a cada lado  
`visibleSlots = 3` → `halfVisible = 1` (1 a la izquierda, 1 a la derecha).

```csharp
230:             for (int i = 0; i < slotUIList.Count; i++)
231:             {
232:                 int offset = i - centerSlot;
233:                 
234:                 float xPosition = -offset * slotSpacing;
235:                 targetPositions[i] = new Vector3(xPosition, 0f, 0f);
```

**Línea 232:** Calcula offset del slot  
`offset` = distancia al centro (negativo = izquierda, positivo = derecha).

**Línea 234:** Calcula posición X  
`-offset * spacing` → Invierte dirección (slot mayor = izquierda).

**Ejemplo con centerSlot = 1:**
```
Slot 0: offset = 0-1 = -1 → xPosition = -(-1)*220 = +220  (derecha)
Slot 1: offset = 1-1 =  0 → xPosition = -(0)*220  =    0  (centro)
Slot 2: offset = 2-1 = +1 → xPosition = -(1)*220  = -220  (izquierda)
```

```csharp
237:                 bool shouldBeVisible = Mathf.Abs(offset) <= halfVisible;
```

**Línea 237:** Determina visibilidad  
Solo visible si está dentro del rango (|offset| ≤ 1 con visibleSlots=3).

**Ejemplo:**
```
Slot 0: |offset| = 1 ≤ 1 → visible ✓
Slot 1: |offset| = 0 ≤ 1 → visible ✓
Slot 2: |offset| = 1 ≤ 1 → visible ✓
Slot 3: |offset| = 2 ≤ 1 → invisible ✗
```

```csharp
239:                 RectTransform rectTransform = slotUIList[i].GetComponent<RectTransform>();
240:                 if (rectTransform != null)
241:                 {
242:                     if (immediate)
243:                     {
244:                         rectTransform.anchoredPosition = targetPositions[i];
245:                         SetSlotVisibility(slotUIList[i], shouldBeVisible, immediate);
246:                     }
247:                     else
248:                     {
249:                         if (!isAnimating)
250:                         {
251:                             StartCoroutine(AnimateCarousel());
252:                         }
253:                     }
254:                 }
255:             }
256:         }
```

**Líneas 242-246:** Modo inmediato  
Posiciona y ajusta visibilidad instantáneamente (usado en `Start`).

**Líneas 247-253:** Modo animado  
Solo inicia animación si no hay una en curso.

---

## 🎬 Sección 22: Animar Carrusel (Líneas 258-309)

```csharp
258:         private IEnumerator AnimateCarousel()
259:         {
260:             isAnimating = true;
261:             
262:             Vector3[] startPositions = new Vector3[slotUIList.Count];
263:             for (int i = 0; i < slotUIList.Count; i++)
264:             {
265:                 RectTransform rt = slotUIList[i].GetComponent<RectTransform>();
266:                 if (rt != null)
267:                 {
268:                     startPositions[i] = rt.anchoredPosition;
269:                 }
270:             }
```

**Línea 260:** Marca como animando.

**Líneas 262-270:** Guarda posiciones iniciales  
Necesario para interpolar desde la posición actual.

```csharp
272:             float elapsed = 0f;
273:             float duration = 1f / transitionSpeed;
```

**Línea 273:** Calcula duración  
`transitionSpeed = 8` → `duration = 0.125s` (muy rápido).

```csharp
275:             while (elapsed < duration)
276:             {
277:                 elapsed += Time.unscaledDeltaTime;
278:                 float t = Mathf.Clamp01(elapsed / duration);
279:                 float curveValue = transitionCurve.Evaluate(t);
280: 
281:                 int halfVisible = visibleSlots / 2;
282: 
283:                 for (int i = 0; i < slotUIList.Count; i++)
284:                 {
285:                     RectTransform rt = slotUIList[i].GetComponent<RectTransform>();
286:                     if (rt != null)
287:                     {
288:                         rt.anchoredPosition = Vector3.Lerp(startPositions[i], targetPositions[i], curveValue);
289: 
290:                         int offset = Mathf.Abs(i - currentHighlightedSlot);
291:                         bool shouldBeVisible = offset <= halfVisible;
292:                         SetSlotVisibility(slotUIList[i], shouldBeVisible, false);
293:                     }
294:                 }
295: 
296:                 yield return null;
297:             }
```

**Línea 277:** `Time.unscaledDeltaTime`  
Funciona aunque el juego esté pausado.

**Línea 279:** Aplica curva de suavizado  
`EaseInOut` = movimiento suave.

**Línea 288:** Interpola posición  
De posición actual a posición objetivo usando curva.

**Línea 292:** Ajusta visibilidad durante animación  
Fade in/out de slots según se acercan/alejan del centro.

```csharp
299:             for (int i = 0; i < slotUIList.Count; i++)
300:             {
301:                 RectTransform rt = slotUIList[i].GetComponent<RectTransform>();
302:                 if (rt != null)
303:                 {
304:                     rt.anchoredPosition = targetPositions[i];
305:                 }
306:             }
307: 
308:             isAnimating = false;
309:         }
```

**Líneas 299-306:** Asegura posiciones finales exactas.

**Línea 308:** Libera flag de animación.

---

## 👁️ Sección 23: Visibilidad de Slot (Líneas 311-331)

```csharp
311:         private void SetSlotVisibility(InventorySlotUI slot, bool visible, bool immediate)
312:         {
313:             CanvasGroup slotCanvas = slot.GetComponent<CanvasGroup>();
314:             if (slotCanvas == null)
315:             {
316:                 slotCanvas = slot.gameObject.AddComponent<CanvasGroup>();
317:             }
```

**Líneas 313-317:** Obtiene o crea CanvasGroup  
Si el slot no tiene CanvasGroup, lo añade automáticamente.

```csharp
319:             if (immediate)
320:             {
321:                 slotCanvas.alpha = visible ? 1f : 0f;
322:             }
323:             else
324:             {
325:                 float targetAlpha = visible ? 1f : 0f;
326:                 slotCanvas.alpha = Mathf.Lerp(slotCanvas.alpha, targetAlpha, Time.unscaledDeltaTime * transitionSpeed);
327:             }
```

**Líneas 319-322:** Modo inmediato  
Asigna alpha directamente (0 o 1).

**Líneas 323-327:** Modo animado  
Interpola alpha gradualmente para efecto fade.

```csharp
329:             slotCanvas.interactable = visible;
330:             slotCanvas.blocksRaycasts = visible;
331:         }
```

**Líneas 329-330:** Ajusta interacción  
Slots invisibles no reciben input ni bloquean raycasts.

---

## 🎯 Flujo Completo de Carrusel

### Inicio (Start)

```
Start()
  ↓
InitializeCarouselPositions()
  └─ targetPositions = [Vector3.zero × 6]
  ↓
UpdateCarouselPositions(centerSlot: 0, immediate: true)
  ↓
  Slot 0: offset =  0 → xPos =    0 → visible ✓ (centro)
  Slot 1: offset = +1 → xPos = -220 → visible ✓ (izquierda)
  Slot 2: offset = +2 → xPos = -440 → invisible ✗
  Slot 3: offset = +3 → xPos = -660 → invisible ✗
  ...
  ↓
Posiciona instantáneamente (sin animación)
```

### Usuario Navega a la Derecha

```
Input: Right Arrow
  ↓
InventorySystem.SelectNext()  → selectedSlot = 1
  ↓
OnSelectionChanged(previousSlot: 0, newSlot: 1)
  ↓
UpdateHighlight(1)
  ├─ slotUIList[0].Unhighlight() (blanco)
  └─ slotUIList[1].Highlight()   (amarillo)
  ↓
UpdateCarouselPositions(centerSlot: 1, immediate: false)
  ↓
  Recalcula posiciones:
    Slot 0: offset = -1 → xPos = +220 → visible ✓ (derecha)
    Slot 1: offset =  0 → xPos =    0 → visible ✓ (centro)
    Slot 2: offset = +1 → xPos = -220 → visible ✓ (izquierda)
    Slot 3: offset = +2 → xPos = -440 → invisible ✗
  ↓
StartCoroutine(AnimateCarousel())
  ↓
  Anima de posiciones actuales → posiciones objetivo
  Duration: 0.125s (transitionSpeed = 8)
  Curva: EaseInOut (suave)
  ↓
Carrusel se desliza a la izquierda (slots se mueven)
```

### Resultado Visual

```
ANTES (centerSlot = 0):
┌──────────────────────────────┐
│ [Slot 0] [Slot 1] [Slot 2]  │
│     ▲                        │
└──────────────────────────────┘

DESPUÉS (centerSlot = 1):
┌──────────────────────────────┐
│ [Slot 0] [Slot 1] [Slot 2]  │  ← Carrusel se movió a la izquierda
│             ▲                │
└──────────────────────────────┘
```

---

## ✅ Responsabilidades Clave

1. **Creación de Slots:** Instancia 6 slots dinámicamente
2. **Sincronización:** Actualiza slots cuando cambian items
3. **Carrusel:** Calcula posiciones y anima transiciones
4. **Visibilidad:** Muestra solo 3 slots a la vez con fade
5. **Highlight:** Marca el slot seleccionado
6. **Panel:** Muestra/oculta según estado del inventario

---

## 🔗 Interacción con Otros Scripts

**Escucha eventos de:**
- `InventorySystem` → OnItemAdded, OnItemRemoved, OnSelectionChanged
- `InventoryUIController` → OnStateChanged

**Controla:**
- `InventorySlotUI` → UpdateSlot, Highlight, Unhighlight
- `CanvasGroup` → Visibilidad (panel y slots)
- `RectTransform` → Posiciones del carrusel

---

## 🎨 Configuración Recomendada en Unity

**GameObject:** `InventoryPanel`

**Inventory Panel UI Component:**
```
References:
  Inventory System: (auto-asignado)
  UI Controller: (auto-asignado)

Slot Settings:
  Slots Container: SlotsContainer (Transform)
  Slot Prefab: SlotTemplate (GameObject con InventorySlotUI)

Panels:
  Canvas Group: (auto-asignado)

Carousel Settings:
  Visible Slots: 3
  Slot Spacing: 220
  Transition Speed: 8
  Transition Curve: EaseInOut
```

**SlotsContainer (RectTransform):**
```
Anchors: Top Center
Pos X: 0
Pos Y: -500
Width: 1200
Height: 250
Pivot: (0.5, 1.0)
```

---

¡Este script es el **motor del carrusel** del inventario! 🎠✨
