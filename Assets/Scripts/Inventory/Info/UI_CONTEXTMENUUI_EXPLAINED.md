# 📖 ContextMenuUI.cs - Explicación Línea por Línea

**Ubicación:** `/Assets/Scripts/Inventory/UI/ContextMenuUI.cs`  
**Responsabilidad:** Gestiona la visualización del menú contextual con opciones dinámicas y animación de escala vertical.

---

## 📦 Sección 1: Imports y Namespace (Líneas 1-6)

```csharp
1: using UnityEngine;
2: using TMPro;
3: using System.Collections;
4: using System.Collections.Generic;
5: 
6: namespace TheHunt.Inventory
```

**Línea 1:** `UnityEngine` → Clases base de Unity.  
**Línea 2:** `TMPro` → TextMeshPro para textos de alta calidad.  
**Línea 3:** `System.Collections` → Necesario para coroutines (`IEnumerator`).  
**Línea 4:** `System.Collections.Generic` → Para `List<T>`.

---

## 🏗️ Sección 2: Declaración de Clase (Línea 7-8)

```csharp
7:     public class ContextMenuUI : MonoBehaviour
8:     {
```

**Clase pública** que gestiona la UI del menú contextual.

**Responsabilidades:**
- Crear opciones dinámicamente según las acciones disponibles
- Mostrar/ocultar menú con animación de escala
- Actualizar highlight de la opción seleccionada
- Gestionar CanvasGroup para visibilidad

---

## 🔧 Sección 3: Referencias (Líneas 9-15)

```csharp
9:         [Header("References")]
10:         [SerializeField] private InventoryUIController uiController;
11: 
12:         [Header("UI Elements")]
13:         [SerializeField] private CanvasGroup canvasGroup;
14:         [SerializeField] private Transform optionsContainer;
15:         [SerializeField] private GameObject optionPrefab;
```

**Línea 10:** `uiController`  
Referencia al controlador para:
- Escuchar eventos (`OnContextMenuOpened`, `OnContextMenuClosed`)
- Obtener nombres de acciones (`GetContextActionDisplayName()`)

**Línea 13:** `canvasGroup`  
Controla visibilidad del menú (`alpha`, `interactable`, `blocksRaycasts`).

**Línea 14:** `optionsContainer`  
Transform donde se instancian las opciones (normalmente `OptionContainer`).

**Línea 15:** `optionPrefab`  
Prefab de cada opción (debe tener componente `TextMeshProUGUI`).

---

## 🎨 Sección 4: Configuración Visual (Líneas 17-19)

```csharp
17:         [Header("Visual Settings")]
18:         [SerializeField] private Color normalColor = Color.white;
19:         [SerializeField] private Color selectedColor = Color.yellow;
```

**Línea 18:** `normalColor`  
Color de las opciones NO seleccionadas (blanco por defecto).

**Línea 19:** `selectedColor`  
Color de la opción seleccionada (amarillo por defecto).

**Uso:**
```
Examine    ← Blanco (normal)
Drop       ← Amarillo (seleccionado)
```

---

## 🎬 Sección 5: Configuración de Animación (Líneas 21-25)

```csharp
21:         [Header("Animation Settings")]
22:         [SerializeField] private float animationDuration = 0.3f;
23:         [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
24:         [SerializeField] private bool animateOnOpen = true;
25:         [SerializeField] private bool animateOnClose = true;
```

**Línea 22:** `animationDuration`  
Duración de la animación de escala en segundos (0.3s = rápido y fluido).

**Línea 23:** `scaleCurve`  
Curva de animación que controla el suavizado:
- `EaseInOut` → Lento al inicio, rápido en medio, lento al final
- Valores: `(0,0)` inicio → `(1,1)` final

**Línea 24:** `animateOnOpen`  
Si es `true`, el menú se despliega con animación al abrir.

**Línea 25:** `animateOnClose`  
Si es `true`, el menú se colapsa con animación al cerrar.

---

## 📊 Sección 6: Variables Privadas (Líneas 27-30)

```csharp
27:         private List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>();
28:         private int currentSelection = 0;
29:         private RectTransform rectTransform;
30:         private Coroutine currentAnimation;
```

**Línea 27:** `optionTexts`  
Lista de los componentes TextMeshProUGUI de cada opción creada.  
**Uso:** Actualizar colores al cambiar selección.

**Línea 28:** `currentSelection`  
Índice de la opción actualmente seleccionada (0, 1, 2...).

**Línea 29:** `rectTransform`  
RectTransform del menú para animar su escala.

**Línea 30:** `currentAnimation`  
Referencia a la coroutine de animación actual (para detenerla si es necesario).

---

## 🏁 Sección 7: Awake (Líneas 32-41)

```csharp
32:         private void Awake()
33:         {
34:             if (uiController == null)
35:                 uiController = FindFirstObjectByType<InventoryUIController>();
36: 
37:             if (canvasGroup == null)
38:                 canvasGroup = GetComponent<CanvasGroup>();
39: 
40:             rectTransform = GetComponent<RectTransform>();
41:         }
```

**Líneas 34-35:** Auto-referencia del controller  
Si no está asignado, busca en la escena.

**Líneas 37-38:** Auto-referencia del CanvasGroup  
Si no está asignado, busca en el mismo GameObject.

**Línea 40:** Obtiene RectTransform  
Necesario para animar la escala (`localScale`).

---

## 🔄 Sección 8: Start (Líneas 43-46)

```csharp
43:         private void Start()
44:         {
45:             HideMenu();
46:         }
```

**Línea 45:** Oculta el menú al inicio  
Asegura que comience invisible (alpha=0, interactable=false).

---

## 📡 Sección 9: Suscripción a Eventos (Líneas 48-66)

```csharp
48:         private void OnEnable()
49:         {
50:             if (uiController != null)
51:             {
52:                 uiController.OnContextMenuOpened += OnContextMenuOpened;
53:                 uiController.OnContextMenuClosed += OnContextMenuClosed;
54:                 uiController.OnContextMenuSelectionChanged += OnSelectionChanged;
55:             }
56:         }
57: 
58:         private void OnDisable()
59:         {
60:             if (uiController != null)
61:             {
62:                 uiController.OnContextMenuOpened -= OnContextMenuOpened;
63:                 uiController.OnContextMenuClosed -= OnContextMenuClosed;
64:                 uiController.OnContextMenuSelectionChanged -= OnSelectionChanged;
65:             }
66:         }
```

**OnEnable:** Se suscribe a eventos del controller cuando el script se activa.  
**OnDisable:** Se desuscribe cuando se desactiva.

**Eventos escuchados:**
1. `OnContextMenuOpened` → Crear opciones
2. `OnContextMenuClosed` → Limpiar opciones
3. `OnContextMenuSelectionChanged` → Actualizar highlight

**Nota:** Usar `OnEnable`/`OnDisable` en lugar de `Awake`/`OnDestroy` permite que el GameObject se desactive sin perder suscripciones.

---

## 📂 Sección 10: Menú Abierto (Líneas 68-81)

```csharp
68:         private void OnContextMenuOpened(List<ItemContextAction> actions)
69:         {
70:             ShowMenu();
71:             ClearOptions();
72: 
73:             foreach (ItemContextAction action in actions)
74:             {
75:                 CreateOption(action);
76:             }
77: 
78:             UpdateSelectionVisual(0);
79: 
80:             Debug.Log($"<color=cyan>[CONTEXT MENU UI] Opened with {actions.Count} actions</color>");
81:         }
```

**Método privado** llamado cuando el controller abre el menú.

**Parámetro:** `actions`  
Lista de acciones disponibles (ej. `[Use, Examine, Drop]`).

**Flujo:**
1. **Línea 70:** Muestra el menú (alpha=1, animación de escala)
2. **Línea 71:** Limpia opciones anteriores
3. **Líneas 73-76:** Crea una opción por cada acción
4. **Línea 78:** Selecciona la primera opción (índice 0)
5. **Línea 80:** Logea cuántas acciones se crearon

---

## 🚫 Sección 11: Menú Cerrado (Líneas 83-89)

```csharp
83:         private void OnContextMenuClosed()
84:         {
85:             ClearOptions();
86:             HideMenu();
87: 
88:             Debug.Log("<color=cyan>[CONTEXT MENU UI] Closed</color>");
89:         }
```

**Método privado** llamado cuando el controller cierra el menú.

**Flujo:**
1. **Línea 85:** Limpia opciones (destruye GameObjects)
2. **Línea 86:** Oculta el menú (alpha=0, animación de colapso)

---

## 🎯 Sección 12: Cambio de Selección (Líneas 91-94)

```csharp
91:         private void OnSelectionChanged(int newIndex)
92:         {
93:             UpdateSelectionVisual(newIndex);
94:         }
```

**Método privado** llamado cuando el usuario navega con Up/Down en el menú.

**Parámetro:** `newIndex`  
Nuevo índice seleccionado (0, 1, 2...).

**Acción:**  
Actualiza colores: opción seleccionada → amarillo, resto → blanco.

---

## 🔨 Sección 13: Crear Opción (Líneas 96-118)

```csharp
96:         private void CreateOption(ItemContextAction action)
97:         {
98:             if (optionsContainer == null || optionPrefab == null)
99:             {
100:                 Debug.LogWarning("<color=yellow>[CONTEXT MENU UI] Container or prefab is null!</color>");
101:                 return;
102:             }
```

**Líneas 98-102:** Validación  
Verifica que existan el contenedor y el prefab.

```csharp
104:             GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
105:             TextMeshProUGUI textComponent = optionObj.GetComponent<TextMeshProUGUI>();
```

**Línea 104:** Instancia el prefab  
Crea una nueva opción como hijo del `optionsContainer`.

**Línea 105:** Obtiene el componente de texto  
El prefab debe tener un `TextMeshProUGUI`.

```csharp
107:             if (textComponent != null)
108:             {
109:                 textComponent.text = uiController.GetContextActionDisplayName(action);
110:                 textComponent.color = normalColor;
111:                 optionTexts.Add(textComponent);
112:                 Debug.Log($"<color=green>[CONTEXT MENU UI] Created option: {textComponent.text}</color>");
113:             }
114:             else
115:             {
116:                 Debug.LogWarning("<color=yellow>[CONTEXT MENU UI] Prefab has no TextMeshProUGUI component!</color>");
117:             }
118:         }
```

**Línea 109:** Asigna el texto  
Convierte `ItemContextAction.Use` → `"Use"`.

**Línea 110:** Color inicial  
Blanco por defecto (no seleccionado).

**Línea 111:** Añade a la lista  
Para poder actualizar su color después.

**Ejemplo:**
```
Action: ItemContextAction.Use
  ↓
GetContextActionDisplayName(Use)
  ↓
Texto: "Use"
  ↓
Opción creada con color blanco
```

---

## 🧹 Sección 14: Limpiar Opciones (Líneas 120-132)

```csharp
120:         private void ClearOptions()
121:         {
122:             if (optionsContainer == null)
123:                 return;
124: 
125:             foreach (Transform child in optionsContainer)
126:             {
127:                 Destroy(child.gameObject);
128:             }
129: 
130:             optionTexts.Clear();
131:             currentSelection = 0;
132:         }
```

**Método privado** que destruye todas las opciones creadas.

**Líneas 125-128:** Destruye todos los hijos  
Itera sobre los hijos del contenedor y los destruye.

**Línea 130:** Limpia la lista de textos.  
**Línea 131:** Resetea la selección a 0.

---

## 🎨 Sección 15: Actualizar Visual de Selección (Líneas 134-151)

```csharp
134:         private void UpdateSelectionVisual(int selectedIndex)
135:         {
136:             currentSelection = selectedIndex;
137: 
138:             for (int i = 0; i < optionTexts.Count; i++)
139:             {
140:                 if (i == selectedIndex)
141:                 {
142:                     optionTexts[i].color = selectedColor;
143:                     optionTexts[i].fontSize = optionTexts[i].fontSize * 1.1f;
144:                 }
145:                 else
146:                 {
147:                     optionTexts[i].color = normalColor;
148:                     optionTexts[i].fontSize = optionTexts[i].fontSize / 1.1f;
149:                 }
150:             }
151:         }
```

**Método privado** que actualiza colores y tamaños según selección.

**Línea 136:** Guarda el índice seleccionado.

**Líneas 138-150:** Itera sobre todas las opciones  
- **Seleccionada:** Color amarillo + tamaño 110%
- **No seleccionada:** Color blanco + tamaño 100%

**Ejemplo visual:**
```
ANTES (selección en 0):
Use       ← Amarillo, grande
Examine   ← Blanco, normal
Drop      ← Blanco, normal

Usuario navega Down (selección → 1):
Use       ← Blanco, normal
Examine   ← Amarillo, grande
Drop      ← Blanco, normal
```

---

## 👁️ Sección 16: Mostrar Menú (Líneas 153-165)

```csharp
153:         private void ShowMenu()
154:         {
155:             if (canvasGroup != null)
156:             {
157:                 canvasGroup.alpha = 1f;
158:                 canvasGroup.interactable = true;
159:                 canvasGroup.blocksRaycasts = true;
160:             }
161: 
162:             if (animateOnOpen)
163:             {
164:                 if (currentAnimation != null)
165:                     StopCoroutine(currentAnimation);
```

**Líneas 157-159:** Activa el CanvasGroup  
- `alpha = 1f` → Visible
- `interactable = true` → Puede recibir input
- `blocksRaycasts = true` → Bloquea clicks detrás del menú

**Líneas 162-165:** Detiene animación previa  
Si hay una animación en curso, la detiene.

```csharp
167:                 currentAnimation = StartCoroutine(AnimateScale(Vector3.one, Vector3.one));
168:             }
169:         }
```

**Línea 167:** Inicia animación de apertura  
Llama a `AnimateScale()` que despliega el menú desde arriba.

---

## 🙈 Sección 17: Ocultar Menú (Líneas 171-189)

```csharp
171:         private void HideMenu()
172:         {
173:             if (animateOnClose && gameObject.activeInHierarchy)
174:             {
175:                 if (currentAnimation != null)
176:                     StopCoroutine(currentAnimation);
177:                 
178:                 currentAnimation = StartCoroutine(AnimateScaleAndHide());
179:             }
180:             else
181:             {
182:                 if (canvasGroup != null)
183:                 {
184:                     canvasGroup.alpha = 0f;
185:                     canvasGroup.interactable = false;
186:                     canvasGroup.blocksRaycasts = false;
187:                 }
188:             }
189:         }
```

**Línea 173:** Verifica si debe animar  
Y si el GameObject está activo (necesario para coroutines).

**Líneas 175-178:** Con animación  
Detiene animación previa y comienza colapso animado.

**Líneas 180-188:** Sin animación  
Oculta instantáneamente (alpha=0, interactable=false).

---

## 🎬 Sección 18: Animación de Apertura (Líneas 191-218)

```csharp
191:         private IEnumerator AnimateScale(Vector3 from, Vector3 to)
192:         {
193:             if (rectTransform == null)
194:                 yield break;
195: 
196:             Vector3 startScale = new Vector3(1f, 0f, 1f);
197:             Vector3 targetScale = new Vector3(1f, 1f, 1f);
```

**Línea 196:** Escala inicial  
`(X=1, Y=0, Z=1)` → Menú colapsado verticalmente (invisible).

**Línea 197:** Escala objetivo  
`(X=1, Y=1, Z=1)` → Menú completamente visible.

```csharp
199:             rectTransform.localScale = startScale;
200: 
201:             float elapsed = 0f;
202: 
203:             while (elapsed < animationDuration)
204:             {
205:                 elapsed += Time.unscaledDeltaTime;
206:                 float t = Mathf.Clamp01(elapsed / animationDuration);
207:                 float curveValue = scaleCurve.Evaluate(t);
208: 
209:                 rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
210: 
211:                 yield return null;
212:             }
```

**Línea 199:** Resetea a escala inicial.

**Línea 205:** `Time.unscaledDeltaTime`  
Funciona aunque el juego esté pausado (`Time.timeScale = 0`).

**Línea 206:** `t` → Progreso de 0 a 1.

**Línea 207:** `curveValue`  
Aplica la curva de suavizado (`EaseInOut`).

**Línea 209:** Interpola la escala  
De `(1,0,1)` a `(1,1,1)` usando el valor de la curva.

**Línea 211:** Espera un frame  
Permite que Unity actualice la UI.

```csharp
214:             rectTransform.localScale = targetScale;
215:             currentAnimation = null;
216:         }
```

**Línea 214:** Asegura escala final exacta.  
**Línea 215:** Limpia referencia de coroutine.

**Efecto visual:**
```
Frame 0:  localScale = (1, 0.0, 1)  ← Colapsado
Frame 5:  localScale = (1, 0.3, 1)  ← Expandiéndose
Frame 10: localScale = (1, 0.7, 1)  ← Casi completo
Frame 15: localScale = (1, 1.0, 1)  ← Completamente visible
```

---

## 🎬 Sección 19: Animación de Cierre (Líneas 218-248)

```csharp
219:         private IEnumerator AnimateScaleAndHide()
220:         {
221:             if (rectTransform == null)
222:                 yield break;
223: 
224:             Vector3 startScale = rectTransform.localScale;
225:             Vector3 targetScale = new Vector3(1f, 0f, 1f);
```

**Línea 224:** Escala inicial  
Comienza desde la escala actual (normalmente `(1,1,1)`).

**Línea 225:** Escala objetivo  
`(1,0,1)` → Colapsado verticalmente.

```csharp
227:             float elapsed = 0f;
228: 
229:             while (elapsed < animationDuration)
230:             {
231:                 elapsed += Time.unscaledDeltaTime;
232:                 float t = Mathf.Clamp01(elapsed / animationDuration);
233:                 float curveValue = scaleCurve.Evaluate(t);
234: 
235:                 rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
236: 
237:                 yield return null;
238:             }
```

**Similar a la animación de apertura, pero inversa:**
- De `(1,1,1)` → `(1,0,1)`

```csharp
240:             rectTransform.localScale = targetScale;
241: 
242:             if (canvasGroup != null)
243:             {
244:                 canvasGroup.alpha = 0f;
245:                 canvasGroup.interactable = false;
246:                 canvasGroup.blocksRaycasts = false;
247:             }
248: 
249:             currentAnimation = null;
250:         }
```

**Línea 240:** Asegura escala final.

**Líneas 242-247:** Desactiva CanvasGroup  
Después de la animación, oculta completamente el menú.

**Línea 249:** Limpia referencia.

**Efecto visual:**
```
Frame 0:  localScale = (1, 1.0, 1)  ← Completamente visible
Frame 5:  localScale = (1, 0.7, 1)  ← Colapsándose
Frame 10: localScale = (1, 0.3, 1)  ← Casi colapsado
Frame 15: localScale = (1, 0.0, 1)  ← Invisible
→ CanvasGroup.alpha = 0
```

---

## 🎯 Flujo Completo de Uso

### 1. Usuario Abre Menú

```
InventoryUIController.OpenContextMenu()
  ↓
OnContextMenuOpened(actions: [Use, Examine, Drop])
  ↓
ShowMenu()
  ├─ canvasGroup.alpha = 1
  └─ StartCoroutine(AnimateScale)
      ↓
      Anima de (1,0,1) → (1,1,1) en 0.3s
  ↓
ClearOptions()
  ↓
CreateOption(Use)
CreateOption(Examine)
CreateOption(Drop)
  ↓
UpdateSelectionVisual(0)  ← "Use" en amarillo
```

### 2. Usuario Navega

```
Input: Arrow Down
  ↓
InventoryUIController.NavigateContextMenu(1f)
  ↓
OnContextMenuSelectionChanged(newIndex: 1)
  ↓
UpdateSelectionVisual(1)
  ├─ optionTexts[0].color = white   (Use)
  ├─ optionTexts[1].color = yellow  (Examine) ← Seleccionado
  └─ optionTexts[2].color = white   (Drop)
```

### 3. Usuario Cierra Menú

```
Input: Escape
  ↓
InventoryUIController.CloseContextMenu()
  ↓
OnContextMenuClosed()
  ↓
ClearOptions()
  ├─ Destroy(optionGameObject[0])
  ├─ Destroy(optionGameObject[1])
  ├─ Destroy(optionGameObject[2])
  └─ optionTexts.Clear()
  ↓
HideMenu()
  └─ StartCoroutine(AnimateScaleAndHide)
      ↓
      Anima de (1,1,1) → (1,0,1) en 0.3s
      ↓
      canvasGroup.alpha = 0
```

---

## ✅ Responsabilidades Clave

1. **Creación Dinámica:** Genera opciones basadas en la lista de acciones
2. **Visualización:** Muestra/oculta menú con CanvasGroup
3. **Animación:** Despliega/colapsa con escala vertical suave
4. **Highlight:** Actualiza colores según selección
5. **Eventos:** Escucha cambios del `InventoryUIController`

---

## 🔗 Interacción con Otros Scripts

**Escucha eventos de:**
- `InventoryUIController` → OnContextMenuOpened, OnContextMenuClosed, OnContextMenuSelectionChanged

**Usa componentes:**
- `CanvasGroup` → Visibilidad (alpha, interactable, blocksRaycasts)
- `RectTransform` → Animación de escala (localScale)
- `TextMeshProUGUI` → Textos de opciones (color, text)

---

## 🎨 Configuración Recomendada en Unity

**GameObject:** `ContextMenuPanel`

**RectTransform:**
```
Pivot: (0.5, 1.0)  ← IMPORTANTE para que se expanda desde arriba
```

**Canvas Group:**
```
Alpha: 0 (start)
Interactable: false
Blocks Raycasts: false
```

**Context Menu UI Component:**
```
UI Controller: (auto-asignado)
Canvas Group: (auto-asignado)
Options Container: OptionContainer (Transform)
Option Prefab: OptionTemplate (GameObject con TextMeshProUGUI)

Normal Color: White (255, 255, 255, 255)
Selected Color: Yellow (255, 255, 0, 255)

Animation Duration: 0.3
Scale Curve: EaseInOut
Animate On Open: ✓
Animate On Close: ✓
```

---

¡Este script crea la **magia visual** del menú contextual! ✨📜
