# Configuración de Slots de Armas - Estilo Resident Evil

## Resumen

Sistema de UI para mostrar las armas equipadas (Primary y Secondary) dentro del panel de inventario, con estilo visual similar a Resident Evil clásico.

## Estructura del GameObject

```
InventoryPanel
├── SlotsContainer (inventario general)
├── ContextMenuPanel
├── HealthPanel
└── WeaponEquipmentPanel (NUEVO)
    ├── PrimaryWeaponSlot
    │   ├── SlotBackground (Image)
    │   ├── WeaponIcon (Image)
    │   ├── SlotLabel (TextMeshProUGUI)
    │   └── WeaponName (TextMeshProUGUI)
    └── SecondaryWeaponSlot
        ├── SlotBackground (Image)
        ├── WeaponIcon (Image)
        ├── SlotLabel (TextMeshProUGUI)
        └── WeaponName (TextMeshProUGUI)
```

## Configuración Paso a Paso

### Paso 1: Crear el Panel de Equipamiento

1. Haz clic derecho en **InventoryPanel** > `Create Empty`
2. Renómbralo a **WeaponEquipmentPanel**
3. Configura su **RectTransform**:
   ```
   Anchors: Bottom-Right
   Pivot: (1, 0)
   Pos X: -20
   Pos Y: 20
   Width: 180
   Height: 160
   ```

### Paso 2: Crear el Slot de Arma Principal

1. Haz clic derecho en **WeaponEquipmentPanel** > `Create Empty`
2. Renómbralo a **PrimaryWeaponSlot**
3. Añade componente **Image** (será el fondo)
4. Añade componente **WeaponSlotUI**
5. Configura **RectTransform**:
   ```
   Anchors: Top-Stretch
   Height: 70
   Left: 0, Right: 0
   Pos Y: 0
   ```

#### 2.1 Fondo del Slot Principal

En el componente **Image** del PrimaryWeaponSlot:
```
Color: (60, 60, 60, 200) - Gris oscuro semi-transparente
```

#### 2.2 Icono del Arma Principal

1. Haz clic derecho en **PrimaryWeaponSlot** > `UI > Image`
2. Renómbralo a **WeaponIcon**
3. Configura **RectTransform**:
   ```
   Anchors: Left-Center
   Pos X: 35
   Pos Y: 0
   Width: 50
   Height: 50
   ```
4. En componente **Image**:
   ```
   Preserve Aspect: ✓
   Color: White
   ```

#### 2.3 Etiqueta del Slot Principal

1. Haz clic derecho en **PrimaryWeaponSlot** > `UI > Text - TextMeshPro`
2. Renómbralo a **SlotLabel**
3. Configura **RectTransform**:
   ```
   Anchors: Top-Stretch
   Height: 20
   Left: 5, Right: -5
   Pos Y: -5
   ```
4. En componente **TextMeshProUGUI**:
   ```
   Text: "Primary"
   Font Size: 12
   Alignment: Center
   Color: RGB(200, 200, 200)
   Font Style: Bold
   ```

#### 2.4 Nombre del Arma Principal

1. Haz clic derecho en **PrimaryWeaponSlot** > `UI > Text - TextMeshPro`
2. Renómbralo a **WeaponName**
3. Configura **RectTransform**:
   ```
   Anchors: Bottom-Stretch
   Height: 18
   Left: 5, Right: -5
   Pos Y: 3
   ```
4. En componente **TextMeshProUGUI**:
   ```
   Text: "Empty"
   Font Size: 11
   Alignment: Center
   Color: RGB(180, 180, 180)
   ```

#### 2.5 Configurar Script WeaponSlotUI del Primary

En el Inspector de **PrimaryWeaponSlot**, configura el componente **WeaponSlotUI**:

**Slot Configuration:**
```
Slot Type: Primary
```

**UI Elements:**
```
Slot Background: (arrastra el mismo PrimaryWeaponSlot - su componente Image)
Weapon Icon: WeaponIcon
Weapon Name Text: WeaponName
Slot Label: SlotLabel
```

**Visual States:**
```
Empty Slot Color: RGB(76, 76, 76, 128)
Equipped Slot Color: RGB(51, 153, 51, 204)
Selected Slot Color: RGB(204, 204, 51, 255)
Empty Weapon Sprite: (opcional - deja None por ahora)
```

### Paso 3: Crear el Slot de Arma Secundaria

1. **Duplica** el **PrimaryWeaponSlot** (Ctrl+D)
2. Renómbralo a **SecondaryWeaponSlot**
3. Muévelo dentro de **WeaponEquipmentPanel** si no está
4. Configura su **RectTransform**:
   ```
   Anchors: Bottom-Stretch
   Height: 70
   Left: 0, Right: 0
   Pos Y: 0
   ```

#### 3.1 Configurar Script WeaponSlotUI del Secondary

En el Inspector de **SecondaryWeaponSlot**, modifica el componente **WeaponSlotUI**:

**Slot Configuration:**
```
Slot Type: Secondary  ← CAMBIAR ESTO
```

Todo lo demás permanece igual.

Verifica que **SlotLabel/Text** diga "Secondary".

### Paso 4: Configurar el Panel Principal

1. Selecciona **WeaponEquipmentPanel**
2. Añade componente **WeaponEquipmentPanel**
3. Configura en el Inspector:

**References:**
```
Weapon Manager: (Arrastra el GameObject que tiene WeaponInventoryManager - normalmente el InventoryCanvas)
UI Controller: (Arrastra el InventoryUIController)
```

**Weapon Slots:**
```
Primary Slot: PrimaryWeaponSlot
Secondary Slot: SecondaryWeaponSlot
```

**Settings:**
```
Auto Hide When Empty: ☐ (desactivado)
Canvas Group: (opcional - si quieres fade effects)
```

### Paso 5: Añadir Layout (Opcional)

Para organizar mejor los slots, añade un **Vertical Layout Group** al **WeaponEquipmentPanel**:

```
Child Force Expand Height: ☐
Child Force Expand Width: ✓
Child Control Height: ✓
Child Control Width: ✓
Spacing: 10
Padding: 5 en todos lados
```

## Resultado Final

Deberías tener dos slots visibles en la esquina inferior derecha del inventario:

```
┌──────────────────┐
│    PRIMARY       │
│  [🔫]  Pistol    │
└──────────────────┘

┌──────────────────┐
│   SECONDARY      │
│  [🗡️]  Knife     │
└──────────────────┘
```

## Características

### ✅ Feedback Visual

- **Slot vacío**: Gris oscuro, texto "Empty"
- **Slot con arma**: Verde, muestra icono y nombre
- **Slot seleccionado**: Amarillo brillante
- **Animación de equipar**: Pulso al equipar arma

### ✅ Auto-sincronización

El sistema se conecta automáticamente a:
- `WeaponInventoryManager.OnWeaponEquipped`
- `WeaponInventoryManager.OnWeaponUnequipped`
- `WeaponInventoryManager.OnWeaponsSwapped`

### ✅ Información Mostrada

- **Label del slot**: "Primary" o "Secondary"
- **Icono del arma**: Usa `EquipIcon` o `ItemIcon` del arma
- **Nombre del arma**: Muestra el `ItemName`

## Testing

Para probar el sistema:

1. **Crea armas** (ScriptableObjects):
   - `Assets > Create > Inventory > Weapon Item`
   - Configura nombre, icono, stats
   
2. **Añade al inventario** en runtime

3. **Equipa desde código**:
```csharp
weaponManager.EquipWeapon(weaponData, EquipSlot.Primary);
weaponManager.EquipWeapon(weaponData2, EquipSlot.Secondary);
```

## Personalización

### Cambiar Colores

Ajusta en cada **WeaponSlotUI**:
- `Empty Slot Color`: Color cuando está vacío
- `Equipped Slot Color`: Color cuando tiene arma
- `Selected Slot Color`: Color cuando está seleccionado

### Cambiar Tamaño

Modifica **WeaponEquipmentPanel** y ajusta:
- Width/Height del panel principal
- Height de cada slot
- Tamaño del icono (WeaponIcon Width/Height)

### Cambiar Posición

Ajusta anchors del **WeaponEquipmentPanel**:
- **Top-Right**: Esquina superior derecha
- **Bottom-Left**: Esquina inferior izquierda
- **Top-Left**: Esquina superior izquierda (estilo RE4)

### Añadir Sprite de Arma Vacía

1. Importa un sprite de "slot vacío" (ej: silueta de arma)
2. Asigna en `Empty Weapon Sprite` de cada WeaponSlotUI

## Integración con Sistema de Contexto

Si quieres añadir opciones de "Equip" en el menú contextual del inventario:

1. El sistema ya detecta automáticamente si un item es arma
2. El `WeaponInventoryManager` gestiona el equipamiento
3. El UI se actualiza automáticamente vía eventos

## Troubleshooting

**Los slots no se actualizan:**
- Verifica que WeaponManager esté asignado
- Revisa que los eventos estén conectados (OnEnable)
- Comprueba la consola por errores

**Los iconos no aparecen:**
- Asegúrate de que el arma tenga `EquipIcon` o `ItemIcon` asignado
- Verifica que WeaponIcon esté correctamente asignado

**Los colores no cambian:**
- Revisa que Slot Background esté asignado
- Comprueba los colores en el Inspector

**El texto no se ve:**
- Asegúrate de tener TextMeshPro importado
- Verifica que Weapon Name Text esté asignado
