# 📖 InventorySlotUI.cs - Explicación Línea por Línea

**Ubicación:** `/Assets/Scripts/Inventory/UI/InventorySlotUI.cs`  
**Responsabilidad:** Representa un slot individual del inventario, mostrando icono, cantidad y estado de selección.

---

## 📦 Sección 1: Imports y Namespace (Líneas 1-6)

```csharp
1: using UnityEngine;
2: using UnityEngine.UI;
3: using TMPro;
4: 
5: namespace TheHunt.Inventory
6: {
```

**Línea 1:** `UnityEngine` → Clases base de Unity.  
**Línea 2:** `UnityEngine.UI` → Componentes de UI (Image).  
**Línea 3:** `TMPro` → TextMeshPro para textos de alta calidad.

---

## 🏗️ Sección 2: Declaración de Clase (Línea 7-8)

```csharp
7:     public class InventorySlotUI : MonoBehaviour
8:     {
```

**Clase pública** que representa un slot visual del inventario.

**Responsabilidades:**
- Mostrar icono del item
- Mostrar cantidad (si es stackable y > 1)
- Visualizar estado de selección (highlight)
- Cambiar apariencia entre vacío y lleno

---

## 🖼️ Sección 3: Referencias de UI (Líneas 9-13)

```csharp
9:         [Header("UI References")]
10:         [SerializeField] private Image iconImage;
11:         [SerializeField] private TextMeshProUGUI quantityText;
12:         [SerializeField] private Image highlightImage;
13:         [SerializeField] private Image backgroundImage;
```

**Línea 10:** `iconImage`  
Imagen que muestra el sprite del item (ej. icono de poción, arma, etc.).

**Línea 11:** `quantityText`  
Texto que muestra la cantidad (ej. "x3" si hay 3 items apilados).

**Línea 12:** `highlightImage`  
Imagen que indica que este slot está seleccionado (borde amarillo).

**Línea 13:** `backgroundImage`  
Imagen de fondo del slot (cambia de color según estado).

**Jerarquía típica:**
```
InventorySlot (GameObject)
  ├─ Background (Image) ← backgroundImage
  ├─ Icon (Image) ← iconImage
  ├─ Quantity (TextMeshProUGUI) ← quantityText
  └─ Highlight (Image) ← highlightImage
```

---

## 🎨 Sección 4: Configuración Visual (Líneas 15-19)

```csharp
15:         [Header("Visual Settings")]
16:         [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
17:         [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 1f);
18:         [SerializeField] private Color emptyIconColor = new Color(1f, 1f, 1f, 0.3f);
19:         [SerializeField] private Color fullIconColor = new Color(1f, 1f, 1f, 1f);
```

**Línea 16:** `normalColor`  
Color del fondo cuando NO está seleccionado (gris oscuro semi-transparente).  
RGB: `(51, 51, 51)` con alpha `0.8`.

**Línea 17:** `highlightColor`  
Color del highlight cuando está seleccionado (amarillo brillante).  
RGB: `(255, 255, 0)` con alpha `1.0`.

**Línea 18:** `emptyIconColor`  
Color del icono cuando el slot está vacío (blanco muy transparente).  
RGB: `(255, 255, 255)` con alpha `0.3`.

**Línea 19:** `fullIconColor`  
Color del icono cuando hay un item (blanco opaco).  
RGB: `(255, 255, 255)` con alpha `1.0`.

**Estados visuales:**
```
Vacío + No seleccionado:
  Background: Gris oscuro (normalColor)
  Icon: Blanco 30% alpha (emptyIconColor)
  Highlight: Desactivado

Lleno + No seleccionado:
  Background: Gris oscuro (normalColor)
  Icon: Blanco 100% alpha (fullIconColor)
  Quantity: "x3" (si hay 3)
  Highlight: Desactivado

Lleno + Seleccionado:
  Background: Amarillo 30% (highlightColor * 0.3)
  Icon: Blanco 100% alpha (fullIconColor)
  Quantity: "x3"
  Highlight: Activado (amarillo)
```

---

## 📊 Sección 5: Variables Privadas (Líneas 21-22)

```csharp
21:         private int slotIndex;
22:         private bool isHighlighted = false;
```

**Línea 21:** `slotIndex`  
Índice de este slot en el inventario (0-5).  
Útil para debugging y tracking.

**Línea 22:** `isHighlighted`  
Flag que indica si este slot está destacado actualmente.

---

## 🔍 Sección 6: Propiedad Pública (Línea 24)

```csharp
24:         public int SlotIndex => slotIndex;
```

**Propiedad de solo lectura** que expone el índice del slot.

**Uso:**  
Otros scripts pueden consultar qué índice tiene este slot.

---

## 🏁 Sección 7: Initialize (Líneas 26-31)

```csharp
26:         public void Initialize(int index)
27:         {
28:             slotIndex = index;
29:             ClearSlot();
30:             Unhighlight();
31:         }
```

**Método público** llamado por `InventoryPanelUI` al crear el slot.

**Parámetro:** `index`  
Índice asignado a este slot (0, 1, 2, 3, 4, 5).

**Flujo:**
1. **Línea 28:** Guarda el índice
2. **Línea 29:** Limpia el slot (sin icono, sin texto)
3. **Línea 30:** Asegura que comienza sin highlight

**Cuándo se llama:**
```csharp
// En InventoryPanelUI.CreateSlots()
for (int i = 0; i < 6; i++)
{
    slotUI.Initialize(i);  ← Aquí
}
```

---

## 🔄 Sección 8: UpdateSlot (Líneas 33-60)

```csharp
33:         public void UpdateSlot(ItemInstance item)
34:         {
35:             if (item == null)
36:             {
37:                 ClearSlot();
38:                 return;
39:             }
```

**Método público** que actualiza el contenido del slot.

**Parámetro:** `item`  
ItemInstance a mostrar (puede ser `null` si el slot está vacío).

**Líneas 35-39:** Slot vacío  
Si `item == null`, limpia el slot y termina.

```csharp
41:             if (iconImage != null)
42:             {
43:                 iconImage.sprite = item.itemData.ItemIcon;
44:                 iconImage.color = fullIconColor;
45:                 iconImage.enabled = true;
46:             }
```

**Líneas 41-46:** Actualiza icono  
- **Línea 43:** Asigna sprite del item
- **Línea 44:** Color opaco (blanco 100%)
- **Línea 45:** Activa la imagen

**Ejemplo:**
```csharp
item.itemData.ItemIcon → Sprite de health potion (🧪)
iconImage.sprite = 🧪
iconImage.color = (1, 1, 1, 1)  // Blanco opaco
```

```csharp
48:             if (quantityText != null)
49:             {
50:                 if (item.quantity > 1)
51:                 {
52:                     quantityText.text = $"x{item.quantity}";
53:                     quantityText.enabled = true;
54:                 }
55:                 else
56:                 {
57:                     quantityText.enabled = false;
58:                 }
59:             }
60:         }
```

**Líneas 48-59:** Actualiza cantidad  
Solo muestra texto si la cantidad es mayor a 1.

**Ejemplos:**
```
item.quantity = 1  → Texto desactivado (solo icono)
item.quantity = 3  → Texto "x3" visible
item.quantity = 6  → Texto "x6" visible
```

---

## 🧹 Sección 9: ClearSlot (Líneas 62-75)

```csharp
62:         public void ClearSlot()
63:         {
64:             if (iconImage != null)
65:             {
66:                 iconImage.sprite = null;
67:                 iconImage.color = emptyIconColor;
68:                 iconImage.enabled = false;
69:             }
70: 
71:             if (quantityText != null)
72:             {
73:                 quantityText.enabled = false;
74:             }
75:         }
```

**Método público** que limpia el slot (lo deja vacío).

**Líneas 64-69:** Limpia icono  
- **Línea 66:** Elimina sprite
- **Línea 67:** Color transparente (blanco 30%)
- **Línea 68:** Desactiva la imagen

**Líneas 71-74:** Oculta texto de cantidad

**Resultado visual:**
```
ANTES (con item):
┌──────────┐
│   🧪     │  ← Icono visible
│   x3     │  ← Cantidad visible
└──────────┘

DESPUÉS (limpiado):
┌──────────┐
│          │  ← Sin icono
│          │  ← Sin texto
└──────────┘
```

---

## 🌟 Sección 10: Highlight (Líneas 77-91)

```csharp
77:         public void Highlight()
78:         {
79:             isHighlighted = true;
80:             
81:             if (highlightImage != null)
82:             {
83:                 highlightImage.enabled = true;
84:                 highlightImage.color = highlightColor;
85:             }
86: 
87:             if (backgroundImage != null)
88:             {
89:                 backgroundImage.color = highlightColor * 0.3f;
90:             }
91:         }
```

**Método público** que marca el slot como seleccionado.

**Línea 79:** Marca flag como `true`.

**Líneas 81-85:** Activa imagen de highlight  
- **Línea 83:** Activa la imagen
- **Línea 84:** Color amarillo brillante

**Líneas 87-90:** Cambia color de fondo  
- **Línea 89:** Amarillo al 30% de intensidad  
  `highlightColor * 0.3f` = `(1, 1, 0, 1) * 0.3 = (0.3, 0.3, 0, 0.3)`

**Resultado visual:**
```
ANTES (sin highlight):
┌──────────┐
│ ┌──────┐ │  ← Fondo gris oscuro
│ │  🧪  │ │
│ └──────┘ │
└──────────┘

DESPUÉS (con highlight):
┌══════════┐  ← Borde amarillo (highlightImage)
║ ┌──────┐ ║  ← Fondo amarillo claro (background * 0.3)
║ │  🧪  │ ║
║ └──────┘ ║
└══════════┘
```

---

## 🔇 Sección 11: Unhighlight (Líneas 93-106)

```csharp
93:         public void Unhighlight()
94:         {
95:             isHighlighted = false;
96:             
97:             if (highlightImage != null)
98:             {
99:                 highlightImage.enabled = false;
100:             }
101: 
102:             if (backgroundImage != null)
103:             {
104:                 backgroundImage.color = normalColor;
105:             }
106:         }
```

**Método público** que quita el highlight del slot.

**Línea 95:** Marca flag como `false`.

**Líneas 97-100:** Desactiva imagen de highlight  
- **Línea 99:** Oculta el borde amarillo

**Líneas 102-105:** Restaura color de fondo  
- **Línea 104:** Vuelve al gris oscuro normal

**Resultado visual:**
```
ANTES (con highlight):
┌══════════┐
║ ┌──────┐ ║  ← Fondo amarillo claro
║ │  🧪  │ ║
║ └──────┘ ║
└══════════┘

DESPUÉS (sin highlight):
┌──────────┐
│ ┌──────┐ │  ← Fondo gris oscuro
│ │  🧪  │ │
│ └──────┘ │
└──────────┘
```

---

## 🎯 Flujo Completo de Uso

### 1. Creación del Slot

```
InventoryPanelUI.CreateSlots()
  ↓
Instantiate(slotPrefab)
  ↓
slotUI.Initialize(index: 2)
  ↓
  slotIndex = 2
  ClearSlot()
    ├─ iconImage.sprite = null
    ├─ iconImage.enabled = false
    └─ quantityText.enabled = false
  Unhighlight()
    ├─ highlightImage.enabled = false
    └─ backgroundImage.color = normalColor
```

### 2. Añadir Item al Slot

```
Usuario recoge item
  ↓
InventorySystem.AddItem(healthPotion)
  ↓
OnItemAdded → InventoryPanelUI.OnItemAdded(slotIndex: 2, item: healthPotion)
  ↓
slotUIList[2].UpdateSlot(healthPotion)
  ↓
  iconImage.sprite = healthPotion.ItemIcon (🧪)
  iconImage.color = fullIconColor (blanco opaco)
  iconImage.enabled = true
  
  if quantity > 1:
    quantityText.text = "x1"
    quantityText.enabled = false  ← Solo 1, no muestra texto
```

### 3. Seleccionar el Slot

```
Usuario navega al slot 2
  ↓
InventorySystem.SelectSlot(2)
  ↓
OnSelectionChanged → InventoryPanelUI.OnSelectionChanged(newSlot: 2)
  ↓
UpdateHighlight(2)
  ↓
  for (i = 0; i < 6; i++):
    if i == 2:
      slotUIList[2].Highlight()  ← Este
        ├─ highlightImage.enabled = true
        ├─ highlightImage.color = yellow
        └─ backgroundImage.color = yellow * 0.3
    else:
      slotUIList[i].Unhighlight()
        ├─ highlightImage.enabled = false
        └─ backgroundImage.color = normalColor
```

### 4. Añadir Más Items (Stack)

```
Usuario recoge otra health potion
  ↓
InventorySystem.AddItem(healthPotion)  ← Ya existe en slot 2
  ↓
items[2].quantity++  (1 → 2)
  ↓
OnItemAdded → slotUIList[2].UpdateSlot(items[2])
  ↓
  iconImage.sprite = healthPotion.ItemIcon (🧪)  ← Igual
  iconImage.color = fullIconColor
  iconImage.enabled = true
  
  if quantity > 1:  ← Ahora es 2
    quantityText.text = "x2"  ← Cambia
    quantityText.enabled = true  ← Se activa
```

### 5. Usar/Eliminar Item

```
Usuario usa health potion
  ↓
InventorySystem.UseCurrentItem()
  ↓
items[2].quantity--  (2 → 1)
RemoveItem si quantity == 0
  ↓
OnItemRemoved → slotUIList[2].UpdateSlot(items[2])
  ↓
  Si items[2] == null:
    ClearSlot()
      ├─ iconImage.sprite = null
      ├─ iconImage.enabled = false
      └─ quantityText.enabled = false
  
  Si items[2].quantity == 1:
    iconImage.sprite = healthPotion.ItemIcon
    quantityText.enabled = false  ← Oculta "x1"
```

---

## 📊 Estados del Slot

### Estado 1: Vacío + No Seleccionado

```
Visual:
┌──────────┐
│          │  Fondo gris oscuro
│          │  Sin icono
└──────────┘  Sin highlight

Propiedades:
- slotIndex: 2
- isHighlighted: false
- iconImage.enabled: false
- quantityText.enabled: false
- highlightImage.enabled: false
- backgroundImage.color: normalColor
```

### Estado 2: Lleno (1 item) + No Seleccionado

```
Visual:
┌──────────┐
│    🧪    │  Fondo gris oscuro
│          │  Icono visible
└──────────┘  Sin texto (quantity = 1)

Propiedades:
- iconImage.sprite: healthPotion.ItemIcon
- iconImage.color: fullIconColor
- iconImage.enabled: true
- quantityText.enabled: false
- highlightImage.enabled: false
- backgroundImage.color: normalColor
```

### Estado 3: Lleno (3 items) + No Seleccionado

```
Visual:
┌──────────┐
│    🧪    │  Fondo gris oscuro
│    x3    │  Icono + cantidad
└──────────┘

Propiedades:
- iconImage.sprite: healthPotion.ItemIcon
- iconImage.enabled: true
- quantityText.text: "x3"
- quantityText.enabled: true
- highlightImage.enabled: false
```

### Estado 4: Lleno (3 items) + Seleccionado

```
Visual:
┌══════════┐
║    🧪    ║  Fondo amarillo claro
║    x3    ║  Borde amarillo
└══════════┘  Icono + cantidad

Propiedades:
- isHighlighted: true
- iconImage.sprite: healthPotion.ItemIcon
- iconImage.enabled: true
- quantityText.text: "x3"
- quantityText.enabled: true
- highlightImage.enabled: true
- highlightImage.color: highlightColor (yellow)
- backgroundImage.color: highlightColor * 0.3 (yellow tint)
```

---

## ✅ Responsabilidades Clave

1. **Visualización:** Muestra icono y cantidad del item
2. **Estados:** Diferencia vacío/lleno y seleccionado/no seleccionado
3. **Actualización:** Responde a cambios en el inventario
4. **Highlight:** Indica visualmente el slot activo
5. **Colores:** Usa diferentes colores según estado

---

## 🔗 Interacción con Otros Scripts

**Controlado por:**
- `InventoryPanelUI` → Initialize, UpdateSlot, Highlight, Unhighlight

**Usa componentes:**
- `Image` (iconImage, backgroundImage, highlightImage)
- `TextMeshProUGUI` (quantityText)
- `RectTransform` (para posicionamiento en carrusel)
- `CanvasGroup` (para fade in/out en carrusel)

---

## 🎨 Configuración Recomendada en Unity

**GameObject:** `SlotTemplate` (Prefab)

**Jerarquía:**
```
SlotTemplate
  ├─ Background (Image)
  │    └─ Color: (51, 51, 51, 204)  ← normalColor
  ├─ Icon (Image)
  │    └─ Color: (255, 255, 255, 255)
  ├─ QuantityText (TextMeshProUGUI)
  │    └─ Anchor: Bottom Right
  └─ Highlight (Image)
       └─ Color: (255, 255, 0, 255)
       └─ Enabled: false (inicio)
```

**Inventory Slot UI Component:**
```
UI References:
  Icon Image: Icon (Image)
  Quantity Text: QuantityText (TextMeshProUGUI)
  Highlight Image: Highlight (Image)
  Background Image: Background (Image)

Visual Settings:
  Normal Color: (51, 51, 51, 204)       ← Gris oscuro
  Highlight Color: (255, 255, 0, 255)   ← Amarillo
  Empty Icon Color: (255, 255, 255, 76) ← Blanco 30%
  Full Icon Color: (255, 255, 255, 255) ← Blanco opaco
```

**RectTransform:**
```
Anchors: Middle Center
Width: 200
Height: 200
Pivot: (0.5, 0.5)
```

---

## 💡 Consejos de Uso

### 1. Customización de Colores

Puedes cambiar los colores en el Inspector sin modificar código:

```
Normal Color → Color de fondo cuando NO está seleccionado
Highlight Color → Color del borde/fondo cuando está seleccionado
Empty Icon Color → Tinte del icono cuando está vacío
Full Icon Color → Color del icono cuando hay item
```

### 2. Efectos Adicionales

Puedes añadir efectos en los métodos `Highlight()` y `Unhighlight()`:

```csharp
public void Highlight()
{
    // Código existente...
    
    // Añadir escala
    transform.localScale = Vector3.one * 1.1f;
}

public void Unhighlight()
{
    // Código existente...
    
    // Restaurar escala
    transform.localScale = Vector3.one;
}
```

### 3. Animaciones

Puedes animar transiciones usando coroutines:

```csharp
private IEnumerator AnimateHighlight()
{
    float duration = 0.2f;
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = elapsed / duration;
        
        // Interpola escala
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.1f, t);
        
        yield return null;
    }
}
```

---

¡Este script es la **pieza fundamental** de cada slot del inventario! 📦✨
