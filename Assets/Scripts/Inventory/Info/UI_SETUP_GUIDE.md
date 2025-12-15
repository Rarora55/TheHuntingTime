# 📋 Guía de Configuración de UI del Inventario

**Proyecto:** TheHuntProject | **Unity:** 6000.3

---

## 🎯 Objetivo

Crear la interfaz visual completa para el sistema de inventario con:
- 6 slots de inventario
- Highlight del slot seleccionado
- Menú contextual de acciones
- Iconos y cantidades de items

---

## 📦 Scripts Creados

✅ `InventorySlotUI.cs` - Maneja un slot individual
✅ `InventoryPanelUI.cs` - Maneja el panel completo y conexión con eventos
✅ `ContextMenuUI.cs` - Menú contextual de acciones

---

## 🔨 Paso 1: Crear la Jerarquía de UI

### 1.1 Crear Canvas Principal

1. **Clic derecho en Hierarchy** → UI → Canvas
2. Renombrar a `InventoryCanvas`
3. En Inspector:
   - Canvas → Render Mode: **Screen Space - Overlay**
   - Canvas Scaler → UI Scale Mode: **Scale With Screen Size**
   - Canvas Scaler → Reference Resolution: **1920 x 1080**

### 1.2 Crear Panel de Inventario

1. **Clic derecho en InventoryCanvas** → UI → Panel
2. Renombrar a `InventoryPanel`
3. En Inspector (RectTransform):
   - Anchor: **Center**
   - Width: **800**
   - Height: **500**
   - Color del Image: **RGBA(0, 0, 0, 200)** (negro semi-transparente)

---

## 🎨 Paso 2: Crear el Slot Prefab

### 2.1 Crear Estructura del Slot

1. **Clic derecho en InventoryPanel** → UI → Image
2. Renombrar a `SlotTemplate`
3. Configurar RectTransform:
   - Width: **120**
   - Height: **120**

### 2.2 Configurar Fondo del Slot

En `SlotTemplate`:
- Componente **Image** (Background):
  - Color: **RGBA(50, 50, 50, 200)**
  - Sprite: Deja en None o usa un sprite cuadrado

### 2.3 Crear Highlight

1. **Clic derecho en SlotTemplate** → UI → Image
2. Renombrar a `Highlight`
3. Configurar:
   - Anchors: **Stretch** (todas las direcciones)
   - Left/Right/Top/Bottom: **-5** (para que sea más grande que el slot)
   - Color: **RGBA(255, 255, 0, 255)** (amarillo)
   - Deshabilitar por defecto (checkbox desactivado)

### 2.4 Crear Icono del Item

1. **Clic derecho en SlotTemplate** → UI → Image
2. Renombrar a `Icon`
3. Configurar:
   - Anchors: **Stretch**
   - Left/Right/Top/Bottom: **10** (padding interno)
   - Preserve Aspect: **✓** (activado)
   - Color: **RGBA(255, 255, 255, 100)** (semi-transparente cuando vacío)
   - Deshabilitar por defecto

### 2.5 Crear Texto de Cantidad

1. **Clic derecho en SlotTemplate** → UI → Text - TextMeshPro
   (Si aparece ventana de importar TMP Essentials, haz clic en "Import")
2. Renombrar a `QuantityText`
3. Configurar RectTransform:
   - Anchor Preset: **Bottom Right**
   - Pos X: **-10**, Pos Y: **10**
   - Width: **50**, Height: **30**
4. Configurar TextMeshProUGUI:
   - Text: `x99` (solo para previsualizar)
   - Font Size: **18**
   - Alignment: **Bottom Right**
   - Color: **Blanco**
   - Outline: **Activar** con color negro y Size: **0.2**
   - Deshabilitar por defecto

### 2.6 Añadir Script al Slot

1. Selecciona `SlotTemplate`
2. **Add Component** → Busca `InventorySlotUI`
3. Arrastra referencias:
   - **Icon Image** → Arrastra el objeto `Icon`
   - **Quantity Text** → Arrastra el objeto `QuantityText`
   - **Highlight Image** → Arrastra el objeto `Highlight`
   - **Background Image** → Arrastra el componente Image del propio `SlotTemplate`

---

## 📁 Paso 3: Crear el Prefab del Slot

1. **Crea carpeta** `/Assets/Prefabs/UI` (si no existe)
2. **Arrastra `SlotTemplate`** desde Hierarchy a `/Assets/Prefabs/UI/`
3. Esto crea el prefab `SlotTemplate.prefab`
4. **Elimina `SlotTemplate`** de la jerarquía (ya está como prefab)

---

## 📐 Paso 4: Crear el Contenedor de Slots

### 4.1 Crear Grid Layout

1. **Clic derecho en InventoryPanel** → UI → Empty (GameObject vacío con RectTransform)
2. Renombrar a `SlotsContainer`
3. **Add Component** → Busca `Grid Layout Group`
4. Configurar Grid Layout Group:
   - Cell Size: **120 x 120**
   - Spacing: **15 x 15**
   - Start Corner: **Upper Left**
   - Start Axis: **Horizontal**
   - Child Alignment: **Middle Center**
   - Constraint: **Fixed Column Count = 3** (3 columnas, 2 filas)

### 4.2 Centrar el Container

En `SlotsContainer` RectTransform:
- Anchor: **Center**
- Pos X: **0**, Pos Y: **0**
- Width: **390** (3 slots × 120 + 2 espacios × 15)
- Height: **255** (2 slots × 120 + 1 espacio × 15)

---

## 🎬 Paso 5: Crear el Menú Contextual

### 5.1 Crear Panel del Menú

1. **Clic derecho en InventoryPanel** → UI → Panel
2. Renombrar a `ContextMenuPanel`
3. Configurar RectTransform:
   - Anchor: **Center**
   - Pos X: **300** (a la derecha del inventario)
   - Pos Y: **0**
   - Width: **250**
   - Height: **300**
4. Color: **RGBA(30, 30, 30, 230)**
5. **Deshabilitar por defecto**

### 5.2 Crear Título del Menú

1. **Clic derecho en ContextMenuPanel** → UI → Text - TextMeshPro
2. Renombrar a `MenuTitle`
3. Configurar RectTransform:
   - Anchor: **Top Stretch**
   - Height: **50**
   - Left/Right: **10**
   - Top: **-10**
4. Configurar TextMeshProUGUI:
   - Text: `"Actions"`
   - Font Size: **24**
   - Alignment: **Center**
   - Color: **Amarillo**

### 5.3 Crear Contenedor de Opciones

1. **Clic derecho en ContextMenuPanel** → UI → Vertical Layout Group
2. Renombrar a `OptionsContainer`
3. Configurar RectTransform:
   - Anchor: **Stretch Stretch**
   - Left/Right/Bottom: **10**
   - Top: **-60** (debajo del título)
4. Configurar Vertical Layout Group:
   - Spacing: **10**
   - Child Alignment: **Upper Center**
   - Child Force Expand: **Width ✓**, **Height ✗**

### 5.4 Crear Prefab de Opción

1. **Clic derecho en OptionsContainer** → UI → Text - TextMeshPro
2. Renombrar a `OptionTemplate`
3. Configurar:
   - Height: **40**
   - Text: `"Use Item"`
   - Font Size: **20**
   - Alignment: **Left**
4. **Arrastra a `/Assets/Prefabs/UI/`** para crear prefab
5. **Elimina `OptionTemplate`** de la jerarquía

### 5.5 Añadir Script al Context Menu

1. Selecciona `ContextMenuPanel`
2. **Add Component** → `ContextMenuUI`
3. Arrastra referencias:
   - **Options Container** → `OptionsContainer`
   - **Option Prefab** → El prefab `OptionTemplate` desde Project

---

## 🔌 Paso 6: Conectar Todo con InventoryPanelUI

### 6.1 Añadir Script Principal

1. Selecciona `InventoryPanel`
2. **Add Component** → `InventoryPanelUI`

### 6.2 Asignar Referencias

En el Inspector de `InventoryPanelUI`:
- **Inventory System** → Arrastra el GameObject del Player que tiene `InventorySystem`
- **UI Controller** → Arrastra el GameObject del Player que tiene `InventoryUIController`
- **Slots Container** → Arrastra `SlotsContainer`
- **Slot Prefab** → Arrastra el prefab `SlotTemplate` desde Project
- **Inventory Panel** → Arrastra `InventoryPanel` (el panel completo)
- **Context Menu Panel** → Arrastra `ContextMenuPanel`

---

## 🎮 Paso 7: Verificar la Configuración

### 7.1 Jerarquía Final

Tu jerarquía debería verse así:

```
InventoryCanvas
└── InventoryPanel [InventoryPanelUI]
    ├── SlotsContainer [Grid Layout Group]
    │   (Los slots se crearán automáticamente en Play Mode)
    └── ContextMenuPanel [ContextMenuUI]
        ├── MenuTitle (TextMeshPro)
        └── OptionsContainer [Vertical Layout Group]
            (Las opciones se crearán automáticamente)
```

### 7.2 Prefabs Creados

Deberías tener en `/Assets/Prefabs/UI/`:
- ✅ `SlotTemplate.prefab`
- ✅ `OptionTemplate.prefab`

---

## ✅ Paso 8: Probar la UI

### 8.1 Entrar en Play Mode

1. Dale a **Play**
2. La UI debería estar **oculta** por defecto
3. Presiona **Tab** para abrir inventario
   - Debería aparecer el panel con 6 slots vacíos
   - El primer slot debería estar highlighted en amarillo

### 8.2 Probar Navegación

- **W/S** o **Flechas** → Navegar entre slots
- El highlight debería moverse

### 8.3 Añadir Items de Prueba

Si tienes items configurados:
1. Acércate a un item del mundo
2. Presiona **F** para recogerlo
3. Debería aparecer en un slot con su icono
4. Si tiene cantidad > 1, debería mostrar "x2", "x3", etc.

### 8.4 Probar Menú Contextual

1. Con inventario abierto y un item seleccionado
2. Presiona **E** (Interact)
3. Debería aparecer el menú contextual a la derecha
4. Navega con **W/S**
5. Presiona **E** para ejecutar acción

---

## 🎨 Opcional: Mejoras Visuales

### Añadir Fondo con Blur (Avanzado)

1. En `InventoryPanel` → Add Component → **UI Blur** (si tienes paquete instalado)
2. O añade un **Image** con color semi-transparente

### Añadir Iconos Personalizados

1. Importa sprites de UI (bordes, fondos, etc.)
2. Asígnalos en:
   - `SlotTemplate` → Image → Sprite
   - `InventoryPanel` → Image → Sprite

### Añadir Animaciones

1. Selecciona `InventoryPanel`
2. Window → Animation → Animation
3. Crea animaciones de fade in/out

---

## 🐛 Troubleshooting

### Los Slots No Aparecen
- ✅ Verifica que `SlotPrefab` esté asignado en `InventoryPanelUI`
- ✅ Verifica que `SlotsContainer` esté asignado
- ✅ Revisa la consola por errores

### El Highlight No Funciona
- ✅ Verifica que las referencias en `InventorySlotUI` estén conectadas
- ✅ Verifica que `Highlight` tenga componente Image

### El Menú Contextual No Aparece
- ✅ Verifica que `ContextMenuPanel` esté asignado
- ✅ Verifica que `OptionsContainer` y `OptionPrefab` estén asignados en `ContextMenuUI`

### Los Iconos No Se Muestran
- ✅ Verifica que los `ItemData` tengan sprites asignados en el campo `Icon`
- ✅ Verifica que el componente `Image` del icono esté habilitado

---

## 📊 Resumen de Componentes

| GameObject | Scripts | Función |
|------------|---------|---------|
| `InventoryPanel` | InventoryPanelUI | Gestiona toda la UI y eventos |
| `SlotTemplate` (prefab) | InventorySlotUI | Representa un slot individual |
| `ContextMenuPanel` | ContextMenuUI | Muestra opciones de acción |

---

## 🎯 Siguiente Paso

Una vez que tengas la UI funcionando, puedes:
1. Personalizar colores y tamaños
2. Añadir sonidos de UI
3. Añadir animaciones
4. Crear tooltips al pasar el mouse
5. Añadir teclas rápidas (1-6 para selección directa)

---

**¡Tu UI de inventario estilo Resident Evil está lista!** 🎮✨
