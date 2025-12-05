# 🌿 Setup de Items Recogibles - Guía Completa

## 🎯 Objetivo

Configurar un GameObject en el mundo que el jugador pueda recoger y añadir al inventario.

---

## 📋 Componentes Requeridos

Para que un item sea recogible, necesita **4 componentes obligatorios**:

```
GameObject: GreenHerb
├── Transform              ✅ (Automático)
├── SpriteRenderer         ✅ (Visual)
├── Collider2D             ✅ (Detección)
└── PickupItem (Script)    ✅ (Lógica)
```

---

## 🔧 Setup Paso a Paso

### 1. Crear GameObject

**Opción A: Desde Cero**
1. Click derecho en Hierarchy → **2D Object** → **Sprite**
2. Renombrar: `GreenHerb`

**Opción B: Prefab**
1. Hierarchy → Click derecho → **Create Empty**
2. Renombrar: `GreenHerb`

---

### 2. Configurar Transform

```
Transform
┌────────────────────────────────┐
│ Position:   X: 0, Y: 0, Z: 0   │
│ Rotation:   X: 0, Y: 0, Z: 0   │
│ Scale:      X: 1, Y: 1, Z: 1   │
└────────────────────────────────┘
```

**Ajusta según tu escena:**
- **Position:** Donde aparece en el mundo
- **Scale:** Tamaño del sprite (ej: 0.5 para más pequeño)

---

### 3. Configurar SpriteRenderer (Visual)

```
Sprite Renderer
┌────────────────────────────────┐
│ Sprite:      [Tu sprite aquí]  │ ← Arrastra sprite de planta
│ Color:       White (RGB 255)   │
│ Flip:        None              │
│ Sorting Layer: Default         │
│ Order in Layer: 1              │ ← Sobre el fondo
└────────────────────────────────┘
```

**Importante:**
- ✅ Usa un sprite visible
- ✅ Ajusta `Order in Layer` para que se vea sobre el fondo
- ✅ Puedes añadir un tinte verde en `Color` si quieres

---

### 4. Configurar Collider2D (Detección) ⭐

**Este es el componente MÁS IMPORTANTE para la interacción.**

#### Opción A: BoxCollider2D (Recomendado para items cuadrados)

```
Box Collider 2D
┌────────────────────────────────┐
│ Is Trigger:     ☑ TRUE         │ ← OBLIGATORIO
│ Size:           X: 1, Y: 1     │
│ Offset:         X: 0, Y: 0     │
└────────────────────────────────┘
```

#### Opción B: CircleCollider2D (Recomendado para items redondos)

```
Circle Collider 2D
┌────────────────────────────────┐
│ Is Trigger:     ☑ TRUE         │ ← OBLIGATORIO
│ Radius:         0.5            │
│ Offset:         X: 0, Y: 0     │
└────────────────────────────────┘
```

**Configuración CRÍTICA:**

| Propiedad | Valor | ¿Por qué? |
|-----------|-------|-----------|
| **Is Trigger** | ✅ TRUE | Para que `Physics2D.OverlapCircle` lo detecte |
| **Size/Radius** | Ajustar al sprite | Área de interacción |
| **Offset** | Ajustar si es necesario | Centrar el área de detección |

**⚠️ ERRORES COMUNES:**

```
❌ Is Trigger = false  → PlayerInteractionController NO lo detectará
❌ Collider muy pequeño → Jugador no puede alcanzarlo
❌ Collider muy grande → Se activa desde muy lejos
```

---

### 5. Configurar Layer (IMPORTANTE) ⭐

El `PlayerInteractionController` usa un `LayerMask` para filtrar qué objetos puede detectar.

#### Opción A: Usar Layer "Interactable" (Recomendado)

```
Inspector → GameObject: GreenHerb
┌────────────────────────────────┐
│ Tag:      Untagged             │
│ Layer:    Interactable   ◄──── │ ← Cambia esto
└────────────────────────────────┘
```

**Ya tienes el layer "Interactable" en tu proyecto** según el contexto.

#### Opción B: Crear Layer (Si no existe)

1. Inspector → Layer → **Add Layer...**
2. User Layer 8: `Interactable`
3. Volver al GameObject y asignar Layer

---

### 6. Configurar PickupItem Script

```
Pickup Item (Script)
┌────────────────────────────────┐
│ Item Data:                     │
│   [GreenHerbItem]      ◄────── │ Arrastra ScriptableObject
│                                │
│ Interaction Prompt:            │
│   "Pick up"                    │
│                                │
│ Destroy On Pickup:  ☑ true    │
└────────────────────────────────┘
```

**Propiedades:**

| Campo | Descripción | Ejemplo |
|-------|-------------|---------|
| **Item Data** | ScriptableObject del item | `GreenHerbItem.asset` |
| **Interaction Prompt** | Texto base del prompt | "Pick up", "Recoger", "Take" |
| **Destroy On Pickup** | Destruir después de recoger | ✅ true (normal) |

**Prompt Final:**
- Si escribes `"Pick up"`, el jugador verá: **"Pick up Green Herb"**
- El nombre del item se añade automáticamente

---

## 🎮 Configurar PlayerInteractionController

Tu Player también necesita configuración correcta:

### En el GameObject Player

```
Player
├── Transform
├── PlayerInteractionController   ◄── Debe tener este componente
│   ┌────────────────────────────┐
│   │ Detection Radius:   2.0    │ ← Alcance de interacción
│   │ Interaction Layer:         │
│   │   [Interactable]     ◄──── │ ← DEBE incluir "Interactable"
│   └────────────────────────────┘
├── InventorySystem
└── PlayerInputHandler
```

**Configuración PlayerInteractionController:**

```
Player Interaction Controller
┌────────────────────────────────┐
│ Detection Radius:   2.0        │ ← Radio de detección
│                                │
│ Interaction Layer:             │
│   ☐ Default                    │
│   ☐ TransparentFX              │
│   ☐ Ignore Raycast             │
│   ☑ Interactable      ◄──────  │ ← MARCAR ESTE
│   ☐ UI                         │
│   ☐ Player                     │
│   ☐ Ground                     │
│   ☐ Wall                       │
└────────────────────────────────┘
```

**Importante:**
- `Detection Radius`: Radio en metros donde el jugador puede interactuar
- `Interaction Layer`: **DEBE** incluir el layer `Interactable`

---

## ✅ Checklist Final

### GameObject del Item (GreenHerb)

- [ ] ✅ Tiene `SpriteRenderer` con sprite asignado
- [ ] ✅ Tiene `Collider2D` (Box o Circle)
- [ ] ✅ Collider tiene `Is Trigger = TRUE`
- [ ] ✅ GameObject tiene Layer `Interactable`
- [ ] ✅ Tiene componente `PickupItem`
- [ ] ✅ `PickupItem.itemData` tiene ScriptableObject asignado

### GameObject del Player

- [ ] ✅ Tiene `PlayerInteractionController`
- [ ] ✅ `Interaction Layer` incluye `Interactable`
- [ ] ✅ `Detection Radius` es mayor que 0 (ej: 2.0)
- [ ] ✅ Tiene `InventorySystem`
- [ ] ✅ Tiene `PlayerInputHandler`

### Input System

- [ ] ✅ Input Action `Interact` está mapeado (tecla E)
- [ ] ✅ `PlayerInputHandler.OnInteractInput()` llama a `interactionController.TryInteract()`

---

## 🧪 Probar el Sistema

### Test 1: Detección Visual

1. **Play Mode**
2. **Selecciona Player en Hierarchy**
3. **Scene View → Verás un círculo cyan** (Gizmo de detección)
4. **Acércate al item** (dentro del círculo cyan)
5. **Console debería mostrar:**
   ```
   [INTERACTION] Detected: Pick up Green Herb
   ```

### Test 2: Recoger Item

1. **Play Mode**
2. **Acércate al item**
3. **Presiona E**
4. **Console debería mostrar:**
   ```
   [INTERACTION] Interacting with: Pick up Green Herb
   [PICKUP] Picked up Green Herb
   ```
5. **El GameObject desaparece** (si `Destroy On Pickup = true`)
6. **El item aparece en tu inventario** (slot 0)

### Test 3: Inventario

1. **Presiona Tab** (abrir inventario)
2. **Deberías ver:**
   - Slot 0: `Green Herb`
3. **Presiona E** (abrir menú contextual)
4. **Deberías ver:**
   ```
   ► Use
     Examine
     Drop
   ```

---

## 🐛 Troubleshooting

### Problema 1: "No detecta el item"

**Síntomas:**
- Te acercas al item
- No aparece mensaje en Console

**Soluciones:**

| Verificar | Cómo |
|-----------|------|
| ✅ Layer correcto | Inspector → Item → Layer = `Interactable` |
| ✅ LayerMask del Player | Player → PlayerInteractionController → Interaction Layer incluye `Interactable` |
| ✅ Is Trigger activado | Item → Collider2D → Is Trigger = TRUE |
| ✅ Detection Radius | Player → PlayerInteractionController → Detection Radius > 0 |

### Problema 2: "Detecta pero no recoge al presionar E"

**Síntomas:**
- Console muestra "Detected"
- Al presionar E no pasa nada

**Soluciones:**

| Verificar | Cómo |
|-----------|------|
| ✅ Input mapeado | Player.inputactions → Interact → Binding = E |
| ✅ PlayerInputHandler conectado | PlayerInputHandler.OnInteractInput() llama a interactionController.TryInteract() |
| ✅ ItemData asignado | Item → PickupItem → Item Data tiene ScriptableObject |
| ✅ InventorySystem existe | Player tiene componente InventorySystem |

### Problema 3: "Recoge pero no va al inventario"

**Síntomas:**
- Console muestra "Picked up"
- No aparece en inventario

**Soluciones:**

| Verificar | Cómo |
|-----------|------|
| ✅ Inventario no lleno | Máximo 6 items |
| ✅ ItemData válido | ScriptableObject no es null |
| ✅ InventorySystem activo | Component enabled = true |

### Problema 4: "Error: Layer 'Interactable' doesn't exist"

**Solución:**
1. Edit → Project Settings → Tags and Layers
2. User Layer 8: `Interactable`
3. Guardar

---

## 📐 Ejemplo de Setup Visual

```
SCENE VIEW:

           ┌─────────────┐
           │   Player    │ ← Tiene PlayerInteractionController
           │     ○       │
           └──────┬──────┘
                  │
         ┌────────┴────────┐
         │   Detection     │
         │   Radius: 2.0m  │ ← Círculo cyan (Gizmo)
         │                 │
         │    🌿 GreenHerb │ ← Dentro del radio
         │    (Layer:      │
         │    Interactable)│
         └─────────────────┘


INSPECTOR VIEW - GreenHerb:

GreenHerb
├── Tag:      Untagged
├── Layer:    Interactable      ◄─── IMPORTANTE
│
├── Transform
│   └── Position: (5, 0, 0)
│
├── Sprite Renderer
│   └── Sprite: [herb_sprite]
│
├── Box Collider 2D
│   ├── Is Trigger:  ☑ TRUE     ◄─── IMPORTANTE
│   └── Size: (1, 1)
│
└── Pickup Item (Script)
    ├── Item Data: GreenHerbItem.asset  ◄─── IMPORTANTE
    ├── Interaction Prompt: "Pick up"
    └── Destroy On Pickup: ☑
```

---

## 🎨 Template GameObject

**Puedes crear un Prefab con esta configuración:**

1. Configura un item completo
2. Arrastra a `/Assets/Prefabs/Items/PickupItemTemplate.prefab`
3. Para nuevos items:
   - Instancia el prefab
   - Cambia el sprite
   - Cambia el ItemData

---

## 🚀 Tips Avanzados

### 1. Highlight Visual

Añade feedback visual cuando el jugador está cerca:

```csharp
// Añadir a PickupItem.cs
private SpriteRenderer spriteRenderer;
private Color originalColor;

void Awake()
{
    spriteRenderer = GetComponent<SpriteRenderer>();
    originalColor = spriteRenderer.color;
}

void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
        spriteRenderer.color = Color.yellow;  // Highlight
}

void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
        spriteRenderer.color = originalColor;
}
```

### 2. Partículas al Recoger

```csharp
[SerializeField] private GameObject pickupVFX;

public void Interact(GameObject interactor)
{
    // ... código de pickup
    
    if (pickupVFX != null)
        Instantiate(pickupVFX, transform.position, Quaternion.identity);
    
    Destroy(gameObject);
}
```

### 3. Audio al Recoger

```csharp
[SerializeField] private AudioClip pickupSound;

public void Interact(GameObject interactor)
{
    if (pickupSound != null)
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    
    // ... código de pickup
}
```

---

## 📊 Resumen de Layers

| Layer | Usado Para | Detectado Por |
|-------|------------|---------------|
| **Default** | Objetos generales | - |
| **Player** | Jugador | Enemigos, trampas |
| **Ground** | Suelo, plataformas | Raycast de movimiento |
| **Wall** | Paredes | Raycast de movimiento |
| **Interactable** | Items, NPCs, puertas | **PlayerInteractionController** |

---

## 🎓 Resumen

Para que un item sea recogible necesita:

1. ✅ **Collider2D** con `Is Trigger = TRUE`
2. ✅ **Layer** = `Interactable`
3. ✅ **PickupItem Script** con ItemData asignado
4. ✅ **Player** con PlayerInteractionController configurado

**La configuración MÁS IMPORTANTE es el Layer y el Is Trigger.** Sin esto, el sistema de detección no funciona. 🎮✨
