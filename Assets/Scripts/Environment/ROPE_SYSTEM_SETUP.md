# 🪢 Rope System - Guía de Setup Completa

## 🎯 Descripción del Sistema

Sistema de cuerdas que permite al player **descender de plataformas** usando un **RopeAnchorPoint** (gancho fijo) y teniendo un **objeto Rope** equipado en el inventario.

---

## 📋 Componentes del Sistema

### 1. **RopeAnchorPoint.cs**
GameObject fijo en el terreno que actúa como punto de anclaje.

### 2. **RopeClimbable.cs**
La cuerda desplegada que el player puede trepar/descender.

### 3. **RopeAnchorInteraction.cs**
Sistema de interacción para desplegar la cuerda.

### 4. **WeaponItemData (Rope)**
Item de tipo Tool/Rope en el inventario.

---

## 🔧 Setup Paso a Paso

### PASO 1: Crear el Item Rope en el Inventario

#### 1.1 Crear el WeaponItemData

1. En Project, navega a la carpeta donde tienes tus items (ej: `/Assets/Data/Items/Tools`)
2. Click derecho → **Create** → **Inventory** → **Weapon Item Data**
3. Nombre: `RopeItem`

---

#### 1.2 Configurar el Rope Item

Selecciona `RopeItem` y configura:

```
Basic Info:
├── Item Name: "Rope"
├── Description: "Climbing rope for rappelling"
├── Icon: (tu sprite de cuerda)
└── Item Type: Weapon

Weapon Settings:
├── Weapon Type: Tool
├── Tool Type: Rope
└── Can Be Equipped: true

Inventory Settings:
├── Is Stackable: false
├── Max Stack Size: 1
└── Weight: 2
```

**⚠️ IMPORTANTE**: 
- `WeaponType` debe ser **Tool**
- `ToolType` debe ser **Rope**

---

### PASO 2: Crear el Rope Prefab (Cuerda Desplegable)

#### 2.1 Crear GameObject Base

1. En Hierarchy: Click derecho → **Create Empty**
2. Nombre: `RopeClimbable`
3. Position: (0, 0, 0)
4. Tag: `FrontLadder` ✅

---

#### 2.2 Agregar Componentes

Selecciona `RopeClimbable` y agrega:

```
Components:
├── BoxCollider2D
│   ├── Is Trigger: ✅ true
│   ├── Size: (0.5, 5.0)  ← Altura de la cuerda
│   └── Offset: (0, -2.5)  ← Centro vertical
│
├── LineRenderer (opcional - visual)
│   ├── Positions: 10 puntos
│   ├── Width: 0.1
│   └── Color: Café/Marrón
│
└── RopeClimbable.cs
    ├── Rope Length: 5.0
    ├── Require Interaction Input: false
    ├── Rope Segments: 10
    ├── Rope Color: (0.6, 0.4, 0.2)
    └── Rope Width: 0.1
```

---

#### 2.3 Guardar como Prefab

1. Arrastra `RopeClimbable` desde Hierarchy a `/Assets/Prefabs/Environment/`
2. Nombre: `RopeClimbable.prefab`
3. Elimina el GameObject original de la escena

---

### PASO 3: Crear el Rope Anchor Point (Gancho Fijo)

#### 3.1 Crear GameObject en la Escena

1. En Hierarchy: Click derecho → **Create Empty**
2. Nombre: `RopeAnchor_01`
3. Posición: Donde quieras el punto de anclaje (ej: borde de plataforma)

---

#### 3.2 Estructura Jerárquica

```
RopeAnchor_01
├── AnchorVisual (Sprite - opcional)
│   └── SpriteRenderer: Sprite de gancho/anilla
│
└── RopeSpawnPoint (Transform vacío)
    └── Position: Ligeramente debajo del gancho visual
```

---

#### 3.3 Configurar Componentes

Selecciona `RopeAnchor_01` y agrega:

**A) CircleCollider2D** (para interacción):
```
├── Is Trigger: ✅ true
├── Radius: 1.5
└── Layer: Default o Interactable
```

**B) RopeAnchorPoint.cs**:
```
Anchor Settings:
├── Rope Spawn Point: (arrastra RopeSpawnPoint aquí)
├── Rope Length: 5.0
└── Rope Prefab: (arrastra RopeClimbable.prefab aquí)

Visual Feedback:
├── Anchor Visual: (arrastra AnchorVisual SpriteRenderer aquí)
├── Available Color: Verde (0, 1, 0)
└── Used Color: Gris (0.5, 0.5, 0.5)
```

**C) RopeAnchorInteraction.cs**:
```
InteractableObject Settings:
├── Interaction Prompt: "Deploy Rope"
├── Interaction Range: 2.0
└── Can Interact: ✅ true
```

**D) InteractableObject** (heredado):
```
Base Settings:
├── Interaction Prompt: "Deploy Rope"
└── Interaction Range: 2.0
```

---

### PASO 4: Configurar el Player Inventory

El player debe poder equipar el Rope en el **Secondary Slot**.

#### 4.1 Verificar WeaponInventoryManager

El player debe tener:
```
Player GameObject:
└── WeaponInventoryManager.cs
    ├── Primary Weapon Slot
    └── Secondary Weapon Slot ← Aquí va el Rope
```

---

#### 4.2 Equipar Rope en Slot Secundario

**Opción A - En Runtime**:
1. Inicia el juego
2. Abre el inventario
3. Arrastra `RopeItem` al **Secondary Slot**

**Opción B - Por Código** (auto-equipar al inicio):
```csharp
// En algún script de inicio o test
WeaponInventoryManager weaponManager = player.GetComponent<WeaponInventoryManager>();
weaponManager.EquipWeaponToSecondarySlot(ropeItemData);
```

---

## 🎮 Uso en el Juego

### Flujo de Interacción:

```
1. Player se acerca al RopeAnchor
   ↓
2. Aparece prompt: "Deploy Rope" [E]
   ↓
3. Player presiona [E]
   ↓
4. Sistema verifica:
   - ¿Tiene Rope equipado en Secondary Slot? ✅
   - ¿Ya hay cuerda desplegada? ❌
   ↓
5. Se instancia RopeClimbable desde el RopeSpawnPoint
   ↓
6. Player puede usar W/S para trepar/descender la cuerda
   ↓
7. Al llegar abajo, player puede saltar o moverse
```

---

## 🔍 Verificación del Setup

### Checklist RopeAnchorPoint:

- [ ] Tiene `CircleCollider2D` con `isTrigger = true`
- [ ] Tiene `RopeAnchorPoint.cs`
- [ ] Tiene `RopeAnchorInteraction.cs`
- [ ] Tiene `InteractableObject.cs`
- [ ] `Rope Prefab` asignado en inspector
- [ ] `Rope Spawn Point` asignado
- [ ] `Rope Length` configurado (ej: 5.0)
- [ ] `Anchor Visual` opcional configurado

---

### Checklist RopeClimbable Prefab:

- [ ] Tag: `FrontLadder`
- [ ] Tiene `BoxCollider2D` con `isTrigger = true`
- [ ] Tiene `RopeClimbable.cs`
- [ ] `Rope Length` configurado
- [ ] Opcional: `LineRenderer` para visual

---

### Checklist RopeItem:

- [ ] Es `WeaponItemData`
- [ ] `WeaponType = Tool`
- [ ] `ToolType = Rope`
- [ ] Tiene sprite/icono asignado
- [ ] Puede ser equipado

---

### Checklist Player:

- [ ] Tiene `WeaponInventoryManager`
- [ ] Tiene `PlayerInteractionController`
- [ ] Tiene `LadderClimbState` (para trepar la cuerda)
- [ ] Puede interactuar con objetos

---

## 🎨 Visualización en Scene View

### Gizmos del RopeAnchorPoint:

```
    🟢  ← Sphere (verde = disponible / gris = usado)
    |
    |   ← Línea amarilla (muestra la longitud)
    |
    🟡  ← Sphere (fin de la cuerda)
```

---

### Gizmos del RopeClimbable:

```
    🔵  ← Inicio
    |
    |   ← Línea cyan
    |
    📦  ← BoxCollider (área de interacción)
    |
    |
    🔵  ← Fin
```

---

## 🐛 Troubleshooting

### ❌ "No rope equipped in secondary slot"

**Causa**: No tienes el RopeItem equipado en el Secondary Slot.

**Solución**:
1. Abre el inventario
2. Equipa el `RopeItem` en el **Secondary Weapon Slot**

---

### ❌ "Rope already deployed here"

**Causa**: Ya hay una cuerda desplegada en ese anchor point.

**Solución**:
- Solo puedes desplegar una cuerda por anchor
- Usa otro anchor point
- O implementa un sistema para recoger/remover cuerdas

---

### ❌ Player no puede trepar la cuerda

**Causas posibles**:
1. RopeClimbable no tiene Tag `FrontLadder`
2. Player no tiene `LadderClimbState`
3. BoxCollider2D no es trigger
4. Player no está presionando W/S

**Solución**:
1. Verifica el Tag
2. Verifica el componente `LadderClimbState` en Player
3. `isTrigger = true` en BoxCollider2D
4. Presiona W (arriba) o S (abajo) para trepar

---

### ❌ La cuerda no se visualiza

**Causa**: No hay LineRenderer o no está configurado.

**Solución**:
1. El LineRenderer es opcional (solo visual)
2. El sistema funciona sin él usando el BoxCollider2D
3. Si quieres visual, asegúrate de que:
   - `LineRenderer` tiene material asignado
   - `Rope Segments` > 2
   - `Start Color` y `End Color` configurados

---

### ❌ "Player doesn't have WeaponInventoryManager"

**Causa**: Falta el componente en el Player.

**Solución**:
1. Selecciona el Player GameObject
2. Add Component → `WeaponInventoryManager`

---

## 🔧 Código de Verificación Manual

Si quieres verificar en runtime:

```csharp
// En una clase de testing o debug
void TestRopeSystem()
{
    // 1. Verificar que el player tiene WeaponInventoryManager
    WeaponInventoryManager weaponManager = player.GetComponent<WeaponInventoryManager>();
    Debug.Log($"Has WeaponManager: {weaponManager != null}");
    
    // 2. Verificar que tiene rope equipado
    if (weaponManager != null)
    {
        WeaponItemData secondary = weaponManager.SecondaryWeapon;
        bool hasRope = secondary != null && 
                       secondary.WeaponType == WeaponType.Tool && 
                       secondary.ToolType == ToolType.Rope;
        Debug.Log($"Has Rope Equipped: {hasRope}");
    }
    
    // 3. Verificar que el anchor tiene el prefab
    RopeAnchorPoint anchor = GetComponent<RopeAnchorPoint>();
    Debug.Log($"Anchor has prefab: {anchor != null && anchor.RopePrefab != null}");
}
```

---

## 📊 Resumen Visual del Setup

```
🎮 PLAYER
└── WeaponInventoryManager
    └── Secondary Slot
        └── 🪢 RopeItem (Tool/Rope)

🏔️ SCENE
└── 🪝 RopeAnchor_01
    ├── CircleCollider2D (trigger)
    ├── RopeAnchorPoint.cs
    │   ├── Rope Spawn Point: Transform
    │   ├── Rope Length: 5.0
    │   └── Rope Prefab: RopeClimbable
    │
    ├── RopeAnchorInteraction.cs
    └── InteractableObject.cs

📦 PREFAB
└── 🪢 RopeClimbable
    ├── Tag: FrontLadder
    ├── BoxCollider2D (trigger)
    ├── LineRenderer (optional)
    └── RopeClimbable.cs
        └── Rope Length: 5.0
```

---

## 🎯 Ejemplo de Uso

### Escenario: Descender de una Plataforma Alta

```
1. Setup inicial:
   ════════════  ← Plataforma alta
       🪝        ← RopeAnchor_01
   
   Player en la plataforma con RopeItem equipado

2. Player interactúa [E]:
   ════════════
       🪝
       |
       |  ← Cuerda desplegada
       |
       |
       
3. Player trepa hacia abajo [S]:
   ════════════
       🪝
       |
       👤 ← Player descendiendo
       |
       |
       
4. Player llega al suelo:
   ════════════
       🪝
       |
       |
       |
   ════ 👤 ════  ← Suelo
```

---

## ✅ Testing Final

### Test 1: Desplegar Cuerda

1. Equipa `RopeItem` en Secondary Slot
2. Acércate al RopeAnchor
3. Presiona [E] para interactuar
4. **Resultado esperado**: Aparece la cuerda visual y el collider

---

### Test 2: Trepar/Descender

1. Con la cuerda desplegada, toca el collider
2. Presiona [W] o [S]
3. **Resultado esperado**: Player entra en LadderClimbState y se mueve verticalmente

---

### Test 3: Salir de la Cuerda

1. Mientras trepas, presiona [Space] (Jump)
2. **Resultado esperado**: Player sale del LadderClimbState y entra en AirState

---

### Test 4: Anchor Ya Usado

1. Con una cuerda ya desplegada
2. Intenta interactuar de nuevo
3. **Resultado esperado**: Mensaje "Rope already deployed here"

---

## 🎨 Mejoras Opcionales

### 1. Animación de Cuerda Balanceándose

```csharp
// En RopeClimbable.cs
void Update()
{
    if (lineRenderer != null)
    {
        for (int i = 0; i < ropeSegments; i++)
        {
            float t = i / (float)(ropeSegments - 1);
            float sway = Mathf.Sin(Time.time + t * 3f) * 0.15f;
            Vector3 pos = transform.position + Vector3.down * (ropeLength * t);
            pos.x += sway;
            lineRenderer.SetPosition(i, pos);
        }
    }
}
```

---

### 2. Sistema de Recoger Cuerda

```csharp
// Agregar en RopeAnchorInteraction.cs
void Update()
{
    if (anchorPoint.IsRopeDeployed && Input.GetKeyDown(KeyCode.R))
    {
        anchorPoint.RemoveRope();
        interactionPrompt = "Deploy Rope";
        SetInteractable(true);
    }
}
```

---

### 3. Partículas al Desplegar

```csharp
// En RopeAnchorPoint.DeployRope()
if (dustParticles != null)
{
    ParticleSystem particles = Instantiate(dustParticles, ropeSpawnPoint.position, Quaternion.identity);
    particles.Play();
}
```

---

### 4. Sonidos

```csharp
// En RopeAnchorPoint.DeployRope()
if (deploySound != null)
{
    AudioSource.PlayClipAtPoint(deploySound, ropeSpawnPoint.position);
}
```

---

¡Sistema de cuerdas listo para usar! 🪢✨
