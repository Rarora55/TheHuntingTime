# 💡 Lighting System - Scripts Documentation

Sistema modular de iluminación 2D para Unity URP.

## 📁 Estructura

```
/Lighting
├── BaseLightController.cs        # Script base para todas las luces
├── TorchLight.cs                  # Antorchas con efectos
├── InteractiveLightSwitch.cs     # Interruptores de luz
├── LightZoneController.cs        # Control de zonas de luz
├── LightEffects.cs               # Efectos especiales (fade, distancia)
├── DarkZoneTrigger.cs            # Zonas oscuras (reduce luz global)
└── /Examples
    └── LightingExamples.cs       # Ejemplos de uso
```

## 🔧 Scripts

### BaseLightController
**Propósito:** Script base para todas las luces con comportamientos configurables.

**Comportamientos:**
- `Static`: Luz constante
- `Flickering`: Parpadeo usando Perlin Noise
- `Pulsating`: Pulsación sinusoidal
- `Random`: Cambios aleatorios

**API Pública:**
```csharp
void TurnOn()                    // Enciende la luz
void TurnOff()                   // Apaga la luz
void SetIntensity(float value)   // Cambia intensidad
void SetColor(Color color)       // Cambia color
```

**Uso:**
```csharp
var lightController = GetComponent<BaseLightController>();
lightController.TurnOn();
lightController.SetIntensity(1.5f);
```

---

### TorchLight
**Hereda de:** `BaseLightController`

**Propósito:** Antorchas con auto-flickering y control de partículas/audio.

**Features:**
- Auto-flickering aleatorio
- Control de ParticleSystem (fuego)
- Control de AudioSource (sonido)

**Uso:**
```csharp
// El script se auto-configura en Awake
// Solo necesitas asignar partículas y audio en el Inspector
```

---

### InteractiveLightSwitch
**Implementa:** `IInteractable`

**Propósito:** Interruptor que controla una o más luces.

**Features:**
- Control de múltiples luces
- Feedback visual (sprites on/off)
- Sonido de switch
- Compatible con sistema de interacción

**API Pública:**
```csharp
void Interact(GameObject interactor)  // Toggle luces
void SetLightsState(bool turnOn)     // Estado forzado
```

**Uso:**
```csharp
// Asignar luces en Inspector:
// - Controlled Lights: array de BaseLightController
// - O activar "Find Lights In Children"

// Interactuar:
lightSwitch.Interact(playerGameObject);
```

---

### LightZoneController
**Propósito:** Controla grupos de luces como una zona.

**Features:**
- Control de múltiples luces
- Activación por trigger del jugador
- Modificadores de intensidad/color por zona

**API Pública:**
```csharp
void ActivateZone()                 // Activa todas las luces
void DeactivateZone()               // Desactiva todas las luces
void ToggleZone()                   // Toggle estado
void SetZoneIntensity(float mult)   // Multiplica intensidad
void SetZoneColor(Color color)      // Aplica tinte de color
```

**Uso:**
```csharp
var zone = GetComponent<LightZoneController>();
zone.ActivateZone();
zone.SetZoneIntensity(0.5f); // 50% intensidad
```

---

### LightEffects
**Propósito:** Efectos especiales de luz.

**Features:**
- Fade in/out suave
- Intensidad basada en distancia al jugador
- Transiciones animadas

**API Pública:**
```csharp
void FadeIn(float duration)              // Fade in
void FadeOut(float duration)             // Fade out
void FadeTo(float intensity, float dur)  // Fade a valor específico
void ResetToOriginal()                   // Restaura intensidad original
```

**Uso:**
```csharp
var effects = GetComponent<LightEffects>();
effects.FadeIn(2f);  // Fade in en 2 segundos

// O configurar fade por distancia en Inspector:
// - Fade By Distance: true
// - Max Distance: 10
```

---

### DarkZoneTrigger
**Propósito:** Reduce la luz global cuando el jugador entra.

**Features:**
- Transición suave de intensidad
- Control automático de luz global
- Detección por trigger

**Uso:**
```csharp
// Configurar en Inspector:
// - Global Light: referencia a Light2D global
// - Normal Intensity: 1.0
// - Dark Zone Intensity: 0.1
// - Transition Speed: 2.0

// El script funciona automáticamente con triggers
```

---

## 🎨 Ejemplos de Uso

### Crear Antorcha Simple
```csharp
GameObject torch = new GameObject("Torch");
var light2D = torch.AddComponent<Light2D>();
light2D.lightType = Light2D.LightType.Point;
light2D.intensity = 1.3f;
light2D.color = new Color(1f, 0.7f, 0.4f); // Naranja

var torchLight = torch.AddComponent<TorchLight>();
// Auto-configura flickering
```

### Crear Zona de Luz
```csharp
GameObject zone = new GameObject("LightZone");
var controller = zone.AddComponent<LightZoneController>();

// Añadir luces como hijos y activar auto-find
// O asignar manualmente en el array
```

### Fade In al Entrar a Habitación
```csharp
public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private LightEffects[] roomLights;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var light in roomLights)
            {
                light.FadeIn(2f);
            }
        }
    }
}
```

### Switch Interactivo
```csharp
// En el Inspector del GameObject:
// 1. Añadir InteractiveLightSwitch
// 2. Asignar luces a controlar
// 3. Añadir BoxCollider2D (Is Trigger: true)
// 4. Layer: Interactable

// El Player interactúa automáticamente
```

---

## 🎯 Configuraciones Recomendadas

### Antorcha (Flickering)
```
Behavior: Flickering
Flicker Speed: 5
Flicker Amount: 0.2
Base Intensity: 1.3
Color: #FFB366
```

### Vela (Pulsating)
```
Behavior: Pulsating
Flicker Speed: 2
Flicker Amount: 0.15
Base Intensity: 0.8
Color: #FFD700
```

### Lámpara (Static)
```
Behavior: Static
Base Intensity: 1.0
Color: #FFFFCC
```

---

## 🔍 Namespace

Todos los scripts usan:
```csharp
namespace TheHunt.Lighting
```

Excepción:
```csharp
namespace TheHunt.Lighting.Examples  // LightingExamples.cs
```

---

## 📦 Dependencias

- `UnityEngine`
- `UnityEngine.Rendering.Universal` (URP Light2D)
- `TheHunt.Interaction` (solo InteractiveLightSwitch)

---

## 🚀 Ver También

- [💡 Sistema de Iluminación - Guía Completa] - Documentación completa
- [🚀 Quick Start - Sistema de Luces] - Guía de inicio rápido
- [Unity URP 2D Lights](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/Lights-2D-intro.html)
