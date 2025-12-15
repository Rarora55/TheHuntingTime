# 🎮 Configuración del Input System para Inventario (Simplificado)

## ⚠️ IMPORTANTE: NO EDITAR EL ARCHIVO .inputactions GENERADO

El archivo `Player.inputactions` es un archivo binario generado por Unity. **NO puedes editarlo manualmente**.

---

## 🎯 Nuevo Diseño Simplificado

En lugar de 7 inputs diferentes, ahora solo necesitas **4 inputs** que se comportan diferente según el contexto:

### Flujo del Sistema

```
1. Presionas Tab/I → Abre inventario
2. ← → para navegar entre items
3. E/Enter sobre un item → Abre menú contextual
4. ↑ ↓ en menú contextual → Navegar opciones
5. E/Enter → Ejecutar acción seleccionada
6. Esc/Back → Cerrar menú contextual o inventario
```

### Menú Contextual Dinámico

El menú muestra opciones según el tipo de item:

**Consumible:**
- Use
- Examine
- Drop

**Arma:**
- Equip Primary
- Equip Secondary
- Examine
- Drop

**Key Item:**
- Examine
- Drop

---

## 📝 Pasos para Configurar Input Actions

### 1. Abrir Input Actions Editor

1. En el Project Window, navega a `/Assets/Scripts/NewInput/`
2. Doble click en `Player.inputactions`
3. Se abrirá el **Input Actions Editor**

---

### 2. Añadir Acciones de Inventario

En el Action Map `GamePlay`, añade las siguientes **4 acciones**:

#### A. Inventory Toggle (Abrir/Cerrar Inventario)

```
1. Click en [+] bajo "Actions"
2. Nombre: "InventoryToggle"
3. Action Type: Button
4. Control Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/tab` o `<Keyboard>/i`
- **Gamepad:** `<Gamepad>/select` (Back/Select)

---

#### B. Inventory Navigate (Navegación Universal)

```
1. Nombre: "InventoryNavigate"
2. Action Type: Value
3. Control Type: Axis (float)
```

**Bindings:**
- **Keyboard Left/Right:** 
  - Left: `<Keyboard>/leftArrow` → Processor: Scale → Factor: -1
  - Right: `<Keyboard>/rightArrow` → Value: 1
- **Keyboard Up/Down (para menú contextual):**
  - Up: `<Keyboard>/upArrow` → Processor: Scale → Factor: -1
  - Down: `<Keyboard>/downArrow` → Value: 1
- **Gamepad D-Pad:** `<Gamepad>/dpad/x` y `<Gamepad>/dpad/y`

**NOTA:** Este input sirve para:
- **← →** cuando el inventario está abierto (navegar items)
- **↑ ↓** cuando el menú contextual está abierto (navegar opciones)

---

#### C. Inventory Interact (Interactuar/Confirmar)

```
1. Nombre: "InventoryInteract"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/e` o `<Keyboard>/enter`
- **Gamepad:** `<Gamepad>/buttonSouth` (A/Cross)

**NOTA:** Este input sirve para:
- **Abrir menú contextual** cuando estás en el inventario
- **Ejecutar acción** cuando estás en el menú contextual

---

#### D. Inventory Cancel (Cancelar/Volver)

```
1. Nombre: "InventoryCancel"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/escape` o `<Keyboard>/backspace`
- **Gamepad:** `<Gamepad>/buttonEast` (B/Circle)

**NOTA:** Este input sirve para:
- **Cerrar menú contextual** cuando estás en el menú
- **Cerrar inventario** cuando estás en el inventario principal

---

### 3. Guardar Input Actions

1. Click en **"Save Asset"** en la esquina superior
2. Espera a que Unity recompile

---

### 4. Conectar en PlayerInputHandler

El script `PlayerInputHandler.cs` ya tiene los métodos implementados. Ahora necesitas conectarlos:

1. Selecciona el GameObject `Player` en la jerarquía
2. Busca el componente `Player Input`
3. Conecta los callbacks:

```
InventoryToggle → PlayerInputHandler.OnInventoryToggleInput
InventoryNavigate → PlayerInputHandler.OnInventoryNavigateInput
InventoryInteract → PlayerInputHandler.OnInventoryInteractInput
InventoryCancel → PlayerInputHandler.OnInventoryCancelInput
```

---

## 🎯 Resumen de Input Mapping

| Acción | Keyboard | Gamepad | Método |
|--------|----------|---------|--------|
| Toggle Inventory | Tab / I | Select | `OnInventoryToggleInput()` |
| Navigate Left/Right | ← → | D-Pad X | `OnInventoryNavigateInput()` |
| Navigate Up/Down | ↑ ↓ | D-Pad Y | `OnInventoryNavigateInput()` |
| Interact/Confirm | E / Enter | A (South) | `OnInventoryInteractInput()` |
| Cancel/Back | Esc | B (East) | `OnInventoryCancelInput()` |

---

## 🎮 Flujo de Uso

### Escenario 1: Usar una Poción

```
1. Tab → Abre inventario
2. ← → → Navega a la poción
3. E → Abre menú contextual
4. "Use" está seleccionado por defecto
5. E → Usa la poción (cura vida)
6. Menú se cierra automáticamente
7. Tab → Cierra inventario
```

### Escenario 2: Equipar un Arma

```
1. Tab → Abre inventario
2. ← → → Navega al arma
3. E → Abre menú contextual
4. ↓ ↓ → Selecciona "Equip Primary"
5. E → Equipa el arma
6. Menú se cierra automáticamente
7. Tab → Cierra inventario
```

### Escenario 3: Soltar un Item

```
1. Tab → Abre inventario
2. ← → → Navega al item
3. E → Abre menú contextual
4. ↓ ↓ ↓ → Selecciona "Drop"
5. E → Suelta el item
6. Menú se cierra automáticamente
```

---

## ✅ Ventajas del Nuevo Sistema

1. **Menos inputs** - Solo 4 en lugar de 7
2. **Contexto inteligente** - Las mismas teclas hacen cosas diferentes según el estado
3. **Más intuitivo** - Similar a Silent Hill y RE4
4. **Menos conflictos** - No necesitas recordar 7 teclas diferentes
5. **Extensible** - Fácil añadir nuevas acciones al menú contextual

---

## ✅ Verificación

Para verificar que todo está conectado:

1. Entra en Play Mode
2. Presiona **Tab** → Debería abrir inventario y pausar el juego
3. Presiona **← →** → Debería navegar entre slots
4. Presiona **E** sobre un item → Debería abrir menú contextual
5. Presiona **↑ ↓** → Debería navegar opciones del menú
6. Presiona **E** → Debería ejecutar acción
7. Presiona **Esc** → Debería cerrar menú/inventario

---

## 📌 Notas Importantes

- El inventario **pausa el juego** (`Time.timeScale = 0`)
- El menú contextual es **dinámico** (solo muestra acciones válidas)
- Si un consumible no se puede usar, "Use" no aparece en el menú
- Las armas se pueden equipar sin salir del inventario

### 1. Abrir Input Actions Editor

1. En el Project Window, navega a `/Assets/Scripts/NewInput/`
2. Doble click en `Player.inputactions`
3. Se abrirá el **Input Actions Editor**

---

### 2. Crear Nueva Action Map (Opcional)

Si quieres separar las acciones del inventario:

1. Click en el botón `+` junto a "Action Maps"
2. Nombre: `Inventory`
3. **O** puedes añadir todo al Action Map `GamePlay` existente

**Recomendación:** Añádelas a `GamePlay` para simplicidad.

---

### 3. Añadir Acciones de Inventario

En el Action Map `GamePlay`, añade las siguientes acciones:

#### A. Inventory Navigate (Navegación del Carrusel)

```
1. Click en [+] bajo "Actions"
2. Nombre: "InventoryNavigate"
3. Action Type: Value
4. Control Type: Axis (float)
```

**Bindings:**
- **Keyboard Left Arrow:** `<Keyboard>/leftArrow` → Value: -1
- **Keyboard Right Arrow:** `<Keyboard>/rightArrow` → Value: 1
- **Gamepad D-Pad X:** `<Gamepad>/dpad/x`

**Cómo añadir bindings:**
```
1. Click derecho en "InventoryNavigate"
2. Add Binding → Keyboard
3. Path: <Keyboard>/leftArrow
4. Processors: Click [+] → Scale → Factor: -1
```

---

#### B. Inventory Use (Usar Item)

```
1. Click en [+] bajo "Actions"
2. Nombre: "InventoryUse"
3. Action Type: Button
4. Control Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/e` o `<Keyboard>/enter`
- **Gamepad:** `<Gamepad>/buttonSouth` (A/Cross)

---

#### C. Inventory Examine (Examinar Item)

```
1. Nombre: "InventoryExamine"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/q`
- **Gamepad:** `<Gamepad>/buttonWest` (X/Square)

---

#### D. Inventory Drop (Soltar Item)

```
1. Nombre: "InventoryDrop"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/x`
- **Gamepad:** `<Gamepad>/buttonEast` (B/Circle)

---

#### E. Equip Primary (Equipar Slot Principal)

```
1. Nombre: "EquipPrimary"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/1`
- **Gamepad:** `<Gamepad>/leftShoulder` (LB/L1)

---

#### F. Equip Secondary (Equipar Slot Secundario)

```
1. Nombre: "EquipSecondary"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/2`
- **Gamepad:** `<Gamepad>/rightShoulder` (RB/R1)

---

#### G. Swap Weapons (Intercambiar Armas)

```
1. Nombre: "SwapWeapons"
2. Action Type: Button
```

**Bindings:**
- **Keyboard:** `<Keyboard>/z` o `<Keyboard>/tab`
- **Gamepad:** `<Gamepad>/buttonNorth` (Y/Triangle)

---

### 4. Guardar Input Actions

1. Click en **"Save Asset"** en la esquina superior
2. Espera a que Unity recompile

---

### 5. Conectar en PlayerInputHandler

El script `PlayerInputHandler.cs` ya tiene los métodos implementados. Ahora necesitas conectarlos:

1. Selecciona el GameObject `Player` en la jerarquía
2. Busca el componente `Player Input` (si existe) o añádelo
3. En el componente `Player Input`:
   - **Actions:** Asigna `Player.inputactions`
   - **Behavior:** Invoke Unity Events o Send Messages

#### Si usas **Invoke Unity Events:**

1. En cada evento de Input Actions, añade el callback correspondiente:

```
InventoryNavigate → PlayerInputHandler.OnInventoryNavigateInput
InventoryUse → PlayerInputHandler.OnInventoryUseInput
InventoryExamine → PlayerInputHandler.OnInventoryExamineInput
InventoryDrop → PlayerInputHandler.OnInventoryDropInput
EquipPrimary → PlayerInputHandler.OnEquipPrimaryInput
EquipSecondary → PlayerInputHandler.OnEquipSecondaryInput
SwapWeapons → PlayerInputHandler.OnSwapWeaponsInput
```

#### Si usas **Send Messages o Broadcast:**

Los métodos ya están nombrados correctamente con el prefijo `On` + nombre de acción + `Input`.

---

## 🎯 Resumen de Input Mapping

| Acción | Keyboard | Gamepad | Método |
|--------|----------|---------|--------|
| Navigate Left | ← | D-Pad Left | `OnInventoryNavigateInput()` |
| Navigate Right | → | D-Pad Right | `OnInventoryNavigateInput()` |
| Use Item | E / Enter | A (South) | `OnInventoryUseInput()` |
| Examine | Q | X (West) | `OnInventoryExamineInput()` |
| Drop | X | B (East) | `OnInventoryDropInput()` |
| Equip Primary | 1 | LB (L Shoulder) | `OnEquipPrimaryInput()` |
| Equip Secondary | 2 | RB (R Shoulder) | `OnEquipSecondaryInput()` |
| Swap Weapons | Z / Tab | Y (North) | `OnSwapWeaponsInput()` |

---

## ✅ Verificación

Para verificar que todo está conectado:

1. Entra en Play Mode
2. Abre la consola
3. Presiona las teclas configuradas
4. Deberías ver logs como:
   ```
   [INVENTORY] Selected slot 1
   [INVENTORY] No item selected
   ```

---

## 🐛 Troubleshooting

### No se detecta el input

1. ✅ Verifica que `Player Input` component esté en el Player
2. ✅ Verifica que `Player.inputactions` esté asignado
3. ✅ Verifica que el Action Map esté activado (default en Play Mode)
4. ✅ Verifica que `PlayerInputHandler` esté en el mismo GameObject

### Los métodos no se llaman

1. ✅ Verifica que el comportamiento sea correcto (Invoke Unity Events vs Send Messages)
2. ✅ Verifica que los nombres de los métodos coincidan exactamente
3. ✅ Revisa que `InventorySystem` esté añadido al Player

---

## 📌 Notas Adicionales

- **Por ahora, el inventario se controla siempre**, más adelante añadiremos lógica para abrir/cerrar UI
- Los inputs están separados de la UI - el sistema funciona con o sin interfaz visual
- Los métodos en `PlayerInputHandler` verifican si `inventorySystem != null` antes de ejecutar
