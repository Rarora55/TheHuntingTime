# 🎠 Configuración del Carrusel de Inventario (Estilo Silent Hill)

## 📋 Resumen

El inventario ahora funciona como un **carrusel horizontal** donde solo se muestran 3 slots a la vez (izquierda, centro, derecha). Al navegar con las flechas, los slots se desplazan suavemente con animación.

---

## 🔧 Configuración en Unity

### 1. Configurar InventoryPanel

Selecciona `/InventoryCanvas/InventoryPanel` en la escena `Character.unity`:

**En el componente `Inventory Panel UI`:**

```
Carousel Settings:
  Visible Slots: 3           (cuántos slots visibles simultáneamente)
  Slot Spacing: 220          (distancia en píxeles entre slots)
  Transition Speed: 8        (velocidad de animación, mayor = más rápido)
  Transition Curve: EaseInOut (curva de animación suave)
```

**Valores recomendados:**
- `Visible Slots: 3` → Muestra slot izquierdo, central y derecho
- `Slot Spacing: 220` → Ajusta según el tamaño de tus slots
- `Transition Speed: 8` → Animación fluida pero rápida
- `Transition Curve: AnimationCurve.EaseInOut` → Movimiento suave

---

### 2. Configurar SlotsContainer (Contenedor de Slots)

Selecciona `/InventoryCanvas/InventoryPanel/SlotsContainer`:

**En el componente `RectTransform`:**

```
Anchors: Top Center (mantén Shift+Alt, click en segunda fila, centro)
Pos X: 0
Pos Y: -150    (ajusta según qué tan abajo del borde superior quieres el carrusel)
Width: 1200    (debe ser suficientemente ancho para los slots)
Height: 250    (altura de los slots)
Pivot: 0.5, 1  (anclaje desde arriba)
```

**IMPORTANTE:**
- Usa **Top Center** anchors para posicionar desde la parte superior
- El `Pos Y` es negativo porque va hacia abajo desde el borde superior
- El contenedor debe ser **ancho** para que los slots puedan deslizarse horizontalmente
- Los slots se posicionarán automáticamente en código
- **NO uses** `Horizontal Layout Group` ni `Grid Layout Group`

---

### 3. Configurar SlotTemplate.prefab

En Project, abre `/Assets/Prefabs/UI/Inventory/SlotTemplate.prefab`:

**En el componente `RectTransform`:**

```
Anchors: Middle Center
Width: 200     (tamaño del slot)
Height: 200    (tamaño del slot)
Pos X: 0
Pos Y: 0
Pivot: 0.5, 0.5
```

**IMPORTANTE:**
- Usa **Middle Center** anchors para que el slot se centre correctamente
- El tamaño debe coincidir con `Slot Spacing` (ej. si spacing=220, width puede ser 200 con 20px de margen)
- Asegúrate de que tenga el componente `CanvasGroup` (se añade automáticamente si falta)

---

## 🎮 Funcionamiento

### Navegación

```
Estado: Inventario Abierto
Input: Arrow Left/Right (o A/D)

Resultado:
┌─────────────────────────────────┐
│  [Slot 0]  [Slot 1]  [Slot 2]  │  ← Inicial (centro en slot 1)
│              ▲                  │
│         Destacado                │
└─────────────────────────────────┘

Presiona Right →

┌─────────────────────────────────┐
│  [Slot 1]  [Slot 2]  [Slot 3]  │  ← Nuevo (centro en slot 2)
│              ▲                  │
│         Destacado                │
└─────────────────────────────────┘
```

### Visibilidad

- **Solo 3 slots visibles** a la vez
- El slot **central** es el destacado (seleccionado)
- Los slots **fuera del rango visible** tienen `alpha = 0` (invisibles)
- **Animación suave** al cambiar de slot

---

## ⚙️ Parámetros Ajustables

### Visible Slots (Slots Visibles)

```
Visible Slots: 1  → Solo el slot central
Visible Slots: 3  → Slot izquierdo, central, derecho (recomendado)
Visible Slots: 5  → 2 slots a cada lado + central
```

**Recomendado: 3** (estilo Silent Hill clásico)

### Slot Spacing (Espaciado)

```
Slot Spacing: 150  → Slots más juntos
Slot Spacing: 220  → Espaciado normal (recomendado)
Slot Spacing: 300  → Slots más separados
```

**Fórmula sugerida:** `Slot Width + Margen` (ej. 200 + 20 = 220)

### Transition Speed (Velocidad)

```
Transition Speed: 4   → Lento y dramático
Transition Speed: 8   → Rápido y fluido (recomendado)
Transition Speed: 12  → Muy rápido
```

### Transition Curve (Curva de Animación)

```
EaseInOut    → Acelera al inicio, desacelera al final (suave)
Linear       → Velocidad constante
EaseIn       → Comienza lento, acelera al final
EaseOut      → Comienza rápido, desacelera al final
```

**Recomendado:** `EaseInOut` para movimiento natural

---

## 🎨 Mejoras Visuales Sugeridas

### Escalado del Slot Central

Puedes añadir un efecto de escala al slot destacado para que se vea más prominente:

1. En `InventorySlotUI.cs`, en los métodos `Highlight()` y `Unhighlight()`:
   ```csharp
   public void Highlight()
   {
       // Código existente...
       transform.localScale = Vector3.one * 1.1f; // 10% más grande
   }

   public void Unhighlight()
   {
       // Código existente...
       transform.localScale = Vector3.one; // Tamaño normal
   }
   ```

### Desenfoque de Slots Laterales

Puedes reducir la opacidad de los slots no centrales para enfatizar el seleccionado:

1. Modifica el método `SetSlotVisibility()` en `InventoryPanelUI.cs` para graduar el alpha:
   ```csharp
   int distance = Mathf.Abs(i - currentHighlightedSlot);
   float alpha = distance == 0 ? 1f : 0.5f; // Central 100%, laterales 50%
   ```

---

## 🐛 Solución de Problemas

### Los slots no se mueven

1. Verifica que `SlotsContainer` **NO tenga** `Horizontal Layout Group` ni `Grid Layout Group`
2. Verifica que `Slot Spacing` sea > 0
3. Comprueba que los slots tienen `RectTransform`

### Los slots se solapan

1. Aumenta `Slot Spacing` (ej. de 220 a 300)
2. Verifica que `SlotsContainer` sea suficientemente ancho

### Los slots laterales no desaparecen

1. Verifica que cada slot tenga un componente `CanvasGroup` (se añade automáticamente)
2. Comprueba que `Visible Slots` esté configurado correctamente

### La animación es muy lenta/rápida

1. Ajusta `Transition Speed` (valores entre 4-12)
2. Modifica `Transition Curve` para cambiar la sensación

---

## 📝 Notas Técnicas

- La animación usa `Time.unscaledDeltaTime` para funcionar correctamente aunque el juego esté pausado (`Time.timeScale = 0`)
- Los slots fuera de rango tienen `alpha = 0`, `interactable = false` y `blocksRaycasts = false`
- La posición se calcula como: `offset * spacing` donde `offset = currentSlot - centerSlot`
- El sistema es circular: al llegar al último slot y presionar Right, vuelve al primero

---

## ✅ Checklist Final

- [ ] `InventoryPanel` tiene `Carousel Settings` configurados
- [ ] `SlotsContainer` **NO tiene** Layout Group
- [ ] `SlotsContainer` es suficientemente ancho (ej. 1200px)
- [ ] `SlotTemplate.prefab` usa **Middle Center** anchors
- [ ] `Slot Spacing` ≈ `Slot Width` + margen
- [ ] Probado en Play mode: navegar con arrows mueve los slots suavemente
- [ ] Solo 3 slots visibles simultáneamente

---

¡Disfruta de tu carrusel de inventario estilo Silent Hill! 🎮✨
