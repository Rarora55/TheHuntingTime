# 🚀 Quick Start - Sistema de Inventario Simplificado

## ✨ Cambios del Nuevo Sistema

Hemos simplificado el sistema de input para hacerlo más intuitivo:

### Antes (7 inputs)
- ❌ Navigate Left/Right
- ❌ Use Item
- ❌ Examine Item
- ❌ Drop Item
- ❌ Equip Primary
- ❌ Equip Secondary
- ❌ Swap Weapons

### Ahora (4 inputs)
- ✅ **Toggle** - Abrir/Cerrar inventario (Tab/I)
- ✅ **Navigate** - Moverse en inventario y menús (← → ↑ ↓)
- ✅ **Interact** - Confirmar acciones (E/Enter)
- ✅ **Cancel** - Volver/Cancelar (Esc)

---

## 🎮 Cómo Funciona

### Sistema de Menú Contextual

Cuando presionas **E** sobre un item, se abre un **menú contextual** con las acciones disponibles:

```
┌─────────────────────┐
│  Health Potion x3   │
├─────────────────────┤
│  ► Use              │  ← Navegas con ↑ ↓
│    Examine          │
│    Drop             │
└─────────────────────┘
```

El menú es **dinámico** y solo muestra opciones válidas:
- Si la salud está llena, "Use" no aparece
- Solo las armas muestran "Equip Primary/Secondary"
- Key items solo tienen "Examine" y "Drop"

---

## 🔧 Setup Rápido (5 minutos)

### 1. Añadir Componentes al Player

```
Player GameObject
├── InventorySystem          ✅ (ya existe)
├── InventoryUIController    ⬅️ AÑADIR ESTE
└── InventoryDebugger        ⬅️ OPCIONAL (para testing)
```

### 2. Configurar Input Actions

Abre `Player.inputactions` y añade **4 acciones**:

| Nombre | Tipo | Bindings |
|--------|------|----------|
| `InventoryToggle` | Button | Tab, I |
| `InventoryNavigate` | Axis | ← → ↑ ↓ |
| `InventoryInteract` | Button | E, Enter |
| `InventoryCancel` | Button | Esc |

### 3. Conectar Callbacks

En el componente `Player Input`, conecta:
```
InventoryToggle   → OnInventoryToggleInput
InventoryNavigate → OnInventoryNavigateInput
InventoryInteract → OnInventoryInteractInput
InventoryCancel   → OnInventoryCancelInput
```

### 4. ¡Listo!

Presiona **Play** y prueba:
- `F1` - Añadir poción
- `F2` - Añadir arma
- `Tab` - Abrir inventario
- `E` - Menú contextual

---

## 📖 Ejemplo de Uso

### Usar una Poción

```
Tab          → Abre inventario
← →          → Navega a poción
E            → Abre menú
              [► Use, Examine, Drop]
E            → Usa poción (cura vida)
              Menú se cierra automáticamente
Tab          → Cierra inventario
```

### Equipar Arma

```
Tab          → Abre inventario
← →          → Navega a pistola
E            → Abre menú
              [► Equip Primary, Equip Secondary, Examine, Drop]
↓            → Selecciona "Equip Primary"
E            → Equipa arma
Tab          → Cierra inventario
```

---

## 🏗️ Arquitectura

```
PlayerInputHandler
    ↓ input
InventoryUIController (nuevo)
    ↓ lógica de UI/estado
InventorySystem
    ↓ lógica de datos
ItemData (ScriptableObjects)
```

### Nuevos Componentes

**InventoryUIController**
- Maneja estados (Closed, Open, ContextMenu)
- Construye menú contextual dinámico
- Pausa el juego cuando está abierto
- Delega acciones al InventorySystem

**Nuevos Enums**
- `InventoryState` - Closed, Open, ContextMenu
- `ItemContextAction` - Use, Examine, Drop, EquipPrimary, EquipSecondary

---

## 🎯 Estados del Sistema

```
CLOSED
  │
  │ Tab
  ↓
OPEN (pausado, navegación con ← →)
  │
  │ E
  ↓
CONTEXT MENU (navegación con ↑ ↓)
  │
  │ E → ejecuta acción
  │ Esc → vuelve a OPEN
  ↓
OPEN
  │
  │ Esc
  ↓
CLOSED
```

---

## 🔍 Debug

El `InventoryDebugger` ahora muestra:
- Estado actual (Closed/Open/ContextMenu)
- Item seleccionado
- **Menú contextual en tiempo real**

```
🎒 INVENTORY DEBUGGER
State: ContextMenu
Selected: 0
Full: False

--- Context Menu ---
  Use
► Examine
  Drop
```

---

## 📚 Documentación Completa

- `README.md` - Guía completa del sistema
- `INPUT_SETUP.md` - Configuración detallada de inputs
- `IMPLEMENTATION_STATUS.md` - Estado del proyecto

---

## ✅ Ventajas

1. **Menos teclas** - Solo 4 en lugar de 7
2. **Contexto inteligente** - Acciones se adaptan al item
3. **Estilo Silent Hill** - Menú contextual clásico
4. **Extensible** - Fácil añadir nuevas acciones
5. **Pausa automática** - El juego se pausa al abrir inventario

---

¡Ahora puedes configurar los inputs y probar el sistema! 🎮
