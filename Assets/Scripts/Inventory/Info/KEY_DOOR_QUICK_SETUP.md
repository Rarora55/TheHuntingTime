# 🔑🚪 Configuración Rápida: Llave y Puerta

Guía paso a paso para implementar el sistema de llave → inventario → puerta en 5 minutos.

---

## ✅ 1. Crear el Asset de la Llave

```
1. Project → Assets/Assets/Data/Items/
2. Click derecho → Create → Inventory → Key Item
3. Nombre: "RustyKeyData"

Inspector:
  General:
    - Item Name: "Rusty Key"
    - Item Description: "An old rusty key"
    - Item Icon: (arrastra sprite de llave)
    - Item Type: KeyItem
    - Is Stackable: false ✗
    - Max Stack Size: 1
  
  Key Item Settings:
    - Unlocks: (Array)
        Size: 1
        Element 0: "rusty_key"  ← Este ID es importante!
    - Is Quest Item: false ✗
    - Can Be Discarded: true ✓
```

---

## 🔑 2. Crear GameObject Llave en la Escena

```
1. Hierarchy → Create Empty
2. Nombre: "RustyKey"
3. Position: (10, 0, 0) (donde quieras)

Add Component → Sprite Renderer:
  - Sprite: (tu sprite de llave)
  - Color: blanco
  - Sorting Layer: Items (o Default)

Add Component → Circle Collider 2D:
  - Is Trigger: ✓ true
  - Radius: 0.5

Add Component → PickupInteractable:
  - Item Data: RustyKeyData (arrastra el asset)
  - Item Name: "Rusty Key"
  - Destroy On Pickup: ✓ true
  - Pickup VFX: (opcional)
  - Pickup Sound: (opcional)

GameObject Settings:
  - Layer: Interactable
  - Tag: Untagged
```

---

## 🚪 3. Crear GameObject Puerta en la Escena

```
1. Hierarchy → Create Empty
2. Nombre: "RustyDoor"
3. Position: (20, 0, 0) (donde quieras)

Add Component → Sprite Renderer:
  - Sprite: (tu sprite de puerta cerrada)
  - Color: blanco
  - Sorting Layer: Environment (o Default)

Add Component → Box Collider 2D:
  - Is Trigger: ✓ true
  - Size: (1, 2) (ajusta al sprite)

Add Component → LockedDoorInteractable:
  Door Settings:
    - Required Key ID: "rusty_key"  ← Mismo ID que en el asset!
    - Door Name: "Basement Door"
    - Consume Key On Unlock: ✓ true (llave desaparece al usar)
    - Is Locked: ✓ true
  
  Visuals:
    - Sprite Renderer: (auto-detectado)
    - Locked Sprite: (sprite puerta cerrada)
    - Unlocked Sprite: (sprite puerta abierta, opcional)
    - Door Animator: (opcional, para animación)
  
  Feedback:
    - Unlock Sound: (sonido de desbloqueo, opcional)
    - Open Sound: (sonido de apertura, opcional)
    - Locked Sound: (sonido cuando está bloqueada, opcional)
    - Unlock VFX: (partículas, opcional)

GameObject Settings:
  - Layer: Interactable
  - Tag: Untagged
```

---

## 🎮 4. Verificar Configuración del Player

```
Player GameObject debe tener:

Components:
  - InventorySystem ✓
  - PlayerInteractionController ✓
  - PlayerInputHandler ✓

Settings:
  - Tag: "Player"
  - Layer: Player

PlayerInteractionController:
  - Detection Radius: 2
  - Interaction Layer: Interactable (selecciona en el dropdown)
```

---

## ⚙️ 5. Configurar Layers

```
1. Edit → Project Settings → Tags and Layers

Layers:
  - Layer 8: Interactable ✓ (debe existir)

2. Edit → Project Settings → Physics 2D

Layer Collision Matrix:
  - Player ✓ colisiona con Interactable ✓
```

---

## 🧪 6. Probar en Play Mode

### Test 1: Recoger Llave

```
1. Play
2. Acércate a la llave (círculo de detección 2m)
3. Verás: "Press E to pick up Rusty Key"
4. Presiona E
5. ✅ Llave desaparece del mundo
6. ✅ Llave aparece en inventario (Tab para abrir)
7. Console muestra: "[INVENTORY] Added Rusty Key to slot X"
```

### Test 2: Intentar Abrir Puerta (sin llave)

```
1. NO recojas la llave primero
2. Acércate a la puerta
3. Verás: "Locked. Requires: Basement Door Key"
4. Presiona E
5. ✅ No pasa nada
6. Console muestra: "[DOOR] Basement Door is locked!"
```

### Test 3: Abrir Puerta (con llave)

```
1. Recoge la llave primero
2. Acércate a la puerta
3. Verás: "Press E to unlock Basement Door"
4. Presiona E
5. ✅ Puerta se desbloquea
6. ✅ Llave desaparece del inventario (si consume_key=true)
7. ✅ Puerta se abre (sprite cambia o animación)
8. ✅ Puedes pasar (collider desactivado)
9. Console muestra:
   - "[INVENTORY] Found key for 'rusty_key': Rusty Key"
   - "[DOOR] Basement Door unlocked!"
   - "[INVENTORY] Consumed key: Rusty Key"
   - "[DOOR] Basement Door opened!"
```

---

## 🐛 Troubleshooting

### "Press E to pick up" no aparece

```
Problema: Player no detecta la llave

Solución:
  1. RustyKey Layer = Interactable ✓
  2. RustyKey Collider2D.isTrigger = true ✓
  3. PlayerInteractionController.interactionLayer = Interactable ✓
  4. Physics2D: Player colisiona con Interactable ✓
  5. Detection Radius ≥ distancia al objeto
```

### Llave no se añade al inventario

```
Problema: PickupInteractable no conecta con inventario

Solución:
  1. PickupInteractable.itemData asignado ✓
  2. Player tiene InventorySystem component ✓
  3. Inventario no está lleno (6 slots) ✓
  4. Console muestra error específico
```

### Puerta no se desbloquea con llave

```
Problema: LockedDoorInteractable no encuentra llave

Solución:
  1. LockedDoorInteractable.requiredKeyID = "rusty_key" ✓
  2. RustyKeyData.Unlocks[0] = "rusty_key" ✓
  3. IDs coinciden EXACTAMENTE (case sensitive) ✓
  4. Llave está en el inventario ✓
  5. Console muestra: "[INVENTORY] Found key for 'rusty_key'"
```

### "Locked. Requires..." siempre aparece

```
Problema: CanInteract() no detecta la llave

Solución:
  1. Recoge la llave primero ✓
  2. Abre inventario (Tab) y verifica que está ahí ✓
  3. Verifica que el KeyItemData.Unlocks incluye el ID correcto ✓
  4. Console al acercarse a puerta debe mostrar:
     "[INVENTORY] Found key for 'rusty_key': Rusty Key"
```

---

## 📝 Notas Importantes

### IDs de Llave

```
El requiredKeyID DEBE coincidir exactamente:

LockedDoorInteractable:
  requiredKeyID: "rusty_key"
               ↓ DEBEN SER IGUALES
KeyItemData:
  Unlocks[0]: "rusty_key"

❌ NO coinciden:
  - "rusty_key" vs "Rusty_Key"
  - "rusty_key" vs "rustykey"
  - "rusty_key" vs "rusty key"

✅ SÍ coinciden:
  - "rusty_key" vs "rusty_key"
```

### Una Llave, Múltiples Puertas

```
Una sola llave puede abrir varias puertas:

KeyItemData:
  Unlocks:
    - "rusty_key"
    - "basement_door"
    - "storage_room"

LockedDoorInteractable (Puerta 1):
  requiredKeyID: "rusty_key" ✓

LockedDoorInteractable (Puerta 2):
  requiredKeyID: "basement_door" ✓

LockedDoorInteractable (Puerta 3):
  requiredKeyID: "storage_room" ✓

Resultado: 1 llave abre 3 puertas
```

### Consumir vs No Consumir Llave

```
Consume Key On Unlock = true:
  - Llave desaparece al abrir puerta
  - Uso: Llaves de un solo uso

Consume Key On Unlock = false:
  - Llave permanece en inventario
  - Uso: Llaves maestras, llaves reutilizables
```

---

## 🎨 Variaciones

### Llave que No se Consume

```
LockedDoorInteractable:
  - Consume Key On Unlock: false ✗

Resultado: Llave permanece, puede abrir múltiples puertas
```

### Múltiples Llaves para la Misma Puerta

```
No soportado directamente, pero puedes:

Opción 1: Crear KeyItemData con múltiples IDs
KeyItemData (Master Key):
  Unlocks: ["key1", "key2", "key3"]

LockedDoorInteractable:
  requiredKeyID: "key1"  ← Master Key puede abrir

Opción 2: Modificar LockedDoorInteractable para aceptar array
```

### Puerta que Requiere Múltiples Llaves

```
Necesita modificar LockedDoorInteractable:

string[] requiredKeyIDs = {"red_key", "blue_key", "green_key"};

bool HasAllKeys(GameObject interactor)
{
    foreach (string keyID in requiredKeyIDs)
    {
        if (!inventory.HasKeyItem(keyID))
            return false;
    }
    return true;
}
```

---

## 🚀 Próximos Pasos

1. ✅ Crear más KeyItemData (gold_key, silver_key, etc.)
2. ✅ Crear más LockedDoorInteractable en tu nivel
3. ✅ Añadir animaciones de apertura (Animator)
4. ✅ Añadir sonidos de desbloqueo/apertura
5. ✅ Crear partículas de desbloqueo (VFX)
6. ✅ Diseñar puzzles con múltiples llaves

---

## 📚 Documentación Relacionada

- **Flujo Completo:** `/Assets/Scripts/Inventory/Info/KEY_DOOR_SCENARIO.md`
- **Sistema de Interacción:** `/Assets/Explains/INTERACTION_SYSTEM_GUIDE.md`
- **Arquitectura de Inventario:** `/Assets/Scripts/Inventory/Info/ARCHITECTURE.md`

---

¡Listo para implementar! 🔑🚪✨
