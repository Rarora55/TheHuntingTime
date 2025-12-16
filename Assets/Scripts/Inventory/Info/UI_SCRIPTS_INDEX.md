# 📚 Índice de Scripts de UI del Inventario - Explicaciones Completas

Esta carpeta contiene explicaciones **línea por línea** de todos los scripts de UI del sistema de inventario.

---

## 📖 Archivos de Explicación

### 1. InventoryUIController.cs
**Archivo:** `UI_INVENTORYUICONTROLLER_EXPLAINED.md`  
**Script:** `/Assets/Scripts/Inventory/UI/InventoryUIController.cs`

**Responsabilidad:** Controlador central de la UI del inventario

**Contenido:**
- ✅ Gestión de estados (Closed, Open, ContextMenu)
- ✅ Navegación entre slots
- ✅ Apertura/cierre de menú contextual
- ✅ Construcción dinámica de acciones disponibles
- ✅ Ejecución de acciones (Use, Examine, Drop, Equip)
- ✅ Control de pausa del juego (Time.timeScale)
- ✅ Sistema de eventos para coordinar UI

**Líneas totales:** 264

---

### 2. ContextMenuUI.cs
**Archivo:** `UI_CONTEXTMENUUI_EXPLAINED.md`  
**Script:** `/Assets/Scripts/Inventory/UI/ContextMenuUI.cs`

**Responsabilidad:** Visualización del menú contextual con animación

**Contenido:**
- ✅ Creación dinámica de opciones de menú
- ✅ Animación de escala vertical (apertura/cierre)
- ✅ Highlight de opción seleccionada
- ✅ Gestión de CanvasGroup para visibilidad
- ✅ Coroutines de animación con curvas de suavizado
- ✅ Sistema de suscripción a eventos

**Líneas totales:** 251

---

### 3. InventoryPanelUI.cs
**Archivo:** `UI_INVENTORYPANELUI_EXPLAINED.md`  
**Script:** `/Assets/Scripts/Inventory/UI/InventoryPanelUI.cs`

**Responsabilidad:** Panel principal y carrusel de slots

**Contenido:**
- ✅ Creación dinámica de 6 slots
- ✅ Sistema de carrusel animado (estilo Silent Hill)
- ✅ Cálculo de posiciones y visibilidad de slots
- ✅ Animación suave de transiciones
- ✅ Sincronización con InventorySystem
- ✅ Gestión de highlight de selección
- ✅ Fade in/out de slots según visibilidad

**Líneas totales:** 334

---

### 4. InventorySlotUI.cs
**Archivo:** `UI_INVENTORYSLOTUI_EXPLAINED.md`  
**Script:** `/Assets/Scripts/Inventory/UI/InventorySlotUI.cs`

**Responsabilidad:** Representación visual de un slot individual

**Contenido:**
- ✅ Visualización de icono de item
- ✅ Texto de cantidad (si es stackable)
- ✅ Estados vacío/lleno
- ✅ Highlight de selección
- ✅ Colores dinámicos según estado
- ✅ Limpieza y actualización de contenido

**Líneas totales:** 109

---

## 🔗 Relación Entre Scripts

```
PlayerInputHandler
        ↓
┌───────────────────────────────────────┐
│   InventoryUIController               │  ← Controlador central
│   (Gestiona estados y navegación)    │
└───────────┬───────────────────────────┘
            │
            ├─────────────────┬─────────────────┐
            ↓                 ↓                 ↓
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│ InventoryPanel  │  │ ContextMenuUI   │  │ InventorySystem │
│ UI              │  │ (Menú acciones) │  │ (Backend)       │
│ (Panel + slots) │  └─────────────────┘  └─────────────────┘
└────────┬────────┘
         │
         ├──────────────┬──────────────┬──────────────┐
         ↓              ↓              ↓              ↓
  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐
  │ SlotUI [0] │ │ SlotUI [1] │ │ SlotUI [2] │ │ ... [5]    │
  └────────────┘ └────────────┘ └────────────┘ └────────────┘
```

---

## 🎯 Flujo de Datos Completo

### 1. Usuario Abre Inventario (Tab)

```
PlayerInputHandler
  ↓ OnToggleInventory()
InventoryUIController
  ↓ OpenInventory()
  ↓ SetState(Open)
  ↓ OnStateChanged evento
InventoryPanelUI
  ↓ OnInventoryStateChanged()
  ↓ ShowInventory()
  ↓ canvasGroup.alpha = 1
  ↓ RefreshAllSlots()
InventorySlotUI (x6)
  ↓ UpdateSlot()
  ↓ Muestra iconos y cantidades
```

### 2. Usuario Navega (Arrow Right)

```
PlayerInputHandler
  ↓ OnNavigate(1f)
InventoryUIController
  ↓ NavigateInventory(1f)
  ↓ inventorySystem.SelectNext()
InventorySystem
  ↓ selectedIndex++
  ↓ OnSelectionChanged evento
InventoryPanelUI
  ↓ OnSelectionChanged()
  ↓ UpdateHighlight()
  ↓ UpdateCarouselPositions()
  ↓ StartCoroutine(AnimateCarousel)
InventorySlotUI (x6)
  ↓ Slot anterior: Unhighlight()
  ↓ Slot nuevo: Highlight()
  ↓ Posiciones animadas
  ↓ Fade in/out según visibilidad
```

### 3. Usuario Abre Menú Contextual (E)

```
PlayerInputHandler
  ↓ OnInteract()
InventoryUIController
  ↓ InteractWithCurrentItem()
  ↓ OpenContextMenu()
  ↓ Construye lista de acciones:
      - Verifica IUsable → Use
      - Verifica CanBeExamined → Examine
      - Verifica WeaponItemData → Equip
      - Siempre → Drop
  ↓ SetState(ContextMenu)
  ↓ OnContextMenuOpened evento
ContextMenuUI
  ↓ OnContextMenuOpened()
  ↓ ShowMenu()
  ↓ StartCoroutine(AnimateScale)
  ↓ ClearOptions()
  ↓ CreateOption() x N
  ↓ Instancia prefabs de opciones
  ↓ UpdateSelectionVisual(0)
  ↓ Menú visible con animación
```

### 4. Usuario Ejecuta Acción (E)

```
PlayerInputHandler
  ↓ OnInteract()
InventoryUIController
  ↓ InteractWithCurrentItem()
  ↓ ExecuteContextAction()
  ↓ Switch según acción:
      Use → inventorySystem.UseCurrentItem()
      Examine → inventorySystem.ExamineCurrentItem()
      Drop → inventorySystem.DropCurrentItem()
      Equip → inventorySystem.EquipWeapon()
  ↓ CloseContextMenu()
  ↓ OnContextMenuClosed evento
ContextMenuUI
  ↓ OnContextMenuClosed()
  ↓ ClearOptions()
  ↓ HideMenu()
  ↓ StartCoroutine(AnimateScaleAndHide)
  ↓ Menú oculto con animación
```

---

## 📊 Estadísticas

**Total de líneas explicadas:** ~958 líneas  
**Archivos de explicación:** 4 documentos  
**Secciones por archivo:** ~15-25 secciones  
**Ejemplos visuales:** ~40+ diagramas ASCII  
**Flujos completos:** ~10 flujos paso a paso

---

## 🎓 Cómo Usar Estas Explicaciones

### Para Aprender

1. **Lee en orden:**
   - Primero: `InventoryUIController` (controlador central)
   - Segundo: `InventorySlotUI` (componente básico)
   - Tercero: `InventoryPanelUI` (gestión de slots)
   - Cuarto: `ContextMenuUI` (menú contextual)

2. **Abre el script original en Visual Studio** junto con la explicación
3. **Compara línea por línea** para entender cada parte
4. **Usa los diagramas de flujo** para visualizar el funcionamiento

### Para Debuggear

1. **Identifica el problema:**
   - ¿El inventario no abre? → `InventoryUIController`
   - ¿Los slots no se actualizan? → `InventoryPanelUI`
   - ¿Un slot no muestra el icono? → `InventorySlotUI`
   - ¿El menú no aparece? → `ContextMenuUI`

2. **Lee la sección específica** en la explicación
3. **Verifica el flujo de datos** en los diagramas
4. **Revisa los eventos** que deberían dispararse

### Para Modificar

1. **Identifica qué quieres cambiar:**
   - Añadir nueva acción → `InventoryUIController` (sección 17)
   - Cambiar animación → `ContextMenuUI` (sección 18-19) o `InventoryPanelUI` (sección 22)
   - Modificar apariencia → `InventorySlotUI` (sección 10-11)
   - Ajustar carrusel → `InventoryPanelUI` (sección 20-23)

2. **Lee las secciones relacionadas**
3. **Modifica el código** siguiendo el patrón existente
4. **Prueba** en Play mode

---

## 🛠️ Herramientas Útiles

### Visual Studio

1. **Abre el archivo .md** en VS Code (mejor renderizado de Markdown)
2. **Usa "Go to Line"** (Ctrl+G) para saltar a líneas específicas
3. **Split View** para ver explicación y código simultáneamente

### Unity

1. **Console** → Activa "Collapse" y filtra por "[INVENTORY UI]"
2. **Inspector** → Pin los GameObjects clave (InventoryPanel, ContextMenuPanel)
3. **Hierarchy** → Observa cómo se crean/destruyen los slots/opciones en Play mode

---

## 📝 Notas Importantes

### Conceptos Clave

1. **Estados del Inventario:**
   - `Closed` → Juego activo (Time.timeScale = 1)
   - `Open` → Navegando slots (Time.timeScale = 0)
   - `ContextMenu` → Navegando acciones (Time.timeScale = 0)

2. **Eventos vs Llamadas Directas:**
   - Los scripts de UI **escuchan eventos** del sistema
   - NO llaman directamente a métodos de otros scripts de UI
   - Esto mantiene el código desacoplado y modular

3. **CanvasGroup para Visibilidad:**
   - Preferido sobre `SetActive()` para evitar perder suscripciones
   - Permite animaciones mientras el GameObject está activo
   - Controla `alpha`, `interactable` y `blocksRaycasts`

4. **Time.unscaledDeltaTime:**
   - Usado en todas las animaciones
   - Permite que la UI se anime aunque el juego esté pausado
   - Esencial para inventarios que pausan el juego

---

## ✅ Checklist de Comprensión

Después de leer las explicaciones, deberías poder responder:

### InventoryUIController
- [ ] ¿Cuáles son los 3 estados del inventario?
- [ ] ¿Cómo se construye dinámicamente la lista de acciones?
- [ ] ¿Qué eventos emite y quién los escucha?
- [ ] ¿Cuándo se pausa/reactiva el juego?

### ContextMenuUI
- [ ] ¿Cómo se crean las opciones del menú?
- [ ] ¿Cómo funciona la animación de escala vertical?
- [ ] ¿Por qué se usa CanvasGroup en lugar de SetActive?
- [ ] ¿Qué hace `Time.unscaledDeltaTime`?

### InventoryPanelUI
- [ ] ¿Cómo se calculan las posiciones del carrusel?
- [ ] ¿Por qué la fórmula usa `-offset * spacing`?
- [ ] ¿Cuándo se usan animaciones vs posicionamiento inmediato?
- [ ] ¿Cómo se determina qué slots son visibles?

### InventorySlotUI
- [ ] ¿Cuándo se muestra el texto de cantidad?
- [ ] ¿Qué diferencia hay entre `emptyIconColor` y `fullIconColor`?
- [ ] ¿Cómo funciona el highlight?
- [ ] ¿Qué hace `UpdateSlot(null)`?

---

## 🚀 Próximos Pasos Sugeridos

1. **Lee las explicaciones** en orden sugerido
2. **Experimenta** modificando valores en el Inspector
3. **Debuggea** con breakpoints y logs
4. **Modifica** el código para añadir nuevas features
5. **Crea** tu propia UI siguiendo estos patrones

---

## 📞 Referencias Adicionales

**Otros documentos en esta carpeta:**
- `ARCHITECTURE.md` → Arquitectura completa del sistema de inventario
- `USE_ITEM_GUIDE.md` → Guía de items usables
- `CAROUSEL_SETUP.md` → Configuración del carrusel
- `CAROUSEL_FIXES.md` → Correcciones aplicadas
- `CONTEXT_MENU_ANIMATION.md` → Detalles de animación del menú

---

¡Disfruta aprendiendo el sistema de UI del inventario! 📚✨
