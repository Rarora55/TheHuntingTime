# 🎬 Animación del Menú Contextual

## 📋 Resumen

El menú contextual ahora tiene una **animación de escala vertical** que hace que aparezca desplegándose de arriba hacia abajo, como una cortina o persiana. Este efecto es común en juegos de survival horror como Silent Hill y Resident Evil.

---

## 🎨 Efecto Visual

### Apertura (ShowMenu)

```
Inicio (Scale Y = 0):
┌─────────────┐
│             │  ← Invisible (colapsado verticalmente)
└─────────────┘

Durante animación (Scale Y: 0 → 1):
┌─────────────┐
│ ITEM AC...  │  ← Se expande desde arriba
│ Exami...    │
└─────────────┘

Final (Scale Y = 1):
┌─────────────┐
│ ITEM ACTIONS│  ← Completamente visible
│ Examine     │
│ Drop        │
└─────────────┘
```

### Cierre (HideMenu)

```
Inicio (Scale Y = 1):
┌─────────────┐
│ ITEM ACTIONS│  ← Completamente visible
│ Examine     │
│ Drop        │
└─────────────┘

Durante animación (Scale Y: 1 → 0):
┌─────────────┐
│ Exami...    │  ← Se colapsa hacia arriba
│ D...        │
└─────────────┘

Final (Scale Y = 0):
┌─────────────┐
│             │  ← Invisible (colapsado)
└─────────────┘
```

---

## ⚙️ Configuración en Unity

Selecciona `/InventoryCanvas/InventoryPanel/ContextMenuPanel` en la escena:

**En el componente `Context Menu UI`:**

```
Animation Settings:
  Animation Duration: 0.3       (duración de la animación en segundos)
  Scale Curve: AnimationCurve   (curva de suavizado)
  Animate On Open: ✓            (animar al abrir)
  Animate On Close: ✓           (animar al cerrar)
```

### Parámetros Explicados

#### Animation Duration (Duración)

Controla qué tan rápida o lenta es la animación:

```
0.1 → Muy rápido (abrupto)
0.2 → Rápido
0.3 → Normal (recomendado) ✅
0.5 → Lento (dramático)
0.8 → Muy lento
```

**Recomendado:** `0.3` segundos para un efecto fluido pero rápido.

#### Scale Curve (Curva de Animación)

Define cómo se suaviza la animación. Por defecto es `EaseInOut`:

```
EaseInOut:  Lento al inicio, rápido en medio, lento al final (suave) ✅
Linear:     Velocidad constante (mecánico)
EaseIn:     Comienza lento, acelera al final
EaseOut:    Comienza rápido, desacelera al final
Custom:     Define tu propia curva
```

**Recomendado:** `EaseInOut` para animación natural y pulida.

#### Animate On Open / Close

Activa/desactiva las animaciones:

```
Animate On Open: ✓   → El menú se despliega al abrir
Animate On Open: ✗   → El menú aparece instantáneamente

Animate On Close: ✓  → El menú se colapsa al cerrar
Animate On Close: ✗  → El menú desaparece instantáneamente
```

**Recomendado:** Ambos activados para máximo efecto visual.

---

## 🎮 Funcionamiento Técnico

### Escala Vertical

La animación usa `localScale` del `RectTransform`:

```csharp
// Al abrir
Vector3 startScale = new Vector3(1f, 0f, 1f);  // Y=0 (colapsado)
Vector3 targetScale = new Vector3(1f, 1f, 1f); // Y=1 (completo)

// Al cerrar (inverso)
Vector3 startScale = new Vector3(1f, 1f, 1f);  // Y=1 (completo)
Vector3 targetScale = new Vector3(1f, 0f, 1f); // Y=0 (colapsado)
```

**Ejes:**
- **X = 1.0** → Ancho siempre al 100% (no cambia)
- **Y = 0→1** → Altura va de 0% a 100% (animación)
- **Z = 1.0** → Profundidad siempre al 100% (no cambia)

### Pivot Point

Para que la animación se expanda **desde arriba hacia abajo**, el pivot del `ContextMenuPanel` debe estar en la parte superior:

**RectTransform del ContextMenuPanel:**
```
Pivot: 0.5, 1.0
       ↑    ↑
       X    Y = 1 (arriba)
```

Si `Pivot Y = 1`, el menú se expande hacia abajo ✅  
Si `Pivot Y = 0.5`, el menú se expande desde el centro  
Si `Pivot Y = 0`, el menú se expande hacia arriba  

---

## 🔧 Configuración del Pivot

### Paso 1: Verificar el Pivot

Selecciona `ContextMenuPanel` en Hierarchy:

**Inspector → RectTransform:**
```
Pivot:
  X: 0.5   (centrado horizontalmente)
  Y: 1.0   (anclado arriba) ✅
```

### Paso 2: Si necesitas cambiar el Pivot

1. En Inspector → RectTransform
2. Busca el campo **Pivot**
3. Cambia `Y` a `1.0`

**Importante:** Al cambiar el pivot, puede que el menú se mueva visualmente. Ajusta la posición después si es necesario.

---

## 🎨 Variaciones de Animación

### 🚀 Rápida y Directa (Acción)

```
Animation Duration: 0.15
Scale Curve: Linear
```

### 🌊 Suave y Elegante (RPG/Aventura)

```
Animation Duration: 0.4
Scale Curve: EaseInOut
```

### ⚡ Snappy (Arcade)

```
Animation Duration: 0.2
Scale Curve: EaseOut
```

### 🎭 Dramática (Horror/Suspense)

```
Animation Duration: 0.6
Scale Curve: Custom (lento al inicio, muy rápido al final)
```

---

## 💡 Mejoras Adicionales Sugeridas

### 1. Sonido de Apertura/Cierre

Añade efectos de sonido cuando el menú se abre/cierra:

```csharp
// En OnContextMenuOpened
AudioSource.PlayOneShot(menuOpenSound);

// En OnContextMenuClosed
AudioSource.PlayOneShot(menuCloseSound);
```

Sonidos sugeridos:
- Apertura: "whoosh" suave, "paper rustle"
- Cierre: "click", "swoosh" inverso

### 2. Fade del CanvasGroup Sincronizado

Puedes hacer que el `alpha` del CanvasGroup también se anime junto con la escala:

```csharp
// Durante la animación
canvasGroup.alpha = curveValue; // 0 → 1 al abrir, 1 → 0 al cerrar
```

### 3. Animación de las Opciones Individuales

Haz que cada opción aparezca con un pequeño delay:

```csharp
for (int i = 0; i < optionTexts.Count; i++)
{
    float delay = i * 0.05f; // 50ms entre cada opción
    StartCoroutine(FadeInOption(optionTexts[i], delay));
}
```

---

## 🐛 Solución de Problemas

### El menú se expande desde el centro

**Problema:** El pivot no está configurado correctamente.

**Solución:**
1. Selecciona `ContextMenuPanel`
2. RectTransform → Pivot Y = 1.0

### La animación es demasiado rápida/lenta

**Problema:** `Animation Duration` no está ajustada.

**Solución:**
- Aumenta el valor para más lento (ej. 0.5)
- Disminuye el valor para más rápido (ej. 0.2)

### La animación se ve "mecánica"

**Problema:** La curva de animación es lineal.

**Solución:**
1. En Inspector → `Scale Curve`
2. Cambia a `AnimationCurve.EaseInOut`
3. O crea tu propia curva personalizada

### El menú no aparece después de la animación

**Problema:** El CanvasGroup puede estar desactivado.

**Solución:**
Verifica que al final de la animación:
```
canvasGroup.alpha = 1f
canvasGroup.interactable = true
canvasGroup.blocksRaycasts = true
```

### La animación se interrumpe

**Problema:** Múltiples animaciones ejecutándose simultáneamente.

**Solución:**
El código ya detiene animaciones previas con:
```csharp
if (currentAnimation != null)
    StopCoroutine(currentAnimation);
```

---

## 📊 Comparación: Con vs Sin Animación

### Sin Animación (Antes)

```
Estado: Closed
[Presiona E]
Estado: Open ← Aparece instantáneamente
```

**Sensación:** Abrupto, brusco, poco pulido

### Con Animación (Ahora)

```
Estado: Closed
[Presiona E]
Estado: Animating (0.3s)
  ┌───┐
  │ I │  ← Frame 1 (Y=0.3)
  │ E │
  
  ┌─────┐
  │ IT  │  ← Frame 2 (Y=0.6)
  │ Exa │
  │ Dro │
  
  ┌──────────┐
  │ ITEM ACT │  ← Frame 3 (Y=1.0)
  │ Examine  │
  │ Drop     │
  └──────────┘
Estado: Open
```

**Sensación:** Suave, profesional, satisfactorio

---

## ✅ Checklist

Después de implementar la animación:

- [ ] `Animation Duration` configurado (recomendado: 0.3)
- [ ] `Scale Curve` configurado (recomendado: EaseInOut)
- [ ] `Animate On Open` activado
- [ ] `Animate On Close` activado
- [ ] `ContextMenuPanel` tiene `Pivot Y = 1.0`
- [ ] Probado en Play mode: el menú se despliega desde arriba
- [ ] Probado cerrar: el menú se colapsa hacia arriba
- [ ] La animación es fluida y no se ve entrecortada

---

## 🎯 Resultado Final

**Al abrir inventario y presionar E:**
1. Menú aparece colapsado (invisible)
2. Se despliega suavemente desde arriba en 0.3 segundos
3. Queda completamente visible con opciones legibles

**Al cerrar el menú (Escape o seleccionar acción):**
1. Menú comienza completamente visible
2. Se colapsa suavemente hacia arriba en 0.3 segundos
3. Desaparece y libera la UI

---

¡Disfruta de tu menú contextual animado estilo survival horror! 🎮✨
