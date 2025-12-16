# 🔧 Instrucciones para Arreglar el Menú Contextual

## ❌ Problema Identificado

El menú contextual aparece de forma incorrecta porque:
1. El prefab `OptionTemplate.prefab` tiene el texto **hardcodeado** como "Use Item"
2. El `RectTransform` del prefab está mal configurado (altura negativa, anchors incorrectos)
3. El `Vertical Layout Group` puede no estar correctamente configurado

---

## ✅ Solución Completa - Paso a Paso

### Paso 1: Arreglar el Prefab OptionTemplate

1. En **Project**, ve a `/Assets/Prefabs/UI/Inventory/`
2. Haz **doble clic** en `OptionTemplate.prefab` para editarlo
3. Con el prefab abierto en Inspector:

   **Componente RectTransform:**
   - Anchors: Click en el cuadro de anchors → Shift+Alt → Click en **Top Stretch** (arriba, expansión horizontal)
   - Pos Y: `0`
   - Height: `30`
   
   **Componente TextMeshProUGUI:**
   - **Text:** Borra completamente (déjalo vacío - esto es CRÍTICO)
   - Font Size: `20`
   - Color: Blanco `(255, 255, 255, 255)`
   - Alignment: **Left** y **Middle** (Centered Vertically)
   - Wrapping: Disabled
   - Overflow: Overflow
   
4. **Guarda** el prefab (Ctrl+S o File → Save)

---

### Paso 2: Configurar OptionContainer Correctamente

1. En Hierarchy: Selecciona `/InventoryCanvas/InventoryPanel/ContextMenuPanel/OptionContainer`
2. En Inspector, **asegúrate de tener**:

   **Componente RectTransform:**
   - Anchors: Top Stretch
   - Pivot X: `0.5`, Y: `1`
   - Left: `10`, Right: `10`
   - Top: `50`, Bottom: (cualquier valor)
   - Height: Ajusta según necesites (ej. `150`)
   
   **Componente Vertical Layout Group:**
   - Padding: Left `10`, Right `10`, Top `10`, Bottom `10`
   - Spacing: `10`
   - Child Alignment: **Upper Left**
   - Control Child Size:
     - Width: ✓ (marcado)
     - Height: ✗ (desmarcado)
   - Child Force Expand:
     - Width: ✓ (marcado)
     - Height: ✗ (desmarcado)

---

### Paso 3: Verificar ContextMenuPanel

1. Selecciona `/InventoryCanvas/InventoryPanel/ContextMenuPanel` en Hierarchy
2. En Inspector:

   **Componente RectTransform:**
   - Asegúrate de que esté bien posicionado en el centro o donde quieras
   - Anchors: Center
   - Width: `250`
   - Height: `300`
   
   **Componente Image:**
   - Color: Semi-transparente (ej. negro con alpha 0.9)

---

### Paso 4: Verificar Referencias en ContextMenuUI

1. Con `ContextMenuPanel` seleccionado
2. Componente `ContextMenuUI`:
   ```
   UI Controller: Player ✅
   Options Container: OptionContainer ✅
   Option Prefab: OptionTemplate ✅
   Normal Color: Blanco (255, 255, 255, 255)
   Selected Color: Amarillo (255, 235, 4, 255)
   ```

---

## 🎮 Probar

1. Presiona **Play**
2. Recoge items
3. Abre inventario (**Tab**)
4. Selecciona el item con las flechas
5. Presiona **E**
6. **Deberías ver:**
   ```
   ┌─────────────────┐
   │   ITEM ACTIONS  │ ← MenuTitle
   ├─────────────────┤
   │ Use             │ ← Opción 1
   │ Examine         │ ← Opción 2  
   │ Drop            │ ← Opción 3
   └─────────────────┘
   ```

---

## 💡 ¿Por Qué Fallaba?

### Problema 1: Texto Hardcodeado
El prefab tenía `m_text: Use Item` en el archivo YAML. Cuando el código ejecutaba:
```csharp
textComponent.text = "Examine";
```
No funcionaba porque el prefab ya estaba compilado con el texto anterior.

### Problema 2: RectTransform Incorrecto
El `SizeDelta` era `{x: 200, y: -30}` con altura negativa, causando que apareciera en posiciones raras.

### Problema 3: Anchors
Los anchors en `{0, 0}` hacían que el elemento se posicionara desde la esquina inferior izquierda en vez de expandirse correctamente en el contenedor.

---

## ✅ Verificación Final

Después de los cambios:
- El texto debe estar **vacío** en el prefab
- El Vertical Layout Group debe **controlar el ancho** pero **no la altura**
- Las opciones deben apilarse verticalmente
- Cada opción debe mostrar el texto correcto dinámicamente

---

**¡Importante!** Si sigues viendo "Use Item", asegúrate de:
1. **Guardar el prefab** después de borrar el texto
2. **Cerrar y reabrir Unity** si es necesario
3. **Verificar que no haya una copia del prefab** en la escena

---

¿Necesitas ayuda con algún paso específico? 🎯✨

