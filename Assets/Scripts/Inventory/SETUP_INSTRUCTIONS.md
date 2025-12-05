# 🎯 Instrucciones de Setup - Paso a Paso

## ⏱️ Tiempo Total: 15-20 minutos

---

## 📋 Paso 1: Añadir Componente al Player (2 min)

### 1.1 Seleccionar Player
```
1. En la ventana Hierarchy, busca "Player"
2. Click en el GameObject "Player"
```

### 1.2 Añadir InventoryUIController
```
1. En Inspector, baja hasta el final
2. Click en botón "Add Component"
3. Escribe "InventoryUIController"
4. Selecciona "Inventory UI Controller"
```

### 1.3 Verificar Componentes
Tu Player debe tener ahora:
```
✅ InventorySystem          (debe existir ya)
✅ InventoryUIController    (recién añadido)
✅ PlayerInputHandler       (debe existir ya)
```

### 1.4 (Opcional) Añadir Debugger
```
1. Add Component → "InventoryDebugger"
2. Esto te permitirá usar F1-F4 para testing
```

---

## 🎮 Paso 2: Configurar Input Actions (10 min)

### 2.1 Abrir Input Actions Editor
```
1. En Project Window, navega a: Assets/Scripts/NewInput/
2. Encuentra "Player.inputactions"
3. Doble click en "Player.inputactions"
4. Se abre el Input Actions Editor
```

### 2.2 Verificar Action Map
```
1. En la izquierda, debe estar seleccionado "GamePlay"
2. Si no existe, créalo (Add Action Map → "GamePlay")
```

### 2.3 Crear Acción: InventoryToggle

**A. Crear Acción**
```
1. En la columna "Actions", click en [+]
2. Nombre: "InventoryToggle"
3. Action Type: Button
4. Control Type: Button
```

**B. Añadir Binding de Teclado**
```
1. Click derecho en "InventoryToggle"
2. Selecciona "Add Binding"
3. Click en "<No Binding>"
4. Presiona la tecla "Tab"
5. Se debería mostrar: "Keyboard/tab"
```

**C. Añadir Binding Alternativo (Opcional)**
```
1. Click derecho en "InventoryToggle" otra vez
2. "Add Binding"
3. Presiona la tecla "I"
4. Resultado: "Keyboard/i"
```

### 2.4 Crear Acción: InventoryNavigate

**A. Crear Acción**
```
1. Click en [+] bajo Actions
2. Nombre: "InventoryNavigate"
3. Action Type: Value
4. Control Type: Axis (float)
```

**B. Añadir Binding: Left Arrow**
```
1. Click derecho en "InventoryNavigate"
2. "Add Binding"
3. Click en "<No Binding>"
4. Presiona "←" (flecha izquierda)
5. Se muestra: "Keyboard/leftArrow"
6. IMPORTANTE: Click en "Keyboard/leftArrow"
7. En Inspector, busca "Processors"
8. Click en [+] → "Scale"
9. En "Factor" escribe: -1
```

**C. Añadir Binding: Right Arrow**
```
1. Click derecho en "InventoryNavigate"
2. "Add Binding"
3. Presiona "→" (flecha derecha)
4. Se muestra: "Keyboard/rightArrow"
5. (No necesita processor, valor por defecto es 1)
```

**D. Añadir Binding: Up Arrow**
```
1. Click derecho en "InventoryNavigate"
2. "Add Binding"
3. Presiona "↑" (flecha arriba)
4. Click en "Keyboard/upArrow"
5. Processors → [+] → "Scale"
6. Factor: -1
```

**E. Añadir Binding: Down Arrow**
```
1. Click derecho en "InventoryNavigate"
2. "Add Binding"
3. Presiona "↓" (flecha abajo)
4. (No necesita processor)
```

**Resultado Final:**
```
InventoryNavigate
  ├─ Keyboard/leftArrow  (Scale: -1)
  ├─ Keyboard/rightArrow
  ├─ Keyboard/upArrow    (Scale: -1)
  └─ Keyboard/downArrow
```

### 2.5 Crear Acción: InventoryInteract

**A. Crear Acción**
```
1. Click en [+] bajo Actions
2. Nombre: "InventoryInteract"
3. Action Type: Button
4. Control Type: Button
```

**B. Añadir Bindings**
```
1. Click derecho → "Add Binding"
2. Presiona "E"
3. Click derecho → "Add Binding"
4. Presiona "Enter"
```

**Resultado:**
```
InventoryInteract
  ├─ Keyboard/e
  └─ Keyboard/enter
```

### 2.6 Crear Acción: InventoryCancel

**A. Crear Acción**
```
1. Click en [+] bajo Actions
2. Nombre: "InventoryCancel"
3. Action Type: Button
```

**B. Añadir Binding**
```
1. Click derecho → "Add Binding"
2. Presiona "Escape"
```

**Resultado:**
```
InventoryCancel
  └─ Keyboard/escape
```

### 2.7 Guardar Input Actions
```
1. En la esquina superior del Input Actions Editor
2. Click en "Save Asset"
3. Espera a que Unity recompile (barra de progreso abajo)
```

---

## 🔗 Paso 3: Conectar Callbacks (3 min)

### 3.1 Seleccionar Player
```
1. En Hierarchy, selecciona "Player"
2. En Inspector, busca el componente "Player Input"
```

### 3.2 Verificar Events
```
1. En "Player Input", busca la sección "Events"
2. Debe tener "Behavior: Invoke Unity Events" o "Send Messages"
3. Si dice "Send Messages", déjalo así (ya está configurado)
```

### 3.3 Conectar Eventos (si usa Invoke Unity Events)

Si tu `Player Input` tiene "Behavior: Invoke Unity Events":

**A. InventoryToggle**
```
1. Busca "Inventory Toggle (Action)"
2. Click en [+] si está vacío
3. Arrastra el GameObject "Player" al campo de objeto
4. En dropdown, selecciona: PlayerInputHandler → OnInventoryToggleInput
```

**B. InventoryNavigate**
```
1. Busca "Inventory Navigate (Action)"
2. [+] → Arrastra "Player"
3. PlayerInputHandler → OnInventoryNavigateInput
```

**C. InventoryInteract**
```
1. Busca "Inventory Interact (Action)"
2. [+] → Arrastra "Player"
3. PlayerInputHandler → OnInventoryInteractInput
```

**D. InventoryCancel**
```
1. Busca "Inventory Cancel (Action)"
2. [+] → Arrastra "Player"
3. PlayerInputHandler → OnInventoryCancelInput
```

### 3.4 Si usa Send Messages

Si tu `Player Input` tiene "Behavior: Send Messages":

✅ **No necesitas hacer nada más!** Los métodos en `PlayerInputHandler` ya tienen los nombres correctos:
- `OnInventoryToggleInput()`
- `OnInventoryNavigateInput()`
- `OnInventoryInteractInput()`
- `OnInventoryCancelInput()`

Unity automáticamente los conectará por nombre.

---

## 🧪 Paso 4: Testing (5 min)

### 4.1 Preparar Items de Prueba (Opcional)

**Si tienes InventoryDebugger:**
```
1. Selecciona "Player" en Hierarchy
2. En Inspector, busca "Inventory Debugger"
3. Expande "Debug Items"
4. Asigna cualquier ItemData a:
   - Test Consumable
   - Test Weapon
   - Test Ammo
```

**Si no tienes items aún:**
```
No te preocupes, igual puedes probar el sistema.
Los items se pueden crear después.
```

### 4.2 Entrar en Play Mode
```
1. Click en el botón "Play" (▶) arriba en el centro
2. Espera a que cargue la escena
```

### 4.3 Test Básico: Abrir/Cerrar

**Prueba 1: Toggle**
```
1. Presiona "Tab"
   ✅ Debe aparecer mensaje en Console: "Inventory opened"
   ✅ El juego debe pausarse (Time.timeScale = 0)

2. Presiona "Tab" de nuevo
   ✅ Debe aparecer: "Inventory closed"
   ✅ El juego debe resumirse
```

**Prueba 2: Cerrar con Escape**
```
1. Presiona "Tab" (abre)
2. Presiona "Esc" (cierra)
   ✅ Debe cerrar el inventario
```

### 4.4 Test con Debug Items

**Si tienes InventoryDebugger configurado:**

```
1. En Play Mode, presiona F1
   ✅ Console: "Item Added to slot 0..."

2. Presiona F1 varias veces (hasta 6)
   ✅ Console muestra items añadidos

3. Presiona Tab (abre inventario)

4. Presiona ← →
   ✅ Console: "Selection Changed: 0 → 1..."

5. Presiona E (sobre un item)
   ✅ Console: "Context menu opened..."

6. Presiona ↑ ↓
   ✅ Console: "Context menu selection: Use/Examine/Drop"

7. Presiona E (ejecuta acción)
   ✅ Console muestra acción ejecutada

8. Presiona F4
   ✅ Console imprime estado completo del inventario
```

### 4.5 Verificar Debug Panel

**Si InventoryDebugger está activo:**

En la esquina superior izquierda de la Game View debe aparecer:
```
┌─────────────────────────────┐
│ 🎒 INVENTORY DEBUGGER       │
│ State: Open                 │
│ Selected: 0                 │
│ Full: False                 │
│                             │
│ --- Quick Add ---           │
│ F1: Add Test Consumable     │
│ F2: Add Test Weapon         │
│ F3: Add Test Ammo           │
│ F4: Print Inventory         │
│                             │
│ --- Current Item ---        │
│ Name: Health Potion         │
│ Type: Consumable            │
│                             │
│ --- Context Menu ---        │
│ ► Use                       │
│   Examine                   │
│   Drop                      │
└─────────────────────────────┘
```

---

## ✅ Checklist Final

### Configuración
- [ ] `InventoryUIController` añadido al Player
- [ ] 4 Input Actions creadas en `Player.inputactions`
- [ ] Callbacks conectados (o "Send Messages" configurado)
- [ ] (Opcional) `InventoryDebugger` añadido

### Testing Básico
- [ ] Tab abre inventario
- [ ] Tab cierra inventario
- [ ] Esc cierra inventario
- [ ] Console muestra mensajes de apertura/cierre

### Testing con Items
- [ ] F1 añade item (si debugger configurado)
- [ ] ← → navega items
- [ ] E abre menú contextual
- [ ] ↑ ↓ navega en menú
- [ ] E ejecuta acción
- [ ] Esc cierra menú
- [ ] F4 imprime estado

### Verificación Visual
- [ ] Debug panel visible en Game View
- [ ] Estado del inventario actualiza
- [ ] Menú contextual se muestra cuando está abierto

---

## 🐛 Troubleshooting

### "No pasa nada al presionar Tab"

**Posibles causas:**
1. Input Actions no guardadas
   - Solución: Abre `Player.inputactions`, click "Save Asset"

2. Callbacks no conectados
   - Solución: Verifica el componente "Player Input" en Player

3. `InventoryUIController` no añadido
   - Solución: Add Component → InventoryUIController

### "Console muestra errores"

**Error: NullReferenceException en PlayerInputHandler**
- Causa: `InventoryUIController` no está en Player
- Solución: Añadir componente al Player

**Error: Missing Component**
- Causa: Falta `InventorySystem`
- Solución: Añadir `InventorySystem` al Player

### "F1-F4 no funcionan"

**Causa:** `InventoryDebugger` no añadido o no configurado
- Solución: Add Component → InventoryDebugger
- Asignar items de prueba en Inspector

### "Navegación no funciona"

**Causa:** Action Type incorrecto en InventoryNavigate
- Solución: Debe ser "Value" con "Axis (float)"

**Causa:** Scale processor faltante
- Solución: Left y Up arrow necesitan "Scale: -1"

---

## 📖 Próximos Pasos

Una vez que todo funcione:

### Crear Items Reales
```
1. Crear carpeta: Assets/Data/Items/
2. Click derecho → Create → Inventory → Consumable Item
3. Configurar propiedades en Inspector
4. Repetir para Weapons, Ammo, Keys
```

### Integrar con UI Visual
```
1. Crear Canvas para inventario
2. Crear slots visuales (sprites)
3. Suscribirse a eventos de InventoryUIController
4. Actualizar UI según estado
```

### Añadir Pickups en Escena
```
1. Crear GameObject vacío
2. Añadir Sprite Renderer
3. Añadir componente "Pickup Item"
4. Asignar ItemData
5. Configurar layer "Interactable"
```

---

## 🎉 ¡Felicidades!

Si llegaste hasta aquí y todos los tests pasan:

✅ **Tu sistema de inventario está completamente funcional!**

Ahora tienes:
- ✅ Sistema de datos robusto
- ✅ Input simplificado (4 teclas)
- ✅ Menú contextual dinámico
- ✅ Pausa automática
- ✅ Debug tools integrados

**Siguiente fase:** Crear UI visual y sistema de examinación 3D

---

**¿Problemas?** Revisa:
- `QUICKSTART.md` - Setup rápido
- `INPUT_SETUP.md` - Detalles de input
- `README.md` - Documentación completa
- `EXECUTIVE_SUMMARY.md` - Resumen ejecutivo
