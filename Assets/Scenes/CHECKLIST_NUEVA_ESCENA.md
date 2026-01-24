# ✅ Checklist: Nueva Escena Gym

## 🎯 Elementos Esenciales para que Funcione Como Character.unity

---

## 1️⃣ CORE - Objetos Obligatorios

### ✅ Player (Prefab)
```
Prefab: /Assets/Prefabs/CORE/Player/Player.prefab

Drag & Drop desde Project → Hierarchy

Componentes críticos (ya incluidos en prefab):
├─ Player (script principal)
├─ PlayerInputHandler
├─ Rigidbody2D
├─ BoxCollider2D
├─ Animator
├─ HealthController
├─ StaminaController
├─ PlayerRespawnController
├─ InventorySystem
├─ PlayerWeaponController (x2, esto es normal)
├─ PlayerInteractionController
└─ SecondaryEquipmentController

Configuración en Inspector:
└─ Player Data: Debe estar asignado → "/Assets/Scripts/Data/PlayerData.asset"

Hijos obligatorios (ya vienen con prefab):
├─ GroundCheck    (Transform vacío, posición: y: -0.5 aprox)
├─ WallCheck      (Transform vacío, posición: x: +0.5 aprox)
├─ LedgeCheck     (Transform vacío, posición: x: +0.5, y: +0.5 aprox)
├─ CeilingCheck   (Transform vacío, posición: y: +1.0 aprox)
├─ FirePoint      (Transform vacío, posición: frente al player)
└─ FlashLight     (GameObject con Light2D)
```

**IMPORTANTE:** Si usas prefab, arrastra directamente desde `/Assets/Prefabs/CORE/Player/Player.prefab`

---

### ✅ Main Camera
```
OPCIÓN A - Crear desde cero:
├─ GameObject → Camera
├─ Tag: "MainCamera"
├─ Add Component: Audio Listener
├─ Add Component: CinemachineBrain (de Cinemachine)
├─ Add Component: Universal Additional Camera Data (URP)

Configuración:
├─ Projection: Orthographic
├─ Orthographic Size: 2 (para 2D platformer)
├─ Near Clip: 0.3
├─ Far Clip: 1000
├─ Background: Negro (0, 0, 0, 0)
├─ Clear Flags: Solid Color
├─ Culling Mask: Everything
└─ Depth: -1

Transform:
└─ Position: (0, 0, -10) ← Z debe ser negativo

OPCIÓN B - Usar prefab (si existe):
└─ Drag prefab de cámara si tienes uno
```

---

### ✅ CinemachineCamera
```
Hierarchy → Create → Cinemachine → Cinemachine Camera

Configuración:
├─ Tracking Target: Drag el Player aquí
├─ Look At Target: Drag el Player aquí (o dejarlo vacío)

Add Extension:
└─ CinemachineFollow (componente de seguimiento)
    ├─ Follow Offset: (0, 0, -10)
    ├─ Damping: (1, 1, 0) ← Ajusta según quieras suavizado

Transform:
└─ Position: (0, 0, -10)

NOTA: CinemachineBrain en Main Camera debe estar activo
```

---

### ✅ EventSystem (UI)
```
Hierarchy → Right Click → UI → Event System

Se crea automáticamente con:
├─ EventSystem (component)
└─ InputSystemUIInputModule (para New Input System)

Configuración:
├─ Send Navigation Events: ✓
└─ Pixel Drag Threshold: 10 (default)

Transform:
└─ Position: (0, 0, 0)

IMPORTANTE: Solo puede haber 1 EventSystem en la escena
```

---

### ✅ RespawnManager
```
Hierarchy → Create Empty → Nombrar "RespawnManager"

Add Component:
└─ TheHunt.Respawn.RespawnManager

Configuración en Inspector:
Events:
├─ On Respawn Activated  → /Assets/Scripts/Respawn/RespawnActivatedEvent.asset
└─ On Respawn Request    → /Assets/Scripts/Respawn/RespawnRequestEvent.asset

Runtime Data:
└─ Runtime Data          → /Assets/Scripts/Respawn/RespawnRuntimeData.asset

Settings:
└─ Log Respawn Changes   → ✓

Transform:
└─ Position: (0, 0, 0)

IMPORTANTE: Estos 3 ScriptableObjects DEBEN existir antes (ver sección Assets)
```

---

### ✅ UIFeedBackManager
```
Hierarchy → Create Empty → Nombrar "UIFeedBackManager"

Add Component:
├─ TheHunt.UI.UIFeedbackManager
├─ TheHunt.Inventory.InventoryFullDialog
└─ TheHunt.Inventory.InventoryFullDialogTester (opcional, para testing)

Configuración:
UIFeedbackManager:
└─ Inventory Full Dialog → Drag el mismo GameObject (self-reference)

InventoryFullDialog:
├─ Dialog Title: "Inventario Lleno"
├─ Dialog Message: "I have the back a bit full..."
├─ Auto Find Inventory System: ✓
├─ Inventory System: /Player (auto-detectado)
└─ Dialog Service: /Player (auto-detectado)

Transform:
└─ Position: (0, 0, 0)
```

---

## 2️⃣ LIGHTING - Sistema de Luces

### ✅ GlobalLight (Obligatorio para URP 2D)
```
Hierarchy → Create → Light → 2D → Global Light 2D

Configuración:
├─ Light Type: Global
├─ Blend Style Index: 0 (default)
├─ Color: Blanco (1, 1, 1, 1)
├─ Intensity: 1.0
├─ Shadow Intensity: 0.75
├─ Shadow Softness: 0.3
├─ Shadows Enabled: ✓
├─ Shadow Volume Intensity: 0.75
└─ Target Sorting Layers: Default, Items, Environment (todos)

Transform:
└─ Position: (0, 0, 0) ← No importa, es global

NOTA: Sin esta luz, la escena estará completamente negra en URP 2D
```

---

### 🔦 FlashLight (Opcional - duplicado)
```
Si tienes FlashLight suelto en la escena (aparte del que está en Player):

Es opcional, el Player ya tiene su propia linterna.
Puedes eliminarlo o dejarlo como backup.
```

---

### 🌑 DarkZone (Opcional)
```
Si quieres zonas oscuras:

Hierarchy → Create → 2D Object → Sprites → Square
├─ Nombrar: "DarkZone"
├─ Add Component: Light2D (Shape Light)
├─ Configuración:
│   ├─ Intensity: 0 (oscuridad total)
│   ├─ Color: Negro
│   └─ Order in Layer: Mayor que GlobalLight
└─ Ajusta tamaño con Transform.Scale

Ver: /Pages/💡 Sistema de Iluminación - Guía Completa.md
```

---

## 3️⃣ PHYSICS - Entorno y Plataformas

### ✅ Ground (Suelos)
```
OPCIÓN A - Sprites básicos:
├─ GameObject → 2D Object → Sprites → Square
├─ Nombrar: "Ground" o "Suelos"
├─ Add Component: BoxCollider2D
├─ Layer: Ground (IMPORTANTE: crea el layer si no existe)
└─ Ajusta escala para hacer plataformas

OPCIÓN B - Tilemap (recomendado para niveles):
├─ GameObject → 2D Object → Tilemap → Rectangular
├─ Layer: Ground
├─ Paint con Tile Palette
└─ Add Component: TilemapCollider2D (auto)

OPCIÓN C - Organizar en carpeta:
├─ Create Empty "Suelos"
└─ Dentro: Múltiples GameObjects con Sprites/Colliders
    ├─ Square
    ├─ Square (1)
    ├─ Square (2)
    └─ ...

Layer Configuration:
Project Settings → Tags & Layers:
└─ Layer 6: "Ground"
```

---

### 🧱 Walls (Opcional)
```
Si quieres paredes para wall slide:
├─ GameObject → 2D Object → Sprites → Square
├─ Nombrar: "Wall" o "WallSlide"
├─ Add Component: BoxCollider2D
├─ Layer: Wall
└─ Rotar/Escalar para hacer pared vertical

IMPORTANTE: Player detecta "Ground" y "Wall" layers por separado
```

---

## 4️⃣ GAMEPLAY - Elementos Interactivos (Opcionales)

### 🪜 Ladder (Escalera)
```
Prefab: Posiblemente existe en /Assets/Prefabs/

Componentes:
├─ BoxCollider2D (Trigger)
├─ Ladder script
└─ Tag: "FrontLadder" (o similar)

Ver: /Pages/🪜 Sistema LadderClimb - Guía de Implementación.md
```

---

### 🎯 RespawnPoint (Checkpoints)
```
Hierarchy → Create Empty → Nombrar "Checkpoint_01"

Add Component:
├─ BoxCollider2D (Trigger: ✓, Size: 2x2)
└─ TheHunt.Respawn.RespawnPoint

Configuración:
Events:
└─ On Respawn Activated → /Assets/Scripts/Respawn/RespawnActivatedEvent.asset

Settings:
├─ Respawn ID: "Checkpoint_01" (ÚNICO por checkpoint)
├─ Auto Activate On Enter: ✓
├─ One Time Use: ❌ (normalmente)
├─ Gizmo Color: Verde
└─ Show Label: ✓

Layer:
└─ Interactable (opcional)

NOTA: Necesitas el RespawnManager en la escena
```

---

### 🔫 Weapons/Items (Opcionales)
```
Ejemplos de la escena Character:
├─ Revolver (prefab de arma)
├─ RustyKey (item)
├─ RustyDoor (interactable)
└─ Plant, Box (objetos con pickup)

Cada uno tiene:
├─ SpriteRenderer
├─ Collider2D (Trigger)
├─ Script específico (ItemPickup, WeaponPickup, etc)
└─ detectionGround (hijo, para detectar suelo)

NOTA: Puedes omitir para gym básico
```

---

### 🧗 Platforming Elements (Opcionales)
```
Elementos avanzados en Character.unity:
├─ Slide_01 (plataforma deslizante)
├─ JumpToLedgeA (ledge grab)
├─ RopeAnchor (cuerda)
├─ RopePickup (item de cuerda)
└─ ClimbSpawnPoints (ReSpawnTop/Down)

NOTA: Omite para gym básico, añade gradualmente
```

---

## 5️⃣ ASSETS - ScriptableObjects Necesarios

### 📦 Obligatorios

```
/Assets/Scripts/Data/
└─ PlayerData.asset
   └─ ScriptableObject con configuración del player
   └─ Create → TheHunt → Data → Player Data

/Assets/Scripts/Respawn/
├─ RespawnActivatedEvent.asset
│  └─ Create → TheHunt → Events → Respawn Activated Event
├─ RespawnRequestEvent.asset
│  └─ Create → TheHunt → Events → Respawn Request Event
└─ RespawnRuntimeData.asset
   └─ Create → TheHunt → Data → Respawn Runtime Data

CRÍTICO: Estos 3 assets DEBEN existir para RespawnManager
```

---

## 6️⃣ PROJECT SETTINGS - Configuración Global

### 🏷️ Tags (Project Settings → Tags & Layers)
```
Tags necesarios:
├─ Player
├─ MainCamera
├─ Interactable
├─ FrontLadder (si usas ladders)
└─ Respawn (opcional)

Layers necesarios:
├─ Layer 5: UI
├─ Layer 6: Player
├─ Layer 7: Ground
├─ Layer 8: Wall
└─ Layer 9: Interactable

Sorting Layers (2D):
├─ Default (0)
├─ Items (1)
└─ Environment (2)
```

---

### ⚙️ Physics 2D (Project Settings → Physics 2D)
```
Configurar Layer Collision Matrix:

Player:
├─ Colisiona con: Ground, Wall, Interactable
└─ NO colisiona con: Player (si tienes múltiples)

Ground:
└─ Colisiona con: Player

IMPORTANTE: Verifica que Player + Ground tengan colisión activa
```

---

### 🎮 Input System
```
Ya configurado globalmente en el proyecto.

Player prefab tiene:
├─ PlayerInput component
└─ Input Actions: Asignado automáticamente

No necesitas hacer nada extra por escena.
```

---

## 7️⃣ CHECKLIST RÁPIDO - Orden de Creación

### Orden Recomendado:

```
1. ✅ Crear escena nueva
   File → New Scene → Basic (Built-in) o 2D (URP)

2. ✅ Eliminar Main Camera default (si viene)

3. ✅ Añadir Player (prefab)
   Drag: /Assets/Prefabs/CORE/Player/Player.prefab

4. ✅ Añadir Main Camera
   GameObject → Camera
   └─ Configure como arriba

5. ✅ Añadir CinemachineCamera
   Cinemachine → Cinemachine Camera
   └─ Tracking: Player

6. ✅ Añadir EventSystem
   UI → Event System

7. ✅ Añadir GlobalLight
   Light → 2D → Global Light 2D

8. ✅ Añadir RespawnManager
   Create Empty + Component

9. ✅ Añadir UIFeedBackManager
   Create Empty + Component

10. ✅ Añadir Ground
    Create sprites/tilemaps + BoxCollider2D

11. ✅ Añadir Checkpoints (opcional)
    Create Empty + RespawnPoint component

12. ✅ Testear
    Play Mode → Player debe moverse y cámara seguir
```

---

## 8️⃣ VALIDACIÓN - Cómo Verificar que Todo Funciona

### ✅ Tests Básicos

```
1. Play Mode:
   └─ Player aparece en escena ✓

2. Movimiento:
   ├─ A/D o Flechas: Player se mueve ✓
   ├─ Space: Player salta ✓
   └─ Shift: Player corre (si está implementado) ✓

3. Cámara:
   └─ Cinemachine sigue al Player suavemente ✓

4. Physics:
   ├─ Player cae por gravedad ✓
   ├─ Player colisiona con Ground ✓
   └─ No atraviesa plataformas ✓

5. Respawn:
   ├─ Player toca Checkpoint → Console log ✓
   ├─ Player cae al vacío → Respawn en último checkpoint ✓
   └─ Presionar R → Respawn manual ✓

6. UI:
   ├─ Inventory funciona (Tab o I) ✓
   ├─ Health bar visible ✓
   └─ Stamina bar visible ✓

7. Luz:
   └─ Escena visible (no negra) ✓
```

---

## 9️⃣ TROUBLESHOOTING - Problemas Comunes

### ❌ Escena completamente negra
```
SOLUCIÓN:
└─ Añadir GlobalLight (Global Light 2D)
└─ Verificar: Intensity: 1.0, Color: Blanco
```

### ❌ Player no colisiona con suelo
```
SOLUCIÓN:
├─ Ground Layer: "Ground"
├─ Ground tiene BoxCollider2D
└─ Player Rigidbody2D: Gravity Scale > 0
└─ Physics 2D Settings: Player + Ground collision ✓
```

### ❌ Cámara no sigue al Player
```
SOLUCIÓN:
├─ CinemachineCamera → Tracking Target: Player ✓
└─ Main Camera → CinemachineBrain ✓
```

### ❌ Input no funciona
```
SOLUCIÓN:
├─ Player tiene PlayerInput component ✓
├─ Input Actions asset asignado ✓
└─ EventSystem en escena ✓
```

### ❌ RespawnManager errores en Console
```
SOLUCIÓN:
└─ Crear los 3 ScriptableObjects:
    ├─ RespawnActivatedEvent.asset
    ├─ RespawnRequestEvent.asset
    └─ RespawnRuntimeData.asset
└─ Asignar en RespawnManager Inspector
```

### ❌ UI no funciona (inventario, etc)
```
SOLUCIÓN:
├─ EventSystem en escena ✓
├─ UIFeedBackManager en escena ✓
└─ Canvas debe tener GraphicRaycaster ✓
```

---

## 🔟 TEMPLATE - Escena Mínima

### Scene Hierarchy (Mínimo Funcional):

```
Gym.unity:
├─ Player                     ← Prefab
├─ Main Camera                ← Camera + CinemachineBrain
├─ CinemachineCamera          ← Sigue a Player
├─ EventSystem                ← UI
├─ RespawnManager             ← Sistema de respawn
├─ UIFeedBackManager          ← Feedback UI
├─ GlobalLight                ← URP 2D Light
└─ Ground                     ← Plataforma básica
    └─ Floor                  ← Sprite + BoxCollider2D
```

**Con estos 8 elementos tienes un gym funcional.**

---

## 📦 EXTRAS - Para Gym Completo

### Elementos Adicionales (Opcional):

```
├─ Checkpoint_Start           ← RespawnPoint
├─ Checkpoint_Mid             ← RespawnPoint
├─ Checkpoint_End             ← RespawnPoint
├─ Ladder_01                  ← Para testear escalada
├─ WallSlide_Left             ← Pared para wall slide
├─ WallSlide_Right            ← Pared para wall slide
├─ JumpTestPlatforms          ← Plataformas a diferentes alturas
├─ SlideTestSlope             ← Rampa para sliding
└─ TestWeapon                 ← Arma para testear combate
```

---

## 🎯 RESUMEN FINAL

### OBLIGATORIOS (8 elementos):
1. ✅ Player (prefab)
2. ✅ Main Camera
3. ✅ CinemachineCamera
4. ✅ EventSystem
5. ✅ RespawnManager
6. ✅ UIFeedBackManager
7. ✅ GlobalLight
8. ✅ Ground (al menos 1 plataforma)

### ASSETS OBLIGATORIOS (4):
1. ✅ PlayerData.asset
2. ✅ RespawnActivatedEvent.asset
3. ✅ RespawnRequestEvent.asset
4. ✅ RespawnRuntimeData.asset

### PROJECT SETTINGS:
1. ✅ Tags: Player, MainCamera
2. ✅ Layers: Player (6), Ground (7)
3. ✅ Physics 2D: Collision matrix configurada

---

## 🚀 QUICK START

```bash
# Paso 1: Crear escena
File → New Scene → 2D (URP)

# Paso 2: Drag & drop
- Player prefab → Hierarchy
- Configure Main Camera (Orthographic)
- Add CinemachineCamera → Track Player
- Add GlobalLight
- Create Ground sprite + BoxCollider2D

# Paso 3: Managers
- Create Empty → RespawnManager + component
- Create Empty → UIFeedBackManager + component
- EventSystem (UI → EventSystem)

# Paso 4: Assign ScriptableObjects
- RespawnManager → 3 assets
- Player → PlayerData.asset

# Paso 5: Test
- Play Mode
- Move with A/D, Jump with Space
```

---

## 📚 Referencias

```
Ver también:
├─ /Pages/🏋️ Testing Gym - Guía de Creación Completa.md
├─ /Pages/💡 Sistema de Iluminación - Guía Completa.md
├─ /Pages/📷 Cámaras - Guía de Prefabs y Setup.md
└─ /Assets/Scripts/Respawn/RESUMEN_SISTEMAS.md
```

---

**¡Con esto tienes TODO lo necesario para crear una escena Gym funcional! 🎮**
