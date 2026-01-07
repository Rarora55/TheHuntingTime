# ✅ RopeSystem - Fix InteractableObject

## 🔍 Problema Encontrado

Reportaste: **"No encuentro el cs InteractableObject"**

---

## ✅ Solución

**`InteractableObject` SÍ existe** en tu proyecto:

📍 `/Assets/Scripts/Interaction/InteractableObject.cs`

---

## 🧩 ¿Por Qué No Lo Ves Como Componente?

`InteractableObject` es una **clase base abstracta** (`abstract class`).

```csharp
public abstract class InteractableObject : MonoBehaviour, IInteractable
```

**Esto significa**:
- ❌ **NO puedes** agregar `InteractableObject` directamente como componente
- ✅ **SÍ puedes** usar clases que heredan de él como `RopeAnchorInteraction`

---

## 🎯 Jerarquía de Clases

```
IInteractable (Interface)
    ↓
InteractableObject (Abstract Base Class) ← NO se agrega directamente
    ↓
RopeAnchorInteraction ← SÍ se agrega como componente
```

---

## 🔧 Tu RopeAnchor Actual

### Estado Actual:

```
RopeAnchor
├── Transform (28.7, 7.14, 0)
├── CircleCollider2D ✅
│   ├── Is Trigger: true
│   └── Radius: 0.5
├── RopeAnchorPoint ✅
│   ├── Rope Spawn Point: /RopeAnchor/RopeSpawnPoint ✅
│   ├── Rope Length: 5.0
│   ├── Rope Prefab: null ⚠️ FALTA ASIGNAR
│   ├── Anchor Visual: null (opcional)
│   ├── Available Color: Verde (0, 1, 0)
│   └── Used Color: Gris (0.5, 0.5, 0.5)
└── RopeAnchorInteraction ✅
    ├── Interaction Prompt: "Press E to interact"
    └── Is Interactable: true
```

---

## ⚠️ Lo Que Falta

### 1. Crear el Prefab RopeClimbable

El `ropePrefab` está en `null`. Necesitas crear el prefab.

---

## 🔧 Paso a Paso - Crear RopeClimbable Prefab

### PASO 1: Crear GameObject Base

1. Hierarchy → Click derecho → **Create Empty**
2. Nombre: `RopeClimbable`
3. Position: (0, 0, 0)

---

### PASO 2: Configurar Tag

1. Selecciona `RopeClimbable`
2. En Inspector → **Tag** → Selecciona `FrontLadder`

⚠️ **CRÍTICO**: El tag DEBE ser `FrontLadder` para que el player pueda trepar.

---

### PASO 3: Agregar BoxCollider2D

1. Con `RopeClimbable` seleccionado
2. Add Component → **Box Collider 2D**

**Configuración**:
```
Box Collider 2D:
├── Is Trigger: ✅ true
├── Size: (0.5, 5.0)
└── Offset: (0, -2.5)
```

**Explicación**:
- `Size.y = 5.0` → Altura de la cuerda (mismo que `ropeLength`)
- `Offset.y = -2.5` → Centro del collider (la mitad hacia abajo)

---

### PASO 4: Agregar RopeClimbable Script

1. Con `RopeClimbable` seleccionado
2. Add Component → **Rope Climbable**

**Configuración**:
```
Rope Climbable:
├── Rope Length: 5.0
├── Require Interaction Input: false
├── Rope Segments: 10
├── Rope Color: (0.6, 0.4, 0.2) ← Café/marrón
└── Rope Width: 0.1
```

---

### PASO 5: Guardar Como Prefab

1. Arrastra `RopeClimbable` desde Hierarchy a Project
2. Carpeta: `/Assets/Prefabs/Environment/`
3. Nombre: `RopeClimbable.prefab`

**Si no existe la carpeta**:
- Project → Assets → Click derecho → Create → Folder → `Prefabs`
- Dentro de Prefabs → Create → Folder → `Environment`

4. **Elimina** el GameObject `RopeClimbable` de la Hierarchy (ya está como prefab)

---

### PASO 6: Asignar Prefab al RopeAnchor

1. En Hierarchy, selecciona `RopeAnchor`
2. En Inspector → Componente `RopeAnchorPoint`
3. En el campo **Rope Prefab**:
   - Arrastra `RopeClimbable.prefab` desde Project

---

## ✅ Verificación Final

### Checklist RopeAnchor:

```
RopeAnchor
├── RopeSpawnPoint (hijo) ✅
│   └── Position: Ligeramente debajo del anchor
│
├── CircleCollider2D ✅
│   └── Is Trigger: true
│
├── RopeAnchorPoint ✅
│   ├── Rope Spawn Point: /RopeAnchor/RopeSpawnPoint ✅
│   ├── Rope Length: 5.0 ✅
│   └── Rope Prefab: RopeClimbable.prefab ✅
│
└── RopeAnchorInteraction ✅
    └── (Hereda de InteractableObject)
```

---

### Checklist RopeClimbable.prefab:

- [ ] Tag = `FrontLadder`
- [ ] BoxCollider2D → isTrigger = true
- [ ] BoxCollider2D → Size = (0.5, 5.0)
- [ ] BoxCollider2D → Offset = (0, -2.5)
- [ ] RopeClimbable script agregado
- [ ] Rope Length = 5.0

---

## 🎮 Testing

### Test 1: Verificar Setup

1. Selecciona `RopeAnchor` en Hierarchy
2. Verifica en Inspector:
   - ✅ Rope Prefab asignado
   - ✅ Rope Spawn Point asignado
3. Verifica que el prefab existe en Project

---

### Test 2: Runtime Test

1. **Equipa RopeItem** en Secondary Weapon Slot
2. Inicia Play Mode
3. Acércate al RopeAnchor
4. Deberías ver: `"Press E to interact"` o tu prompt personalizado
5. Presiona [E]
6. **Resultado esperado**: La cuerda se despliega desde el RopeSpawnPoint

---

### Test 3: Trepar la Cuerda

1. Con la cuerda desplegada
2. Toca la cuerda (entra en el collider)
3. Presiona [W] o [S]
4. **Resultado esperado**: Player trepa/desciende

---

## 🐛 Errores y Soluciones

### ❌ "InteractableObject not found"

**Explicación**: 
- `InteractableObject` es una clase abstracta
- NO se agrega como componente directamente
- Se usa como base para otras clases

**Solución**:
- ✅ Usa `RopeAnchorInteraction` (ya lo tienes)
- ❌ No busques agregar `InteractableObject` manualmente

---

### ❌ "Rope Prefab is null"

**Causa**: No asignaste el prefab en `RopeAnchorPoint`.

**Solución**:
1. Crea el prefab `RopeClimbable` (pasos arriba)
2. Asígnalo en el campo `Rope Prefab`

---

### ❌ Player no trepa la cuerda

**Causas posibles**:
1. Tag del prefab no es `FrontLadder`
2. BoxCollider2D no es trigger
3. Player no tiene `LadderClimbState`

**Solución**:
1. Verifica Tag en el prefab
2. `isTrigger = true` en BoxCollider2D
3. Verifica que Player tiene el estado

---

### ❌ "No rope equipped in secondary slot"

**Causa**: RopeItem no está equipado.

**Solución**:
1. Abre inventario
2. Equipa RopeItem en Secondary Weapon Slot

---

## 📊 Estructura Final Correcta

```
SCENE:
└── RopeAnchor
    ├── RopeSpawnPoint (Transform vacío)
    ├── CircleCollider2D (trigger)
    ├── RopeAnchorPoint (con prefab asignado)
    └── RopeAnchorInteraction (hereda de InteractableObject)

PROJECT:
└── /Assets/Prefabs/Environment/RopeClimbable.prefab
    ├── Tag: FrontLadder
    ├── BoxCollider2D (trigger, size: 0.5x5.0)
    └── RopeClimbable script

PLAYER:
└── WeaponInventoryManager
    └── Secondary Slot → RopeItem (Tool/Rope)
```

---

## 📝 Resumen

### ¿Por qué no encontrabas InteractableObject?

Porque es una **clase abstracta base** que no se agrega como componente.

### ¿Qué componente necesitas?

`RopeAnchorInteraction` - que **ya tienes** en tu RopeAnchor ✅

### ¿Qué te falta?

1. Crear el prefab `RopeClimbable` ⚠️
2. Asignarlo en el campo `Rope Prefab` ⚠️

---

¡Una vez que crees el prefab y lo asignes, el sistema funcionará! 🪢✨
