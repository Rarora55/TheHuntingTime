# 🎯 Sistema de Respawn - Guía Completa

## 📋 Quick Start

### ✅ RESPUESTAS A TUS PREGUNTAS:

**1. ¿Cómo añado más RespawnPoints?**

Sí, simplemente duplica o instancia el prefab múltiples veces:

```
OPCIÓN A (Duplicar en Hierarchy):
1. Selecciona RespawnPoint_Example en Hierarchy
2. Ctrl+D para duplicar
3. Renombra: "RespawnPoint_Checkpoint1"
4. Mueve a nueva posición
5. En Inspector → Cambiar Respawn ID: "Checkpoint_01"

OPCIÓN B (Drag & Drop desde Project):
1. Drag /Assets/Prefabs/Environment/RespawnPoint.prefab
2. Drop en Scene View
3. Posicionar
4. Cambiar Respawn ID en Inspector
```

**2. ¿Por qué no funcionan los diálogos?**

El problema estaba en que buscaba el DialogService en el Player en vez de en la escena.

✅ **YA ARREGLADO** - Ahora busca automáticamente en la escena.

---

## 🚀 Usar el Prefab

1. **Drag & Drop:** `/Assets/Prefabs/Environment/RespawnPoint.prefab` en tu escena
2. **Configurar Inspector:**
   - Respawn ID: ID único (ej: "Checkpoint_City", "Respawn_Boss")
   - Require Confirmation: ✓ (muestra diálogo "¿Quieres bajar?")
   - Dialog Title: "Punto de Descenso"
   - Dialog Message: "¿Quieres bajar a este punto?"
3. **¡Listo!** El punto funciona con diálogos YES/NO automáticamente

**IMPORTANTE:** Cada RespawnPoint en escena debe tener un Respawn ID ÚNICO.

---

## 🔄 Crear Múltiples Puntos

### Método 1: Duplicar en Escena

```
Hierarchy:
├─ RespawnPoint_Example
├─ RespawnPoint_City (duplicado, ID: "City_01")
├─ RespawnPoint_Boss (duplicado, ID: "Boss_Entrance")
└─ RespawnPoint_Secret (duplicado, ID: "Secret_Area")

PASOS:
1. Selecciona RespawnPoint_Example
2. Ctrl+D → Duplicar
3. Renombrar GameObject
4. Mover a nueva posición
5. Inspector → Cambiar "Respawn ID" a algo único
```

### Método 2: Drag & Drop Prefab

```
1. Abre /Assets/Prefabs/Environment/
2. Drag RespawnPoint.prefab al Scene View
3. Suelta en la posición deseada
4. Inspector → Configurar Respawn ID único
5. Repetir para cada punto que necesites
```

---

## 🏗️ Arquitectura

```
COMPONENTES:
├─ RespawnPoint.cs            → Trigger individual (coloca en escena)
├─ RespawnManager.cs          → Singleton global (auto-crea)
├─ PlayerRespawnController.cs → En el Player (auto-respawn on death)
└─ RespawnData.cs             → ScriptableObject config (opcional)

ASSETS:
├─ RespawnPoint.prefab        → Prefab listo para usar
└─ DefaultRespawnData.asset   → Configuración por defecto
```

---

## ⚙️ Configuración de RespawnPoint

```csharp
[Respawn Settings]
Respawn ID: "Respawn_01"              // ID único
Auto Activate On Enter: ✓             // Auto-trigger al entrar
Require Confirmation: ✓               // Mostrar diálogo YES/NO

[Dialog Settings]
Dialog Title: "Punto de Descenso"     // Título del popup
Dialog Message: "¿Quieres bajar?"     // Mensaje del popup

[Visual Feedback]
Gizmo Color: Green                    // Color en Scene View
Gizmo Radius: 0.5                     // Tamaño del gizmo
Show Label: ✓                         // Mostrar label

[Advanced]
One Time Use: ✗                       // Solo una vez
Save To Global Manager: ✓             // Guardar en manager
```

---

## 💬 Integración con Diálogos

El sistema usa **DialogService** automáticamente:

**REQUERIDO:** El Player debe tener `DialogService` con:
- Use Prefab: ✓
- Confirmation Dialog Prefab: `ConfirmationDialoguePanel1.prefab`

**Flujo automático:**
1. Player entra al trigger
2. Aparece diálogo: "¿Quieres bajar?" [YES] [NO]
3. YES → Teleport al punto
4. NO → Cancela

---

## 🎯 Casos de Uso

### 1. Punto de Descenso (Con Confirmación)

```
USE CASE: Bajar a una zona peligrosa

CONFIG:
├─ Require Confirmation: ✓
├─ Dialog: "¿Bajar al foso?"
└─ Auto Activate: ✓

RESULTADO: Pregunta antes de teleportar
```

### 2. Checkpoint Automático (Sin Confirmación)

```
USE CASE: Checkpoint al pasar

CONFIG:
├─ Require Confirmation: ✗
├─ One Time Use: ✓
└─ Auto Activate: ✓

RESULTADO: Guarda automáticamente (sin preguntar)
```

### 3. Respawn al Morir

```
SETUP:
1. Añadir PlayerRespawnController al Player
2. Configurar DefaultRespawnData.asset:
   ├─ Auto Respawn On Death: ✓
   ├─ Respawn Delay: 2s
   └─ Reset Health: ✓

RESULTADO: Al morir → espera 2s → respawn en último checkpoint
```

---

## 📚 API Reference

### RespawnPoint

```csharp
// Propiedades
string RespawnID { get; }
bool HasBeenUsed { get; }
Vector3 Position { get; }

// Métodos
void ManualActivate()     // Activar manualmente
void ResetUsage()         // Resetear "one time use"
```

### RespawnManager (Singleton)

```csharp
// Acceso
RespawnManager.Instance

// Métodos
void SetCurrentRespawn(string id, Vector3 pos)
void RespawnPlayer(Player player)
bool HasRespawnPoint()

// Propiedades
string CurrentRespawnID { get; }
Vector3 CurrentRespawnPosition { get; }
```

### PlayerRespawnController

```csharp
// En el Player

void RespawnPlayer()                          // Respawn manual
void SetRespawnPoint(string id, Vector3 pos)  // Setear checkpoint

// Debug: Presiona 'R' en Play Mode para respawn manual
```

---

## 🧪 Testing

### Test: Diálogo de Confirmación

```
1. Coloca RespawnPoint prefab
2. Require Confirmation: ✓
3. Enter Play Mode
4. Camina al trigger
5. ✅ Aparece: "¿Quieres bajar?" [YES] [NO]
```

### Test: Auto-Respawn on Death

```
1. Añade PlayerRespawnController al Player
2. Activa un checkpoint (camina sobre él)
3. Simula muerte (HealthDebugger: Y para daño)
4. ✅ Espera 2s → Respawn en checkpoint
```

---

## 🐛 Troubleshooting

### "Los diálogos no aparecen / Teleporta automáticamente"

**Causa 1: No hay DialogService en escena**

Solución:
```
Hierarchy → Busca "Player"
└─ Verifica que Player tenga componente: DialogService
   └─ Si NO existe → Add Component → Dialog Service
```

**Causa 2: DialogService no está en modo PREFAB**

Solución:
```
Player → Inspector → Dialog Service
├─ Use Prefab: ✓ (debe estar activado)
└─ Confirmation Dialog Prefab: ConfirmationDialoguePanel1
```

**Causa 3: Require Confirmation está desactivado**

Solución:
```
RespawnPoint → Inspector
└─ Require Confirmation: ✓ (activar)
```

**Causa 4: DialogService está en objeto desactivado**

Solución:
```
Verifica que el GameObject con DialogService esté activo en Hierarchy
```

---

### "Múltiples puntos tienen el mismo ID"

**Problema:** Dos RespawnPoints con el mismo Respawn ID

**Solución:**
```
Selecciona cada RespawnPoint en Hierarchy
└─ Inspector → Respawn ID → Cambiar a ID único

BUENOS EJEMPLOS:
├─ "Checkpoint_Level1_Start"
├─ "Respawn_BossRoom"
├─ "Safe_City_Plaza"
└─ "Secret_Cave_01"

MALOS EJEMPLOS (duplicados):
├─ "Respawn_01"  ❌
├─ "Respawn_01"  ❌ (duplicado!)
└─ "Respawn_01"  ❌ (duplicado!)
```

---

### "No DialogService found"

**Problema:** Player no tiene DialogService

**Solución:**
```
Player Inspector
└─ Verificar componente: Dialog Service
   ├─ Use Prefab: ✓
   └─ Confirmation Dialog Prefab: Asignado
```

---

## ✅ Archivos Creados

```
SCRIPTS:
├─ /Assets/Scripts/Respawn/RespawnPoint.cs
├─ /Assets/Scripts/Respawn/RespawnManager.cs
├─ /Assets/Scripts/Respawn/PlayerRespawnController.cs
└─ /Assets/Scripts/Respawn/RespawnData.cs

ASSETS:
├─ /Assets/Scripts/Respawn/DefaultRespawnData.asset
└─ /Assets/Prefabs/Environment/RespawnPoint.prefab
```

---

## 🎯 Respuesta a tu Pregunta Original

**"¿Prefabs o ScriptableObject?"**

✅ **AMBOS (Sistema Híbrido):**

- **Prefabs** → Para colocar puntos en escena (fácil drag & drop)
- **ScriptableObject** → Para config global (opcional)
- **RespawnManager** → Singleton para gestión centralizada

**"¿Cuadros de diálogo YES/NO?"**

✅ **YA IMPLEMENTADO:**

- Usa tu DialogService existente
- Configuración: `requireConfirmation = true`
- Automáticamente muestra: "¿Quieres bajar?" [YES] [NO]

---

**¡Sistema completo y listo para usar! 🎮**
