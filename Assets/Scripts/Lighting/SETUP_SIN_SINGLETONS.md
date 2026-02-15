# ⚡ Setup Rápido - Sistema de Iluminación sin Singletons

## 🎯 Configuración en 5 Minutos

Esta guía te ayuda a configurar el sistema de iluminación sin singletons en cualquier escena.

---

## 📦 Paso 1: Crear los ScriptableObject Events (SOLO UNA VEZ)

Si no existen, créalos:

### Opción A: Usar Assets Existentes

Si ya existen en `/Assets/Data/Events/`, úsalos directamente.

### Opción B: Crear Nuevos

1. Crea la carpeta `/Assets/Data/Events/` si no existe
2. Haz clic derecho en la carpeta → Create:

```
Create → TheHunt → Events → Light Control Event
   Nombre: LightRegisteredEvent

Create → TheHunt → Events → Light Control Event
   Nombre: LightUnregisteredEvent

Create → TheHunt → Events → Global Light Command Event
   Nombre: GlobalLightCommandEvent

Create → TheHunt → Events → Screen Fade Event
   Nombre: ScreenFadeEvent
```

---

## 🏗️ Paso 2: Configurar LightManager en la Escena

1. Crea un GameObject vacío: `LightManager`
2. Añade el componente: `LightManager`
3. Configura en el inspector:

```yaml
LightManager
  Events:
    On Light Registered: [Arrastra] → LightRegisteredEvent
    On Light Unregistered: [Arrastra] → LightUnregisteredEvent
    On Global Light Command: [Arrastra] → GlobalLightCommandEvent
  
  Settings:
    Auto Register Lights: ✓
    Max Active Lights: 15
    Use Culling: ✓
    Culling Distance: 20
  
  Global Controls:
    Global Lights Enabled: ✓
    Global Intensity Multiplier: 1
  
  Debug:
    Show Debug Info: □ (activa para ver logs)
```

---

## 💡 Paso 3: Configurar Todas las Luces

Para **CADA** luz en tu escena (antorchas, lámparas, luz global):

1. Selecciona el GameObject con `BaseLightController`
2. Configura en el inspector:

```yaml
BaseLightController
  Events:
    On Light Registered: [Arrastra] → LightRegisteredEvent
    On Light Unregistered: [Arrastra] → LightUnregisteredEvent
  
  References:
    Light2D: [Auto-asignado]
  
  Base Settings:
    Start Enabled: ✓
    Base Intensity: 1
  
  Behavior:
    Behavior: Static / Flickering / Pulsating / Random
```

### 🔥 Ejemplo: Configurar una Antorcha

```
GameObject: Torch_01
├── Light2D (Global/Freeform/Point)
└── BaseLightController
    ├── On Light Registered: → LightRegisteredEvent
    ├── On Light Unregistered: → LightUnregisteredEvent
    ├── Base Intensity: 1.0
    └── Behavior: Flickering
```

---

## 🌅 Paso 4: Configurar Ciclo Día/Noche (Opcional)

Si quieres ciclo día/noche automático:

1. Crea un GameObject vacío: `DayNightSystem`
2. Añade el componente: `DayNightCycle`
3. Configura:

```yaml
DayNightCycle
  Events:
    On Global Light Command: [Arrastra] → GlobalLightCommandEvent
  
  Time Settings:
    Enable Cycle: ✓
    Cycle Duration: 300 (segundos)
    Current Time: 0.5 (mediodía)
    Time Speed: 1.0
  
  Light References:
    Global Light: [Arrastra] → Global Light 2D en la escena
    Control Global Light: ✓
  
  Day Settings:
    Day Night Gradient: [Auto-generado]
    Intensity Curve: [Auto-generado]
    Day Color: (1, 0.96, 0.8)
    Day Intensity: 1
  
  Transition Settings:
    Dawn Color: (1, 0.7, 0.5)
    Dusk Color: (0.9, 0.5, 0.3)
  
  Night Settings:
    Night Color: (0.3, 0.4, 0.6)
    Night Intensity: 0.3
  
  Day Period Thresholds:
    Dawn Start: 0.2
    Day Start: 0.3
    Dusk Start: 0.7
    Night Start: 0.8
  
  Auto-Control Lights:
    Auto Control Artificial Lights: ✓
```

---

## 🎬 Paso 5: Configurar Screen Fade (Para Teletransportes)

1. Crea un GameObject vacío: `ScreenFadeManager`
2. Añade el componente: `ScreenFadeManager`
3. Configura:

```yaml
ScreenFadeManager
  Events:
    Screen Fade Event: [Arrastra] → ScreenFadeEvent
  
  Settings:
    Create Canvas On Awake: ✓
```

---

## 🪜 Paso 6: Configurar Objetos que Usan Fade

Para cada objeto que usa fade (Ladders, Ropes, ClimbSpawnPoints):

### Ladder

```yaml
Ladder
  Events:
    Screen Fade Event: [Arrastra] → ScreenFadeEvent
  
  Settings:
    Require Interaction Input: □
    Interaction Key: W
  
  Teleport Settings:
    Top Exit Point: [Arrastra Transform]
    Fade Duration: 0.5
```

### ClimbSpawnPoint

```yaml
ClimbSpawnPoint
  Events:
    Screen Fade Event: [Arrastra] → ScreenFadeEvent
  
  Spawn Point Settings:
    Spawn Point ID: "Bottom"
    Target Spawn Point ID: "Top"
    Fade Duration: 0.5
    Cooldown After Teleport: 0.5
```

### RopeAnchor

```yaml
RopeAnchorPassiveItem
  Events:
    Screen Fade Event: [Arrastra] → ScreenFadeEvent
  
  Rope Settings:
    Rope Length: 5
    Rope Prefab: [Arrastra]
    Fade Duration: 0.5
```

### ClimbableObject

```yaml
ClimbableWithTeleport
  Events:
    Screen Fade Event: [Arrastra] → ScreenFadeEvent
  
  Climb Settings:
    Exit Point: [Arrastra Transform]
    Fade Duration: 0.5
    Auto Climb: ✓
```

---

## ✅ Verificación

Después de configurar todo:

### 1. Verifica Eventos Asignados

- [ ] LightManager tiene los 3 eventos asignados
- [ ] Todas las luces tienen los 2 eventos asignados
- [ ] DayNightCycle tiene GlobalLightCommandEvent
- [ ] ScreenFadeManager tiene ScreenFadeEvent
- [ ] Todos los Ladders/Ropes/Climbs tienen ScreenFadeEvent

### 2. Prueba en Play Mode

1. **Presiona Play**
2. Verifica en la consola:
   ```
   [LIGHT MANAGER] Initialized with X lights
   [LIGHT MANAGER] Registered: [nombre de luz]
   ```
3. Si `Show Debug Info` está activo, verás logs de registro

### 3. Prueba el Ciclo Día/Noche

1. En Play Mode
2. Observa cómo cambia la iluminación
3. Las luces artificiales deben:
   - Encenderse al atardecer/noche
   - Apagarse durante el día

### 4. Prueba el Fade

1. Interactúa con una escalera/cuerda
2. Debe hacer fade a negro
3. Teletransportar
4. Fade desde negro

---

## 🚨 Solución de Problemas

### ❌ "NullReferenceException" en LightManager

**Problema:** Los eventos no están asignados

**Solución:**
1. Selecciona LightManager
2. Asigna los 3 eventos en el inspector
3. Guarda la escena

### ❌ "No Light2D component found"

**Problema:** La luz no tiene el componente Light2D

**Solución:**
1. Selecciona el GameObject de la luz
2. Add Component → Rendering → Light 2D
3. Configura el tipo (Global/Point/Freeform)

### ❌ Las luces no se registran

**Problema:** Los eventos no coinciden

**Solución:**
1. Verifica que LightManager y BaseLightController usan LOS MISMOS eventos
2. Deben apuntar al mismo ScriptableObject asset

### ❌ El fade no funciona

**Problema:** ScreenFadeEvent no asignado

**Solución:**
1. Selecciona el objeto (Ladder/Rope/etc)
2. Busca la sección Events
3. Arrastra ScreenFadeEvent
4. Guarda la escena

### ❌ Las luces no se encienden/apagan con día/noche

**Problema:** GlobalLightCommandEvent no coincide

**Solución:**
1. Verifica que DayNightCycle y LightManager usan el MISMO GlobalLightCommandEvent
2. Activa `Auto Control Artificial Lights` en DayNightCycle

---

## 📝 Plantilla de Configuración Rápida

Copia esto y reemplaza `[...]`:

```
Escena: [Nombre de tu escena]

1. ScriptableObjects creados:
   ✓ LightRegisteredEvent
   ✓ LightUnregisteredEvent
   ✓ GlobalLightCommandEvent
   ✓ ScreenFadeEvent

2. LightManager configurado:
   ✓ Eventos asignados
   ✓ Settings configurados

3. Luces configuradas: [Número]
   ✓ Global Light
   ✓ [Lista tus luces]

4. DayNightCycle (si aplica):
   ✓ Eventos asignados
   ✓ Global Light asignado
   ✓ Thresholds configurados

5. ScreenFadeManager:
   ✓ ScreenFadeEvent asignado

6. Objetos con Fade: [Número]
   ✓ [Lista tus objetos]
```

---

## 🎉 ¡Listo!

Tu escena ahora usa el sistema sin singletons. Todo está desacoplado, testeable y fácil de mantener.

### Ventajas que Obtuviste:

- ✅ Sin dependencias ocultas
- ✅ Fácil de testear cada escena
- ✅ Inspector muestra todas las conexiones
- ✅ No más problemas con DontDestroyOnLoad
- ✅ Guardado/carga simple
- ✅ Múltiples escenas funcionan bien

---

## 📚 Siguiente Lectura

- Guía completa de migración: `/Assets/Scripts/Architecture/MIGRACION_SINGLETONS_A_EVENTOS.md`
- Alternativas a Singletons: `/Assets/Scripts/Architecture/ALTERNATIVAS_A_SINGLETONS.md`
